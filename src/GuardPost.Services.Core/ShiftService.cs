using Microsoft.EntityFrameworkCore;

using GuardPost.Common;
using GuardPost.Common.Extensions;
using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Data.Models;
using GuardPost.Services.Core.Contracts;
using GuardPost.Services.Core.Mapping;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Shifts;

using static GuardPost.Common.EntityValidationConstants.Shift;

namespace GuardPost.Services.Core;

public class ShiftService : IShiftService
{
    private readonly GuardPostDbContext dbContext;
    private readonly IDateTimeProvider clock;

    public ShiftService(GuardPostDbContext dbContext, IDateTimeProvider clock)
    {
        this.dbContext = dbContext;
        this.clock = clock;
    }

    public async Task<ShiftWeekViewModel> GetWeekAsync(DateTime? weekStart, int? siteId, int? guardId)
    {
        var start = (weekStart ?? clock.Today).StartOfWeek();
        var end = start.AddDays(7);

        IQueryable<Shift> query = dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.StartTime < end && sh.EndTime > start);

        if (siteId.HasValue)
        {
            query = query.Where(sh => sh.SiteId == siteId.Value);
        }

        if (guardId.HasValue)
        {
            query = query.Where(sh => sh.GuardId == guardId.Value);
        }

        var shifts = await query
            .OrderBy(sh => sh.StartTime)
            .ThenBy(sh => sh.Site.Name)
            .Select(Projections.ShiftToListItem)
            .ToListAsync();

        var days = Enumerable.Range(0, 7)
            .Select(offset => new ShiftDayViewModel { Date = start.AddDays(offset) })
            .ToList();

        foreach (var shift in shifts)
        {            
            var day = shift.StartTime.Date < start ? start : shift.StartTime.Date;
            days[(day - start).Days].Shifts.Add(shift);
        }

        return new ShiftWeekViewModel
        {
            WeekStart = start,
            Now = clock.Now,
            SiteId = siteId,
            GuardId = guardId,
            Days = days,
        };
    }

    public async Task<ShiftListItemViewModel?> GetDetailsAsync(int id)
        => await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.Id == id)
            .Select(Projections.ShiftToListItem)
            .FirstOrDefaultAsync();

    public async Task<ShiftFormViewModel?> GetForEditAsync(int id)
        => await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.Id == id)
            .Select(sh => new ShiftFormViewModel
            {
                Id = sh.Id,
                SiteId = sh.SiteId,
                GuardId = sh.GuardId,
                StartTime = sh.StartTime,
                EndTime = sh.EndTime,
                Notes = sh.Notes,
            })
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyList<ValidationError>> ValidateAsync(ShiftFormViewModel model)
    {
        var errors = new List<ValidationError>();

        if (model.SiteId is null || model.GuardId is null || model.StartTime is null || model.EndTime is null)
        {            
            return errors;
        }

        int siteId = model.SiteId.Value;
        int guardId = model.GuardId.Value;
        var start = model.StartTime.Value;
        var end = model.EndTime.Value;
        var original = model.Id == 0
            ? null
            : await dbContext.Shifts
                .AsNoTracking()
                .Where(sh => sh.Id == model.Id)
                .Select(sh => new { sh.SiteId, sh.GuardId })
                .FirstOrDefaultAsync();

        bool keepsOriginalSite = original?.SiteId == siteId;
        bool keepsOriginalGuard = original?.GuardId == guardId;

        bool siteAvailable = await dbContext.Sites
            .AnyAsync(s => s.Id == siteId && (s.IsActive || keepsOriginalSite));
        if (!siteAvailable)
        {
            errors.Add(new ValidationError(nameof(model.SiteId), ValidationMessages.SiteNotAvailable));
        }

        var guard = await dbContext.Guards
            .AsNoTracking()
            .Where(g => g.Id == guardId)
            .Select(g => new { g.IsActive, FullName = g.FirstName + " " + g.LastName })
            .FirstOrDefaultAsync();

        bool guardAvailable = guard != null && (guard.IsActive || keepsOriginalGuard);
        if (!guardAvailable)
        {
            errors.Add(new ValidationError(nameof(model.GuardId), ValidationMessages.GuardNotAvailable));
        }

        double hours = (end - start).TotalHours;
        if (end > start && (hours < MinDurationHours || hours > MaxDurationHours))
        {
            errors.Add(new ValidationError(
                nameof(model.EndTime),
                string.Format(ValidationMessages.ShiftDurationOutOfRange, MinDurationHours, MaxDurationHours)));
        }

        if (guard is null || end <= start)
        {
            return errors;
        }

        var nearby = await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.GuardId == guardId
                         && sh.Id != model.Id
                         && sh.StartTime < end.AddHours(MinRestHours)
                         && sh.EndTime > start.AddHours(-MinRestHours))
            .OrderBy(sh => sh.StartTime)
            .Select(sh => new { sh.StartTime, sh.EndTime, SiteName = sh.Site.Name })
            .ToListAsync();

        var overlapping = nearby.FirstOrDefault(sh => sh.StartTime < end && sh.EndTime > start);
        if (overlapping != null)
        {
            errors.Add(new ValidationError(
                string.Empty,
                string.Format(
                    ValidationMessages.ShiftOverlaps,
                    guard.FullName,
                    overlapping.SiteName,
                    FormatPeriod(overlapping.StartTime, overlapping.EndTime))));
        }
        else if (nearby.Count > 0)
        {
            var tooClose = nearby[0];
            errors.Add(new ValidationError(
                string.Empty,
                string.Format(
                    ValidationMessages.ShiftRestViolation,
                    guard.FullName,
                    MinRestHours,
                    tooClose.SiteName,
                    FormatPeriod(tooClose.StartTime, tooClose.EndTime))));
        }

        return errors;
    }

    public async Task<int> CreateAsync(ShiftFormViewModel model)
    {
        var shift = new Shift();
        MapToEntity(model, shift);

        await dbContext.Shifts.AddAsync(shift);
        await dbContext.SaveChangesAsync();

        return shift.Id;
    }

    public async Task<bool> UpdateAsync(ShiftFormViewModel model)
    {
        var shift = await dbContext.Shifts.FirstOrDefaultAsync(sh => sh.Id == model.Id);
        if (shift is null)
        {
            return false;
        }

        MapToEntity(model, shift);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var shift = await dbContext.Shifts.FirstOrDefaultAsync(sh => sh.Id == id);
        if (shift is null)
        {
            return false;
        }

        dbContext.Shifts.Remove(shift);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static string FormatPeriod(DateTime start, DateTime end)
        => $"{start.ToString(ApplicationConstants.DateTimeFormat)} – {end.ToString(ApplicationConstants.DateTimeFormat)}";

    private static void MapToEntity(ShiftFormViewModel model, Shift shift)
    {
        shift.SiteId = model.SiteId!.Value;
        shift.GuardId = model.GuardId!.Value;
        shift.StartTime = TrimSeconds(model.StartTime!.Value);
        shift.EndTime = TrimSeconds(model.EndTime!.Value);
        shift.Notes = string.IsNullOrWhiteSpace(model.Notes) ? null : model.Notes.Trim();
    }

    private static DateTime TrimSeconds(DateTime value)
        => new(value.Year, value.Month, value.Day, value.Hour, value.Minute, 0, value.Kind);
}

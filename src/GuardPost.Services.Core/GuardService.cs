using Microsoft.EntityFrameworkCore;

using GuardPost.Common.Extensions;
using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Data.Models;
using GuardPost.Data.Models.Enums;
using GuardPost.Services.Core.Contracts;
using GuardPost.Services.Core.Mapping;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Guards;

namespace GuardPost.Services.Core;

public class GuardService : IGuardService
{
    private const int UpcomingShiftsOnDetails = 10;
    private const int IncidentsOnDetails = 5;

    private readonly GuardPostDbContext dbContext;
    private readonly IDateTimeProvider clock;

    public GuardService(GuardPostDbContext dbContext, IDateTimeProvider clock)
    {
        this.dbContext = dbContext;
        this.clock = clock;
    }

    public async Task<GuardIndexViewModel> GetIndexAsync(string? searchTerm, GuardRank? rank, bool showInactive, int page, int pageSize)
    {
        IQueryable<Guard> query = dbContext.Guards.AsNoTracking();

        if (!showInactive)
        {
            query = query.Where(g => g.IsActive);
        }

        if (rank.HasValue)
        {
            query = query.Where(g => g.Rank == rank.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim();
            query = query.Where(g => g.FirstName.Contains(term)
                                     || g.LastName.Contains(term)
                                     || g.LicenseNumber.Contains(term)
                                     || g.PhoneNumber.Contains(term));
        }

        int totalCount = await query.CountAsync();
        page = Paging.Normalize(page, totalCount, pageSize);

        var items = await query
            .OrderByDescending(g => g.IsActive)
            .ThenBy(g => g.FirstName)
            .ThenBy(g => g.LastName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(g => new GuardListItemViewModel
            {
                Id = g.Id,
                FullName = g.FirstName + " " + g.LastName,
                LicenseNumber = g.LicenseNumber,
                PhoneNumber = g.PhoneNumber,
                Rank = g.Rank,
                IsActive = g.IsActive,
            })
            .ToListAsync();

        var hours = await GetHoursThisMonthAsync(items.Select(i => i.Id).ToList());
        foreach (var item in items)
        {
            item.HoursThisMonth = hours.GetValueOrDefault(item.Id);
        }

        return new GuardIndexViewModel
        {
            SearchTerm = searchTerm,
            Rank = rank,
            ShowInactive = showInactive,
            Result = new PagedResult<GuardListItemViewModel>(items, totalCount, page, pageSize),
        };
    }

    public async Task<GuardDetailsViewModel?> GetDetailsAsync(int id)
    {
        var now = clock.Now;

        var model = await dbContext.Guards
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GuardDetailsViewModel
            {
                Id = g.Id,
                FirstName = g.FirstName,
                LastName = g.LastName,
                LicenseNumber = g.LicenseNumber,
                PhoneNumber = g.PhoneNumber,
                Email = g.Email,
                HireDate = g.HireDate,
                Rank = g.Rank,
                IsActive = g.IsActive,
            })
            .FirstOrDefaultAsync();

        if (model is null)
        {
            return null;
        }

        var monthStart = now.StartOfMonth();
        var monthEnd = monthStart.AddMonths(1);

        var monthShifts = await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.GuardId == id && sh.StartTime < monthEnd && sh.EndTime > monthStart)
            .Select(sh => new { sh.StartTime, sh.EndTime })
            .ToListAsync();

        model.ShiftsThisMonth = monthShifts.Count;
        model.HoursThisMonth = monthShifts.Sum(sh => DateTimeExtensions.OverlapHours(sh.StartTime, sh.EndTime, monthStart, monthEnd));

        model.UpcomingShifts = await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.GuardId == id && sh.EndTime > now)
            .OrderBy(sh => sh.StartTime)
            .Take(UpcomingShiftsOnDetails)
            .Select(Projections.ShiftToListItem)
            .ToListAsync();

        model.ReportedIncidents = await dbContext.Incidents
            .AsNoTracking()
            .Where(i => i.ReportedByGuardId == id)
            .OrderByDescending(i => i.OccurredOn)
            .Take(IncidentsOnDetails)
            .Select(Projections.IncidentToListItem)
            .ToListAsync();

        return model;
    }

    public async Task<GuardFormViewModel?> GetForEditAsync(int id)
        => await dbContext.Guards
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GuardFormViewModel
            {
                Id = g.Id,
                FirstName = g.FirstName,
                LastName = g.LastName,
                LicenseNumber = g.LicenseNumber,
                PhoneNumber = g.PhoneNumber,
                Email = g.Email,
                HireDate = g.HireDate,
                Rank = g.Rank,
                IsActive = g.IsActive,
            })
            .FirstOrDefaultAsync();

    public async Task<int> CreateAsync(GuardFormViewModel model)
    {
        var guard = new Guard();
        MapToEntity(model, guard);

        await dbContext.Guards.AddAsync(guard);
        await dbContext.SaveChangesAsync();

        return guard.Id;
    }

    public async Task<bool> UpdateAsync(GuardFormViewModel model)
    {
        var guard = await dbContext.Guards.FirstOrDefaultAsync(g => g.Id == model.Id);
        if (guard is null)
        {
            return false;
        }

        MapToEntity(model, guard);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<GuardDeleteViewModel?> GetForDeleteAsync(int id)
    {
        var now = clock.Now;

        return await dbContext.Guards
            .AsNoTracking()
            .Where(g => g.Id == id)
            .Select(g => new GuardDeleteViewModel
            {
                Id = g.Id,
                FullName = g.FirstName + " " + g.LastName,
                LicenseNumber = g.LicenseNumber,
                Rank = g.Rank,
                FutureShiftsCount = g.Shifts.Count(sh => sh.StartTime > now),
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var guard = await dbContext.Guards.FirstOrDefaultAsync(g => g.Id == id);
        if (guard is null)
        {
            return false;
        }

        var now = clock.Now;
        var futureShifts = await dbContext.Shifts
            .Where(sh => sh.GuardId == id && sh.StartTime > now)
            .ToListAsync();

        dbContext.Shifts.RemoveRange(futureShifts);

        guard.IsDeleted = true;
        guard.IsActive = false;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsLicenseNumberTakenAsync(string licenseNumber, int excludeGuardId = 0)
    {
        string normalized = licenseNumber.Trim().ToUpperInvariant();

        return await dbContext.Guards
            .IgnoreQueryFilters()
            .AnyAsync(g => g.LicenseNumber == normalized && g.Id != excludeGuardId);
    }

    public async Task<IEnumerable<DropdownItemViewModel>> GetDropdownAsync(bool onlyActive, int? includeId = null)
        => await dbContext.Guards
            .AsNoTracking()
            .Where(g => !onlyActive || g.IsActive || g.Id == includeId)
            .OrderBy(g => g.FirstName)
            .ThenBy(g => g.LastName)
            .Select(g => new DropdownItemViewModel
            {
                Id = g.Id,
                Text = g.FirstName + " " + g.LastName + " (" + g.LicenseNumber + ")",
            })
            .ToListAsync();

    private async Task<Dictionary<int, double>> GetHoursThisMonthAsync(IReadOnlyCollection<int> guardIds)
    {
        if (guardIds.Count == 0)
        {
            return new Dictionary<int, double>();
        }

        var monthStart = clock.Now.StartOfMonth();
        var monthEnd = monthStart.AddMonths(1);

        var shifts = await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => guardIds.Contains(sh.GuardId) && sh.StartTime < monthEnd && sh.EndTime > monthStart)
            .Select(sh => new { sh.GuardId, sh.StartTime, sh.EndTime })
            .ToListAsync();

        return shifts
            .GroupBy(sh => sh.GuardId)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(sh => DateTimeExtensions.OverlapHours(sh.StartTime, sh.EndTime, monthStart, monthEnd)));
    }

    private static void MapToEntity(GuardFormViewModel model, Guard guard)
    {
        guard.FirstName = model.FirstName.Trim();
        guard.LastName = model.LastName.Trim();
        guard.LicenseNumber = model.LicenseNumber.Trim().ToUpperInvariant();
        guard.PhoneNumber = model.PhoneNumber.Trim();
        guard.Email = string.IsNullOrWhiteSpace(model.Email) ? null : model.Email.Trim();
        guard.HireDate = model.HireDate!.Value.Date;
        guard.Rank = model.Rank!.Value;
        guard.IsActive = model.IsActive;
    }
}

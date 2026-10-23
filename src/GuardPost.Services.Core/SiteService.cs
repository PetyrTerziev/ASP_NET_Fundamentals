using Microsoft.EntityFrameworkCore;

using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Data.Models;
using GuardPost.Data.Models.Enums;
using GuardPost.Services.Core.Contracts;
using GuardPost.Services.Core.Mapping;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Sites;

namespace GuardPost.Services.Core;

public class SiteService : ISiteService
{
    private const int UpcomingShiftsOnDetails = 10;
    private const int IncidentsOnDetails = 5;

    private readonly GuardPostDbContext dbContext;
    private readonly IDateTimeProvider clock;

    public SiteService(GuardPostDbContext dbContext, IDateTimeProvider clock)
    {
        this.dbContext = dbContext;
        this.clock = clock;
    }

    public async Task<SiteIndexViewModel> GetIndexAsync(string? searchTerm, string? city, bool showInactive, int page, int pageSize)
    {
        var now = clock.Now;
        IQueryable<Site> query = dbContext.Sites.AsNoTracking();

        if (!showInactive)
        {
            query = query.Where(s => s.IsActive);
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(s => s.City == city);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim();
            query = query.Where(s => s.Name.Contains(term)
                                     || s.ClientName.Contains(term)
                                     || s.Address.Contains(term));
        }

        int totalCount = await query.CountAsync();
        page = Paging.Normalize(page, totalCount, pageSize);

        var items = await query
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SiteListItemViewModel
            {
                Id = s.Id,
                Name = s.Name,
                City = s.City,
                Address = s.Address,
                ClientName = s.ClientName,
                RequiredGuardsPerShift = s.RequiredGuardsPerShift,
                IsActive = s.IsActive,
                UpcomingShiftsCount = s.Shifts.Count(sh => sh.EndTime > now),
                OpenIncidentsCount = s.Incidents.Count(i => i.Status != IncidentStatus.Resolved),
            })
            .ToListAsync();

        var cities = await dbContext.Sites
            .AsNoTracking()
            .Select(s => s.City)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();

        return new SiteIndexViewModel
        {
            SearchTerm = searchTerm,
            City = city,
            ShowInactive = showInactive,
            Cities = cities,
            Result = new PagedResult<SiteListItemViewModel>(items, totalCount, page, pageSize),
        };
    }

    public async Task<SiteDetailsViewModel?> GetDetailsAsync(int id)
    {
        var now = clock.Now;

        var model = await dbContext.Sites
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SiteDetailsViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                City = s.City,
                ClientName = s.ClientName,
                ContactPhone = s.ContactPhone,
                RequiredGuardsPerShift = s.RequiredGuardsPerShift,
                Description = s.Description,
                IsActive = s.IsActive,
                CreatedOn = s.CreatedOn,
                GuardsOnDutyNow = s.Shifts.Count(sh => sh.StartTime <= now && sh.EndTime > now),
            })
            .FirstOrDefaultAsync();

        if (model is null)
        {
            return null;
        }

        model.UpcomingShifts = await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.SiteId == id && sh.EndTime > now)
            .OrderBy(sh => sh.StartTime)
            .Take(UpcomingShiftsOnDetails)
            .Select(Projections.ShiftToListItem)
            .ToListAsync();

        model.RecentIncidents = await dbContext.Incidents
            .AsNoTracking()
            .Where(i => i.SiteId == id)
            .OrderByDescending(i => i.OccurredOn)
            .Take(IncidentsOnDetails)
            .Select(Projections.IncidentToListItem)
            .ToListAsync();

        return model;
    }

    public async Task<SiteFormViewModel?> GetForEditAsync(int id)
        => await dbContext.Sites
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SiteFormViewModel
            {
                Id = s.Id,
                Name = s.Name,
                Address = s.Address,
                City = s.City,
                ClientName = s.ClientName,
                ContactPhone = s.ContactPhone,
                RequiredGuardsPerShift = s.RequiredGuardsPerShift,
                Description = s.Description,
                IsActive = s.IsActive,
            })
            .FirstOrDefaultAsync();

    public async Task<int> CreateAsync(SiteFormViewModel model)
    {
        var site = new Site { CreatedOn = clock.Now };
        MapToEntity(model, site);

        await dbContext.Sites.AddAsync(site);
        await dbContext.SaveChangesAsync();

        return site.Id;
    }

    public async Task<bool> UpdateAsync(SiteFormViewModel model)
    {
        var site = await dbContext.Sites.FirstOrDefaultAsync(s => s.Id == model.Id);
        if (site is null)
        {
            return false;
        }

        MapToEntity(model, site);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<SiteDeleteViewModel?> GetForDeleteAsync(int id)
    {
        var now = clock.Now;

        return await dbContext.Sites
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(s => new SiteDeleteViewModel
            {
                Id = s.Id,
                Name = s.Name,
                City = s.City,
                Address = s.Address,
                ClientName = s.ClientName,
                FutureShiftsCount = s.Shifts.Count(sh => sh.StartTime > now),
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var site = await dbContext.Sites.FirstOrDefaultAsync(s => s.Id == id);
        if (site is null)
        {
            return false;
        }

        var now = clock.Now;
        var futureShifts = await dbContext.Shifts
            .Where(sh => sh.SiteId == id && sh.StartTime > now)
            .ToListAsync();

        dbContext.Shifts.RemoveRange(futureShifts);

        site.IsDeleted = true;
        site.IsActive = false;

        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DropdownItemViewModel>> GetDropdownAsync(bool onlyActive, int? includeId = null)
        => await dbContext.Sites
            .AsNoTracking()
            .Where(s => !onlyActive || s.IsActive || s.Id == includeId)
            .OrderBy(s => s.Name)
            .Select(s => new DropdownItemViewModel
            {
                Id = s.Id,
                Text = s.Name + " (" + s.City + ")",
            })
            .ToListAsync();

    private static void MapToEntity(SiteFormViewModel model, Site site)
    {
        site.Name = model.Name.Trim();
        site.Address = model.Address.Trim();
        site.City = model.City.Trim();
        site.ClientName = model.ClientName.Trim();
        site.ContactPhone = model.ContactPhone.Trim();
        site.RequiredGuardsPerShift = model.RequiredGuardsPerShift;
        site.Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim();
        site.IsActive = model.IsActive;
    }
}

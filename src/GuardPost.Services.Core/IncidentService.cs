using Microsoft.EntityFrameworkCore;

using GuardPost.Common;
using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Data.Models;
using GuardPost.Data.Models.Enums;
using GuardPost.Services.Core.Contracts;
using GuardPost.Services.Core.Mapping;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Incidents;

namespace GuardPost.Services.Core;

public class IncidentService : IIncidentService
{
    private readonly GuardPostDbContext dbContext;
    private readonly IDateTimeProvider clock;

    public IncidentService(GuardPostDbContext dbContext, IDateTimeProvider clock)
    {
        this.dbContext = dbContext;
        this.clock = clock;
    }

    public async Task<IncidentIndexViewModel> GetIndexAsync(
        string? searchTerm,
        IncidentSeverity? severity,
        IncidentStatus? status,
        int? siteId,
        int page,
        int pageSize)
    {
        IQueryable<Incident> query = dbContext.Incidents.AsNoTracking();

        if (severity.HasValue)
        {
            query = query.Where(i => i.Severity == severity.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        if (siteId.HasValue)
        {
            query = query.Where(i => i.SiteId == siteId.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim();
            query = query.Where(i => i.Title.Contains(term) || i.Description.Contains(term));
        }

        int totalCount = await query.CountAsync();
        page = Paging.Normalize(page, totalCount, pageSize);

        var items = await query
            .OrderBy(i => i.Status == IncidentStatus.Resolved)
            .ThenByDescending(i => i.OccurredOn)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(Projections.IncidentToListItem)
            .ToListAsync();

        return new IncidentIndexViewModel
        {
            SearchTerm = searchTerm,
            Severity = severity,
            Status = status,
            SiteId = siteId,
            Result = new PagedResult<IncidentListItemViewModel>(items, totalCount, page, pageSize),
        };
    }

    public async Task<IncidentDetailsViewModel?> GetDetailsAsync(int id)
        => await dbContext.Incidents
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new IncidentDetailsViewModel
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                OccurredOn = i.OccurredOn,
                CreatedOn = i.CreatedOn,
                Severity = i.Severity,
                Status = i.Status,
                ResolutionNotes = i.ResolutionNotes,
                SiteId = i.SiteId,
                SiteName = i.Site.Name,
                SiteAddress = i.Site.Address + ", " + i.Site.City,
                ReportedByGuardId = i.ReportedByGuardId,
                ReportedBy = i.ReportedByGuard == null
                    ? null
                    : i.ReportedByGuard.FirstName + " " + i.ReportedByGuard.LastName,
            })
            .FirstOrDefaultAsync();

    public async Task<IncidentFormViewModel?> GetForEditAsync(int id)
        => await dbContext.Incidents
            .AsNoTracking()
            .Where(i => i.Id == id)
            .Select(i => new IncidentFormViewModel
            {
                Id = i.Id,
                Title = i.Title,
                Description = i.Description,
                OccurredOn = i.OccurredOn,
                Severity = i.Severity,
                Status = i.Status,
                ResolutionNotes = i.ResolutionNotes,
                SiteId = i.SiteId,
                ReportedByGuardId = i.ReportedByGuardId,
            })
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyList<ValidationError>> ValidateAsync(IncidentFormViewModel model)
    {
        var errors = new List<ValidationError>();

        if (model.SiteId.HasValue && !await dbContext.Sites.AnyAsync(s => s.Id == model.SiteId.Value))
        {
            errors.Add(new ValidationError(nameof(model.SiteId), ValidationMessages.SiteNotAvailable));
        }

        if (model.ReportedByGuardId.HasValue && !await dbContext.Guards.AnyAsync(g => g.Id == model.ReportedByGuardId.Value))
        {
            errors.Add(new ValidationError(nameof(model.ReportedByGuardId), ValidationMessages.GuardNotAvailable));
        }

        return errors;
    }

    public async Task<int> CreateAsync(IncidentFormViewModel model)
    {
        var incident = new Incident { CreatedOn = clock.Now };
        MapToEntity(model, incident);

        await dbContext.Incidents.AddAsync(incident);
        await dbContext.SaveChangesAsync();

        return incident.Id;
    }

    public async Task<bool> UpdateAsync(IncidentFormViewModel model)
    {
        var incident = await dbContext.Incidents.FirstOrDefaultAsync(i => i.Id == model.Id);
        if (incident is null)
        {
            return false;
        }

        MapToEntity(model, incident);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var incident = await dbContext.Incidents.FirstOrDefaultAsync(i => i.Id == id);
        if (incident is null)
        {
            return false;
        }

        dbContext.Incidents.Remove(incident);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static void MapToEntity(IncidentFormViewModel model, Incident incident)
    {
        incident.Title = model.Title.Trim();
        incident.Description = model.Description.Trim();
        incident.OccurredOn = model.OccurredOn!.Value;
        incident.Severity = model.Severity!.Value;
        incident.Status = model.Status!.Value;
        incident.ResolutionNotes = string.IsNullOrWhiteSpace(model.ResolutionNotes) ? null : model.ResolutionNotes.Trim();
        incident.SiteId = model.SiteId!.Value;
        incident.ReportedByGuardId = model.ReportedByGuardId;
    }
}


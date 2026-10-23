using Microsoft.EntityFrameworkCore;

using GuardPost.Common.Providers;
using GuardPost.Data;
using GuardPost.Data.Models.Enums;
using GuardPost.Services.Core.Contracts;
using GuardPost.Services.Core.Mapping;
using GuardPost.Web.ViewModels.Home;

namespace GuardPost.Services.Core;

public class DashboardService : IDashboardService
{
    private const int IncidentsOnDashboard = 5;

    private readonly GuardPostDbContext dbContext;
    private readonly IDateTimeProvider clock;

    public DashboardService(GuardPostDbContext dbContext, IDateTimeProvider clock)
    {
        this.dbContext = dbContext;
        this.clock = clock;
    }

    public async Task<DashboardViewModel> GetDashboardAsync()
    {
        var now = clock.Now;
        var today = now.Date;
        var tomorrow = today.AddDays(1);

        var todayShifts = await dbContext.Shifts
            .AsNoTracking()
            .Where(sh => sh.StartTime < tomorrow && sh.EndTime > today)
            .OrderBy(sh => sh.StartTime)
            .ThenBy(sh => sh.Site.Name)
            .Select(Projections.ShiftToListItem)
            .ToListAsync();

        var coverage = await dbContext.Sites
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SiteCoverageViewModel
            {
                SiteId = s.Id,
                SiteName = s.Name,
                Required = s.RequiredGuardsPerShift,
                OnDuty = s.Shifts.Count(sh => sh.StartTime <= now && sh.EndTime > now),
            })
            .ToListAsync();

        var openIncidents = await dbContext.Incidents
            .AsNoTracking()
            .Where(i => i.Status != IncidentStatus.Resolved)
            .OrderByDescending(i => i.Severity)
            .ThenByDescending(i => i.OccurredOn)
            .Take(IncidentsOnDashboard)
            .Select(Projections.IncidentToListItem)
            .ToListAsync();

        return new DashboardViewModel
        {
            Now = now,
            ActiveSites = coverage.Count,
            ActiveGuards = await dbContext.Guards.CountAsync(g => g.IsActive),
            ShiftsToday = todayShifts.Count,
            OpenIncidents = await dbContext.Incidents.CountAsync(i => i.Status != IncidentStatus.Resolved),
            Coverage = coverage,
            TodayShifts = todayShifts,
            LatestOpenIncidents = openIncidents,
        };
    }
}

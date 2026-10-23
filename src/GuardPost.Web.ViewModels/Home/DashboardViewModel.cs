using GuardPost.Web.ViewModels.Incidents;
using GuardPost.Web.ViewModels.Shifts;

namespace GuardPost.Web.ViewModels.Home;

public class DashboardViewModel
{
    public DateTime Now { get; set; }

    public int ActiveSites { get; set; }

    public int ActiveGuards { get; set; }

    public int ShiftsToday { get; set; }

    public int OpenIncidents { get; set; }

    public IEnumerable<SiteCoverageViewModel> Coverage { get; set; } = new List<SiteCoverageViewModel>();

    public IEnumerable<ShiftListItemViewModel> TodayShifts { get; set; } = new List<ShiftListItemViewModel>();

    public IEnumerable<IncidentListItemViewModel> LatestOpenIncidents { get; set; } = new List<IncidentListItemViewModel>();
}

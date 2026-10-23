using GuardPost.Web.ViewModels.Incidents;
using GuardPost.Web.ViewModels.Shifts;

namespace GuardPost.Web.ViewModels.Sites;

public class SiteDetailsViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string City { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public string ContactPhone { get; set; } = null!;

    public int RequiredGuardsPerShift { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public int GuardsOnDutyNow { get; set; }

    public IEnumerable<ShiftListItemViewModel> UpcomingShifts { get; set; } = new List<ShiftListItemViewModel>();

    public IEnumerable<IncidentListItemViewModel> RecentIncidents { get; set; } = new List<IncidentListItemViewModel>();
}

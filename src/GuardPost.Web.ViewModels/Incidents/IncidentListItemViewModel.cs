using GuardPost.Data.Models.Enums;

namespace GuardPost.Web.ViewModels.Incidents;

public class IncidentListItemViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public DateTime OccurredOn { get; set; }

    public IncidentSeverity Severity { get; set; }

    public IncidentStatus Status { get; set; }

    public string? ReportedBy { get; set; }
}

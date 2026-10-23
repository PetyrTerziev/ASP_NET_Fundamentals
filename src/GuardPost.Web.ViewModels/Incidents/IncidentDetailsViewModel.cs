using GuardPost.Data.Models.Enums;

namespace GuardPost.Web.ViewModels.Incidents;

public class IncidentDetailsViewModel
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime OccurredOn { get; set; }

    public DateTime CreatedOn { get; set; }

    public IncidentSeverity Severity { get; set; }

    public IncidentStatus Status { get; set; }

    public string? ResolutionNotes { get; set; }

    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public string SiteAddress { get; set; } = null!;

    public int? ReportedByGuardId { get; set; }

    public string? ReportedBy { get; set; }
}

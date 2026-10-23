using GuardPost.Data.Models.Enums;

namespace GuardPost.Data.Models;

public class Incident
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime OccurredOn { get; set; }

    public IncidentSeverity Severity { get; set; }

    public IncidentStatus Status { get; set; }

    public string? ResolutionNotes { get; set; }

    public DateTime CreatedOn { get; set; }

    public int SiteId { get; set; }

    public virtual Site Site { get; set; } = null!;

    public int? ReportedByGuardId { get; set; }

    public virtual Guard? ReportedByGuard { get; set; }
}

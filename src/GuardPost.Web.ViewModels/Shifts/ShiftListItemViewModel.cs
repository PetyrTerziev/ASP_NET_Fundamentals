namespace GuardPost.Web.ViewModels.Shifts;

public class ShiftListItemViewModel
{
    public int Id { get; set; }

    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public int GuardId { get; set; }

    public string GuardName { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public string? Notes { get; set; }

    public double DurationHours => (EndTime - StartTime).TotalHours;

    public bool IsNightShift => StartTime.Hour >= 18 || StartTime.Hour < 6;

    public bool IsOngoing(DateTime now) => StartTime <= now && EndTime > now;
}

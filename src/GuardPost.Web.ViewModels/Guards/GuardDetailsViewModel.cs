using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Incidents;
using GuardPost.Web.ViewModels.Shifts;

namespace GuardPost.Web.ViewModels.Guards;

public class GuardDetailsViewModel
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FullName => $"{FirstName} {LastName}";

    public string LicenseNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime HireDate { get; set; }

    public GuardRank Rank { get; set; }

    public bool IsActive { get; set; }

    public double HoursThisMonth { get; set; }

    public int ShiftsThisMonth { get; set; }

    public IEnumerable<ShiftListItemViewModel> UpcomingShifts { get; set; } = new List<ShiftListItemViewModel>();

    public IEnumerable<IncidentListItemViewModel> ReportedIncidents { get; set; } = new List<IncidentListItemViewModel>();
}

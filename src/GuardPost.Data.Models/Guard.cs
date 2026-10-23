using GuardPost.Data.Models.Enums;

namespace GuardPost.Data.Models;

public class Guard
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    /// <summary>Unique professional license number, e.g. ОХ-104233.</summary>
    public string LicenseNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime HireDate { get; set; }

    public GuardRank Rank { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<Shift> Shifts { get; set; } = new HashSet<Shift>();

    public virtual ICollection<Incident> ReportedIncidents { get; set; } = new HashSet<Incident>();
}

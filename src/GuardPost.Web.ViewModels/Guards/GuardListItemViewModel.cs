using GuardPost.Data.Models.Enums;

namespace GuardPost.Web.ViewModels.Guards;

public class GuardListItemViewModel
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public GuardRank Rank { get; set; }

    public bool IsActive { get; set; }

    public double HoursThisMonth { get; set; }
}

using GuardPost.Data.Models.Enums;

namespace GuardPost.Web.ViewModels.Guards;

public class GuardDeleteViewModel
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string LicenseNumber { get; set; } = null!;

    public GuardRank Rank { get; set; }

    public int FutureShiftsCount { get; set; }
}

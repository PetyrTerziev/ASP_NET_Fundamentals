namespace GuardPost.Web.ViewModels.Sites;

public class SiteDeleteViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public int FutureShiftsCount { get; set; }
}

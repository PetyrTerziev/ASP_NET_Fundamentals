namespace GuardPost.Web.ViewModels.Sites;

public class SiteListItemViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string ClientName { get; set; } = null!;

    public int RequiredGuardsPerShift { get; set; }

    public bool IsActive { get; set; }

    public int UpcomingShiftsCount { get; set; }

    public int OpenIncidentsCount { get; set; }
}

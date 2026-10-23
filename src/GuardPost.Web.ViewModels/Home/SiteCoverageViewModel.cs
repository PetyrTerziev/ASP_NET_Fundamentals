namespace GuardPost.Web.ViewModels.Home;

/// <summary>How many guards are on duty right now compared to what the contract requires.</summary>
public class SiteCoverageViewModel
{
    public int SiteId { get; set; }

    public string SiteName { get; set; } = null!;

    public int Required { get; set; }

    public int OnDuty { get; set; }

    public bool IsCovered => OnDuty >= Required;
}

namespace GuardPost.Web.ViewModels.Common;

/// <summary>
/// Model of the reusable _PaginationPartial – keeps the current filters when changing pages.
/// </summary>
public class PagerViewModel
{
    public int CurrentPage { get; set; }

    public int TotalPages { get; set; }

    public string Action { get; set; } = "Index";

    public IDictionary<string, string?> RouteValues { get; set; } = new Dictionary<string, string?>();

    public IDictionary<string, string?> RouteValuesFor(int page)
    {
        var values = new Dictionary<string, string?>(RouteValues)
        {
            ["page"] = page.ToString(),
        };

        return values;
    }
}

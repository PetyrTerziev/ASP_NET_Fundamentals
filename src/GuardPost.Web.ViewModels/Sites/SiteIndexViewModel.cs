using GuardPost.Web.ViewModels.Common;

namespace GuardPost.Web.ViewModels.Sites;

public class SiteIndexViewModel
{
    public string? SearchTerm { get; set; }

    public string? City { get; set; }

    public bool ShowInactive { get; set; }

    public IEnumerable<string> Cities { get; set; } = new List<string>();

    public PagedResult<SiteListItemViewModel> Result { get; set; } = null!;
}

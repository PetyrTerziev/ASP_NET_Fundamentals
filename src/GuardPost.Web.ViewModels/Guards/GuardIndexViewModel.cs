using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Common;

namespace GuardPost.Web.ViewModels.Guards;

public class GuardIndexViewModel
{
    public string? SearchTerm { get; set; }

    public GuardRank? Rank { get; set; }

    public bool ShowInactive { get; set; }

    public PagedResult<GuardListItemViewModel> Result { get; set; } = null!;
}

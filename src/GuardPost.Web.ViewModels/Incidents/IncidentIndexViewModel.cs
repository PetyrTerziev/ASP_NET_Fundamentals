using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Common;

namespace GuardPost.Web.ViewModels.Incidents;

public class IncidentIndexViewModel
{
    public string? SearchTerm { get; set; }

    public IncidentSeverity? Severity { get; set; }

    public IncidentStatus? Status { get; set; }

    public int? SiteId { get; set; }

    public IEnumerable<DropdownItemViewModel> Sites { get; set; } = new List<DropdownItemViewModel>();

    public PagedResult<IncidentListItemViewModel> Result { get; set; } = null!;
}

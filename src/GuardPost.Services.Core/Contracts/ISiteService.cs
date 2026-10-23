using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Sites;

namespace GuardPost.Services.Core.Contracts;

public interface ISiteService
{
    Task<SiteIndexViewModel> GetIndexAsync(string? searchTerm, string? city, bool showInactive, int page, int pageSize);

    Task<SiteDetailsViewModel?> GetDetailsAsync(int id);

    Task<SiteFormViewModel?> GetForEditAsync(int id);

    Task<int> CreateAsync(SiteFormViewModel model);

    Task<bool> UpdateAsync(SiteFormViewModel model);

    Task<SiteDeleteViewModel?> GetForDeleteAsync(int id);

    Task<bool> DeleteAsync(int id);

    Task<IEnumerable<DropdownItemViewModel>> GetDropdownAsync(bool onlyActive, int? includeId = null);
}

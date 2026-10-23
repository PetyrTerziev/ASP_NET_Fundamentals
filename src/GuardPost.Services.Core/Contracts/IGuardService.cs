using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Guards;

namespace GuardPost.Services.Core.Contracts;

public interface IGuardService
{
    Task<GuardIndexViewModel> GetIndexAsync(string? searchTerm, GuardRank? rank, bool showInactive, int page, int pageSize);

    Task<GuardDetailsViewModel?> GetDetailsAsync(int id);

    Task<GuardFormViewModel?> GetForEditAsync(int id);

    Task<int> CreateAsync(GuardFormViewModel model);

    Task<bool> UpdateAsync(GuardFormViewModel model);

    Task<GuardDeleteViewModel?> GetForDeleteAsync(int id);

    Task<bool> DeleteAsync(int id);

    Task<bool> IsLicenseNumberTakenAsync(string licenseNumber, int excludeGuardId = 0);

    Task<IEnumerable<DropdownItemViewModel>> GetDropdownAsync(bool onlyActive, int? includeId = null);
}

using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Shifts;

namespace GuardPost.Services.Core.Contracts;

public interface IShiftService
{
    Task<ShiftWeekViewModel> GetWeekAsync(DateTime? weekStart, int? siteId, int? guardId);

    Task<ShiftListItemViewModel?> GetDetailsAsync(int id);

    Task<ShiftFormViewModel?> GetForEditAsync(int id);

    Task<IReadOnlyList<ValidationError>> ValidateAsync(ShiftFormViewModel model);

    Task<int> CreateAsync(ShiftFormViewModel model);

    Task<bool> UpdateAsync(ShiftFormViewModel model);

    Task<bool> DeleteAsync(int id);
}

using GuardPost.Data.Models.Enums;
using GuardPost.Web.ViewModels.Common;
using GuardPost.Web.ViewModels.Incidents;

namespace GuardPost.Services.Core.Contracts;

public interface IIncidentService
{
    Task<IncidentIndexViewModel> GetIndexAsync(
        string? searchTerm,
        IncidentSeverity? severity,
        IncidentStatus? status,
        int? siteId,
        int page,
        int pageSize);

    Task<IncidentDetailsViewModel?> GetDetailsAsync(int id);

    Task<IncidentFormViewModel?> GetForEditAsync(int id);

    Task<IReadOnlyList<ValidationError>> ValidateAsync(IncidentFormViewModel model);

    Task<int> CreateAsync(IncidentFormViewModel model);

    Task<bool> UpdateAsync(IncidentFormViewModel model);

    Task<bool> DeleteAsync(int id);
}

using GuardPost.Web.ViewModels.Home;

namespace GuardPost.Services.Core.Contracts;

public interface IDashboardService
{
    Task<DashboardViewModel> GetDashboardAsync();
}

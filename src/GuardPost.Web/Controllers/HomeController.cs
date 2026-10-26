using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;

using GuardPost.Services.Core.Contracts;
using GuardPost.Web.ViewModels.Home;

namespace GuardPost.Web.Controllers;

public class HomeController : BaseController
{
    private readonly IDashboardService dashboardService;

    public HomeController(IDashboardService dashboardService)
    {
        this.dashboardService = dashboardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var model = await dashboardService.GetDashboardAsync();
        return View(model);
    }

    [HttpGet]
    public IActionResult About() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

    /// <summary>Friendly page for 404 / 400 and other status codes (see UseStatusCodePagesWithReExecute).</summary>
    [HttpGet]
    public IActionResult HttpError(int code)
    {
        Response.StatusCode = code;
        return View(new ErrorViewModel { StatusCode = code });
    }
}

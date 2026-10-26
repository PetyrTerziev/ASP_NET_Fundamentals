using Microsoft.AspNetCore.Mvc;

using GuardPost.Common;
using GuardPost.Services.Core.Contracts;
using GuardPost.Web.ViewModels.Sites;

namespace GuardPost.Web.Controllers;

public class SitesController : BaseController
{
    private readonly ISiteService siteService;

    public SitesController(ISiteService siteService)
    {
        this.siteService = siteService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, string? city, bool showInactive = false, int page = 1)
    {
        var model = await siteService.GetIndexAsync(searchTerm, city, showInactive, page, ApplicationConstants.DefaultPageSize);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = await siteService.GetDetailsAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public IActionResult Create() => View(new SiteFormViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(SiteFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int id = await siteService.CreateAsync(model);
        ShowSuccess($"Обектът „{model.Name}“ беше създаден.");

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await siteService.GetForEditAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, SiteFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool updated = await siteService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        ShowSuccess($"Обектът „{model.Name}“ беше редактиран.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await siteService.GetForDeleteAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await siteService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        ShowSuccess("Обектът беше изтрит.");
        return RedirectToAction(nameof(Index));
    }
}

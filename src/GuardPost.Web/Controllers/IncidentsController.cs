using Microsoft.AspNetCore.Mvc;

using GuardPost.Common;
using GuardPost.Common.Providers;
using GuardPost.Data.Models.Enums;
using GuardPost.Services.Core.Contracts;
using GuardPost.Web.ViewModels.Incidents;

namespace GuardPost.Web.Controllers;

public class IncidentsController : BaseController
{
    private readonly IIncidentService incidentService;
    private readonly ISiteService siteService;
    private readonly IGuardService guardService;
    private readonly IDateTimeProvider clock;

    public IncidentsController(
        IIncidentService incidentService,
        ISiteService siteService,
        IGuardService guardService,
        IDateTimeProvider clock)
    {
        this.incidentService = incidentService;
        this.siteService = siteService;
        this.guardService = guardService;
        this.clock = clock;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        IncidentSeverity? severity,
        IncidentStatus? status,
        int? siteId,
        int page = 1)
    {
        var model = await incidentService.GetIndexAsync(searchTerm, severity, status, siteId, page, ApplicationConstants.DefaultPageSize);
        model.Sites = await siteService.GetDropdownAsync(onlyActive: false);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = await incidentService.GetDetailsAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? siteId)
    {
        var now = clock.Now;
        var model = new IncidentFormViewModel
        {
            SiteId = siteId,
            OccurredOn = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0),
        };

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(IncidentFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            AddModelErrors(await incidentService.ValidateAsync(model));
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        int id = await incidentService.CreateAsync(model);
        ShowSuccess("Инцидентът беше регистриран.");

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await incidentService.GetForEditAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, IncidentFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            AddModelErrors(await incidentService.ValidateAsync(model));
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        bool updated = await incidentService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        ShowSuccess("Инцидентът беше редактиран.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await incidentService.GetDetailsAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await incidentService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        ShowSuccess("Инцидентът беше изтрит.");
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdownsAsync(IncidentFormViewModel model)
    {
        model.Sites = await siteService.GetDropdownAsync(onlyActive: false);
        model.Guards = await guardService.GetDropdownAsync(onlyActive: true, includeId: model.ReportedByGuardId);
    }
}

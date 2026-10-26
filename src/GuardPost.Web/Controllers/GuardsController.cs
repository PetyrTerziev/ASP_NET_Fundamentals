using Microsoft.AspNetCore.Mvc;

using GuardPost.Common;
using GuardPost.Data.Models.Enums;
using GuardPost.Services.Core.Contracts;
using GuardPost.Web.ViewModels.Guards;

namespace GuardPost.Web.Controllers;

public class GuardsController : BaseController
{
    private readonly IGuardService guardService;

    public GuardsController(IGuardService guardService)
    {
        this.guardService = guardService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? searchTerm, GuardRank? rank, bool showInactive = false, int page = 1)
    {
        var model = await guardService.GetIndexAsync(searchTerm, rank, showInactive, page, ApplicationConstants.DefaultPageSize);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = await guardService.GetDetailsAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public IActionResult Create() => View(new GuardFormViewModel());

    [HttpPost]
    public async Task<IActionResult> Create(GuardFormViewModel model)
    {
        await ValidateLicenseNumberAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        int id = await guardService.CreateAsync(model);
        ShowSuccess($"{model.FirstName} {model.LastName} беше добавен(а) към екипа.");

        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await guardService.GetForEditAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, GuardFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        await ValidateLicenseNumberAsync(model);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        bool updated = await guardService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        ShowSuccess($"Данните на {model.FirstName} {model.LastName} бяха редактирани.");
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await guardService.GetForDeleteAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        bool deleted = await guardService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }

        ShowSuccess("Охранителят беше изтрит.");
        return RedirectToAction(nameof(Index));
    }

    /// <summary>Endpoint used by the [Remote] attribute for live (client-side) uniqueness check.</summary>
    [AcceptVerbs("GET", "POST")]
    public async Task<IActionResult> CheckLicenseNumber(string licenseNumber, int id)
    {
        if (string.IsNullOrWhiteSpace(licenseNumber))
        {
            return Json(true);
        }

        bool taken = await guardService.IsLicenseNumberTakenAsync(licenseNumber, id);

        return taken
            ? Json(string.Format(ValidationMessages.LicenseNumberTaken, licenseNumber.Trim().ToUpperInvariant()))
            : Json(true);
    }

    private async Task ValidateLicenseNumberAsync(GuardFormViewModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.LicenseNumber)
            && await guardService.IsLicenseNumberTakenAsync(model.LicenseNumber, model.Id))
        {
            ModelState.AddModelError(
                nameof(model.LicenseNumber),
                string.Format(ValidationMessages.LicenseNumberTaken, model.LicenseNumber.Trim().ToUpperInvariant()));
        }
    }
}

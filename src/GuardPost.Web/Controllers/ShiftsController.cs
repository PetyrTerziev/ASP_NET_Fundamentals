using Microsoft.AspNetCore.Mvc;

using GuardPost.Common.Extensions;
using GuardPost.Common.Providers;
using GuardPost.Services.Core.Contracts;
using GuardPost.Web.ViewModels.Shifts;

namespace GuardPost.Web.Controllers;

public class ShiftsController : BaseController
{
    private const int DefaultShiftStartHour = 8;
    private const int DefaultShiftLengthHours = 12;

    private readonly IShiftService shiftService;
    private readonly ISiteService siteService;
    private readonly IGuardService guardService;
    private readonly IDateTimeProvider clock;

    public ShiftsController(
        IShiftService shiftService,
        ISiteService siteService,
        IGuardService guardService,
        IDateTimeProvider clock)
    {
        this.shiftService = shiftService;
        this.siteService = siteService;
        this.guardService = guardService;
        this.clock = clock;
    }

    /// <summary>Weekly schedule.</summary>
    [HttpGet]
    public async Task<IActionResult> Index(DateTime? weekStart, int? siteId, int? guardId)
    {
        var model = await shiftService.GetWeekAsync(weekStart, siteId, guardId);
        model.Sites = await siteService.GetDropdownAsync(onlyActive: false);
        model.Guards = await guardService.GetDropdownAsync(onlyActive: false);

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var model = await shiftService.GetDetailsAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? siteId, int? guardId, DateTime? date)
    {
        var day = date?.Date ?? clock.Today.AddDays(1);
        var start = day.AddHours(DefaultShiftStartHour);

        var model = new ShiftFormViewModel
        {
            SiteId = siteId,
            GuardId = guardId,
            StartTime = start,
            EndTime = start.AddHours(DefaultShiftLengthHours),
        };

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ShiftFormViewModel model)
    {
        if (ModelState.IsValid)
        {
            AddModelErrors(await shiftService.ValidateAsync(model));
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        await shiftService.CreateAsync(model);
        ShowSuccess("Смяната беше добавена в графика.");

        return RedirectToAction(nameof(Index), new { weekStart = WeekParameter(model.StartTime!.Value) });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var model = await shiftService.GetForEditAsync(id);
        if (model is null)
        {
            return NotFound();
        }

        await PopulateDropdownsAsync(model);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, ShiftFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            AddModelErrors(await shiftService.ValidateAsync(model));
        }

        if (!ModelState.IsValid)
        {
            await PopulateDropdownsAsync(model);
            return View(model);
        }

        bool updated = await shiftService.UpdateAsync(model);
        if (!updated)
        {
            return NotFound();
        }

        ShowSuccess("Смяната беше редактирана.");
        return RedirectToAction(nameof(Index), new { weekStart = WeekParameter(model.StartTime!.Value) });
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var model = await shiftService.GetDetailsAsync(id);
        return model is null ? NotFound() : View(model);
    }

    [HttpPost]
    [ActionName(nameof(Delete))]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var shift = await shiftService.GetDetailsAsync(id);
        if (shift is null || !await shiftService.DeleteAsync(id))
        {
            return NotFound();
        }

        ShowSuccess("Смяната беше премахната от графика.");
        return RedirectToAction(nameof(Index), new { weekStart = WeekParameter(shift.StartTime) });
    }

    private async Task PopulateDropdownsAsync(ShiftFormViewModel model)
    {
        model.Sites = await siteService.GetDropdownAsync(onlyActive: true, includeId: model.Id == 0 ? null : model.SiteId);
        model.Guards = await guardService.GetDropdownAsync(onlyActive: true, includeId: model.Id == 0 ? null : model.GuardId);
    }

    private static string WeekParameter(DateTime date) => date.StartOfWeek().ToString("yyyy-MM-dd");
}

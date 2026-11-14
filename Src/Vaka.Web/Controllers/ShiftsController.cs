using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class ShiftsController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IShiftService _shifts;

    public ShiftsController(IShiftService shifts)
    {
        _shifts = shifts;
    }

    [HttpGet("~/shifts")]
    public async Task<IActionResult> Index(int? facilityId, CancellationToken cancellationToken)
    {
        var model = await _shifts.ListAsync(facilityId, cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? facilityId, CancellationToken cancellationToken) =>
        await ShowFormAsync(null, facilityId, null, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(ShiftFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _shifts.CreateAsync(model.FacilityId, ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Vardiya eklendi.";
            return RedirectToAction(nameof(Index), new { facilityId = result.EntityId });
        }

        return await RejectAsync(result, null, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, ShiftFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _shifts.UpdateAsync(id, ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Vardiya güncellendi.";
            return RedirectToAction(nameof(Index), new { facilityId = result.EntityId });
        }

        return await RejectAsync(result, id, model, cancellationToken);
    }

    // Çözülemeyen saat modelde boş gelir; hata iletisini servis Türkçe verir, bağlayıcının İngilizce iletisi atılır.
    private ShiftRequest ToRequest(ShiftFormViewModel model)
    {
        ModelState.Remove(nameof(ShiftFormViewModel.StartTime));
        ModelState.Remove(nameof(ShiftFormViewModel.EndTime));
        return new ShiftRequest(model.Name, model.StartTime, model.EndTime, model.IsActive);
    }

    private async Task<IActionResult> RejectAsync(
        CommandResult result,
        int? id,
        ShiftFormViewModel posted,
        CancellationToken cancellationToken)
    {
        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(id, posted.FacilityId, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? id, int? facilityId, ShiftFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _shifts.GetFormAsync(id, facilityId, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        if (posted is not null)
        {
            model.Name = posted.Name;
            model.StartTime = posted.StartTime;
            model.EndTime = posted.EndTime;
            model.IsActive = posted.IsActive;
        }

        return View("Form", model);
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class CleaningTypesController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly ICleaningTypeService _types;
    private readonly IMachineMappingService _mapping;

    public CleaningTypesController(ICleaningTypeService types, IMachineMappingService mapping)
    {
        _types = types;
        _mapping = mapping;
    }

    [HttpGet("~/cleaning-types")]
    public async Task<IActionResult> Index(CleaningTypeKind? kind, bool showInactive, CancellationToken cancellationToken)
    {
        var model = await _types.ListAsync(kind, showInactive, cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        await ShowFormAsync(null, null, false, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(CleaningTypeFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _types.CreateAsync(ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Tip eklendi.";
            return RedirectToAction(nameof(Details), new { id = result.EntityId });
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(null, model, false, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, false, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, CleaningTypeFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _types.UpdateAsync(id, User.GetUserId(), ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Tip güncellendi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        var blocked = result.Errors.Any(error => error.Field == nameof(CleaningTypeFormViewModel.IsActive));
        return await ShowFormAsync(id, model, blocked, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var lookup = await _types.GetDetailAsync(id, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Machines(int id, CancellationToken cancellationToken)
    {
        var lookup = await _mapping.GetAsync(id, cancellationToken);
        return lookup.Status switch
        {
            CommandStatus.Succeeded => View(lookup.Value),
            CommandStatus.Rejected => RedirectWithMessage(id, "Acil tipe makine eşlenmez."),
            _ => NotFound()
        };
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Machines(int id, int[]? selectedMachineIds, CancellationToken cancellationToken)
    {
        var result = await _mapping.SaveAsync(id, selectedMachineIds ?? [], cancellationToken);
        return result.Status switch
        {
            CommandStatus.Succeeded => RedirectWithMessage(id, "Makine eşlemesi kaydedildi."),
            CommandStatus.NotFound => NotFound(),
            _ => RedirectWithMessage(id, result.Errors[0].Message)
        };
    }

    private IActionResult RedirectWithMessage(int id, string message)
    {
        TempData[StatusKey] = message;
        return RedirectToAction(nameof(Details), new { id });
    }

    private static CleaningTypeRequest ToRequest(CleaningTypeFormViewModel model) =>
        new(model.Code, model.Name, model.Kind, model.DefaultLockMinutes, model.IsActive);

    private async Task<IActionResult> ShowFormAsync(int? id, CleaningTypeFormViewModel? posted, bool withBlocking, CancellationToken cancellationToken)
    {
        var lookup = await _types.GetFormAsync(id, User.GetUserId(), withBlocking, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        if (posted is not null)
        {
            model.Code = posted.Code;
            model.Name = posted.Name;
            model.IsActive = posted.IsActive;
            if (!model.KindLocked)
            {
                model.Kind = posted.Kind;
                model.DefaultLockMinutes = posted.DefaultLockMinutes;
            }
            else if (model.Kind == CleaningTypeKind.Planned)
            {
                model.DefaultLockMinutes = posted.DefaultLockMinutes;
            }
        }

        return View("Form", model);
    }
}

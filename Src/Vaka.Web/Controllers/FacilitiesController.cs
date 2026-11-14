using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class FacilitiesController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IFacilityService _facilities;

    public FacilitiesController(IFacilityService facilities)
    {
        _facilities = facilities;
    }

    [HttpGet("~/facilities")]
    public async Task<IActionResult> Index(bool showInactive, CancellationToken cancellationToken)
    {
        var model = await _facilities.ListAsync(showInactive, cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        await ShowFormAsync(null, null, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(FacilityFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _facilities.CreateAsync(new FacilityRequest(model.Code, model.Name, model.IsActive), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Tesis eklendi.";
            return RedirectToAction(nameof(Details), new { id = result.EntityId });
        }

        return await RejectAsync(result, null, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, FacilityFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _facilities.UpdateAsync(id, new FacilityRequest(model.Code, model.Name, model.IsActive), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Tesis güncellendi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return await RejectAsync(result, id, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var lookup = await _facilities.GetDetailAsync(id, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    private async Task<IActionResult> RejectAsync(
        CommandResult result,
        int? id,
        FacilityFormViewModel posted,
        CancellationToken cancellationToken)
    {
        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(id, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? id, FacilityFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _facilities.GetFormAsync(id, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        if (posted is not null)
        {
            if (!model.CodeLocked)
            {
                model.Code = posted.Code;
            }

            model.Name = posted.Name;
            model.IsActive = posted.IsActive;
        }

        return View("Form", model);
    }
}

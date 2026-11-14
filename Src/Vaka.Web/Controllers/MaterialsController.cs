using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class MaterialsController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IMaterialService _materials;

    public MaterialsController(IMaterialService materials)
    {
        _materials = materials;
    }

    [HttpGet("~/materials")]
    public async Task<IActionResult> Index(string? query, bool showInactive, int page = 1, CancellationToken cancellationToken = default)
    {
        var model = await _materials.ListAsync(query, showInactive, page, cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        await ShowFormAsync(null, null, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(MaterialFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _materials.CreateAsync(new MaterialRequest(model.Code, model.Name, model.IsActive), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Malzeme eklendi.";
            return RedirectToAction(nameof(Details), new { id = result.EntityId });
        }

        return await RejectAsync(result, null, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, MaterialFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _materials.UpdateAsync(id, new MaterialRequest(model.Code, model.Name, model.IsActive), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Malzeme güncellendi.";
            return RedirectToAction(nameof(Details), new { id });
        }

        return await RejectAsync(result, id, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var lookup = await _materials.GetDetailAsync(id, cancellationToken);
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
        MaterialFormViewModel posted,
        CancellationToken cancellationToken)
    {
        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(id, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? id, MaterialFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _materials.GetFormAsync(id, cancellationToken);
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
        }

        return View("Form", model);
    }
}

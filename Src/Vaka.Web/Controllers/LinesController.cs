using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class LinesController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly ILineService _lines;

    public LinesController(ILineService lines)
    {
        _lines = lines;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int? facilityId, CancellationToken cancellationToken) =>
        await ShowFormAsync(null, facilityId, null, cancellationToken);

    [HttpPost]
    public async Task<IActionResult> Create(LineFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _lines.CreateAsync(model.FacilityId, new LineRequest(model.Code, model.Name, model.IsActive), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Hat eklendi.";
            return RedirectToAction("Details", "Facilities", new { id = result.EntityId });
        }

        return await RejectAsync(result, null, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, LineFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _lines.UpdateAsync(id, new LineRequest(model.Code, model.Name, model.IsActive), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Hat güncellendi.";
            return RedirectToAction("Details", "Facilities", new { id = result.EntityId });
        }

        return await RejectAsync(result, id, model, cancellationToken);
    }

    private async Task<IActionResult> RejectAsync(
        CommandResult result,
        int? id,
        LineFormViewModel posted,
        CancellationToken cancellationToken)
    {
        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(id, posted.FacilityId, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? id, int? facilityId, LineFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _lines.GetFormAsync(id, facilityId, cancellationToken);
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

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class MachinesController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IMachineService _machines;
    private readonly IMachineRetirement _retirement;

    public MachinesController(IMachineService machines, IMachineRetirement retirement)
    {
        _machines = machines;
        _retirement = retirement;
    }

    [HttpGet("~/machines")]
    public async Task<IActionResult> Index(
        int? facilityId,
        int? lineId,
        string? query,
        bool showRetired,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var model = await _machines.ListAsync(new MachineListQuery(facilityId, lineId, query, showRetired, page), cancellationToken);
        model.StatusMessage = TempData[StatusKey] as string;
        return View(model);
    }

    // JS kapalıyken tesis yenileme düğmesi formu GET ile gönderir; girilen kod ve ad kaybolmasın diye geri bağlanır.
    [HttpGet]
    public async Task<IActionResult> Create(int? facilityId, int lineId, string? code, string? name, CancellationToken cancellationToken)
    {
        var posted = new MachineFormViewModel { FacilityId = facilityId ?? 0, LineId = lineId, Code = code ?? string.Empty, Name = name ?? string.Empty };
        return await ShowFormAsync(null, facilityId, posted, cancellationToken);
    }

    [HttpPost]
    public async Task<IActionResult> Create(MachineFormViewModel model, bool refresh, CancellationToken cancellationToken)
    {
        if (refresh)
        {
            ModelState.Clear();
            return await ShowFormAsync(null, model.FacilityId, model, cancellationToken);
        }

        var result = await _machines.CreateAsync(ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Makine eklendi.";
            return RedirectToAction(nameof(Index));
        }

        return await RejectAsync(result, null, model, cancellationToken);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken) =>
        await ShowFormAsync(id, null, null, cancellationToken);

    [HttpPost("{id:int}")]
    public async Task<IActionResult> Edit(int id, MachineFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _machines.UpdateAsync(id, ToRequest(model), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Makine güncellendi.";
            return RedirectToAction(nameof(Index));
        }

        return await RejectAsync(result, id, model, cancellationToken);
    }

    [HttpGet]
    public async Task<IActionResult> Lines(int facilityId, CancellationToken cancellationToken)
    {
        var lines = await _machines.ListLinesAsync(facilityId, cancellationToken);
        return Json(lines.Select(line => new { id = line.Id, label = line.Label }));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Retire(int id, CancellationToken cancellationToken)
    {
        var lookup = await _retirement.GetAsync(id, User.GetUserId(), cancellationToken);
        return lookup.Status == CommandStatus.Succeeded ? View(lookup.Value) : NotFound();
    }

    [HttpPost("{id:int}")]
    [ActionName(nameof(Retire))]
    public async Task<IActionResult> RetireConfirmed(int id, CancellationToken cancellationToken)
    {
        var result = await _retirement.RetireAsync(id, User.GetUserId(), cancellationToken);
        if (result.Succeeded)
        {
            TempData[StatusKey] = "Makine kullanımdan kaldırıldı.";
            return RedirectToAction(nameof(Index));
        }

        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        var lookup = await _retirement.GetAsync(id, User.GetUserId(), cancellationToken);
        return lookup.Status == CommandStatus.Succeeded ? View(lookup.Value) : NotFound();
    }

    private static MachineRequest ToRequest(MachineFormViewModel model) =>
        new(model.FacilityId, model.LineId, model.Code, model.Name);

    private async Task<IActionResult> RejectAsync(
        CommandResult result,
        int? id,
        MachineFormViewModel posted,
        CancellationToken cancellationToken)
    {
        if (result.Status == CommandStatus.NotFound)
        {
            return NotFound();
        }

        ModelState.AddErrors(result);
        return await ShowFormAsync(id, posted.FacilityId, posted, cancellationToken);
    }

    private async Task<IActionResult> ShowFormAsync(int? id, int? facilityId, MachineFormViewModel? posted, CancellationToken cancellationToken)
    {
        var lookup = await _machines.GetFormAsync(id, facilityId, cancellationToken);
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
                model.LineId = model.Lines.Any(line => line.Id == posted.LineId) ? posted.LineId : 0;
            }

            model.Name = posted.Name;
        }

        return View("Form", model);
    }
}

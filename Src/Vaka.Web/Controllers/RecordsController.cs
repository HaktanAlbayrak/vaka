using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Operator)]
[Route("[controller]/[action]")]
public sealed class RecordsController : Controller
{
    private readonly IRecordCreateForm _form;
    private readonly IRecordOpener _opener;
    private readonly IRecordOptions _options;
    private readonly ILotCatalog _lots;
    private readonly IRecordRunReader _run;
    private readonly IRecordMaterialService _materials;
    private readonly IBreakReader _breaks;

    public RecordsController(
        IRecordCreateForm form,
        IRecordOpener opener,
        IRecordOptions options,
        ILotCatalog lots,
        IRecordRunReader run,
        IRecordMaterialService materials,
        IBreakReader breaks)
    {
        _form = form;
        _opener = opener;
        _options = options;
        _lots = lots;
        _run = run;
        _materials = materials;
        _breaks = breaks;
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await _form.BuildAsync(null, false, cancellationToken));

    // Ret ve yenileme yanıtları her zaman 200 görünümdür; CommandForm.js yalnız yönlendirmeyi izler.
    [HttpPost]
    public async Task<IActionResult> Create(RecordCreateViewModel model, bool refresh, CancellationToken cancellationToken)
    {
        ModelState.Clear();
        model.ErrorMessage = null;
        model.ErrorUrl = null;
        if (refresh)
        {
            return View(await _form.BuildAsync(model, true, cancellationToken));
        }

        var outcome = await _opener.OpenAsync(User.GetUserId(), ToRequest(model), cancellationToken);
        if (outcome.Result.Succeeded)
        {
            return RedirectToAction(nameof(Run), new { id = outcome.Result.EntityId });
        }

        if (outcome.Result.Status == CommandStatus.Forbidden)
        {
            return StatusCode(StatusCodes.Status403Forbidden);
        }

        if (outcome.Result.Status == CommandStatus.OnBreak)
        {
            return RedirectToAction("Current", "Breaks");
        }

        ModelState.AddErrors(outcome.Result);
        model.ErrorMessage = JoinGeneral(outcome.Result);
        model.ErrorUrl = outcome.BlockingRecordId is { } blockingId ? RecordLinks.Run(blockingId) : null;
        return View(await _form.BuildAsync(model, false, cancellationToken));
    }

    [HttpGet]
    public async Task<IActionResult> Lines(int facilityId, CancellationToken cancellationToken) =>
        Json(await _options.ListLinesAsync(facilityId, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Machines(int lineId, CancellationToken cancellationToken) =>
        Json(await _options.ListMachinesAsync(lineId, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> CleaningTypes(int machineId, CancellationToken cancellationToken) =>
        Json(await _options.ListCleaningTypesAsync(machineId, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Lots(string? q, int? recordId, CancellationToken cancellationToken) =>
        Json((await _lots.FindAsync(q, null, recordId, cancellationToken)).Lots);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Run(int id, string? q, int? stepId, string? people, CancellationToken cancellationToken)
    {
        if (await _breaks.IsOnBreakAsync(User.GetUserId(), cancellationToken))
        {
            return RedirectToAction("Current", "Breaks");
        }

        return await ShowRunAsync(id, new RunViewOptions(q, null, stepId, people), null, null, null, cancellationToken);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> AddMaterial(int id, AddMaterialViewModel model, CancellationToken cancellationToken)
    {
        var result = await _materials.AddAsync(
            id,
            User.GetUserId(),
            new AddMaterialRequest(model.CommandId, model.MaterialLotId, model.ExpiredReason, model.Pin),
            cancellationToken);

        var pending = new PendingMaterialViewModel { MaterialLotId = model.MaterialLotId, ExpiredReason = model.ExpiredReason };
        return await FinishAsync(id, result, model.CommandId, pending, cancellationToken);
    }

    [HttpPost("{id:int}")]
    public async Task<IActionResult> RemoveMaterial(int id, RemoveMaterialViewModel model, CancellationToken cancellationToken)
    {
        var result = await _materials.RemoveAsync(
            id,
            User.GetUserId(),
            new RemoveMaterialRequest(model.CommandId, model.RecordMaterialId, model.Pin),
            cancellationToken);

        return await FinishAsync(id, result, model.CommandId, null, cancellationToken);
    }

    private async Task<IActionResult> FinishAsync(
        int id,
        CommandResult result,
        Guid commandId,
        PendingMaterialViewModel? pending,
        CancellationToken cancellationToken)
    {
        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                return RedirectToAction(nameof(Run), new { id });
            case CommandStatus.NotFound:
                return NotFound();
            case CommandStatus.Forbidden:
                return StatusCode(StatusCodes.Status403Forbidden);
            case CommandStatus.OnBreak:
                return RedirectToAction("Current", "Breaks");
            default:
                return await ShowRunAsync(id, new RunViewOptions(PendingLotId: pending?.MaterialLotId), pending, string.Join(" ", result.Errors.Select(error => error.Message)), commandId, cancellationToken);
        }
    }

    private async Task<IActionResult> ShowRunAsync(
        int id,
        RunViewOptions options,
        PendingMaterialViewModel? pending,
        string? errorMessage,
        Guid? commandId,
        CancellationToken cancellationToken)
    {
        var lookup = await _run.GetAsync(id, User.GetUserId(), options, cancellationToken);
        if (lookup.Value is not { } model)
        {
            return NotFound();
        }

        if (commandId is { } kept && kept != Guid.Empty)
        {
            model.CommandId = kept;
        }

        model.ErrorMessage = errorMessage;
        model.PendingMaterial = pending;
        return View("Run", model);
    }

    private static RecordOpenRequest ToRequest(RecordCreateViewModel model) =>
        new(
            model.CommandId,
            model.FacilityId,
            model.LineId,
            model.MachineId,
            model.CleaningTypeId,
            model.WorkOrderNo,
            model.Note,
            model.LockMinutes,
            [.. model.Materials.Select(item => new LotEntryRequest(item.MaterialLotId, item.ExpiredReason))],
            model.Pin);

    private static string? JoinGeneral(CommandResult result)
    {
        var messages = result.Errors.Where(error => error.Field.Length == 0).Select(error => error.Message).ToArray();
        return messages.Length == 0 ? null : string.Join(" ", messages);
    }
}

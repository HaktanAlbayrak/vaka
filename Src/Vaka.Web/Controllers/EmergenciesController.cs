using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Emergencies;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Operator)]
[Route("[controller]/[action]")]
public sealed class EmergenciesController : Controller
{
    private readonly IEmergencyForm _form;
    private readonly IEmergencyOpener _opener;
    private readonly IRecordOptions _options;

    public EmergenciesController(IEmergencyForm form, IEmergencyOpener opener, IRecordOptions options)
    {
        _form = form;
        _opener = opener;
        _options = options;
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken) =>
        View(await _form.BuildAsync(null, false, cancellationToken));

    // Ret ve yenileme yanıtları her zaman 200 görünümdür; CommandForm.js yalnız yönlendirmeyi izler.
    [HttpPost]
    public async Task<IActionResult> Create(EmergencyCreateViewModel model, bool refresh, CancellationToken cancellationToken)
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
            return Redirect(RecordLinks.Run(outcome.Result.EntityId!.Value));
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
    public async Task<IActionResult> Machines(int lineId, CancellationToken cancellationToken) =>
        Json(await _options.ListMachinesAsync(lineId, cancellationToken));

    private static EmergencyOpenRequest ToRequest(EmergencyCreateViewModel model) =>
        new(
            model.CommandId,
            model.FacilityId,
            model.LineId,
            model.MachineId,
            model.CleaningTypeId,
            model.Description,
            [.. model.Materials.Select(item => new LotEntryRequest(item.MaterialLotId, item.ExpiredReason))],
            model.Pin);

    private static string? JoinGeneral(CommandResult result)
    {
        var messages = result.Errors.Where(error => error.Field.Length == 0).Select(error => error.Message).ToArray();
        return messages.Length == 0 ? null : string.Join(" ", messages);
    }
}

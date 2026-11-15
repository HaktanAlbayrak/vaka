using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Interventions;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Manager)]
[Route("[controller]/[action]")]
public sealed class InterventionsController : Controller
{
    private readonly IInterventionForms _forms;
    private readonly IInterventionService _interventions;

    public InterventionsController(IInterventionForms forms, IInterventionService interventions)
    {
        _forms = forms;
        _interventions = interventions;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int recordId, InterventionType? type, CancellationToken cancellationToken)
    {
        var lookup = await _forms.BuildCreateAsync(recordId, type, cancellationToken);
        return lookup.Value is { } model ? View(model) : NotFound();
    }

    // Ret yanıtı her zaman 200 görünümdür; yalnız başarı yönlendirir.
    [HttpPost]
    public async Task<IActionResult> Create(InterventionViewModel model, CancellationToken cancellationToken)
    {
        var result = await _interventions.InterveneAsync(
            model.RecordId,
            User.GetUserId(),
            new InterventionRequest(model.CommandId, model.Type, model.Reason, model.Pin),
            cancellationToken);

        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                return RedirectToAction("Details", "Records", new { id = model.RecordId });
            case CommandStatus.NotFound:
                return NotFound();
            case CommandStatus.Forbidden:
                return StatusCode(StatusCodes.Status403Forbidden);
        }

        var lookup = await _forms.BuildCreateAsync(model.RecordId, model.Type, cancellationToken);
        if (lookup.Value is not { } form)
        {
            return NotFound();
        }

        ModelState.Clear();
        ModelState.AddErrors(result);
        form.CommandId = model.CommandId == Guid.Empty ? form.CommandId : model.CommandId;
        form.Reason = model.Reason ?? string.Empty;
        form.ErrorMessage = GeneralMessage(result);
        return View(form);
    }

    [HttpGet("{breakId:int}")]
    public async Task<IActionResult> EndBreak(int breakId, CancellationToken cancellationToken)
    {
        var lookup = await _forms.BuildEndBreakAsync(breakId, cancellationToken);
        return lookup.Value is { } model ? View(model) : NotFound();
    }

    [HttpPost("{breakId:int}")]
    public async Task<IActionResult> EndBreak(int breakId, EndBreakViewModel model, CancellationToken cancellationToken)
    {
        var result = await _interventions.EndBreakAsync(
            breakId,
            User.GetUserId(),
            new ManagerBreakEndRequest(model.CommandId, model.Reason, model.Pin),
            cancellationToken);

        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                return RedirectToAction("Details", "Records", new { id = result.EntityId });
            case CommandStatus.NotFound:
                return NotFound();
            case CommandStatus.Forbidden:
                return StatusCode(StatusCodes.Status403Forbidden);
        }

        var lookup = await _forms.BuildEndBreakAsync(breakId, cancellationToken);
        if (lookup.Value is not { } form)
        {
            return NotFound();
        }

        ModelState.Clear();
        ModelState.AddErrors(result);
        form.CommandId = model.CommandId == Guid.Empty ? form.CommandId : model.CommandId;
        form.Reason = model.Reason ?? string.Empty;
        form.ErrorMessage = GeneralMessage(result);
        return View(form);
    }

    private static string? GeneralMessage(CommandResult result)
    {
        var messages = result.Errors.Where(error => error.Field.Length == 0).Select(error => error.Message).ToArray();
        return messages.Length == 0 ? null : string.Join(" ", messages);
    }
}

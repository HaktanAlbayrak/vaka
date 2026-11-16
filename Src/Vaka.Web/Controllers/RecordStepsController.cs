using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.Operator)]
[Route("records/[action]")]
public sealed class RecordStepsController : Controller
{
    private const string RunView = "~/Views/Records/Run.cshtml";
    private const string FinishView = "~/Views/Records/FinishStep.cshtml";

    private readonly IStepStarter _starter;
    private readonly IStepFinisher _finisher;
    private readonly IParticipantService _participants;
    private readonly IParticipantDirectory _directory;
    private readonly IRecordRunReader _run;
    private readonly IStepLocator _steps;

    public RecordStepsController(
        IStepStarter starter,
        IStepFinisher finisher,
        IParticipantService participants,
        IParticipantDirectory directory,
        IRecordRunReader run,
        IStepLocator steps)
    {
        _starter = starter;
        _finisher = finisher;
        _participants = participants;
        _directory = directory;
        _run = run;
        _steps = steps;
    }

    [HttpPost("{stepId:int}")]
    public async Task<IActionResult> StartStep(int stepId, StartStepViewModel model, CancellationToken cancellationToken)
    {
        var result = await _starter.StartAsync(stepId, User.GetUserId(), new StartStepRequest(model.CommandId, model.Pin), cancellationToken);
        return await RespondAsync(stepId, result, model.CommandId, null, cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    public async Task<IActionResult> FinishStep(int stepId, FinishStepViewModel model, CancellationToken cancellationToken)
    {
        var outcome = await _finisher.FinishAsync(stepId, User.GetUserId(), new FinishStepRequest(model.CommandId, model.Pin, model.BelowMinimumReason), cancellationToken);
        if (outcome.Prompt is not { } prompt || outcome.Result.Succeeded)
        {
            return await RespondAsync(stepId, outcome.Result, model.CommandId, null, cancellationToken);
        }

        ModelState.Clear();
        var attempted = !string.IsNullOrWhiteSpace(model.BelowMinimumReason);
        if (attempted)
        {
            ModelState.AddErrors(outcome.Result);
        }

        return View(FinishView, new FinishStepViewModel
        {
            StepId = stepId,
            RecordId = prompt.RecordId,
            PhaseTitle = prompt.PhaseTitle,
            PhaseNetSeconds = prompt.PhaseNetSeconds,
            MinDurationSeconds = prompt.MinDurationSeconds,
            CommandId = model.CommandId,
            BelowMinimumReason = model.BelowMinimumReason,
            ErrorMessage = attempted ? Join(outcome.Result) : null
        });
    }

    [HttpGet]
    public async Task<IActionResult> People(string? q, int? stepId, CancellationToken cancellationToken)
    {
        if (stepId is not { } id)
        {
            return BadRequest();
        }

        var lookup = await _directory.SearchForOwnerAsync(id, User.GetUserId(), q, cancellationToken);
        return lookup.Status switch
        {
            CommandStatus.NotFound => NotFound(),
            CommandStatus.Forbidden => Forbid(),
            _ => Json(new ParticipantSearchResponseViewModel { Items = lookup.Value!.Options, Truncated = lookup.Value.Truncated })
        };
    }

    [HttpPost("{stepId:int}")]
    public async Task<IActionResult> AddParticipant(int stepId, AddParticipantViewModel model, CancellationToken cancellationToken)
    {
        var result = await _participants.AddAsync(stepId, User.GetUserId(), new AddParticipantRequest(model.CommandId, model.ParticipantUserId, model.Pin), cancellationToken);
        var pending = new PendingParticipantViewModel { StepId = stepId, ParticipantUserId = model.ParticipantUserId };
        return await RespondAsync(stepId, result, model.CommandId, pending, cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    public async Task<IActionResult> RemoveParticipant(int stepId, RemoveParticipantViewModel model, CancellationToken cancellationToken)
    {
        var result = await _participants.RemoveAsync(stepId, User.GetUserId(), new RemoveParticipantRequest(model.CommandId, model.ParticipantUserId, model.Pin), cancellationToken);
        return await RespondAsync(stepId, result, model.CommandId, null, cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    public async Task<IActionResult> JoinStep(int stepId, JoinStepViewModel model, CancellationToken cancellationToken)
    {
        var result = await _participants.JoinAsync(stepId, User.GetUserId(), new JoinStepRequest(model.CommandId, model.ParticipantUserId, model.Pin), cancellationToken);
        var pending = new PendingParticipantViewModel { StepId = stepId, ParticipantUserId = model.ParticipantUserId };
        return await RespondAsync(stepId, result, model.CommandId, pending, cancellationToken, stepId);
    }

    [HttpPost("{stepId:int}")]
    public async Task<IActionResult> LeaveStep(int stepId, LeaveStepViewModel model, CancellationToken cancellationToken)
    {
        var result = await _participants.LeaveAsync(stepId, User.GetUserId(), new LeaveStepRequest(model.CommandId, model.ParticipantUserId, model.Pin), cancellationToken);
        var pending = new PendingParticipantViewModel { StepId = stepId, ParticipantUserId = model.ParticipantUserId };
        return await RespondAsync(stepId, result, model.CommandId, pending, cancellationToken, stepId);
    }

    // Ret her zaman 200 görünümdür (PL-62); yönlendirme yalnız başarı ve molada olan kişi içindir (BR-39).
    private async Task<IActionResult> RespondAsync(int stepId, CommandResult result, Guid commandId, PendingParticipantViewModel? pending, CancellationToken cancellationToken, int? returnStepId = null)
    {
        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                return RedirectToAction("Run", "Records", new { id = result.EntityId, stepId = returnStepId });
            case CommandStatus.NotFound:
                return NotFound();
            case CommandStatus.Forbidden:
                return StatusCode(StatusCodes.Status403Forbidden);
            case CommandStatus.OnBreak:
                return RedirectToAction("Current", "Breaks");
        }

        if (await _steps.GetRecordIdAsync(stepId, cancellationToken) is not { } recordId)
        {
            return NotFound();
        }

        var options = new RunViewOptions(ParticipantStepId: pending?.StepId, PendingParticipant: pending);
        var lookup = await _run.GetAsync(recordId, User.GetUserId(), options, cancellationToken);
        if (lookup.Value is not { } model)
        {
            return NotFound();
        }

        ModelState.Clear();
        ModelState.AddErrors(result);
        model.CommandId = commandId == Guid.Empty ? model.CommandId : commandId;
        model.ErrorMessage = Join(result);
        return View(RunView, model);
    }

    private static string Join(CommandResult result) => string.Join(" ", result.Errors.Select(error => error.Message));
}

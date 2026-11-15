using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Vaka.Web.Authorization;
using Vaka.Web.Services;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Controllers;

[Authorize(Roles = Roles.ManagerOrAuditor)]
[Route("[controller]/[action]")]
public sealed class ProcedureVersionsController : Controller
{
    private const string StatusKey = "StatusMessage";

    private readonly IProcedureVersionReader _reader;
    private readonly IProcedureVersionPublisher _publisher;
    private readonly IProcedureDraftEditor _drafts;
    private readonly IProcedureStepEditor _steps;
    private readonly IStepMediaService _media;

    public ProcedureVersionsController(
        IProcedureVersionReader reader,
        IProcedureVersionPublisher publisher,
        IProcedureDraftEditor drafts,
        IProcedureStepEditor steps,
        IStepMediaService media)
    {
        _reader = reader;
        _publisher = publisher;
        _drafts = drafts;
        _steps = steps;
        _media = media;
    }

    [HttpPost]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> CreateDraft(CreateDraftViewModel model, CancellationToken cancellationToken)
    {
        var result = await _publisher.CreateDraftAsync(model.CleaningTypeId, cancellationToken);
        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                TempData[StatusKey] = "Taslak açıldı.";
                return RedirectToAction(nameof(Edit), new { id = result.EntityId });
            case CommandStatus.NotFound:
                return NotFound();
            default:
                TempData[StatusKey] = result.Errors[0].Message;
                return RedirectToAction("Details", "CleaningTypes", new { id = model.CleaningTypeId });
        }
    }

    [HttpGet("{id:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> Edit(int id, CancellationToken cancellationToken)
    {
        var lookup = await _reader.GetDraftAsync(id, cancellationToken);
        switch (lookup.Status)
        {
            case CommandStatus.Succeeded:
                var model = lookup.Value!;
                model.StatusMessage = TempData[StatusKey] as string;
                return View(model);
            case CommandStatus.Rejected:
                return RedirectToAction(nameof(Details), new { id });
            default:
                return NotFound();
        }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken)
    {
        var lookup = await _reader.GetVersionAsync(id, cancellationToken);
        switch (lookup.Status)
        {
            case CommandStatus.Succeeded:
                var model = lookup.Value!;
                model.StatusMessage = TempData[StatusKey] as string;
                return View(model);
            case CommandStatus.Rejected when User.IsInRole(Roles.Manager):
                return RedirectToAction(nameof(Edit), new { id });
            default:
                return NotFound();
        }
    }

    [HttpPost("{id:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> UpdateHeader(int id, DraftHeaderViewModel model, CancellationToken cancellationToken)
    {
        var result = await _drafts.UpdateHeaderAsync(id, new HeaderRequest(model.MaterialRequired, model.DocumentRef), cancellationToken);
        return await RespondAsync(id, result, "header", "Üst bilgi kaydedildi.", new DraftFormErrorViewModel
        {
            DocumentRef = model.DocumentRef,
            MaterialRequired = model.MaterialRequired
        }, cancellationToken);
    }

    [HttpPost("{id:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> AddPhase(int id, PhaseFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _drafts.AddPhaseAsync(id, new PhaseRequest(model.Title, model.MinDurationMinutes), cancellationToken);
        return await RespondAsync(id, result, "add-phase", "Faz eklendi.", PhaseEcho(model), cancellationToken);
    }

    [HttpPost("{phaseId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> UpdatePhase(int phaseId, PhaseFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _drafts.UpdatePhaseAsync(phaseId, new PhaseRequest(model.Title, model.MinDurationMinutes), cancellationToken);
        return await RespondAsync(null, result, $"phase:{phaseId}", "Faz güncellendi.", PhaseEcho(model), cancellationToken);
    }

    [HttpPost("{phaseId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> MovePhase(int phaseId, MoveViewModel model, CancellationToken cancellationToken)
    {
        var result = await _drafts.MovePhaseAsync(phaseId, model.Direction, cancellationToken);
        return await RespondAsync(null, result, $"phase:{phaseId}", "Faz taşındı.", null, cancellationToken);
    }

    [HttpPost("{phaseId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> RemovePhase(int phaseId, CancellationToken cancellationToken)
    {
        var result = await _drafts.RemovePhaseAsync(phaseId, cancellationToken);
        return await RespondAsync(null, result, $"phase:{phaseId}", "Faz çıkarıldı.", null, cancellationToken);
    }

    [HttpPost("{phaseId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> AddStep(int phaseId, StepFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _steps.AddStepAsync(phaseId, new StepRequest(model.Title, model.Description), cancellationToken);
        return await RespondAsync(null, result, $"add-step:{phaseId}", "Adım eklendi.", StepEcho(model), cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> UpdateStep(int stepId, StepFormViewModel model, CancellationToken cancellationToken)
    {
        var result = await _steps.UpdateStepAsync(stepId, new StepRequest(model.Title, model.Description), cancellationToken);
        return await RespondAsync(null, result, $"step:{stepId}", "Adım güncellendi.", StepEcho(model), cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> MoveStep(int stepId, MoveViewModel model, CancellationToken cancellationToken)
    {
        var result = await _steps.MoveStepAsync(stepId, model.Direction, cancellationToken);
        return await RespondAsync(null, result, $"step:{stepId}", "Adım taşındı.", null, cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> RemoveStep(int stepId, CancellationToken cancellationToken)
    {
        var result = await _steps.RemoveStepAsync(stepId, cancellationToken);
        return await RespondAsync(null, result, $"step:{stepId}", "Adım çıkarıldı.", null, cancellationToken);
    }

    [HttpPost("{stepId:int}")]
    [Authorize(Roles = Roles.Manager)]
    [ServiceFilter(typeof(UploadLimitFilter), Order = 900)]
    public async Task<IActionResult> UploadMedia(int stepId, MediaUploadViewModel model, CancellationToken cancellationToken)
    {
        MediaUploadRequest? upload = null;
        await using var content = model.File?.OpenReadStream();
        if (model.File is not null && content is not null)
        {
            upload = new MediaUploadRequest(model.File.FileName, model.File.Length, content);
        }

        var result = await _media.UploadAsync(stepId, upload, cancellationToken);
        return await RespondAsync(null, result, $"media:{stepId}", "Medya eklendi.", null, cancellationToken);
    }

    [HttpPost("{mediaId:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> RemoveMedia(int mediaId, CancellationToken cancellationToken)
    {
        var stepId = await _media.FindStepIdAsync(mediaId, cancellationToken);
        var result = await _media.RemoveAsync(mediaId, cancellationToken);
        return await RespondAsync(null, result, $"media:{stepId}", "Medya çıkarıldı.", null, cancellationToken);
    }

    [HttpPost("{id:int}")]
    [Authorize(Roles = Roles.Manager)]
    public async Task<IActionResult> Publish(int id, PublishViewModel model, CancellationToken cancellationToken)
    {
        var outcome = await _publisher.PublishAsync(id, User.GetUserId(), model.Pin, cancellationToken);
        var result = outcome.Result;
        if (result.Succeeded)
        {
            TempData[StatusKey] = $"v{outcome.VersionNo} yayınlandı.";
            return RedirectToAction("Details", "CleaningTypes", new { id = outcome.CleaningTypeId });
        }

        return await RespondAsync(id, result, "publish", string.Empty, null, cancellationToken);
    }

    private async Task<IActionResult> RespondAsync(
        int? versionId,
        CommandResult result,
        string target,
        string successMessage,
        DraftFormErrorViewModel? echo,
        CancellationToken cancellationToken)
    {
        var id = result.EntityId ?? versionId;
        switch (result.Status)
        {
            case CommandStatus.Succeeded:
                TempData[StatusKey] = successMessage;
                return RedirectToAction(nameof(Edit), new { id });
            case CommandStatus.NotFound:
                return NotFound();
            case CommandStatus.NotEditable:
                TempData[StatusKey] = result.Errors[0].Message;
                return RedirectToAction(nameof(Details), new { id });
            case CommandStatus.PinLocked:
                return await this.SignOutToLoginAsync(SessionEndReason.PinLocked);
        }

        var lookup = await _reader.GetDraftAsync(id!.Value, cancellationToken);
        if (lookup.Status != CommandStatus.Succeeded)
        {
            return NotFound();
        }

        var model = lookup.Value!;
        var error = echo ?? new DraftFormErrorViewModel();
        error.Target = target;
        error.Messages = result.Errors.Select(item => item.Message).ToArray();
        model.Error = error;
        return View(nameof(Edit), model);
    }

    private static DraftFormErrorViewModel PhaseEcho(PhaseFormViewModel model) =>
        new() { Title = model.Title, MinDurationMinutes = model.MinDurationMinutes };

    private static DraftFormErrorViewModel StepEcho(StepFormViewModel model) =>
        new() { Title = model.Title, Description = model.Description };
}

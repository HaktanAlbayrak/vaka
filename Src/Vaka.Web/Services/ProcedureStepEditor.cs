using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed class ProcedureStepEditor : IProcedureStepEditor
{
    private readonly VakaDbContext _db;

    public ProcedureStepEditor(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<CommandResult> AddStepAsync(int phaseId, StepRequest request, CancellationToken cancellationToken)
    {
        var phase = await _db.ProcedurePhases
            .Include(item => item.Version)
            .FirstOrDefaultAsync(item => item.Id == phaseId, cancellationToken);

        if (phase is null)
        {
            return CommandResult.NotFound();
        }

        if (DraftRules.CheckEditable(phase.Version) is { } blocked)
        {
            return blocked;
        }

        var errors = new List<FieldError>();
        var title = DraftRules.CheckTitle(request.Title, "Adım adı", errors);
        var description = DraftRules.CheckOptionalText(request.Description, "Description", "Açıklama", DraftRules.DescriptionMaxLength, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(phase.ProcedureVersionId, errors);
        }

        var count = await _db.ProcedureSteps.CountAsync(step => step.ProcedurePhaseId == phaseId, cancellationToken);
        _db.ProcedureSteps.Add(new ProcedureStep
        {
            ProcedurePhaseId = phaseId,
            OrderNo = count + 1,
            Title = title,
            Description = description
        });

        return await DraftRules.SaveAsync(_db, phase.ProcedureVersionId, cancellationToken);
    }

    public async Task<CommandResult> UpdateStepAsync(int stepId, StepRequest request, CancellationToken cancellationToken)
    {
        var step = await LoadStepAsync(stepId, cancellationToken);
        if (step is null)
        {
            return CommandResult.NotFound();
        }

        var versionId = step.Phase.ProcedureVersionId;
        if (DraftRules.CheckEditable(step.Phase.Version) is { } blocked)
        {
            return blocked;
        }

        var errors = new List<FieldError>();
        var title = DraftRules.CheckTitle(request.Title, "Adım adı", errors);
        var description = DraftRules.CheckOptionalText(request.Description, "Description", "Açıklama", DraftRules.DescriptionMaxLength, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(versionId, errors);
        }

        step.Title = title;
        step.Description = description;
        return await DraftRules.SaveAsync(_db, versionId, cancellationToken);
    }

    public async Task<CommandResult> MoveStepAsync(int stepId, MoveDirection direction, CancellationToken cancellationToken)
    {
        var step = await LoadStepAsync(stepId, cancellationToken);
        if (step is null)
        {
            return CommandResult.NotFound();
        }

        var versionId = step.Phase.ProcedureVersionId;
        if (DraftRules.CheckEditable(step.Phase.Version) is { } blocked)
        {
            return blocked;
        }

        var siblings = await _db.ProcedureSteps
            .Where(item => item.ProcedurePhaseId == step.ProcedurePhaseId)
            .OrderBy(item => item.OrderNo)
            .ToListAsync(cancellationToken);

        if (!DraftRules.TryMove(siblings, siblings.FindIndex(item => item.Id == stepId), direction, (item, orderNo) => item.OrderNo = orderNo))
        {
            return CommandResult.Rejected(versionId, "Direction", "Taşıma yönü geçersiz.");
        }

        return await DraftRules.SaveAsync(_db, versionId, cancellationToken);
    }

    public async Task<CommandResult> RemoveStepAsync(int stepId, CancellationToken cancellationToken)
    {
        var step = await LoadStepAsync(stepId, cancellationToken);
        if (step is null)
        {
            return CommandResult.NotFound();
        }

        var versionId = step.Phase.ProcedureVersionId;
        if (DraftRules.CheckEditable(step.Phase.Version) is { } blocked)
        {
            return blocked;
        }

        var media = await _db.StepMedia.Where(item => item.ProcedureStepId == stepId).ToListAsync(cancellationToken);
        _db.StepMedia.RemoveRange(media);
        _db.ProcedureSteps.Remove(step);

        var remaining = await _db.ProcedureSteps
            .Where(item => item.ProcedurePhaseId == step.ProcedurePhaseId && item.Id != stepId)
            .OrderBy(item => item.OrderNo)
            .ToListAsync(cancellationToken);
        DraftRules.Renumber(remaining, (item, orderNo) => item.OrderNo = orderNo);

        return await DraftRules.SaveAsync(_db, versionId, cancellationToken);
    }

    private Task<ProcedureStep?> LoadStepAsync(int stepId, CancellationToken cancellationToken) =>
        _db.ProcedureSteps
            .Include(step => step.Phase)
            .ThenInclude(phase => phase.Version)
            .FirstOrDefaultAsync(step => step.Id == stepId, cancellationToken);
}

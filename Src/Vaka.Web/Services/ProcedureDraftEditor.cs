using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed class ProcedureDraftEditor : IProcedureDraftEditor
{
    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;

    public ProcedureDraftEditor(VakaDbContext db, IDurationCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public async Task<CommandResult> UpdateHeaderAsync(int versionId, HeaderRequest request, CancellationToken cancellationToken)
    {
        var version = await _db.ProcedureVersions.FirstOrDefaultAsync(item => item.Id == versionId, cancellationToken);
        if (version is null)
        {
            return CommandResult.NotFound();
        }

        if (DraftRules.CheckEditable(version) is { } blocked)
        {
            return blocked;
        }

        var errors = new List<FieldError>();
        var documentRef = DraftRules.CheckOptionalText(request.DocumentRef, "DocumentRef", "Doküman no", DraftRules.DocumentRefMaxLength, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(versionId, errors);
        }

        version.MaterialRequired = request.MaterialRequired;
        version.DocumentRef = documentRef;
        return await DraftRules.SaveAsync(_db, versionId, cancellationToken);
    }

    public async Task<CommandResult> AddPhaseAsync(int versionId, PhaseRequest request, CancellationToken cancellationToken)
    {
        var version = await _db.ProcedureVersions
            .Include(item => item.CleaningType)
            .FirstOrDefaultAsync(item => item.Id == versionId, cancellationToken);

        if (version is null)
        {
            return CommandResult.NotFound();
        }

        if (DraftRules.CheckEditable(version) is { } blocked)
        {
            return blocked;
        }

        var errors = new List<FieldError>();
        var title = DraftRules.CheckTitle(request.Title, "Faz adı", errors);
        var minimum = DraftRules.CheckMinimum(request.MinDurationMinutes, version.CleaningType.Kind, _calculator, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(versionId, errors);
        }

        var count = await _db.ProcedurePhases.CountAsync(phase => phase.ProcedureVersionId == versionId, cancellationToken);
        _db.ProcedurePhases.Add(new ProcedurePhase
        {
            ProcedureVersionId = versionId,
            OrderNo = count + 1,
            Title = title,
            MinDurationSeconds = minimum
        });

        return await DraftRules.SaveAsync(_db, versionId, cancellationToken);
    }

    public async Task<CommandResult> UpdatePhaseAsync(int phaseId, PhaseRequest request, CancellationToken cancellationToken)
    {
        var phase = await LoadPhaseAsync(phaseId, cancellationToken);
        if (phase is null)
        {
            return CommandResult.NotFound();
        }

        if (DraftRules.CheckEditable(phase.Version) is { } blocked)
        {
            return blocked;
        }

        var errors = new List<FieldError>();
        var title = DraftRules.CheckTitle(request.Title, "Faz adı", errors);
        var minimum = DraftRules.CheckMinimum(request.MinDurationMinutes, phase.Version.CleaningType.Kind, _calculator, errors);
        if (errors.Count > 0)
        {
            return CommandResult.Rejected(phase.ProcedureVersionId, errors);
        }

        phase.Title = title;
        phase.MinDurationSeconds = minimum;
        return await DraftRules.SaveAsync(_db, phase.ProcedureVersionId, cancellationToken);
    }

    public async Task<CommandResult> MovePhaseAsync(int phaseId, MoveDirection direction, CancellationToken cancellationToken)
    {
        var phase = await LoadPhaseAsync(phaseId, cancellationToken);
        if (phase is null)
        {
            return CommandResult.NotFound();
        }

        if (DraftRules.CheckEditable(phase.Version) is { } blocked)
        {
            return blocked;
        }

        var siblings = await _db.ProcedurePhases
            .Where(item => item.ProcedureVersionId == phase.ProcedureVersionId)
            .OrderBy(item => item.OrderNo)
            .ToListAsync(cancellationToken);

        if (!DraftRules.TryMove(siblings, siblings.FindIndex(item => item.Id == phaseId), direction, (item, orderNo) => item.OrderNo = orderNo))
        {
            return CommandResult.Rejected(phase.ProcedureVersionId, "Direction", "Taşıma yönü geçersiz.");
        }

        return await DraftRules.SaveAsync(_db, phase.ProcedureVersionId, cancellationToken);
    }

    public async Task<CommandResult> RemovePhaseAsync(int phaseId, CancellationToken cancellationToken)
    {
        var phase = await LoadPhaseAsync(phaseId, cancellationToken);
        if (phase is null)
        {
            return CommandResult.NotFound();
        }

        if (DraftRules.CheckEditable(phase.Version) is { } blocked)
        {
            return blocked;
        }

        var steps = await _db.ProcedureSteps
            .Include(step => step.Media)
            .Where(step => step.ProcedurePhaseId == phaseId)
            .ToListAsync(cancellationToken);

        _db.StepMedia.RemoveRange(steps.SelectMany(step => step.Media));
        _db.ProcedureSteps.RemoveRange(steps);
        _db.ProcedurePhases.Remove(phase);

        var remaining = await _db.ProcedurePhases
            .Where(item => item.ProcedureVersionId == phase.ProcedureVersionId && item.Id != phaseId)
            .OrderBy(item => item.OrderNo)
            .ToListAsync(cancellationToken);
        DraftRules.Renumber(remaining, (item, orderNo) => item.OrderNo = orderNo);

        return await DraftRules.SaveAsync(_db, phase.ProcedureVersionId, cancellationToken);
    }

    private Task<ProcedurePhase?> LoadPhaseAsync(int phaseId, CancellationToken cancellationToken) =>
        _db.ProcedurePhases
            .Include(phase => phase.Version)
            .ThenInclude(version => version.CleaningType)
            .FirstOrDefaultAsync(phase => phase.Id == phaseId, cancellationToken);
}

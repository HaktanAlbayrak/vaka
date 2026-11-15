using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Interventions;

namespace Vaka.Web.Services;

public sealed class InterventionForms : IInterventionForms
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ITurkeyClock _clock;

    public InterventionForms(VakaDbContext db, IRecordTimeouts timeouts, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _clock = clock;
    }

    public async Task<LookupResult<InterventionViewModel>> BuildCreateAsync(int recordId, InterventionType? type, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);

        var row = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Id == recordId)
            .Select(record => new
            {
                record.RecordNo,
                record.Status,
                Machine = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                OwnerName = record.Owner.FullName
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            return LookupResult<InterventionViewModel>.Missing();
        }

        var allowed = InterventionRules.AllowedTypes(row.Status);
        var inProgressStep = await _db.RecordSteps
            .AsNoTracking()
            .Where(step => step.Phase.CleaningRecordId == recordId && step.Status == StepStatus.InProgress)
            .Select(step => step.ProcedureStep.Title)
            .FirstOrDefaultAsync(cancellationToken);
        var openBreak = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId && item.EndedAtUtc == null)
            .Select(item => new { StartedByName = item.StartedBy.FullName, item.StartedAtUtc })
            .FirstOrDefaultAsync(cancellationToken);

        return LookupResult<InterventionViewModel>.Found(new InterventionViewModel
        {
            CommandId = Guid.NewGuid(),
            RecordId = recordId,
            RecordNo = row.RecordNo,
            MachineLabel = row.Machine,
            OwnerName = row.OwnerName,
            RecordStatusLabel = RecordLabels.Status(row.Status),
            Type = type is { } chosen && allowed.Contains(chosen) ? chosen : null,
            AllowedTypes = [.. allowed.Select(item => new InterventionTypeOptionViewModel { Value = (int)item, Label = RecordLabels.Intervention(item) })],
            InProgressStepTitle = inProgressStep,
            OpenBreak = openBreak is null ? null : new InterventionBreakViewModel { StartedByName = openBreak.StartedByName, StartedAtLocal = _clock.ToLocal(openBreak.StartedAtUtc) },
            BlockReason = InterventionRules.BlockReason(row.Status)
        });
    }

    public async Task<LookupResult<EndBreakViewModel>> BuildEndBreakAsync(int breakId, CancellationToken cancellationToken)
    {
        var row = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.Id == breakId)
            .Select(item => new
            {
                item.CleaningRecordId,
                item.Record.RecordNo,
                StartedByName = item.StartedBy.FullName,
                item.StartedAtUtc,
                item.EndedAtUtc,
                DuringStepTitle = item.DuringStep == null ? null : item.DuringStep.ProcedureStep.Title
            })
            .FirstOrDefaultAsync(cancellationToken);
        if (row is null)
        {
            return LookupResult<EndBreakViewModel>.Missing();
        }

        return LookupResult<EndBreakViewModel>.Found(new EndBreakViewModel
        {
            CommandId = Guid.NewGuid(),
            BreakId = breakId,
            RecordId = row.CleaningRecordId,
            RecordNo = row.RecordNo,
            StartedByName = row.StartedByName,
            StartedAtLocal = _clock.ToLocal(row.StartedAtUtc),
            DuringStepTitle = row.DuringStepTitle,
            BlockReason = row.EndedAtUtc is null ? null : "Mola zaten bitmiş."
        });
    }
}

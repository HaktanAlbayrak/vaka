using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class RecordPhaseBuilder : IRecordPhaseBuilder
{
    private readonly VakaDbContext _db;

    public RecordPhaseBuilder(VakaDbContext db)
    {
        _db = db;
    }

    public async Task<RecordPhase[]> BuildAsync(int procedureVersionId, int ownerUserId, CancellationToken cancellationToken)
    {
        var phases = await _db.ProcedurePhases
            .AsNoTracking()
            .Where(phase => phase.ProcedureVersionId == procedureVersionId)
            .OrderBy(phase => phase.OrderNo)
            .Select(phase => new
            {
                phase.Id,
                phase.MinDurationSeconds,
                StepIds = phase.Steps.OrderBy(step => step.OrderNo).Select(step => step.Id).ToList()
            })
            .ToListAsync(cancellationToken);

        return [.. phases.Select(phase => new RecordPhase
        {
            ProcedurePhaseId = phase.Id,
            Status = PhaseStatus.Waiting,
            MinDurationSecondsSnapshot = phase.MinDurationSeconds,
            Steps = phase.StepIds.Select(stepId => new RecordStep
            {
                ProcedureStepId = stepId,
                Status = StepStatus.Waiting,
                Participants = [new StepParticipant { UserId = ownerUserId, AddedByUserId = ownerUserId }]
            }).ToList()
        })];
    }
}

using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class StepStartApplier : IStepStartApplier
{
    private readonly VakaDbContext _db;
    private readonly INumberSequences _numbers;
    private readonly ICleaningEventWriter _events;
    private readonly ITurkeyClock _clock;

    public StepStartApplier(VakaDbContext db, INumberSequences numbers, ICleaningEventWriter events, ITurkeyClock clock)
    {
        _db = db;
        _numbers = numbers;
        _events = events;
        _clock = clock;
    }

    public async Task<CommandResult?> CheckParticipantsAsync(RecordStep step, CancellationToken cancellationToken)
    {
        var ids = step.Participants.Select(item => item.UserId).ToArray();
        if (ids.Length == 0)
        {
            return Reject("Adımda görevli yok. Başlatmadan önce görevli ekleyin.");
        }

        var inactive = await _db.Users.AsNoTracking().Where(user => ids.Contains(user.Id) && !user.IsActive).Select(user => user.FullName).FirstOrDefaultAsync(cancellationToken);
        if (inactive is not null)
        {
            return Reject($"{inactive} pasif. Görevli listesinden çıkarın.");
        }

        var busy = await _db.StepParticipants.AsNoTracking()
            .Where(item => ids.Contains(item.UserId) && item.IsWorking)
            .Select(item => new { item.User.FullName, item.Step.Phase.Record.RecordNo })
            .FirstOrDefaultAsync(cancellationToken);
        if (busy is not null)
        {
            return Reject($"{busy.FullName} başka kayıtta devam eden adımda: {busy.RecordNo}.");
        }

        var onBreak = await _db.RecordBreaks.AsNoTracking()
            .Where(item => item.EndedAtUtc == null && ids.Contains(item.StartedByUserId))
            .Select(item => item.StartedBy.FullName)
            .FirstOrDefaultAsync(cancellationToken);
        return onBreak is null ? null : Reject($"{onBreak} molada.");
    }

    public async Task<CommandResult?> ApplyAsync(CleaningRecord record, RecordStep step, bool isFirst, int actorUserId, Guid commandId, DateTime now, CancellationToken cancellationToken)
    {
        if (isFirst)
        {
            if (record.Kind == CleaningTypeKind.Planned)
            {
                var facility = await _db.Machines.AsNoTracking()
                    .Where(machine => machine.Id == record.MachineId)
                    .Select(machine => machine.Line.FacilityId)
                    .FirstAsync(cancellationToken);
                var year = _clock.Today().Year;
                var sequence = await _numbers.NextAsync(RecordNumbers.FieldBookScope(facility), year, cancellationToken);
                record.FieldBookRef = RecordNumbers.FormatFieldBook(record.FacilityCodeSnapshot, year, sequence);
            }

            record.Status = RecordStatus.InProgress;
            record.StartedAtUtc = now;
            record.Version++;
        }

        step.Status = StepStatus.InProgress;
        step.StartedAtUtc = now;
        step.StartedByUserId = actorUserId;
        step.Phase.Status = PhaseStatus.InProgress;
        foreach (var participant in step.Participants)
        {
            participant.IsWorking = true;
        }

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (ConstraintViolations.IsUniqueViolation(exception, "StepParticipants.UserId"))
        {
            _db.ChangeTracker.Clear();
            return Reject("Görevlilerden biri başka bir adımda çalışıyor. Birkaç saniye sonra yeniden deneyin.");
        }

        var payload = new
        {
            StepId = step.Id,
            Title = step.ProcedureStep.Title,
            FirstStep = isFirst,
            record.FieldBookRef,
            ParticipantUserIds = step.Participants.Select(item => item.UserId).ToArray()
        };
        await _events.AppendAsync(new EventDraft(record.Id, EventTypes.StepStarted, payload, actorUserId, commandId, now), cancellationToken);
        return null;
    }

    private static CommandResult Reject(string message) => CommandResult.Rejected(string.Empty, message);
}

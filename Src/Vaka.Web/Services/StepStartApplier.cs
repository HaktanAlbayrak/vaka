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

    public async Task<CommandResult?> CheckParticipantsAsync(RecordStep step, int starterUserId, int ownerUserId, CancellationToken cancellationToken)
    {
        if (step.Participants.Count == 0)
        {
            return Reject("Adımda görevli yok. Başlatmadan önce görevli ekleyin.");
        }

        var required = step.Participants.Where(item => item.UserId == starterUserId || item.UserId == ownerUserId).Select(item => item.UserId).ToArray();
        var inactive = await _db.Users.AsNoTracking().Where(user => required.Contains(user.Id) && !user.IsActive).Select(user => user.FullName).FirstOrDefaultAsync(cancellationToken);
        if (inactive is not null)
        {
            return Reject($"{inactive} pasif. Görevli listesinden çıkarın.");
        }

        var busy = await _db.StepParticipants.AsNoTracking()
            .Where(item => required.Contains(item.UserId) && item.IsWorking)
            .Select(item => new { item.User.FullName, item.Step.Phase.Record.RecordNo })
            .FirstOrDefaultAsync(cancellationToken);
        if (busy is not null)
        {
            return Reject($"{busy.FullName} başka kayıtta devam eden adımda: {busy.RecordNo}.");
        }

        var onBreak = await _db.RecordBreaks.AsNoTracking()
            .Where(item => item.EndedAtUtc == null && required.Contains(item.StartedByUserId))
            .Select(item => item.StartedBy.FullName)
            .FirstOrDefaultAsync(cancellationToken);
        if (onBreak is not null)
        {
            return Reject($"{onBreak} molada.");
        }

        var workers = await SelectWorkersAsync(step, starterUserId, ownerUserId, cancellationToken);
        return workers.Count == 0
            ? Reject("Adımı başlatmak için görevli listesinde girişini yapmış ya da başlatan en az bir kişi olmalı. Kendinizi ekleyin ya da bir görevlinin girişini yapın.")
            : null;
    }

    /// <summary>
    /// Adım başlarken çalışmaya başlayanlar: başlatan, listedeki sahip ve girişini (<c>ConfirmedAtUtc</c>) yapmış, o anda aktif, molada olmayan ve
    /// başka adımda çalışmayan kişiler. Uygun olmayan girişli kişi sessizce dışarıda kalır ve Girişi bekleniyor olur.
    /// </summary>
    private async Task<List<StepParticipant>> SelectWorkersAsync(RecordStep step, int starterUserId, int ownerUserId, CancellationToken cancellationToken)
    {
        var confirmed = step.Participants
            .Where(item => item.UserId != starterUserId && item.UserId != ownerUserId && item.ConfirmedAtUtc is not null)
            .Select(item => item.UserId)
            .ToArray();
        var blocked = new HashSet<int>();
        if (confirmed.Length > 0)
        {
            blocked.UnionWith(await _db.Users.AsNoTracking().Where(user => confirmed.Contains(user.Id) && !user.IsActive).Select(user => user.Id).ToListAsync(cancellationToken));
            blocked.UnionWith(await _db.StepParticipants.AsNoTracking().Where(item => confirmed.Contains(item.UserId) && item.IsWorking).Select(item => item.UserId).ToListAsync(cancellationToken));
            blocked.UnionWith(await _db.RecordBreaks.AsNoTracking().Where(item => item.EndedAtUtc == null && confirmed.Contains(item.StartedByUserId)).Select(item => item.StartedByUserId).ToListAsync(cancellationToken));
        }

        return [.. step.Participants.Where(item =>
            item.UserId == starterUserId || item.UserId == ownerUserId || (item.ConfirmedAtUtc is not null && !blocked.Contains(item.UserId)))];
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
        var workers = await SelectWorkersAsync(step, actorUserId, record.OwnerUserId, cancellationToken);
        foreach (var participant in workers)
        {
            participant.IsWorking = true;
            if (participant.UserId != record.OwnerUserId)
            {
                participant.ConfirmedAtUtc ??= now;
            }

            participant.WorkInterval = new StepWorkInterval { StartedAtUtc = now };
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
            ParticipantUserIds = workers.Select(item => item.UserId).ToArray()
        };
        await _events.AppendAsync(new EventDraft(record.Id, EventTypes.StepStarted, payload, actorUserId, commandId, now), cancellationToken);
        return null;
    }

    private static CommandResult Reject(string message) => CommandResult.Rejected(string.Empty, message);
}

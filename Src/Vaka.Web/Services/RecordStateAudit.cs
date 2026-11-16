using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordStateAudit : IRecordStateAudit
{
    private readonly VakaDbContext _db;
    private readonly ITurkeyClock _clock;

    public RecordStateAudit(VakaDbContext db, ITurkeyClock clock)
    {
        _db = db;
        _clock = clock;
    }

    public async Task<IReadOnlyList<IntegrityMismatchViewModel>> FindMismatchesAsync(int recordId, IReadOnlyList<ParsedEvent> events, CancellationToken cancellationToken)
    {
        var facts = await LoadAsync(recordId, cancellationToken);
        if (facts is null)
        {
            return [];
        }

        var found = new StateMismatchCollector(_clock, await LoadUserNamesAsync(facts, events, cancellationToken));
        RecordStateRules.Compare(facts, events, found);
        ItemStateRules.Compare(facts, events, found);
        return found.Items.OrderBy(item => item.Sequence == 0 ? int.MaxValue : item.Sequence).ToArray();
    }

    private async Task<StateFacts?> LoadAsync(int recordId, CancellationToken cancellationToken)
    {
        var record = await _db.CleaningRecords
            .AsNoTracking()
            .Where(item => item.Id == recordId)
            .Select(item => new RecordFacts(
                item.RecordNo,
                item.Status,
                item.MachineId,
                item.ProcedureVersionId,
                item.OwnerUserId,
                item.WorkOrderNo,
                item.OpenedAtUtc,
                item.StartedAtUtc,
                item.CompletedAtUtc,
                item.ClosedAtUtc,
                item.ClosureKind,
                item.ClosedByUserId,
                item.ClosureReason))
            .FirstOrDefaultAsync(cancellationToken);
        if (record is null)
        {
            return null;
        }

        var steps = await _db.RecordSteps
            .AsNoTracking()
            .Where(step => step.Phase.CleaningRecordId == recordId)
            .OrderBy(step => step.Phase.ProcedurePhase.OrderNo).ThenBy(step => step.ProcedureStep.OrderNo)
            .Select(step => new
            {
                step.Id,
                PhaseOrder = step.Phase.ProcedurePhase.OrderNo,
                StepOrder = step.ProcedureStep.OrderNo,
                step.ProcedureStep.Title,
                step.StartedAtUtc,
                step.StartedByUserId,
                step.EndedAtUtc,
                step.EndedByUserId,
                step.DurationSeconds,
                Participants = step.Participants.Select(item => new { item.UserId, item.IsWorking, HasInterval = item.WorkInterval != null }).ToList()
            })
            .ToListAsync(cancellationToken);

        var phases = await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phase.CleaningRecordId == recordId)
            .OrderBy(phase => phase.ProcedurePhase.OrderNo)
            .Select(phase => new PhaseFacts(phase.Id, phase.ProcedurePhase.OrderNo, phase.Status, phase.BelowMinimum, phase.BelowMinimumReason, phase.NetSeconds))
            .ToArrayAsync(cancellationToken);

        var materials = await _db.RecordMaterials
            .AsNoTracking()
            .Where(material => material.CleaningRecordId == recordId && material.RemovedAtUtc == null)
            .Select(material => new MaterialFacts(material.MaterialLotId, material.LotNoSnapshot, material.ExpiredAtEntry, material.ExpiredReason))
            .ToArrayAsync(cancellationToken);

        var breaks = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId)
            .Select(item => new BreakFacts(item.Id, item.StartedByUserId, item.StartedAtUtc, item.EndedAtUtc, item.EndedByUserId, item.EndKind))
            .ToArrayAsync(cancellationToken);

        return new StateFacts(
            record,
            [.. steps.Select(step => new StepFacts(step.Id, step.PhaseOrder, step.StepOrder, step.Title, step.StartedAtUtc, step.StartedByUserId, step.EndedAtUtc, step.EndedByUserId, step.DurationSeconds, [.. WorkingParticipants(step.EndedAtUtc is not null, step.Participants.Select(item => (item.UserId, item.IsWorking, item.HasInterval)))]))],
            phases,
            materials,
            breaks);
    }

    /// <summary>
    /// Adımda çalışmış kişiler: aralığı olan, şu an çalışan ve Faz 6E öncesi biten adımda (hiçbir kişinin aralığı yok) tüm görevliler.
    /// Hiç katılmamış kişi çalışmış sayılmaz; olay tarafı <c>StepStarted</c> görevlileri ile adım sürerken <c>ParticipantJoined</c> olaylarıdır.
    /// </summary>
    private static IEnumerable<int> WorkingParticipants(bool ended, IEnumerable<(int UserId, bool IsWorking, bool HasInterval)> participants)
    {
        var list = participants.ToList();
        var legacyEnded = ended && list.All(item => !item.HasInterval);
        return list.Where(item => item.HasInterval || item.IsWorking || legacyEnded).Select(item => item.UserId);
    }

    private async Task<Dictionary<int, string>> LoadUserNamesAsync(StateFacts facts, IReadOnlyList<ParsedEvent> events, CancellationToken cancellationToken)
    {
        var ids = new HashSet<int> { facts.Record.OwnerUserId };
        ids.UnionWith(events.Where(item => item.ActorUserId is not null).Select(item => item.ActorUserId!.Value));
        ids.UnionWith(facts.Steps.SelectMany(step => step.ParticipantUserIds));
        foreach (var step in facts.Steps)
        {
            AddIfPresent(ids, step.StartedByUserId, step.EndedByUserId);
        }

        foreach (var item in facts.Breaks)
        {
            AddIfPresent(ids, item.StartedByUserId, item.EndedByUserId);
        }

        AddIfPresent(ids, facts.Record.ClosedByUserId);
        foreach (var item in events)
        {
            ids.UnionWith(item.Numbers("ParticipantUserIds") ?? []);
        }

        var list = ids.ToArray();
        return await _db.Users
            .AsNoTracking()
            .Where(user => list.Contains(user.Id))
            .ToDictionaryAsync(user => user.Id, user => user.FullName + " (" + user.RegistryNo + ")", cancellationToken);
    }

    private static void AddIfPresent(HashSet<int> ids, params int?[] values)
    {
        foreach (var value in values)
        {
            if (value is { } id)
            {
                ids.Add(id);
            }
        }
    }
}

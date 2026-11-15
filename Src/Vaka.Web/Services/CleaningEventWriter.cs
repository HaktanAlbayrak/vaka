using System.Globalization;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class CleaningEventWriter : ICleaningEventWriter
{
    private const string MomentFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";

    private readonly VakaDbContext _db;
    private readonly IEventDigest _digest;

    public CleaningEventWriter(VakaDbContext db, IEventDigest digest)
    {
        _db = db;
        _digest = digest;
    }

    public async Task AppendAsync(EventDraft draft, CancellationToken cancellationToken)
    {
        var last = await _db.CleaningEvents
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == draft.RecordId)
            .OrderByDescending(item => item.Sequence)
            .Select(item => new { item.Sequence, item.Hash })
            .FirstOrDefaultAsync(cancellationToken);

        var sequence = (last?.Sequence ?? 0) + 1;
        var payload = JsonSerializer.Serialize(draft.Payload);
        var commandId = draft.CommandId?.ToString("D");
        var canonical = string.Join(
            '|',
            draft.RecordId.ToString(CultureInfo.InvariantCulture),
            sequence.ToString(CultureInfo.InvariantCulture),
            draft.EventType,
            draft.ActorUserId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            draft.OccurredAtUtc.ToString(MomentFormat, CultureInfo.InvariantCulture),
            commandId ?? string.Empty,
            payload);

        _db.CleaningEvents.Add(new CleaningEvent
        {
            CleaningRecordId = draft.RecordId,
            Sequence = sequence,
            EventType = draft.EventType,
            PayloadJson = payload,
            ActorUserId = draft.ActorUserId,
            OccurredAtUtc = draft.OccurredAtUtc,
            CommandId = commandId,
            PreviousHash = last?.Hash,
            Hash = _digest.Compute(last?.Hash, canonical)
        });
        await _db.SaveChangesAsync(cancellationToken);
    }
}

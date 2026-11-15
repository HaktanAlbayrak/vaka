using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;

namespace Vaka.Web.Services;

public sealed class RecordIntegrity : IRecordIntegrity
{
    private readonly VakaDbContext _db;
    private readonly IEventDigest _digest;
    private readonly IRecordStateAudit _state;

    public RecordIntegrity(VakaDbContext db, IEventDigest digest, IRecordStateAudit state)
    {
        _db = db;
        _digest = digest;
        _state = state;
    }

    public async Task<IntegrityResult?> CheckAsync(int recordId, CancellationToken cancellationToken)
    {
        if (!await _db.CleaningRecords.AsNoTracking().AnyAsync(record => record.Id == recordId, cancellationToken))
        {
            return null;
        }

        var rows = await _db.CleaningEvents
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId)
            .OrderBy(item => item.Sequence)
            .Select(item => new { item.Sequence, item.EventType, item.PayloadJson, item.ActorUserId, item.OccurredAtUtc, item.CommandId, item.PreviousHash, item.Hash })
            .ToListAsync(cancellationToken);

        var checks = new List<EventCheck>(rows.Count);
        var parsed = new List<ParsedEvent>(rows.Count);
        string? previousHash = null;
        var expectedSequence = 1;
        foreach (var row in rows)
        {
            string? problem = null;
            if (row.Sequence != expectedSequence)
            {
                problem = $"Sıra boşluğu: {expectedSequence}. olay bulunamadı.";
            }
            else if (row.PreviousHash != previousHash)
            {
                problem = "Önceki özet, bir önceki olayın özetiyle uyuşmuyor.";
            }
            else
            {
                var canonical = EventCanonical.Build(recordId, row.Sequence, row.EventType, row.ActorUserId, row.OccurredAtUtc, row.CommandId, row.PayloadJson);
                if (_digest.Compute(row.PreviousHash, canonical) != row.Hash)
                {
                    problem = "Özet, olaydaki değerlerle uyuşmuyor.";
                }
            }

            checks.Add(new EventCheck(row.Sequence, row.EventType, row.OccurredAtUtc, row.Hash, problem is null, problem));
            parsed.Add(ParsedEvent.From(row.Sequence, row.EventType, row.ActorUserId, row.OccurredAtUtc, row.PayloadJson));
            previousHash = row.Hash;
            expectedSequence = row.Sequence + 1;
        }

        var mismatches = await _state.FindMismatchesAsync(recordId, parsed, cancellationToken);
        var broken = checks.Where(check => !check.IsValid).Select(check => check.Sequence)
            .Concat(mismatches.Where(item => item.Sequence > 0).Select(item => item.Sequence))
            .ToArray();

        return new IntegrityResult(
            broken.Length == 0 && mismatches.Count == 0,
            broken.Length == 0 ? null : broken.Min(),
            checks,
            mismatches);
    }
}

namespace Vaka.Web.Services;

/// <summary>Kayıt malzemeleri ve molalarının olay günlüğüyle uyumunu denetler (PL-11); <see cref="RecordStateRules"/> ile aynı ilkedir.</summary>
public static class ItemStateRules
{
    private sealed record ExpectedLot(int LotId, string? LotNo, bool? ExpiredAtEntry, string? ExpiredReason, int Sequence);

    public static void Compare(StateFacts facts, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        CompareMaterials(facts.Materials, events, found);
        foreach (var item in facts.Breaks)
        {
            CompareBreak(item, events, found);
        }
    }

    private static void CompareMaterials(MaterialFacts[] stored, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        var expected = ExpectedLots(events);
        foreach (var material in stored)
        {
            var label = $"Malzeme, lot {material.LotNo}: ";
            if (!expected.TryGetValue(material.MaterialLotId, out var lot))
            {
                found.Add(label + "kayıt", "Var", "Olay yok", 0);
                continue;
            }

            if (lot.LotNo is not null)
            {
                found.Text(label + "lot no", material.LotNo, lot.LotNo, lot.Sequence);
                found.Text(label + "SKT geçmiş", material.ExpiredAtEntry ? "Evet" : "Hayır", lot.ExpiredAtEntry == true ? "Evet" : "Hayır", lot.Sequence);
                found.Text(label + "SKT gerekçesi", material.ExpiredReason, lot.ExpiredReason, lot.Sequence);
            }
        }

        var storedIds = stored.Select(material => material.MaterialLotId).ToHashSet();
        foreach (var lot in expected.Values.Where(lot => !storedIds.Contains(lot.LotId)))
        {
            found.Add($"Malzeme, lot {lot.LotNo ?? lot.LotId.ToString()}: kayıt", "Yok", "Olayda var", lot.Sequence);
        }
    }

    private static Dictionary<int, ExpectedLot> ExpectedLots(IReadOnlyList<ParsedEvent> events)
    {
        var expected = new Dictionary<int, ExpectedLot>();
        foreach (var item in events)
        {
            switch (item.EventType)
            {
                case EventTypes.RecordOpened:
                    foreach (var lot in item.Items("MaterialLots"))
                    {
                        var id = ParsedEvent.NumberOf(lot, "LotId") ?? 0;
                        expected[id] = new ExpectedLot(
                            id,
                            ParsedEvent.TextOf(lot, "LotNo"),
                            ParsedEvent.FlagOf(lot, "ExpiredAtEntry"),
                            ParsedEvent.TextOf(lot, "ExpiredReason"),
                            item.Sequence);
                    }

                    if (!item.Has("MaterialLots"))
                    {
                        foreach (var id in item.Numbers("MaterialLotIds") ?? [])
                        {
                            expected[id] = new ExpectedLot(id, null, null, null, item.Sequence);
                        }
                    }

                    break;
                case EventTypes.MaterialAdded when item.Number("MaterialLotId") is { } added:
                    expected[added] = new ExpectedLot(added, item.Text("LotNo"), item.Flag("ExpiredAtEntry"), item.Text("ExpiredReason"), item.Sequence);
                    break;
                case EventTypes.MaterialRemoved when item.Number("MaterialLotId") is { } removed:
                    expected.Remove(removed);
                    break;
            }
        }

        return expected;
    }

    private static void CompareBreak(BreakFacts item, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        var label = $"Mola {item.Id}: ";
        var started = events.FirstOrDefault(entry => entry.EventType == EventTypes.BreakStarted && entry.Number("BreakId") == item.Id);
        var ended = events.FirstOrDefault(entry => entry.EventType == EventTypes.BreakEnded && entry.Number("BreakId") == item.Id);

        found.Moment(label + "başlangıç anı", item.StartedAtUtc, started?.OccurredAtUtc, started?.Sequence ?? 0);
        found.User(label + "başlatan", item.StartedByUserId, started?.ActorUserId, started?.Sequence ?? 0);
        found.Moment(label + "bitiş anı", item.EndedAtUtc, ended?.OccurredAtUtc, ended?.Sequence ?? 0);
        found.User(label + "bitiren", item.EndedByUserId, ended?.ActorUserId, ended?.Sequence ?? 0);
        found.Text(label + "kapanış türü", item.EndKind?.ToString(), ended?.Text("EndKind"), ended?.Sequence ?? 0);
    }
}

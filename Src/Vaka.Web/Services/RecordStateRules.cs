namespace Vaka.Web.Services;

/// <summary>
/// Kayıt, adım ve faz tablolarındaki zaman, kişi ve değer alanlarının olay günlüğündeki karşılıklarıyla aynı olduğunu denetler (PL-11, SC-21).
/// Olay günlüğü özet zinciriyle korunur; durum tabloları değiştirilirse fark buradan çıkar.
/// </summary>
public static class RecordStateRules
{
    public static void Compare(StateFacts facts, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        CompareRecord(facts.Record, events, found);
        foreach (var step in facts.Steps)
        {
            CompareStep(step, events, found);
        }

        foreach (var phase in facts.Phases)
        {
            ComparePhase(phase, events, found);
        }
    }

    private static void CompareRecord(RecordFacts record, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        var opened = events.FirstOrDefault(item => item.EventType == EventTypes.RecordOpened);
        if (opened is null)
        {
            found.Add("Kayıt açılış olayı", record.RecordNo, "Olay yok", 0);
        }
        else
        {
            found.Text("Kayıt no", record.RecordNo, opened.Text("RecordNo"), opened.Sequence);
            found.Number("Makine", record.MachineId, opened.Number("MachineId"), opened.Sequence);
            found.Number("Prosedür versiyonu", record.ProcedureVersionId, opened.Number("ProcedureVersionId"), opened.Sequence);
            found.Text("İş emri no", record.WorkOrderNo, opened.Text("WorkOrderNo"), opened.Sequence);
            found.User("Kaydı açan", record.OwnerUserId, opened.ActorUserId, opened.Sequence);
            found.Moment("Açılış anı", record.OpenedAtUtc, opened.OccurredAtUtc, opened.Sequence);
        }

        var firstStart = events.FirstOrDefault(item => item.EventType == EventTypes.StepStarted && item.Flag("FirstStep") == true);
        found.Moment("Başlangıç anı", record.StartedAtUtc, firstStart?.OccurredAtUtc, firstStart?.Sequence ?? 0);

        var completed = events.FirstOrDefault(item => item.EventType == EventTypes.RecordCompleted);
        found.Moment("Tamamlanma anı", record.CompletedAtUtc, completed?.OccurredAtUtc, completed?.Sequence ?? 0);

        CompareClosure(record, events, found);
        found.Text("Durum", RecordLabels.Status(record.Status), RecordLabels.Status(DeriveStatus(events)), events.Count == 0 ? 0 : events[^1].Sequence);
    }

    private static void CompareClosure(RecordFacts record, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        var closing = events.FirstOrDefault(item => item.EventType is EventTypes.RecordCancelled or EventTypes.RecordAborted);
        var timedOut = events.FirstOrDefault(item => item.EventType == EventTypes.RecordTimedOut);
        var expectedClosedAt = closing?.OccurredAtUtc ?? timedOut?.Moment("lockExpiresAtUtc");
        var sequence = closing?.Sequence ?? timedOut?.Sequence ?? 0;

        found.Moment("Kapanış anı", record.ClosedAtUtc, expectedClosedAt, sequence);
        found.User("Kapatan", record.ClosedByUserId, closing?.ActorUserId, closing?.Sequence ?? 0);
        found.Text("Kapanış gerekçesi", record.ClosureReason, closing?.Text("Reason"), closing?.Sequence ?? 0);
        found.Number("Kapanış türü", record.ClosureKind, closing?.Number("ClosureKind"), closing?.Sequence ?? 0);
    }

    private static RecordStatus DeriveStatus(IReadOnlyList<ParsedEvent> events)
    {
        var status = RecordStatus.Opened;
        foreach (var item in events)
        {
            status = item.EventType switch
            {
                EventTypes.StepStarted => RecordStatus.InProgress,
                EventTypes.RecordCompleted => RecordStatus.Completed,
                EventTypes.RecordTimedOut => RecordStatus.TimedOut,
                EventTypes.RecordCancelled => RecordStatus.Cancelled,
                EventTypes.RecordAborted => RecordStatus.Aborted,
                _ => status
            };
        }

        return status;
    }

    private static void CompareStep(StepFacts step, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        var label = $"Faz {step.PhaseOrder}, adım {step.StepOrder} ({step.Title}): ";
        var started = events.FirstOrDefault(item => item.EventType == EventTypes.StepStarted && item.Number("StepId") == step.Id);
        var ended = events.FirstOrDefault(item => item.EventType is EventTypes.StepFinished or EventTypes.StepInterrupted && item.Number("StepId") == step.Id);

        found.Moment(label + "başlangıç anı", step.StartedAtUtc, started?.OccurredAtUtc, started?.Sequence ?? 0);
        found.User(label + "başlatan", step.StartedByUserId, started?.ActorUserId, started?.Sequence ?? 0);
        if (started is not null)
        {
            found.Users(label + "görevliler", step.ParticipantUserIds, started.Numbers("ParticipantUserIds"), started.Sequence);
        }

        found.Moment(label + "bitiş anı", step.EndedAtUtc, ended?.OccurredAtUtc, ended?.Sequence ?? 0);
        found.User(label + "bitiren", step.EndedByUserId, ended?.ActorUserId, ended?.Sequence ?? 0);
        found.Number(label + "süre", step.DurationSeconds, ended?.Number("DurationSeconds"), ended?.Sequence ?? 0);
    }

    private static void ComparePhase(PhaseFacts phase, IReadOnlyList<ParsedEvent> events, StateMismatchCollector found)
    {
        var label = $"Faz {phase.OrderNo}: ";
        var evaluated = events.FirstOrDefault(item => item.EventType == EventTypes.PhaseCompleted && item.Number("PhaseId") == phase.Id);
        var isCompleted = phase.Status is PhaseStatus.Completed or PhaseStatus.CompletedBelowMinimum;
        if (evaluated is null)
        {
            if (isCompleted)
            {
                found.Add(label + "değerlendirme", RecordLabels.Phase(phase.Status), "Olay yok", 0);
            }

            return;
        }

        var eventStatus = evaluated.Number("Status") is { } value && Enum.IsDefined((PhaseStatus)value) ? RecordLabels.Phase((PhaseStatus)value) : "Bilinmiyor";
        found.Text(label + "durum", RecordLabels.Phase(phase.Status), eventStatus, evaluated.Sequence);
        found.Text(label + "süre altı", phase.BelowMinimum ? "Evet" : "Hayır", evaluated.Flag("BelowMinimum") == true ? "Evet" : "Hayır", evaluated.Sequence);
        found.Text(label + "süre altı gerekçesi", phase.BelowMinimumReason, evaluated.Text("Reason"), evaluated.Sequence);
        found.Number(label + "net süre", phase.NetSeconds, evaluated.Number("NetSeconds"), evaluated.Sequence);
    }
}

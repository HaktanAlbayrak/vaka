namespace Vaka.Web.Services;

/// <summary>Olay günlüğü satırının görünür adı ve özeti. Bilinmeyen tür (ileri fazların olayı) kendi adıyla gösterilir.</summary>
public static class RecordEventText
{
    public static string Label(string eventType) => eventType switch
    {
        EventTypes.RecordOpened => "Kayıt açıldı",
        EventTypes.RecordTimedOut => "Zaman aşımı",
        EventTypes.MaterialAdded => "Malzeme eklendi",
        EventTypes.MaterialRemoved => "Malzeme çıkarıldı",
        EventTypes.StepStarted => "Adım başladı",
        EventTypes.StepFinished => "Adım bitti",
        EventTypes.PhaseCompleted => "Faz değerlendirildi",
        EventTypes.RecordCompleted => "Kayıt tamamlandı",
        EventTypes.ParticipantAdded => "Görevli eklendi",
        EventTypes.ParticipantRemoved => "Görevli çıkarıldı",
        EventTypes.BreakStarted => "Mola başladı",
        EventTypes.BreakEnded => "Mola bitti",
        EventTypes.StepInterrupted => "Adım kesildi",
        EventTypes.RecordCancelled => "Kayıt iptal edildi",
        EventTypes.RecordAborted => "Kayıt yarıda kapatıldı",
        _ => eventType
    };

    /// <summary>Yükteki değerlerden tek satırlık özet; kişi ve adım adları çağıranın sözlüklerinden gelir, süreler saklanan saniyelerdir.</summary>
    public static string Summary(ParsedEvent item, IReadOnlyDictionary<int, string> users, IReadOnlyDictionary<int, string> steps, Func<DateTime, string> local)
    {
        string User(string key) => item.Number(key) is { } id ? users.GetValueOrDefault(id) ?? $"Kullanıcı {id}" : "-";
        string Step() => item.Number("StepId") is { } id ? steps.GetValueOrDefault(id) ?? $"Adım {id}" : "-";
        string BreakEnd() => Enum.TryParse<BreakEndKind>(item.Text("EndKind"), out var kind) ? RecordLabels.BreakEnd(kind) : "-";
        string WithReason(string text) => item.Text("Reason") is { Length: > 0 } reason ? $"{text}. Gerekçe: {reason}" : text;

        return item.EventType switch
        {
            EventTypes.RecordOpened => $"Kayıt no {item.Text("RecordNo")}, versiyon {item.Number("VersionNo")}" + (item.Text("WorkOrderNo") is { Length: > 0 } order ? $", iş emri {order}" : string.Empty),
            EventTypes.RecordTimedOut => item.Moment("lockExpiresAtUtc") is { } expires ? $"Kilit bitişi {local(expires)}" : "Kilit süresi doldu",
            EventTypes.MaterialAdded => $"Lot {item.Text("LotNo")}" + (item.Flag("ExpiredAtEntry") == true ? $", SKT geçmiş: {item.Text("ExpiredReason")}" : string.Empty),
            EventTypes.MaterialRemoved => $"Lot {item.Text("LotNo")}",
            EventTypes.StepStarted => item.Text("Title") + (item.Text("FieldBookRef") is { Length: > 0 } reference && item.Flag("FirstStep") == true ? $", saha referansı {reference}" : string.Empty),
            EventTypes.StepFinished => $"{Step()}, süre {item.Number("DurationSeconds")} sn, efor {item.Number("EffortSeconds")} sn",
            EventTypes.StepInterrupted => $"{Step()}, süre {item.Number("DurationSeconds")} sn",
            EventTypes.PhaseCompleted => $"Net {item.Number("NetSeconds")} sn" + (item.Flag("BelowMinimum") == true ? $", süre altı: {item.Text("Reason")}" : string.Empty),
            EventTypes.RecordCompleted => $"Brüt {item.Number("GrossSeconds")} sn, mola {item.Number("BreakSeconds")} sn, net {item.Number("NetSeconds")} sn, efor {item.Number("EffortSeconds")} sn",
            EventTypes.ParticipantAdded => $"{User("ParticipantUserId")}, {Step()}",
            EventTypes.ParticipantRemoved => $"{User("ParticipantUserId")}, {Step()}",
            EventTypes.BreakStarted => "Mola açıldı",
            EventTypes.BreakEnded => WithReason($"{BreakEnd()}, süre {item.Number("DurationSeconds")} sn"),
            EventTypes.RecordCancelled or EventTypes.RecordAborted => WithReason(item.Number("InterruptedStepId") is { } interrupted ? $"Kesilen adım: {steps.GetValueOrDefault(interrupted) ?? $"Adım {interrupted}"}" : "Devam eden adım yoktu"),
            _ => string.Empty
        };
    }
}

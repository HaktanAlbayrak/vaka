using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordTimelineBuilder : IRecordTimelineBuilder
{
    private const string SystemName = "Sistem";

    private readonly ITurkeyClock _clock;

    public RecordTimelineBuilder(ITurkeyClock clock)
    {
        _clock = clock;
    }

    /// <remarks>
    /// Süre alanları olay yükündeki saklı saniyelerdir: adım bitişinde DurationSeconds ve EffortSeconds, faz tamamlanmasında NetSeconds,
    /// kayıt tamamlanmasında NetSeconds ve EffortSeconds, mola bitişinde DurationSeconds. Burada süre hesaplanmaz.
    /// </remarks>
    public RecordTimelineDayViewModel[] Build(IReadOnlyList<ParsedEvent> events, TimelineNames names)
    {
        var entries = events
            .OrderBy(item => item.Sequence)
            .Select(item => Entry(item, names))
            .ToList();

        return [.. entries
            .GroupBy(entry => DateOnly.FromDateTime(entry.OccurredAtLocal))
            .OrderBy(group => group.Key)
            .Select(group => new RecordTimelineDayViewModel { DateLocal = group.Key, Entries = [.. group] })];
    }

    private RecordTimelineEntryViewModel Entry(ParsedEvent item, TimelineNames names)
    {
        string User(int? id) => id is { } value ? names.Users.GetValueOrDefault(value) ?? $"Kullanıcı {value}" : SystemName;
        string? Step(string key) => item.Number(key) is { } id ? names.StepLabels.GetValueOrDefault(id) ?? $"Adım {id}" : null;
        var actor = User(item.ActorUserId);
        var reason = item.Text("Reason") is { Length: > 0 } text ? text : null;
        var entry = new RecordTimelineEntryViewModel
        {
            Sequence = item.Sequence,
            OccurredAtLocal = _clock.ToLocal(item.OccurredAtUtc),
            Kind = TimelineEntryKind.Other,
            Title = RecordEventText.Label(item.EventType),
            StepLabel = Step("StepId")
        };

        switch (item.EventType)
        {
            case EventTypes.RecordOpened:
                entry.Kind = TimelineEntryKind.Opened;
                entry.Title = $"{actor} kaydı açtı";
                entry.Detail = $"Kayıt no {item.Text("RecordNo")}" + (item.Text("WorkOrderNo") is { Length: > 0 } workOrder ? $", iş emri {workOrder}" : string.Empty);
                break;

            case EventTypes.StepStarted:
                var started = ParticipantNames(item, names, item.ActorUserId);
                entry.Kind = TimelineEntryKind.StepStarted;
                entry.Title = started.Length <= 1 ? $"{(started.Length == 1 ? started[0] : actor)} adıma başladı" : $"{JoinNames(started)} birlikte adıma başladı";
                entry.Detail = Compose(
                    item.ActorUserId is { } starter && item.Numbers("ParticipantUserIds") is { Length: > 0 } members && !members.Contains(starter) ? $"Başlatan: {actor}" : null,
                    item.Flag("FirstStep") == true && item.Text("FieldBookRef") is { Length: > 0 } reference ? $"Saha referansı {reference}" : null);
                break;

            case EventTypes.StepFinished:
                var finished = ParticipantNames(item, names, item.ActorUserId);
                entry.Kind = TimelineEntryKind.StepFinished;
                entry.Title = $"{actor} adımı bitirdi";
                entry.Detail = finished.Length >= 2 ? $"Görevliler: {string.Join(", ", finished.Select((name, index) => WorkText(name, item.Numbers("ParticipantWorkSeconds"), index)))}" : null;
                entry.DurationSeconds = item.Number("DurationSeconds");
                entry.EffortSeconds = item.Number("EffortSeconds");
                break;

            case EventTypes.StepInterrupted:
                entry.Kind = TimelineEntryKind.StepInterrupted;
                entry.Title = "Adım kesildi";
                entry.DurationSeconds = item.Number("DurationSeconds");
                entry.EffortSeconds = item.Number("EffortSeconds");
                break;

            case EventTypes.PhaseCompleted:
                var below = item.Flag("BelowMinimum") == true;
                var phaseLabel = item.Number("PhaseId") is { } phaseId && names.PhaseOrders.TryGetValue(phaseId, out var number) ? $"Faz {number}" : "Faz";
                entry.Kind = TimelineEntryKind.PhaseCompleted;
                entry.Title = below ? $"{phaseLabel} süre altında tamamlandı" : $"{phaseLabel} tamamlandı";
                entry.Detail = below && reason is not null ? $"Gerekçe: {reason}" : null;
                entry.DurationSeconds = item.Number("NetSeconds");
                entry.IsWarning = below;
                break;

            case EventTypes.RecordCompleted:
                entry.Kind = TimelineEntryKind.Completed;
                entry.Title = "Kayıt tamamlandı";
                entry.DurationSeconds = item.Number("NetSeconds");
                entry.EffortSeconds = item.Number("EffortSeconds");
                break;

            case EventTypes.BreakStarted:
                entry.Kind = TimelineEntryKind.BreakStarted;
                entry.Title = $"{actor} mola başlattı";
                break;

            case EventTypes.BreakEnded:
                var byPerson = item.Text("EndKind") == BreakEndKind.Person.ToString();
                entry.Kind = TimelineEntryKind.BreakEnded;
                entry.Title = byPerson ? $"{actor} molayı bitirdi" : $"{actor} molayı yönetici olarak bitirdi";
                entry.Detail = reason is null ? null : $"Gerekçe: {reason}";
                entry.DurationSeconds = item.Number("DurationSeconds");
                break;

            case EventTypes.MaterialAdded:
                var expired = item.Flag("ExpiredAtEntry") == true;
                entry.Kind = TimelineEntryKind.MaterialAdded;
                entry.Title = $"{actor} malzeme ekledi";
                entry.Detail = $"Lot {item.Text("LotNo")}" + (expired ? $", SKT geçmiş: {item.Text("ExpiredReason")}" : string.Empty);
                entry.IsWarning = expired;
                break;

            case EventTypes.MaterialRemoved:
                entry.Kind = TimelineEntryKind.MaterialRemoved;
                entry.Title = $"{actor} malzeme çıkardı";
                entry.Detail = $"Lot {item.Text("LotNo")}";
                break;

            case EventTypes.ParticipantAdded:
                entry.Kind = TimelineEntryKind.ParticipantAdded;
                entry.Title = $"{actor} görevli ekledi: {User(item.Number("ParticipantUserId"))}";
                break;

            case EventTypes.ParticipantRemoved:
                entry.Kind = TimelineEntryKind.ParticipantRemoved;
                entry.Title = $"{actor} görevli çıkardı: {User(item.Number("ParticipantUserId"))}";
                break;

            case EventTypes.ParticipantJoined:
                var joined = User(item.Number("ParticipantUserId"));
                entry.Kind = TimelineEntryKind.ParticipantJoined;
                entry.Title = item.Flag("BeforeStart") == true ? $"{joined} adıma katılacağını onayladı" : $"{joined} adıma katıldı";
                entry.Detail = SessionDetail(item, names);
                break;

            case EventTypes.ParticipantLeft:
                entry.Kind = TimelineEntryKind.ParticipantLeft;
                entry.Title = $"{User(item.Number("ParticipantUserId"))} adımdan ayrıldı";
                entry.Detail = SessionDetail(item, names);
                entry.DurationSeconds = item.Number("WorkSeconds");
                break;

            case EventTypes.RecordCancelled:
            case EventTypes.RecordAborted:
                var cancelled = item.EventType == EventTypes.RecordCancelled;
                entry.Kind = cancelled ? TimelineEntryKind.Cancelled : TimelineEntryKind.Aborted;
                entry.Title = cancelled ? $"{actor} kaydı iptal etti" : $"{actor} kaydı yarıda kapattı";
                entry.Detail = reason is null ? null : $"Gerekçe: {reason}";
                entry.StepLabel = Step("InterruptedStepId");
                entry.IsWarning = true;
                break;

            case EventTypes.RecordTimedOut:
                entry.Kind = TimelineEntryKind.TimedOut;
                entry.Title = "Kilit süresi doldu, kayıt zaman aşımına düştü";
                entry.Detail = item.Moment("lockExpiresAtUtc") is { } expires ? $"Kilit bitişi {_clock.ToLocal(expires):dd.MM.yyyy HH:mm:ss}" : null;
                entry.IsWarning = true;
                break;
        }

        return entry;
    }

    private static string WorkText(string name, int[]? seconds, int index) =>
        seconds is not null && index < seconds.Length ? $"{name} ({Minutes(seconds[index])})" : name;

    private static string Minutes(int seconds) => seconds < 60 ? $"{seconds} sn" : $"{seconds / 60} dk {seconds % 60} sn";

    private static string? SessionDetail(ParsedEvent item, TimelineNames names) =>
        item.Number("SessionUserId") is { } session && session != item.Number("ParticipantUserId")
            ? $"Cihazdaki oturum: {names.Users.GetValueOrDefault(session) ?? $"Kullanıcı {session}"}"
            : null;

    private static string[] ParticipantNames(ParsedEvent item, TimelineNames names, int? fallbackUserId)
    {
        var ids = item.Numbers("ParticipantUserIds") is { Length: > 0 } members ? members : fallbackUserId is { } id ? [id] : [];
        return [.. ids.Select(id => names.Users.GetValueOrDefault(id) ?? $"Kullanıcı {id}")];
    }

    private static string JoinNames(string[] values) => values.Length switch
    {
        0 => SystemName,
        1 => values[0],
        _ => $"{string.Join(", ", values[..^1])} ve {values[^1]}"
    };

    private static string? Compose(params string?[] parts)
    {
        var present = parts.Where(part => part is { Length: > 0 }).ToArray();
        return present.Length == 0 ? null : string.Join(". ", present);
    }
}

namespace Vaka.Web.Services;

/// <summary>Zaman çizelgesi satırının türü; görünüm ikon ve tonu bu değerden seçer.</summary>
public enum TimelineEntryKind
{
    Opened,
    TimedOut,
    MaterialAdded,
    MaterialRemoved,
    ParticipantAdded,
    ParticipantRemoved,
    ParticipantJoined,
    ParticipantLeft,
    StepStarted,
    StepFinished,
    StepInterrupted,
    BreakStarted,
    BreakEnded,
    PhaseCompleted,
    Completed,
    Cancelled,
    Aborted,
    Other
}

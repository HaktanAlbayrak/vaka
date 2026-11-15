namespace Vaka.Web.Services;

public enum PhaseStatus
{
    Waiting = 1,
    InProgress = 2,
    Completed = 3,
    CompletedBelowMinimum = 4,
    Interrupted = 5
}

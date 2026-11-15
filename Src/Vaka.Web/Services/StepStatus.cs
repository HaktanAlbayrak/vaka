namespace Vaka.Web.Services;

/// <summary>"Hazır" saklanmaz; ekranda <c>IsNext</c> ile türetilir.</summary>
public enum StepStatus
{
    Waiting = 1,
    InProgress = 2,
    Completed = 3,
    Interrupted = 4
}

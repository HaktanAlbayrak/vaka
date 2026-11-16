using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordTimelineEntryViewModel
{
    public int Sequence { get; set; }

    public DateTime OccurredAtLocal { get; set; }

    public TimelineEntryKind Kind { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? StepLabel { get; set; }

    public string? Detail { get; set; }

    /// <summary>Olay yükünde saklanan süre, saniye; görünüm <c>_Duration</c> ile biçimler.</summary>
    public int? DurationSeconds { get; set; }

    public int? EffortSeconds { get; set; }

    public bool IsWarning { get; set; }
}

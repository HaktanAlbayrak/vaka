namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailBreakViewModel
{
    public string StartedByName { get; set; } = string.Empty;

    public DateTime StartedAtLocal { get; set; }

    public DateTime? EndedAtLocal { get; set; }

    public int? DurationSeconds { get; set; }

    public string EndKindLabel { get; set; } = string.Empty;

    public string? EndedByName { get; set; }

    public string EndReason { get; set; } = string.Empty;

    public string? DuringStepTitle { get; set; }
}

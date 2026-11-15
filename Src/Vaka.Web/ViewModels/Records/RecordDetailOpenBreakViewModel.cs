namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailOpenBreakViewModel
{
    public int BreakId { get; set; }

    public string StartedByName { get; set; } = string.Empty;

    public DateTime StartedAtLocal { get; set; }

    public string? DuringStepTitle { get; set; }
}

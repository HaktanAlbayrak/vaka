namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailPhaseViewModel
{
    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? MinDurationSeconds { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public int? GrossSeconds { get; set; }

    public int? BreakSeconds { get; set; }

    public int? NetSeconds { get; set; }

    public bool BelowMinimum { get; set; }

    public string BelowMinimumReason { get; set; } = string.Empty;

    public RecordDetailStepViewModel[] Steps { get; set; } = [];
}

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailSummaryViewModel
{
    public int? GrossSeconds { get; set; }

    public int? BreakSeconds { get; set; }

    public int? HoldSeconds { get; set; }

    public int? NetSeconds { get; set; }

    public int? EffortSeconds { get; set; }
}

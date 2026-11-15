namespace Vaka.Web.ViewModels.Reports;

public sealed class PhaseDurationRowViewModel
{
    public string CleaningTypeName { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public int PhaseOrderNo { get; set; }

    public string PhaseTitle { get; set; } = string.Empty;

    public int? MinDurationSeconds { get; set; }

    public int RecordCount { get; set; }

    public int AvgNetSeconds { get; set; }

    public int BelowMinimumCount { get; set; }
}

namespace Vaka.Web.ViewModels.Reports;

public sealed class LongestStepRowViewModel
{
    public string CleaningTypeName { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public string PhaseTitle { get; set; } = string.Empty;

    public string StepTitle { get; set; } = string.Empty;

    public int RecordCount { get; set; }

    public int AvgDurationSeconds { get; set; }

    public int MaxDurationSeconds { get; set; }
}

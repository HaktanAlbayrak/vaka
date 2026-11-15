namespace Vaka.Web.ViewModels.Reports;

public sealed class MachineDurationRowViewModel
{
    public string MachineLabel { get; set; } = string.Empty;

    public string CleaningTypeName { get; set; } = string.Empty;

    public int RecordCount { get; set; }

    public int AvgGrossSeconds { get; set; }

    public int AvgBreakSeconds { get; set; }

    public int AvgNetSeconds { get; set; }

    public int AvgEffortSeconds { get; set; }
}

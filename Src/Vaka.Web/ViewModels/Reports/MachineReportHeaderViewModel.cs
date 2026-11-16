namespace Vaka.Web.ViewModels.Reports;

public sealed class MachineReportHeaderViewModel
{
    public string Label { get; set; } = string.Empty;

    public string LineLabel { get; set; } = string.Empty;

    public string FacilityLabel { get; set; } = string.Empty;

    public DateTime? RetiredAtLocal { get; set; }
}

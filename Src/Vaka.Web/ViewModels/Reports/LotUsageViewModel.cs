namespace Vaka.Web.ViewModels.Reports;

public sealed class LotUsageViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public DateTime OpenedAtLocal { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public bool ExpiredAtEntry { get; set; }

    public string Url { get; set; } = string.Empty;
}

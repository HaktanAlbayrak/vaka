namespace Vaka.Web.ViewModels.Reports;

public sealed class MaterialUsageRowViewModel
{
    public string MaterialCode { get; set; } = string.Empty;

    public string MaterialName { get; set; } = string.Empty;

    public int RecordCount { get; set; }

    /// <summary>Yüzde, bir ondalık (ör. 66,7).</summary>
    public decimal UsageRatePercent { get; set; }

    public int LotCount { get; set; }

    public int ExpiredUseCount { get; set; }
}

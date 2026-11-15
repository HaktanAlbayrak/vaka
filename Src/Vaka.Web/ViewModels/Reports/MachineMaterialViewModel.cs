namespace Vaka.Web.ViewModels.Reports;

public sealed class MachineMaterialViewModel
{
    public DateTime EnteredAtLocal { get; set; }

    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string MaterialCode { get; set; } = string.Empty;

    public string MaterialName { get; set; } = string.Empty;

    public string LotNo { get; set; } = string.Empty;

    public DateOnly ExpiryDate { get; set; }

    public bool ExpiredAtEntry { get; set; }

    public string Url { get; set; } = string.Empty;
}

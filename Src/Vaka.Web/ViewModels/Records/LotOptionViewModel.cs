namespace Vaka.Web.ViewModels.Records;

public sealed class LotOptionViewModel
{
    public int Id { get; set; }

    public string MaterialCode { get; set; } = string.Empty;

    public string MaterialName { get; set; } = string.Empty;

    public string LotNo { get; set; } = string.Empty;

    public DateOnly ExpiryDate { get; set; }

    public bool IsExpired { get; set; }
}

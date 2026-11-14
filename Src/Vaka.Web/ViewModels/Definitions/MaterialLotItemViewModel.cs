namespace Vaka.Web.ViewModels.Definitions;

public sealed class MaterialLotItemViewModel
{
    public int Id { get; set; }

    public string LotNo { get; set; } = string.Empty;

    public DateOnly ExpiryDate { get; set; }

    public bool IsExpired { get; set; }
}

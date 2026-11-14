namespace Vaka.Web.ViewModels.Definitions;

public sealed class MaterialLotFormViewModel
{
    public int? Id { get; set; }

    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = string.Empty;

    public string LotNo { get; set; } = string.Empty;

    public DateOnly? ExpiryDate { get; set; }
}

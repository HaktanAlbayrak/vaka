namespace Vaka.Web.Domain.Entities;

public class MaterialLot
{
    public int Id { get; set; }
    public int MaterialId { get; set; }
    public string LotNo { get; set; } = string.Empty;
    public DateOnly ExpiryDate { get; set; }

    public Material Material { get; set; } = null!;
}

namespace Vaka.Web.Domain.Entities;

public class Material
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<MaterialLot> Lots { get; set; } = new List<MaterialLot>();
}

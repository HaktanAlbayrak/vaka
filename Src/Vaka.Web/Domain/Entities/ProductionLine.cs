namespace Vaka.Web.Domain.Entities;

public class ProductionLine
{
    public int Id { get; set; }
    public int FacilityId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public Facility Facility { get; set; } = null!;
    public ICollection<Machine> Machines { get; set; } = new List<Machine>();
}

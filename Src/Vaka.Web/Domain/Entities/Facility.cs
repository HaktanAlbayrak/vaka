namespace Vaka.Web.Domain.Entities;

public class Facility
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ProductionLine> Lines { get; set; } = new List<ProductionLine>();
    public ICollection<Shift> Shifts { get; set; } = new List<Shift>();
}

namespace Vaka.Web.Domain.Entities;

public class Machine
{
    public int Id { get; set; }
    public int ProductionLineId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? RetiredAtUtc { get; set; }

    public ProductionLine Line { get; set; } = null!;
}

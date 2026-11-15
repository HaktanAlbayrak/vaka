namespace Vaka.Web.Domain.Entities;

public class ProcedureStep
{
    public int Id { get; set; }
    public int ProcedurePhaseId { get; set; }
    public int OrderNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    public ProcedurePhase Phase { get; set; } = null!;
    public ICollection<StepMedia> Media { get; set; } = new List<StepMedia>();
}

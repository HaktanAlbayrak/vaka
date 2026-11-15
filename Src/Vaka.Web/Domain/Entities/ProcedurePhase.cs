namespace Vaka.Web.Domain.Entities;

public class ProcedurePhase
{
    public int Id { get; set; }
    public int ProcedureVersionId { get; set; }
    public int OrderNo { get; set; }
    public string Title { get; set; } = string.Empty;
    public int? MinDurationSeconds { get; set; }

    public ProcedureVersion Version { get; set; } = null!;
    public ICollection<ProcedureStep> Steps { get; set; } = new List<ProcedureStep>();
}

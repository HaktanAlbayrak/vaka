using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class StepMedia
{
    public int Id { get; set; }
    public int ProcedureStepId { get; set; }
    public StepMediaType MediaType { get; set; }
    public string Sha256 { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }

    public ProcedureStep Step { get; set; } = null!;
}

using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class ProcedureVersion
{
    public int Id { get; set; }
    public int CleaningTypeId { get; set; }
    public int VersionNo { get; set; }
    public ProcedureVersionStatus Status { get; set; }
    public bool MaterialRequired { get; set; }
    public string? DocumentRef { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public int? PublishedByUserId { get; set; }

    public CleaningType CleaningType { get; set; } = null!;
    public User? PublishedBy { get; set; }
    public ICollection<ProcedurePhase> Phases { get; set; } = new List<ProcedurePhase>();
}

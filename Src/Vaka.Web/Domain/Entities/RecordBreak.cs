using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class RecordBreak
{
    public int Id { get; set; }
    public int CleaningRecordId { get; set; }
    public int StartedByUserId { get; set; }
    public int? DuringStepId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public int? EndedByUserId { get; set; }
    public BreakEndKind? EndKind { get; set; }
    public string? EndReason { get; set; }

    public CleaningRecord Record { get; set; } = null!;
    public User StartedBy { get; set; } = null!;
    public User? EndedBy { get; set; }
    public RecordStep? DuringStep { get; set; }
}

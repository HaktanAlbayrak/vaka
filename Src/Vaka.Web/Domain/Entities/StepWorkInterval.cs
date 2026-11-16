namespace Vaka.Web.Domain.Entities;

public class StepWorkInterval
{
    public int Id { get; set; }
    public int StepParticipantId { get; set; }
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public StepIntervalEndKind? EndKind { get; set; }
    public int? EndedByUserId { get; set; }
    public int? WorkSeconds { get; set; }

    public StepParticipant Participant { get; set; } = null!;
    public User? EndedBy { get; set; }
}

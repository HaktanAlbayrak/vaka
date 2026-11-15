using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class RecordStep
{
    public int Id { get; set; }
    public int RecordPhaseId { get; set; }
    public int ProcedureStepId { get; set; }
    public StepStatus Status { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public int? StartedByUserId { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    public int? EndedByUserId { get; set; }
    public int BreakSeconds { get; set; }
    public int? DurationSeconds { get; set; }
    public int? EffortSeconds { get; set; }

    public RecordPhase Phase { get; set; } = null!;
    public ProcedureStep ProcedureStep { get; set; } = null!;
    public User? StartedBy { get; set; }
    public User? EndedBy { get; set; }
    public ICollection<StepParticipant> Participants { get; set; } = new List<StepParticipant>();
}

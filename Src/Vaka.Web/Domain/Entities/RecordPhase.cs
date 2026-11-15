using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class RecordPhase
{
    public int Id { get; set; }
    public int CleaningRecordId { get; set; }
    public int ProcedurePhaseId { get; set; }
    public PhaseStatus Status { get; set; }
    public int? MinDurationSecondsSnapshot { get; set; }
    public int? GrossSeconds { get; set; }
    public int? BreakSeconds { get; set; }
    public int? HoldSeconds { get; set; }
    public int? NetSeconds { get; set; }
    public bool BelowMinimum { get; set; }
    public string? BelowMinimumReason { get; set; }

    public CleaningRecord Record { get; set; } = null!;
    public ProcedurePhase ProcedurePhase { get; set; } = null!;
    public ICollection<RecordStep> Steps { get; set; } = new List<RecordStep>();
}

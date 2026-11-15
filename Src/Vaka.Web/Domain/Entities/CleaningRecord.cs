using Vaka.Web.Services;

namespace Vaka.Web.Domain.Entities;

public class CleaningRecord
{
    public int Id { get; set; }
    public string RecordNo { get; set; } = string.Empty;
    public string? FieldBookRef { get; set; }
    public CleaningTypeKind Kind { get; set; }
    public RecordStatus Status { get; set; }
    public int MachineId { get; set; }
    public int ProcedureVersionId { get; set; }
    public int OwnerUserId { get; set; }
    public string FacilityCodeSnapshot { get; set; } = string.Empty;
    public string FacilityNameSnapshot { get; set; } = string.Empty;
    public string LineCodeSnapshot { get; set; } = string.Empty;
    public string LineNameSnapshot { get; set; } = string.Empty;
    public string MachineCodeSnapshot { get; set; } = string.Empty;
    public string MachineNameSnapshot { get; set; } = string.Empty;
    public string? WorkOrderNo { get; set; }
    public string? Note { get; set; }
    public string? EmergencyDescription { get; set; }
    public int? LockMinutes { get; set; }
    public DateTime? LockExpiresAtUtc { get; set; }
    public DateTime OpenedAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
    public int? ClosureKind { get; set; }
    public int? ClosedByUserId { get; set; }
    public string? ClosureReason { get; set; }
    public int? GrossSeconds { get; set; }
    public int? BreakSeconds { get; set; }
    public int? HoldSeconds { get; set; }
    public int? NetSeconds { get; set; }
    public int? EffortSeconds { get; set; }
    public int Version { get; set; }

    public Machine Machine { get; set; } = null!;
    public ProcedureVersion ProcedureVersion { get; set; } = null!;
    public User Owner { get; set; } = null!;
    public User? ClosedBy { get; set; }
    public ICollection<RecordPhase> Phases { get; set; } = new List<RecordPhase>();
    public ICollection<RecordMaterial> Materials { get; set; } = new List<RecordMaterial>();
    public ICollection<CleaningEvent> Events { get; set; } = new List<CleaningEvent>();
}

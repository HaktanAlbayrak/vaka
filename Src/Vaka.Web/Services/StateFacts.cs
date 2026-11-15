namespace Vaka.Web.Services;

public sealed record RecordFacts(
    string RecordNo,
    RecordStatus Status,
    int MachineId,
    int ProcedureVersionId,
    int OwnerUserId,
    string? WorkOrderNo,
    DateTime OpenedAtUtc,
    DateTime? StartedAtUtc,
    DateTime? CompletedAtUtc,
    DateTime? ClosedAtUtc,
    int? ClosureKind,
    int? ClosedByUserId,
    string? ClosureReason);

public sealed record StepFacts(
    int Id,
    int PhaseOrder,
    int StepOrder,
    string Title,
    DateTime? StartedAtUtc,
    int? StartedByUserId,
    DateTime? EndedAtUtc,
    int? EndedByUserId,
    int? DurationSeconds,
    int[] ParticipantUserIds);

public sealed record PhaseFacts(int Id, int OrderNo, PhaseStatus Status, bool BelowMinimum, string? BelowMinimumReason, int? NetSeconds);

public sealed record MaterialFacts(int MaterialLotId, string LotNo, bool ExpiredAtEntry, string? ExpiredReason);

public sealed record BreakFacts(
    int Id,
    int StartedByUserId,
    DateTime StartedAtUtc,
    DateTime? EndedAtUtc,
    int? EndedByUserId,
    BreakEndKind? EndKind);

public sealed record StateFacts(
    RecordFacts Record,
    StepFacts[] Steps,
    PhaseFacts[] Phases,
    MaterialFacts[] Materials,
    BreakFacts[] Breaks);

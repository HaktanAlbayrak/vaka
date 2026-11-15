namespace Vaka.Web.Services;

public sealed record EmergencyOpenRequest(
    Guid CommandId,
    int? FacilityId,
    int? LineId,
    int? MachineId,
    int? CleaningTypeId,
    string? Description,
    IReadOnlyList<LotEntryRequest> Materials,
    string? Pin);

public sealed record EmergencyPlan(
    PlannedMachine Machine,
    int CleaningTypeId,
    int VersionId,
    int VersionNo,
    string Description,
    IReadOnlyList<PlannedLot> Lots);

public sealed record EmergencyValidation(EmergencyPlan? Plan, IReadOnlyList<FieldError> Errors);

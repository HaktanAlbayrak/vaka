namespace Vaka.Web.Services;

public sealed record PlannedMachine(
    int Id,
    string Code,
    string Name,
    int LineId,
    string LineCode,
    string LineName,
    int FacilityId,
    string FacilityCode,
    string FacilityName);

public sealed record PlannedLot(
    int LotId,
    string MaterialCode,
    string MaterialName,
    string LotNo,
    DateOnly ExpiryDate,
    bool IsExpired,
    string? ExpiredReason);

/// <summary>Doğrulanmış kayıt açma isteği: tüm kimlikler birbiriyle tutarlı ve aktiftir.</summary>
public sealed record RecordOpenPlan(
    PlannedMachine Machine,
    int CleaningTypeId,
    int VersionId,
    int VersionNo,
    string WorkOrderNo,
    string? Note,
    int LockMinutes,
    IReadOnlyList<PlannedLot> Lots);

public sealed record OpenValidation(RecordOpenPlan? Plan, IReadOnlyList<FieldError> Errors);

public interface IRecordOpenValidator
{
    /// <summary>Sunucu doğrulaması: istek gövdesindeki kimlikler yeniden kontrol edilir, istemci listesine güvenilmez (BR-01, BR-02, K-18, BR-45, BR-12).</summary>
    Task<OpenValidation> ValidateAsync(RecordOpenRequest request, CancellationToken cancellationToken);
}

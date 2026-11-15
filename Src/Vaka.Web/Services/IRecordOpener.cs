namespace Vaka.Web.Services;

public sealed record LotEntryRequest(int MaterialLotId, string? ExpiredReason);

public sealed record RecordOpenRequest(
    Guid CommandId,
    int? FacilityId,
    int? LineId,
    int? MachineId,
    int? CleaningTypeId,
    string? WorkOrderNo,
    string? Note,
    int LockMinutes,
    IReadOnlyList<LotEntryRequest> Materials,
    string? Pin);

/// <summary><paramref name="BlockingRecordId"/> ret kaydın makineyi ya da sahibi tutan kaydıysa onun kimliğidir (hata bağlantısı için).</summary>
public sealed record OpenOutcome(CommandResult Result, int? BlockingRecordId);

public interface IRecordOpener
{
    /// <summary>
    /// Yeni planlı kayıt açar (Faz 4). Sıra: komut tekrarı denetimi, alan ve tutarlılık doğrulaması (BR-01, BR-02, K-18, BR-45, BR-12),
    /// makine ve sahip kilidi, PIN en son; hepsi yazma kilitli tek işlemde, olay ve komut satırı kayıtla birlikte onaylanır.
    /// Reddedilen istek olay ve komut satırı bırakmaz. Eşzamanlı istekte kısmi benzersiz index ihlali nedenli ret olur.
    /// Başarıda <c>EntityId</c> kayıt kimliğidir; aynı <c>CommandId</c> ile tekrar istek ilk sonucu döner (BR-33).
    /// </summary>
    Task<OpenOutcome> OpenAsync(int ownerUserId, RecordOpenRequest request, CancellationToken cancellationToken);
}

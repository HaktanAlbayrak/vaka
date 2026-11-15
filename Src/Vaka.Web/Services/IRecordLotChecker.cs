namespace Vaka.Web.Services;

public interface IRecordLotChecker
{
    /// <summary>
    /// Açılış formundaki lotları sunucuda yeniden doğrular (BR-11, BR-12): lot var ve malzemesi aktif olmalı, aynı lot iki kez eklenemez,
    /// SKT geçmiş lot için gerekçe zorunlu (1-500 karakter). Hatalar <paramref name="errors"/> listesine <c>Materials[i].…</c> alanlarıyla eklenir.
    /// </summary>
    Task<IReadOnlyList<PlannedLot>> CheckAsync(IReadOnlyList<LotEntryRequest> entries, List<FieldError> errors, CancellationToken cancellationToken);
}

namespace Vaka.Web.Services;

public sealed record InterventionRequest(Guid CommandId, InterventionType? Type, string? Reason, string? Pin);

public sealed record ManagerBreakEndRequest(Guid CommandId, string? Reason, string? Pin);

/// <summary>
/// Yönetici müdahalesi (BR-27, BR-40). Başarıda <c>EntityId</c> kayıt kimliğidir; aynı <c>CommandId</c> ile tekrar istek ilk sonucu döner (BR-33).
/// Reddedilen istek olay ve komut satırı bırakmaz; yanlış PIN yalnız hatalı deneme sayacını artırır. Çağıranın rolü controller'da denetlenir.
/// </summary>
public interface IInterventionService
{
    /// <summary>
    /// İptal (Açıldı ya da Devam ediyor) ve yarıda kapatma (Devam ediyor). Önce kayıt için zaman aşımı uygulanır (BR-26); zaman aşımına düşmüş kayıt reddedilir.
    /// Tür, durum ve gerekçe (1-500 karakter) serviste yeniden denetlenir, PIN en son. Tek işlemde: açık mola müdahale anında "Müdahale" türüyle kapanır;
    /// devam eden adım Kesildi olur, bitişi müdahale anıdır ve adım molası SH-17, süresi SH-01, eforu SH-02 ile yazılır; tamamlanmamış fazlar Kesildi olur;
    /// görevlilerin çalışıyor bayrağı kalkar; kayıt iptal edildi ya da yarıda kapatıldı olur ve kapanış türü, kapatan, gerekçe ve an yazılır (makine kilidi böylece kalkar).
    /// Kayıt silinmez, sahip değişmez, kayıt no yeniden kullanılmaz.
    /// </summary>
    Task<CommandResult> InterveneAsync(int recordId, int actorUserId, InterventionRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Açık molayı yönetici bitirir (BR-40): bitiş = yöneticinin işlem anı, kapanış türü Yönetici, gerekçe ve PIN zorunlu. Adım "Devam ediyor" kalır,
    /// molayı başlatan kişinin yazma kilidi kalkar. Mola zaten bitmişse ret.
    /// </summary>
    Task<CommandResult> EndBreakAsync(int breakId, int actorUserId, ManagerBreakEndRequest request, CancellationToken cancellationToken);
}

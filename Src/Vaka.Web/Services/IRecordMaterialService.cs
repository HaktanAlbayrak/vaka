namespace Vaka.Web.Services;

public sealed record AddMaterialRequest(Guid CommandId, int MaterialLotId, string? ExpiredReason, string? Pin);

public sealed record RemoveMaterialRequest(Guid CommandId, int RecordMaterialId, string? Pin);

/// <summary>
/// Kayda malzeme ekleme ve çıkarma (BR-12, BR-46, BR-47). Yalnız sahip, kendi PIN'iyle, ilk adımdan önce ve kilit bitişi geçmemişken.
/// Önce kaydın zaman aşımı uygulanır ve onaylanır, sonra istek reddedilir. Malzeme eklemek kilidi uzatmaz.
/// Sonuç: kayıt yoksa <c>NotFound</c>, sahip değilse <c>Forbidden</c>, iş kuralı ihlalinde <c>Rejected</c>; başarıda <c>EntityId</c> kayıt kimliği.
/// Aynı <c>CommandId</c> ile tekrar istek ilk sonucu döner (BR-33).
/// </summary>
public interface IRecordMaterialService
{
    Task<CommandResult> AddAsync(int recordId, int actorUserId, AddMaterialRequest request, CancellationToken cancellationToken);

    Task<CommandResult> RemoveAsync(int recordId, int actorUserId, RemoveMaterialRequest request, CancellationToken cancellationToken);
}

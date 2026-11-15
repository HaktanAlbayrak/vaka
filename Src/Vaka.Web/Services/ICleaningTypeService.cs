using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed record CleaningTypeRequest(string? Code, string? Name, CleaningTypeKind? Kind, int? DefaultLockMinutes, bool IsActive);

public interface ICleaningTypeService
{
    Task<CleaningTypeListViewModel> ListAsync(CleaningTypeKind? kind, bool showInactive, CancellationToken cancellationToken);

    /// <summary><paramref name="withBlockingRecords"/> yalnız pasifleştirme reddi sonrası yeniden çizimde true gelir.</summary>
    Task<LookupResult<CleaningTypeFormViewModel>> GetFormAsync(int? id, int viewerUserId, bool withBlockingRecords, CancellationToken cancellationToken);

    Task<LookupResult<CleaningTypeDetailViewModel>> GetDetailAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Tür zorunlu. Planlı tipte varsayılan kilit süresi zorunlu ve 0 &lt; süre &lt;= AzamiKilitSüresi (BR-45, ayar her istekte okunur);
    /// acil tipte dolu süre <c>DefaultLockMinutes</c> alan hatasıyla reddedilir (BR-50).
    /// </summary>
    Task<CommandResult> CreateAsync(CleaningTypeRequest request, CancellationToken cancellationToken);

    /// <summary>Kayıtlı türden farklı tür reddedilir (PL-04). Aktif ya da beklemedeki kaydı olan tip pasifleştirilemez (BR-35).</summary>
    Task<CommandResult> UpdateAsync(int id, int viewerUserId, CleaningTypeRequest request, CancellationToken cancellationToken);
}

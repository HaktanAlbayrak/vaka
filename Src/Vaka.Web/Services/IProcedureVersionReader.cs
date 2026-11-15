using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public interface IProcedureVersionReader
{
    /// <summary>Taslak olmayan versiyon için <see cref="CommandStatus.Rejected"/> döner (düzenleyici açılmaz).</summary>
    Task<LookupResult<ProcedureDraftViewModel>> GetDraftAsync(int id, CancellationToken cancellationToken);

    /// <summary>Taslak versiyon için <see cref="CommandStatus.Rejected"/> döner; çağıran Yönetici'yi düzenleyiciye yönlendirir, Denetçi'ye 404 verir (PL-51).</summary>
    Task<LookupResult<ProcedureVersionViewModel>> GetVersionAsync(int id, CancellationToken cancellationToken);
}

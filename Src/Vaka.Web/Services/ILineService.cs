using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed record LineRequest(string? Code, string? Name, bool IsActive);

public interface ILineService
{
    /// <summary>Kayıtlı hat için <paramref name="id"/>, yeni hat için <paramref name="facilityId"/> verilir; kayıt bulunmazsa NotFound.</summary>
    Task<LookupResult<LineFormViewModel>> GetFormAsync(int? id, int? facilityId, CancellationToken cancellationToken);

    /// <summary>Pasif tesise hat eklenemez (BR-05). Başarıda <c>EntityId</c> tesisin kimliğidir.</summary>
    Task<CommandResult> CreateAsync(int facilityId, LineRequest request, CancellationToken cancellationToken);

    /// <summary>Başarıda <c>EntityId</c> hattın tesisinin kimliğidir. Kayıtlı koddan farklı dolu kod reddedilir (PL-04).</summary>
    Task<CommandResult> UpdateAsync(int id, LineRequest request, CancellationToken cancellationToken);
}

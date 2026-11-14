using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed record FacilityRequest(string? Code, string? Name, bool IsActive);

public interface IFacilityService
{
    Task<FacilityListViewModel> ListAsync(bool showInactive, CancellationToken cancellationToken);

    /// <summary><paramref name="id"/> boşsa yeni tesis formu; kayıtlı tesiste <c>CodeLocked</c> true (PL-04).</summary>
    Task<LookupResult<FacilityFormViewModel>> GetFormAsync(int? id, CancellationToken cancellationToken);

    Task<LookupResult<FacilityDetailViewModel>> GetDetailAsync(int id, CancellationToken cancellationToken);

    Task<CommandResult> CreateAsync(FacilityRequest request, CancellationToken cancellationToken);

    /// <summary>Kayıtlı koddan farklı dolu kod reddedilir (PL-04, "Code" alan hatası).</summary>
    Task<CommandResult> UpdateAsync(int id, FacilityRequest request, CancellationToken cancellationToken);
}

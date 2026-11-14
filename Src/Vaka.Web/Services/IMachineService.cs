using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.Services;

public sealed record MachineListQuery(int? FacilityId, int? LineId, string? Query, bool ShowRetired, int Page);

public sealed record MachineRequest(int FacilityId, int LineId, string? Code, string? Name);

public interface IMachineService
{
    Task<MachineListViewModel> ListAsync(MachineListQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Yeni makinede <paramref name="facilityId"/> hat listesini belirler (JS kapalıyken seçim sunucuda, K3); yalnız aktif tesis ve hatlar listelenir (BR-05).
    /// Kayıtlı makinede tesis ve hat sabittir, listeler yalnız bunları taşır.
    /// </summary>
    Task<LookupResult<MachineFormViewModel>> GetFormAsync(int? id, int? facilityId, CancellationToken cancellationToken);

    /// <summary>Tesisin aktif hatları, kod sırasıyla (bağlı seçim için).</summary>
    Task<SelectOptionViewModel[]> ListLinesAsync(int facilityId, CancellationToken cancellationToken);

    Task<CommandResult> CreateAsync(MachineRequest request, CancellationToken cancellationToken);

    /// <summary>Kayıtlı koddan farklı dolu kod ve kayıtlı hattan farklı sıfırdan büyük hat reddedilir (PL-04).</summary>
    Task<CommandResult> UpdateAsync(int id, MachineRequest request, CancellationToken cancellationToken);
}

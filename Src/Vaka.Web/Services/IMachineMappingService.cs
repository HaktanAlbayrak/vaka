using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public interface IMachineMappingService
{
    /// <summary>Acil tip için <see cref="CommandStatus.Rejected"/> döner (BR-50); eşleme listelenmez.</summary>
    Task<LookupResult<MachineMappingViewModel>> GetAsync(int cleaningTypeId, CancellationToken cancellationToken);

    /// <summary>
    /// Yalnız seçilebilir makineler (aktif, kaldırılmamış, aktif hat ve tesis) değişir: gönderilmeyen eşli makine kaldırılır, gönderilen eşsiz makine eklenir.
    /// Seçilemez makinenin eşlemesi gönderilmese de korunur, seçilemez ve eşsiz makine gönderilse de eşlenmez (K3, PL-53).
    /// </summary>
    Task<CommandResult> SaveAsync(int cleaningTypeId, IReadOnlyCollection<int> selectedMachineIds, CancellationToken cancellationToken);
}

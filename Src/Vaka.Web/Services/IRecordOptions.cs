using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

/// <summary>Kayıt açma formunun bağlı seçim listeleri (BR-01, BR-02). Listeler yalnız aktif tanımları taşır.</summary>
public interface IRecordOptions
{
    Task<SelectOptionViewModel[]> ListFacilitiesAsync(CancellationToken cancellationToken);

    Task<SelectOptionViewModel[]> ListLinesAsync(int facilityId, CancellationToken cancellationToken);

    /// <summary>
    /// Aktif, kullanımdan kaldırılmamış makineler. Önce zaman aşımı uygulanır; Açıldı, Devam ediyor ya da Beklemede kaydı olan makine
    /// <c>IsAvailable</c> false ve <c>BusyReason</c> sahip, durum, açılış saatiyle gelir (BR-23).
    /// </summary>
    Task<MachineOptionViewModel[]> ListMachinesAsync(int lineId, CancellationToken cancellationToken);

    /// <summary>Makineye eşli, aktif, yürürlükte versiyonu olan planlı tipler (BR-02).</summary>
    Task<CleaningTypeOptionViewModel[]> ListCleaningTypesAsync(int machineId, CancellationToken cancellationToken);
}

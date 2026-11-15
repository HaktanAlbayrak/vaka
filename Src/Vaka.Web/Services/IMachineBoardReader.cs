using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IMachineBoardReader
{
    /// <summary>
    /// Makine durum panosu (E-16). Önce tüm Açıldı kayıtlara zaman aşımı uygulanır; kilidi biten kayıt panoda Açıldı görünmez, makine Boş olur (BR-26).
    /// Yalnız aktif, kullanımdan kaldırılmamış makineler listelenir. <c>RecordUrl</c> yalnız yürütme ekranını açabilen Operatör'e verilir.
    /// </summary>
    Task<MachineBoardViewModel> GetAsync(int? facilityId, bool viewerIsOperator, CancellationToken cancellationToken);
}

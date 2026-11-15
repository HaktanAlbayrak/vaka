using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IMachineBoardReader
{
    /// <summary>
    /// Makine durum panosu (E-16). Önce tüm Açıldı kayıtlara zaman aşımı uygulanır; kilidi biten kayıt panoda Açıldı görünmez, makine Boş olur (BR-26).
    /// Yalnız aktif, kullanımdan kaldırılmamış makineler listelenir. <c>RecordUrl</c> görüntüleyenin rolüne göre <see cref="RecordLinks.For"/> ile verilir: Operatör yürütme ekranına, Yönetici ve Denetçi detaya gider.
    /// </summary>
    Task<MachineBoardViewModel> GetAsync(int? facilityId, IReadOnlyCollection<string> viewerRoles, CancellationToken cancellationToken);
}

using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record RecordViewer(int UserId, string FullName, IReadOnlyCollection<string> Roles);

public interface IRecordDetailReader
{
    /// <summary>
    /// Kayıt detayı (E-20): Brief §4.3'ün 13 sorusu, açılış anı değerleri (anlık görüntü), prosedür versiyonunun içeriği (FR-09),
    /// zaman çizelgesi, mola, malzeme, olay günlüğü, bütünlük sonucu ve kapanış bilgisi. Önce kaydın zaman aşımı uygulanır (BR-26).
    /// Süreler kayıtta saklanan değerlerdir; bitmemiş kayıtta özet süreleri boştur. Yazdırılan sayfa için oluşturma anı ve oluşturan kişi doldurulur.
    /// Müdahale ve mola bitirme bağlantıları yalnız Yönetici'ye, yürütme bağlantısı yalnız Operatör'e, bütünlük ve versiyon bağlantıları Yönetici ve Denetçi'ye verilir.
    /// </summary>
    Task<LookupResult<RecordDetailViewModel>> GetAsync(int recordId, RecordViewer viewer, CancellationToken cancellationToken);
}

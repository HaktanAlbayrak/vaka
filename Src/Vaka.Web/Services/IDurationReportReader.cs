using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public interface IDurationReportReader
{
    /// <summary>
    /// Süre ve efor raporu (FR-21, FR-22, FR-36, FR-40). Tarih süzgeci kaydın açılış anına, Türkiye tarihine göre gün dahil uygulanır; süre hesabı değildir.
    /// Ortalamalar yalnız Tamamlandı kayıtlardan alınır ve <see cref="IDurationCalculator.CalculateAverage"/> ile hesaplanır (SH-15):
    /// makine satırı makine ve tip başına (versiyonlar birlikte), faz ve adım satırları tip versiyonu içindedir. Durum sayıları süzgece uyan tüm kayıtlardandır.
    /// En uzun adımlar ortalama süreye göre ilk 10 satırdır. Sayfalama yoktur.
    /// <paramref name="inputError"/> doluysa (okunamayan değer) ya da süzgeç geçersizse (bitiş başlangıçtan önce, olmayan ya da birbirine uymayan tesis, hat, makine, tip, tanımsız tür)
    /// tablolar boş döner ve <c>ErrorMessage</c> dolar.
    /// </summary>
    Task<DurationReportViewModel> BuildAsync(DurationReportFilterViewModel filter, string? inputError, CancellationToken cancellationToken);
}

using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public interface IMaterialTraceReader
{
    /// <summary>
    /// Malzeme izlenebilirliği (FR-37, SC-22). Lot modunda lotun kullanıldığı kayıtlar (makine, açılış zamanı, durum), makine modunda makineye
    /// girilen malzemeler giriş zamanı sırasıyla döner; kayıttan çıkarılmış malzeme listelenmez. Tarih süzgeci Türkiye tarihine göre gün dahildir:
    /// lot modunda kaydın açılışına, makine modunda malzemenin giriş anına uygulanır. Sorgu yalnız seçim (lot ya da makine) varsa çalışır.
    /// <paramref name="inputError"/> doluysa ya da süzgeç geçersizse (bitiş başlangıçtan önce, geçersiz mod, olmayan lot ya da makine) sorgu çalışmaz, <c>ErrorMessage</c> dolar.
    /// <c>LotQuery</c> seçilebilir lotları (en çok 50) bulur. Kullanım oranı modunda seçim gerekmez; süzgeç hatasızsa <see cref="IMaterialUsageReader"/> ile sorgu çalışır
    /// ve süzgeç tesis, hat, makine ve tip kimliklerinin varlığı ile uyumu da denetlenir.
    /// </summary>
    Task<MaterialTraceViewModel> BuildAsync(MaterialTraceViewModel request, string? inputError, CancellationToken cancellationToken);
}

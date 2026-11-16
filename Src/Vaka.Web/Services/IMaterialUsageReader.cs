using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public interface IMaterialUsageReader
{
    /// <summary>
    /// Malzeme kullanım oranı (PL-85 madde 7). Süzgece uyan Tamamlandı kayıtlar üzerinden malzeme başına kayıt sayısı, oran, farklı lot sayısı ve
    /// SKT geçmiş kullanım sayısı döner; kayıttan çıkarılmış malzeme (<c>RemovedAtUtc</c> dolu) sayılmaz. Tarih süzgeci kaydın açılış anına, Türkiye
    /// tarihine göre gün dahil uygulanır. Tamamlandı kayıt yoksa satır yoktur. Süzgeç doğrulaması çağıranındır.
    /// </summary>
    Task<MaterialUsageViewModel> ReadAsync(MaterialUsageFilter filter, CancellationToken cancellationToken);
}

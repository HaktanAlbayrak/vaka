using Vaka.Web.ViewModels.Dashboard;

namespace Vaka.Web.Services;

public interface IDashboardReader
{
    /// <summary>
    /// Yönetici panosunun verisi (E-22, PL-76). Önce kilidi geçmiş kayıtlara zaman aşımı uygulanır. Süreler kayıt ve fazda saklı saniyelerdir.
    /// Bugün = Türkiye takvim günü [gün başı, ertesi gün başı); son 7 gün = bugün dahil 7 takvim günü, açılış anına göre, kaydı olmayan gün 0.
    /// </summary>
    Task<DashboardViewModel> GetAsync(CancellationToken cancellationToken);
}

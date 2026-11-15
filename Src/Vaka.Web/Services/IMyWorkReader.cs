using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public interface IMyWorkReader
{
    /// <summary>
    /// İşlerim (E-13). Önce sahibin Açıldı kaydının zaman aşımı uygulanır (BR-26). Aktif kayıt Açıldı ya da Devam ediyor durumundaki kayıttır (BR-37);
    /// varsa yeni kayıt açılamaz ve neden kaydın numarası ve bağlantısıyla döner.
    /// </summary>
    Task<MyWorkViewModel> GetAsync(int userId, CancellationToken cancellationToken);
}

namespace Vaka.Web.Authorization;

/// <summary>
/// Çıkış yapılmış oturum kimlikleri (K23, PL-71). Bellekte tutulur, şema gerektirmez; uygulama yeniden başlayınca liste boşalır.
/// Kalıcı çözüm kullanıcı satırında oturum sürümüdür (şema değişikliği, kullanıcı onayı ister).
/// </summary>
public interface ISessionRevocations
{
    /// <summary>
    /// Kimliği iptal listesine alır. Listede kalma = OturumSüresi (dakika): işlemsizlik OturumSüresi'ni aşan çerez zaten kapanır,
    /// çıkış anından sonra bundan daha uzun tutmak gerekmez. Süresi dolan girdiler her yazmada temizlenir.
    /// </summary>
    void Revoke(string sessionId, int sessionMinutes);

    bool IsRevoked(string sessionId);
}

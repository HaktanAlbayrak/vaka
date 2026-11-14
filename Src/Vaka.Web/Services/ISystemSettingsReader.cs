namespace Vaka.Web.Services;

public interface ISystemSettingsReader
{
    /// <summary>Ayarın güncel değerini veritabanından okur; satır yoksa <see cref="InvalidOperationException"/> atar, varsayılan değer uydurmaz.</summary>
    Task<int> GetValueAsync(string key, CancellationToken cancellationToken);
}

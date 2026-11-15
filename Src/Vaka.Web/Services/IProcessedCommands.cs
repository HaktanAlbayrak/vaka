namespace Vaka.Web.Services;

public interface IProcessedCommands
{
    /// <summary>
    /// Komut kimliği daha önce işlendiyse ilk sonucu döner, yeni olay üretilmez (BR-33). Başka kullanıcının kimliği reddedilir.
    /// İşlenmediyse boş döner.
    /// </summary>
    Task<CommandResult?> ReplayAsync(Guid commandId, int userId, CancellationToken cancellationToken);

    /// <summary>Komutun ilk sonucunu, etkisiyle aynı işlemde kaydedilmek üzere ekler (çağıran <c>SaveChanges</c> yapar).</summary>
    void Remember(Guid commandId, string commandType, int userId, int recordId, DateTime createdAtUtc);
}

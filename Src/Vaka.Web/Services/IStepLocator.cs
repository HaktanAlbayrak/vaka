namespace Vaka.Web.Services;

public interface IStepLocator
{
    /// <summary>Adımın kayıt kimliği; reddedilen adım isteğinde yürütme ekranını yeniden çizmek için. Adım yoksa boş.</summary>
    Task<int?> GetRecordIdAsync(int stepId, CancellationToken cancellationToken);
}

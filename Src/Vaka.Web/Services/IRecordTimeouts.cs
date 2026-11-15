namespace Vaka.Web.Services;

/// <summary>Zaman aşımının uygulanacağı kayıtlar; 0 olan alan süzgeç koymaz, <see cref="All"/> hepsini kapsar.</summary>
public sealed record TimeoutScope(bool All, int RecordId, int MachineId, int OwnerUserId)
{
    public static TimeoutScope Everything { get; } = new(true, 0, 0, 0);

    public static TimeoutScope ForRecord(int recordId) => new(false, recordId, 0, 0);

    public static TimeoutScope ForMachineAndOwner(int machineId, int ownerUserId) => new(false, 0, machineId, ownerUserId);

    public static TimeoutScope ForOwner(int ownerUserId) => new(false, 0, 0, ownerUserId);
}

public interface IRecordTimeouts
{
    /// <summary>
    /// Kilit bitişi geçmiş Açıldı kayıtları Zaman aşımına geçirir (BR-26). Kayda ya da makineye dokunan her istekten ve pano sorgusundan önce çağrılır;
    /// kendi işleminde onaylanır, böylece sonraki işlem reddedilse de geçiş kalır (BR-46). Zaman aşımı anı kilit bitişidir, olayın yazıldığı an ayrıca tutulur.
    /// Geçmiş kayıt yoksa işlem açılmaz.
    /// </summary>
    Task ApplyAsync(TimeoutScope scope, CancellationToken cancellationToken);
}

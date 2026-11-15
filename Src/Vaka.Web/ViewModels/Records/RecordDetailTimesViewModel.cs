namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailTimesViewModel
{
    public DateTime OpenedAtLocal { get; set; }

    public DateTime? StartedAtLocal { get; set; }

    public DateTime? CompletedAtLocal { get; set; }

    /// <summary>Zaman aşımı, iptal ve yarıda kapatmada kapanış anı.</summary>
    public DateTime? ClosedAtLocal { get; set; }

    /// <summary>Yalnız Açıldı durumunda dolu.</summary>
    public DateTime? LockExpiresAtLocal { get; set; }
}

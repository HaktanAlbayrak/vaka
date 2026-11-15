namespace Vaka.Web.Services;

public static class InterventionRules
{
    public const int ReasonMaxLength = 500;

    /// <summary>Açıldı kayıtta yalnız İptal, Devam ediyor kayıtta İptal ve Yarıda kapatma; diğer durumlarda hiçbiri (BR-27, K15, K16).</summary>
    public static InterventionType[] AllowedTypes(RecordStatus status) => status switch
    {
        RecordStatus.Opened => [InterventionType.Cancel],
        RecordStatus.InProgress => [InterventionType.Cancel, InterventionType.Abort],
        _ => []
    };

    /// <summary>Müdahale sunulmayan durumun nedeni; müdahale edilebiliyorsa boş.</summary>
    public static string? BlockReason(RecordStatus status) => status switch
    {
        RecordStatus.Opened or RecordStatus.InProgress => null,
        RecordStatus.TimedOut => "Kayıt zaman aşımına düştü.",
        RecordStatus.Completed => "Kayıt tamamlandı, müdahale edilemez.",
        RecordStatus.Cancelled => "Kayıt zaten iptal edildi.",
        RecordStatus.Aborted => "Kayıt zaten yarıda kapatıldı.",
        RecordStatus.OnHold => "Beklemedeki kayıtta müdahale yapılamaz.",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Bilinmeyen kayıt durumu.")
    };

    /// <summary>Gerekçe boş ya da uzunsa alana bağlı ret; geçerliyse boş.</summary>
    public static CommandResult? CheckReason(string? reason) => reason switch
    {
        null or "" => CommandResult.Rejected("Reason", "Gerekçe yazın."),
        { Length: > ReasonMaxLength } => CommandResult.Rejected("Reason", $"Gerekçe en çok {ReasonMaxLength} karakter olabilir."),
        _ => null
    };
}

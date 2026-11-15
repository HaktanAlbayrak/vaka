namespace Vaka.Web.ViewModels.Records;

public sealed class ParticipantOptionViewModel
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string RegistryNo { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    public string? UnavailableReason { get; set; }

    /// <summary>Kişinin kendi kaydı Devam ediyor; eklenebilir, ekranda K-41 uyarısı gösterilir.</summary>
    public bool HasOwnActiveRecord { get; set; }
}

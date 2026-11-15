namespace Vaka.Web.ViewModels.Records;

/// <summary><c>Pin</c> sahibin kendi PIN'idir; eklenen kişinin PIN'i Faz 7'de gelir (PL-57).</summary>
public sealed class AddParticipantViewModel
{
    public Guid CommandId { get; set; }

    public int ParticipantUserId { get; set; }

    public string Pin { get; set; } = string.Empty;
}

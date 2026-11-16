namespace Vaka.Web.ViewModels.Records;

/// <summary><c>Pin</c> katılan kişinin kendi PIN'idir; sahip tabletinden girilse de sahibin PIN'i geçmez.</summary>
public sealed class JoinStepViewModel
{
    public Guid CommandId { get; set; }

    public int ParticipantUserId { get; set; }

    public string Pin { get; set; } = string.Empty;
}

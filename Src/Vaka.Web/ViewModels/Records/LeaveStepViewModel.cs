namespace Vaka.Web.ViewModels.Records;

/// <summary><c>Pin</c> ayrılan kişinin kendi PIN'idir.</summary>
public sealed class LeaveStepViewModel
{
    public Guid CommandId { get; set; }

    public int ParticipantUserId { get; set; }

    public string Pin { get; set; } = string.Empty;
}

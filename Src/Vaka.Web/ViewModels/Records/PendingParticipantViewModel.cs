namespace Vaka.Web.ViewModels.Records;

/// <summary>Reddedilen görevli ekleme isteğinde seçili kişi korunur; PIN korunmaz.</summary>
public sealed class PendingParticipantViewModel
{
    public int StepId { get; set; }

    public int ParticipantUserId { get; set; }
}

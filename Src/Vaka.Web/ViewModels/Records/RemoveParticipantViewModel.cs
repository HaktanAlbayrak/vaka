namespace Vaka.Web.ViewModels.Records;

public sealed class RemoveParticipantViewModel
{
    public Guid CommandId { get; set; }

    public int ParticipantUserId { get; set; }

    public string Pin { get; set; } = string.Empty;
}

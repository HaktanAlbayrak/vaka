namespace Vaka.Web.ViewModels.Records;

public sealed class StepParticipantViewModel
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string RegistryNo { get; set; } = string.Empty;

    public bool IsOwner { get; set; }

    public DateTime AddedAtLocal { get; set; }
}

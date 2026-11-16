namespace Vaka.Web.ViewModels.Records;

public sealed class AssignedStepViewModel
{
    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string StepTitle { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string Url { get; set; } = string.Empty;

    public int StepId { get; set; }

    /// <summary>Kişi adımın listesinde ama girişini yapmamış; kart listede öne çıkar.</summary>
    public bool AwaitingMyEntry { get; set; }
}

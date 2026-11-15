namespace Vaka.Web.ViewModels.Interventions;

public sealed class EndBreakViewModel
{
    public Guid CommandId { get; set; }

    public int BreakId { get; set; }

    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string StartedByName { get; set; } = string.Empty;

    public DateTime StartedAtLocal { get; set; }

    public string? DuringStepTitle { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;

    public string? BlockReason { get; set; }

    public string? ErrorMessage { get; set; }
}

namespace Vaka.Web.ViewModels.Records;

/// <summary>
/// Bitirme isteği ve süre altı gerekçe formu. Gerekçesiz istek faz minimumunun altındaysa bu model 200 görünümle
/// (<c>FinishStep.cshtml</c>) döner; gerekçeli gönderim aynı <c>CommandId</c> ile bitirir. PIN korunmaz.
/// </summary>
public sealed class FinishStepViewModel
{
    public int StepId { get; set; }

    public int RecordId { get; set; }

    public string PhaseTitle { get; set; } = string.Empty;

    public int PhaseNetSeconds { get; set; }

    public int MinDurationSeconds { get; set; }

    public Guid CommandId { get; set; }

    public string? BelowMinimumReason { get; set; }

    public string Pin { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

namespace Vaka.Web.ViewModels.Breaks;

public sealed class CurrentBreakViewModel
{
    public int BreakId { get; set; }

    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public DateTime StartedAtLocal { get; set; }

    /// <summary>Sunucunun hesapladığı geçen süre; <c>LiveTimer.js</c> bundan ilerler.</summary>
    public int ElapsedSeconds { get; set; }

    public string? DuringStepTitle { get; set; }

    public Guid CommandId { get; set; }

    public string? ErrorMessage { get; set; }
}

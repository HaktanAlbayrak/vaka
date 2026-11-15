namespace Vaka.Web.ViewModels.Records;

public sealed class OpenBreakViewModel
{
    public int Id { get; set; }

    public string StartedByName { get; set; } = string.Empty;

    public DateTime StartedAtLocal { get; set; }

    public string? DuringStepTitle { get; set; }

    /// <summary>Sunucunun hesapladığı geçen süre; <c>LiveTimer.js</c> bundan ilerler.</summary>
    public int ElapsedSeconds { get; set; }
}

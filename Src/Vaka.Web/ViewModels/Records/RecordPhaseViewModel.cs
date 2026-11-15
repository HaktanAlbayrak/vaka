using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordPhaseViewModel
{
    public int Id { get; set; }

    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? MinDurationSeconds { get; set; }

    public PhaseStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    /// <summary>Faz tamamlanınca saklanan değerler; sürerken boştur.</summary>
    public int? GrossSeconds { get; set; }

    public int? BreakSeconds { get; set; }

    public int? NetSeconds { get; set; }

    /// <summary>Devam eden fazda sunucunun hesapladığı güncel net süre; <c>LiveTimer.js</c> bundan ilerler, kendi hesabı yoktur (SH-14).</summary>
    public int? LiveNetSeconds { get; set; }

    public bool BelowMinimum { get; set; }

    public string? BelowMinimumReason { get; set; }

    public RecordStepViewModel[] Steps { get; set; } = [];
}

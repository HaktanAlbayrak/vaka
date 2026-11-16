namespace Vaka.Web.ViewModels.Reports;

/// <summary>
/// Ortalamalar yalnız Tamamlandı kayıtlardan alınır; Tamamlandı kayıt yoksa boştur. Ortalama planlanan minimum ile plan farkı acil tipte boştur.
/// <c>PlanVarianceSeconds</c> negatif olabilir (planlanandan kısa sürmüş).
/// </summary>
public sealed class MachineTypeRowViewModel
{
    public string CleaningTypeName { get; set; } = string.Empty;

    public string KindLabel { get; set; } = string.Empty;

    public int RecordCount { get; set; }

    public int CompletedCount { get; set; }

    public int? AvgGrossSeconds { get; set; }

    public int? AvgBreakSeconds { get; set; }

    public int? AvgNetSeconds { get; set; }

    public int? AvgEffortSeconds { get; set; }

    public int? AvgPlannedMinSeconds { get; set; }

    public int? PlanVarianceSeconds { get; set; }

    public int BelowMinimumPhaseCount { get; set; }

    public int EvaluatedPhaseCount { get; set; }
}

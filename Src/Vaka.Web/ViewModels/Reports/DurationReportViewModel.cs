using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Reports;

/// <summary>Faz 7'de bekleme ortalaması (<c>AvgHoldSeconds</c>) eklenir.</summary>
public sealed class DurationReportViewModel
{
    public DurationReportFilterViewModel Filter { get; set; } = new();

    /// <summary>Süzgeç geçersizse (bitiş başlangıçtan önce ya da okunamayan değer) dolar; tablolar boş gelir.</summary>
    public string? ErrorMessage { get; set; }

    public MachineDurationRowViewModel[] ByMachine { get; set; } = [];

    public PhaseDurationRowViewModel[] ByPhase { get; set; } = [];

    public LongestStepRowViewModel[] LongestSteps { get; set; } = [];

    public StatusCountViewModel[] StatusCounts { get; set; } = [];

    public SelectOptionViewModel[] Facilities { get; set; } = [];

    public SelectOptionViewModel[] Lines { get; set; } = [];

    public SelectOptionViewModel[] Machines { get; set; } = [];

    public SelectOptionViewModel[] CleaningTypes { get; set; } = [];

    public SelectOptionViewModel[] KindOptions { get; set; } = [];
}

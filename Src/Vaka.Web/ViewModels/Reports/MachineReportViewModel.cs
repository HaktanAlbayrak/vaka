using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Reports;

/// <summary>
/// Makine raporu (E-34). <c>Machine</c> makine seçilmediyse ya da süzgeç geçersizse boştur; geçersizse <c>ErrorMessage</c> dolar ve tablolar boş gelir.
/// </summary>
public sealed class MachineReportViewModel
{
    public MachineReportFilterViewModel Filter { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public SelectOptionViewModel[] FacilityOptions { get; set; } = [];

    public SelectOptionViewModel[] LineOptions { get; set; } = [];

    public SelectOptionViewModel[] MachineOptions { get; set; } = [];

    public SelectOptionViewModel[] KindOptions { get; set; } = [];

    public MachineReportHeaderViewModel? Machine { get; set; }

    public StatusCountViewModel[] StatusCounts { get; set; } = [];

    public MachineTypeRowViewModel[] ByType { get; set; } = [];

    public MachineRecentRecordViewModel[] Recent { get; set; } = [];

    /// <summary>Malzeme kullanım oranının paydası; süzgece uyan Tamamlandı kayıt sayısı.</summary>
    public int CompletedRecordCount { get; set; }

    public MaterialUsageRowViewModel[] Materials { get; set; } = [];
}

using Vaka.Web.Services;
using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.ViewModels.Reports;

/// <summary>
/// Lot modunda <c>MaterialLotId</c>, makine modunda <c>MachineId</c> sorguyu çalıştırır. <c>HasResult</c> sorgu çalıştıysa true olur
/// (boş sonuç ile sorgu yapılmadı ayrımı için). JS kapalıyken lot önce <c>LotQuery</c> ile aranır, <c>LotOptions</c> o aramanın sonucudur.
/// Faz 8'de düzeltilmiş işareti (<c>IsCorrected</c>) eklenir.
/// </summary>
public sealed class MaterialTraceViewModel
{
    public MaterialTraceMode Mode { get; set; } = MaterialTraceMode.Lot;

    public string? LotQuery { get; set; }

    public int? MaterialLotId { get; set; }

    public int? MachineId { get; set; }

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public bool HasResult { get; set; }

    /// <summary>Süzgeç geçersizse dolar; sorgu çalışmaz.</summary>
    public string? ErrorMessage { get; set; }

    public string? SelectedLotLabel { get; set; }

    public LotUsageViewModel[] LotUsages { get; set; } = [];

    public MachineMaterialViewModel[] MachineMaterials { get; set; } = [];

    public LotOptionViewModel[] LotOptions { get; set; } = [];

    public bool LotOptionsTruncated { get; set; }

    public SelectOptionViewModel[] Machines { get; set; } = [];

    public SelectOptionViewModel[] ModeOptions { get; set; } = [];
}

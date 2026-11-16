namespace Vaka.Web.ViewModels.Reports;

/// <summary><c>CompletedRecordCount</c> oranın paydasıdır; sıfırsa <c>Rows</c> boştur.</summary>
public sealed class MaterialUsageViewModel
{
    public int CompletedRecordCount { get; set; }

    public int RecordsWithMaterialCount { get; set; }

    public MaterialUsageRowViewModel[] Rows { get; set; } = [];
}

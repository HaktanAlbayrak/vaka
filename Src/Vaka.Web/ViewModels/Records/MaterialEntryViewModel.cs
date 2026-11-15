namespace Vaka.Web.ViewModels.Records;

/// <summary>
/// Formdan yalnız <c>MaterialLotId</c> ve <c>ExpiredReason</c> okunur. Diğer alanlar reddedilen istek yeniden çizilirken
/// sunucu tarafından, seçili lotun görünmesi için doldurulur.
/// </summary>
public sealed class MaterialEntryViewModel
{
    public int MaterialLotId { get; set; }

    public string? ExpiredReason { get; set; }

    public string MaterialCode { get; set; } = string.Empty;

    public string MaterialName { get; set; } = string.Empty;

    public string LotNo { get; set; } = string.Empty;

    public DateOnly ExpiryDate { get; set; }

    public bool IsExpired { get; set; }
}

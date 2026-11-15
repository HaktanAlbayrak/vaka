namespace Vaka.Web.ViewModels.Records;

/// <summary>Reddedilen malzeme ekleme isteğinde seçili lot ve gerekçe korunur; PIN korunmaz.</summary>
public sealed class PendingMaterialViewModel
{
    public int MaterialLotId { get; set; }

    public string? ExpiredReason { get; set; }
}

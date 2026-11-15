namespace Vaka.Web.ViewModels.Records;

public sealed class AddMaterialViewModel
{
    public Guid CommandId { get; set; }

    public int MaterialLotId { get; set; }

    public string? ExpiredReason { get; set; }

    public string Pin { get; set; } = string.Empty;
}

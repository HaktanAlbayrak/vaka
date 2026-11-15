namespace Vaka.Web.ViewModels.Records;

public sealed class RecordMaterialViewModel
{
    public int Id { get; set; }

    public string MaterialCode { get; set; } = string.Empty;

    public string MaterialName { get; set; } = string.Empty;

    public string LotNo { get; set; } = string.Empty;

    public DateOnly ExpiryDate { get; set; }

    public bool ExpiredAtEntry { get; set; }

    public string? ExpiredReason { get; set; }

    public string EnteredByName { get; set; } = string.Empty;

    public DateTime EnteredAtLocal { get; set; }
}

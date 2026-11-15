namespace Vaka.Web.ViewModels.Records;

public sealed class RemoveMaterialViewModel
{
    public Guid CommandId { get; set; }

    public int RecordMaterialId { get; set; }

    public string Pin { get; set; } = string.Empty;
}

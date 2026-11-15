namespace Vaka.Web.ViewModels.Records;

public sealed class RecordClosureViewModel
{
    public string KindLabel { get; set; } = string.Empty;

    /// <summary>Zaman aşımında "Sistem".</summary>
    public string ClosedByName { get; set; } = string.Empty;

    public DateTime ClosedAtLocal { get; set; }

    public string Reason { get; set; } = string.Empty;
}

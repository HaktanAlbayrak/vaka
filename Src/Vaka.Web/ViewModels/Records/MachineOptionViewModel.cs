namespace Vaka.Web.ViewModels.Records;

public sealed class MachineOptionViewModel
{
    public int Id { get; set; }

    public string Label { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }

    /// <summary>Makine seçilemiyorsa nedeni (sahip adı, durum, açılış saati ya da pasif); seçilebilirse boş.</summary>
    public string? BusyReason { get; set; }
}

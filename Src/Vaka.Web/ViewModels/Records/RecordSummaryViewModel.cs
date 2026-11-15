namespace Vaka.Web.ViewModels.Records;

/// <summary>Kayıt toplamları; kayıt tamamlanınca doludur. <c>StartedAtLocal</c> ilk adım başlangıcıdır, açılış anı değildir (BR-13).</summary>
public sealed class RecordSummaryViewModel
{
    public int? GrossSeconds { get; set; }

    public int? BreakSeconds { get; set; }

    public int? NetSeconds { get; set; }

    public int? EffortSeconds { get; set; }

    public DateTime? StartedAtLocal { get; set; }

    public DateTime? CompletedAtLocal { get; set; }
}

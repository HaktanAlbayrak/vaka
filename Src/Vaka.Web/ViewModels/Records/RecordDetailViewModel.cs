namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailViewModel
{
    public int Id { get; set; }

    public RecordDetailHeaderViewModel Header { get; set; } = new();

    public RecordDetailPeopleViewModel People { get; set; } = new();

    public RecordDetailTimesViewModel Times { get; set; } = new();

    public RecordDetailSummaryViewModel Summary { get; set; } = new();

    public RecordDetailPhaseViewModel[] Phases { get; set; } = [];

    public RecordDetailBreakViewModel[] Breaks { get; set; } = [];

    public RecordMaterialViewModel[] Materials { get; set; } = [];

    public RecordTimelineDayViewModel[] Timeline { get; set; } = [];

    public RecordDetailEventViewModel[] Events { get; set; } = [];

    public RecordIntegrityViewModel Integrity { get; set; } = new();

    public RecordClosureViewModel? Closure { get; set; }

    public RecordDetailOpenBreakViewModel? OpenBreak { get; set; }

    public bool CanIntervene { get; set; }

    public string? InterventionUrl { get; set; }

    public bool CanEndBreak { get; set; }

    public string? EndBreakUrl { get; set; }

    /// <summary>Yalnız Operatör'de ve kayıt Açıldı, Devam ediyor ya da Beklemede iken dolu.</summary>
    public string? RunUrl { get; set; }

    public DateTime GeneratedAtLocal { get; set; }

    public string GeneratedByName { get; set; } = string.Empty;
}

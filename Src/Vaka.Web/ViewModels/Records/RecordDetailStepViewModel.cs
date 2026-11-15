using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordDetailStepViewModel
{
    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public StepMediaViewModel[] Media { get; set; } = [];

    public string StatusLabel { get; set; } = string.Empty;

    public DateTime? StartedAtLocal { get; set; }

    public string? StartedByName { get; set; }

    public DateTime? EndedAtLocal { get; set; }

    public string? EndedByName { get; set; }

    public int? BreakSeconds { get; set; }

    public int? DurationSeconds { get; set; }

    public int? EffortSeconds { get; set; }

    public RecordDetailParticipantViewModel[] Participants { get; set; } = [];
}

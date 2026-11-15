using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordPhaseViewModel
{
    public int Id { get; set; }

    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public int? MinDurationSeconds { get; set; }

    public PhaseStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public RecordStepViewModel[] Steps { get; set; } = [];
}

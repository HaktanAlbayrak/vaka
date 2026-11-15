using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordListFilterViewModel
{
    public string? Query { get; set; }

    public int? FacilityId { get; set; }

    public int? LineId { get; set; }

    public int? MachineId { get; set; }

    public CleaningTypeKind? Kind { get; set; }

    public RecordStatus? Status { get; set; }

    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public bool MineOnly { get; set; }
}

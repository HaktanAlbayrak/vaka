using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Reports;

public sealed class MachineReportFilterViewModel
{
    public DateOnly? From { get; set; }

    public DateOnly? To { get; set; }

    public int? FacilityId { get; set; }

    public int? LineId { get; set; }

    public int? MachineId { get; set; }

    public CleaningTypeKind? Kind { get; set; }
}

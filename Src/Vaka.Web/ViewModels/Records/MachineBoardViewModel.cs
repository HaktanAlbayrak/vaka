using Vaka.Web.ViewModels.Definitions;

namespace Vaka.Web.ViewModels.Records;

public sealed class MachineBoardViewModel
{
    public int? FacilityId { get; set; }

    public SelectOptionViewModel[] Facilities { get; set; } = [];

    public MachineBoardGroupViewModel[] Groups { get; set; } = [];

    public DateTime GeneratedAtLocal { get; set; }
}

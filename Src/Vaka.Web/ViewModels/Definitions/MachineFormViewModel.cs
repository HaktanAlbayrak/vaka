namespace Vaka.Web.ViewModels.Definitions;

public sealed class MachineFormViewModel
{
    public int? Id { get; set; }

    public int FacilityId { get; set; }

    public int LineId { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public bool CodeLocked { get; set; }

    public DateTime? RetiredAtLocal { get; set; }

    public SelectOptionViewModel[] Facilities { get; set; } = [];

    public SelectOptionViewModel[] Lines { get; set; } = [];
}

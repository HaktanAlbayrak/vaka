using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class CleaningTypeDetailViewModel
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public int? DefaultLockMinutes { get; set; }

    public bool IsActive { get; set; }

    public int MachineCount { get; set; }

    public bool CanMapMachines { get; set; }

    public ProcedureVersionItemViewModel[] Versions { get; set; } = [];

    public int? DraftId { get; set; }

    public bool CanCreateDraft { get; set; }

    public string? StatusMessage { get; set; }
}

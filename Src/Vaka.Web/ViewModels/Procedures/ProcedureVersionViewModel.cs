using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class ProcedureVersionViewModel
{
    public int Id { get; set; }

    public int CleaningTypeId { get; set; }

    public string CleaningTypeName { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public ProcedureVersionStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public DateTime? PublishedAtLocal { get; set; }

    public string? PublishedByName { get; set; }

    public string? DocumentRef { get; set; }

    public bool MaterialRequired { get; set; }

    public VersionPhaseViewModel[] Phases { get; set; } = [];

    public string? StatusMessage { get; set; }
}

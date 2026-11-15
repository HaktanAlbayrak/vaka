using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class ProcedureDraftViewModel
{
    public int Id { get; set; }

    public int CleaningTypeId { get; set; }

    public string CleaningTypeName { get; set; } = string.Empty;

    public CleaningTypeKind Kind { get; set; }

    public string KindLabel { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public bool MaterialRequired { get; set; }

    public string? DocumentRef { get; set; }

    public DraftPhaseViewModel[] Phases { get; set; } = [];

    public string[] PublishBlockers { get; set; } = [];

    public int MaxPhotoSizeMb { get; set; }

    public int MaxVideoSizeMb { get; set; }

    public string MediaAccept { get; set; } = string.Empty;

    public DraftFormErrorViewModel? Error { get; set; }

    public string? StatusMessage { get; set; }
}

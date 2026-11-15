using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Procedures;

public sealed class ProcedureVersionItemViewModel
{
    public int Id { get; set; }

    public int VersionNo { get; set; }

    public ProcedureVersionStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    public DateTime? PublishedAtLocal { get; set; }

    public string? PublishedByName { get; set; }

    public string? DocumentRef { get; set; }

    public string Url { get; set; } = string.Empty;
}

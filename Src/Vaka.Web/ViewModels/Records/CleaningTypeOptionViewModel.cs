namespace Vaka.Web.ViewModels.Records;

public sealed class CleaningTypeOptionViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public bool MaterialRequired { get; set; }

    public int DefaultLockMinutes { get; set; }
}

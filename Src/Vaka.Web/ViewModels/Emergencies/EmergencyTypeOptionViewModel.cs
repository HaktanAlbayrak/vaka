namespace Vaka.Web.ViewModels.Emergencies;

public sealed class EmergencyTypeOptionViewModel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int VersionNo { get; set; }

    public bool MaterialRequired { get; set; }
}

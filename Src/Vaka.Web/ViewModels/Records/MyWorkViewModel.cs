namespace Vaka.Web.ViewModels.Records;

/// <summary>Faz 5 ve Faz 7 alanları (atanan adımlar, bekleyen kayıtlar, açık mola) o fazlarda eklenir.</summary>
public sealed class MyWorkViewModel
{
    public ActiveRecordViewModel? ActiveRecord { get; set; }

    public bool CanOpenRecord { get; set; }

    public string? CannotOpenReason { get; set; }

    public string? CannotOpenUrl { get; set; }
}

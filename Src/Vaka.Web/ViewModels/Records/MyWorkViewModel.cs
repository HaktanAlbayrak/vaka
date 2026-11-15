namespace Vaka.Web.ViewModels.Records;

/// <summary>Faz 7 alanları (bekleyen kayıtlar) o fazda eklenir.</summary>
public sealed class MyWorkViewModel
{
    public ActiveRecordViewModel? ActiveRecord { get; set; }

    public bool CanOpenRecord { get; set; }

    public string? CannotOpenReason { get; set; }

    public string? CannotOpenUrl { get; set; }

    public AssignedStepViewModel[] AssignedSteps { get; set; } = [];

    public MyWorkBreakViewModel? OpenBreak { get; set; }
}

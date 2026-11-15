using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Interventions;

public sealed class InterventionViewModel
{
    public Guid CommandId { get; set; }

    public int RecordId { get; set; }

    public string RecordNo { get; set; } = string.Empty;

    public string MachineLabel { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string RecordStatusLabel { get; set; } = string.Empty;

    public InterventionType? Type { get; set; }

    public InterventionTypeOptionViewModel[] AllowedTypes { get; set; } = [];

    public string Reason { get; set; } = string.Empty;

    public string Pin { get; set; } = string.Empty;

    public string? InProgressStepTitle { get; set; }

    public InterventionBreakViewModel? OpenBreak { get; set; }

    /// <summary>Dolu ise form çizilmez ve gönderilemez; neden metindir.</summary>
    public string? BlockReason { get; set; }

    /// <summary>Reddedilen isteğin alana bağlı olmayan nedeni; alan hataları ModelState'te (<c>Type</c>, <c>Reason</c>, <c>Pin</c>).</summary>
    public string? ErrorMessage { get; set; }
}

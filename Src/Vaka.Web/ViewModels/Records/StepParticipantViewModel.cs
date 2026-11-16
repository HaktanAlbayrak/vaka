using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class StepParticipantViewModel
{
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string RegistryNo { get; set; } = string.Empty;

    public bool IsOwner { get; set; }

    public DateTime AddedAtLocal { get; set; }

    public ParticipantState State { get; set; }

    public DateTime? JoinedAtLocal { get; set; }

    public DateTime? LeftAtLocal { get; set; }

    public int? WorkSeconds { get; set; }

    public bool CanJoin { get; set; }

    public bool CanLeave { get; set; }

    public bool CanRemove { get; set; }

    /// <summary>Katılma ya da ayrılma şu an mümkün değilse nedeni; görünüm düğme yerine bu metni yazar.</summary>
    public string? BlockedReason { get; set; }
}

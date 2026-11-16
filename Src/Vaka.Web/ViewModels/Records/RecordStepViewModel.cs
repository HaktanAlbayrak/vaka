using Vaka.Web.Services;
using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordStepViewModel
{
    public int Id { get; set; }

    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public StepMediaViewModel[] Media { get; set; } = [];

    public StepStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    /// <summary>Önceki adım tamamlanmış ve bu adım bekliyor (ilk adım için kayıt başlamamış olması yeter).</summary>
    public bool IsNext { get; set; }

    public bool IsLastOfPhase { get; set; }

    public DateTime? StartedAtLocal { get; set; }

    public string? StartedByName { get; set; }

    public DateTime? EndedAtLocal { get; set; }

    public string? EndedByName { get; set; }

    public int? BreakSeconds { get; set; }

    public int? DurationSeconds { get; set; }

    public int? EffortSeconds { get; set; }

    /// <summary>Devam eden adımın başlangıçtan bu yana geçen süresi, sunucu hesabı; mola açıkken ilerlemez.</summary>
    public int? LiveElapsedSeconds { get; set; }

    public StepParticipantViewModel[] Participants { get; set; } = [];

    public bool CanStart { get; set; }

    public bool CanFinish { get; set; }

    public string? CannotStartReason { get; set; }

    /// <summary>Bitirme kapısı doluysa <c>Girişi bekleniyor: ...</c> metnini de taşır.</summary>
    public string? CannotFinishReason { get; set; }

    /// <summary>Sahip dışında girişini yapmamış kişilerin adları; boşsa kapı açık.</summary>
    public string[] PendingNames { get; set; } = [];

    public bool CanManageParticipants { get; set; }
}

using Vaka.Web.Services;

namespace Vaka.Web.ViewModels.Records;

public sealed class RecordStepViewModel
{
    public int Id { get; set; }

    public int OrderNo { get; set; }

    public string Title { get; set; } = string.Empty;

    public StepStatus Status { get; set; }

    public string StatusLabel { get; set; } = string.Empty;

    /// <summary>Önceki adım tamamlanmış ve bu adım bekliyor (ilk adım için kayıt başlamamış olması yeter).</summary>
    public bool IsNext { get; set; }
}

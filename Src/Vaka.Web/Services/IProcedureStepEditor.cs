using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed record StepRequest(string? Title, string? Description);

/// <summary><see cref="IProcedureDraftEditor"/> ile aynı sonuç sözleşmesi: taslak olmayan versiyonda <see cref="CommandStatus.NotEditable"/>, <c>EntityId</c> versiyon kimliği.</summary>
public interface IProcedureStepEditor
{
    Task<CommandResult> AddStepAsync(int phaseId, StepRequest request, CancellationToken cancellationToken);

    Task<CommandResult> UpdateStepAsync(int stepId, StepRequest request, CancellationToken cancellationToken);

    Task<CommandResult> MoveStepAsync(int stepId, MoveDirection direction, CancellationToken cancellationToken);

    /// <summary>Adımın medya bağlantıları da çıkar; medya dosyası silinmez.</summary>
    Task<CommandResult> RemoveStepAsync(int stepId, CancellationToken cancellationToken);
}

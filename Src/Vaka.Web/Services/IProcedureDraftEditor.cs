using Vaka.Web.ViewModels.Procedures;

namespace Vaka.Web.Services;

public sealed record HeaderRequest(bool MaterialRequired, string? DocumentRef);

public sealed record PhaseRequest(string? Title, int? MinDurationMinutes);

/// <summary>
/// Her işlem önce versiyonun taslak olduğunu denetler; yayınlanmış versiyonda veri değişmez ve <see cref="CommandStatus.NotEditable"/> döner (BR-04).
/// Başarı ve ret sonuçlarında <see cref="CommandResult.EntityId"/> versiyon kimliğidir; kayıt yoksa <see cref="CommandStatus.NotFound"/>.
/// </summary>
public interface IProcedureDraftEditor
{
    Task<CommandResult> UpdateHeaderAsync(int versionId, HeaderRequest request, CancellationToken cancellationToken);

    Task<CommandResult> AddPhaseAsync(int versionId, PhaseRequest request, CancellationToken cancellationToken);

    Task<CommandResult> UpdatePhaseAsync(int phaseId, PhaseRequest request, CancellationToken cancellationToken);

    Task<CommandResult> MovePhaseAsync(int phaseId, MoveDirection direction, CancellationToken cancellationToken);

    /// <summary>Fazın adımları ve medya bağlantıları da çıkarır; medya dosyası silinmez (başka versiyon paylaşabilir).</summary>
    Task<CommandResult> RemovePhaseAsync(int phaseId, CancellationToken cancellationToken);
}

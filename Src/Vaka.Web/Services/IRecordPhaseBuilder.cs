using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public interface IRecordPhaseBuilder
{
    /// <summary>
    /// Versiyonun fazlarından ve adımlarından, sıra no sırasıyla, kayda eklenecek bekleyen faz ve adım satırlarını kurar (BR-14).
    /// Her adımın görevlisi kaydı açan kişidir; faz minimumu versiyondan kopyalanır (acil tipte boştur).
    /// </summary>
    Task<RecordPhase[]> BuildAsync(int procedureVersionId, int ownerUserId, CancellationToken cancellationToken);
}

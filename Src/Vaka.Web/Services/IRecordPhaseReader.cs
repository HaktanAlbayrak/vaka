using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record PhaseSection(RecordDetailPhaseViewModel[] Phases, string? FirstStepStartedByName, RecordHelperViewModel[] Helpers);

public interface IRecordPhaseReader
{
    /// <summary>
    /// Kaydın faz ve adımları sıralı, görevlileri ve prosedür versiyonundaki adım içeriğiyle (FR-09). Adım ve faz süreleri kayıtta saklanan
    /// değerlerdir (SH-01..SH-07); bitmemiş adımda boştur. Görevlinin eklenme anı <c>ParticipantAdded</c> olayından, açılışta eklenen için kayıt açılış anından okunur; onay anı yalnız <c>ConfirmedAtUtc</c> doluysa gelir (Faz 7).
    /// <c>Helpers</c> kayıt sahibi dışında herhangi bir adımın görevlisi olmuş kişilerdir.
    /// </summary>
    Task<PhaseSection> ReadAsync(int recordId, int ownerUserId, DateTime openedAtUtc, CancellationToken cancellationToken);
}

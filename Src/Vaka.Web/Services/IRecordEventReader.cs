using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record RecordEventSection(RecordDetailEventViewModel[] Events, RecordTimelineDayViewModel[] Timeline);

public interface IRecordEventReader
{
    /// <summary>Kaydın olay günlüğü satırları ve gün gün zaman çizelgesi; sıra numarasına göre, kişi ve adım adları çözülmüş (PL-95, PL-96).</summary>
    Task<RecordEventSection> ReadAsync(int recordId, CancellationToken cancellationToken);
}

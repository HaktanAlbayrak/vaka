using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record RunProgress(
    RecordPhaseViewModel[] Phases,
    bool CanStartBreak,
    string? CannotBreakReason,
    RecordSummaryViewModel Summary,
    string? FirstStepStartedByName);

public interface IRunProgressReader
{
    /// <summary>
    /// Fazlar, adımlar, görevliler, görüntüleyen için eylem bayrakları ve nedenleri, mola başlatma durumu ve kayıt özeti.
    /// Bayraklar yalnız ekranı yönlendirir; asıl denetim ilgili servistedir. Canlı değerler SH-14 ile sunucuda hesaplanır;
    /// kayıtta açık mola varsa değerler ilerlemez. Görevlinin eklenme anı <c>ParticipantAdded</c> olayından okunur, ilk görevli (sahip) için kayıt açılış anıdır.
    /// </summary>
    Task<RunProgress> ReadAsync(RunContext context, RunBreakPanel breaks, DateTime openedAtUtc, CancellationToken cancellationToken);
}

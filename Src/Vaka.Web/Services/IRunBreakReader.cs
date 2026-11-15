using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed record RunBreakPanel(RecordBreakViewModel[] Breaks, OpenBreakViewModel? OpenBreak, IReadOnlyList<BreakSpan> Spans);

public interface IRunBreakReader
{
    /// <summary>
    /// Kaydın molaları başlangıç sırasıyla, açık mola ve sayaç hesabı için tüm aralıklar. Açık molanın geçen süresi <c>ElapsedSeconds</c>
    /// (<see cref="IDurationCalculator.CalculateBreakSeconds"/>), kapalı molanın süresi <c>DurationSeconds</c> aynı formülle yazılır.
    /// </summary>
    Task<RunBreakPanel> ReadAsync(RunContext context, CancellationToken cancellationToken);
}

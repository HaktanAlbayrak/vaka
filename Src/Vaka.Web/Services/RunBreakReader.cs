using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RunBreakReader : IRunBreakReader
{
    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;

    public RunBreakReader(VakaDbContext db, IDurationCalculator calculator, ITurkeyClock clock)
    {
        _db = db;
        _calculator = calculator;
        _clock = clock;
    }

    public async Task<RunBreakPanel> ReadAsync(RunContext context, CancellationToken cancellationToken)
    {
        var rows = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == context.RecordId)
            .OrderBy(item => item.StartedAtUtc).ThenBy(item => item.Id)
            .Select(item => new
            {
                item.Id,
                StartedBy = item.StartedBy.FullName,
                item.StartedAtUtc,
                item.EndedAtUtc,
                item.EndKind,
                DuringStepTitle = item.DuringStep == null ? null : item.DuringStep.ProcedureStep.Title
            })
            .ToListAsync(cancellationToken);

        var breaks = rows.Select(row => new RecordBreakViewModel
        {
            StartedByName = row.StartedBy,
            StartedAtLocal = _clock.ToLocal(row.StartedAtUtc),
            EndedAtLocal = row.EndedAtUtc is { } ended ? _clock.ToLocal(ended) : null,
            DurationSeconds = row.EndedAtUtc is { } end ? _calculator.CalculateBreakSeconds(UtcMoments.Of(row.StartedAtUtc), UtcMoments.Of(end)) : null,
            EndKindLabel = RecordLabels.BreakEnd(row.EndKind),
            DuringStepTitle = row.DuringStepTitle
        }).ToArray();

        var open = rows.FirstOrDefault(row => row.EndedAtUtc is null);
        var openBreak = open is null
            ? null
            : new OpenBreakViewModel
            {
                Id = open.Id,
                StartedByName = open.StartedBy,
                StartedAtLocal = _clock.ToLocal(open.StartedAtUtc),
                DuringStepTitle = open.DuringStepTitle,
                ElapsedSeconds = _calculator.CalculateBreakSeconds(UtcMoments.Of(open.StartedAtUtc), context.Now)
            };

        return new RunBreakPanel(breaks, openBreak, [.. rows.Select(row => UtcMoments.Span(row.StartedAtUtc, row.EndedAtUtc))]);
    }
}

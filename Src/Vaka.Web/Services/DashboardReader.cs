using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Dashboard;

namespace Vaka.Web.Services;

public sealed class DashboardReader : IDashboardReader
{
    private const int ListLimit = 10;
    private const int DayCount = 7;
    private static readonly CultureInfo TurkishCulture = CultureInfo.GetCultureInfo("tr-TR");

    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ITurkeyClock _clock;

    public DashboardReader(VakaDbContext db, IRecordTimeouts timeouts, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _clock = clock;
    }

    public async Task<DashboardViewModel> GetAsync(CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);

        var today = _clock.Today();
        var todayStart = _clock.StartOfDayUtc(today);
        var tomorrowStart = _clock.StartOfDayUtc(today.AddDays(1));

        return new DashboardViewModel
        {
            OpenCount = await _db.CleaningRecords.CountAsync(record => record.Status == RecordStatus.Opened, cancellationToken),
            InProgressCount = await _db.CleaningRecords.CountAsync(record => record.Status == RecordStatus.InProgress, cancellationToken),
            CompletedTodayCount = await _db.CleaningRecords.CountAsync(
                record => record.Status == RecordStatus.Completed && record.CompletedAtUtc >= todayStart && record.CompletedAtUtc < tomorrowStart,
                cancellationToken),
            ActiveRecords = await ReadActiveAsync(cancellationToken),
            BelowMinimum = await ReadBelowMinimumAsync(cancellationToken),
            RecentCompleted = await ReadCompletedAsync(cancellationToken),
            Last7Days = await ReadLastDaysAsync(today, cancellationToken)
        };
    }

    private async Task<DashboardActiveRecordViewModel[]> ReadActiveAsync(CancellationToken cancellationToken)
    {
        var rows = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress)
            .OrderBy(record => record.OpenedAtUtc).ThenBy(record => record.Id)
            .Select(record => new
            {
                record.Id,
                record.RecordNo,
                record.Kind,
                record.Status,
                Machine = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                OwnerName = record.Owner.FullName,
                record.OpenedAtUtc,
                record.LockExpiresAtUtc,
                StepTitle = record.Phases
                    .SelectMany(phase => phase.Steps)
                    .Where(step => step.Status == StepStatus.InProgress)
                    .Select(step => step.ProcedureStep.Title)
                    .FirstOrDefault()
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new DashboardActiveRecordViewModel
        {
            RecordId = row.Id,
            RecordNo = row.RecordNo,
            Kind = row.Kind,
            KindLabel = ProcedureLabels.Kind(row.Kind),
            Status = row.Status,
            StatusLabel = RecordLabels.Status(row.Status),
            MachineLabel = row.Machine,
            OwnerName = row.OwnerName,
            OpenedAtLocal = _clock.ToLocal(row.OpenedAtUtc),
            LockExpiresAtLocal = row.Status == RecordStatus.Opened && row.LockExpiresAtUtc is { } expires ? _clock.ToLocal(expires) : null,
            CurrentStepTitle = row.StepTitle,
            Url = RecordLinks.Details(row.Id)
        })];
    }

    private async Task<DashboardBelowMinimumViewModel[]> ReadBelowMinimumAsync(CancellationToken cancellationToken)
    {
        var rows = await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phase.BelowMinimum && phase.Record.Status == RecordStatus.Completed && phase.Record.CompletedAtUtc != null)
            .OrderByDescending(phase => phase.Record.CompletedAtUtc).ThenByDescending(phase => phase.Id)
            .Take(ListLimit)
            .Select(phase => new
            {
                phase.CleaningRecordId,
                phase.Record.RecordNo,
                Machine = phase.Record.MachineCodeSnapshot + " - " + phase.Record.MachineNameSnapshot,
                phase.ProcedurePhase.Title,
                Net = phase.NetSeconds ?? 0,
                Minimum = phase.MinDurationSecondsSnapshot ?? 0,
                phase.BelowMinimumReason,
                CompletedAtUtc = phase.Record.CompletedAtUtc!.Value
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new DashboardBelowMinimumViewModel
        {
            RecordId = row.CleaningRecordId,
            RecordNo = row.RecordNo,
            MachineLabel = row.Machine,
            PhaseTitle = row.Title,
            NetSeconds = row.Net,
            MinDurationSeconds = row.Minimum,
            Reason = row.BelowMinimumReason ?? string.Empty,
            CompletedAtLocal = _clock.ToLocal(row.CompletedAtUtc),
            Url = RecordLinks.Details(row.CleaningRecordId)
        })];
    }

    private async Task<DashboardCompletedViewModel[]> ReadCompletedAsync(CancellationToken cancellationToken)
    {
        var rows = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Status == RecordStatus.Completed && record.CompletedAtUtc != null)
            .OrderByDescending(record => record.CompletedAtUtc).ThenByDescending(record => record.Id)
            .Take(ListLimit)
            .Select(record => new
            {
                record.Id,
                record.RecordNo,
                record.Kind,
                Machine = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                CompletedAtUtc = record.CompletedAtUtc!.Value,
                Net = record.NetSeconds ?? 0,
                Effort = record.EffortSeconds ?? 0
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new DashboardCompletedViewModel
        {
            RecordId = row.Id,
            RecordNo = row.RecordNo,
            Kind = row.Kind,
            KindLabel = ProcedureLabels.Kind(row.Kind),
            MachineLabel = row.Machine,
            CompletedAtLocal = _clock.ToLocal(row.CompletedAtUtc),
            NetSeconds = row.Net,
            EffortSeconds = row.Effort,
            Url = RecordLinks.Details(row.Id)
        })];
    }

    private async Task<DashboardDayCountViewModel[]> ReadLastDaysAsync(DateOnly today, CancellationToken cancellationToken)
    {
        var first = today.AddDays(1 - DayCount);
        var from = _clock.StartOfDayUtc(first);
        var to = _clock.StartOfDayUtc(today.AddDays(1));

        var opened = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.OpenedAtUtc >= from && record.OpenedAtUtc < to)
            .Select(record => new { record.Kind, record.OpenedAtUtc })
            .ToListAsync(cancellationToken);

        var perDay = opened
            .GroupBy(row => DateOnly.FromDateTime(_clock.ToLocal(row.OpenedAtUtc)))
            .ToDictionary(group => group.Key, group => group.ToList());

        return [.. Enumerable.Range(0, DayCount).Select(offset =>
        {
            var day = first.AddDays(offset);
            var rows = perDay.GetValueOrDefault(day) ?? [];
            return new DashboardDayCountViewModel
            {
                Date = day,
                DateLabel = day.ToString("d MMM ddd", TurkishCulture),
                PlannedCount = rows.Count(row => row.Kind == CleaningTypeKind.Planned),
                EmergencyCount = rows.Count(row => row.Kind == CleaningTypeKind.Emergency)
            };
        })];
    }
}

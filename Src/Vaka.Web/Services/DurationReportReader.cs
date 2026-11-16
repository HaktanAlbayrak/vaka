using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public sealed class DurationReportReader : IDurationReportReader
{
    private const int LongestStepCount = 10;

    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly IDurationCalculator _calculator;
    private readonly IReportOptions _options;
    private readonly ITurkeyClock _clock;

    public DurationReportReader(VakaDbContext db, IRecordTimeouts timeouts, IDurationCalculator calculator, IReportOptions options, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _calculator = calculator;
        _options = options;
        _clock = clock;
    }

    public async Task<DurationReportViewModel> BuildAsync(DurationReportFilterViewModel filter, string? inputError, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);
        (filter.FacilityId, filter.LineId, filter.MachineId) = await _options.ReconcileSelectionAsync(filter.FacilityId, filter.LineId, filter.MachineId, cancellationToken);

        var problem = inputError
            ?? (filter.From > filter.To ? ReportMessages.InvalidRange : null)
            ?? (filter.Kind is { } kind && !Enum.IsDefined(kind) ? ReportMessages.InvalidFilter : null)
            ?? await _options.FindInvalidSelectionAsync(filter.FacilityId, filter.LineId, filter.MachineId, filter.CleaningTypeId, cancellationToken);
        if (problem is not null)
        {
            var empty = await BuildEmptyAsync(filter, cancellationToken);
            empty.ErrorMessage = problem;
            return empty;
        }

        var records = Filter(_db.CleaningRecords.AsNoTracking(), filter);
        var completed = records.Where(record => record.Status == RecordStatus.Completed);

        return new DurationReportViewModel
        {
            Filter = filter,
            ByMachine = await ReadByMachineAsync(completed, filter, cancellationToken),
            ByPhase = await ReadByPhaseAsync(completed, cancellationToken),
            LongestSteps = await ReadLongestStepsAsync(completed, cancellationToken),
            StatusCounts = await ReadStatusCountsAsync(records, cancellationToken),
            Facilities = await _options.ListFacilitiesAsync(cancellationToken),
            Lines = await _options.ListLinesAsync(filter.FacilityId, cancellationToken),
            Machines = await _options.ListMachinesAsync(filter.FacilityId, filter.LineId, cancellationToken),
            CleaningTypes = await _options.ListCleaningTypesAsync(cancellationToken),
            KindOptions = ProcedureLabels.KindOptions()
        };
    }

    private async Task<DurationReportViewModel> BuildEmptyAsync(DurationReportFilterViewModel filter, CancellationToken cancellationToken) =>
        new()
        {
            Filter = filter,
            StatusCounts = [.. Enum.GetValues<RecordStatus>().Select(status => new StatusCountViewModel { StatusLabel = RecordLabels.Status(status) })],
            Facilities = await _options.ListFacilitiesAsync(cancellationToken),
            Lines = await _options.ListLinesAsync(filter.FacilityId, cancellationToken),
            Machines = await _options.ListMachinesAsync(filter.FacilityId, filter.LineId, cancellationToken),
            CleaningTypes = await _options.ListCleaningTypesAsync(cancellationToken),
            KindOptions = ProcedureLabels.KindOptions()
        };

    private IQueryable<CleaningRecord> Filter(IQueryable<CleaningRecord> records, DurationReportFilterViewModel filter)
    {
        if (filter.FacilityId is { } facilityId)
        {
            records = records.Where(record => record.Machine.Line.FacilityId == facilityId);
        }

        if (filter.LineId is { } lineId)
        {
            records = records.Where(record => record.Machine.ProductionLineId == lineId);
        }

        if (filter.MachineId is { } machineId)
        {
            records = records.Where(record => record.MachineId == machineId);
        }

        if (filter.CleaningTypeId is { } typeId)
        {
            records = records.Where(record => record.ProcedureVersion.CleaningTypeId == typeId);
        }

        if (filter.Kind is { } kind)
        {
            records = records.Where(record => record.Kind == kind);
        }

        if (filter.From is { } from)
        {
            var start = _clock.StartOfDayUtc(from);
            records = records.Where(record => record.OpenedAtUtc >= start);
        }

        if (filter.To is { } to)
        {
            var end = _clock.StartOfDayUtc(to.AddDays(1));
            records = records.Where(record => record.OpenedAtUtc < end);
        }

        return records;
    }

    private async Task<MachineDurationRowViewModel[]> ReadByMachineAsync(IQueryable<CleaningRecord> completed, DurationReportFilterViewModel filter, CancellationToken cancellationToken)
    {
        var rows = await completed
            .Select(record => new
            {
                record.MachineId,
                MachineLabel = record.FacilityCodeSnapshot + "-" + record.LineCodeSnapshot + "-" + record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                TypeId = record.ProcedureVersion.CleaningTypeId,
                TypeName = record.ProcedureVersion.CleaningType.Name,
                Gross = record.GrossSeconds ?? 0,
                Break = record.BreakSeconds ?? 0,
                Net = record.NetSeconds ?? 0,
                Effort = record.EffortSeconds ?? 0
            })
            .ToListAsync(cancellationToken);

        return [.. rows
            .GroupBy(row => new { row.MachineId, row.TypeId })
            .Select(group => new MachineDurationRowViewModel
            {
                MachineLabel = group.First().MachineLabel,
                MachineReportUrl = MachineReportLinks.For(group.Key.MachineId, filter.From, filter.To, filter.Kind),
                CleaningTypeName = group.First().TypeName,
                RecordCount = group.Count(),
                AvgGrossSeconds = _calculator.CalculateAverage([.. group.Select(row => row.Gross)]),
                AvgBreakSeconds = _calculator.CalculateAverage([.. group.Select(row => row.Break)]),
                AvgNetSeconds = _calculator.CalculateAverage([.. group.Select(row => row.Net)]),
                AvgEffortSeconds = _calculator.CalculateAverage([.. group.Select(row => row.Effort)])
            })
            .OrderBy(row => row.MachineLabel, StringComparer.CurrentCulture).ThenBy(row => row.CleaningTypeName, StringComparer.CurrentCulture)];
    }

    private async Task<PhaseDurationRowViewModel[]> ReadByPhaseAsync(IQueryable<CleaningRecord> completed, CancellationToken cancellationToken)
    {
        var ids = completed.Select(record => record.Id);
        var rows = await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => ids.Contains(phase.CleaningRecordId) && phase.NetSeconds != null)
            .Select(phase => new
            {
                phase.ProcedurePhaseId,
                TypeName = phase.ProcedurePhase.Version.CleaningType.Name,
                phase.ProcedurePhase.Version.VersionNo,
                phase.ProcedurePhase.OrderNo,
                phase.ProcedurePhase.Title,
                phase.ProcedurePhase.MinDurationSeconds,
                Net = phase.NetSeconds!.Value,
                phase.BelowMinimum
            })
            .ToListAsync(cancellationToken);

        return [.. rows
            .GroupBy(row => row.ProcedurePhaseId)
            .Select(group => new PhaseDurationRowViewModel
            {
                CleaningTypeName = group.First().TypeName,
                VersionNo = group.First().VersionNo,
                PhaseOrderNo = group.First().OrderNo,
                PhaseTitle = group.First().Title,
                MinDurationSeconds = group.First().MinDurationSeconds,
                RecordCount = group.Count(),
                AvgNetSeconds = _calculator.CalculateAverage([.. group.Select(row => row.Net)]),
                BelowMinimumCount = group.Count(row => row.BelowMinimum)
            })
            .OrderBy(row => row.CleaningTypeName, StringComparer.CurrentCulture).ThenBy(row => row.VersionNo).ThenBy(row => row.PhaseOrderNo)];
    }

    private async Task<LongestStepRowViewModel[]> ReadLongestStepsAsync(IQueryable<CleaningRecord> completed, CancellationToken cancellationToken)
    {
        var ids = completed.Select(record => record.Id);
        var rows = await _db.RecordSteps
            .AsNoTracking()
            .Where(step => ids.Contains(step.Phase.CleaningRecordId) && step.DurationSeconds != null)
            .Select(step => new
            {
                step.ProcedureStepId,
                TypeName = step.Phase.ProcedurePhase.Version.CleaningType.Name,
                step.Phase.ProcedurePhase.Version.VersionNo,
                PhaseTitle = step.Phase.ProcedurePhase.Title,
                StepTitle = step.ProcedureStep.Title,
                Duration = step.DurationSeconds!.Value
            })
            .ToListAsync(cancellationToken);

        return [.. rows
            .GroupBy(row => row.ProcedureStepId)
            .Select(group => new LongestStepRowViewModel
            {
                CleaningTypeName = group.First().TypeName,
                VersionNo = group.First().VersionNo,
                PhaseTitle = group.First().PhaseTitle,
                StepTitle = group.First().StepTitle,
                RecordCount = group.Count(),
                AvgDurationSeconds = _calculator.CalculateAverage([.. group.Select(row => row.Duration)]),
                MaxDurationSeconds = group.Max(row => row.Duration)
            })
            .OrderByDescending(row => row.AvgDurationSeconds).ThenByDescending(row => row.MaxDurationSeconds)
            .Take(LongestStepCount)];
    }

    private static async Task<StatusCountViewModel[]> ReadStatusCountsAsync(IQueryable<CleaningRecord> records, CancellationToken cancellationToken)
    {
        var counts = await records
            .GroupBy(record => record.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionaryAsync(row => row.Status, row => row.Count, cancellationToken);

        return [.. Enum.GetValues<RecordStatus>().Select(status => new StatusCountViewModel
        {
            StatusLabel = RecordLabels.Status(status),
            Count = counts.GetValueOrDefault(status)
        })];
    }
}

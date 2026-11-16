using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Reports;

namespace Vaka.Web.Services;

public sealed class MachineReportReader : IMachineReportReader
{
    private const int RecentCount = 20;

    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly IDurationCalculator _calculator;
    private readonly IReportOptions _options;
    private readonly IMaterialUsageReader _usage;
    private readonly ITurkeyClock _clock;

    public MachineReportReader(VakaDbContext db, IRecordTimeouts timeouts, IDurationCalculator calculator, IReportOptions options, IMaterialUsageReader usage, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _calculator = calculator;
        _options = options;
        _usage = usage;
        _clock = clock;
    }

    public async Task<MachineReportViewModel> BuildAsync(MachineReportFilterViewModel filter, string? inputError, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);
        (filter.FacilityId, filter.LineId, filter.MachineId) = await _options.ReconcileSelectionAsync(filter.FacilityId, filter.LineId, filter.MachineId, cancellationToken);

        var problem = inputError
            ?? (filter.From > filter.To ? ReportMessages.InvalidRange : null)
            ?? (filter.Kind is { } kind && !Enum.IsDefined(kind) ? ReportMessages.InvalidFilter : null)
            ?? await _options.FindInvalidSelectionAsync(filter.FacilityId, filter.LineId, filter.MachineId, null, cancellationToken);
        if (problem is not null)
        {
            var invalid = await BuildEmptyAsync(filter, cancellationToken);
            invalid.ErrorMessage = problem;
            return invalid;
        }

        if (filter.MachineId is not { } machineId)
        {
            return await BuildEmptyAsync(filter, cancellationToken);
        }

        var header = await ReadHeaderAsync(machineId, cancellationToken);

        var records = Filter(_db.CleaningRecords.AsNoTracking().Where(record => record.MachineId == machineId), filter);
        var model = await BuildEmptyAsync(filter, cancellationToken);
        model.Machine = header;
        model.StatusCounts = await ReadStatusCountsAsync(records, cancellationToken);
        (model.ByType, model.Recent) = await ReadRecordsAsync(records, cancellationToken);

        var usage = await _usage.ReadAsync(new MaterialUsageFilter
        {
            From = filter.From,
            To = filter.To,
            FacilityId = filter.FacilityId,
            LineId = filter.LineId,
            MachineId = machineId,
            Kind = filter.Kind
        }, cancellationToken);
        model.CompletedRecordCount = usage.CompletedRecordCount;
        model.Materials = usage.Rows;
        return model;
    }

    private async Task<MachineReportViewModel> BuildEmptyAsync(MachineReportFilterViewModel filter, CancellationToken cancellationToken) =>
        new()
        {
            Filter = filter,
            StatusCounts = [.. Enum.GetValues<RecordStatus>().Select(status => new StatusCountViewModel { StatusLabel = RecordLabels.Status(status) })],
            FacilityOptions = await _options.ListFacilitiesAsync(cancellationToken),
            LineOptions = await _options.ListLinesAsync(filter.FacilityId, cancellationToken),
            MachineOptions = await _options.ListMachinesAsync(filter.FacilityId, filter.LineId, cancellationToken),
            KindOptions = ProcedureLabels.KindOptions()
        };

    private async Task<MachineReportHeaderViewModel> ReadHeaderAsync(int machineId, CancellationToken cancellationToken)
    {
        var row = await _db.Machines
            .AsNoTracking()
            .Where(machine => machine.Id == machineId)
            .Select(machine => new
            {
                machine.Code,
                machine.Name,
                machine.RetiredAtUtc,
                LineCode = machine.Line.Code,
                LineName = machine.Line.Name,
                FacilityCode = machine.Line.Facility.Code,
                FacilityName = machine.Line.Facility.Name
            })
            .SingleAsync(cancellationToken);

        return new MachineReportHeaderViewModel
        {
            Label = row.FacilityCode + "-" + row.LineCode + "-" + row.Code + " - " + row.Name,
            LineLabel = row.LineCode + " - " + row.LineName,
            FacilityLabel = row.FacilityCode + " - " + row.FacilityName,
            RetiredAtLocal = row.RetiredAtUtc is { } retired ? _clock.ToLocal(retired) : null
        };
    }

    private IQueryable<CleaningRecord> Filter(IQueryable<CleaningRecord> records, MachineReportFilterViewModel filter)
    {
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

    private async Task<(MachineTypeRowViewModel[] ByType, MachineRecentRecordViewModel[] Recent)> ReadRecordsAsync(IQueryable<CleaningRecord> records, CancellationToken cancellationToken)
    {
        var rows = await records
            .Select(record => new RecordRow(
                record.Id,
                record.RecordNo,
                record.ProcedureVersion.CleaningTypeId,
                record.ProcedureVersion.CleaningType.Name,
                record.Kind,
                record.Status,
                record.OpenedAtUtc,
                record.GrossSeconds,
                record.BreakSeconds,
                record.NetSeconds,
                record.EffortSeconds))
            .ToListAsync(cancellationToken);

        var recent = rows.OrderByDescending(row => row.OpenedAtUtc).ThenByDescending(row => row.Id).Take(RecentCount).ToList();
        var phaseRecordIds = rows.Where(row => row.Status == RecordStatus.Completed).Select(row => row.Id).Union(recent.Select(row => row.Id)).ToList();
        var phases = (await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phaseRecordIds.Contains(phase.CleaningRecordId))
            .Select(phase => new PhaseRow(phase.CleaningRecordId, phase.MinDurationSecondsSnapshot, phase.NetSeconds, phase.BelowMinimum))
            .ToListAsync(cancellationToken))
            .ToLookup(phase => phase.RecordId);

        int? PlannedMinimum(int recordId) => _calculator.CalculateRecordPlannedMinimum([.. phases[recordId].Select(phase => phase.MinimumSnapshot)]);

        var byType = rows
            .GroupBy(row => row.TypeId)
            .Select(group => BuildTypeRow(group.ToList(), phases, PlannedMinimum))
            .OrderBy(row => row.CleaningTypeName, StringComparer.CurrentCulture);

        var recentRows = recent.Select(row => new MachineRecentRecordViewModel
        {
            RecordId = row.Id,
            RecordNo = row.RecordNo,
            CleaningTypeName = row.TypeName,
            KindLabel = ProcedureLabels.Kind(row.Kind),
            OpenedAtLocal = _clock.ToLocal(row.OpenedAtUtc),
            StatusLabel = RecordLabels.Status(row.Status),
            NetSeconds = row.Net,
            EffortSeconds = row.Effort,
            PlannedMinSeconds = PlannedMinimum(row.Id),
            Url = RecordLinks.Details(row.Id)
        });

        return ([.. byType], [.. recentRows]);
    }

    private MachineTypeRowViewModel BuildTypeRow(IReadOnlyList<RecordRow> group, ILookup<int, PhaseRow> phases, Func<int, int?> plannedMinimum)
    {
        var completed = group.Where(row => row.Status == RecordStatus.Completed).ToList();
        var row = new MachineTypeRowViewModel
        {
            CleaningTypeName = group[0].TypeName,
            KindLabel = ProcedureLabels.Kind(group[0].Kind),
            RecordCount = group.Count,
            CompletedCount = completed.Count
        };
        if (completed.Count == 0)
        {
            return row;
        }

        row.AvgGrossSeconds = _calculator.CalculateAverage([.. completed.Select(item => item.Gross ?? 0)]);
        row.AvgBreakSeconds = _calculator.CalculateAverage([.. completed.Select(item => item.Break ?? 0)]);
        row.AvgNetSeconds = _calculator.CalculateAverage([.. completed.Select(item => item.Net ?? 0)]);
        row.AvgEffortSeconds = _calculator.CalculateAverage([.. completed.Select(item => item.Effort ?? 0)]);

        var planned = completed.Select(item => plannedMinimum(item.Id)).OfType<int>().ToList();
        if (planned.Count > 0)
        {
            row.AvgPlannedMinSeconds = _calculator.CalculateAverage(planned);
            row.PlanVarianceSeconds = _calculator.CalculatePlanVariance(row.AvgNetSeconds.Value, row.AvgPlannedMinSeconds.Value);
        }

        var evaluated = completed
            .SelectMany(item => phases[item.Id])
            .Where(phase => phase.MinimumSnapshot is not null && phase.NetSeconds is not null)
            .ToList();
        row.EvaluatedPhaseCount = evaluated.Count;
        row.BelowMinimumPhaseCount = evaluated.Count(phase => phase.BelowMinimum);
        return row;
    }

    private sealed record RecordRow(int Id, string RecordNo, int TypeId, string TypeName, CleaningTypeKind Kind, RecordStatus Status, DateTime OpenedAtUtc, int? Gross, int? Break, int? Net, int? Effort);

    private sealed record PhaseRow(int RecordId, int? MinimumSnapshot, int? NetSeconds, bool BelowMinimum);
}

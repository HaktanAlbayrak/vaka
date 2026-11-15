using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;
using Vaka.Web.ViewModels.Definitions;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordListReader : IRecordListReader
{
    private const int MaxPageSize = 100;
    private const string LikeEscape = "\\";

    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ITurkeyClock _clock;
    private readonly IReportOptions _options;

    public RecordListReader(VakaDbContext db, IRecordTimeouts timeouts, ITurkeyClock clock, IReportOptions options)
    {
        _db = db;
        _timeouts = timeouts;
        _clock = clock;
        _options = options;
    }

    public async Task<RecordListViewModel> ListAsync(RecordListQuery query, int viewerUserId, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.Everything, cancellationToken);

        var filtered = Filter(_db.CleaningRecords.AsNoTracking(), query, viewerUserId);
        var total = await filtered.CountAsync(cancellationToken);
        var pageSize = Math.Clamp(query.PageSize, 1, MaxPageSize);
        var pageCount = Math.Max(1, (total + pageSize - 1) / pageSize);
        var page = Math.Clamp(query.Page, 1, pageCount);

        var rows = await filtered
            .OrderByDescending(record => record.OpenedAtUtc).ThenByDescending(record => record.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(record => new
            {
                record.Id,
                record.RecordNo,
                record.Kind,
                record.Status,
                record.FieldBookRef,
                FacilityName = record.FacilityNameSnapshot,
                LineName = record.LineNameSnapshot,
                MachineLabel = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                CleaningTypeName = record.ProcedureVersion.CleaningType.Name,
                record.ProcedureVersion.VersionNo,
                OwnerName = record.Owner.FullName,
                record.OpenedAtUtc,
                record.CompletedAtUtc,
                record.NetSeconds,
                HasBelowMinimum = record.Phases.Any(phase => phase.BelowMinimum),
                HasExpiredMaterial = record.Materials.Any(material => material.ExpiredAtEntry && material.RemovedAtUtc == null)
            })
            .ToListAsync(cancellationToken);

        return new RecordListViewModel
        {
            Items =
            [
                .. rows.Select(row => new RecordListItemViewModel
                {
                    Id = row.Id,
                    RecordNo = row.RecordNo,
                    Kind = row.Kind,
                    KindLabel = ProcedureLabels.Kind(row.Kind),
                    Status = row.Status,
                    StatusLabel = RecordLabels.Status(row.Status),
                    FieldBookRef = row.FieldBookRef,
                    FacilityName = row.FacilityName,
                    LineName = row.LineName,
                    MachineLabel = row.MachineLabel,
                    CleaningTypeName = row.CleaningTypeName,
                    VersionNo = row.VersionNo,
                    OwnerName = row.OwnerName,
                    OpenedAtLocal = _clock.ToLocal(row.OpenedAtUtc),
                    CompletedAtLocal = row.CompletedAtUtc is { } completed ? _clock.ToLocal(completed) : null,
                    NetSeconds = row.NetSeconds,
                    HasBelowMinimum = row.HasBelowMinimum,
                    HasExpiredMaterial = row.HasExpiredMaterial,
                    Url = RecordLinks.Details(row.Id)
                })
            ],
            Filter = new RecordListFilterViewModel
            {
                Query = query.Query?.Trim(),
                FacilityId = query.FacilityId,
                LineId = query.LineId,
                MachineId = query.MachineId,
                Kind = query.Kind,
                Status = query.Status,
                From = query.From,
                To = query.To,
                MineOnly = query.MineOnly
            },
            Facilities = await _options.ListFacilitiesAsync(cancellationToken),
            Lines = await _options.ListLinesAsync(query.FacilityId, cancellationToken),
            Machines = await _options.ListMachinesAsync(query.FacilityId, query.LineId, cancellationToken),
            KindOptions = [.. Enum.GetValues<CleaningTypeKind>().Select(kind => new SelectOptionViewModel { Id = (int)kind, Label = ProcedureLabels.Kind(kind) })],
            StatusOptions = [.. Enum.GetValues<RecordStatus>().Select(status => new SelectOptionViewModel { Id = (int)status, Label = RecordLabels.Status(status) })],
            Page = page,
            PageSize = pageSize,
            Total = total,
            PageCount = pageCount
        };
    }

    private IQueryable<CleaningRecord> Filter(IQueryable<CleaningRecord> records, RecordListQuery query, int viewerUserId)
    {
        if (query.FacilityId is { } facilityId)
        {
            records = records.Where(record => record.Machine.Line.FacilityId == facilityId);
        }

        if (query.LineId is { } lineId)
        {
            records = records.Where(record => record.Machine.ProductionLineId == lineId);
        }

        if (query.MachineId is { } machineId)
        {
            records = records.Where(record => record.MachineId == machineId);
        }

        if (query.Kind is { } kind)
        {
            records = records.Where(record => record.Kind == kind);
        }

        if (query.Status is { } status)
        {
            records = records.Where(record => record.Status == status);
        }

        if (query.From is { } fromDay)
        {
            var start = _clock.StartOfDayUtc(fromDay);
            records = records.Where(record => record.OpenedAtUtc >= start);
        }

        if (query.To is { } toDay)
        {
            var end = _clock.StartOfDayUtc(toDay.AddDays(1));
            records = records.Where(record => record.OpenedAtUtc < end);
        }

        if (query.MineOnly)
        {
            records = records.Where(record => record.OwnerUserId == viewerUserId
                || record.Phases.Any(phase => phase.Steps.Any(step => step.Participants.Any(participant => participant.UserId == viewerUserId))));
        }

        if (!string.IsNullOrWhiteSpace(query.Query))
        {
            var pattern = "%" + query.Query.Trim().Replace("\\", "\\\\").Replace("%", "\\%").Replace("_", "\\_") + "%";
            records = records.Where(record =>
                EF.Functions.Like(record.RecordNo, pattern, LikeEscape)
                || (record.FieldBookRef != null && EF.Functions.Like(record.FieldBookRef, pattern, LikeEscape))
                || (record.WorkOrderNo != null && EF.Functions.Like(record.WorkOrderNo, pattern, LikeEscape))
                || record.Materials.Any(material => material.RemovedAtUtc == null && EF.Functions.Like(material.LotNoSnapshot, pattern, LikeEscape)));
        }

        return records;
    }
}

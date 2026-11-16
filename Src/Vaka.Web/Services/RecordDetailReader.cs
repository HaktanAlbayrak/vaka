using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordDetailReader : IRecordDetailReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly IRecordPhaseReader _phases;
    private readonly IRecordIntegrity _integrity;
    private readonly IProcedureLinks _links;
    private readonly IDurationCalculator _calculator;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;
    private readonly IRecordEventReader _eventReader;

    public RecordDetailReader(
        VakaDbContext db,
        IRecordTimeouts timeouts,
        IRecordPhaseReader phases,
        IRecordIntegrity integrity,
        IProcedureLinks links,
        IDurationCalculator calculator,
        ITurkeyClock clock,
        TimeProvider time,
        IRecordEventReader eventReader)
    {
        _db = db;
        _timeouts = timeouts;
        _phases = phases;
        _integrity = integrity;
        _links = links;
        _calculator = calculator;
        _clock = clock;
        _time = time;
        _eventReader = eventReader;
    }

    public async Task<LookupResult<RecordDetailViewModel>> GetAsync(int recordId, RecordViewer viewer, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);

        var row = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Id == recordId)
            .Select(record => new
            {
                record.RecordNo,
                record.Kind,
                record.Status,
                record.FieldBookRef,
                Facility = record.FacilityCodeSnapshot + " - " + record.FacilityNameSnapshot,
                Line = record.LineCodeSnapshot + " - " + record.LineNameSnapshot,
                Machine = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                CleaningTypeName = record.ProcedureVersion.CleaningType.Name,
                record.ProcedureVersionId,
                VersionStatus = record.ProcedureVersion.Status,
                record.ProcedureVersion.VersionNo,
                record.ProcedureVersion.DocumentRef,
                record.WorkOrderNo,
                record.Note,
                record.EmergencyDescription,
                record.OwnerUserId,
                OwnerName = record.Owner.FullName,
                OwnerRegistryNo = record.Owner.RegistryNo,
                record.OpenedAtUtc,
                record.StartedAtUtc,
                record.CompletedAtUtc,
                record.ClosedAtUtc,
                record.LockExpiresAtUtc,
                record.ClosureKind,
                ClosedByName = record.ClosedBy == null ? null : record.ClosedBy.FullName,
                record.ClosureReason,
                record.GrossSeconds,
                record.BreakSeconds,
                record.HoldSeconds,
                record.NetSeconds,
                record.EffortSeconds
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return LookupResult<RecordDetailViewModel>.Missing();
        }

        var section = await _phases.ReadAsync(recordId, row.OwnerUserId, row.OpenedAtUtc, cancellationToken);
        var breaks = await ReadBreaksAsync(recordId, cancellationToken);
        var integrity = await _integrity.CheckAsync(recordId, cancellationToken);
        var isManager = viewer.Roles.Contains(Roles.Manager);
        var canSeeProcedure = isManager || viewer.Roles.Contains(Roles.Auditor);
        var isActive = row.Status is RecordStatus.Opened or RecordStatus.InProgress or RecordStatus.OnHold;
        var openBreak = breaks.Open;
        var events = await _eventReader.ReadAsync(recordId, cancellationToken);

        var model = new RecordDetailViewModel
        {
            Id = recordId,
            Header = new RecordDetailHeaderViewModel
            {
                RecordNo = row.RecordNo,
                Kind = row.Kind,
                KindLabel = ProcedureLabels.Kind(row.Kind),
                Status = row.Status,
                StatusLabel = RecordLabels.Status(row.Status),
                FieldBookRef = row.FieldBookRef,
                Facility = row.Facility,
                Line = row.Line,
                Machine = row.Machine,
                CleaningTypeName = row.CleaningTypeName,
                VersionNo = row.VersionNo,
                VersionUrl = canSeeProcedure ? _links.Version(row.ProcedureVersionId, row.VersionStatus) : null,
                DocumentRef = row.DocumentRef,
                WorkOrderNo = row.WorkOrderNo ?? string.Empty,
                Note = row.Note ?? string.Empty,
                EmergencyDescription = row.EmergencyDescription
            },
            People = new RecordDetailPeopleViewModel
            {
                OwnerName = row.OwnerName,
                OwnerRegistryNo = row.OwnerRegistryNo,
                FirstStepStartedByName = section.FirstStepStartedByName,
                Helpers = section.Helpers
            },
            Times = new RecordDetailTimesViewModel
            {
                OpenedAtLocal = _clock.ToLocal(row.OpenedAtUtc),
                StartedAtLocal = Local(row.StartedAtUtc),
                CompletedAtLocal = Local(row.CompletedAtUtc),
                ClosedAtLocal = Local(row.ClosedAtUtc),
                LockExpiresAtLocal = row.Status == RecordStatus.Opened ? Local(row.LockExpiresAtUtc) : null
            },
            Summary = new RecordDetailSummaryViewModel
            {
                GrossSeconds = row.GrossSeconds,
                BreakSeconds = row.BreakSeconds,
                HoldSeconds = row.HoldSeconds,
                NetSeconds = row.NetSeconds,
                EffortSeconds = row.EffortSeconds
            },
            Phases = section.Phases,
            Breaks = breaks.Items,
            Materials = await ReadMaterialsAsync(recordId, cancellationToken),
            Events = events.Events,
            Timeline = events.Timeline,
            Integrity = new RecordIntegrityViewModel
            {
                IsValid = integrity?.IsValid ?? false,
                FirstBrokenSequence = integrity?.FirstBrokenSequence,
                CheckedAtLocal = _clock.ToLocal(_time.GetUtcNow().UtcDateTime),
                ReportUrl = canSeeProcedure ? RecordLinks.Integrity(recordId) : null
            },
            Closure = ClosureOf(row.Status, row.ClosureKind, row.ClosedByName, row.ClosedAtUtc, row.ClosureReason),
            OpenBreak = openBreak,
            CanIntervene = isManager && row.Status is RecordStatus.Opened or RecordStatus.InProgress,
            CanEndBreak = isManager && openBreak is not null,
            RunUrl = viewer.Roles.Contains(Roles.Operator) && isActive ? RecordLinks.Run(recordId) : null,
            GeneratedAtLocal = _clock.ToLocal(_time.GetUtcNow().UtcDateTime),
            GeneratedByName = viewer.FullName
        };

        model.InterventionUrl = model.CanIntervene ? RecordLinks.Intervene(recordId) : null;
        model.EndBreakUrl = model.CanEndBreak ? RecordLinks.EndBreak(openBreak!.BreakId) : null;
        return LookupResult<RecordDetailViewModel>.Found(model);
    }

    private DateTime? Local(DateTime? utc) => utc is { } value ? _clock.ToLocal(value) : null;

    private RecordClosureViewModel? ClosureOf(RecordStatus status, int? kind, string? closedByName, DateTime? closedAtUtc, string? reason)
    {
        if (closedAtUtc is not { } closedAt)
        {
            return null;
        }

        if (status == RecordStatus.TimedOut)
        {
            return new RecordClosureViewModel { KindLabel = RecordLabels.Status(status), ClosedByName = "Sistem", ClosedAtLocal = _clock.ToLocal(closedAt), Reason = "Kilit süresi doldu" };
        }

        return kind is { } value && Enum.IsDefined((InterventionType)value)
            ? new RecordClosureViewModel
            {
                KindLabel = RecordLabels.Intervention((InterventionType)value),
                ClosedByName = closedByName ?? string.Empty,
                ClosedAtLocal = _clock.ToLocal(closedAt),
                Reason = reason ?? string.Empty
            }
            : null;
    }

    private async Task<(RecordDetailBreakViewModel[] Items, RecordDetailOpenBreakViewModel? Open)> ReadBreaksAsync(int recordId, CancellationToken cancellationToken)
    {
        var rows = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.CleaningRecordId == recordId)
            .OrderBy(item => item.StartedAtUtc).ThenBy(item => item.Id)
            .Select(item => new
            {
                item.Id,
                StartedByName = item.StartedBy.FullName,
                item.StartedAtUtc,
                item.EndedAtUtc,
                item.EndKind,
                EndedByName = item.EndedBy == null ? null : item.EndedBy.FullName,
                item.EndReason,
                DuringStepTitle = item.DuringStep == null ? null : item.DuringStep.ProcedureStep.Title
            })
            .ToListAsync(cancellationToken);

        var items = rows.Select(row => new RecordDetailBreakViewModel
        {
            StartedByName = row.StartedByName,
            StartedAtLocal = _clock.ToLocal(row.StartedAtUtc),
            EndedAtLocal = Local(row.EndedAtUtc),
            DurationSeconds = row.EndedAtUtc is { } ended ? _calculator.CalculateBreakSeconds(UtcMoments.Of(row.StartedAtUtc), UtcMoments.Of(ended)) : null,
            EndKindLabel = RecordLabels.BreakEnd(row.EndKind),
            EndedByName = row.EndedByName,
            EndReason = row.EndReason ?? string.Empty,
            DuringStepTitle = row.DuringStepTitle
        }).ToArray();

        var open = rows.FirstOrDefault(row => row.EndedAtUtc is null);
        return (items, open is null ? null : new RecordDetailOpenBreakViewModel
        {
            BreakId = open.Id,
            StartedByName = open.StartedByName,
            StartedAtLocal = _clock.ToLocal(open.StartedAtUtc),
            DuringStepTitle = open.DuringStepTitle
        });
    }

    private async Task<RecordMaterialViewModel[]> ReadMaterialsAsync(int recordId, CancellationToken cancellationToken)
    {
        var rows = await _db.RecordMaterials
            .AsNoTracking()
            .Where(material => material.CleaningRecordId == recordId && material.RemovedAtUtc == null)
            .OrderBy(material => material.EnteredAtUtc).ThenBy(material => material.Id)
            .Select(material => new
            {
                material.Id,
                material.MaterialCodeSnapshot,
                material.MaterialNameSnapshot,
                material.LotNoSnapshot,
                material.ExpiryDateSnapshot,
                material.ExpiredAtEntry,
                material.ExpiredReason,
                EnteredByName = material.EnteredBy.FullName,
                material.EnteredAtUtc
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new RecordMaterialViewModel
        {
            Id = row.Id,
            MaterialCode = row.MaterialCodeSnapshot,
            MaterialName = row.MaterialNameSnapshot,
            LotNo = row.LotNoSnapshot,
            ExpiryDate = row.ExpiryDateSnapshot,
            ExpiredAtEntry = row.ExpiredAtEntry,
            ExpiredReason = row.ExpiredReason,
            EnteredByName = row.EnteredByName,
            EnteredAtLocal = _clock.ToLocal(row.EnteredAtUtc)
        })];
    }
}

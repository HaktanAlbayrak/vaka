using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordRunReader : IRecordRunReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ILotCatalog _lots;
    private readonly IRunBreakReader _breaks;
    private readonly IRunProgressReader _progress;
    private readonly IParticipantDirectory _participants;
    private readonly ITurkeyClock _clock;
    private readonly TimeProvider _time;

    public RecordRunReader(
        VakaDbContext db,
        IRecordTimeouts timeouts,
        ILotCatalog lots,
        IRunBreakReader breaks,
        IRunProgressReader progress,
        IParticipantDirectory participants,
        ITurkeyClock clock,
        TimeProvider time)
    {
        _db = db;
        _timeouts = timeouts;
        _lots = lots;
        _breaks = breaks;
        _progress = progress;
        _participants = participants;
        _clock = clock;
        _time = time;
    }

    public async Task<LookupResult<RecordRunViewModel>> GetAsync(
        int recordId,
        int viewerUserId,
        RunViewOptions options,
        CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);

        var row = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.Id == recordId)
            .Select(record => new
            {
                record.Id,
                record.RecordNo,
                record.Kind,
                record.Status,
                record.FieldBookRef,
                record.FacilityCodeSnapshot,
                record.FacilityNameSnapshot,
                record.LineCodeSnapshot,
                record.LineNameSnapshot,
                record.MachineCodeSnapshot,
                record.MachineNameSnapshot,
                CleaningTypeName = record.ProcedureVersion.CleaningType.Name,
                record.ProcedureVersion.VersionNo,
                record.ProcedureVersion.MaterialRequired,
                record.OwnerUserId,
                OwnerName = record.Owner.FullName,
                record.OpenedAtUtc,
                record.LockExpiresAtUtc,
                record.WorkOrderNo,
                record.Note
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return LookupResult<RecordRunViewModel>.Missing();
        }

        var context = new RunContext(recordId, viewerUserId, row.OwnerUserId, row.Status, row.MaterialRequired, _time.GetUtcNow());
        var breakPanel = await _breaks.ReadAsync(context, cancellationToken);
        var progress = await _progress.ReadAsync(context, breakPanel, row.OpenedAtUtc, cancellationToken);
        var phases = progress.Phases;
        var materials = await ReadMaterialsAsync(recordId, cancellationToken);
        var isOwner = row.OwnerUserId == viewerUserId;
        var noStepStarted = phases.All(phase => phase.Steps.All(step => step.Status == StepStatus.Waiting));
        var canManage = isOwner && row.Status == RecordStatus.Opened && noStepStarted;

        var model = new RecordRunViewModel
        {
            Id = row.Id,
            RecordNo = row.RecordNo,
            Kind = row.Kind,
            KindLabel = ProcedureLabels.Kind(row.Kind),
            Status = row.Status,
            StatusLabel = RecordLabels.Status(row.Status),
            FieldBookRef = row.FieldBookRef,
            FacilityLabel = row.FacilityCodeSnapshot + " - " + row.FacilityNameSnapshot,
            LineLabel = row.LineCodeSnapshot + " - " + row.LineNameSnapshot,
            MachineLabel = row.MachineCodeSnapshot + " - " + row.MachineNameSnapshot,
            CleaningTypeName = row.CleaningTypeName,
            VersionNo = row.VersionNo,
            OwnerName = row.OwnerName,
            OpenedAtLocal = _clock.ToLocal(row.OpenedAtUtc),
            LockExpiresAtLocal = row.Status == RecordStatus.Opened && row.LockExpiresAtUtc is { } expires ? _clock.ToLocal(expires) : null,
            WorkOrderNo = row.WorkOrderNo ?? string.Empty,
            Note = row.Note ?? string.Empty,
            MaterialRequired = row.MaterialRequired,
            IsOwner = isOwner,
            CanManageMaterials = canManage,
            CommandId = Guid.NewGuid(),
            Materials = materials,
            Phases = phases,
            LotQuery = options.LotQuery,
            Breaks = breakPanel.Breaks,
            OpenBreak = breakPanel.OpenBreak,
            TimersRunning = breakPanel.OpenBreak is null,
            CanStartBreak = progress.CanStartBreak,
            CannotBreakReason = progress.CannotBreakReason,
            Summary = progress.Summary,
            FirstStepStartedByName = progress.FirstStepStartedByName,
            PendingParticipant = options.PendingParticipant
        };

        if (canManage)
        {
            var catalog = await _lots.FindAsync(options.LotQuery, options.PendingLotId, recordId, cancellationToken);
            model.AvailableLots = catalog.Lots;
            model.AvailableLotsTruncated = catalog.Truncated;
        }

        if (options.ParticipantStepId is { } stepId && phases.SelectMany(phase => phase.Steps).FirstOrDefault(step => step.Id == stepId) is { CanManageParticipants: true })
        {
            var found = await _participants.SearchAsync(options.ParticipantQuery, stepId, options.PendingParticipant?.ParticipantUserId, cancellationToken);
            model.ParticipantStepId = stepId;
            model.ParticipantQuery = options.ParticipantQuery;
            model.ParticipantOptions = found.Options;
            model.ParticipantOptionsTruncated = found.Truncated;
        }

        return LookupResult<RecordRunViewModel>.Found(model);
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
                Code = material.MaterialCodeSnapshot,
                Name = material.MaterialNameSnapshot,
                LotNo = material.LotNoSnapshot,
                Expiry = material.ExpiryDateSnapshot,
                material.ExpiredAtEntry,
                material.ExpiredReason,
                EnteredBy = material.EnteredBy.FullName,
                material.EnteredAtUtc
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new RecordMaterialViewModel
        {
            Id = row.Id,
            MaterialCode = row.Code,
            MaterialName = row.Name,
            LotNo = row.LotNo,
            ExpiryDate = row.Expiry,
            ExpiredAtEntry = row.ExpiredAtEntry,
            ExpiredReason = row.ExpiredReason,
            EnteredByName = row.EnteredBy,
            EnteredAtLocal = _clock.ToLocal(row.EnteredAtUtc)
        })];
    }
}

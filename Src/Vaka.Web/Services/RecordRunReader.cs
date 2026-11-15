using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class RecordRunReader : IRecordRunReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ILotCatalog _lots;
    private readonly ITurkeyClock _clock;

    public RecordRunReader(VakaDbContext db, IRecordTimeouts timeouts, ILotCatalog lots, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _lots = lots;
        _clock = clock;
    }

    public async Task<LookupResult<RecordRunViewModel>> GetAsync(
        int recordId,
        int viewerUserId,
        string? lotQuery,
        int? pendingLotId,
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

        var phases = await ReadPhasesAsync(recordId, cancellationToken);
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
            LotQuery = lotQuery
        };

        if (canManage)
        {
            var catalog = await _lots.FindAsync(lotQuery, pendingLotId, cancellationToken);
            model.AvailableLots = catalog.Lots;
            model.AvailableLotsTruncated = catalog.Truncated;
        }

        return LookupResult<RecordRunViewModel>.Found(model);
    }

    private async Task<RecordPhaseViewModel[]> ReadPhasesAsync(int recordId, CancellationToken cancellationToken)
    {
        var phases = await _db.RecordPhases
            .AsNoTracking()
            .Where(phase => phase.CleaningRecordId == recordId)
            .OrderBy(phase => phase.ProcedurePhase.OrderNo)
            .Select(phase => new RecordPhaseViewModel
            {
                Id = phase.Id,
                OrderNo = phase.ProcedurePhase.OrderNo,
                Title = phase.ProcedurePhase.Title,
                MinDurationSeconds = phase.MinDurationSecondsSnapshot,
                Status = phase.Status,
                Steps = phase.Steps
                    .OrderBy(step => step.ProcedureStep.OrderNo)
                    .Select(step => new RecordStepViewModel
                    {
                        Id = step.Id,
                        OrderNo = step.ProcedureStep.OrderNo,
                        Title = step.ProcedureStep.Title,
                        Status = step.Status
                    })
                    .ToArray()
            })
            .ToArrayAsync(cancellationToken);

        var previousCompleted = true;
        foreach (var phase in phases)
        {
            phase.StatusLabel = RecordLabels.Phase(phase.Status);
            foreach (var step in phase.Steps)
            {
                step.StatusLabel = RecordLabels.Step(step.Status);
                step.IsNext = previousCompleted && step.Status == StepStatus.Waiting;
                previousCompleted = step.Status == StepStatus.Completed;
            }
        }

        return phases;
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

using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

public sealed class MyWorkReader : IMyWorkReader
{
    private readonly VakaDbContext _db;
    private readonly IRecordTimeouts _timeouts;
    private readonly ITurkeyClock _clock;

    public MyWorkReader(VakaDbContext db, IRecordTimeouts timeouts, ITurkeyClock clock)
    {
        _db = db;
        _timeouts = timeouts;
        _clock = clock;
    }

    public async Task<MyWorkViewModel> GetAsync(int userId, CancellationToken cancellationToken)
    {
        await _timeouts.ApplyAsync(TimeoutScope.ForOwner(userId), cancellationToken);

        var row = await _db.CleaningRecords
            .AsNoTracking()
            .Where(record => record.OwnerUserId == userId && (record.Status == RecordStatus.Opened || record.Status == RecordStatus.InProgress))
            .Select(record => new
            {
                record.Id,
                record.RecordNo,
                record.Kind,
                record.Status,
                Machine = record.MachineCodeSnapshot + " - " + record.MachineNameSnapshot,
                TypeName = record.ProcedureVersion.CleaningType.Name,
                record.ProcedureVersion.VersionNo,
                record.LockExpiresAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);

        var assigned = await ReadAssignedStepsAsync(userId, cancellationToken);
        var openBreak = await _db.RecordBreaks
            .AsNoTracking()
            .Where(item => item.StartedByUserId == userId && item.EndedAtUtc == null)
            .Select(item => new { item.Id, item.Record.RecordNo, item.StartedAtUtc })
            .FirstOrDefaultAsync(cancellationToken);
        var breakModel = openBreak is null
            ? null
            : new MyWorkBreakViewModel { BreakId = openBreak.Id, RecordNo = openBreak.RecordNo, StartedAtLocal = _clock.ToLocal(openBreak.StartedAtUtc) };

        if (row is null)
        {
            return new MyWorkViewModel { CanOpenRecord = true, CanOpenEmergency = true, AssignedSteps = assigned, OpenBreak = breakModel };
        }

        var url = RecordLinks.Run(row.Id);
        return new MyWorkViewModel
        {
            ActiveRecord = new ActiveRecordViewModel
            {
                Id = row.Id,
                RecordNo = row.RecordNo,
                Kind = row.Kind,
                KindLabel = ProcedureLabels.Kind(row.Kind),
                Status = row.Status,
                StatusLabel = RecordLabels.Status(row.Status),
                MachineLabel = row.Machine,
                CleaningTypeName = row.TypeName,
                VersionNo = row.VersionNo,
                LockExpiresAtLocal = row.Status == RecordStatus.Opened && row.LockExpiresAtUtc is { } expires ? _clock.ToLocal(expires) : null,
                Url = url
            },
            CanOpenRecord = false,
            CannotOpenReason = $"Açık kaydınız var: {row.RecordNo} ({RecordLabels.Status(row.Status).ToLowerInvariant()}). Yeni kayıt açmadan önce onu sürdürün.",
            CannotOpenUrl = url,
            AssignedSteps = assigned,
            OpenBreak = breakModel
        };
    }

    private async Task<AssignedStepViewModel[]> ReadAssignedStepsAsync(int userId, CancellationToken cancellationToken)
    {
        var rows = await _db.StepParticipants
            .AsNoTracking()
            .Where(item => item.UserId == userId
                && item.Step.Phase.Record.OwnerUserId != userId
                && (item.Step.Status == StepStatus.Waiting || item.Step.Status == StepStatus.InProgress)
                && (item.Step.Phase.Record.Status == RecordStatus.Opened || item.Step.Phase.Record.Status == RecordStatus.InProgress))
            .OrderBy(item => item.Step.Phase.Record.OpenedAtUtc).ThenBy(item => item.Step.Phase.ProcedurePhase.OrderNo).ThenBy(item => item.Step.ProcedureStep.OrderNo)
            .Select(item => new
            {
                RecordId = item.Step.Phase.CleaningRecordId,
                item.Step.Phase.Record.RecordNo,
                Machine = item.Step.Phase.Record.MachineCodeSnapshot + " - " + item.Step.Phase.Record.MachineNameSnapshot,
                StepTitle = item.Step.ProcedureStep.Title,
                OwnerName = item.Step.Phase.Record.Owner.FullName
            })
            .ToListAsync(cancellationToken);

        return [.. rows.Select(row => new AssignedStepViewModel
        {
            RecordId = row.RecordId,
            RecordNo = row.RecordNo,
            MachineLabel = row.Machine,
            StepTitle = row.StepTitle,
            OwnerName = row.OwnerName,
            Url = RecordLinks.Run(row.RecordId)
        })];
    }
}

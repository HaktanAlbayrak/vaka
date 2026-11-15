using Microsoft.EntityFrameworkCore;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class StepStarter : IStepStarter
{
    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IBreakReader _breaks;
    private readonly IPinService _pins;
    private readonly IStepStartApplier _applier;
    private readonly IProcessedCommands _commands;
    private readonly IDurationCalculator _calculator;
    private readonly TimeProvider _time;

    public StepStarter(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IBreakReader breaks,
        IPinService pins,
        IStepStartApplier applier,
        IProcessedCommands commands,
        IDurationCalculator calculator,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _breaks = breaks;
        _pins = pins;
        _applier = applier;
        _commands = commands;
        _calculator = calculator;
        _time = time;
    }

    public async Task<CommandResult> StartAsync(int stepId, int actorUserId, StartStepRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin.");
        }

        if (await _db.GetRecordIdAsync(stepId, cancellationToken) is { } recordId)
        {
            await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);
        }

        return await _transactions.RunAsync(async token =>
        {
            if (await _commands.ReplayAsync(request.CommandId, actorUserId, token) is { } replay)
            {
                return replay;
            }

            if (await _breaks.CheckAsync(actorUserId, token) is { } onBreak)
            {
                return onBreak;
            }

            var record = await _db.LoadRecordForStepAsync(stepId, token);
            if (record is null)
            {
                return CommandResult.NotFound();
            }

            var ordered = record.OrderedSteps();
            var step = ordered.First(item => item.Id == stepId);
            var isFirst = ordered[0].Id == stepId;
            var failure = await CheckOrderAsync(record, ordered, step, actorUserId, token)
                ?? await CheckMaterialsAsync(record, isFirst, token)
                ?? await _applier.CheckParticipantsAsync(step, token);
            if (failure is not null)
            {
                return failure;
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            return await ApplyAsync(record, step, isFirst, actorUserId, request.CommandId, token);
        }, cancellationToken);
    }

    private async Task<CommandResult?> CheckOrderAsync(CleaningRecord record, List<RecordStep> ordered, RecordStep step, int actorUserId, CancellationToken cancellationToken)
    {
        if (record.OwnerUserId != actorUserId && step.Participants.All(item => item.UserId != actorUserId))
        {
            return CommandResult.Forbidden();
        }

        if (record.Status == RecordStatus.TimedOut)
        {
            return Reject("Kilit süresi doldu, kayıt Zaman aşımına düştü. Adım başlatılamaz.");
        }

        if (record.Status is not (RecordStatus.Opened or RecordStatus.InProgress))
        {
            return Reject("Kayıt bu durumda adım başlatılamaz.");
        }

        if (record.Status == RecordStatus.Opened && record.LockExpiresAtUtc is { } expires && _calculator.IsLockExpired(UtcMoments.Of(expires), _time.GetUtcNow()))
        {
            return Reject("Kilit süresi doldu. Adım başlatılamaz.");
        }

        if (await _db.RecordBreaks.AnyAsync(item => item.CleaningRecordId == record.Id && item.EndedAtUtc == null, cancellationToken))
        {
            return Reject("Kayıtta açık mola var.");
        }

        if (step.Status != StepStatus.Waiting)
        {
            return Reject("Adım zaten başlamış ya da tamamlanmış.");
        }

        if (ordered.Any(item => item.Status == StepStatus.InProgress))
        {
            return Reject("Kayıtta devam eden bir adım var. Önce onu bitirin.");
        }

        return ordered.TakeWhile(item => item.Id != step.Id).Any(item => item.Status != StepStatus.Completed)
            ? Reject("Önceki adım tamamlanmadan bu adım başlatılamaz.")
            : null;
    }

    private async Task<CommandResult?> CheckMaterialsAsync(CleaningRecord record, bool isFirst, CancellationToken cancellationToken)
    {
        if (!isFirst)
        {
            return null;
        }

        var required = await _db.ProcedureVersions.AsNoTracking()
            .Where(version => version.Id == record.ProcedureVersionId)
            .Select(version => version.MaterialRequired)
            .FirstAsync(cancellationToken);
        if (!required || await _db.RecordMaterials.AnyAsync(item => item.CleaningRecordId == record.Id && item.RemovedAtUtc == null, cancellationToken))
        {
            return null;
        }

        return Reject("Bu temizlik tipi malzeme ister. İlk adımdan önce kayda en az bir malzeme ekleyin.");
    }

    private async Task<CommandResult> ApplyAsync(CleaningRecord record, RecordStep step, bool isFirst, int actorUserId, Guid commandId, CancellationToken cancellationToken)
    {
        var now = _time.GetUtcNow().UtcDateTime;
        if (await _applier.ApplyAsync(record, step, isFirst, actorUserId, commandId, now, cancellationToken) is { } conflict)
        {
            return conflict;
        }

        _commands.Remember(commandId, nameof(StartAsync), actorUserId, record.Id, now);
        await _db.SaveChangesAsync(cancellationToken);
        return CommandResult.Success(record.Id);
    }

    private static CommandResult Reject(string message) => CommandResult.Rejected(string.Empty, message);
}

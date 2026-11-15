using Microsoft.EntityFrameworkCore;
using Vaka.Web.Authorization;
using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class ParticipantService : IParticipantService
{
    private readonly VakaDbContext _db;
    private readonly IWriteTransactionRunner _transactions;
    private readonly IRecordTimeouts _timeouts;
    private readonly IBreakReader _breaks;
    private readonly IPinService _pins;
    private readonly ICleaningEventWriter _events;
    private readonly IProcessedCommands _commands;
    private readonly TimeProvider _time;

    public ParticipantService(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IBreakReader breaks,
        IPinService pins,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _breaks = breaks;
        _pins = pins;
        _events = events;
        _commands = commands;
        _time = time;
    }

    public async Task<CommandResult> AddAsync(int stepId, int actorUserId, AddParticipantRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
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

            var context = await CheckEditableAsync(stepId, actorUserId, token);
            if (context.Failure is { } failure)
            {
                return failure;
            }

            if (await CheckCandidateAsync(stepId, request.ParticipantUserId, token) is { } candidateFailure)
            {
                return candidateFailure;
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            _db.StepParticipants.Add(new StepParticipant { RecordStepId = stepId, UserId = request.ParticipantUserId, AddedByUserId = actorUserId });
            await _db.SaveChangesAsync(token);
            await _events.AppendAsync(
                new EventDraft(context.RecordId, EventTypes.ParticipantAdded, new { StepId = stepId, ParticipantUserId = request.ParticipantUserId, AddedByUserId = actorUserId }, actorUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(AddAsync), actorUserId, context.RecordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(context.RecordId);
        }, cancellationToken);
    }

    public async Task<CommandResult> RemoveAsync(int stepId, int actorUserId, RemoveParticipantRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
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

            var context = await CheckEditableAsync(stepId, actorUserId, token);
            if (context.Failure is { } failure)
            {
                return failure;
            }

            var participants = await _db.StepParticipants.Where(item => item.RecordStepId == stepId).ToListAsync(token);
            var target = participants.FirstOrDefault(item => item.UserId == request.ParticipantUserId);
            if (target is null)
            {
                return CommandResult.Rejected("ParticipantUserId", "Kişi bu adımın görevlisi değil.");
            }

            if (participants.Count == 1)
            {
                return CommandResult.Rejected("ParticipantUserId", "Adımda en az bir görevli kalmalı.");
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            _db.StepParticipants.Remove(target);
            await _db.SaveChangesAsync(token);
            await _events.AppendAsync(
                new EventDraft(context.RecordId, EventTypes.ParticipantRemoved, new { StepId = stepId, ParticipantUserId = request.ParticipantUserId, RemovedByUserId = actorUserId }, actorUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(RemoveAsync), actorUserId, context.RecordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(context.RecordId);
        }, cancellationToken);
    }

    private async Task<StepContext> CheckEditableAsync(int stepId, int actorUserId, CancellationToken cancellationToken)
    {
        var step = await _db.RecordSteps
            .AsNoTracking()
            .Where(item => item.Id == stepId)
            .Select(item => new
            {
                item.Status,
                RecordId = item.Phase.CleaningRecordId,
                RecordStatus = item.Phase.Record.Status,
                item.Phase.Record.OwnerUserId
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (step is null)
        {
            return new StepContext(0, CommandResult.NotFound());
        }

        if (step.OwnerUserId != actorUserId)
        {
            return new StepContext(step.RecordId, CommandResult.Forbidden());
        }

        if (step.RecordStatus is not (RecordStatus.Opened or RecordStatus.InProgress))
        {
            var reason = step.RecordStatus == RecordStatus.TimedOut
                ? "Kilit süresi doldu, kayıt Zaman aşımına düştü. Görevli değiştirilemez."
                : "Kayıt bu durumda görevli değiştirilemez.";
            return new StepContext(step.RecordId, CommandResult.Rejected(string.Empty, reason));
        }

        if (await _db.RecordBreaks.AnyAsync(item => item.CleaningRecordId == step.RecordId && item.EndedAtUtc == null, cancellationToken))
        {
            return new StepContext(step.RecordId, CommandResult.Rejected(string.Empty, "Kayıtta açık mola var."));
        }

        return step.Status == StepStatus.Waiting
            ? new StepContext(step.RecordId, null)
            : new StepContext(step.RecordId, CommandResult.Rejected(string.Empty, "Adım başladıktan sonra görevli eklenmez ya da çıkarılmaz."));
    }

    private async Task<CommandResult?> CheckCandidateAsync(int stepId, int candidateId, CancellationToken cancellationToken)
    {
        var candidate = await _db.Users
            .AsNoTracking()
            .Where(user => user.Id == candidateId && user.Roles.Any(role => role.Role == Roles.Operator))
            .Select(user => new { user.FullName, user.IsActive })
            .FirstOrDefaultAsync(cancellationToken);

        if (candidate is null)
        {
            return CommandResult.Rejected("ParticipantUserId", "Kişi seçilemez.");
        }

        if (!candidate.IsActive)
        {
            return CommandResult.Rejected("ParticipantUserId", $"{candidate.FullName} pasif, görevli olarak eklenemez.");
        }

        if (await _db.StepParticipants.AnyAsync(item => item.RecordStepId == stepId && item.UserId == candidateId, cancellationToken))
        {
            return CommandResult.Rejected("ParticipantUserId", $"{candidate.FullName} bu adımda zaten görevli.");
        }

        return await _breaks.IsOnBreakAsync(candidateId, cancellationToken)
            ? CommandResult.Rejected("ParticipantUserId", $"{candidate.FullName} molada, görevli olarak eklenemez.")
            : null;
    }

    private static CommandResult MissingCommandId() =>
        CommandResult.Rejected(string.Empty, "İstek kimliği eksik. Sayfayı yenileyip yeniden deneyin.");

    private sealed record StepContext(int RecordId, CommandResult? Failure);
}

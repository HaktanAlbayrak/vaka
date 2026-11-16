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
    private readonly IDurationCalculator _calculator;
    private readonly TimeProvider _time;

    public ParticipantService(
        VakaDbContext db,
        IWriteTransactionRunner transactions,
        IRecordTimeouts timeouts,
        IBreakReader breaks,
        IPinService pins,
        ICleaningEventWriter events,
        IProcessedCommands commands,
        IDurationCalculator calculator,
        TimeProvider time)
    {
        _db = db;
        _transactions = transactions;
        _timeouts = timeouts;
        _breaks = breaks;
        _pins = pins;
        _events = events;
        _commands = commands;
        _calculator = calculator;
        _time = time;
    }

    public async Task<CommandResult> AddAsync(int stepId, int actorUserId, AddParticipantRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
        }

        await ApplyTimeoutAsync(stepId, cancellationToken);
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

            var joinsNow = context.Status == StepStatus.InProgress && request.ParticipantUserId == context.OwnerUserId;
            if (joinsNow && await IsWorkingElsewhereAsync(request.ParticipantUserId, stepId, token))
            {
                return CommandResult.Rejected("ParticipantUserId", "Başka kayıtta devam eden adımda çalışıyorsunuz.");
            }

            if (await _pins.ConfirmAsync(actorUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            var participant = new StepParticipant { RecordStepId = stepId, UserId = request.ParticipantUserId, AddedByUserId = actorUserId };
            if (joinsNow)
            {
                participant.IsWorking = true;
                participant.WorkInterval = new StepWorkInterval { StartedAtUtc = now };
            }

            _db.StepParticipants.Add(participant);
            if (await SaveJoinAsync(token) is { } conflict)
            {
                return conflict;
            }

            await _events.AppendAsync(
                new EventDraft(context.RecordId, EventTypes.ParticipantAdded, new { StepId = stepId, ParticipantUserId = request.ParticipantUserId, AddedByUserId = actorUserId }, actorUserId, request.CommandId, now),
                token);
            if (joinsNow)
            {
                await _events.AppendAsync(
                    new EventDraft(context.RecordId, EventTypes.ParticipantJoined, new { StepId = stepId, ParticipantUserId = actorUserId, SessionUserId = actorUserId, BeforeStart = false }, actorUserId, request.CommandId, now),
                    token);
            }

            _commands.Remember(request.CommandId, nameof(AddAsync), actorUserId, context.RecordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(context.RecordId);
        }, cancellationToken);
    }

    public async Task<CommandResult> JoinAsync(int stepId, int actorUserId, JoinStepRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
        }

        await ApplyTimeoutAsync(stepId, cancellationToken);
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

            var context = await CheckSelfOrOwnerAsync(stepId, actorUserId, request.ParticipantUserId, token);
            if (context.Failure is { } failure)
            {
                return failure;
            }

            var participant = await _db.StepParticipants
                .Include(item => item.WorkInterval)
                .Include(item => item.User)
                .FirstOrDefaultAsync(item => item.RecordStepId == stepId && item.UserId == request.ParticipantUserId, token);
            if (await CheckJoinAsync(participant, context, stepId, token) is { } joinFailure)
            {
                return joinFailure;
            }

            if (await _pins.ConfirmAsync(request.ParticipantUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            var beforeStart = context.Status == StepStatus.Waiting;
            participant!.ConfirmedAtUtc ??= now;
            if (!beforeStart)
            {
                participant.IsWorking = true;
                participant.WorkInterval = new StepWorkInterval { StartedAtUtc = now };
            }

            if (await SaveJoinAsync(token) is { } conflict)
            {
                return conflict;
            }

            await _events.AppendAsync(
                new EventDraft(context.RecordId, EventTypes.ParticipantJoined, new { StepId = stepId, ParticipantUserId = request.ParticipantUserId, SessionUserId = actorUserId, BeforeStart = beforeStart }, request.ParticipantUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(JoinAsync), actorUserId, context.RecordId, now);
            await _db.SaveChangesAsync(token);
            return CommandResult.Success(context.RecordId);
        }, cancellationToken);
    }

    /// <summary>
    /// Ayrılan kişinin çalışma süresi SH-02a ile hesaplanır (aralık kapanışında yazılır):
    /// Kişi başlangıcı = en geç (katılma anı, adım başlangıcı); kişi bitişi = ayrılma anı
    /// Çalışma = (Kişi bitişi - Kişi başlangıcı) - Σ kesişen kapanmış kayıt molası; saniyeye aşağı yuvarlanır, 0'ın altına inmez
    /// </summary>
    public async Task<CommandResult> LeaveAsync(int stepId, int actorUserId, LeaveStepRequest request, CancellationToken cancellationToken)
    {
        if (request.CommandId == Guid.Empty)
        {
            return MissingCommandId();
        }

        await ApplyTimeoutAsync(stepId, cancellationToken);
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

            var context = await CheckSelfOrOwnerAsync(stepId, actorUserId, request.ParticipantUserId, token);
            if (context.Failure is { } failure)
            {
                return failure;
            }

            if (context.Status != StepStatus.InProgress)
            {
                return CommandResult.Rejected("ParticipantUserId", "Adım devam etmiyor, ayrılınamaz.");
            }

            var participants = await _db.StepParticipants.Include(item => item.WorkInterval).Where(item => item.RecordStepId == stepId).ToListAsync(token);
            var person = participants.FirstOrDefault(item => item.UserId == request.ParticipantUserId);
            if (person is null)
            {
                return CommandResult.Rejected("ParticipantUserId", "Kişi bu adımın görevlisi değil.");
            }

            if (person.WorkInterval is { EndedAtUtc: not null })
            {
                return CommandResult.Rejected("ParticipantUserId", "Kişi adımdan zaten ayrıldı.");
            }

            if (person.WorkInterval is null && !person.IsWorking)
            {
                return CommandResult.Rejected("ParticipantUserId", "Kişi adıma katılmadı, ayrılamaz.");
            }

            if (participants.Count(item => item.IsWorking) <= 1)
            {
                return CommandResult.Rejected("ParticipantUserId", "Adımda çalışan son kişi ayrılamaz. Adımı bitirin.");
            }

            if (await _pins.ConfirmAsync(request.ParticipantUserId, request.Pin, token) is { } pinFailure)
            {
                return pinFailure;
            }

            var now = _time.GetUtcNow().UtcDateTime;
            var stepStart = await _db.RecordSteps.AsNoTracking().Where(item => item.Id == stepId).Select(item => item.StartedAtUtc!.Value).FirstAsync(token);
            var interval = person.WorkInterval ?? new StepWorkInterval { Participant = person, StartedAtUtc = stepStart };
            if (person.WorkInterval is null)
            {
                _db.StepWorkIntervals.Add(interval);
            }

            var spans = await _db.GetBreakSpansAsync(context.RecordId, token);
            interval.EndedAtUtc = now;
            interval.EndKind = StepIntervalEndKind.Left;
            interval.EndedByUserId = request.ParticipantUserId;
            interval.WorkSeconds = _calculator.CalculateParticipantWork(UtcMoments.Of(stepStart), UtcMoments.Of(now), UtcMoments.Of(interval.StartedAtUtc), UtcMoments.Of(now), spans);
            person.IsWorking = false;
            await _db.SaveChangesAsync(token);
            await _events.AppendAsync(
                new EventDraft(context.RecordId, EventTypes.ParticipantLeft, new { StepId = stepId, ParticipantUserId = request.ParticipantUserId, SessionUserId = actorUserId, interval.WorkSeconds }, request.ParticipantUserId, request.CommandId, now),
                token);
            _commands.Remember(request.CommandId, nameof(LeaveAsync), actorUserId, context.RecordId, now);
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

        await ApplyTimeoutAsync(stepId, cancellationToken);
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

            var participants = await _db.StepParticipants.Include(item => item.WorkInterval).Include(item => item.User).Where(item => item.RecordStepId == stepId).ToListAsync(token);
            var target = participants.FirstOrDefault(item => item.UserId == request.ParticipantUserId);
            if (target is null)
            {
                return CommandResult.Rejected("ParticipantUserId", "Kişi bu adımın görevlisi değil.");
            }

            if (target.WorkInterval is not null || target.IsWorking)
            {
                return CommandResult.Rejected("ParticipantUserId", $"{target.User.FullName} adıma katıldı, listeden çıkarılamaz.");
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

    private async Task ApplyTimeoutAsync(int stepId, CancellationToken cancellationToken)
    {
        if (await _db.GetRecordIdAsync(stepId, cancellationToken) is { } recordId)
        {
            await _timeouts.ApplyAsync(TimeoutScope.ForRecord(recordId), cancellationToken);
        }
    }

    private async Task<CommandResult?> SaveJoinAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _db.SaveChangesAsync(cancellationToken);
            return null;
        }
        catch (DbUpdateException exception) when (ConstraintViolations.IsUniqueViolation(exception, "StepParticipants.UserId"))
        {
            _db.ChangeTracker.Clear();
            return CommandResult.Rejected("ParticipantUserId", "Kişi başka bir adımda çalışıyor. Birkaç saniye sonra yeniden deneyin.");
        }
    }

    private Task<bool> IsWorkingElsewhereAsync(int userId, int stepId, CancellationToken cancellationToken) =>
        _db.StepParticipants.AsNoTracking().AnyAsync(item => item.UserId == userId && item.IsWorking && item.RecordStepId != stepId, cancellationToken);

    private async Task<StepContext> LoadStepAsync(int stepId, CancellationToken cancellationToken)
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
            return new StepContext(0, StepStatus.Waiting, 0, CommandResult.NotFound());
        }

        var context = new StepContext(step.RecordId, step.Status, step.OwnerUserId, null);
        if (step.RecordStatus is not (RecordStatus.Opened or RecordStatus.InProgress))
        {
            var reason = step.RecordStatus == RecordStatus.TimedOut
                ? "Kilit süresi doldu, kayıt Zaman aşımına düştü. Görevli işlemi yapılamaz."
                : "Kayıt bu durumda görevli işlemi yapılamaz.";
            return context with { Failure = CommandResult.Rejected(string.Empty, reason) };
        }

        if (await _db.RecordBreaks.AnyAsync(item => item.CleaningRecordId == step.RecordId && item.EndedAtUtc == null, cancellationToken))
        {
            return context with { Failure = CommandResult.Rejected(string.Empty, "Kayıtta açık mola var.") };
        }

        return step.Status is StepStatus.Waiting or StepStatus.InProgress
            ? context
            : context with { Failure = CommandResult.Rejected(string.Empty, "Adım bitti, görevli işlemi yapılamaz.") };
    }

    private async Task<StepContext> CheckEditableAsync(int stepId, int actorUserId, CancellationToken cancellationToken)
    {
        var context = await LoadStepAsync(stepId, cancellationToken);
        return context.Failure is { Status: CommandStatus.NotFound } || context.OwnerUserId == actorUserId
            ? context
            : context with { Failure = CommandResult.Forbidden() };
    }

    private async Task<StepContext> CheckSelfOrOwnerAsync(int stepId, int actorUserId, int participantUserId, CancellationToken cancellationToken)
    {
        var context = await LoadStepAsync(stepId, cancellationToken);
        return context.Failure is { Status: CommandStatus.NotFound } || context.OwnerUserId == actorUserId || actorUserId == participantUserId
            ? context
            : context with { Failure = CommandResult.Forbidden() };
    }

    private async Task<CommandResult?> CheckJoinAsync(StepParticipant? participant, StepContext context, int stepId, CancellationToken cancellationToken)
    {
        if (participant is null)
        {
            return CommandResult.Rejected("ParticipantUserId", "Kişi bu adımın görevlisi değil.");
        }

        var name = participant.User.FullName;
        if (participant.UserId == context.OwnerUserId && context.Status == StepStatus.Waiting)
        {
            return CommandResult.Rejected("ParticipantUserId", "Kayıt sahibi adım başlayınca çalışmaya başlar, ayrıca katılmaz.");
        }

        if (participant.WorkInterval is { EndedAtUtc: not null, EndKind: StepIntervalEndKind.Left })
        {
            return CommandResult.Rejected("ParticipantUserId", $"{name} adımdan ayrıldı, aynı adıma yeniden katılamaz.");
        }

        if (participant.WorkInterval is not null || participant.IsWorking)
        {
            return CommandResult.Rejected("ParticipantUserId", $"{name} bu adımda zaten çalışıyor.");
        }

        if (context.Status == StepStatus.Waiting && participant.ConfirmedAtUtc is not null)
        {
            return CommandResult.Rejected("ParticipantUserId", $"{name} katılacağını zaten onayladı.");
        }

        var reason = ParticipantStateRules.JoinBlock(
            name,
            participant.User.IsActive,
            participant.User.PinLockedAtUtc is not null,
            await _breaks.IsOnBreakAsync(participant.UserId, cancellationToken),
            await IsWorkingElsewhereAsync(participant.UserId, stepId, cancellationToken));
        return reason is null ? null : CommandResult.Rejected("ParticipantUserId", reason);
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

    private sealed record StepContext(int RecordId, StepStatus Status, int OwnerUserId, CommandResult? Failure);
}

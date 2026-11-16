using Vaka.Web.Data;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Services;

public sealed class StepWorkCloser : IStepWorkCloser
{
    private readonly VakaDbContext _db;
    private readonly IDurationCalculator _calculator;

    public StepWorkCloser(VakaDbContext db, IDurationCalculator calculator)
    {
        _db = db;
        _calculator = calculator;
    }

    public StepWorkResult Close(RecordStep step, StepIntervalEndKind kind, int actorUserId, DateTime now, IReadOnlyList<BreakSpan> breaks)
    {
        var stepStart = UtcMoments.Of(step.StartedAtUtc!.Value);
        var stepEnd = UtcMoments.Of(now);
        var users = new List<int>();
        var seconds = new List<int>();

        foreach (var participant in step.Participants.OrderBy(item => item.Id))
        {
            var interval = participant.WorkInterval;
            if (interval is null && participant.IsWorking)
            {
                interval = new StepWorkInterval { Participant = participant, StartedAtUtc = step.StartedAtUtc!.Value };
                _db.StepWorkIntervals.Add(interval);
            }

            participant.IsWorking = false;
            if (interval is null)
            {
                continue;
            }

            if (interval.EndedAtUtc is null)
            {
                interval.EndedAtUtc = now;
                interval.EndKind = kind;
                interval.EndedByUserId = actorUserId;
                interval.WorkSeconds = _calculator.CalculateParticipantWork(stepStart, stepEnd, UtcMoments.Of(interval.StartedAtUtc), null, breaks);
            }

            users.Add(participant.UserId);
            seconds.Add(interval.WorkSeconds ?? 0);
        }

        return new StepWorkResult([.. users], [.. seconds]);
    }
}

namespace Vaka.Web.Services;

public sealed class TurkeyClock : ITurkeyClock
{
    private readonly TimeProvider _time;

    public TurkeyClock(TimeProvider time)
    {
        _time = time;
        Zone = FindZone();
    }

    public TimeZoneInfo Zone { get; }

    public DateTime ToLocal(DateTime utc) =>
        TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc), Zone);

    public DateOnly Today() => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(_time.GetUtcNow(), Zone).DateTime);

    private static TimeZoneInfo FindZone()
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Europe/Istanbul");
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("Turkey Standard Time");
        }
    }
}

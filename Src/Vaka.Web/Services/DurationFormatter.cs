namespace Vaka.Web.Services;

public sealed class DurationFormatter : IDurationFormatter
{
    private const int SecondsPerHour = 3600;
    private const int SecondsPerMinute = 60;

    public DurationPart[] Format(int seconds)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(seconds);

        var hours = seconds / SecondsPerHour;
        var minutes = seconds % SecondsPerHour / SecondsPerMinute;
        var rest = seconds % SecondsPerMinute;

        var parts = new List<DurationPart>(3);
        if (hours > 0)
        {
            parts.Add(new DurationPart(hours, "sa"));
        }

        if (minutes > 0)
        {
            parts.Add(new DurationPart(minutes, "dk"));
        }

        if (rest > 0 || parts.Count == 0)
        {
            parts.Add(new DurationPart(rest, "sn"));
        }

        return [.. parts];
    }
}

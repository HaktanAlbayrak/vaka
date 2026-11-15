namespace Vaka.Web.Services;

public static class UtcMoments
{
    /// <summary>Veritabanından Unspecified gelen UTC zamanı <see cref="DateTimeOffset"/> yapar.</summary>
    public static DateTimeOffset Of(DateTime utc) => new(DateTime.SpecifyKind(utc, DateTimeKind.Utc));

    public static DateTimeOffset? Of(DateTime? utc) => utc is { } value ? Of(value) : null;

    public static BreakSpan Span(DateTime startedUtc, DateTime? endedUtc) => new(Of(startedUtc), Of(endedUtc));
}

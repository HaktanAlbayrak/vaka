using System.Globalization;

namespace Vaka.Web.Services;

public static class EventCanonical
{
    private const string MomentFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'";

    /// <summary>
    /// Özetin girdisi olan kanonik olay metni: kayıt|sıra|tür|yapan|an(UTC, 7 hane)|komut|yük.
    /// Yazma ve bütünlük doğrulaması bu tek biçimi kullanır; biçim değişirse eski zincirler doğrulanamaz.
    /// </summary>
    public static string Build(int recordId, int sequence, string eventType, int? actorUserId, DateTime occurredAtUtc, string? commandId, string payloadJson) =>
        string.Join(
            '|',
            recordId.ToString(CultureInfo.InvariantCulture),
            sequence.ToString(CultureInfo.InvariantCulture),
            eventType,
            actorUserId?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
            occurredAtUtc.ToString(MomentFormat, CultureInfo.InvariantCulture),
            commandId ?? string.Empty,
            payloadJson);
}

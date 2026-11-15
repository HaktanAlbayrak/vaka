using System.Globalization;
using System.Text.Json;

namespace Vaka.Web.Services;

/// <summary>Olay günlüğü satırının yükü çözülmüş hali; bozuk yük boş nesne sayılır (özet zaten tutmaz).</summary>
public sealed record ParsedEvent(int Sequence, string EventType, int? ActorUserId, DateTime OccurredAtUtc, JsonElement Payload)
{
    public static ParsedEvent From(int sequence, string eventType, int? actorUserId, DateTime occurredAtUtc, string payloadJson) =>
        new(sequence, eventType, actorUserId, occurredAtUtc, ParsePayload(payloadJson));

    public string? Text(string name) => TextOf(Payload, name);

    public int? Number(string name) => NumberOf(Payload, name);

    public bool? Flag(string name) => FlagOf(Payload, name);

    public DateTime? Moment(string name) =>
        Find(Payload, name) is { ValueKind: JsonValueKind.String } value
        && DateTime.TryParse(value.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var moment)
            ? moment
            : null;

    public int[]? Numbers(string name) =>
        Find(Payload, name) is { ValueKind: JsonValueKind.Array } value
            ? [.. value.EnumerateArray().Where(item => item.ValueKind == JsonValueKind.Number && item.TryGetInt32(out _)).Select(item => item.GetInt32())]
            : null;

    public JsonElement[] Items(string name) =>
        Find(Payload, name) is { ValueKind: JsonValueKind.Array } value ? [.. value.EnumerateArray()] : [];

    public bool Has(string name) => Find(Payload, name) is not null;

    public static string? TextOf(JsonElement element, string name) =>
        Find(element, name) is { ValueKind: JsonValueKind.String } value ? value.GetString() : null;

    public static int? NumberOf(JsonElement element, string name) =>
        Find(element, name) is { ValueKind: JsonValueKind.Number } value && value.TryGetInt32(out var number) ? number : null;

    public static bool? FlagOf(JsonElement element, string name) =>
        Find(element, name) is { ValueKind: JsonValueKind.True or JsonValueKind.False } value ? value.GetBoolean() : null;

    public static JsonElement? Find(JsonElement element, string name)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return null;
        }

        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return property.Value;
            }
        }

        return null;
    }

    private static JsonElement ParsePayload(string payloadJson)
    {
        try
        {
            using var document = JsonDocument.Parse(payloadJson);
            return document.RootElement.Clone();
        }
        catch (JsonException)
        {
            using var empty = JsonDocument.Parse("{}");
            return empty.RootElement.Clone();
        }
    }
}

using Vaka.Web.ViewModels.Records;

namespace Vaka.Web.Services;

/// <summary>Saklanan değer ile olaydaki değeri karşılaştırır, farkı biçimlenmiş metinle toplar (PL-11).</summary>
public sealed class StateMismatchCollector
{
    private const string MissingEvent = "Olay yok";
    private const string Empty = "Boş";

    private readonly ITurkeyClock _clock;
    private readonly IReadOnlyDictionary<int, string> _userNames;
    private readonly List<IntegrityMismatchViewModel> _items = [];

    public StateMismatchCollector(ITurkeyClock clock, IReadOnlyDictionary<int, string> userNames)
    {
        _clock = clock;
        _userNames = userNames;
    }

    public IReadOnlyList<IntegrityMismatchViewModel> Items => _items;

    /// <summary>Karşılaştırmalarda olay hiç bulunamadıysa sıra 0 verilir ve olaydaki değer "Olay yok" yazılır.</summary>
    public void Moment(string field, DateTime? stored, DateTime? eventValue, int sequence) =>
        Compare(field, stored?.Ticks, eventValue?.Ticks, FormatMoment(stored), FormatMoment(eventValue), sequence);

    public void Text(string field, string? stored, string? eventValue, int sequence) =>
        Compare(field, stored ?? string.Empty, eventValue ?? string.Empty, Quote(stored), Quote(eventValue), sequence);

    public void Number(string field, int? stored, int? eventValue, int sequence) =>
        Compare(field, stored, eventValue, stored?.ToString() ?? Empty, eventValue?.ToString() ?? Empty, sequence);

    public void User(string field, int? stored, int? eventValue, int sequence) =>
        Compare(field, stored, eventValue, Name(stored), Name(eventValue), sequence);

    public void Users(string field, IEnumerable<int> stored, IEnumerable<int>? eventValue, int sequence)
    {
        var left = stored.Order().ToArray();
        var right = eventValue?.Order().ToArray();
        Compare(field, string.Join(",", left), right is null ? null : string.Join(",", right), JoinNames(left), right is null ? MissingEvent : JoinNames(right), sequence);
    }

    public void Add(string field, string stored, string eventValue, int sequence) =>
        _items.Add(new IntegrityMismatchViewModel { Field = field, StoredValue = stored, EventValue = eventValue, Sequence = sequence });

    private void Compare(string field, object? stored, object? eventValue, string storedText, string eventText, int sequence)
    {
        if (!Equals(stored, eventValue))
        {
            Add(field, storedText, sequence == 0 ? MissingEvent : eventText, sequence);
        }
    }

    private string FormatMoment(DateTime? utc) => utc is { } value ? _clock.ToLocal(value).ToString("dd.MM.yyyy HH:mm:ss.fffffff") : Empty;

    private static string Quote(string? value) => string.IsNullOrEmpty(value) ? Empty : value;

    private string Name(int? userId) =>
        userId is { } id ? _userNames.GetValueOrDefault(id) ?? $"Kullanıcı {id}" : Empty;

    private string JoinNames(IEnumerable<int> ids) => string.Join(", ", ids.Select(id => Name(id)));
}

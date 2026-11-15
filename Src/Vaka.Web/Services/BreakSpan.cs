namespace Vaka.Web.Services;

/// <summary>Bir molanın aralığı; <paramref name="End"/> boşsa mola açıktır.</summary>
public readonly record struct BreakSpan(DateTimeOffset Start, DateTimeOffset? End);

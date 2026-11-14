namespace Vaka.Web.Services;

/// <summary>Vardiyanın günlük aralığı, Türkiye saatiyle, [Start, End). End &lt; Start ise ertesi gün biter (K-38).</summary>
public sealed record ShiftWindow(int Id, TimeOnly Start, TimeOnly End);

/// <summary>Bir anı içeren vardiyanın o güne düşen başlangıç ve bitiş anları.</summary>
public sealed record ShiftOccurrence(int ShiftId, DateTimeOffset StartAt, DateTimeOffset EndAt);

namespace Vaka.Web.Services;

/// <summary>Süre metninin bir parçası: rakam mono, birim sans yazılır (T-B14). <c>Unit</c> değerleri <c>sa</c>, <c>dk</c>, <c>sn</c>.</summary>
public sealed record DurationPart(int Value, string Unit);

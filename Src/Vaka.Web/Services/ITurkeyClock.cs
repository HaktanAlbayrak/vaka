namespace Vaka.Web.Services;

public interface ITurkeyClock
{
    TimeZoneInfo Zone { get; }

    /// <summary>K-32: UTC saklanan anın Türkiye saatiyle gösterimi.</summary>
    DateTime ToLocal(DateTime utc);

    /// <summary>Sunucu anının Türkiye saatine göre takvim günü (BR-12).</summary>
    DateOnly Today();
}

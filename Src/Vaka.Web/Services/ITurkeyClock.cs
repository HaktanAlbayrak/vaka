namespace Vaka.Web.Services;

public interface ITurkeyClock
{
    TimeZoneInfo Zone { get; }

    /// <summary>K-32: UTC saklanan anın Türkiye saatiyle gösterimi.</summary>
    DateTime ToLocal(DateTime utc);

    /// <summary>Türkiye takvim gününün başlangıcı (00:00), UTC; tarih aralığı süzgeçleri için. Gün sonu = ertesi günün başlangıcı (yarı açık aralık).</summary>
    DateTime StartOfDayUtc(DateOnly day);

    /// <summary>Sunucu anının Türkiye saatine göre takvim günü (BR-12).</summary>
    DateOnly Today();
}

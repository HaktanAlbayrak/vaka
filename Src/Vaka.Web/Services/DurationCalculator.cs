namespace Vaka.Web.Services;

public sealed class DurationCalculator : IDurationCalculator
{
    private const long BytesPerMegabyte = 1_048_576;
    private const long MultipartEnvelopeBytes = 65_536;

    private readonly ITurkeyClock _clock;

    public DurationCalculator(ITurkeyClock clock)
    {
        _clock = clock;
    }

    /// <summary>
    /// Oturumun işlemsizlikten kapanıp kapanmadığı (NFR-09, PL-24, K12).
    /// İşlemsizlik = şimdi - son kullanıcı isteği anı
    /// Oturum kapanır: İşlemsizlik &gt;= OturumSüresi (dakika)
    /// </summary>
    public bool IsSessionIdle(DateTimeOffset lastRequestAt, DateTimeOffset now, int sessionMinutes) =>
        now - lastRequestAt >= TimeSpan.FromMinutes(sessionMinutes);

    /// <summary>
    /// Vardiyanın ertesi gün bitip bitmediği (K-38).
    /// Ertesi gün biter: Bitiş &lt; Başlangıç
    /// </summary>
    public bool EndsNextDay(TimeOnly start, TimeOnly end) => end < start;

    /// <summary>
    /// İki vardiya aralığının çakışıp çakışmadığı (K-38, FR-49).
    /// Aralık = [Başlangıç, Bitiş); Bitiş &lt; Başlangıç ise günün sonunda ikiye bölünür: [Başlangıç, 24:00) ve [00:00, Bitiş)
    /// Çakışır: bir parçada A.Başlangıç &lt; B.Bitiş ve B.Başlangıç &lt; A.Bitiş
    /// </summary>
    public bool DoShiftsOverlap(ShiftWindow first, ShiftWindow second)
    {
        foreach (var (firstStart, firstEnd) in Segments(first))
        {
            foreach (var (secondStart, secondEnd) in Segments(second))
            {
                if (firstStart < secondEnd && secondStart < firstEnd)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// Verilen anı içeren vardiya (K-38; Faz 4 SH-13 ve Faz 6 SH-16 bu metodu kullanır).
    /// t = anın Türkiye saatindeki günün saati, d = anın Türkiye saatindeki takvim günü
    /// Normal vardiya (Bitiş &gt; Başlangıç): Başlangıç &lt;= t &lt; Bitiş ise içerir; başlangıç = d + Başlangıç, bitiş = d + Bitiş
    /// Gece yarısını geçen vardiya (Bitiş &lt; Başlangıç): t &gt;= Başlangıç ise başlangıç = d + Başlangıç, bitiş = (d + 1 gün) + Bitiş;
    /// t &lt; Bitiş ise başlangıç = (d - 1 gün) + Başlangıç, bitiş = d + Bitiş
    /// </summary>
    public ShiftOccurrence? FindShiftAt(DateTimeOffset moment, IReadOnlyList<ShiftWindow> shifts)
    {
        var local = TimeZoneInfo.ConvertTime(moment, _clock.Zone).DateTime;
        var day = DateOnly.FromDateTime(local);
        var time = TimeOnly.FromDateTime(local);

        foreach (var shift in shifts)
        {
            if (shift.End > shift.Start)
            {
                if (time >= shift.Start && time < shift.End)
                {
                    return Occurrence(shift, day, day);
                }
            }
            else if (time >= shift.Start)
            {
                return Occurrence(shift, day, day.AddDays(1));
            }
            else if (time < shift.End)
            {
                return Occurrence(shift, day.AddDays(-1), day);
            }
        }

        return null;
    }

    /// <summary>
    /// Lotun SKT'sinin geçmiş olup olmadığı (BR-12, K-12).
    /// SKT geçmiş: SKT &lt; bugün (Türkiye saatine göre takvim günü)
    /// </summary>
    public bool IsLotExpired(DateOnly expiryDate, DateOnly today) => expiryDate < today;

    private ShiftOccurrence Occurrence(ShiftWindow shift, DateOnly startDay, DateOnly endDay) =>
        new(shift.Id, AtLocal(startDay, shift.Start), AtLocal(endDay, shift.End));

    private DateTimeOffset AtLocal(DateOnly day, TimeOnly time)
    {
        var local = day.ToDateTime(time);
        return new DateTimeOffset(local, _clock.Zone.GetUtcOffset(local));
    }

    private static (long Start, long End)[] Segments(ShiftWindow shift) =>
        shift.End > shift.Start
            ? [(shift.Start.Ticks, shift.End.Ticks)]
            : [(shift.Start.Ticks, TimeSpan.TicksPerDay), (0, shift.End.Ticks)];

    public int MinutesToSeconds(int minutes) => checked(minutes * 60);

    public int SecondsToMinutes(int seconds) => seconds / 60;

    public bool IsMinimumMissing(bool isPlanned, int? minDurationSeconds) => isPlanned && minDurationSeconds is null;

    public bool IsLockWithinLimit(int lockMinutes, int maxLockMinutes) => lockMinutes > 0 && lockMinutes <= maxLockMinutes;

    public bool IsMediaWithinLimit(long sizeBytes, int maxMegabytes) => sizeBytes > 0 && sizeBytes <= GetMediaLimitBytes(maxMegabytes);

    public long GetMediaLimitBytes(int maxMegabytes) => (long)maxMegabytes * BytesPerMegabyte;

    public long GetUploadBodyLimitBytes(int maxPhotoMb, int maxVideoMb) =>
        (long)Math.Max(maxPhotoMb, maxVideoMb) * BytesPerMegabyte + MultipartEnvelopeBytes;
}

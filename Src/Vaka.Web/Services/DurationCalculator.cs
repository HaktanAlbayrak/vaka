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

    /// <summary>
    /// Makine kilidinin bitişi (SH-13, BR-45, K-38).
    /// Kilit bitişi = en erken (Açılış + KilitSüresi dakika, açılış anını içeren vardiyanın bitişi)
    /// Açılış bir vardiyaya düşmüyorsa ya da tesiste aktif vardiya yoksa: Kilit bitişi = Açılış + KilitSüresi dakika
    /// </summary>
    public DateTimeOffset CalculateLockExpiry(DateTimeOffset openedAt, int lockMinutes, ShiftOccurrence? shiftAtOpening)
    {
        var byLock = openedAt.AddMinutes(lockMinutes);
        return shiftAtOpening is { } shift && shift.EndAt < byLock ? shift.EndAt : byLock;
    }

    /// <summary>
    /// Kilit bitişinin geçip geçmediği (BR-26, BR-46).
    /// Geçti: Şimdi &gt;= Kilit bitişi (bitiş anının kendisinde kilit kalkar)
    /// </summary>
    public bool IsLockExpired(DateTimeOffset lockExpiresAt, DateTimeOffset now) => now >= lockExpiresAt;

    public int CalculateStepBreaks(DateTimeOffset stepStart, DateTimeOffset stepEnd, IReadOnlyList<BreakSpan> breaks) =>
        OverlapSeconds(stepStart, stepEnd, breaks, null);

    public int CalculateStepDuration(DateTimeOffset start, DateTimeOffset end, int stepBreakSeconds) =>
        Math.Max(0, WholeSeconds(end - start) - stepBreakSeconds);

    public int CalculateStepEffort(int durationSeconds, int participantCount) => checked(durationSeconds * participantCount);

    public int CalculatePhaseGross(DateTimeOffset firstStepStart, DateTimeOffset lastStepEnd) =>
        Math.Max(0, WholeSeconds(lastStepEnd - firstStepStart));

    public int CalculatePhaseBreaks(DateTimeOffset phaseStart, DateTimeOffset phaseEnd, IReadOnlyList<BreakSpan> breaks) =>
        OverlapSeconds(phaseStart, phaseEnd, breaks, null);

    public int CalculatePhaseNet(int phaseGrossSeconds, int phaseBreakSeconds) => Math.Max(0, phaseGrossSeconds - phaseBreakSeconds);

    public bool IsBelowMinimum(int phaseNetSeconds, int? minDurationSeconds) =>
        minDurationSeconds is { } minimum && phaseNetSeconds < minimum;

    public int CalculateRecordGross(DateTimeOffset firstStepStart, DateTimeOffset lastStepEnd) =>
        Math.Max(0, WholeSeconds(lastStepEnd - firstStepStart));

    public int CalculateRecordBreaks(DateTimeOffset recordStart, DateTimeOffset recordEnd, IReadOnlyList<BreakSpan> breaks) =>
        OverlapSeconds(recordStart, recordEnd, breaks, null);

    public int CalculateRecordNet(int recordGrossSeconds, int recordBreakSeconds) => Math.Max(0, recordGrossSeconds - recordBreakSeconds);

    public int CalculateRecordEffort(IReadOnlyList<int> stepEffortSeconds) => stepEffortSeconds.Sum();

    public int CalculateLiveElapsed(DateTimeOffset start, DateTimeOffset now, IReadOnlyList<BreakSpan> breaks)
    {
        if (now <= start)
        {
            return 0;
        }

        var ticks = (now - start).Ticks - OverlapTicks(start, now, breaks, now);
        return Math.Max(0, WholeSeconds(TimeSpan.FromTicks(ticks)));
    }

    public int CalculateAverage(IReadOnlyList<int> values) =>
        values.Count == 0 ? 0 : (int)(values.Sum(value => (long)value) / values.Count);

    public int CalculateBreakSeconds(DateTimeOffset breakStart, DateTimeOffset breakEnd) => Math.Max(0, WholeSeconds(breakEnd - breakStart));

    private static int OverlapSeconds(DateTimeOffset from, DateTimeOffset to, IReadOnlyList<BreakSpan> breaks, DateTimeOffset? openBreakEnd) =>
        WholeSeconds(TimeSpan.FromTicks(OverlapTicks(from, to, breaks, openBreakEnd)));

    private static long OverlapTicks(DateTimeOffset from, DateTimeOffset to, IReadOnlyList<BreakSpan> breaks, DateTimeOffset? openBreakEnd)
    {
        var ticks = 0L;
        foreach (var span in breaks)
        {
            var end = span.End ?? openBreakEnd;
            if (end is null)
            {
                continue;
            }

            var overlap = (end.Value < to ? end.Value : to) - (span.Start > from ? span.Start : from);
            if (overlap > TimeSpan.Zero)
            {
                ticks += overlap.Ticks;
            }
        }

        return ticks;
    }

    /// <summary>
    /// Kaydın planlanan minimum süresi, saniye (SH-18, PL-85).
    /// Planlanan minimum = Σ kaydın <c>RecordPhases.MinDurationSecondsSnapshot</c> dolu değerleri; hiçbiri dolu değilse boş
    /// </summary>
    public int? CalculateRecordPlannedMinimum(IReadOnlyList<int?> phaseMinimumSnapshots) =>
        phaseMinimumSnapshots.Any(value => value is not null) ? phaseMinimumSnapshots.Sum(value => value ?? 0) : null;

    /// <summary>
    /// Plan farkı, saniye (SH-19, PL-85).
    /// Plan farkı = ortalama net süre (SH-15) - ortalama planlanan minimum (SH-15 ile alınmış SH-18 değerleri)
    /// </summary>
    public int CalculatePlanVariance(int averageNetSeconds, int averagePlannedMinimumSeconds) =>
        averageNetSeconds - averagePlannedMinimumSeconds;

    private static int WholeSeconds(TimeSpan span) => (int)Math.Floor(span.TotalSeconds);
}

namespace Vaka.Web.Services;

public interface IDurationCalculator
{
    /// <summary>
    /// Oturumun işlemsizlikten kapanıp kapanmadığı (NFR-09, PL-24, K12).
    /// İşlemsizlik = şimdi - son kullanıcı isteği anı
    /// Oturum kapanır: İşlemsizlik &gt;= OturumSüresi (dakika)
    /// </summary>
    /// <remarks>
    /// Son istek anı her kullanıcı isteğinde yenilenir; çerezin kayan süresi kullanılmaz. Saat geri giderse
    /// işlemsizlik negatif çıkar ve oturum kapanmaz.
    /// </remarks>
    bool IsSessionIdle(DateTimeOffset lastRequestAt, DateTimeOffset now, int sessionMinutes);

    /// <summary>
    /// Vardiyanın ertesi gün bitip bitmediği (K-38).
    /// Ertesi gün biter: Bitiş &lt; Başlangıç
    /// </summary>
    bool EndsNextDay(TimeOnly start, TimeOnly end);

    /// <summary>
    /// İki vardiya aralığının çakışıp çakışmadığı (K-38, FR-49).
    /// Aralık = [Başlangıç, Bitiş); Bitiş &lt; Başlangıç ise günün sonunda ikiye bölünür: [Başlangıç, 24:00) ve [00:00, Bitiş)
    /// Çakışır: bir parçada A.Başlangıç &lt; B.Bitiş ve B.Başlangıç &lt; A.Bitiş
    /// </summary>
    /// <remarks>
    /// Bitişi diğerinin başlangıcına eşit aralıklar çakışmaz. Başlangıç ve bitişi aynı olan aralık 24 saat sayılır;
    /// vardiya formu bunu zaten reddeder. Yalnız aktif vardiyalar karşılaştırılır, bu seçimi çağıran yapar.
    /// </remarks>
    bool DoShiftsOverlap(ShiftWindow first, ShiftWindow second);

    /// <summary>
    /// Verilen anı içeren vardiya (K-38; Faz 4 SH-13 ve Faz 6 SH-16 bu metodu kullanır).
    /// t = anın Türkiye saatindeki günün saati, d = anın Türkiye saatindeki takvim günü
    /// Normal vardiya (Bitiş &gt; Başlangıç): Başlangıç &lt;= t &lt; Bitiş ise içerir; başlangıç = d + Başlangıç, bitiş = d + Bitiş
    /// Gece yarısını geçen vardiya (Bitiş &lt; Başlangıç): t &gt;= Başlangıç ise başlangıç = d + Başlangıç, bitiş = (d + 1 gün) + Bitiş;
    /// t &lt; Bitiş ise başlangıç = (d - 1 gün) + Başlangıç, bitiş = d + Bitiş
    /// </summary>
    /// <remarks>
    /// Aralık başlangıç dahil bitiş hariçtir: bitiş anının kendisi sonraki vardiyaya düşer. Hiçbir vardiya içermiyorsa null.
    /// Çağıran yalnız aktif vardiyaları verir; aktif vardiyalar çakışmadığı için en çok bir sonuç çıkar.
    /// </remarks>
    ShiftOccurrence? FindShiftAt(DateTimeOffset moment, IReadOnlyList<ShiftWindow> shifts);

    /// <summary>
    /// Lotun SKT'sinin geçmiş olup olmadığı (BR-12, K-12).
    /// SKT geçmiş: SKT &lt; bugün (Türkiye saatine göre takvim günü)
    /// </summary>
    /// <remarks>Bugünkü SKT geçmiş sayılmaz; lot o günün sonuna kadar kullanılabilir.</remarks>
    bool IsLotExpired(DateOnly expiryDate, DateOnly today);

    /// <summary>
    /// Faz minimumunun dakikadan saniyeye çevrimi (PL-14, FR-05); saklanan değer saniyedir.
    /// Saniye = Dakika x 60
    /// </summary>
    int MinutesToSeconds(int minutes);

    /// <summary>
    /// Saklanan faz minimumunun düzenleyicide gösterilen dakikası.
    /// Dakika = Saniye / 60 (tam bölme; arayüz yalnız tam dakika yazar, kopyalanan değer de dakikadan gelir)
    /// </summary>
    int SecondsToMinutes(int seconds);

    /// <summary>
    /// Planlı tipte fazın yayını engelleyen minimum eksiği (PL-14).
    /// Eksik: tip planlı ve faz minimumu tanımsız (boş)
    /// </summary>
    /// <remarks>Acil tipte minimum alanı yoktur, eksik sayılmaz.</remarks>
    bool IsMinimumMissing(bool isPlanned, int? minDurationSeconds);

    /// <summary>
    /// Varsayılan kilit süresinin azami sınır içinde olup olmadığı (BR-45).
    /// Kabul: 0 &lt; Dakika &lt;= AzamiKilitSüresi (dakika; sınıra eşit kabul)
    /// </summary>
    bool IsLockWithinLimit(int lockMinutes, int maxLockMinutes);

    /// <summary>
    /// Medya dosyasının azami boyut içinde olup olmadığı (K-26, PL-47, PL-51).
    /// Sınır baytı = AzamiMB x 1 048 576; Kabul: 0 &lt; Boyut &lt;= Sınır baytı (sınıra eşit kabul)
    /// </summary>
    bool IsMediaWithinLimit(long sizeBytes, int maxMegabytes);

    /// <summary>Medya sınırı (bayt) = AzamiMB x 1 048 576 (PL-51).</summary>
    long GetMediaLimitBytes(int maxMegabytes);

    /// <summary>
    /// İstek gövdesi üst sınırı (bayt) = max(AzamiFotoğrafMB, AzamiVideoMB) x 1 048 576 + zarf payı;
    /// zarf payı çok parçalı gövdedeki alan başlıkları ve antiforgery alanı içindir.
    /// </summary>
    long GetUploadBodyLimitBytes(int maxPhotoMb, int maxVideoMb);

    /// <summary>
    /// Makine kilidinin bitişi (SH-13, BR-45, K-38).
    /// Kilit bitişi = en erken (Açılış + KilitSüresi dakika, açılış anını içeren vardiyanın bitişi)
    /// Açılış bir vardiyaya düşmüyorsa ya da tesiste aktif vardiya yoksa: Kilit bitişi = Açılış + KilitSüresi dakika
    /// </summary>
    /// <remarks>
    /// Vardiya aralığı başlangıç dahil bitiş hariçtir; <paramref name="shiftAtOpening"/> bunu <see cref="FindShiftAt"/> ile bulunmuş olarak verir.
    /// Gece yarısını geçen vardiyanın bitişi ertesi güne düşer. Açılışta bir kez hesaplanır ve saklanır; vardiya sonradan değişse de kayıt değişmez.
    /// </remarks>
    DateTimeOffset CalculateLockExpiry(DateTimeOffset openedAt, int lockMinutes, ShiftOccurrence? shiftAtOpening);

    /// <summary>
    /// Kilit bitişinin geçip geçmediği (BR-26, BR-46).
    /// Geçti: Şimdi &gt;= Kilit bitişi (bitiş anının kendisinde kilit kalkar)
    /// </summary>
    /// <remarks>Zaman aşımı anı olarak kaydedilen değer şimdi değil kilit bitişidir.</remarks>
    bool IsLockExpired(DateTimeOffset lockExpiresAt, DateTimeOffset now);
}

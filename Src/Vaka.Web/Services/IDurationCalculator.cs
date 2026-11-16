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

    /// <summary>
    /// Adımla kesişen mola süresi, saniye (SH-17, K-39, PL-19).
    /// Adım molası = Σ max(0, min(Mola bitişi, Adım bitişi) - max(Mola başlangıcı, Adım başlangıcı)), yalnız kapanmış molalar
    /// </summary>
    /// <remarks>
    /// Faz 5'te mola yalnız iki adım arasında verildiğinden hiçbir mola bir adım aralığıyla kesişmez ve sonuç 0 olur;
    /// adım sürerken verilen mola Faz 7'de aynı formülle değer üretir (PL-57). Açık mola hesaba girmez.
    /// </remarks>
    int CalculateStepBreaks(DateTimeOffset stepStart, DateTimeOffset stepEnd, IReadOnlyList<BreakSpan> breaks);

    /// <summary>
    /// Adım süresi, saniye (SH-01).
    /// Süre = (Bitiş - Başlangıç) - SH-17 adım molası; saniyeye aşağı yuvarlanır, 0'ın altına inmez
    /// </summary>
    int CalculateStepDuration(DateTimeOffset start, DateTimeOffset end, int stepBreakSeconds);

    /// <summary>
    /// Kişinin adımdaki çalışma süresi, saniye (SH-02a, O-11).
    /// Kişi başlangıcı = en geç (katılma anı, adım başlangıcı); kişi bitişi = en erken (ayrılma anı, adım bitişi); ayrılmamışsa adım bitişi
    /// Çalışma = (Kişi bitişi - Kişi başlangıcı) - Σ max(0, min(Mola bitişi, Kişi bitişi) - max(Mola başlangıcı, Kişi başlangıcı)), yalnız kapanmış kayıt molaları; saniyeye aşağı yuvarlanır, 0'ın altına inmez
    /// </summary>
    /// <remarks>
    /// Adımın tamamında çalışan kişide sonuç SH-01 adım süresine eşittir (aynı yuvarlama). Katılma anı adım başlangıcından önceyse adım başlangıcı geçerlidir.
    /// </remarks>
    int CalculateParticipantWork(DateTimeOffset stepStart, DateTimeOffset stepEnd, DateTimeOffset joinedAt, DateTimeOffset? leftAt, IReadOnlyList<BreakSpan> breaks);

    /// <summary>
    /// Adım eforu, saniye (SH-02, O-11).
    /// Efor = Σ adımdaki kişilerin SH-02a çalışma süreleri
    /// </summary>
    /// <remarks>
    /// Adımın tamamında çalışan iki kişide efor = süre x 2. Girişi yapmamış kişi efor almaz. Faz 8'de görevli düzeltmesi geldiğinde orijinal değer korunur ve düzeltilmiş efor ayrıca gösterilir (PL-09).
    /// </remarks>
    int CalculateStepEffort(IReadOnlyList<int> participantWorkSeconds);

    /// <summary>
    /// Faz brüt süresi, saniye (SH-03).
    /// Brüt = Fazın son adımının bitişi - fazın ilk adımının başlangıcı
    /// </summary>
    /// <remarks>Kayıt açılış anı ve adımlar arası boşluk faz aralığı içindedir; açılış ile ilk adım arası hiçbir süreye girmez (BR-13).</remarks>
    int CalculatePhaseGross(DateTimeOffset firstStepStart, DateTimeOffset lastStepEnd);

    /// <summary>
    /// Faz molası, saniye (SH-04).
    /// Faz molası = Σ max(0, min(Mola bitişi, Faz bitişi) - max(Mola başlangıcı, Faz başlangıcı)), yalnız kapanmış molalar
    /// </summary>
    /// <remarks>Faz aralığının dışında kalan mola (iki faz arası) faz molasına girmez, kayıt molasına girer (SH-09).</remarks>
    int CalculatePhaseBreaks(DateTimeOffset phaseStart, DateTimeOffset phaseEnd, IReadOnlyList<BreakSpan> breaks);

    /// <summary>
    /// Faz net süresi, saniye (SH-06).
    /// Net = SH-03 faz brüt - SH-04 faz molası; 0'ın altına inmez
    /// </summary>
    /// <remarks>Faz 5'te beklemeler (SH-05) yoktur, terim Faz 7'de eklenir. Mola verilmeden geçen boşluk net süreye dahildir (SC-06).</remarks>
    int CalculatePhaseNet(int phaseGrossSeconds, int phaseBreakSeconds);

    /// <summary>
    /// Faz minimum kontrolü (SH-07, BR-21).
    /// Süre altında: minimum tanımlıysa ve SH-06 faz net &lt; minimum (saniye); minimum tanımsızsa false
    /// </summary>
    /// <remarks>Net süre minimuma eşitse süre altı sayılmaz.</remarks>
    bool IsBelowMinimum(int phaseNetSeconds, int? minDurationSeconds);

    /// <summary>
    /// Kayıt brüt süresi, saniye (SH-08).
    /// Brüt = Son adımın bitişi - ilk adımın başlangıcı
    /// </summary>
    /// <remarks>Açılış anı ayrı alandır; açılış ile ilk adım arasındaki süre hesaba girmez (BR-13).</remarks>
    int CalculateRecordGross(DateTimeOffset firstStepStart, DateTimeOffset lastStepEnd);

    /// <summary>
    /// Kayıt molası, saniye (SH-09).
    /// Kayıt molası = Σ max(0, min(Mola bitişi, Kayıt bitişi) - max(Mola başlangıcı, Kayıt başlangıcı)), yalnız kapanmış molalar
    /// </summary>
    int CalculateRecordBreaks(DateTimeOffset recordStart, DateTimeOffset recordEnd, IReadOnlyList<BreakSpan> breaks);

    /// <summary>
    /// Kayıt net süresi, saniye (SH-11).
    /// Net = SH-08 kayıt brüt - SH-09 kayıt molası; 0'ın altına inmez
    /// </summary>
    /// <remarks>Faz 5'te bekleme (SH-10) yoktur, terim Faz 7'de eklenir.</remarks>
    int CalculateRecordNet(int recordGrossSeconds, int recordBreakSeconds);

    /// <summary>
    /// Kayıt eforu, saniye (SH-12).
    /// Efor = Σ kaydın tamamlanmış adımlarının SH-02 eforu
    /// </summary>
    int CalculateRecordEffort(IReadOnlyList<int> stepEffortSeconds);

    /// <summary>
    /// Canlı geçen süre, saniye (SH-14); adım, faz ve kayıt sayaçları için, sunucu hesaplar, JS yalnız ilerletir.
    /// Canlı = (Şimdi - Başlangıç) - Σ max(0, min(Mola bitişi ya da Şimdi, Şimdi) - max(Mola başlangıcı, Başlangıç)); açık molanın bitişi Şimdi sayılır
    /// Fark tick düzeyinde alınır ve yalnız sonuç saniyeye aşağı yuvarlanır (tek yuvarlama)
    /// </summary>
    /// <remarks>
    /// Açık mola süresince Şimdi ile mola başlangıcı birlikte ilerler, fark sabit kalır: değer artmaz. Mola bitince değer kaldığı yerden sürer.
    /// Başlangıç Şimdi'den sonraysa 0. Faz 7'de beklemeler düşülür.
    /// </remarks>
    int CalculateLiveElapsed(DateTimeOffset start, DateTimeOffset now, IReadOnlyList<BreakSpan> breaks);

    /// <summary>
    /// Mola süresi, saniye (SH-14 canlı süre ve mola listesi, mola ekranı ve bandı).
    /// Açık mola: Süre = Sunucu anı - Mola başlangıcı (bitiş yerine sunucu anı verilir); kapanmış mola: Süre = Bitiş - Başlangıç; 0'ın altına inmez
    /// </summary>
    int CalculateBreakSeconds(DateTimeOffset breakStart, DateTimeOffset breakEnd);

    /// <summary>
    /// Rapor ortalaması, saniye (SH-15, PL-56).
    /// Ortalama = Σ değer / kayıt sayısı; saniyeye aşağı yuvarlanır (tam bölme); değer yoksa 0
    /// </summary>
    /// <remarks>
    /// Çağıran yalnız Tamamlandı kayıtların değerlerini verir: İptal, Yarıda kapatıldı, Zaman aşımı, Açıldı, Devam ediyor ve Beklemede kayıtlar ortalamaya girmez.
    /// Faz ve adım ortalamaları aynı tip versiyonunun kayıtları içinde alınır; farklı versiyonlar ayrı satırdır. Toplam long ile alınır, taşma olmaz.
    /// </remarks>
    int CalculateAverage(IReadOnlyList<int> values);

    /// <summary>
    /// Kaydın planlanan minimum süresi, saniye (SH-18, PL-85).
    /// Planlanan minimum = Σ kaydın <c>RecordPhases.MinDurationSecondsSnapshot</c> dolu değerleri; hiçbiri dolu değilse boş
    /// </summary>
    /// <remarks>Acil kaydın fazlarında minimum yoktur, sonuç boştur. Anlık değer kullanılır; prosedür sonradan değişse de kayıt etkilenmez.</remarks>
    int? CalculateRecordPlannedMinimum(IReadOnlyList<int?> phaseMinimumSnapshots);

    /// <summary>
    /// Plan farkı, saniye (SH-19, PL-85).
    /// Plan farkı = ortalama net süre (SH-15) - ortalama planlanan minimum (SH-15 ile alınmış SH-18 değerleri)
    /// </summary>
    /// <remarks>Pozitif fark ortalamanın planlanan minimumdan uzun sürdüğünü, negatif fark kısa sürdüğünü gösterir.</remarks>
    int CalculatePlanVariance(int averageNetSeconds, int averagePlannedMinimumSeconds);
}

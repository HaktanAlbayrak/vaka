# Faz Planı

> Sahibi: analyst. Kullanıcı onayıyla donar; sonraki kararlar `Docs/Phases/Decisions.md` dosyasına eklenir.
> Durum: **ONAYLANDI** (2026-11-13, `Decisions.md` O-01). K-38, K-39, K-40, K-47 ve PL-05 kullanıcı yanıtlarıyla işlendi.
> 2026-11-14: Salı takvimi B (O-03), Faz 6 ve 7 yeniden yapılandı (PL-46), Faz 3 ve 4 açıkları kapandı (PL-47, PL-48, PL-50).
> 2026-11-15: Doküman Salı öncesi tamamlanır (O-04): yeni Faz 6A, K-39 ve K-10 Faz 7'ye, Ö-01..Ö-05 Salı öncesi (PL-56..PL-59).
> Kimlikler: FR, NFR, BR, SC, SH, SK `Docs/Analysis/Brief.md`; E (ekran) `Docs/Analysis/Requirements.md §4`; K `Docs/Analysis/Kusur.md`; PL `Decisions.md`.

## Özet

| Faz | Başlık | Kullanıcının göreceği çıktı | Paralel | Durum |
|---|---|---|---|---|
| 0 | Analiz ve faz planı | Brief, Kusur, Requirements, plan | - | TAMAMLANDI (2026-11-13) |
| T | Tasarım DNA'sı ve kabuk prototipi | DNA kartı, demo veriyle 5 prototip sayfa | Hayır | TESLİM ALINDI (tur 3, 2026-11-13) |
| 1 | Giriş, PIN, kullanıcılar ve Süper admin | Giriş, ilk giriş, PIN, kullanıcı yönetimi, Süper admin | Evet | TESLİM ALINDI (tur 3, 2026-11-14) |
| 2 | Tesis yapısı, vardiya ve malzeme | Tesis, hat, makine, vardiya, malzeme ve lot ekranları | Evet | TESLİM ALINDI (tur 2, 2026-11-14) |
| 3 | Temizlik tipi ve prosedür versiyonu | Tip, makine eşleme, taslak düzenleyici, yayınlama | Evet | TESLİM ALINDI (tur 3, 2026-11-15) |
| 4 | Kayıt açma ve makine kilidi | Yeni kayıt, İşlerim, Açıldı hali, makine panosu, zaman aşımı | Evet | TESLİM ALINDI (tur 2, 2026-11-15) |
| 5 | Adım yürütme, görevli ve mola | Adım başlat/bitir, sahibin eklediği görevliler, iki adım arasında mola, otomatik tamamlanma | Evet | TESLİM ALINDI (tur 1, 2026-11-15) |
| 6 | Kayıt listesi, detay, bütünlük ve müdahale | 13 soruyu yanıtlayan detay ve yazdırılabilir denetim çıktısı, olay günlüğü, bütünlük, iptal, yarıda kapatma, yöneticinin molayı bitirmesi | Evet | TESLİM ALINDI (tur 2, 2026-11-16) |
| 6A | Acil müdahale, süre raporu ve malzeme izlenebilirliği | Acil müdahale ekranı ve "Acil" ayrımı, süre ve efor raporu (tablo), lot ve makine bazlı malzeme sorgusu | Evet | TESLİM ALINDI (tur 1, 2026-11-16) |
| 7 | Talep, beklemeye alma ve yönetici panosu (Salı sonrası) | Talepler, Taleplerim, bekleme ve devam, yönetici panosu, adım sürerken mola, görevli PIN onayı | Evet | BEKLİYOR |
| 8 | Acil müdahale, bildirim ve düzeltme (Salı sonrası) | Acil ve talep bildirimleri, panoda acil, düzeltme formu | Evet | BEKLİYOR |
| 9 | Raporlar, denetim çıktısı ve sistem ayarları (Salı sonrası) | Rapor grafikleri ve CSV, düzeltmeli denetim çıktısı adresi, Sistem ayarları ekranı, son regresyon | Evet | BEKLİYOR |

Durum değerleri: BEKLİYOR, GELİŞTİRMEDE, QA'DA, İADE, TESLİM ALINDI. Faz 0 QA'sız analiz fazıdır, kullanıcı onayıyla TAMAMLANDI olur.

### Salı (2026-11-17) takvimi (O-03, O-04, O-05, PL-56..PL-59)

2026-11-14'te B seçildi (çekirdek önce). 2026-11-15'te kullanıcı dokümanın Salı'da tam karşılanmasını istedi (O-04): doküman beklentileri
Salı öncesine alındı, yer açmak için kullanıcının kendi iki eki (K-39 adım sürerken mola, K-10 görevli PIN onayı) Salı sonrasına kaydı.

- Salı'dan önce sırayla: Faz 3, Faz 4, Faz 5, Faz 6, Faz 6A. Salı'dan sonra: Faz 7, 8, 9.
- Kontrol noktaları (PL-59, O-05), bilgi amaçlı: Pazar (2026-11-15) sonu Faz 3; Pazartesi (2026-11-16) öğlen Faz 4; Pazartesi sonu Faz 5;
  Salı sunumdan önce Faz 6 ve Faz 6A. Nokta kaçınca koordinatör durmaz, sıradaki faza geçer.
- Öncelik (O-05): doküman kalemleri (Faz 3, 4, 5, 6, 6A) kesintisiz önce tamamlanır, Salı'ya yetişmese de; ardından Faz 7, 8, 9 sırayla.
- Faz 6A'dan sonra, Faz 7'den önce "Sunum hazırlığı" (faz değil, faz kapısı dışı; PL-65): kullanıcının veritabanında uygulama üzerinden gerçek
  zamanlı 2-3 kayıt. Sunum senaryosu `Docs/Local/SunumSenaryosu.md`, her faz tesliminde güncellenir.
- Kabul ekleri Salı öncesi (PL-58): Ö-01 Faz 4 demo verisi, Ö-05 Faz 4 K4, Ö-02 Faz 5 K22, Ö-03 Faz 5 K16, Ö-04 Faz 5 K23.
- Bilinçli tasarım sınırları değişmez, sunumda gerekçesiyle anlatılır: periyodik takvim yok (satır 3, K-34), kişiye makine yetkisi yok (13, K-20),
  saniye eşiği yok (128, İÖ-06).

### Faz sayısı neden arttı

İlk taslak 0, T, 1-6 idi. 2026-11-13 kararları kapsamı büyüttü ve "fazda en çok 3-4 ekran" kuralı bölünme gerektirdi:

- K-09 (ilk giriş, PIN, PIN kilidi) kimliği tek başına bir faz yaptı; eskiden kimlikle aynı fazdaki tesis yapısı Faz 2'ye ayrıldı, K-38 vardiyası oraya girdi.
- Tip ve prosedür (Faz 3) ile kayıt açma (Faz 4) ve yürütme (Faz 5) ayrıldı; K-02 molası, K-10 görevli PIN onayı yürütmeyi büyüttü.
- K-05 talep yapısı ve K-08 beklemeye alma yeni bir faz getirdi (Faz 7).
- K-14 acil müdahale akışı ve uygulama içi bildirim yeni bir faz getirdi (Faz 8); düzeltme (K-16) Faz 7'nin yükünü dengelemek için buraya alındı (PL-13).

2026-11-13 kapanış yanıtları faz sayısını değiştirmedi; etkilenen fazlar:

- PL-05 Süper admin: Faz T (rol sabitleri, menü varyantı), Faz 1 (hesap kapsamı kuralı, seed, kabul kriterleri), Faz 2-9 (G10: Süper admin 403).
- K-39 adım sürerken mola: Faz 4 (adım mola kolonu önden), Faz 5 (kural, SH-17, sayaç durması, kabul kriterleri), Faz 6 ve 7 (gösterim, müdahale).
- K-47 ayar ekranı: Faz 1 (ayar tablosu ve servis), Faz 9 (E-33, Faz 1-8'de ekran sınırı dolu olduğu için, PL-17).
- PL-24, PL-25, PL-34 ayar değerleri: Faz 1 seed'i üç satır (oturum 30 dakika, PIN 4 hane, 5 deneme); Faz 3 kalan üç satırı seed ekleme
  olarak getirir (azami kilit 120 dakika; fotoğraf ve video boyutu açık, Faz 3 öncesi gerekir).
- PL-35 yerleşim ön turu: Faz 2 ve sonraki UI fazlarında teslim testinden önce qa-tester ön turu; faz sayısı ve tur sınırı değişmez.

2026-11-14 Salı takvimi (O-03, PL-46) faz sayısını değiştirmedi, Faz 6 ve 7'nin içeriğini değiştirdi:

- Faz 6: yönetici panosu (E-22) ve SH-16 çıktı; iptal, yarıda kapatma ve yöneticinin molayı bitirmesi (E-26) Faz 7'den geldi. Ekran sayısı 4.
- Faz 7: müdahalenin iptal ve yarıda kapatma kısmı çıktı; pano ve SH-16 geldi. Beklemeye alma ve talepler yerinde kaldı. Ekran sayısı 4.
- Faz 4: kapanış türü, kapatan ve kapanış gerekçesi kolonları kayıt tablosunda önden (PL-02), çünkü müdahale artık Faz 6'dadır.

2026-11-15 doküman tamamlama kararı (O-04, PL-56, PL-57) Faz 6A'yı ekledi:

- Faz 6A: acil müdahale akışı (Faz 8'den), süre raporu tablosu ve malzeme izlenebilirliği (Faz 9'dan). Ekran sayısı 3.
- Faz 6: detayın yazdırma çıktısı denetim çıktısı içeriğini taşır (Faz 9 K6, düzeltmeler hariç). Ekran sayısı değişmedi.
- Faz 5: adım sürerken mola (K-39) ve görevli PIN onayı (K-10) Faz 7'ye taşındı; mola iki adım arasında kalır.
- Faz 8 ve 9: taşınan kalemler çıktı; bildirim, düzeltme, grafik, CSV, ayar ekranı ve son regresyon kaldı.

---

## Tüm fazlarda geçerli kurallar

**Sözleşme biçimi**

- ViewModel'ler `ViewModels/<Alan>/` altında, `...ViewModel` sonekiyle. Zamanlar `...AtLocal` alanlarında Türkiye saatine çevrilmiş gelir; süreler
  `...Seconds` (tam sayı) alanlarında gelir. Görünüm süre hesaplamaz; saniyeyi metne çeviren tek biçimleyici backend'dedir (Faz 4'te gelir).
- Durum değiştiren her saha formunda `CommandId` (GET'te sunucu üretir, gizli alan) ve `Pin` alanı bulunur (BR-33, BR-48). Hiçbir komut
  ViewModel'inde istemciden zaman alanı yoktur (NFR-01).
- İş kuralı reddi servisten nedenli sonuç olarak döner; controller bunu form hatasına ya da 403 sayfasına çevirir (K-30). Reddedilen istek olay üretmez.
- Kabukta oturumdaki kişinin adı ve sicil no her ekranda görünür (NFR-09); görünüm `User.Identity.Name`, `User.IsInRole` ve sicil no claim'ini
  (`Authorization/VakaClaimTypes.cs`) okur. Menü role göre gizlenir; asıl koruma action'dadır.
- Rol metni yalnız `Authorization/Roles.cs` sabitlerinden gelir: `Operator`, `Manager`, `Auditor`, `SuperAdmin` ve birleşik `ManagerOrAuditor`,
  `ManagerOrSuperAdmin`, `RecordReaders`, `Any` (Requirements §2, PL-15).
- Sistem ayarları (`<OturumSüresi>`, `<PinUzunluğu>`, `<PinDenemeSınırı>`, `<AzamiMedyaBoyutu>`, `<AzamiKilitSüresi>`) yalnız ayar
  servisinden okunur (Faz 1); kodda, `appsettings*.json`'da ve görünümde sabit değer yoktur. Değer gereken ViewModel alanı servisten dolar.

**Paralel çalışma**

`Paralel: Evet` fazlarda backend-engineer sözleşmedeki ViewModel sınıflarını aynı adlarla ilk iş olarak yazar; frontend-engineer görünümleri
sözleşmedeki `@model` tipleriyle yazar. Ad uyuşmazlığı devir notuna yazılır, sözleşme değişikliği yalnız analist kararıyla (`Decisions.md`).
Veritabanı ihtiyacı backend-engineer'ın `Docs/Db/DB-<yıl>-<sıra>.md` talebiyle dbadmin'e gider; frontend şemaya bağlı değildir.

**Veritabanı**

Bir tablonun sonraki fazlarda gereken kolonları tablo ilk oluşturulurken eklenir; amaç mevcut tabloya ALTER (kullanıcı onayı, SQLite'ta tablo
yeniden kurma) gerektirmemektir (PL-02). Hangi kolonların önden geldiği her fazın DB kalemindedir. Yeni tablo, index ve tetikleyici o tabloyla
aynı migration'da oluşturulur.

**Süre hesapları: tek servis**

Tüm süre ve süreye bağlı kararlar `Services/IDurationCalculator.cs` ve `Services/DurationCalculator.cs` içindedir (NFR-16, `CodingStandards.md §7`).
Controller, görünüm ve JS süre aritmetiği yapmaz; canlı sayaç sunucunun verdiği değerden yalnız gösterimi ilerletir (SH-14).
Her metodun XML `<summary>`'sinde formül ve SH kimliği yazılır; bu, ilgili fazın kabul kriteridir. Metot adları öneridir, kimlik zorunludur.

| Formül | Metot önerisi | Geldiği faz | Not |
|---|---|---|---|
| SH-13 kilit bitişi | `CalculateLockExpiry` | 4 | Açılış anını içeren vardiyanın bulunması (gece yarısını geçen vardiya dahil) aynı metodun formülündedir |
| BR-26 zaman aşımı kararı | `IsLockExpired` | 4 | Zaman aşımı = durum Açıldı ve sunucu anı >= kilit bitişi |
| SH-17 adım molası | `CalculateStepBreaks` | 5 | Adımla kesişen mola süresi (K-39, PL-19); adım sürerken mola Faz 7'de açılır, Faz 5'te değer 0 (PL-57) |
| SH-01 adım süresi | `CalculateStepDuration` | 5 | Bitiş - başlangıç - SH-17 |
| SH-02 adım eforu | `CalculateStepEffort` | 5 | Faz 8'de görevli düzeltmesi için yorum güncellenir (PL-09) |
| SH-03 faz brüt | `CalculatePhaseGross` | 5 | |
| SH-04 faz molası | `CalculatePhaseBreaks` | 5 | Faz aralığıyla kesişen mola süresi |
| SH-06 faz net | `CalculatePhaseNet` | 5, 7 | Faz 5'te SH-05 terimi yok; Faz 7'de eklenir, yorum aynı commit'te değişir |
| SH-07 faz minimum kontrolü | `IsBelowMinimum` | 5 | Minimum tanımsızsa false |
| SH-08 kayıt brüt | `CalculateRecordGross` | 5 | |
| SH-09 kayıt molası | `CalculateRecordBreaks` | 5 | |
| SH-11 kayıt net | `CalculateRecordNet` | 5, 7 | Faz 7'de SH-10 terimi eklenir |
| SH-12 kayıt eforu | `CalculateRecordEffort` | 5 | |
| SH-14 canlı süre | `CalculateLiveElapsed` | 5, 7 | Açık mola süresince değer artmaz; Faz 7'de beklemeler düşülür |
| SH-16 vardiyası bitmiş açık iş | `IsPastShiftEnd` | 7 | PL-07; pano ile Faz 7'ye taşındı (PL-46) |
| SH-05 faz beklemesi | `CalculatePhaseHolds` | 7 | |
| SH-10 kayıt beklemesi | `CalculateRecordHolds` | 7 | |
| SH-15 rapor ortalaması | `CalculateAverage` | 6A | Yalnız Tamamlandı kayıtlar, versiyon içinde (PL-56) |

**Her kod fazında ortak kabul kriterleri** (qa-tester her fazın raporunda ayrıca işaretler)

- [ ] G1: `dotnet build Src/Vaka.Web -warnaserror` 0 hata 0 uyarı; `npm run build:css --prefix Src/Vaka.Web` başarılı.
- [ ] G2: `CodingStandards.md §4` grep'leri 0 eşleşme; her controller `[Authorize(Roles = Roles.X)]`, `[Authorize]` ya da `[AllowAnonymous]` taşır; rol metni yalnız `Roles` sabitlerinden.
- [ ] G3: UI ekranları `UiDelivery.md` §2 matrisinde kırık hücresiz, §4 sıfır ihlal, konsol 0 hata; DNA kartından sapma yok.
- [ ] G4: Operatör ekranları 360 px'te yatay kaydırmasız; birincil düğmeler en az 44 px yüksekliğinde (NFR-04); durumlar renk dışında metinle de ayrılır (NFR-05).
- [ ] G5: Antiforgery token'sız POST 400 döner (NFR-12).
- [ ] G5a (Faz 3 ve sonrası, PL-60): Fazın sayfalarındaki her `method="post"` formu HTML'de kendi `__RequestVerificationToken` gizli alanını taşır
  (`action` özniteliği yazılı formda `asp-antiforgery="true"`); qa-tester bunu ön turda ve teslim testinde mekanik olarak sayar. Mühendislerin
  ve QA'nın istek doğrulamalarında belirteç gönderilen formun kendi alanından alınır, başka formdan ya da sayfadan alınmaz.
- [ ] G5b (Faz 3 ve sonrası, PL-61): `bool` form alanları tarayıcıdan üç durumla doğru kaydedilir: işaretleyip kaydet -> true, işareti kaldırıp kaydet
  -> false, dokunmadan kaydet -> değer değişmez (iki başlangıç değeriyle, veritabanında doğrulanır). Fazın yeni ve değişen her formundaki her
  onay kutusu için qa-tester ölçer. `Views/**` içinde elle yazılmış `type="hidden"` ve `value="false"` alanı 0 eşleşme; `bool` onay kutusu `asp-for`
  ile çizilir. Mühendis doğrulaması tarayıcının gönderdiği gövdeyle (formdan) yapılır, elle kurulmuş gövdeyle değil.
- [ ] G6: Görünür metin, seed ve dokümanlarda em-dash ve en-dash 0 eşleşme.
- [ ] G7: Migration `Up()` gövdelerinde `Drop` yok; ALTER varsa `Docs/Db/` kaydında kullanıcı onayı yazılı.
- [ ] G8 (Faz 4 ve sonrası): `Controllers/` ve `Views/` altında `TimeSpan` aritmetiği, `.Subtract(` ya da `AtUtc` alanları arasında çıkarma yok; süreler yalnız `IDurationCalculator`'dan.
- [ ] G9 (Faz 4 ve sonrası): Fazın her yeni komutu aynı `CommandId` ile iki kez gönderildiğinde tek etki ve tek olay üretir (BR-33).
- [ ] G10 (Faz 2 ve sonrası): Yalnız `SuperAdmin` rolüne sahip kullanıcı fazın yeni adreslerinin tümünde (GET ve POST) 403 alır ve menüde bu ekranları görmez; istisna Faz 9'daki `/settings` (BR-34, PL-15).
- [ ] G11 (Faz 2 ve sonrası, UI fazları): Fazın her yeni liste ekranı 360, 768, 1024 ve 1440 px'te aşağıdaki yerleşim imzasını taşır; qa-tester imzayı
  Faz 1 `MekanikEval.txt` yöntemiyle `Alan:sütun@satır` biçiminde ölçer ve frontend devir notundaki beklenen imzayla karşılaştırır (PL-28, PL-33).
  - İşlem hücresi (satırın eylem düğmeleri; PL-55) kart modunda her zaman kartın son hücresidir; düğmeleri yan yana, tek satırda ve hücrenin sağ iç
    kenarına hizalı dizer; düğmeler hiçbir genişlikte alt alta yığılmaz. Kartın son satırında İşlem'den sonra boş hücre kalmaz.
  - 768 px altı satır kartı: 2 sütun; varsa tek uzun metin alanı kart başlığından sonraki ilk satırda tam satır; kalan alanlar ikişer yan yana.
    İşlem, kendinden önceki tek kalan alanın yanında 1 sütun alır yalnız düğmeleri orada tek satıra sığıyorsa; sığmıyorsa kendi satırında 2 sütun
    alır. Boş yarım hücre yalnız İşlem'den önceki son alan satırında ve yalnız İşlem kendi satırındayken olabilir.
  - 768-1279 px satır kartı: 4 sütun. İşlem son satırda kalan sütunların tamamını kaplar, en az 2 sütun; önceki satırlarda boş hücre yok.
    Bunu sağlamak için uzun metin alanı 1 ya da 2 sütun alır; yine sağlanamıyorsa bir alan daha 2 sütun alır (önce metin alanı). İşlem dışında
    tam satır hücre yok; İşlem'in 4 sütunluk kendi satırı serbesttir.
  - İmza örneği, `/machines` (alanlar Kod, Ad uzun metin, Tesis ve hat, Durum, İşlem): 768 ve 1024: `Kod:1@1 Ad:2@1 TesisHat:1@1 Durum:1@2 İşlem:3@2`;
    360, düğmeler yarım hücreye sığmıyorsa: `Ad:2@1 Kod:1@2 TesisHat:1@2 Durum:1@3 İşlem:2@4`. Frontend her ekranın beklenen imzasını devir notunda yazar.
  - 1280 px ve üstü: tablo, satır en az 56 px.
  - Form ve detay ekranları: lg ve üstünde ana kolon ve bağlam rayı yan yana, lg altında ray ana kolonun altında.
  - İstisna (PL-39): ana kolonda rayın yanında duran gömülü tablo 1024-1279 px'te 768 px altı kart kuralını izler (2 sütun).
- [ ] G12 (Faz 3 ve sonrası, UI fazları; PL-54): Fazın yeni ve değişen ekranlarında 360, 768, 1024 ve 1440 px'te, açık ve koyu temada hizalama
  kusuru yoktur. qa-tester `getBoundingClientRect` ile ölçer, 1 px'e kadar fark yuvarlama sayılır:
  - Alana gömülü düğme ya da ikon (PIN göster/gizle, arama ikonu): kutusu alanın kenarlık içi kutusunun içinde kalır, kenarlığa binmez;
    dikey merkezi alanın dikey merkeziyle aynıdır.
  - Aynı satırdaki form kontrolleri ve düğmeler (süzgeç satırı, alan + düğme): üst kenar ve yükseklik aynıdır; etiketleri aynı üst kenardadır.
  - Form alanında etiket, alan ve alan altı yardım ya da hata metninin sol kenarı aynıdır.
  - Panel başlığındaki ve gövdesindeki birinci düzey öğelerin sol kenarı panelin sol iç boşluk çizgisindedir; tam genişlik öğelerin sağ kenarı
    sağ iç boşluk çizgisindedir.
  - Kart ya da panel içindeki düğme satırında düğmelerin yüksekliği ve dikey merkezi aynıdır.
  - Satır kartında (360, 768, 1024): İşlem hücresindeki düğmelerin üst kenarı aynıdır (tek satır), en sağdaki düğmenin sağ kenarı hücrenin sağ
    iç kenarındadır; kartın her satırında hücre sağ kenarlarının en büyüğü kartın sağ iç kenarındadır (son satırda boşluk yok) (PL-55).
  Sonuç her ekran ve genişlik için `Ekran@genişlik: kusur 0` ya da kusurlu öğe ve px farkıyla yazılır. G12 kusuru ORTA'dır (aynı fazda kapanır).

**UI fazlarında yerleşim (PL-33)**

- frontend-engineer tarayıcıda ölçmediği yerleşim için devir notunda "DNA ile aynı" yazmaz. Her yeni liste ekranı için G11 imzasını genişlik başına
  "beklenen" olarak yazar (ör. 360: `Ad:2@1 Kod:1@2 Durum:1@2 İşlem:1@3`) ve hangi alanın uzun metin alanı olduğunu belirtir.
- Ekran G11 kalıbına uymayan bir yerleşim gerektiriyorsa frontend bunu devir notunda gerekçesiyle yazar; kabul analist kararıdır (`Decisions.md`).
- Analist iade ederken düzeltme maddesini ve qa-tester'ın yeniden denetim şartını aynı imzayla yazar. Yerleşim iadesinde yeniden denetim
  ekranın dört genişlikteki imzasının tamamını kapsar, yalnız kalan hücreyi değil.
- Geliştirme bitince, teslim testinden önce qa-tester yerleşim ön turu yapar (`Workflow.md §3` adım 2a, PL-35): fazın yeni ekranlarında yalnız
  G11 imzası, G12 hizalama ölçümü (Faz 3'ten başlayarak, PL-54), G5a form belirteci sayımı (PL-60), G5b gizli alan grep'i (PL-61) ve `UiDelivery.md §3` mekanik eval, 360, 768, 1024 ve 1440 px'te. Rapor yazılmaz, faz turu sayılmaz; bulgu listesi koordinatör
  üzerinden frontend'e döner. Ön tur temiz çıkmadan teslim testine geçilmez; teslim testinde G11 yine ölçülür.

---

## Faz 0: Analiz ve faz planı

**Amaç:** İş birimi anlatımını izlenebilir isterlere, rol matrisine ve teslim edilebilir fazlara çevirmek.

**Çıktılar:** `Docs/Analysis/Brief.md`, `Docs/Analysis/Kusur.md`, `Docs/Analysis/Requirements.md`, bu plan, `Docs/Phases/Decisions.md`.

**Kabul:** Kullanıcı Engelleyici K-38, K-39, K-40'ı yanıtlar, onay bekleyen varsayılanları toplu kabul eder ya da değiştirir ve planı onaylar.
Onay `Decisions.md`'ye işlenir, ardından `Docs/Local/ProjeOzeti.md` açılır.

**Sonuç (2026-11-13):** TAMAMLANDI. Yanıtlar işlendi (O-01), `Docs/Local/ProjeOzeti.md` açıldı.

---

## Faz T: Tasarım DNA'sı ve kabuk prototipi

**Amaç:** Bu alana özgü tasarım kimliğini belirlemek ve kullanıcıya demo veriyle çalışan kabuk prototipi üzerinden onaylatmak.

**Tasarım girdisi (frontend-engineer için)**

| Konu | Girdi |
|---|---|
| Alan | Üretim tesislerinde makine temizliğinin adım adım dijital kaydı; kayıt denetimde kanıttır |
| Kitle | Operatör: sahada, ayakta, makine başında, tablet ya da telefonla, çoğu zaman eldivenle; günde birkaç temizlik. Yönetici: masa başında, geniş ekran; tanım, talep ve rapor. Denetçi: seyrek, okuma ve yazdırma. Süper admin: seyrek, masa başı; hesap ve sistem ayarları |
| En sık yapılan iş | Operatörün yürütme ekranında sıradaki adımı PIN'le başlatıp bitirmesi; ikinci sırada makine seçip kayıt açması. Yönetici için aktif kayıtlar ve bekleyen talepler |
| Hava kelimeleri | Net, sakin, güvenilir, sahada okunaklı. Kullanıcı vermedi; alandan türetilen öneri 2026-11-13'te geçerli sayıldı |
| Referans | Yok (kullanıcı vermedi) |
| Kısıtlar | Saha ekranları 360 px'ten başlar, dokunma hedefi 44 px (NFR-04). Hiçbir ekranda zaman giriş alanı yok (NFR-01). Durumlar renk dışında da ayrılır (NFR-05): 7 kayıt, 5 faz, 5 adım, 2 mola, 5 talep durumu (Brief §6). Paylaşılan cihaz: oturumdaki kişinin adı ve "Kullanıcı değiştir" her ekranda (NFR-09). Bağlantı yokken işlemin yapılmadığı açıkça gösterilir (NFR-14). Arayüz Türkçe (NFR-11), latin-ext yazı tipi. Kayıtta mola açıkken yürütme ekranı molada olan kişiyi ve sayaçların durduğunu metinle gösterir; devam eden adım bu sırada da "Devam ediyor"dur (K-39) |
| İmza bileşen adayı | Yürütme ekranındaki faz ve adım şeridi (sıradaki adım, kilitli adımlar, canlı sayaç, mola sırasında duran sayaç, faz minimumuna göre durum). Seçim frontend'indir |

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | BE | `Src/Vaka.Web` projesi: `Vaka.Web.csproj`, `Program.cs` (MVC, küçük harf ve kebab URL, statik dosya, hata ve 403 sayfası route'ları), `Authorization/Roles.cs`: tekil `Operator`, `Manager`, `Auditor`, `SuperAdmin`; birleşik `ManagerOrAuditor`, `ManagerOrSuperAdmin`, `RecordReaders` (Operator, Manager, Auditor), `Any` (dört rol); birleşikler tekil sabitlerden kurulur (PL-15) | backend-engineer |
| 2 | BE | `PrototypeController` (`[AllowAnonymous]`), yalnız Development ortamında yanıt verir, diğer ortamlarda 404 (PL-10) | backend-engineer |
| 3 | FE | `Docs/Design/Dna.md`: Okuma satırı, kadranlar, DNA kartı, kaçınılanlar (`DesignDna.md`) | frontend-engineer |
| 4 | FE | Tailwind CLI kurulumu, token tabanlı açık ve koyu tema, self-host yazı tipleri, `_Layout` kabuğu: rol bazlı menü (Operatör, Yönetici ve Süper admin varyantı), kişi adı, "Kullanıcı değiştir", bağlantı durum göstergesi | frontend-engineer |
| 5 | FE | Prototip sayfaları, Brief kavramlarıyla demo veri (kayıt no biçimi BR-08, durumlar §6): İşlerim (saha), Yürütme ekranı (saha), Yeni kayıt formu (form), Kayıtlar (tablo), Yönetici panosu (özet) | frontend-engineer |
| 6 | FE | Ortak bileşenler: tek rozet biçimi ve durum sözlüğü, PIN giriş bileşeni (`inputmode="numeric"`; iki mod: doğrulama modunda uzunluk sınırı yok, belirleme modunda uzunluk sunucudan gelen `PinLength`, PL-18), boş, yükleniyor ve hata durumları | frontend-engineer |

**Paralel:** Hayır. Gerekçe: frontend'in görünümleri backend'in kurduğu proje ve `Program.cs` olmadan derlenmez; sıra BE 1-2, sonra FE 3-6.

**Kabul kriterleri**

- [ ] TK1: `Docs/Design/Dna.md` ön bloğu ve kartı dolu; kadranlar gerekçeli; yazı tipi üçlüsü Inter değil ve latin-ext ile ğ, ş, İ, ı doğru çizilir.
- [ ] TK2: `/prototype/my-work`, `/prototype/run`, `/prototype/new-record`, `/prototype/records`, `/prototype/dashboard` Development'ta açılır; Production ortam değişkeniyle 404 döner.
- [ ] TK3: qa-tester her prototip sayfanın 1440, 768 ve 390 px görüntüsünü alır; özgünlük kapısı (`DesignDna.md §5`) geçer, anti-slop taraması temiz.
- [ ] TK4: Yürütme ve İşlerim prototipleri 360 px'te yatay kaydırmasız; birincil düğmeler en az 44 px.
- [ ] TK5: Yürütme prototipinde Brief §6'daki kayıt, faz ve adım durumlarının her biri en az bir kez görünür ve metinle ayrılır; aynı rozet biçimi tüm sayfalarda.
- [ ] TK6: Kabukta kişi adı, sicil no yeri ve "Kullanıcı değiştir" 360, 390, 768 ve 1440 px'te çekmece ya da menü açılmadan görünür; "Kullanıcı değiştir" görünür metin etiketi taşır, hedefi en az 44 px ve tek dokunuşla tetiklenir (PL-20); tarayıcı çevrimdışı moduna alınınca bağlantı göstergesi değişir.
- [ ] TK7: Açık ve koyu temada metin kontrastı AA; `prefers-reduced-motion` açıkken hareket kapanır (MOTION > 3 ise).
- [ ] TK8: Kullanıcı DNA kartını ve görüntüleri onaylar; analist onayı `Decisions.md`'ye işler. Onaysız Faz 1 başlamaz.
- [ ] TK9: `Authorization/Roles.cs` dört tekil ve dört birleşik sabiti içerir; birleşik sabitlerde tekil sabit dışında rol metni yoktur; projede `Roles.cs` dışında rol adı metni geçmez.
**Kapsam dışı:** Kimlik doğrulama, veritabanı, gerçek veri.

**Sonuç (2026-11-13):** TESLİM ALINDI (tur 3; tur 1 İADE, tur 3 kullanıcı onaylı ek tur, PL-22). TK8 onayı O-02. Ertelenen DÜŞÜK bulgular
Faz 1, 4 ve 5'in "Faz T'den devreden" satırlarında (PL-21, PL-23). Rapor: `Docs/Delivery/Faz-T-Teslim.md`.

---

## Faz 1: Giriş, PIN, kullanıcılar ve Süper admin

**Amaç:** Kişinin sicil no ve parolayla girdiği, ilk girişte parola ve PIN belirlediği, Süper admin'in yönetici hesaplarını, yöneticinin
Operatör ve Denetçi hesaplarını yönettiği kimlik katmanı; sistem ayarlarının veritabanından okunması.

**Ayar başlangıç değerleri (PL-24, Requirements §6):** `<OturumSüresi>` 30 dakika, `<PinUzunluğu>` 4 hane, `<PinDenemeSınırı>` 5.
Kod ve sözleşme yer tutucu adıyla yazılır; değer yalnız seed'de ve ayar tablosundadır.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Kullanıcı ve kullanıcı rolleri tabloları (Brief §7.g `User`); sicil no benzersiz, oluşturulduktan sonra değişmez (PL-27); PIN hatalı deneme sayacı, PIN kilit anı, parola değiştirme ve PIN belirleme bayrakları bu fazda. PIN yalnız özet olarak saklanır; özet metin üzerinden alınır, baştaki sıfır anlamlıdır. Seed: tek Süper admin (PL-05), geçici parola özetli, geliştirme parolası yalnız README'de; başka kullanıcı seed'lenmez | backend-engineer talebi, dbadmin |
| 2 | DB | Sistem ayarları tablosu, ayar başına bir satır (anahtar, tam sayı değer, birim; değer pozitif, veritabanı kısıtıyla) ve ayar değişiklik geçmişi tablosu (eski, yeni, yapan, zaman) (K-47, PL-17, PL-25). Seed yalnız üç satır yazar: `<OturumSüresi>` 30, `<PinUzunluğu>` 4, `<PinDenemeSınırı>` 5 (PL-24). Medya boyutu ve azami kilit süresi satırı yazılmaz, boş ya da yer tutucu değerli satır yoktur; bu satırlar Faz 3'te seed ekleme olarak gelir | backend-engineer talebi, dbadmin |
| 3 | BE | Ayar servisi: Faz 1'in üç değerini veritabanından okur; bu fazda yazma yok (ekran Faz 9). Oturum süresi ve PIN değerleri her istekte servisten okunur; satırı olmayan ayar istenirse varsayılan değer uydurulmaz, açık hata verilir | backend-engineer |
| 4 | BE | Cookie kimlik doğrulama, sicil no + parola. Oturum son kullanıcı isteğinden `<OturumSüresi>` sonra kapanır; çerezi sürenin yarısı geçince yenileyen hazır kayan süre bu kuralı karşılamaz, son istek anı ölçülür. Claim'ler: ad (`ClaimTypes.Name`), roller (`ClaimTypes.Role`), `VakaClaimTypes.UserId`, `VakaClaimTypes.RegistryNo` | backend-engineer |
| 5 | BE | İlk giriş kapısı: parola değiştirilmemiş ya da PIN'i olmayan kullanıcı `/account/first-login`, `/account/logout`, `/account/access-denied`, `/error/*` ve statik dosya dışındaki her istekte ilk girişe yönlenir (BR-55). Yetki politikası değil, istek filtresi | backend-engineer |
| 6 | BE | PIN servisi: biçim kuralı yalnız yeni PIN belirlenirken (BR-49: yalnız rakam, `<PinUzunluğu>` hane, tüm haneleri aynı ya da ardışık artan/azalan dizi reddedilir), doğrulama uzunluktan bağımsız; art arda `<PinDenemeSınırı>` hatalı denemede kilit ve oturum kapatma, doğru PIN sayacı sıfırlar; PIN düz metin günlüklenmez (NFR-15) | backend-engineer |
| 7 | BE | Kullanıcı servisi: ekleme, düzenleme, rol atama, geçici parola (işlemi yapan girer, BR-55), pasifleştirme ve yeniden etkinleştirme, PIN ve parola sıfırlama; son dördü işlemi yapanın PIN'ini ister (Requirements §3, PL-06). PIN sıfırlama PIN'i, sayacı ve kilidi temizler, PIN belirleme bayrağını açar; parola sıfırlama yeni geçici parola yazar, parola değiştirme bayrağını açar. Kapsam kuralı SK-9: Yönetici yalnız `Manager` ve `SuperAdmin` rolü taşımayan hesapları yönetir ve yalnız `Operator`, `Auditor` atar; Süper admin tümünü; kimse kendini pasifleştiremez ve kendi rollerini değiştiremez; son aktif Süper admin pasifleştirilemez ve rolü kaldırılamaz (PL-05, PL-16). Kural serviste, işlemi yapanın ve hedefin rolleri veritabanından okunarak uygulanır; policy yok. Açık iş listesi arayüzü (`OpenWork`), bu fazda boş döner, Faz 4, 5, 7'de genişler; `Url` yalnız görüntüleyenin erişebildiği ekrana verilir | backend-engineer |
| 8 | BE | Ana sayfa yönlendirmesi: yalnız `SuperAdmin` rolü olan kullanıcı `/` adresinde `/users`'a yönlenir; diğer roller Faz 4 ve 6'ya kadar `HomeViewModel` görür | backend-engineer |
| 9 | BE | İlk iş, frontend bunlara karşı derler: sözleşmedeki ViewModel sınıfları; `Authorization/VakaClaimTypes.cs` (`UserId`, `RegistryNo`); `Roles.Label(string role)`: `Operator` "Operatör", `Manager` "Yönetici", `Auditor` "Denetçi", `SuperAdmin` "Süper admin". Görünümde rolün Türkçe adı yalnız bu metottan gelir | backend-engineer |
| 10 | BE | T-B15: genel antiforgery filtresi; `PrototypeController` action'ları `[HttpGet]` | backend-engineer |
| 11 | FE | E-01 Giriş, E-02 Parola ve PIN (iki mod), E-03 Kullanıcılar, E-04 Kullanıcı formu (sıfırlama ve etkinleştirme bölümleriyle) ve pasifleştirme onayı, 403 sayfası; kabukta gerçek kişi adı, sicil no ve rol bazlı menü (Süper admin menüsünde yalnız Kullanıcılar ve hesap). Menü yalnız var olan ve kullanıcının erişebildiği adreslere bağlantı verir; sonraki fazların kalemleri o fazda eklenir, bağlantısız kalem gösterilmez. "Kullanıcı değiştir" antiforgery token'lı `POST /account/logout` formudur | frontend-engineer |
| 12 | FE | T-B16 oturumsuz hata ve 403 sayfaları gerçek kabukla; T-B21 `Dna.md` veri dili satırı ile E-03 tablosunun eşlenmesi | frontend-engineer |

**Paralel:** Evet. Gerekçe: şema Brief §7.g'de belli (K-40 kararıyla kullanıcıya yönetici alanı eklenmez; roller ayrı tabloda); route, ViewModel,
alt tip, form alanı adı ve rol aşağıda; dosyalar ayrık. Frontend veritabanına bağlı değildir; derlemesi BE 9'a bağlıdır, BE 9 backend'in ilk işidir.

**Sözleşme**

ViewModel ad alanları `Vaka.Web.ViewModels.Account`, `Vaka.Web.ViewModels.Home`, `Vaka.Web.ViewModels.Users`; klasörler `ViewModels/Account/`,
`ViewModels/Home/`, `ViewModels/Users/`. `?` boş olabilir demektir. Rol alanları `Roles` sabitlerinin değerini taşır; görünüm rol adını `Roles.Label` ile yazar.

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET, POST `/account/login` | `AccountController.Login` | anonim | `LoginViewModel { RegistryNo, Password, ReturnUrl?, ErrorMessage?, StatusMessage? }`. `StatusMessage` oturumun neden kapandığını söyler (süre doldu, PIN kilitlendi, çıkış yapıldı) | `Views/Account/Login.cshtml` |
| POST `/account/logout` | `AccountController.Logout` | anonim (oturumu düşmüş kişi de basabilir, K11a) | - ; `/account/login`'e yönlenir | - |
| GET `/account/access-denied` | `AccountController.AccessDenied` | anonim | - ; yanıt durum kodu 403 | `Views/Account/AccessDenied.cshtml` |
| GET, POST `/account/first-login` | `AccountController.FirstLogin` | Any | `FirstLoginViewModel { FullName, MustChangePassword, MustSetPin, CurrentPassword, NewPassword, ConfirmPassword, NewPin, ConfirmPin, PinLength }`; bitince `/`'a yönlenir | `Views/Account/FirstLogin.cshtml` |
| GET `/account/security`; POST `/account/change-password`, `/account/change-pin` | `AccountController.Security`, `.ChangePassword`, `.ChangePin` | Any | `SecurityViewModel { PinLength, StatusMessage?, Password: ChangePasswordViewModel { CurrentPassword, NewPassword, ConfirmPassword }, Pin: ChangePinViewModel { CurrentPin, NewPin, ConfirmPin } }`. Form alanı adları önekli (`Password.CurrentPassword`, `Pin.NewPin`); POST'lar `[Bind(Prefix = "Password")]` ve `[Bind(Prefix = "Pin")]` ile bağlar | `Views/Account/Security.cshtml` |
| GET `/` | `HomeController.Index` | Any | `HomeViewModel { FullName, Roles[] }` (Faz 4 ve 6'da role göre yönlendirmeye döner) | `Views/Home/Index.cshtml` |
| GET `/users?query=&status=&page=` | `UsersController.Index` | ManagerOrSuperAdmin | `UserListViewModel { Items: UserListItemViewModel[] { Id, RegistryNo, FullName, Roles[], IsActive, IsPinLocked, MustChangePassword, MustSetPin, CanManage }, Query?, Status: UserStatusFilter (Active, Inactive, All; varsayılan Active), Page, PageSize, Total, StatusMessage? }` | `Views/Users/Index.cshtml` |
| GET, POST `/users/create`, `/users/edit/{id}` | `UsersController.Create`, `.Edit` | ManagerOrSuperAdmin | `UserFormViewModel { Id?, RegistryNo (Edit'te salt okunur, PL-27), FullName, SelectedRoles[], AvailableRoles[] (işlemi yapanın atayabildiği roller), RolesLocked (kendi hesabı), TemporaryPassword (yalnız Create), IsActive, IsPinLocked, MustChangePassword, MustSetPin, CanResetPin, CanResetPassword, CanDeactivate, CanActivate, StatusMessage?, ResetPin: UserResetViewModel, ResetPassword: UserResetViewModel, Activate: UserResetViewModel }` | `Views/Users/Form.cshtml` |
| GET, POST `/users/deactivate/{id}` | `UsersController.Deactivate` | ManagerOrSuperAdmin | `UserDeactivateViewModel { UserId, FullName, RegistryNo, OpenWork: OpenWorkItemViewModel[] { Kind, Title, Url? }, CanDeactivate, BlockReason?, Pin }` | `Views/Users/Deactivate.cshtml` |
| POST `/users/activate/{id}`, `/users/reset-pin/{id}`, `/users/reset-password/{id}` | `UsersController.Activate`, `.ResetPin`, `.ResetPassword` | ManagerOrSuperAdmin | `UserResetViewModel { TemporaryPassword? (yalnız parola sıfırlamada), Pin }`; hedef `{id}`'den. Form alanı adları önekli: `Activate.Pin`, `ResetPin.Pin`, `ResetPassword.TemporaryPassword`, `ResetPassword.Pin` | `Views/Users/Form.cshtml` içinde |

Ortak davranış:

- Alan hatası `ModelState`'te alan adıyla (önekliyse önekiyle) gelir; alana bağlı olmayan iş kuralı reddi (ör. yanlış PIN, açık iş) boş anahtarla gelir ve form özetinde gösterilir.
- Başarılı POST aynı ekranın GET'ine yönlenir; sonuç metni `StatusMessage` alanında gelir. Başarısız POST aynı görünümü aynı ViewModel'le yeniden çizer;
  `Activate`, `ResetPin`, `ResetPassword` başarısızlığında `Form` görünümü tam `UserFormViewModel` ile çizilir.
- Tüm `/users` istekleri SK-9 kapsamına göre serviste kontrol edilir; kapsam dışı hedefte istek işlem yapmaz ve 403 döner. Rol yetkisizliğinde de
  kapsam reddinde de son yanıt 403 durum kodlu `/account/access-denied` içeriğidir (`/error/403` ile aynı bileşen).
- PIN'in kilitlendiği istekte oturum kapanır ve `/account/login`'e `StatusMessage` ile yönlenir.
- İstemci arka planda sunucuya istek atmaz; bağlantı göstergesi `navigator.onLine` ile çalışır. İşlemsizlik süresini yalnız kullanıcının isteği yeniler.
- PIN bileşeni: `FirstLogin` ve `Security` yeni PIN alanları belirleme modunda `PinLength` ile; diğer tüm PIN alanları doğrulama modunda, uzunluk sınırı olmadan (PL-18).

**Dosya sınırı:** frontend `Views/**` (`Account`, `Users`, `Home`, `Shared`, `Error`, `Prototype`, `_ViewImports.cshtml`, `_ViewStart.cshtml`), `Styles/**`,
`wwwroot/**`, `tailwind.config.js`, `Docs/Design/Dna.md`; backend `Controllers/AccountController.cs`, `Controllers/HomeController.cs`,
`Controllers/UsersController.cs`, `Controllers/PrototypeController.cs` (T-B15), `Controllers/ErrorController.cs` (yalnız gerekirse), `Services/` (kullanıcı,
PIN, parola, ayar okuma), `ViewModels/Account/**`, `ViewModels/Home/**`, `ViewModels/Users/**`, `Authorization/**` (`Roles.cs`, `VakaClaimTypes.cs`,
ilk giriş filtresi), `Program.cs`, `appsettings*.json`, `Docs/Db/DB-<yıl>-<sıra>.md` talebi; dbadmin `Domain/Entities/**`, `Data/**`.

**Kabul kriterleri**

- [ ] K1: Oturumsuz istek `/users` için `/account/login?ReturnUrl=%2Fusers` adresine 302 döner.
- [ ] K2: Seed'de yalnız bir kullanıcı vardır ve rolü Süper admin'dir. Süper admin geçici parolayla girince `/account/first-login` açılır; parola ve PIN belirlenmeden `/`, `/users` ve `/account/security` istekleri ilk girişe yönlenir; bitince `/` adresi `/users`'a yönlenir (SC-34, SC-38).
- [ ] K3: İlk giriş ve PIN değiştirmede, seed değeri `<PinUzunluğu>` 4 ile: 3 ve 5 haneli PIN, rakam dışı karakterli PIN (`12a4`), tüm haneleri aynı PIN (`1111`), ardışık artan (`1234`) ve azalan (`4321`) PIN alan altında nedeniyle reddedilir; `1357` ve `0482` kabul edilir, `0482` sonraki doğrulamada aynı değerle geçer.
- [ ] K4: Süper admin `/users/create` ile Yönetici rolünde kullanıcı açar; o Yönetici `/users/create` ile Operatör ve Denetçi açar; aynı sicil no ile ikinci kayıt "Bu sicil no kayıtlı" hatasıyla reddedilir; yeni kullanıcılar ilk girişte parola ve PIN belirler. `/users/edit/{id}` formunda sicil no değiştirilemez; istek değiştirilerek gönderilen sicil no kaydı değiştirmez (PL-27).
- [ ] K5: Operatör ve Denetçi rolündeki kullanıcı `/users`, `/users/create`, `/users/edit/{id}`, `/users/deactivate/{id}`, `/users/activate/{id}`, `/users/reset-pin/{id}`, `/users/reset-password/{id}` GET ve POST isteklerinde son yanıt olarak 403 durum kodlu sayfa alır; menüde Kullanıcılar yoktur (SC-17).
- [ ] K6: Yönetici ve Süper admin pasifleştirmede ve yeniden etkinleştirmede yanlış PIN girerse hesabın durumu değişmez; doğru PIN'le değişir; pasif kullanıcı doğru parolayla giremez.
- [ ] K7: Seed değeri `<PinDenemeSınırı>` 5 ile: art arda 4 hatalı PIN'den sonra PIN kilitli değildir; 5. hatalı denemede PIN kilitlenir, oturum kapanır ve giriş sayfası nedeni gösterir. 4 hatalı denemeden sonra girilen doğru PIN sayacı sıfırlar. Yeniden girişte PIN isteyen işlem "PIN kilitli" ile reddedilir. Kilitli Operatör'ün PIN'ini Yönetici, kilitli Yönetici'nin PIN'ini Süper admin sıfırlar; kullanıcı sonraki girişte ilk giriş ekranında yalnız yeni PIN belirler (SC-31 kimlik kısmı).
- [ ] K8: Son aktif Süper admin'in pasifleştirme ve Süper admin rolünü kaldırma isteği nedeniyle reddedilir; ikinci bir Süper admin açıldıktan sonra biri pasifleştirilebilir. Kullanıcı kendi hesabını pasifleştiremez ve kendi rollerini değiştiremez (PL-05, PL-16).
- [ ] K8a: Yönetici'nin formunda Yönetici ve Süper admin rolleri sunulmaz; istek değiştirilerek gönderilen `Manager` ya da `SuperAdmin` rolü reddedilir. Yönetici, `Manager` ya da `SuperAdmin` rolü taşıyan bir hesap için düzenleme, pasifleştirme, etkinleştirme, PIN ve parola sıfırlama isteği gönderince 403 alır ve hesap değişmez; listede bu hesaplarda işlem bağlantısı yoktur (SK-9).
- [ ] K8b: Süper admin rolü tek başına olan kullanıcının menüsünde yalnız Kullanıcılar ve hesap bağlantıları vardır. Hiçbir rolün menüsünde bu fazda var olmayan adrese bağlantı ya da bağlantısız kalem yoktur.
- [ ] K9: Kullanıcı `/account/security`'de eski PIN'le yeni PIN belirler; eski PIN yanlışsa PIN değişmez ve hatalı deneme sayılır (K-42).
- [ ] K10: Veritabanında parola ve PIN düz metin olarak bulunmaz; uygulama günlüğünde girilen PIN geçmez (NFR-15).
- [ ] K11: Kabukta oturumdaki kişinin adı ve sicil no 360, 768 ve 1440 px'te görünür; "Kullanıcı değiştir" tek dokunuşla oturumu kapatıp giriş ekranını açar (NFR-09).
- [ ] K11a: Oturum süresi dolduktan sonra sayfada kalan kişi "Kullanıcı değiştir"e basınca 400 değil giriş ekranını görür.
- [ ] K12: Seed'li veritabanında oturum son istekten 30 dakika sonra kapanır (`<OturumSüresi>`, PL-24). QA kopyasında ayar kısa bir değere (ör. 2 dakika) ayarlanır: süreden kısa aralıklarla yapılan istekler, toplamı süreyi aşsa da oturumu açık tutar; süreden uzun işlemsizlikten sonraki istek giriş sayfasına yönlenir. `<PinUzunluğu>` ve `<PinDenemeSınırı>` da ayar tablosundan okunur; QA kopyasında değiştirilen değer sonraki istekte uygulanır; kaynak kodda, `appsettings*.json`'da ve görünümde bu değerler yoktur (K-47, PL-18).
- [ ] K12a: Ayar tablosunda tam üç satır vardır, değerleri `<OturumSüresi>` 30, `<PinUzunluğu>` 4, `<PinDenemeSınırı>` 5'tir (PL-24); medya boyutu ve azami kilit süresi satırı yoktur (PL-25); değişiklik geçmişi tablosu boştur. QA kopyasında sıfır ya da negatif değer yazma veritabanı kısıtıyla reddedilir.
- [ ] K13: Operatör ve Yönetici rollerine birlikte sahip kullanıcı iki rolün menüsünü birlikte görür ve `/users`'a erişir; yalnız Operatör rolü olan kullanıcı Kullanıcılar'ı görmez (K-29).
- [ ] K14: Prototip kabuğu gerçek kabukla değişmiştir; prototip sayfaları Development'ta açılmaya devam eder (PL-10).

**Faz T'den devreden (PL-21, PL-23):**

- [ ] T-B15 (BE): Genel antiforgery filtresi etkin; G5 bu fazın tüm POST'larında 400 verir; prototip action'ları `[HttpGet]` ya da token doğrulayan biçimdedir.
- [ ] T-B16 (FE): Production'da oturumsuz açılan `/error/404`, `/error/403`, `/error` ve `/account/access-denied` sayfalarında demo kişi adı, sicil no ve `/prototype/*` bağlantısı yoktur (K14 ile).
- [ ] T-B21 (FE): `Docs/Design/Dna.md` veri dili satırındaki kart sütun sayısı (768 px altı, 768-1279 px) ve tablo satır yüksekliği uygulamayla aynıdır ya da uygulama karttakine uyar; E-03 tablosu bu satıra göre kurulur.

**Kapsam dışı:** Kullanıcıya tesis ya da yönetici bağı (K-20, K-40), kurumsal dizin, parola karmaşıklık politikası iş biriminden gelmedikçe ASP.NET varsayılanı,
Sistem ayarları ekranı (Faz 9), Süper admin hesabının uygulama dışından kurtarılması, sicil no değiştirme (PL-27).

**Sonuç (2026-11-14):** TESLİM ALINDI (tur 3; tur 1 İADE, tur 2'de B7 kaldı, tur 3 kullanıcı onaylı ek tur, PL-31). Kabul kriterleri 28/28;
T-B15, T-B16, T-B21 kapandı. Faz 4'e devreden iki kalem Faz 4'ün "Faz 1'den devreden" satırlarında (PL-32). Rapor: `Docs/Delivery/Faz-1-Teslim.md`.

---

## Faz 2: Tesis yapısı, vardiya ve malzeme

**Amaç:** Kaydın seçimlerini besleyen tesis, hat, makine, vardiya, malzeme ve lot tanımlarını yöneticinin girmesi.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Tesis, hat, makine, vardiya, malzeme, lot tabloları; BR-54 benzersizlikleri; lot no malzeme içinde benzersiz; makinede kullanımdan kaldırılma anı. Seed: nötr demo tesis yapısı, vardiya, malzeme ve lotlar (V-13), en az bir SKT'si geçmiş lot | backend-engineer talebi, dbadmin |
| 2 | BE | Tanım servisleri: kod doğrulaması (tire yok, benzersizlik), kod ve makinenin hattı oluşturulduktan sonra değişmez (PL-04); pasif tesis ve hattın altındakiler yeni kayıtta seçilemez (BR-05) | backend-engineer |
| 3 | BE | Vardiya kuralları (K-38 kararı): aynı tesiste aktif vardiyalar çakışmaz; bitiş başlangıçtan küçükse ertesi gün biter; aralık başlangıç dahil bitiş hariçtir (bir vardiyanın bitişi sonrakinin başlangıcına eşit olabilir); pasif vardiya çakışma kontrolüne ve hesaplara girmez; saatler Türkiye saatidir. Bir anın vardiyasını bulan tek metot (`IDurationCalculator.FindShiftAt`, saf hesap) bu fazda yazılır, Faz 4 SH-13 ve Faz 7 SH-16 onu kullanır (PL-46); vardiyaları veritabanından okuyup bu metodu çağıran servis Faz 4'tedir (PL-40) | backend-engineer |
| 4 | BE | Makine kullanımdan kaldırma; engelleyen kayıt kontrolü arayüzü bu fazda boş döner, Faz 4 ve 7'de dolar (BR-35) | backend-engineer |
| 5 | BE | SKT geçmiş kararı tek metotta: SKT < Türkiye saatine göre bugünün tarihi (BR-12) | backend-engineer |
| 6 | FE | E-05 Tesisler ve hatlar, E-06 Vardiyalar, E-07 Makineler (tesis ve hat filtreli, bağlı seçim), E-08 Malzemeler ve lotlar ("SKT geçmiş" rozeti) | frontend-engineer |

**Paralel:** Evet. Gerekçe: şema Brief §7.g'de belli, frontend ViewModel'e bağlı, dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/facilities` | `FacilitiesController.Index` | Manager | `FacilityListViewModel { Items[] { Id, Code, Name, IsActive, LineCount, ShiftCount }, ShowInactive, StatusMessage? }` | `Views/Facilities/Index.cshtml` |
| GET, POST `/facilities/create`, `/facilities/edit/{id}` | `FacilitiesController.Create`, `.Edit` | Manager | `FacilityFormViewModel { Id?, Code, Name, IsActive, CodeLocked }` | `Views/Facilities/Form.cshtml` |
| GET `/facilities/details/{id}` | `FacilitiesController.Details` | Manager | `FacilityDetailViewModel { Id, Code, Name, IsActive, Lines[] { Id, Code, Name, IsActive, MachineCount }, StatusMessage? }` | `Views/Facilities/Details.cshtml` |
| GET, POST `/lines/create?facilityId=`, `/lines/edit/{id}` | `LinesController.Create`, `.Edit` | Manager | `LineFormViewModel { Id?, FacilityId, FacilityName, Code, Name, IsActive, CodeLocked }` | `Views/Lines/Form.cshtml` |
| GET `/shifts?facilityId=` | `ShiftsController.Index` | Manager | `ShiftListViewModel { FacilityId, Facilities[]: FacilityOptionViewModel { Id, Name } (pasif dahil tüm tesisler), Items[] { Id, Name, StartTime: TimeOnly, EndTime: TimeOnly, EndsNextDay, IsActive }, StatusMessage? }` | `Views/Shifts/Index.cshtml` |
| GET, POST `/shifts/create?facilityId=`, `/shifts/edit/{id}` | `ShiftsController.Create`, `.Edit` | Manager | `ShiftFormViewModel { Id?, FacilityId, FacilityName, Name, StartTime: TimeOnly?, EndTime: TimeOnly?, IsActive }` | `Views/Shifts/Form.cshtml` |
| GET `/machines` | `MachinesController.Index` | Manager | `MachineListViewModel { Items[] { Id, Code, Name, FacilityName, LineName, IsActive, RetiredAtLocal?: DateTime }, FacilityId?, LineId?, Query, ShowRetired, Facilities[]: SelectOptionViewModel, Lines[]: SelectOptionViewModel, Page, PageSize, Total, StatusMessage? }` | `Views/Machines/Index.cshtml` |
| GET, POST `/machines/create`, `/machines/edit/{id}` | `MachinesController.Create`, `.Edit` | Manager | `MachineFormViewModel { Id?, FacilityId, LineId, Code, Name, CodeLocked, RetiredAtLocal?: DateTime, Facilities[]: SelectOptionViewModel, Lines[]: SelectOptionViewModel }` | `Views/Machines/Form.cshtml` |
| GET `/machines/lines?facilityId=` | `MachinesController.Lines` | Manager | JSON `[{ id, label }]` | - |
| GET, POST `/machines/retire/{id}` | `MachinesController.Retire` | Manager | `MachineRetireViewModel { MachineId, Code, Name, RetiredAtLocal?: DateTime, BlockingRecords[] { RecordNo, StatusLabel, Url }, CanRetire }` | `Views/Machines/Retire.cshtml` |
| GET `/materials` | `MaterialsController.Index` | Manager | `MaterialListViewModel { Items[] { Id, Code, Name, IsActive, LotCount, ExpiredLotCount }, Query, ShowInactive, Page, PageSize, Total, StatusMessage? }` | `Views/Materials/Index.cshtml` |
| GET, POST `/materials/create`, `/materials/edit/{id}` | `MaterialsController.Create`, `.Edit` | Manager | `MaterialFormViewModel { Id?, Code, Name, IsActive }` | `Views/Materials/Form.cshtml` |
| GET `/materials/details/{id}` | `MaterialsController.Details` | Manager | `MaterialDetailViewModel { Id, Code, Name, IsActive, Lots[] { Id, LotNo, ExpiryDate: DateOnly, IsExpired }, StatusMessage? }` | `Views/Materials/Details.cshtml` |
| GET, POST `/material-lots/create?materialId=`, `/material-lots/edit/{id}` | `MaterialLotsController.Create`, `.Edit` | Manager | `MaterialLotFormViewModel { Id?, MaterialId, MaterialName, LotNo, ExpiryDate: DateOnly? }` | `Views/MaterialLots/Form.cshtml` |

Ortak davranış (PL-36..PL-44):

- Seçenek tipleri: `SelectOptionViewModel { Id, Label }`, etiket "Kod - Ad" (makine ekranları ve `/machines/lines` JSON'u); `FacilityOptionViewModel { Id, Name }` (vardiya ekranı).
  `/machines` süzgecinde tesis seçilmemişken hat etiketi birleşik kodla `<TesisKodu>-<HatKodu> - <HatAdı>` (PL-44).
- Form saat ve tarih alanları boş olabilir tiptedir; çözülemeyen ya da boş değer alan altında Türkçe hatayla döner. Bunlar tanım değeridir
  (vardiya saati, SKT), olay zamanı değildir; NFR-01 istemciden olay zamanı almama kuralı değişmez.
- Başarılı POST yönlendirmesi: tesis ve malzeme ekleme Details'e; hat ekleme ve düzenleme tesis Details'e; vardiya `/shifts?facilityId=`'ye;
  makine `/machines`'e; lot malzeme Details'e. Sonuç metni hedef ekranın `StatusMessage` alanında gelir.
- PL-04 düzenleme: gönderilmeyen, boş ya da 0 kod ve hat değeri "değişmedi" sayılır; dolu ve kayıtlıdan farklı değer `Code` (makinede
  `LineId`) alan hatasıyla reddedilir, kayıt değişmez. Makine düzenlemede tesis hattan türer; gönderilen `FacilityId` dolu ve farklıysa `LineId` hatasıyla reddedilir.
- K3 JS kapalı: makine ekleme formunda tesis seçimini yenileyen düğme bulunur; aynı POST formunda `name="refresh" value="true"` gönderir,
  sunucu kaydetmez, alan hatası üretmez, formu gönderilen tesisin aktif hatlarıyla, kod ve ad korunarak çizer (PL-44). `GET /machines/create?facilityId=`
  doğrudan adres olarak kalır. JS açıkken bu düğme gizlenir. POST'ta hat gönderilen tesise ait değilse ya da tesis değişmiş ve hat eskiyse `LineId` alan hatası döner ve form gönderilen
  tesisin aktif hatlarıyla yeniden çizilir. Makine listesi filtresi JS kapalıyken "Uygula" ile aynı sonucu verir.
- Makine kullanımdan kaldırma PIN istemez (BR-48 "PIN istenmeyenler", Requirements §2 matrisi); POST gövdesi yalnız antiforgery token'dır.
- Kaldırılmış makine salt okunurdur (PL-43): düzenleme POST'u nedenli reddedilir; `/machines/edit/{id}` alanları kilitli, kaydet ve kaldır
  bağlantısı olmadan, kaldırılma anıyla açılır; kaldırma ekranı "zaten kaldırıldı" bilgisini gösterir; listede eylem "Görüntüle".
- Sayfalama: `/machines` ve `/materials` sunucuda 20'lik sayfalanır. `/facilities` ve `/shifts` sayfalanmaz; filtreleri (`showInactive`,
  `facilityId`) sunucuda çalışır (PL-38).
- Rota adı kebab dönüştürücüsünden gelir (`Program.cs` `RouteTokenTransformerConvention`): `MaterialLotsController` `[controller]/[action]`
  ile `/material-lots/...` üretir; ayrı sabit rota gerekmez.

**Dosya sınırı:** frontend `Views/Facilities/**`, `Views/Lines/**`, `Views/Shifts/**`, `Views/Machines/**`, `Views/Materials/**`, `Views/MaterialLots/**`,
`wwwroot/js/CascadeSelect.js`, menü; backend ilgili controller'lar, `Services/` tanım servisleri, `ViewModels/Definitions/**`.

**Kabul kriterleri**

- [ ] K1: Yönetici tesis, hat ve makine ekler; aynı tesis kodu, aynı tesiste aynı hat kodu, aynı hatta aynı makine kodu ve tire içeren kod alan altında nedeniyle reddedilir (BR-54).
- [ ] K2: Kaydedilmiş tesis, hat ve makinenin kod alanı düzenleme formunda değiştirilemez; istek değiştirilerek gönderilen yeni kod ya da makine için yeni hat reddedilir (PL-04).
- [ ] K3: Makine formunda hat listesi seçilen tesise göre gelir; JS kapalıyken tesis yenileme düğmesi aynı hat listesini getirir ve başka tesisin
  hattıyla gönderilen form `LineId` alan hatasıyla, gönderilen tesisin hatlarıyla yeniden çizilir; makine kaydedilmez.
- [ ] K4: Aynı tesiste çakışan iki aktif vardiya reddedilir; bir vardiyanın bitişiyle sonrakinin başlangıcı aynı saat olabilir; bitişi başlangıçtan küçük vardiya "ertesi gün biter" olarak listelenir; pasif vardiya çakışma kontrolüne girmez (K-38).
- [ ] K5: Yönetici makineyi kullanımdan kaldırır; liste "Kullanımdan kaldırıldı" ve anını gösterir; makine silinmez (FR-02, BR-05).
- [ ] K6: SKT'si bugünden önce olan lot listede metinli "SKT geçmiş" rozetiyle görünür; bugünkü SKT geçmiş sayılmaz.
- [ ] K7: Operatör ve Denetçi bu fazın tüm adreslerinde 403 alır (BR-34).
- [ ] K8: `/machines` ve `/materials` sayfalaması ve filtreleri, `/facilities` aktiflik filtresi ve `/shifts` tesis seçimi sunucuda çalışır
  (sorgu dizesiyle açılan adres aynı sonucu verir). E-05, E-06, E-07 ve E-08 liste ekranları 360, 768, 1024 ve 1440 px'te G11 imzasını taşır
  ve frontend devir notundaki beklenen imzayla aynıdır; `/facilities/details/{id}` hat tablosu ve `/materials/details/{id}` lot tablosu
  PL-39 istisnasıyla ölçülür; 360 ve 390 px'te yatay kaydırma yok; 1440 px'te tablo.
- [ ] K9: `IDurationCalculator`'ın Faz 2 metotları (ertesi gün bitiş, vardiya çakışması, anın vardiyası, SKT geçmiş kararı) XML yorumunda
  formül ve kural kimliğini taşır; vardiya çakışması ve SKT kararı yalnız bu metotlardan çağrılır, controller ve görünümde saat ya da tarih karşılaştırması yoktur.

**Kapsam dışı:** Toplu içe aktarım (K-19), kullanıcıya tesis ataması (K-20).

**Sonuç (2026-11-14):** TESLİM ALINDI (tur 2; tur 1 İADE, PL-42..PL-45). Kabul kriterleri 18/18; B1-B10 kapandı, açık bulgu 0.
B1 Faz 1 kodundaydı (`ErrorController`), Faz 1 POST'ları tur 2'de yeniden denetlendi. Faz 4'e devreden: A1 makine kaldırma engel listesi,
A2 `FindShiftAt` çalışma zamanı denemesi (qa-tester). Rapor: `Docs/Delivery/Faz-2-Teslim.md`.

---

## Faz 3: Temizlik tipi ve prosedür versiyonu

**Amaç:** Yöneticinin temizlik tiplerini tanımlayıp makinelerle eşlemesi, faz ve adımlardan oluşan prosedürü taslakta hazırlayıp versiyon olarak yayınlaması.

**Ayar değerleri (PL-34, PL-47, PL-51):** `<AzamiKilitSüresi>` 120 dakika; `<AzamiMedyaBoyutu>` fotoğraf 10 MB, video 100 MB. 1 MB = 1 048 576 bayt;
sınıra eşit dosya kabul edilir. Değerler yalnız seed'de ve ayar tablosundadır; kod, `appsettings*.json` ve görünümde yoktur.

**Başlama şartı:** Faz 2 TESLİM ALINDI ve faz commit'i atıldı (CLAUDE.md kural 7). Neden: Faz 3 migration'ı Faz 2 migration'ının üstüne kurulur,
makine eşlemesi Faz 2 tablolarına bağlıdır. Kullanıcıdan beklenen değer ya da karar kalmadı.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Ayar tablosuna üç satır seed ekleme (`HasData`, migration'da yalnız `InsertData`): `MaxLockMinutes` 120 `dakika`, `MaxPhotoMb` 10 `MB`, `MaxVideoMb` 100 `MB`. Faz 1'in üç satırı ve değişiklik geçmişi değişmez, migration'da `UPDATE` yok (PL-25). Anahtar sabitleri `Services/SettingKeys.cs`'tedir (backend, Faz 1 düzeni) | backend-engineer talebi, dbadmin |
| 1a | DB | Temizlik tipi (tür: Planlı, Acil; varsayılan kilit süresi yalnız planlıda), makine eşlemesi, prosedür versiyonu, faz, adım, adım medyası tabloları (Brief §7.g). Unique: tip kodu (büyük-küçük harf duyarsız), (tip, versiyon no). Kısmi unique index: tip başına tek Yürürlükte ve tek Taslak. Faz minimumu saniye olarak saklanır. Medya satırında en az tür, içerik özeti (SHA-256), özgün dosya adı, içerik tipi ve bayt boyutu. Yayınlayan kişi boş olabilir (kurulum verisi, PL-51). Tetikleyiciler (BR-04): Yürürlükte ya da Arşiv versiyonun faz, adım ve medya satırlarında UPDATE ve DELETE ret; versiyon satırında Taslak dışı durumda yalnız Yürürlükte'den Arşiv'e durum geçişine izin, diğer kolonların değişmesi ve DELETE ret | backend-engineer talebi, dbadmin |
| 1b | DB | Demo veri `Data/Seed/DemoData.sql`'e ek (Faz 2 düzeni: `INSERT OR IGNORE`, sabit Id, migration'da `HasData` yok): bir planlı tip (varsayılan kilit süresi 120 dakikayı aşmaz, aktif demo makinelerinin tümüne eşli) ve bir acil tip; her birinin Yürürlükte birer versiyonu; planlıda her fazın minimumu dolu ve en az bir fazda birden çok adım; acilde minimum yok; yayınlayan boş; medya yok. Nötr demo (V-13); dokümandaki örnek yapı Ö-01 ile Salı sonrası (PL-49) | backend-engineer talebi, dbadmin |
| 2 | BE | Tip servisi: kod zorunlu ve benzersiz; kod kayıt numarasına girmez, düzenlemede değişebilir (PL-51). Tür oluşturulduktan sonra değişmez (PL-04). Varsayılan kilit süresi yalnız planlı tipte zorunlu, pozitif tam sayı, `<AzamiKilitSüresi>`'ni aşamaz (BR-45; sınır her istekte ayar servisinden); acil tipte dolu gelen değer `DefaultLockMinutes` alan hatasıyla reddedilir. Acil tip makine eşlemesi almaz (BR-50). Eşlemede yalnız aktif, kullanımdan kaldırılmamış, aktif hat ve tesisteki makine yeni seçilir; seçilemez hale gelmiş makinenin mevcut eşlemesi korunur | backend-engineer |
| 3 | BE | Versiyon servisi: taslak açma, yürürlükte versiyon varsa onun derin kopyası (faz, adım, medya bağlantısı, malzeme zorunluluğu, doküman no; Brief §7.e), yoksa boş taslak; versiyon no tipteki en büyük no + 1, taslak açılırken verilir; tip başına tek taslak. Taslakta faz ve adım ekleme, düzenleme, taşıma, çıkarma (PL-03); faz çıkarma adımlarını ve medya bağlantılarını da çıkarır; sıra no'lar her işlemden sonra 1'den boşluksuz. Faz minimumu dakika olarak girilir, saniyeye çevrim tek yerde serviste; acil tipte dolu gelen minimum reddedilir. Yayın engelleri metinleri: "En az bir faz gerekir", "Faz <n> adımı yok", "Faz <n> minimum süresi yok" (yalnız planlı, PL-14). Yayın PIN ister; PIN doğrulaması ve yayın aynı veritabanı işleminde (F1-A2 kuralı); aynı işlemde eski Yürürlükte Arşiv, taslak Yürürlükte olur, yayın anı sunucu saati, yayınlayan işlemi yapan. Yayınlanmış versiyonu değiştiren her istek nedenli reddedilir | backend-engineer |
| 4 | BE | Medya: fotoğraf JPEG, PNG, WebP; video MP4 (K-26). Biçim uzantıyla ve dosyanın baştaki imza baytlarıyla birlikte doğrulanır, uyuşmazlık reddedilir. Sınır türe göre `MaxPhotoMb` ya da `MaxVideoMb` x 1 048 576 bayt, her istekte ayar servisinden. Kestrel'in varsayılan istek gövdesi sınırı (30 000 000 bayt) video sınırının altında, çok parçalı form sınırı (128 MB) ayar artarsa yetmez: yükleme action'ında ikisi de ayar değerinden türetilir, kodda MB sabiti yoktur. Ayar sınırını aşan dosya düzenleyicide hata olarak döner; gövde sınırını da aşan istek uygulamanın hata sayfasını 413 durum koduyla alır, ham sunucu metni görünmez. Dosya `wwwroot` dışında, `appsettings.json` `Media:RootPath` altında (değer `App_Data/Media`, içerik köküne göre), adı içeriğin SHA-256 özeti; aynı içerik tek dosya. Taslaktan çıkarılan medyanın dosyası silinmez (başka versiyon paylaşabilir). Sunum `/media/{id}` üzerinden, saklanan içerik tipiyle, video için aralıklı okuma (Brief §9.6) | backend-engineer |
| 5 | BE | Tip pasifleştirme ve yeniden etkinleştirme düzenleme formundan; engelleyen kayıt kontrolü arayüzü bu fazda boş döner, Faz 4'te dolar (BR-35). Pasif tip yeni kayıtta seçilmez (Faz 4) | backend-engineer |
| 6 | BE | İlk iş, frontend bunlara karşı derler: sözleşmedeki ViewModel sınıfları, enum'lar ve `SettingKeys`'in üç yeni anahtarı | backend-engineer |
| 7 | FE | E-09 Tipler ve makine eşleme (tesis ve hat gruplu seçim), E-10 Tip detayı ve versiyon geçmişi, E-11 Taslak düzenleyici (faz ve adım listesi, taşıma, medya yükleme, rayda sabit yayınlama paneli: engel listesi, PIN, düğme; PL-53), E-12 Versiyon görüntüleme; Yönetici menüsünde "Tanımlar" grubuna "Temizlik tipleri" | frontend-engineer |
| 8 | FE | `ProcedureEditor.js`: düzenleyici formlarını fetch ile gönderir, yanıttaki düzenleyici bölgesini değiştirir, odağı işlem yapılan öğeye ya da hata özetine taşır; seçilen dosyanın türünü ve boyutunu sunucunun verdiği sınırla önceden denetler (yalnız kolaylık, karar sunucuda). `MediaViewer.js`: fotoğraf ve video önizleme. JS kapalıyken tüm formlar tam sayfa çalışır | frontend-engineer |
| 9 | FE | Hata bildirimi (2026-11-15, PL-54): ortak `Views/Shared/_PinInput.cshtml` göz düğmesi sabit 48 px ve alanın kenarlığına biniyor (alan `text-2xl` ile büyüyor). Düğme yüksekliği alana bağlanır, kenarlık içinde ve dikeyde ortalı oturur; PIN geçen her ekranda (E-01, E-02, E-04 sıfırlama ve etkinleştirme, Faz 3 yayın paneli) etkilidir. Ardından Faz 1, Faz 2 ve Faz 3 ekranları G12 maddelerine göre taranır, bulunan kusur bu fazda düzeltilir; tarama sonucu devir notunda ekran ve genişlik başına "beklenen: kusur 0" olarak yazılır. Prototip sayfaları kapsam dışı (PL-10, Faz 6'da kalkar). Aynı kalemde hata bildirimi (2026-11-15, PL-55): kart modunda İşlem hücresi yeni G11 kuralına göre kurulur; Faz 1, 2 ve 3'ün tüm liste ekranları ve ray yanı tablolarının beklenen imzası yeniden yazılır | frontend-engineer |
| 10 | FE | Tur 2 bulgusu (PL-61): `bool` onay kutularında elle yazılmış gizli `false` alanı onay kutusundan önce geldiği için işaretli değer false kaydediliyor. Düzenleyici `MaterialRequired` ve Faz 2 formlarının `IsActive` alanı (`Facilities`, `Lines`, `Materials`, `Shifts`) `asp-for` ile çizilir; `Views/**` taranır. Faz 2 raporu değişmez (PL-42 düzeni) | frontend-engineer |

**Paralel:** Evet. Gerekçe: şema Brief §7.g'de belli; route, ViewModel, enum, form alanı adı, hata hedefi ve rol aşağıda; düzenleyicinin her işlemi
ayrı route. Frontend veritabanına bağlı değildir; derlemesi BE 6'ya bağlıdır, BE 6 backend'in ilk işidir. Dosyalar ayrık: menü frontend'de,
`Program.cs`, `appsettings.json` ve `SettingKeys.cs` backend'de, `SettingSeed.cs` ve `DemoData.sql` dbadmin'de.

**Sözleşme**

ViewModel ad alanı `Vaka.Web.ViewModels.Procedures`, klasör `ViewModels/Procedures/`. Enum'lar `Services/` altında, ad alanı `Vaka.Web.Services`
(entity ve ViewModel ikisi de kullanır; `SettingKeys` düzeni): `CleaningTypeKind { Planned = 1, Emergency = 2 }`,
`ProcedureVersionStatus { Draft = 1, Current = 2, Archived = 3 }`, `StepMediaType { Photo = 1, Video = 2 }`. Form girdisi
`MoveDirection { Up = 1, Down = 2 }` `ViewModels/Procedures/` altındadır. Görünür adlar backend'den `...Label` alanında gelir (Planlı, Acil;
Taslak, Yürürlükte, Arşiv; Fotoğraf, Video); görünüm enum'dan metin üretmez. Faz 2'den yeniden kullanılan tipler: `SelectOptionViewModel { Id, Label }`,
`BlockingRecordViewModel { RecordNo, StatusLabel, Url }` (`ViewModels/Definitions/`). `?` boş olabilir demektir.

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/cleaning-types?kind=&showInactive=` | `CleaningTypesController.Index` | Manager | `CleaningTypeListViewModel { Items: CleaningTypeListItemViewModel[] { Id, Code, Name, Kind, KindLabel, IsActive, CurrentVersionNo?, DraftVersionNo?, MachineCount }, Kind?: CleaningTypeKind, KindOptions[]: SelectOptionViewModel (Id enum değeri, PL-53), ShowInactive, StatusMessage? }`. Sayfalanmaz (PL-38 gerekçesi); süzgeç sunucuda | `Views/CleaningTypes/Index.cshtml` |
| GET, POST `/cleaning-types/create`, `/cleaning-types/edit/{id}` | `CleaningTypesController.Create`, `.Edit` | Manager | `CleaningTypeFormViewModel { Id?, Code, Name, Kind?: CleaningTypeKind, KindLabel?, KindOptions[]: SelectOptionViewModel (Id enum değeri), KindLocked, DefaultLockMinutes?, MaxLockMinutes, IsActive, BlockingRecords[]: BlockingRecordViewModel }` | `Views/CleaningTypes/Form.cshtml` |
| GET, POST `/cleaning-types/machines/{id}` | `CleaningTypesController.Machines` | Manager | `MachineMappingViewModel { CleaningTypeId, CleaningTypeName, Groups: MachineMappingGroupViewModel[] { FacilityLabel, LineLabel, Machines: MachineMappingItemViewModel[] { Id, Code, Name, IsSelected, IsSelectable, NotSelectableReason? } }, SelectedMachineIds[] }`. POST alanı `SelectedMachineIds` (tekrarlı) | `Views/CleaningTypes/Machines.cshtml` |
| GET `/cleaning-types/details/{id}` | `CleaningTypesController.Details` | Manager | `CleaningTypeDetailViewModel { Id, Code, Name, Kind, KindLabel, DefaultLockMinutes?, IsActive, MachineCount, CanMapMachines, Versions: ProcedureVersionItemViewModel[] { Id, VersionNo, Status, StatusLabel, PublishedAtLocal?, PublishedByName?, DocumentRef?, Url }, DraftId?, CanCreateDraft, StatusMessage? }`. `Url` taslakta düzenleyiciye, diğerlerinde versiyon görüntülemeye | `Views/CleaningTypes/Details.cshtml` |
| POST `/procedure-versions/create-draft` | `ProcedureVersionsController.CreateDraft` | Manager | `CreateDraftViewModel { CleaningTypeId }`; başarıda düzenleyiciye, taslak zaten varsa tip detayına `StatusMessage` ile yönlenir | - |
| GET `/procedure-versions/edit/{id}` | `ProcedureVersionsController.Edit` | Manager | `ProcedureDraftViewModel { Id, CleaningTypeId, CleaningTypeName, Kind, KindLabel, VersionNo, MaterialRequired, DocumentRef?, Phases: DraftPhaseViewModel[] { Id, OrderNo, Title, MinDurationMinutes?, CanMoveUp, CanMoveDown, Steps: DraftStepViewModel[] { Id, OrderNo, Title, Description?, CanMoveUp, CanMoveDown, Media: StepMediaViewModel[] { Id, MediaType, MediaTypeLabel, OriginalFileName, Url } } }, PublishBlockers[] (metin), MaxPhotoSizeMb, MaxVideoSizeMb, MediaAccept, Error?: DraftFormErrorViewModel { Target, Messages[], Title?, Description?, MinDurationMinutes?, DocumentRef? }, StatusMessage? }`. Taslak olmayan versiyonun kimliğiyle açılınca versiyon görüntülemeye yönlenir | `Views/ProcedureVersions/Edit.cshtml` |
| POST `/procedure-versions/update-header/{id}` | `.UpdateHeader` | Manager | `DraftHeaderViewModel { MaterialRequired, DocumentRef? }` | - |
| POST `/procedure-versions/add-phase/{id}`, `/update-phase/{phaseId}`, `/move-phase/{phaseId}`, `/remove-phase/{phaseId}` | `.AddPhase`, `.UpdatePhase`, `.MovePhase`, `.RemovePhase` | Manager | `PhaseFormViewModel { Title, MinDurationMinutes? }`; taşımada `MoveViewModel { Direction: MoveDirection }`; çıkarmada gövde yalnız antiforgery token | - |
| POST `/procedure-versions/add-step/{phaseId}`, `/update-step/{stepId}`, `/move-step/{stepId}`, `/remove-step/{stepId}` | `.AddStep`, `.UpdateStep`, `.MoveStep`, `.RemoveStep` | Manager | `StepFormViewModel { Title, Description? }`; taşımada `MoveViewModel` | - |
| POST `/procedure-versions/upload-media/{stepId}`, `/remove-media/{mediaId}` | `.UploadMedia`, `.RemoveMedia` | Manager | `MediaUploadViewModel { File: IFormFile }` (multipart, tek dosya) | - |
| POST `/procedure-versions/publish/{id}` | `.Publish` | Manager | `PublishViewModel { Pin }`; başarıda tip detayına `StatusMessage` ile ("v<n> yayınlandı"); engel ya da yanlış PIN'de düzenleyici `Error.Target = publish` ile yeniden çizilir | - |
| GET `/procedure-versions/details/{id}` | `.Details` | ManagerOrAuditor | `ProcedureVersionViewModel { Id, CleaningTypeId, CleaningTypeName, Kind, KindLabel, VersionNo, Status, StatusLabel, PublishedAtLocal?, PublishedByName?, DocumentRef?, MaterialRequired, Phases: VersionPhaseViewModel[] { OrderNo, Title, MinDurationMinutes?, Steps: VersionStepViewModel[] { OrderNo, Title, Description?, Media: StepMediaViewModel[] } }, StatusMessage? }`. Yalnız yayınlanmış versiyon; taslak kimliğinde Yönetici düzenleyiciye yönlenir, Denetçi 404 alır (PL-51) | `Views/ProcedureVersions/Details.cshtml` |
| GET `/media/{id}` | `MediaController.Show` | RecordReaders | dosya akışı, saklanan içerik tipiyle; aralıklı istekte 206 ve `Accept-Ranges: bytes` | - |

Ortak davranış:

- Öznitelikler: `CleaningTypesController` `[Authorize(Roles = Roles.Manager)]`; `ProcedureVersionsController` sınıfta
  `[Authorize(Roles = Roles.ManagerOrAuditor)]`, `Details` dışındaki her action'da ayrıca `[Authorize(Roles = Roles.Manager)]` (ikisi birlikte
  uygulanır, Denetçi yazma action'larında 403 alır); `MediaController` `[Authorize(Roles = Roles.RecordReaders)]`.
- Tip formları Faz 1-2 düzenindedir: alan hatası `ModelState`'te alan adıyla, başarılı POST hedef GET'e yönlenir, sonuç `StatusMessage`'dadır.
  Tip ekleme, düzenleme ve eşleme başarıda tip detayına yönlenir.
- Tip formunda tür seçilmeden kayıt yapılmaz. JS açıkken Acil seçilince kilit süresi alanı gizlenir; JS kapalıyken alan "Yalnız planlı tipte"
  yardım metniyle görünür. Düzenlemede tür kilitlidir; acil tipin düzenleme formunda kilit süresi alanı ve detayında eşleme bağlantısı yoktur.
  Acil tipte `/cleaning-types/machines/{id}` GET ve POST tip detayına `StatusMessage` ile yönlenir, eşleme yazılmaz.
- Pasifleştirme engelliyse (Faz 4'ten sonra) form `IsActive` alan hatası ve `BlockingRecords` ile yeniden çizilir.
- Düzenleyici POST'ları başarıda `/procedure-versions/edit/{id}`'ye `StatusMessage` ile yönlenir. Alan hatası, iş kuralı reddi, yayın engeli ve
  yanlış PIN düzenleyiciyi 200 ile `Error` dolu yeniden çizer. `Error.Target` hatalı formu adlandırır: `header`, `add-phase`, `phase:<phaseId>`,
  `add-step:<phaseId>`, `step:<stepId>`, `media:<stepId>`, `publish`; `Error` içindeki değerler formu gönderildiği haliyle doldurur.
  Sunucu fetch isteğini tam sayfa isteğinden ayırt etmez; JS yanıttaki düzenleyici bölgesini alır. `CreateDraft` ve `Publish` fetch'siz tam sayfa formdur.
- Yayınlanmış versiyona (Yürürlükte ya da Arşiv) yönelik her düzenleme POST'u nedenli reddedilir, veri değişmez; yanıt versiyon görüntülemeye
  `StatusMessage` ile yönlenir.
- Acil tipte faz formunda minimum alanı yoktur. Planlı tipte taslakta minimum boş kalabilir, yayını engeller. `MinDurationMinutes` pozitif tam sayıdır.
- Taşıma: ilk öğede yukarı, son öğede aşağı düğmesi sunulmaz (`CanMoveUp`, `CanMoveDown`); istekle gelirse sıra değişmez.
- Medya hatası (biçim, imza uyuşmazlığı, boyut) `Error.Target = media:<stepId>` ile döner; metin sınırı ayar değeriyle MB olarak yazar.
  `MediaAccept` dosya seçicinin `accept` değeridir (`image/jpeg,image/png,image/webp,video/mp4`).
- Yayınlayanı boş yayınlanmış versiyonda backend `PublishedByName` alanına "Kurulum verisi" yazar (PL-51).
- PIN alanı doğrulama modundadır (PL-18). Yayında yanlış PIN hatalı deneme sayar; kilitlenirse Faz 1 düzeniyle oturum kapanır.
- Menü: Yönetici "Tanımlar" grubunda "Temizlik tipleri" (`/cleaning-types`). Denetçi menüsüne bu fazda kalem eklenmez; versiyon görüntülemeye
  Faz 6 kayıt detayından gelir.

**Dosya sınırı:** frontend `Views/CleaningTypes/**`, `Views/ProcedureVersions/**`, menü kalemi ve `_PinInput.cshtml` (`Views/Shared/**`), `Styles/**`,
FE 9 için yalnız hizalama düzeltmesi olarak Faz 1 ve Faz 2 görünümleri (`Views/Account/**`, `Views/Users/**`, `Views/Home/**`, `Views/Facilities/**`, `Views/Lines/**`,
`Views/Shifts/**`, `Views/Machines/**`, `Views/Materials/**`, `Views/MaterialLots/**`),
`wwwroot/js/ProcedureEditor.js`, `wwwroot/js/MediaViewer.js`; backend `Controllers/CleaningTypesController.cs`, `Controllers/ProcedureVersionsController.cs`,
`Controllers/MediaController.cs`, `Services/` (tip, versiyon, medya servisleri, enum'lar, `SettingKeys.cs`), `ViewModels/Procedures/**`, `Program.cs`,
`appsettings.json` (`Media:RootPath`), `Docs/Db/DB-<yıl>-<sıra>.md` talebi; dbadmin `Domain/Entities/**`, `Data/**` (`SettingSeed.cs`, `DemoData.sql`
dahil). Koordinatör: `.gitignore`'a medya kökü (`Src/Vaka.Web/App_Data/`).

**Kabul kriterleri**

- [ ] K1: Yönetici planlı tip açar, varsayılan kilit süresi girer; `<AzamiKilitSüresi>` üstü değer reddedilir (seed değeri 120 ile 120 kabul, 121 red; QA kopyasında ayar değeri değiştirilince sınır yeni değere uyar, kodda sabit yoktur). Acil tipin düzenleme formunda kilit süresi alanı, detayında makine eşleme bağlantısı yoktur; acil tipte gönderilen kilit süresi reddedilir.
- [ ] K2: Kaydedilmiş tipin türü değiştirilemez; istek değiştirilerek gönderilen tür kaydı değiştirmez (PL-04).
- [ ] K3: Planlı tip makinelerle eşlenir; pasif ya da kullanımdan kaldırılmış makine ve pasif hat ya da tesisteki makine eşleme listesinde seçilemez, nedeni yazılıdır; istekle gönderilen seçilemez makine eşlenmez. Acil tipin eşleme adresi tip detayına yönlenir ve eşleme yazmaz.
- [ ] K4: Taslakta faz ve adım eklenir, taşınır, çıkarılır; sıra no'lar boşluksuz kalır. Planlı tipte minimum süresi boş faz varken yayınlama "Faz <n> minimum süresi yok" engeliyle reddedilir (PL-14); fazsız ya da adımsız fazlı taslak yayınlanamaz.
- [ ] K5: Yayınlama yanlış PIN'le yapılmaz ve hatalı deneme sayılır; doğru PIN'le versiyon "Yürürlükte", önceki "Arşiv" olur, versiyon no bir artar.
- [ ] K6: Yürürlükte bir versiyon varken yeni taslak açılır; taslak yürürlüktekinin faz, adım ve medyasını taşır; ikinci taslak açma isteği reddedilir.
- [ ] K7: Yayınlanmış versiyonun faz, adım ve medya satırı QA kopyasında SQL UPDATE ve DELETE ile değiştirilemez (tetikleyici hatası); versiyon satırının malzeme zorunluluğu ve doküman no'su SQL UPDATE ile değiştirilemez, durumu yalnız Yürürlükte'den Arşiv'e geçer. Düzenleme route'ları yayınlanmış versiyon için reddedilir ve veri değişmez (BR-04).
- [ ] K8: JPEG, PNG, WebP ve MP4 yüklenir. İzinsiz biçim (ör. GIF) ve uzantısı izinli ama içeriği o biçimde olmayan dosya reddedilir. Seed değerleriyle 10 485 760 baytlık fotoğraf kabul, 10 485 761 bayt red; 104 857 600 baytlık video kabul, 104 857 601 bayt red; ret düzenleyicide hata olarak görünür. QA kopyasında ayar değiştirilince sınır yeni değere uyar. Aynı dosya iki adıma yüklenince diskte tek kopya kalır; video tarayıcıda ileri sarılabilir, `/media/{id}` aralıklı istekte 206 döner (NFR-10).
- [ ] K9: `/media/{id}` oturumsuz istekte giriş sayfasına yönlenir.
- [ ] K10: Denetçi `/procedure-versions/details/{id}` açar, düzenleme route'larında (GET ve POST) 403 alır, taslak kimliğinde 404 alır; Operatör `/media/{id}` dışında bu fazın tüm adreslerinde 403 alır.
- [ ] K11: Düzenleyici 1024 px'te ve klavyeyle kullanılabilir (taşıma düğmeleri odaklanabilir, odak görünür, fetch işleminden sonra odak işlem yapılan öğede ya da hata özetindedir); JS kapalıyken tüm düzenleyici işlemleri tam sayfa çalışır.
- [ ] K12: Ayar tablosunda altı satır vardır; yeni üçü `MaxLockMinutes` 120 dakika, `MaxPhotoMb` 10 MB, `MaxVideoMb` 100 MB'tır; Faz 1'in üç satırı ve değişiklik geçmişi değişmemiştir; migration'da `UPDATE` yoktur (PL-25, PL-47).
- [ ] K13: Yönetici tipi düzenleme formundan pasifleştirir ve yeniden etkinleştirir; pasif tip listede `showInactive` ile metinli "Pasif" rozetiyle görünür.
- [ ] K14: E-09 liste ekranı 360, 768, 1024 ve 1440 px'te G11 imzasını taşır ve frontend devir notundaki beklenen imzayla aynıdır; E-10 versiyon tablosu bağlam rayı yanındaysa PL-39 istisnasıyla ölçülür; E-11 ve E-12 form ve detay kuralına uyar; 360 ve 390 px'te yatay kaydırma yok.
- [ ] K15: Demo veride bir planlı ve bir acil tip, ikisinin de Yürürlükte birer versiyonu vardır; planlı tip aktif demo makinelerine eşlidir; versiyon görüntülemede yayınlayan "Kurulum verisi" yazar; `DemoData.sql` ikinci kez uygulanınca satır sayıları değişmez.
- [ ] K16 (PL-54): `/account/security`, `/account/first-login`, `/account/login`, `/users/edit/{id}` (sıfırlama ve etkinleştirme), `/users/deactivate/{id}`
  ve Faz 3 yayın panelinde PIN alanının göz düğmesi 360, 768, 1024 ve 1440 px'te, açık ve koyu temada alanın kenarlık içi kutusunda kalır ve
  dikey merkezi alanınkiyle aynıdır (1 px'e kadar). Faz 1, Faz 2 ve Faz 3 ekranlarında G12 gerilemesi kusur 0; Faz 1 ve 2 ekranlarında G11 imzaları
  ve mekanik eval önceki teslimlerle aynıdır; tek fark İşlem hücresidir. Faz 1, 2 ve 3'ün tüm liste ekranları ve ray yanı tabloları
  360, 768, 1024 ve 1440 px'te yeni G11 kuralıyla (PL-55) frontend'in yeni beklenen imzasına uyar; satır kartlarında İşlem düğmeleri tek satırda ve sağa hizalı, son satırda boş hücre yok.
- [ ] K17 (PL-61): "Kayıtta malzeme zorunlu" ve Faz 2 ile Faz 3 düzenleme formlarındaki `IsActive` onay kutuları G5b üç durum testini geçer; `Views/**` G5b grep'i 0.

**Kapsam dışı:** Yayın için ikinci onay ve ileri tarihli yürürlük (K-24), adım sonuç alanları (K-27), dokümandaki örnek prosedürle demo veri (Ö-01, Faz 4'te, PL-58).

**Sonuç (2026-11-15):** TESLİM ALINDI (tur 3; tur 1 İADE B1, B2; tur 2 İADE B3, B4; tur 3 kullanıcı onaylı ek tur, PL-63). Kabul kriterleri 29/29.
B4 Faz 2 kodundaydı, Faz 3'te düzeltildi (PL-61). Yeni ortak kriterler G5a, G5b, G12 bu fazda doğdu. Faz 4'e devreden: F3-A1 (qa-tester).
Rapor: `Docs/Delivery/Faz-3-Teslim.md`.

---

## Faz 4: Kayıt açma ve makine kilidi

**Amaç:** Operatörün makine ve tip seçerek kayıt açması, kaydın makineyi kilitlemesi, kilit süresi dolunca zaman aşımına düşmesi; olay günlüğünün ilk kez yazılması.

**Başlama şartı:** Faz 3 TESLİM ALINDI ve commit'i atıldı. Olay özeti anahtarı engeli kalktı (PL-48); geliştirme anahtarını geliştirici ve
qa-tester kendi makinesinde user-secrets'a yazar, kurulum adımını koordinatör README'ye ekler.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Temizlik kaydı, faz ve adım yürütmeleri, adım görevlileri, kayıt malzemeleri, olay günlüğü, işlenmiş komutlar ve numara sayaçları tabloları. Önden gelen kolonlar (PL-02): saha referansı, başlangıç, tamamlanma ve kapanış zamanları, kapanış türü, kapatan kişi ve kapanış gerekçesi (Faz 6 müdahalesi, PL-46), brüt, mola, bekleme, net ve efor toplamları, acil açıklaması, faz süre altı alanları, adım zaman, adım molası (SH-17, PL-19), süre ve efor alanları, görevli "çalışıyor" bayrağı | backend-engineer talebi, dbadmin |
| 2 | DB | Kısmi unique index'ler: makine başına tek Açıldı, Devam ediyor ya da Beklemede kayıt (BR-23); sahip başına tek Açıldı ya da Devam ediyor kayıt (BR-37); kişi başına tek çalışılan adım (BR-24). Unique: kayıt no, saha referansı, komut kimliği, kayıt içi olay sırası. Tetikleyiciler: olay günlüğünde UPDATE ve DELETE ret (BR-31); adımda dolu başlangıç ve bitiş zamanının değişmesi ret (BR-28). SQLite WAL ve busy timeout | backend-engineer talebi, dbadmin |
| 2a | DB | Demo veri (Ö-01, PL-58): `DemoData.sql`'e dokümandaki örnekle bir planlı tip: 3 faz, sırasıyla 5, 3 ve 4 adım, her fazın minimumu 15 dakika, Yürürlükte v1, en az bir aktif demo makinesine eşli, yayınlayan boş. Faz 3 demo satırları değişmez | backend-engineer talebi, dbadmin |
| 3 | BE | Olay yazıcı: her durum değişikliği aynı transaction'da olay yazar; özet = HMAC-SHA256(anahtar, önceki özet + kanonik olay) (BR-32, NFR-12); komut kimliğiyle tekrar istekte ilk sonuç döner (BR-33). Anahtar yapılandırma anahtarı `EventChain:HmacKey`'den okunur (Base64, çözülünce en az 32 bayt): Development'ta user-secrets (`Vaka.Web.csproj`'a `UserSecretsId`), üretimde ortam değişkeni `EventChain__HmacKey`; `appsettings*.json`'da ve repoda yoktur; üretim anahtarı BT'den gelince kod değişmez (PL-48). Uygulama başlangıçta anahtarı doğrular; yoksa ya da kısaysa başlamaz, nedeni günlüğe yazar, varsayılan anahtar üretmez | backend-engineer |
| 4 | BE | Kayıt açma servisi: BR-01, BR-02 sunucu doğrulaması; yürürlükteki versiyona bağlama ve faz, adım satırlarını Bekliyor olarak oluşturma, sahibi her adıma varsayılan görevli yazma (BR-03, BR-17); açılış anı değerleri (BR-06); kayıt no (BR-08); iş emri planlıda zorunlu (K-18); kilit süresi tipten, `<AzamiKilitSüresi>` sınırı (BR-45); çakışmada sahip adı, durum ve açılış saatiyle hata | backend-engineer |
| 5 | BE | `IDurationCalculator` ilk metotları: SH-13 ve zaman aşımı kararı; tesisin aktif vardiyalarını okuyup Faz 2'nin `FindShiftAt` metodunu çağıran vardiya bulucu servis (PL-40); zaman aşımı kayda ya da makineye dokunan her istekte ve pano sorgusunda aynı transaction'da uygulanır, zaman aşımı anı kilit bitişidir (BR-26) | backend-engineer |
| 6 | BE | Malzeme ekleme ve çıkarma: yalnız sahip, PIN, ilk adımdan önce, kilit bitişi geçmemişken; SKT geçmiş lotta gerekçe zorunlu (BR-12, BR-46, BR-47). Malzeme eklemek kilidi uzatmaz | backend-engineer |
| 7 | BE | Açık iş genişlemesi: sahibi olduğu aktif kayıt kullanıcı pasifleştirmeyi; aktif kayıt makine ve tip kaldırmayı engeller (BR-35, BR-36). Ana sayfa Operatör'ü `/my-work`'e yönlendirir. Süre biçimleyici (saniyeden "sa dk sn" metni) | backend-engineer |
| 8 | FE | E-13 İşlerim, E-14 Yeni kayıt (bağlı seçimler, makinenin meşgul nedeni, lot seçici ve SKT gerekçesi, kilit süresi, PIN), E-15 Yürütme ekranının Açıldı hali (başlık, kilit bitişi, malzeme ekle ve çıkar, faz ve adım listesi salt okunur), E-16 Makine durum panosu | frontend-engineer |
| 9 | FE | `CommandForm.js`: durum değiştiren formu fetch ile gönderir, bağlantı yoksa "İşlem yapılmadı, bağlantı yok" gösterir, yeniden deneme aynı `CommandId` ile gider, düğme gönderim sürerken pasif (BR-52, NFR-14) | frontend-engineer |

**Paralel:** Evet. Gerekçe: frontend yalnız aşağıdaki ViewModel ve JSON uçlarına bağlı; şema ve olay altyapısı backend ve dbadmin tarafında, dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/my-work` | `MyWorkController.Index` | Operator | `MyWorkViewModel { ActiveRecord? { Id, RecordNo, Kind, StatusLabel, MachineLabel, CleaningTypeName, VersionNo, LockExpiresAtLocal?, Url }, AssignedSteps[] (Faz 5), HeldRecords[] (Faz 7), OpenBreak? (Faz 5), CanOpenRecord, CannotOpenReason, CannotOpenUrl? }` | `Views/MyWork/Index.cshtml` |
| GET, POST `/records/create` | `RecordsController.Create` | Operator | `RecordCreateViewModel { CommandId, FacilityId?, LineId?, MachineId?, CleaningTypeId?, WorkOrderNo, Note, LockMinutes, DefaultLockMinutes?, MaxLockMinutes, Materials[] { MaterialLotId, ExpiredReason }, Pin, Facilities[] { Id, Label }, Lines[], Machines[] { Id, Label, IsAvailable, BusyReason }, CleaningTypes[] { Id, Name, VersionNo, MaterialRequired, DefaultLockMinutes }, ErrorMessage, ErrorUrl? }` | `Views/Records/Create.cshtml` |
| GET `/records/lines?facilityId=` | `RecordsController.Lines` | Operator | JSON `[{ id, label }]` | - |
| GET `/records/machines?lineId=` | `RecordsController.Machines` | Operator | JSON `[{ id, label, isAvailable, busyReason }]` | - |
| GET `/records/cleaning-types?machineId=` | `RecordsController.CleaningTypes` | Operator | JSON `[{ id, name, versionNo, materialRequired, defaultLockMinutes }]` | - |
| GET `/records/lots?q=` | `RecordsController.Lots` | Operator | JSON `[{ id, materialCode, materialName, lotNo, expiryDate, isExpired }]` | - |
| GET `/records/run/{id}` | `RecordsController.Run` | Operator | `RecordRunViewModel { Id, RecordNo, Kind, Status, StatusLabel, FieldBookRef?, FacilityLabel, LineLabel, MachineLabel, CleaningTypeName, VersionNo, OwnerName, OpenedAtLocal, LockExpiresAtLocal?, WorkOrderNo, Note, MaterialRequired, IsOwner, CanManageMaterials, CommandId, Materials[] { Id, MaterialCode, MaterialName, LotNo, ExpiryDate, ExpiredAtEntry, ExpiredReason, EnteredByName, EnteredAtLocal }, Phases[] { Id, OrderNo, Title, MinDurationSeconds?, Status, StatusLabel, Steps[] { Id, OrderNo, Title, Status, StatusLabel, IsNext } } }` | `Views/Records/Run.cshtml` |
| POST `/records/add-material/{id}` | `RecordsController.AddMaterial` | Operator | `{ CommandId, MaterialLotId, ExpiredReason?, Pin }` | - |
| POST `/records/remove-material/{id}` | `RecordsController.RemoveMaterial` | Operator | `{ CommandId, RecordMaterialId, Pin }` | - |
| GET `/machine-board?facilityId=` | `MachineBoardController.Index` | RecordReaders | `MachineBoardViewModel { FacilityId?, Facilities[], Groups[] { FacilityName, LineName, Machines[] { Id, Code, Name, State, StateLabel, RecordId?, RecordNo?, OwnerName?, OpenedAtLocal?, LockExpiresAtLocal?, RecordUrl? } }, GeneratedAtLocal }` | `Views/MachineBoard/Index.cshtml` |

Yürütme ekranı yalnız Operatör'e açıktır; Yönetici ve Denetçi kaydı Faz 6'daki detaydan izler (PL-12).
`State` değerleri: `Free`, `Opened`, `InProgress`, `OnHold` (Faz 7'de dolar). `RecordUrl` Operatör'de yürütme ekranı, Yönetici ve Denetçi'de Faz 6'dan sonra detaydır.

Sözleşme ekleri (PL-62, paralel başlangıç için):

- Ad alanı `Vaka.Web.ViewModels.Records`, klasör `ViewModels/Records/` (`MyWorkViewModel` ve `MachineBoardViewModel` dahil). Alt tipler: `ActiveRecordViewModel`,
  `MachineOptionViewModel { Id, Label, IsAvailable, BusyReason }`, `CleaningTypeOptionViewModel`, `MaterialEntryViewModel { MaterialLotId, ExpiredReason }`,
  `RecordMaterialViewModel`, `RecordPhaseViewModel`, `RecordStepViewModel`, `MachineBoardGroupViewModel`, `MachineBoardItemViewModel`; `Facilities[]` ve
  `Lines[]` Faz 2 `SelectOptionViewModel`. `?` boş olabilir.
- Enum'lar `Services/` altında (PL-50 düzeni): `RecordStatus { Opened = 1, InProgress = 2, OnHold = 3, Completed = 4, TimedOut = 5, Cancelled = 6, Aborted = 7 }`,
  `PhaseStatus { Waiting = 1, InProgress = 2, Completed = 3, CompletedBelowMinimum = 4, Interrupted = 5 }`, `StepStatus { Waiting = 1, InProgress = 2,
  Completed = 3, Interrupted = 4 }` ("Hazır" saklanmaz, `IsNext` ile türer), `MachineState { Free = 1, Opened = 2, InProgress = 3, OnHold = 4 }`; `Kind`
  Faz 3 `CleaningTypeKind`. ViewModel `Status`/`State` enum'u ve `...Label` metnini birlikte taşır; rozet biçimini frontend enum'dan Faz T `_Badge`
  sözlüğüne tek yerde eşler, metin Brief §6 ve `_Badge` ile aynıdır.
- Süre biçimleyici: `Services/IDurationFormatter.Format(int seconds)` -> `DurationPart[] { Value, Unit }` (`sa`, `dk`, `sn`); görünüm `@inject` ile
  çağırır, rakamı mono, birimi sans yazar (T-B14). Biçimleyici aritmetik dışında karar vermez.
- `/records/create` POST alanları: `CommandId`, `FacilityId`, `LineId`, `MachineId`, `CleaningTypeId`, `WorkOrderNo`, `Note`, `LockMinutes`,
  `Materials[i].MaterialLotId`, `Materials[i].ExpiredReason`, `Pin`. JS kapalıyken tesis, hat ve makine seçimi aynı formda `refresh=true` POST'uyla
  yenilenir (Faz 2 PL-44 düzeni): kayıt açılmaz, olay yazılmaz, `CommandId` ve girilen alanlar korunur. Başarıda `/records/run/{id}`.
- `CommandForm.js`: sunucu fetch isteğini ayırt etmez. Başarılı komut hedef GET'e yönlenir; JS yanıt adresi farklıysa `location.assign`, aynıysa
  sayfayı yanıtla değiştirir. Reddedilen komut aynı görünümü 200 ile `ErrorMessage` ve `ErrorUrl?` dolu çizer. Ağ hatasında form ve `CommandId`
  değişmez, "İşlem yapılmadı, bağlantı yok" gösterilir. Aynı `CommandId` ile tekrar istek ilk sonucun yönlendirmesini döner (BR-33).
- Malzeme ekle ve çıkar başarıda `/records/run/{id}`'ye yönlenir; reddi `RecordRunViewModel.ErrorMessage` ile (Run'a `ErrorMessage?` eklenir).
- Ortak kriterler G5a, G5b, G11, G12 bu fazın ekranlarına uygulanır.
- JS kapalı lot seçimi (PL-64): `RecordCreateViewModel` ve `RecordRunViewModel`'e `LotQuery?`, `AvailableLots[]: LotOptionViewModel { Id, MaterialCode,
  MaterialName, LotNo, ExpiryDate, IsExpired }` (aktif malzemelerin lotları, makine ve tesisten bağımsız, en çok 50 satır) ve `AvailableLotsTruncated`
  (doluysa "Aramayı daraltın" yazılır) eklenir. Yeni kayıtta JS kapalıyken lot ekleme ve çıkarma aynı formda `refresh=true` POST'uyla yapılır:
  `LotQuery` süzer, `AddMaterialLotId` ve `AddExpiredReason` `Materials[]`'a satır ekler, `RemoveMaterialIndex` çıkarır; kayıt açılmaz, olay
  yazılmaz, diğer alanlar ve `CommandId` korunur. Yürütme ekranında JS kapalıyken lot `AvailableLots` seçimiyle `add-material` POST'una gider,
  süzgeç `GET /records/run/{id}?q=`. JS açıkken `LotPicker.js` `/records/lots?q=` JSON'unu kullanır; iki yolda sunucu kuralları aynıdır.
- Reddedilen malzeme ekleme Run'ı `ErrorMessage` ve `PendingMaterial? { MaterialLotId, ExpiredReason }` ile yeniden çizer; seçili lot ve gerekçe
  korunur, PIN korunmaz (Faz 1 "başarısız POST aynı ViewModel'le" kuralı, PL-64).
- `CommandForm.js` sapması kabul (PL-64): JS yalnız `response.redirected` doğruysa ve adres farklıysa `location.assign` yapar, aksi halde sayfayı
  yanıtla değiştirir. Backend reddi asla yönlendirmeyle değil, 200 görünümle döner; yetki reddi 403 sayfasıdır.
- K10 ve K11 JS kapalıyken de denenir: malzeme zorunlu tipte JS kapalı tarayıcıda lot seçilip kayıt açılır; SKT geçmiş lotta gerekçe istenir.

**Dosya sınırı:** frontend `Views/MyWork/**`, `Views/Records/Create.cshtml`, `Views/Records/Run.cshtml`, `Views/MachineBoard/**`, `wwwroot/js/CommandForm.js`,
`wwwroot/js/LotPicker.js`, `wwwroot/js/CascadeSelect.js`; backend `Controllers/MyWorkController.cs`, `Controllers/RecordsController.cs`,
`Controllers/MachineBoardController.cs`, `Services/` (kayıt açma, olay yazıcı, numara, `IDurationCalculator`), `ViewModels/Records/**`.

**Kabul kriterleri**

- [ ] K1: Yeni kayıtta hat listesi seçilen tesise, makine listesi seçilen hatta göre gelir; pasif ve kullanımdan kaldırılmış makine seçilemez; tip listesinde yalnız makineye eşli, yürürlükte versiyonu olan planlı tipler vardır (BR-01, BR-02).
- [ ] K2: İstek değiştirilerek başka hattın makinesi ya da eşlemesiz tip gönderilince kayıt açılmaz ve olay yazılmaz (BR-02).
- [ ] K3: Planlı iş emri boşken kayıt açılmaz (K-18).
- [ ] K4: Kayıt açılınca yürütme ekranı açılır; kayıt no `<TesisKodu>-<HatKodu>-<MakineKodu>-P<Yıl>-<Sıra>` biçimindedir, aynı makinede aynı yıl ikinci kayıtta sıra bir artar; durum "Açıldı"; saha referansı boştur (BR-08, BR-09). İş emri ve açıklama yürütme ekranında görünür (Ö-05).
- [ ] K5: Kilit bitişi: açılışı içeren vardiyası olan tesiste en erken (açılış + kilit süresi, vardiya bitişi), vardiyasız tesiste ve iki vardiya arasındaki boşlukta açılış + kilit süresi; QA iki tesiste dener, gece yarısını geçen vardiyayı da dener (SH-13, SC-36 kilit kısmı). Kayıt açıldıktan sonra vardiyanın bitiş saati değiştirilince açık kaydın kilit bitişi değişmez (K-38).
- [ ] K6: Kilit süresi `<AzamiKilitSüresi>` üstündeyse kayıt açılmaz (BR-45); seed değeri 120 ile 121 dakika reddedilir, 120 kabul edilir.
- [ ] K7: Makinede Açıldı ya da Devam ediyor kayıt varken ikinci açma reddedilir; mesajda sahip adı, durum ve açılış saati vardır. İki oturumdan eşzamanlı iki açma isteğinde tek kayıt oluşur (SC-04, BR-23).
- [ ] K8: Aktif kaydı olan operatör başka makinede kayıt açamaz; mesaj mevcut kaydın numarasını bağlantıyla gösterir; eşzamanlı iki istekte de tek kayıt oluşur (SC-29, BR-37).
- [ ] K9: Kilit bitişi geçmiş Açıldı kayıt, makine panosu açılınca "Zaman aşımı" olur ve makine Boş görünür; olay günlüğünde zaman aşımı anı kilit bitişidir; aynı kayda malzeme ekleme isteği reddedilir; başka operatör makine için kayıt açabilir (SC-15, BR-26, BR-46).
- [ ] K10: SKT geçmiş lot seçicide "SKT geçmiş" uyarısıyla görünür; gerekçesiz ekleme reddedilir; gerekçeyle eklenen satır işaretli görünür (SC-08).
- [ ] K11: Malzeme ekleme ve çıkarma yalnız sahibin PIN'iyle yapılır; başka operatörün doğrudan isteği reddedilir; ekleme kilit bitişini değiştirmez (BR-46, BR-47).
- [ ] K12: Aynı `CommandId` ile iki kez gönderilen açma isteği tek kayıt ve tek olay üretir (SC-23).
- [ ] K13: Tarayıcı çevrimdışıyken "Kaydı oluştur" işlemin yapılmadığını gösterir; çevrimiçi olunca yeniden deneme tek kayıt üretir (SC-33).
- [ ] K14: QA kopyasında olay günlüğü satırına SQL UPDATE ve DELETE tetikleyici hatasıyla reddedilir (BR-31).
- [ ] K15: Her olay önceki olayın özetini taşır; özet anahtarı `appsettings*.json`, kaynak kod ve repoda yoktur (BR-32, NFR-12). Anahtarı user-secrets'ta olan geliştirme ortamında olay yazılır; `EventChain:HmacKey` tanımsızken uygulama başlamaz ve günlükte nedeni yazar; anahtar ortam değişkeniyle verildiğinde kod ve yapılandırma dosyası değişmeden çalışır (PL-48).
- [ ] K16: `IDurationCalculator` SH-13 ve zaman aşımı metotlarının XML yorumunda formül ve kimlik yazılıdır.
- [ ] K17: Kayıt yürürlükteki versiyona bağlanır; kayıt Açıldı iken yeni versiyon yayınlansa da kaydın versiyon no'su değişmez (BR-03, SC-20).
- [ ] K18: Makine durum panosu Operatör, Yönetici ve Denetçi'de açılır, yalnız Süper admin rolündeki kullanıcıda 403 döner; durum metinle yazılıdır; zaman aşımına düşmesi gereken kayıt panoda Açıldı görünmez.
- [ ] K19: Aktif kaydı olan makine kullanımdan kaldırılamaz ve tipi pasifleştirilemez; sahibi pasifleştirilemez; ekranlar engelleyen kaydı bağlantıyla listeler (BR-35, BR-36).
- [ ] K20: Operatör `/` adresinde `/my-work`'e yönlenir; aktif kaydı varsa kart numara, makine, durum ve kilit bitişiyle görünür; Yönetici rolü tek başına olan kullanıcı `/records/create` ve `/my-work` için 403 alır (K-29).
- [ ] K21 (Ö-01): Demo veride dokümandaki örnek tip vardır: 3 faz, 5, 3 ve 4 adım, her fazın minimumu 15 dakika; tip demo makinesinde yeni kayıt formunda seçilir; `DemoData.sql` ikinci kez uygulanınca satır sayıları değişmez.

**Faz T'den devreden (PL-21, PL-23):**

- [ ] T-B14 (BE, FE): Süre biçimleyicinin çıktısında rakam ve birim ayrı işaretlenir; ekranda yalnız rakam mono, birim sans yazılır.
- [ ] T-B22 (FE): Yürütme ekranı kayıt başlığında 1024-1279 px'te iki ray açıkken alan değerleri en çok iki satıra bölünür; saha referansı ve kayıt no satır içinde bölünmez.

**Faz 1'den devreden (PL-32):**

- [ ] F1-A1 (QA): Sahibi olduğu aktif kaydı olan kullanıcının `/users/deactivate/{id}` ekranı açık işi listeler, pasifleştirme düğmesi yoktur;
  kayıt bağlantısı yalnız görüntüleyenin erişebildiği ekrana verilir, Süper admin görüntülerken bağlantı yoktur (K-04, PL-16, Faz 1 kalem 7).
- [ ] F1-A2 (BE): PIN isteyen her işlemde (Faz 1 hesap işlemleri dahil) PIN doğrulaması ve iş eylemi aynı veritabanı işleminde yapılır; doğru
  PIN'den sonra iş kuralı reddinde kısmi yazma kalmaz, yanlış PIN'de hatalı deneme sayacı yine yazılır (BR-48).

**Faz 3'ten devreden (PL-63):**

- [ ] F3-A1 (QA): Aktif kaydı olan temizlik tipinin pasifleştirilmesi `IsActive` alan hatası ve engelleyen kayıt listesiyle reddedilir; bağlantı
  yalnız görüntüleyenin erişebildiği ekrana verilir (BR-35; K19 ile birlikte).

**Faz 2'den devreden (PL-52):**

- [ ] F2-A1 (QA): Aktif kaydı olan makinenin `/machines/retire/{id}` ekranı engelleyen kaydı bağlantısıyla listeler, kaldırma yapılmaz; bağlantı
  yalnız görüntüleyenin erişebildiği ekrana verilir (BR-35; K19 ile birlikte).
- [ ] F2-A2 (QA): `FindShiftAt` gece yarısını geçen vardiyada ve vardiya sınır anlarında (başlangıç dahil, bitiş hariç) çalışan uygulamada
  doğru vardiyayı bulur (K5 ile birlikte, PL-40).

**Kapsam dışı:** Adım yürütme (Faz 5), kayıt detayı ve listesi (Faz 6), acil kayıt (Faz 6A).

**Sonuç (2026-11-15):** TESLİM ALINDI (tur 2; tur 1 İADE B1, JS kapalı lot akışı, PL-67). Kabul kriterleri 42/42; Faz 1-3'ten devreden F1-A1..F3-A1 kapandı.
Faz 5'e devreden: F4-A1, F4-A2. Rapor: `Docs/Delivery/Faz-4-Teslim.md`.

---

## Faz 5: Adım yürütme, görevli ve mola

**Amaç:** Operatörün adımları sırayla PIN'le başlatıp bitirmesi, sahibin adımlara görevli eklemesi, iki adım arasında mola verilmesi ve kaydın
otomatik tamamlanması. Adım sürerken mola (K-39) ve görevlinin kendi PIN'iyle katılım onayı (K-10) Faz 7'dedir (PL-57).

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Mola tablosu: başlatan, molanın başladığı anda devam eden adım (boş olabilir; Faz 5'te hep boş, Faz 7 için önden, PL-19, PL-57), zamanlar, bitiren, kapanış türü (Kişi, Yönetici, Müdahale), gerekçe; kayıt başına ve kişi başına tek açık mola için kısmi unique index (BR-40); dolu mola zamanlarının değişmesini reddeden tetikleyici | backend-engineer talebi, dbadmin |
| 1b | DB | Kısa demo tipi (PL-65), `DemoData.sql`'e ek (Faz 3-4 düzeni, `INSERT OR IGNORE`, sabit Id): planlı, kod `KSA`, ad "Kısa planlı temizlik", varsayılan kilit süresi PLN ile aynı (90 dakika); 2 faz, her fazda 2 adım, her faz minimumu 60 saniye; malzeme zorunlu değil; Yürürlükte v1, yayınlayan boş; PLN ve PLN3 ile aynı aktif demo makinelerine eşli. PLN, ACL ve PLN3 satırları değişmez | backend-engineer talebi, dbadmin |
| 2 | BE | Adım servisi: SK-1 (sahip ya da görevli), BR-14 katı sıra, BR-15 tek devam eden adım, BR-24 tüm görevliler boşta, BR-11 zorunlu malzeme, BR-46 kilit kontrolü, BR-39 kayıtta açık mola varken başlatma ve bitirme reddi; ilk adımda saha referansı (BR-09); bitirmede SH-17, SH-01, SH-02; faz son adımında SH-07 ve gerekçe (BR-21); faz ve kayıt otomatik tamamlanma, makine kilidinin kalkması (BR-22) | backend-engineer |
| 3 | BE | Görevli servisi: yalnız sahip, adım başlamadan, kendi PIN'iyle ekler ve çıkarır (SK-2); eklenen kişinin PIN'i bu fazda istenmez, katılım onayı Faz 7'de (K-10, PL-57); pasif ya da molada kişi eklenemez; kendi kaydı devam eden kişiye uyarı bilgisi (K-41) | backend-engineer |
| 4 | BE | Mola servisi: BR-38 başlatma koşulu bu fazda: kayıt Devam ediyor ve devam eden adım yok (iki adım arasında; adım sürerken mola Faz 7, PL-57); başlatan sahip ya da sıradaki adımın görevlisi (SK-6); başlatan başka kayıtta devam eden adımda değil; BR-39 kayıt ve kişi kilidi (molada kişinin tüm yazma istekleri servis katmanında reddedilir, controller `/breaks/current`'a yönlendirir; kayıtta adım başlatma, bitirme, görevli ve malzeme reddedilir); BR-40 yalnız başlatan bitirir | backend-engineer |
| 5 | BE | `IDurationCalculator`: SH-17 (formül genel; Faz 5'te adım içi mola olmadığından 0), SH-01, SH-02, SH-03, SH-04, SH-06 (bekleme terimsiz), SH-07, SH-08, SH-09, SH-11 (bekleme terimsiz), SH-12, SH-14 (açık molada değer artmaz); açık iş genişlemesi: tamamlanmamış adımda görevlilik ve açık mola pasifleştirmeyi engeller (BR-36) | backend-engineer |
| 6 | FE | E-15 Yürütme ekranının tam hali: sıradaki adım belirgin, kilitli adımlar ve kilit nedeni, adım açıklaması ve medyası, başlat ve bitir (PIN), faz minimumu ve canlı net süre, süre altı gerekçe alanı, mola düğmesi yalnız iki adım arasında, açık mola bandı (molada olan kişi, başlangıç, "Sayaçlar durdu"), molalar listesi, tamamlanma özeti; E-17 Görevli paneli (kişi arama, sahibin PIN'i, K-41 uyarısı); E-18 Mola ekranı; İşlerim'de görevli olduğum adımlar ve açık mola | frontend-engineer |
| 7 | FE | `LiveTimer.js` (sunucunun verdiği saniyeden gösterimi ilerletir, `TimersRunning` false iken ilerletmez, hesap yapmaz), `ParticipantPicker.js` | frontend-engineer |
| 8 | FE | İlk iş, kullanıcı isteği (2026-11-15, PL-66): PIN bileşeni doğrulama modunda da `maxlength` güncel `<PinUzunluğu>` olur ve rakam dışı karakteri istemcide kabul etmez. Değer `_PinInput.cshtml`'de `@inject ISystemSettingsReader` ile `SettingKeys.PinLength`'ten okunur (tek yer, görünümde sabit yok); sunucu kuralları değişmez. Faz 1-4'teki tüm PIN alanlarına uygulanır; Faz 4 raporu değişmez | frontend-engineer |
| 9 | FE | İlk iş, görsel rötuş (PL-66; DNA kartı değişmez, `Dna.md` bileşen notu güncellenir): (a) PIN göz düğmesi dinlenmede dolgusuz, alanın iç köşe yarıçapıyla uyumlu, hover ve odakta token renginden hafif ton, odak halkası görünür, 44 px hedef korunur; (b) tüm `<select>`'ler `appearance-none`, `.input` ile aynı kenarlık, yarıçap, yükseklik, iç boşluk ve odak; ok proje sprite'ındaki `chevron-down`, açık ve koyu tema ile pasif hali; Faz 1-4'teki tüm select'ler | frontend-engineer |
| 10 | FE | Kullanıcı isteği (2026-11-15, PL-70): süre altı gerekçe JS açıkken yürütme ekranında `<dialog>` içinde. "Bitir" POST'u sunucudan `FinishStep` görünümüyle (200) dönerse `CommandForm.js` sayfayı değiştirmez; yanıttaki `form[data-finish-reason]`'ı dialoga yerleştirir: faz adı, faz net süresi, minimum, gerekçe (zorunlu), PIN, "Gerekçeyle bitir", "Vazgeç". Gönderim aynı `CommandId` ile; ret yanıtı yine `FinishStep` görünümü olduğu için dialog içeriği yenilenir. Odak açılışta ilk alana, kapanışta "Bitir" düğmesine döner; Esc ve "Vazgeç" kapatır, adım "Devam ediyor" kalır. JS kapalıyken `FinishStep.cshtml` sayfası yedektir. Sunucu kuralı ve sözleşme değişmez; backend'e iş yok | frontend-engineer |

**Paralel:** Evet. Gerekçe: ekranın tüm kararları ViewModel bayraklarından gelir (`CanStart`, `CanFinish`, `CanManageParticipants`, `CanStartBreak`, `TimersRunning`); route'lar aşağıda; dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/records/run/{id}` | `RecordsController.Run` | Operator | Faz 4 alanlarına ek: `Phases[] { GrossSeconds?, BreakSeconds?, NetSeconds?, LiveNetSeconds?, BelowMinimum, BelowMinimumReason, Steps[] { Description, Media[] { Id, MediaType, Url }, StartedAtLocal?, StartedByName?, EndedAtLocal?, EndedByName?, BreakSeconds?, DurationSeconds?, EffortSeconds?, LiveElapsedSeconds?, IsLastOfPhase, Participants[] { UserId, FullName, RegistryNo, IsOwner, AddedAtLocal }, CanStart, CanFinish, CannotStartReason, CannotFinishReason, CanManageParticipants } }, Breaks[] { StartedByName, StartedAtLocal, EndedAtLocal?, DurationSeconds?, EndKindLabel, DuringStepTitle? }, OpenBreak? { Id, StartedByName, StartedAtLocal, DuringStepTitle? }, TimersRunning, CanStartBreak, CannotBreakReason, Summary { GrossSeconds?, BreakSeconds?, NetSeconds?, EffortSeconds?, StartedAtLocal?, CompletedAtLocal? }, FirstStepStartedByName? ` | `Views/Records/Run.cshtml` |
| POST `/records/start-step/{stepId}` | `RecordsController.StartStep` | Operator | `{ CommandId, Pin }` | - |
| POST `/records/finish-step/{stepId}` | `RecordsController.FinishStep` | Operator | `{ CommandId, Pin, BelowMinimumReason? }`; süre altındaysa `FinishStepViewModel { StepId, PhaseTitle, PhaseNetSeconds, MinDurationSeconds, CommandId, BelowMinimumReason, Pin }` ile gerekçe formu döner | `Views/Records/FinishStep.cshtml` |
| GET `/records/people?q=&stepId=` | `RecordsController.People` | Operator | JSON `[{ userId, fullName, registryNo, isAvailable, unavailableReason, hasOwnActiveRecord }]` | - |
| POST `/records/add-participant/{stepId}` | `RecordsController.AddParticipant` | Operator | `{ CommandId, ParticipantUserId, Pin }` (sahibin PIN'i; `ParticipantPin` Faz 7'de eklenir) | - |
| POST `/records/remove-participant/{stepId}` | `RecordsController.RemoveParticipant` | Operator | `{ CommandId, ParticipantUserId, Pin }` | - |
| POST `/breaks/start/{recordId}` | `BreaksController.Start` | Operator | `{ CommandId, Pin }` | - |
| GET `/breaks/current` | `BreaksController.Current` | Operator | `CurrentBreakViewModel { BreakId, RecordId, RecordNo, MachineLabel, StartedAtLocal, ElapsedSeconds, DuringStepTitle?, CommandId }` | `Views/Breaks/Current.cshtml` |
| POST `/breaks/end/{breakId}` | `BreaksController.End` | Operator | `{ CommandId, Pin }` | - |
| GET `/my-work` | `MyWorkController.Index` | Operator | Faz 4'e ek: `AssignedSteps[] { RecordId, RecordNo, MachineLabel, StepTitle, OwnerName, Url }`, `OpenBreak? { BreakId, RecordNo, StartedAtLocal }` | `Views/MyWork/Index.cshtml` |

Açık molası olan kullanıcı `/my-work` ve `/records/run/{id}` GET'lerinde `/breaks/current`'a yönlenir (Brief §7.c). Kayıtta mola açıkken
diğer kullanıcılar yürütme ekranını görür: `TimersRunning` false, `OpenBreak` dolu, `CanStart` ve `CanFinish` false, neden "Kayıtta açık mola var".
`DuringStepTitle` molanın başladığı anda devam eden adımın başlığıdır, iki adım arasındaki molada boştur; Faz 5'te hep boştur (PL-57).
`CanStartBreak` devam eden adım varken false, neden "Mola iki adım arasında verilir".

Sözleşme ekleri (PL-68, paralel başlangıç için):

- Ad alanları `Vaka.Web.ViewModels.Records` (Faz 4 `RecordRunViewModel` ve `MyWorkViewModel` aynı sınıflar, alan eklenir) ve `Vaka.Web.ViewModels.Breaks`
  (`CurrentBreakViewModel`). Alt tipler: `RecordBreakViewModel`, `OpenBreakViewModel`, `StepParticipantViewModel`, `RecordSummaryViewModel`,
  `AssignedStepViewModel`, `ParticipantOptionViewModel { UserId, FullName, RegistryNo, IsAvailable, UnavailableReason, HasOwnActiveRecord }`.
- Enum `Services/BreakEndKind { Person = 1, Manager = 2, Intervention = 3 }`; görünür adı `EndKindLabel`. Adım ve faz durumları Faz 4 enum'larıdır.
- Komut formları (başlat, bitir, görevli ekle ve çıkar, mola başlat ve bitir) Faz 4 `CommandForm.js` kuralıyla çalışır (PL-62, PL-64): başarı hedef GET'e
  yönlenir, ret 200 görünüm ve `ErrorMessage`; reddedilen görevli eklemede seçili kişi `PendingParticipant? { StepId, ParticipantUserId }` ile korunur, PIN korunmaz.
- JS kapalı görevli ekleme: `RecordRunViewModel`'e `ParticipantStepId?`, `ParticipantQuery?`, `ParticipantOptions[]` (en çok 50) ve
  `ParticipantOptionsTruncated`; süzgeç `GET /records/run/{id}?stepId=&people=`, ekleme `add-participant` POST'u. JS açıkken `ParticipantPicker.js`
  `/records/people` JSON'unu kullanır; iki yolda sunucu kuralları aynı (Faz 4 B1'in tekrarı olmasın diye).
- Süre altı gerekçe: gerekçesiz bitirme isteği `FinishStepViewModel` ile `Views/Records/FinishStep.cshtml` gerekçe formunu 200 ile çizer;
  gerekçeli gönderim aynı `CommandId` ile bitirir.
- F4-A2: `AvailableLots` ve `/records/lots?q=&recordId=` kayda eklenmiş lotu listelemez.
- Ortak kriterler G5a, G5b, G8, G9, G11, G12 bu fazın ekranlarına uygulanır.

**Dosya sınırı:** frontend `Views/Records/Run.cshtml`, `Views/Records/FinishStep.cshtml`, `Views/Records/_Participants.cshtml`, `Views/Breaks/**`,
`Views/Shared/_PinInput.cshtml`, `Styles/**`, `Docs/Design/Dna.md` (bileşen notu), FE 9 için yalnız select işaretlemesi olarak Faz 1-4 görünümleri,
`Views/MyWork/**`, `wwwroot/js/LiveTimer.js`, `wwwroot/js/ParticipantPicker.js`; backend `Controllers/RecordsController.cs`, `Controllers/BreaksController.cs`,
adım, görevli ve mola servisleri, `Services/DurationCalculator.cs`, `ViewModels/Records/**`, `ViewModels/Breaks/**`; `Controllers/RecordStepsController.cs`
(adım ve görevli action'ları, route'lar değişmez) ve `Program.cs`'te yalnız `RecordSteps` ile `Breaks` için `/Views/Records/` arama konumu (PL-69).

**Kabul kriterleri**

- [ ] K1: Sıradaki adım belirgin, sonraki adımlar kilitli ve nedeni yazılı; 2. adım bitmeden 3. adımı başlatma isteği reddedilir (SC-18, BR-14); bir adım devam ederken başka adım başlatılamaz (BR-15).
- [ ] K2: Görevli olmayan operatör yürütme ekranını görür, başlat ve bitir düğmesi görmez; doğrudan POST reddedilir, adım durumu değişmez (SC-09, BR-16).
- [ ] K3: Sahip görevliyi kendi PIN'iyle ekler; olay ekleyeni ve eklenen kişiyi taşır; yanlış PIN'de kişi eklenmez ve hatalı deneme sahibin sayacına yazılır. Eklenen kişinin PIN onayı (SC-32) Faz 7 K24'tedir (PL-57).
- [ ] K4: Pasif ya da açık molası olan kişi eklenemez (PIN'i kilitli kişi Faz 7 K25), nedeni yazılır; adım başladıktan sonra ekleme ve çıkarma reddedilir; sahip olmayan görevlinin ekleme isteği reddedilir (BR-53, SK-2).
- [ ] K5: Sahip kendini bir adımdan çıkarır ve o adımı yine başlatıp bitirir; efor yalnız listedeki görevlilere yazılır (SC-19).
- [ ] K6: Görevlilerden biri başka kayıtta devam eden adımdaysa adım başlamaz, mesaj kişiyi ve kayıt numarasını gösterir; iki eşzamanlı başlatmada kişi tek devam eden adımda kalır (SC-05, BR-24).
- [ ] K7: Kendi kaydı Devam ediyor olan kişi başka kayıtta görevli eklenirken K-41 uyarısı görünür ve ekleme yapılabilir.
- [ ] K8: Malzeme zorunlu tipte malzemesiz ilk adım başlatma reddedilir ve olay üretmez (SC-07, BR-11).
- [ ] K9: Planlı kayıtta ilk adım başlayınca saha referansı `<TesisKodu>-SD-<Yıl>-<Sıra>` üretilir ve ekranda görünür, kayıt "Devam ediyor" olur; kilit bitişi geçmişse ilk adım başlamaz ve kayıt Zaman aşımı olur (BR-09, BR-46).
- [ ] K10: Molasız, iki görevlili adımda adım süresi = bitiş - başlangıç, efor = süre x 2; QA değerleri veritabanındaki zamanlarla karşılaştırır (SH-01, SH-02).
- [ ] K11: Fazın son adımı, faz net süresi minimumun altındayken gerekçesiz bitirilemez ve istek olay üretmez; gerekçeyle bitirince faz "Tamamlandı - süre altında" olur, bitiş zamanı gerekçeli isteğin sunucu saatidir, gerekçe ekranda görünür (SC-30, BR-21).
- [ ] K12: Son fazın son adımı bitince kayıt "Tamamlandı" olur, tamamlanma zamanı yazılır, makine panosunda makine Boş görünür (BR-22, SC-01 yürütme kısmı).
- [ ] K13: Mola "Devam ediyor" kayıtta iki adım arasında başlatılır; adım devam ederken ve "Açıldı" kayıtta mola düğmesi yoktur ve istek reddedilir (adım sürerken mola Faz 7 K26). Sahip ya da sıradaki adımın görevlisi başlatabilir; kayıtta görevi olmayan operatörün isteği reddedilir (BR-38, SK-6).
- [ ] K13a: Mola açıkken kayıtta adım başlatma isteği "Kayıtta açık mola var" ile reddedilir, olay üretmez; görevli ve malzeme istekleri reddedilir (BR-39, SC-26). Devam eden adımı bitirme reddi Faz 7 K27.
- [ ] K14: Molayı başlatan kişinin başka kayıtta adım başlatma, yeni kayıt açma, görevli ekleme ve malzeme istekleri reddedilir ve `/breaks/current`'a yönlenir; molayı bitirebilir ve oturumu kapatabilir; reddedilen istekler olay üretmez (SC-26).
- [ ] K14a: Başka bir kayıtta devam eden adımda görevli olan kişinin mola başlatma isteği reddedilir (BR-38, SC-37 ek koşul).
- [ ] K15: Molayı başlatandan başka operatörün mola bitirme isteği reddedilir (BR-40). İkinci cümle (adım sürerken açılan mola) Faz 7 K28.
- [ ] K16: Mola süresi faz ve kayıt net süresinden düşülür, brüt süreden düşülmez; mola verilmeden geçen boşluk net süreye dahildir (SC-06 a ve b, SC-25). Ö-03: QA SC-06 zamanlarını (1. adım 10:00:00-10:00:12, 2. adım 10:40:12'de
  başlar, `d` sonra biter) `DurationCalculator` formülüne uygular: molasız faz net 40 dk 12 sn + `d`; arada 40 dk mola verilmişse faz net 12 sn + `d`;
  minimum 15 dk ile ikinci durumda son adım gerekçe ister. Aynı iki durumu minimumu 1 dk olan QA tipiyle ve kısa sürelerle çalışan uygulamada da dener.
- K16a (Faz 7 K29'a taşındı, PL-57): Adım sürerken verilen mola adım süresinden ve eforundan düşülür. QA iki görevlili bir adımda adım sürerken mola verip bitirir; veritabanındaki zamanlarla adım molası = mola süresi, adım süresi = bitiş - başlangıç - adım molası, efor = adım süresi x 2 olduğunu gösterir. Ayrıca QA, SC-37 zamanlarını (adım 10:00:00-10:50:00, mola 10:04:00-10:44:00, iki görevli) `DurationCalculator` kodundaki formüle uygular ve 40 dk, 10 dk, 20 dk bulur (SH-17, SH-01, SH-02).
- K16b (Faz 7 K30'a taşındı, PL-57): Adım içi mola içeren fazda faz net = faz brüt - faz molası ve bu değer fazın adım süreleri toplamı ile adımlar arasındaki molasız boşlukların toplamına eşittir; faz minimumu bu değerle kontrol edilir (SH-06, SH-07).
- [ ] K17: Canlı sayaç sayfa yenilenince sunucudan gelen değerle başlar; JS dosyalarında tarih farkı hesabı yoktur (SH-14). Kayıtta mola açıkken faz ve kayıt sayaçları ilerlemez ve ekranda metinle "Sayaçlar durdu" görünür; mola bitince sayfa yenilendiğinde kaldığı değerden devam eder.
- [ ] K18: Tamamlanmamış adımda görevli ya da açık molası olan kullanıcı pasifleştirilemez; ekran bu işleri bağlantıyla listeler (BR-36).
- [ ] K19: QA kopyasında dolu adım başlangıç zamanı SQL UPDATE ile değiştirilemez (BR-28).
- [ ] K20: `IDurationCalculator`'da SH-17, SH-01, SH-02, SH-03, SH-04, SH-06, SH-07, SH-08, SH-09, SH-11, SH-12, SH-14 kimliklerinin her biri formülüyle XML yorumunda bulunur.
- [ ] K21: Adımın fotoğrafı ve videosu yürütme ekranında açılır; yürütme ekranı 360 px'te yatay kaydırmasız, başlat ve bitir 44 px (FR-06, NFR-04).
- [ ] K22 (Ö-02): QA kaydı açar, bir süre bekler, ilk adımı başlatır; kayıt ve faz brütü ilk adım başlangıcından ölçülür, açılış anı ayrı alanda
  görünür, açılış ile ilk adım arasındaki süre hiçbir süreye girmez (BR-13, SH-03, SH-08; doküman satır 45-47).
- [ ] K23 (Ö-04, SC-03): Sahip adım aralarında oturumu kapatır, yeniden girer; kayıt "Devam ediyor" kalmıştır, İşlerim'de görünür, sıradaki adımdan
  devam edilir; bu sürede makine panosunda makine kilitlidir ve başka operatör o makinede kayıt açamaz (doküman satır 78-81).
- [ ] K24 (PL-65): Demo veride `KSA` tipi vardır: 2 faz, ikişer adım, faz minimumu 60 saniye, malzeme zorunlu değil, demo makinelerinde yeni kayıt formunda seçilir; PLN, ACL, PLN3 değişmemiştir; `DemoData.sql` ikinci kez uygulanınca satır sayıları değişmez.
- [ ] K25 (PL-66): Seed değeri `<PinUzunluğu>` 4 ile Faz 1-5'teki her PIN alanına (doğrulama ve belirleme modu) 5. hane ve rakam dışı karakter
  girilemez, yapıştırmada da; QA kopyasında ayar 6 yapılınca 6 hane girilir; kaynak kodda ve görünümde PIN uzunluğu sabiti yok. Sunucu tarafı
  PIN kuralları (K3 Faz 1, BR-49) değişmemiştir.
- [ ] K26 (PL-66): PIN göz düğmesi ve tüm select'ler Faz 1-5 ekranlarında 360, 768, 1024 ve 1440 px'te, açık ve koyu temada: select yüksekliği aynı
  satırdaki `.input` ile eşit, ok ikonu dikey ortalı (G12); göz düğmesi alanın kenarlık içinde, köşesi alanla uyumlu; hover ve odak hali görünür,
  kontrast AA; `prefers-reduced-motion` açıkken geçiş yok; dokunma hedefi en az 44 px; pasif select ayırt edilir. Faz 1-4 G11 imzaları değişmez.
- [ ] K27 (PL-70): JS açıkken fazın son adımı süre altındayken "Bitir" yürütme ekranında dialog açar; dialog faz adını, net süreyi ve minimumu
  gösterir; gerekçesiz ve yanlış PIN'li gönderim dialog içinde alan hatasıyla reddedilir, adım "Devam ediyor" kalır, olay yazılmaz; gerekçe ve
  doğru PIN'le faz "Tamamlandı - süre altında" olur, aynı `CommandId` tek etki üretir (G9). Dialog `<dialog>` ve erişilebilir adlı, açılışta odak
  içeride, Tab dışarı çıkmaz, Esc kapatır, kapanışta odak "Bitir"e döner; 360 px'te yatay kaydırma yok, düğmeler 44 px; G5a (dialogdaki form
  belirteci), G12. JS kapalıyken aynı akış `FinishStep` sayfasıyla çalışır (K11).

**Faz 4'ten devreden (PL-67):**

- [ ] F4-A1 (QA): Malzeme zorunlu tipte malzemesiz açılmış kaydın ilk adımı başlatılamaz (K8 ile aynı ölçüm; doküman satır 22-23).
- [ ] F4-A2 (BE, FE): Yürütme ekranında `AvailableLots` ve `/records/lots?q=&recordId=` kayda zaten eklenmiş lotu listelemez; istekle gönderilirse
  yine nedenle reddedilir.

**Faz T'den devreden (PL-23):**

- [ ] T-B20 (FE): Mola panelindeki ve mola ekranındaki "Molayı bitir" 320-1920 px'te tek satır ve 56 px yüksekliğindedir; adım eylem düğmeleriyle aynı ızgarayı kullanır.

**Kapsam dışı:** Yöneticinin molayı bitirmesi ve kayıt detayı (Faz 6), beklemeye alma, adım sürerken mola ve görevli PIN onayı (Faz 7, PL-57), kişi düzeyi mola ve adımın ayrıca duraklatılması (K-39, K-21).

**Sonuç (2026-11-15):** TESLİM ALINDI (tur 1). Kabul kriterleri 46/46; F4-A1, F4-A2 kapandı. DÜŞÜK B1 (canlı sayaç yuvarlaması), B2 (koyu tema
dialog perdesi) ve A1 (açık iş bağlantıları) Faz 6'ya devredildi (PL-71). Rapor: `Docs/Delivery/Faz-5-Teslim.md`.

---

## Faz 6: Kayıt listesi, detay, bütünlük ve müdahale

**Amaç:** Kayıtların aranması, tek kayıtta 13 temel sorunun yanıtlanması ve denetim çıktısı olarak yazdırılması, bütünlüğün doğrulanması; yöneticinin kaydı gerekçe ve
PIN'le iptal etmesi, yarıda kapatması ve açık molayı bitirmesi (O-03, PL-46).

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Liste filtreleri ve lot no araması için index'ler (backend'in sorgu listesine göre); yeni tablo yok. Müdahale alanları Faz 4 kayıt tablosunda (kapanış türü, kapatan, gerekçe, kapanış anı), mola kapanış alanları Faz 5 mola tablosunda önden gelir | backend-engineer talebi, dbadmin |
| 2 | BE | Kayıt sorgu servisi: filtre, arama (kayıt no, saha referansı, iş emri, lot no; Ö-06), sunucu sayfalama; Operatör'de varsayılan "benim kayıtlarım" (sahip ya da görevli) (K-17, PL-08) | backend-engineer |
| 3 | BE | Detay sorgusu: Brief §4.3'teki 13 soru, açılış anı değerleri, versiyon içeriği (FR-09), zaman çizelgesi, olay günlüğü, kapanış bilgisi | backend-engineer |
| 4 | BE | Bütünlük doğrulama: (1) özet zinciri, (2) durum tablolarındaki zaman, kişi ve malzeme alanlarının olay değerleriyle aynı olması; ilk tutarsız olay sırası (PL-11, SC-21) | backend-engineer |
| 5 | BE | Müdahale servisi (BR-27, BR-40): iptal (Açıldı ya da Devam ediyor kayıtta) ve yarıda kapatma (Devam ediyor kayıtta); gerekçe ve PIN zorunlu, PIN doğrulaması ve iş eylemi aynı veritabanı işleminde. Aynı işlemde: devam eden adım Kesildi, bitişi müdahale anı, adım molası ve süresi SH-17 ve SH-01 ile; tamamlanmamış fazlar Kesildi; açık mola müdahale anında "Müdahale" türüyle kapanır; görevlilerin çalışıyor bayrağı kalkar; makine kilidi kalkar; kapanış türü, kapatan, gerekçe ve anı kayda yazılır. Kayıt silinmez, sahip değişmez, kayıt no yeniden kullanılmaz. Yönetici açık molayı gerekçe ve PIN'le bitirir, bitiş işlem anıdır; adım "Devam ediyor" kalır. Zaman aşımı kuralı (BR-26) müdahaleden önce aynı işlemde uygulanır | backend-engineer |
| 6 | BE | Ana sayfa Yönetici ve Denetçi'yi `/records`'a yönlendirir (Faz 7'de `/dashboard` olur); `PrototypeController` ve prototip görünümleri kaldırılır (PL-10) | backend-engineer, frontend-engineer |
| 7 | FE | E-19 Kayıtlar, E-20 Kayıt detayı (zaman çizelgesi, görevliler ve onay zamanları, molalar, malzemeler, olay günlüğü, bütünlük rozeti, kapanış bilgisi; yazdırma katmanı: menü, üst bar, alt çubuk ve eylem düğmeleri gizli, çıktı denetim çıktısı içeriğini taşır, Ö-07, PL-56), E-21 Bütünlük doğrulama, E-26 Müdahale formu (iptal, yarıda kapatma) ve mola bitirme formu; detayda "Müdahale et" ve "Molayı bitir" yalnız Yönetici'de | frontend-engineer |
| 8 | BE | İlk iş, Faz 5'ten devreden F5-B1 (PL-71): `CalculateLiveElapsed` farkı tick düzeyinde alıp tek kez yuvarlar, XML yorumu kodla aynı. F5-A1: pasifleştirme ekranındaki açık işlerde kayıt bağlantısı görüntüleyenin erişebildiği detaya gider (PL-16) | backend-engineer |
| 9 | FE | İlk iş, Faz 5'ten devreden F5-B2 (PL-71): dialog perdesi açık ve koyu temada koyu kalan token (`Run.cshtml`, `_MediaDialog.cshtml`) | frontend-engineer |
| 10 | BE | Oturum sertleştirmesi (Faz 1 gözlemi, ORTA, PL-71): her girişte rastgele oturum kimliği claim'i (`VakaClaimTypes.SessionId`); çıkışta kimlik sunucu belleğinde iptal listesine `<OturumSüresi>` boyunca yazılır; çerez doğrulamasında iptal edilmiş kimlik reddedilir ve giriş sayfasına yönlenir. Şema değişmez. Sınır: uygulama yeniden başlayınca liste boşalır; kalıcı çözüm (kullanıcı satırında oturum sürümü) ALTER'dir, kullanıcı onayıyla Salı sonrası | backend-engineer |

**Paralel:** Evet. Gerekçe: veri yapısı Faz 4-5'te oluştu; müdahale yeni tablo istemez (kolonlar önden, PL-02); route, ViewModel ve rol aşağıda; dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/records` | `RecordsController.Index` | RecordReaders | `RecordListViewModel { Items[] { Id, RecordNo, Kind, KindLabel, StatusLabel, FieldBookRef?, FacilityName, LineName, MachineLabel, CleaningTypeName, VersionNo, OwnerName, OpenedAtLocal, CompletedAtLocal?, NetSeconds?, HasBelowMinimum, HasExpiredMaterial, Url }, Filter { Query, FacilityId?, LineId?, MachineId?, Kind?, Status?, From?, To?, MineOnly }, Facilities[], Lines[], Machines[], Page, PageSize, Total }`. `Query` kayıt no, saha referansı, iş emri ve lot no'da arar | `Views/Records/Index.cshtml` |
| GET `/records/details/{id}` | `RecordsController.Details` | RecordReaders | `RecordDetailViewModel { Header { RecordNo, Kind, KindLabel, StatusLabel, FieldBookRef?, Facility, Line, Machine, CleaningTypeName, VersionNo, VersionUrl?, WorkOrderNo, Note, EmergencyDescription? }, People { OwnerName, OwnerRegistryNo, FirstStepStartedByName?, Helpers[] }, Times { OpenedAtLocal, StartedAtLocal?, CompletedAtLocal?, ClosedAtLocal?, LockExpiresAtLocal? }, Summary { GrossSeconds?, BreakSeconds?, HoldSeconds?, NetSeconds?, EffortSeconds? }, Phases[] { OrderNo, Title, MinDurationSeconds?, StatusLabel, GrossSeconds?, BreakSeconds?, NetSeconds?, BelowMinimum, BelowMinimumReason, Steps[] { OrderNo, Title, Description, Media[], StatusLabel, StartedAtLocal?, StartedByName?, EndedAtLocal?, EndedByName?, BreakSeconds?, DurationSeconds?, EffortSeconds?, Participants[] { FullName, RegistryNo, AddedByName, AddedAtLocal, ConfirmedAtLocal? (yalnız gerçek onay, Faz 7; PL-72) } } }, Breaks[] { StartedByName, StartedAtLocal, EndedAtLocal?, DurationSeconds?, EndKindLabel, EndedByName?, EndReason, DuringStepTitle? }, Materials[] { MaterialCode, MaterialName, LotNo, ExpiryDate, ExpiredAtEntry, ExpiredReason, EnteredByName, EnteredAtLocal }, Events[] { Sequence, OccurredAtLocal, ActorName, EventLabel, Summary }, Integrity { IsValid, FirstBrokenSequence?, CheckedAtLocal, ReportUrl? }, Closure? { KindLabel, ClosedByName, ClosedAtLocal, Reason }, OpenBreak? { BreakId, StartedByName, StartedAtLocal, DuringStepTitle? }, CanIntervene, InterventionUrl?, CanEndBreak, EndBreakUrl?, RunUrl?, GeneratedAtLocal, GeneratedByName }`. `GeneratedAtLocal` ve `GeneratedByName` yazdırılan çıktıda görünür | `Views/Records/Details.cshtml` |
| GET `/records/integrity/{id}` | `RecordsController.Integrity` | ManagerOrAuditor | `IntegrityReportViewModel { RecordId, RecordNo, IsValid, CheckedAtLocal, Events[] { Sequence, EventLabel, OccurredAtLocal, HashShort, IsValid, Problem? }, StateMismatches[] { Field, StoredValue, EventValue, Sequence } }` | `Views/Records/Integrity.cshtml` |
| GET, POST `/interventions/create?recordId=&type=` | `InterventionsController.Create` | Manager | `InterventionViewModel { CommandId, RecordId, RecordNo, MachineLabel, OwnerName, RecordStatusLabel, Type?: InterventionType, AllowedTypes[] { Value, Label }, Reason, Pin, InProgressStepTitle?, OpenBreak? { StartedByName, StartedAtLocal }, BlockReason? }`; başarıda `/records/details/{id}` | `Views/Interventions/Create.cshtml` |
| GET, POST `/interventions/end-break/{breakId}` | `InterventionsController.EndBreak` | Manager | `EndBreakViewModel { CommandId, BreakId, RecordId, RecordNo, StartedByName, StartedAtLocal, DuringStepTitle?, Reason, Pin, BlockReason? }`; başarıda `/records/details/{id}` | `Views/Interventions/EndBreak.cshtml` |

`InterventionType` (`Services/`): `Cancel = 1`, `Abort = 2`; görünür adları İptal, Yarıda kapatma. `Hold = 3` Faz 7'de eklenir.
`AllowedTypes`: Açıldı kayıtta yalnız İptal, Devam ediyor kayıtta İptal ve Yarıda kapatma; diğer durumlarda boş ve `BlockReason` dolu.
Formun alanı göstermesi yetmez; tür, durum ve gerekçe serviste yeniden denetlenir. Makine panosundaki kayıt bağlantısı Yönetici ve Denetçi'de detaya gider (Faz 4 sözleşmesi).

**Dosya sınırı:** frontend `Views/Records/Run.cshtml` ve `Views/Shared/_MediaDialog.cshtml` (yalnız F5-B2), `Views/Users/Deactivate.cshtml` (F5-A1 bağlantısı gerekirse), `Views/Records/Index.cshtml`, `Views/Records/Details.cshtml`, `Views/Records/Integrity.cshtml`, `Views/Interventions/**`,
`Views/Prototype/**` (kaldırma), `Styles/**` (yazdırma katmanı); backend `Controllers/RecordsController.cs` (okuma action'ları),
`Controllers/InterventionsController.cs`, `Controllers/HomeController.cs`, `Controllers/PrototypeController.cs` (kaldırma), `Controllers/AccountController.cs`,
`Authorization/**` ve `Program.cs` (oturum sertleştirmesi), `Services/DurationCalculator.cs` (F5-B1), açık iş sağlayıcısı (F5-A1), sorgu, bütünlük ve
müdahale servisleri, `ViewModels/Records/**`, `ViewModels/Interventions/**`.

**Kabul kriterleri**

- [ ] K1: Tamamlanmış planlı kayıtta Brief §4.3'teki 13 soru detayda yanıtlıdır; QA her birini ayrı işaretler (FR-31, SC-01).
- [ ] K2: "Kaydı açan (sorumlu)" ile "İlk adımı başlatan" ayrı satırlardır (K-35); süreler brüt, mola, net ve efor olarak ayrı etiketlidir (FR-21). Adım içi mola gösterimi (SC-37) Faz 7 K31'de (PL-57).
- [ ] K3: v2 yayınlandıktan sonra v1 kaydının detayı v1'in faz minimumunu, adım açıklamasını ve medyasını gösterir (SC-11, FR-09).
- [ ] K4: Makine adı sonradan değiştirildiğinde eski kayıt açılış anındaki adı gösterir (BR-06).
- [ ] K5: Operatör `/records`'u yalnız sahibi ya da görevlisi olduğu kayıtlarla açar; filtreyi kaldırınca tüm kayıtları görür (PL-08); tesis, hat, makine, tür, durum, tarih aralığı ve kayıt no, saha referansı, iş emri, lot no araması birlikte çalışır; lot no ile arama o lotun eklendiği kayıtları getirir (Ö-06); sayfalama sunucuda.
- [ ] K6: Olay günlüğü tüm olayları sırayla, yapan kişi ve sunucu zamanıyla listeler (FR-34, BR-30).
- [ ] K7: Değiştirilmemiş kayıtta rozet "Doğrulandı" yazar. QA kopyasında kaydın iş emri ya da bir malzeme lot değeri SQL ile değiştirilince rozet "Bütünlük ihlali" olur ve doğrulama ekranı alanı, saklanan ve olaydaki değeri gösterir; olay değeri değiştirilmiş kopyada ilk tutarsız olay sırası gösterilir (SC-21, PL-11).
- [ ] K8: `/records/integrity/{id}` Yönetici ve Denetçi'de açılır, Operatör'de 403 (SC-24). Pano kısmı Faz 7 K18'e taşındı (PL-46).
- K9, K10, K12: Faz 7'ye taşındı (K19, K20, K21; PL-46).
- [ ] K11: Yönetici ve Denetçi `/` adresinde `/records`'a yönlenir; makine panosundaki kayıt bağlantısı onlarda detayı açar. Yalnız Süper admin rolündeki kullanıcı `/records`, `/records/details/{id}` ve `/interventions/*` için 403 alır.
- [ ] K13: `Views/Prototype` ve `PrototypeController` repoda yoktur.
- [ ] K14: Detayın yazdırma önizlemesinde menü, üst bar, alt çubuk ve eylem düğmeleri görünmez; 13 sorunun yanıtları, olay günlüğü, bütünlük rozeti ve sayfanın oluşturulma anı çıktıdadır; A4 dikey sayfada yatay taşma yoktur (Ö-07). Çıktı denetim çıktısı içeriğini taşır (eski Faz 9 K6, PL-56): 13 soru, olay günlüğü, bütünlük
  sonucu, versiyon no ve doküman no, SKT işaretleri ve gerekçeleri, süre altı gerekçeleri, kapanış bilgisi, oluşturan kişi ve an (SC-14, FR-35).
  Düzeltmeler Faz 8'de, ayrı `/records/audit/{id}` adresi ve CSV Faz 9'da eklenir.
- [ ] K15 (eski Faz 7 K8): İptalde kayıt "İptal edildi", devam eden adım "Kesildi" ve bitişi müdahale anı, açık mola kapanış türü "Müdahale" ile kapanır, makine Boş olur; kayıt silinmez, kayıt no yeniden kullanılmaz (BR-27, SC-10). Adım sürerken açık molalı iptal Faz 7 K32'de (PL-57). Açıldı kayıt iptal edilebilir.
- [ ] K16 (eski Faz 7 K9): Yarıda kapatmada Devam ediyor kayıt "Yarıda kapatıldı", makine Boş olur; Açıldı kayıtta yarıda kapatma sunulmaz ve istekle gönderilirse reddedilir.
- [ ] K17 (eski Faz 7 K13): Yönetici açık molayı gerekçe ve PIN'le bitirir; bitiş işlem anıdır; mola satırında kapanış türü "Yönetici" ve gerekçe görünür (BR-40). Molayı başlatan kişinin yazma kilidi kalkar. Adım sürerken açılmış mola Faz 7 K33'te (PL-57).
- [ ] K18 (eski Faz 7 K2, müdahale kısmı): Operatör hiçbir ekranda kaydı kendisi iptal etme ya da kapatma düğmesi görmez; `/interventions/*` Operatör ve Denetçi'de GET ve POST'ta 403; Denetçi detayda müdahale ve mola bitirme bağlantısı görmez (K-05).
- [ ] K19: Gerekçesiz ya da yanlış PIN'li müdahale ve mola bitirme kaydı değiştirmez ve olay üretmez; yanlış PIN hatalı deneme sayılır. Tamamlandı, Zaman aşımı, İptal edildi ve Yarıda kapatıldı kayıtta müdahale reddedilir. Kilit bitişi geçmiş Açıldı kayıtta iptal isteği kaydı Zaman aşımı yapar (zaman aşımı olayı, anı kilit bitişi) ve iptal "Kayıt zaman aşımına düştü" nedeniyle reddedilir (BR-26).
- [ ] K20 (eski Faz 7 K15): SC-16 uçtan uca: yönetici kaydı yarıda kapatır, diğer kaydın sahibi kişiyi henüz başlamamış adımdan çıkarır, ardından pasifleştirme yapılır; sahip hiçbir aşamada değişmez.
- [ ] K21 (eski Faz 7 K16, müdahale kısmı): Olay günlüğünde iptal, yarıda kapatma ve yöneticinin mola bitirme olayları yapan, gerekçe ve zamanla yer alır (BR-30); bütünlük rozeti müdahaleden sonra "Doğrulandı" kalır.
- [ ] K22 (F5-B1, F5-B2, F5-A1): Kayıtta açık mola varken yürütme ekranı art arda yenilendiğinde faz ve kayıt sayaçları aynı değeri gösterir;
  `IDurationCalculator` SH-14 yorumu kodla aynıdır. Koyu temada dialog perdesi koyu, içerik kontrastı AA. Pasifleştirme ekranındaki açık işler
  Yönetici'de kayıt detayına bağlantılıdır, Süper admin'de bağlantısızdır.
- [ ] K23 (PL-71): "Kullanıcı değiştir" ile çıkıştan sonra, çıkıştan önce alınmış oturum çereziyle yapılan istek (ör. `/my-work`) giriş
  sayfasına yönlenir; aynı kullanıcının başka tarayıcıdaki oturumu etkilenmez. Uygulama yeniden başlatıldıktan sonraki davranış raporda not edilir.

**Kapsam dışı:** Ayrı denetim çıktısı adresi E-32 ve CSV (Faz 9), acil müdahale ve raporlar (Faz 6A), yönetici panosu ve SH-16 (Faz 7), beklemeye alma ve talepler (Faz 7).

**Sonuç (2026-11-16):** TESLİM ALINDI (tur 2; tur 1 İADE B1 ORTA, onaysız görevlide "onay" gösterimi, PL-72). Kabul kriterleri 34/34;
F5-B1, F5-B2, F5-A1 ve oturum sertleştirmesi kapandı; prototip sayfaları kaldırıldı (PL-10). Açık: kalıcı oturum iptali Faz 7 kalem 9.
Rapor: `Docs/Delivery/Faz-6-Teslim.md`.

---

## Faz 6A: Acil müdahale, süre raporu ve malzeme izlenebilirliği (Salı öncesi)

**Amaç:** Dokümanın Salı öncesi tamamlanması (O-04, PL-56): acil işin ayrı akışla açılıp başlatılması ve kayıtlarda ayrılması (doküman satır
41-44), makine ve aşama bazında süre ve efor raporu (7, 8, 64, 100), lot ve makine bazında malzeme sorgusu (24, 117-118). Bildirim, grafik ve
CSV Salı sonrasıdır (Faz 8, 9).

**Başlama şartı:** Faz 6 TESLİM ALINDI ve commit'i atıldı.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Rapor ve lot sorguları için index'ler (backend'in sorgu listesine göre); yeni tablo yok. Acil açıklaması kolonu Faz 4 kayıt tablosunda önden gelir | backend-engineer talebi, dbadmin |
| 2 | BE | Acil açma servisi (eski Faz 8 kalem 2): yalnız Acil türü, eşleme aranmaz, açıklama zorunlu, zorunlu malzeme aynı formda, PIN; açılış ve ilk adım tek transaction ve aynı sunucu anı; kayıt no `A` türü, saha referansı yok; makine ve kişi kısıtları aynen (BR-50, K-44). Bildirim yok; Faz 8 bu servise bildirim ekler. İşlerim'e `CanOpenEmergency` | backend-engineer |
| 3 | BE | Süre raporu sorgusu (eski Faz 9 kalem 2): makine ve faz bazında ortalama brüt, mola, net, efor; yalnız Tamamlandı kayıtlar, faz ortalaması versiyon içinde (SH-15, `IDurationCalculator`); durum sayıları ayrı; süre altı sayısı; en uzun adımlar (FR-40); tür filtresi. Bekleme ortalaması Faz 7'de eklenir | backend-engineer |
| 4 | BE | Malzeme izlenebilirliği sorgusu (eski Faz 9 kalem 3): lotla ve makineyle (FR-37). Düzeltilmiş işareti Faz 8'de eklenir | backend-engineer |
| 5 | FE | E-27 Acil müdahale (tek ekran; İşlerim'de "Acil müdahale" düğmesi), E-30 Süre ve efor raporu (yalnız tablo), E-31 Malzeme izlenebilirliği; liste ve detayda metinli "Acil" etiketi (Faz 6 `KindLabel`); Yönetici ve Denetçi menüsünde Raporlar | frontend-engineer |

**Paralel:** Evet. Gerekçe: acil yeni tablo istemez (kolonlar Faz 4'te önden), Faz 4 ve 5 servisleri yeniden kullanılır; raporlar salt okuma;
route, ViewModel ve rol aşağıda; dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET, POST `/emergencies/create` | `EmergenciesController.Create` | Operator | `EmergencyCreateViewModel { CommandId, FacilityId?, LineId?, MachineId?, CleaningTypeId?, Description, Materials[] { MaterialLotId, ExpiredReason }, Pin, Facilities[], Lines[], Machines[] { Id, Label, IsAvailable, BusyReason }, EmergencyTypes[] { Id, Name, VersionNo, MaterialRequired }, ErrorMessage, ErrorUrl? }`; başarıda `/records/run/{id}` | `Views/Emergencies/Create.cshtml` |
| GET `/emergencies/machines?lineId=` | `EmergenciesController.Machines` | Operator | JSON `[{ id, label, isAvailable, busyReason }]` | - |
| GET `/reports/durations` | `ReportsController.Durations` | ManagerOrAuditor | `DurationReportViewModel { Filter { From?, To?, FacilityId?, LineId?, MachineId?, CleaningTypeId?, Kind? }, ByMachine[] { MachineLabel, CleaningTypeName, RecordCount, AvgGrossSeconds, AvgBreakSeconds, AvgNetSeconds, AvgEffortSeconds }, ByPhase[] { CleaningTypeName, VersionNo, PhaseOrderNo, PhaseTitle, MinDurationSeconds?, RecordCount, AvgNetSeconds, BelowMinimumCount }, LongestSteps[] { CleaningTypeName, VersionNo, PhaseTitle, StepTitle, RecordCount, AvgDurationSeconds, MaxDurationSeconds }, StatusCounts[] { StatusLabel, Count }, Facilities[], Lines[], Machines[], CleaningTypes[] }`. `AvgHoldSeconds` Faz 7'de eklenir | `Views/Reports/Durations.cshtml` |
| GET `/reports/materials` | `ReportsController.Materials` | ManagerOrAuditor | `MaterialTraceViewModel { Mode (Lot, Machine), LotQuery, MaterialLotId?, MachineId?, From?, To?, LotUsages[] { RecordId, RecordNo, MachineLabel, OpenedAtLocal, StatusLabel, ExpiredAtEntry, Url }, MachineMaterials[] { EnteredAtLocal, RecordId, RecordNo, MaterialCode, MaterialName, LotNo, ExpiryDate, ExpiredAtEntry, Url } }`. `IsCorrected` Faz 8'de eklenir | `Views/Reports/Materials.cshtml` |
| Mevcut ekranlara ek | `MyWork` | Operator | `MyWorkViewModel.CanOpenEmergency` | `Views/MyWork/Index.cshtml` |

Sözleşme ekleri (PL-73, paralel başlangıç için):

- Ad alanları `Vaka.Web.ViewModels.Emergencies` ve `Vaka.Web.ViewModels.Reports`. Seçenek listeleri Faz 2 `SelectOptionViewModel`; makine
  seçenekleri Faz 4 `MachineOptionViewModel`; lotlar Faz 4 `LotOptionViewModel`. Enum `Services/MaterialTraceMode { Lot = 1, Machine = 2 }`;
  `Kind` Faz 3 `CleaningTypeKind`, süzgeçte `KindOptions[]`.
- Acil formu Faz 4 yeni kayıt düzenindedir (PL-62, PL-64): `CommandForm.js`, JS kapalıyken tesis, hat, makine ve lot seçimi `refresh=true`
  POST'uyla (`AvailableLots`, `LotQuery`, `AddMaterialLotId`, `AddExpiredReason`, `RemoveMaterialIndex`); ret 200 görünüm, girilen alanlar ve
  `CommandId` korunur. `EmergencyCreateViewModel`'e `LotQuery?`, `AvailableLots[]`, `AvailableLotsTruncated` eklenir. Faz 4 `RecordRunViewModel`'e
  `EmergencyDescription?` eklenir, acil kayıtta yürütme başlığında görünür.
- `MaterialTraceViewModel`'e lot modunda `LotOptions[]: LotOptionViewModel` (en çok 50, `LotQuery` ile) ve `Machines[]: SelectOptionViewModel`
  eklenir; JS kapalıyken lot önce `LotQuery` GET'iyle bulunur, sonra `MaterialLotId` ile sorgulanır.
- `From?` ve `To?` `DateOnly?`, Türkiye tarihine göre gün dahil; tarih süzgeci süre hesabı değildir, servis sorgusundadır.
- Menü: Yönetici ve Denetçi'de "Raporlar" grubu (Süre ve efor, Malzeme izlenebilirliği); Operatör'ün İşlerim ekranında "Acil müdahale".
- DB: yeni tablo yok. Mevcut tablolara index yalnız ölçüm gerektirirse talep edilir; tekil olmayan index de mevcut tabloya eklendiği için O-07
  düzeninde kullanıcı onayı ister (DbProtocol netleşene kadar). Salı verisi küçük olduğundan index'siz başlanır (K-33).
- Ortak kriterler G5a, G5b, G8, G9, G11, G12 bu fazın ekranlarına uygulanır.

Raporların süzgeçleri GET sorgu dizesiyle sunucuda çalışır; sayfalama yoktur (ölçek sayıları gelince ölçülür, K-33). `Url` kayıt detayına gider.
Acil kayıt yürütme ekranında Faz 5 akışıyla sürer; acil tipte faz minimumu yoktur.

**Dosya sınırı:** frontend `Views/Emergencies/**`, `Views/Reports/**`, `Views/MyWork/**` (acil düğmesi), menü, `wwwroot/js/CascadeSelect.js` ve
`wwwroot/js/LotPicker.js` (yeniden kullanım); backend `Controllers/EmergenciesController.cs`, `Controllers/ReportsController.cs`, acil, rapor ve
izlenebilirlik servisleri, `Services/DurationCalculator.cs` (SH-15), `ViewModels/Emergencies/**`, `ViewModels/Reports/**`, `MyWorkViewModel` (tek alan).

**Kabul kriterleri**

- [ ] K1 (eski Faz 8 K1): Acil müdahale ekranında yalnız Acil türünde, yürürlükte versiyonu olan tipler listelenir; makine eşlemesi aranmaz (BR-50).
- [ ] K2 (eski Faz 8 K2): Gönderimde kayıt açılır ve ilk adım başlar; açılış zamanı ile ilk adım başlangıcı aynı değerdir; kayıt no `...-A<Yıl>-<Sıra>`; saha referansı alanı "yok" yazar (SC-02).
- [ ] K3 (eski Faz 8 K3): Açıklama boşken kayıt açılmaz; malzeme zorunlu acil tipte malzemesiz gönderim reddedilir (SC-02, BR-11).
- [ ] K4 (eski Faz 8 K4): Makine kilitliyse ya da operatörün aktif kaydı varsa acil açılmaz; mesaj kilidi tutan kaydı, sahibini ve durumunu gösterir (K-44).
- [ ] K5 (eski Faz 8 K5): Acil kayıtta fazın son adımı gerekçe istenmeden bitirilir (minimum yok).
- [ ] K6 (eski Faz 8 K8, liste ve detay): Liste ve detayda acil kayıt metinli "Acil" etiketiyle ayrılır; liste türe göre filtrelenir (FR-15, BR-10). Panodaki kısım Faz 8'de.
- [ ] K7 (eski Faz 8 K14): Acil müdahale ekranı 360 px'te yatay kaydırmasız, gönder düğmesi 44 px. Aynı `CommandId` ile iki kez gönderim tek kayıt üretir (G9).
- [ ] K8 (eski Faz 9 K1): Süre raporu makine ve faz bazında ortalama brüt, mola, net ve eforu ayrı kolonlarda gösterir (FR-21, FR-36).
- [ ] K9 (eski Faz 9 K2): Ortalamalara yalnız Tamamlandı kayıtlar girer; İptal, Yarıda kapatıldı ve Zaman aşımı sayıları ayrı gösterilir; QA bir iptal kaydının ortalamayı değiştirmediğini ve iki tamamlanmış kaydın ortalamasının veritabanındaki net sürelerden elle hesaplanan değerle aynı olduğunu gösterir (SH-15).
- [ ] K10 (eski Faz 9 K3): Aynı tipin v1 ve v2 faz ortalamaları ayrı satırdır; süre altı faz sayısı faz satırında görünür (FR-22).
- [ ] K11 (eski Faz 9 K4): En uzun süren adımlar listelenir (FR-40); planlı ve acil kayıtlar tür filtresiyle ayrılır (FR-15).
- [ ] K12 (eski Faz 9 K5): Lotla arama lotun kullanıldığı kayıtları makine, tarih ve kayıt no ile; makineyle arama malzeme ve lotları tarih sırasıyla listeler; SKT geçmiş satırlar işaretlidir (SC-22, FR-37).
- [ ] K13: Operatör `/reports/*` için 403 alır, Denetçi ve Yönetici açar; Yönetici ve Denetçi `/emergencies/*` için 403 alır (BR-34, SC-24).
- [ ] K14 (eski Faz 9 K10): SH-15 formülü `IDurationCalculator`'da XML yorumunda.
- [ ] K15: E-30 ve E-31 tabloları G11 imzasını, üç ekran G12 hizalamasını taşır.

**Kapsam dışı:** Acil bildirimi ve panoda acil bölümü (Faz 8), rapor grafiği ve CSV dışa aktarımı (Faz 9), bekleme ortalaması (Faz 7), düzeltilmiş işareti (Faz 8).

**Sonuç (2026-11-16):** TESLİM ALINDI (tur 1). Kabul kriterleri 29/29; ön tur ÖT-1 (boş PIN gönderiminde göz düğmesi taşması, Faz 4-6
PIN formları dahil) kapandı. Faz 3 ve Faz 6 ekranlarındaki DÜŞÜK B1, B2 D-1 düzeltme işine (PL-74). Rapor: `Docs/Delivery/Faz-6A-Teslim.md`.

---

## Sunum hazırlığı (faz değil, Faz 6A'dan sonra, Faz 7'den önce)

**Amaç:** Salı sunumunda ekranlar boş kalmasın ve süre beklemeden anlatılabilsin diye kullanıcının veritabanında uygulama üzerinden gerçek zamanla
tamamlanmış kayıtlar bulunması (PL-65). Faz kapısı ve teslim raporu yoktur.

**Sıra (PL-74):**

0. Koordinatör: Faz 6A commit'i ve README. D-1 düzeltme işi (Faz 6A raporu bölüm 10): frontend B1 ve B2'yi düzeltir, qa-tester hedefli ölçer,
   koordinatör `Workflow.md §4.4` düzeniyle commit'ler. Ardından `App_Data/vaka.db` yedeği ve `Data/Seed/DemoData.sql`'in `INSERT OR IGNORE` ile
   yüklenmesi; `PLN3` ve `KSA` tip listesinde görünür. Kullanıcının makinesinde user-secrets'ta `EventChain:HmacKey` var, uygulama başlıyor.
1. qa-tester: `App_Data/vaka.db`'yi `.qa/` altına ayrıca yedekler; uygulamayı kullanıcının veritabanıyla kendi başlattığı süreçte, boş portta
   açar. Yalnız arayüz; sahte veri, doğrudan SQL ve geriye tarihli kayıt yok. `PLN` tipinde taslak açmaz (sunumdaki canlı v2 yayını için).
2. Test hesaplarıyla giriş denetimi: `0001`, `1001`, `2001`, `2002`, `3001` (parola ve PIN `Docs/Local/TestHesaplari.md`'de; plana, rapora ve
   mesaja yazılmaz).
3. Kayıt A, 12 sn / 40 dk (doküman satır 67-71; en uzun süren olduğu için önce başlar): sahip `2002`, makine `T1-H2-M01`, tip `KSA`. 1. fazın 1.
   adımı yaklaşık 12 saniyede biter; adımlar arasında `2002` mola başlatır, 40 dakika sonra bitirir; 2. adım kısa sürede bitince faz neti
   minimumun altında kalır, gerekçe dialogda girilir; 2. faz normal tamamlanır.
4. Kayıt B, normal biten (A'nın molası sürerken): sahip `2001`, makine `T1-H1-M01`, tip `KSA`; açılışta bir lot (ör. `L2027-001`) eklenir;
   adımlara `2002` görevli eklenir (A'da açık molası yoksa; varsa görevlisiz); her fazın neti 60 saniyeyi geçer, gerekçe istenmez.
5. Kayıt C, süre altında gerekçeli: sahip `2001`, B bittikten sonra aynı makinede, tip `KSA`; ilk fazın son adımı 60 saniye dolmadan bitirilir,
   gerekçe dialogda.
6. Doğrulama: üç kaydın detayında 13 soru dolu, bütünlük rozeti "Doğrulandı"; süre ve efor raporunda `KSA` satırları; malzeme izlenebilirliğinde
   `L2027-001` kayıt B'yi gösterir. Uygulama süreci kapatılır.
7. Çıktı: kayıt no'lar ve her biri için tek satır sonuç koordinatör mesajıyla analiste; analist `Docs/Local/SunumSenaryosu.md` §5'e işler.
   Bütünlük ihlali gösterimi kullanıcının veritabanında yapılmaz.

---

## Faz 7: Talep, beklemeye alma ve yönetici panosu (Salı sonrası)

**Amaç:** Sahibin yöneticiye gerekçeli talep açması, yöneticinin talepleri karara bağlaması, kaydın beklemeye alınıp devam ettirilmesi ve
yöneticinin aktif işleri tek ekranda görmesi. Faz 5'ten ertelenen adım sürerken mola (K-39) ve görevli PIN onayı (K-10) da bu fazdadır (PL-57).
Takvim: Salı (2026-11-17) sonrası (O-03, PL-46).

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Talep ve bekleme tabloları; kayıt başına tek bekleyen talep ve tek açık bekleme için kısmi unique index (BR-41); pano sorguları için index'ler | backend-engineer talebi, dbadmin |
| 2 | BE | Talep servisi: SK-5, BR-41; türler kayıt durumuna göre (Açıldı: İptal; Devam ediyor: üçü; Beklemede: İptal ve Yarıda kapatma; Brief §6.1); geri çekme; onay işlemi Faz 6 müdahale servisini aynı transaction'da çağırır, koşul sağlanmazsa talep Bekliyor kalır; ilk karar geçerli (K-40); ret gerekçesi zorunlu; kayıt başka yoldan kapanınca talep Geçersiz (BR-42) | backend-engineer |
| 3 | BE | Müdahale servisine ek: beklemeye alma (BR-43); Beklemede kayıtta iptal ve yarıda kapatma, açık bekleme müdahale anında kapanır; doğrudan müdahalede bekleyen talep Geçersiz (BR-42); sahip beklemedeki kaydı PIN'le devam ettirir (BR-44) | backend-engineer |
| 4 | BE | `IDurationCalculator`: SH-05, SH-10, SH-16; SH-06, SH-11, SH-14 bekleme terimiyle güncellenir, yorumlar aynı commit'te; açık iş genişlemesi: bekleyen talep ve Beklemede kayıt pasifleştirmeyi, Beklemede kayıt makine kaldırmayı engeller | backend-engineer |
| 5 | BE | Pano sorgusu; SH-16 (başladığı vardiya bitmiş açık adım ve açık mola, Faz 4 vardiya bulucusuyla); ana sayfa Yönetici ve Denetçi'yi `/records` yerine `/dashboard`'a yönlendirir | backend-engineer |
| 6 | FE | E-22 Yönetici panosu, E-23 Talep formu (yürütme ekranı ve detayda sahibe), E-24 Taleplerim, E-25 Talepler ve karar formu; E-26'ya beklemeye alma türü; detayda talepler ve beklemeler; panoda Beklemede ve bekleyen talepler; İşlerim'de beklemedeki kayıtlar ve "Devam et"; makine panosunda Beklemede | frontend-engineer |
| 7 | BE | Faz 5'ten ertelenen (PL-57): mola servisi adım sürerken molaya izin verir (K-39, BR-38 tam hali; devam eden adım molaya bağlanır, mola açıkken adım bitirilemez, BR-40 sonrası adım "Devam ediyor" sürer, SH-17 değer üretir); görevli eklemede eklenen kişinin PIN'i aynı istekte doğrulanır, hatalı deneme onun sayacına yazılır, PIN'i kilitli kişi eklenemez (K-10, SK-4, BR-53); Faz 6 müdahalesinde adım içi açık molanın süresi düşülür | backend-engineer |
| 8 | FE | Faz 5'ten ertelenen: mola düğmesi adım sürerken de; açık mola bandında ve mola ekranında sürdüğü adım; görevli panelinde katılım PIN'i; detayda adım molası satırı | frontend-engineer |
| 9 | BE | Kalıcı oturum iptali (Faz 6 A1, PL-71, PL-72): yeniden başlatmadan sonra da çıkış yapılmış çerezin reddi için kullanıcı satırında oturum sürümü. ALTER sınıfıdır; dbadmin talebi kullanıcı onayı olmadan uygulanmaz, onay gelmezse Faz 6'nın bellek içi çözümü sürer | backend-engineer talebi, dbadmin |

**Paralel:** Evet. Gerekçe: tablolar yeni, mevcut tabloya dokunulmaz (PL-02); route ve ViewModel aşağıda.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET, POST `/requests/create?recordId=` | `RequestsController.Create` | Operator | `RequestCreateViewModel { CommandId, RecordId, RecordNo, Type, AllowedTypes[], Reason, Pin, PendingRequest? { Id, TypeLabel, RequestedAtLocal } }` | `Views/Requests/Create.cshtml` |
| POST `/requests/withdraw/{id}` | `RequestsController.Withdraw` | Operator | `{ CommandId, Pin }` | - |
| GET `/requests/mine` | `RequestsController.Mine` | Operator | `MyRequestListViewModel { Items[] { Id, RecordId, RecordNo, MachineLabel, TypeLabel, Reason, StatusLabel, RequestedAtLocal, DecidedByName?, DecidedAtLocal?, DecisionNote?, CanWithdraw, CommandId } }` | `Views/Requests/Mine.cshtml` |
| GET `/requests` | `RequestsController.Index` | ManagerOrAuditor | `RequestListViewModel { Items[] { Id, RecordId, RecordNo, MachineLabel, RecordStatusLabel, TypeLabel, Reason, RequestedByName, RequestedAtLocal, StatusLabel, DecidedByName?, DecidedAtLocal?, DecisionNote?, CanDecide }, Status (varsayılan Pending), Page, PageSize, Total }` | `Views/Requests/Index.cshtml` |
| GET `/requests/decide/{id}`; POST `/requests/approve/{id}`, `/requests/reject/{id}` | `RequestsController.Decide`, `.Approve`, `.Reject` | Manager | `RequestDecisionViewModel { CommandId, RequestId, RecordId, RecordNo, TypeLabel, Reason, RequestedByName, RequestedAtLocal, RecordStatusLabel, DecisionNote, Pin, BlockReason? }` | `Views/Requests/Decide.cshtml` |
| GET, POST `/interventions/create?recordId=&type=` | `InterventionsController.Create` | Manager | Faz 6 sözleşmesine ek: `Type = Hold`, `PendingRequest? { Id, TypeLabel, RequestedAtLocal }`. `AllowedTypes`: Devam ediyor kayıtta İptal, Yarıda kapatma, Beklemeye alma; Beklemede kayıtta İptal ve Yarıda kapatma | `Views/Interventions/Create.cshtml` |
| POST `/records/resume/{id}` | `RecordsController.Resume` | Operator | `{ CommandId, Pin }` | - |
| GET `/dashboard` | `DashboardController.Index` | ManagerOrAuditor | `DashboardViewModel { ActiveRecords[] { RecordId, RecordNo, MachineLabel, StatusLabel, OwnerName, OpenedAtLocal, LockExpiresAtLocal?, CurrentStepTitle? }, NeedsAttention[] { KindLabel, RecordId, RecordNo, MachineLabel, PersonName, StartedAtLocal, ShiftName }, BelowMinimum[] { RecordId, RecordNo, PhaseTitle, NetSeconds, MinDurationSeconds, Reason, CompletedAtLocal }, RecentCompleted[] { RecordId, RecordNo, MachineLabel, CompletedAtLocal, NetSeconds, EffortSeconds }, OpenCount, InProgressCount, CompletedTodayCount, OnHold[] { RecordId, RecordNo, MachineLabel, OwnerName, HeldAtLocal }, PendingRequests[] { RequestId, RecordNo, TypeLabel, RequestedByName, RequestedAtLocal }, Emergencies[] (Faz 8) }` | `Views/Dashboard/Index.cshtml` |
| Mevcut ekranlara ek alanlar | `Run`, `Details`, `MyWork`, `MachineBoard` | - | Run: `CanRequest, PendingRequest?, CanResume, Holds[]`; Details: `Requests[] { TypeLabel, Reason, StatusLabel, RequestedByName, RequestedAtLocal, DecidedByName?, DecisionNote? }, Holds[] { StartedAtLocal, EndedAtLocal?, DurationSeconds?, StartedByName, EndedByName? }`; MyWork: `HeldRecords[] { Id, RecordNo, MachineLabel, HeldAtLocal, CanResume }`; MachineBoard: `State = OnHold` | mevcut görünümler |
| POST `/records/add-participant/{stepId}` ve Run | `RecordsController.AddParticipant`, `.Run` | Operator | Faz 5 sözleşmesine ek: `ParticipantPin`; `Participants[].ConfirmedAtLocal?`; `DuringStepTitle` dolar; `CanStartBreak` adım sürerken de true olabilir | mevcut görünümler |

`Type` değerleri: `Cancel`, `Abort`, `Hold`. Görünür adları: İptal, Yarıda kapatma, Beklemeye alma.

**Dosya sınırı:** frontend `Views/Requests/**`, `Views/Dashboard/**`, `Views/Interventions/**` (beklemeye alma), mevcut `Run`, `Details`, `MyWork`, `MachineBoard` görünümlerinde yeni bölümler;
backend `Controllers/RequestsController.cs`, `Controllers/DashboardController.cs`, `Controllers/InterventionsController.cs`, `Controllers/HomeController.cs`, `RecordsController.Resume`, talep, müdahale ve pano servisleri, `DurationCalculator`, `ViewModels/Requests/**`, `ViewModels/Interventions/**`, `ViewModels/Dashboard/**`.

**Kabul kriterleri**

- [ ] K1: Sahip olmayan operatörün talep isteği reddedilir; gerekçesiz talep açılmaz; bekleyen talep varken ikinci talep açılmaz ve bekleyen talep gösterilir (BR-41, SK-5).
- [ ] K2: Müdahale kısmı Faz 6 K18'e taşındı; bu fazda qa-tester talep ekranları için gerileme olarak yeniden denetler: Operatör talep ekranlarında kaydı kendisi iptal etme ya da kapatma düğmesi görmez.
- [ ] K3: Açıldı kayıtta talep türü yalnız İptal'dir; Devam ediyor kayıtta üç tür, Beklemede kayıtta İptal ve Yarıda kapatma sunulur; istekle gönderilen uygunsuz tür reddedilir.
- [ ] K4: Talepler ekranı varsayılan "Bekliyor" filtresiyle açılır; Denetçi listeyi görür, karar düğmesi görmez, karar POST'u 403 (SC-24).
- [ ] K5: Gerekçesiz ret reddedilir; gerekçeyle ret sonrası talep "Reddedildi", kayıt değişmez, sahip Taleplerim'de karar ve gerekçeyi görür; geri çekilen talep "Geri çekildi" olur ve yönetici listesinde karar düğmesi taşımaz (SC-28).
- [ ] K6: Devam eden adım varken beklemeye alma onayı yapılmaz, neden gösterilir, talep Bekliyor kalır; adım bitince onay yapılır (SC-35).
- [ ] K7: İki yönetici aynı talebi art arda onaylarsa ikincisi "Talep zaten karara bağlandı" mesajı alır ve ikinci olay yazılmaz (K-40).
- K8, K9, K13, K15: Faz 6'ya taşındı (Faz 6 K15, K16, K17, K20; PL-46).
- [ ] K10: Beklemeye alınan kayıt "Beklemede", makine panosunda Beklemede ve kilitli; sahip başka makinede kayıt açabilir; sahip "Devam et"le PIN'le devam ettirir; başka aktif kaydı varken devam reddedilir (SC-27, BR-43, BR-44).
- [ ] K11: Detayda bekleme ayrı satırdır; Net = Brüt - mola - bekleme (SH-10, SH-11) ve faz neti beklemeyi düşer (SH-05, SH-06).
- [ ] K12: Yönetici doğrudan müdahale ettiğinde aynı kayıttaki bekleyen talep "Geçersiz" olur (BR-42).
- [ ] K14: Bekleyen talebi ya da Beklemede kaydı olan kullanıcı pasifleştirilemez; Beklemede kaydı olan makine kullanımdan kaldırılamaz (BR-35, BR-36).
- [ ] K16: Olay günlüğünde talep açma, geri çekme, onay, ret, geçersizlik, beklemeye alma ve devam olayları yapan ve zamanla yer alır (BR-30).
- [ ] K17: SH-05 ve SH-10 yeni, SH-06, SH-11 ve SH-14 güncellenmiş formülleriyle XML yorumundadır.
- [ ] K18 (eski Faz 6 K8, pano kısmı): `/dashboard` Yönetici ve Denetçi'de açılır, Operatör'de 403; Denetçi panoda yazma düğmesi görmez (SC-24).
- [ ] K19 (eski Faz 6 K9): Başladığı vardiya bitmiş açık adım ve açık mola panoda "Müdahale bekleyenler"de kişi, kayıt no, başlangıç ve vardiya adıyla görünür; vardiyası sürenler görünmez (SH-16).
- [ ] K20 (eski Faz 6 K10): Süre altında tamamlanan fazlar panoda gerekçesiyle listelenir.
- [ ] K21 (eski Faz 6 K12): `SH-16` formülü `IDurationCalculator`'da XML yorumunda.
- [ ] K22: Yönetici ve Denetçi `/` adresinde `/dashboard`'a yönlenir.
- [ ] K23: Beklemede kayıt doğrudan müdahaleyle iptal edilir ya da yarıda kapatılır; açık bekleme müdahale anında kapanır, makine Boş olur.
- [ ] K24 (eski Faz 5 K3): Sahip görevli ekler, eklenen kişi kendi PIN'ini girer ve görevli olur; olay ekleyeni ve onaylayanı taşır; yanlış katılım PIN'inde kişi eklenmez ve hatalı deneme o kişinin sayacına yazılır (SC-32).
- [ ] K25 (eski Faz 5 K4 kısmı): PIN'i kilitli kişi görevli eklenemez, nedeni yazılır (BR-53).
- [ ] K26 (eski Faz 5 K13 kısmı): Mola bir adım devam ederken de başlatılır; o anki adımın görevlisi başlatabilir (BR-38, SK-6).
- [ ] K27 (eski Faz 5 K13a kısmı): Mola açıkken devam eden adımı bitirme isteği "Kayıtta açık mola var" ile reddedilir, olay üretmez; adım "Devam ediyor" kalır (BR-39).
- [ ] K28 (eski Faz 5 K15 kısmı): Adım sürerken açılan mola başlatan tarafından bitirilince adım "Devam ediyor" olarak sürer ve görevlisi bitirebilir (BR-40).
- [ ] K29 (eski Faz 5 K16a): Adım sürerken verilen mola adım süresinden ve eforundan düşülür. QA iki görevlili bir adımda adım sürerken mola verip bitirir; veritabanındaki zamanlarla adım molası = mola süresi, adım süresi = bitiş - başlangıç - adım molası, efor = adım süresi x 2 olduğunu gösterir. Ayrıca QA, SC-37 zamanlarını (adım 10:00:00-10:50:00, mola 10:04:00-10:44:00, iki görevli) `DurationCalculator` kodundaki formüle uygular ve 40 dk, 10 dk, 20 dk bulur (SH-17, SH-01, SH-02).
- [ ] K30 (eski Faz 5 K16b): Adım içi mola içeren fazda faz net = faz brüt - faz molası ve bu değer fazın adım süreleri toplamı ile adımlar arasındaki molasız boşlukların toplamına eşittir; faz minimumu bu değerle kontrol edilir (SH-06, SH-07).
- [ ] K31 (eski Faz 6 K2 kısmı): Detayda adım içinde mola olan adımın satırında adım molası ayrıca görünür; mola satırı sürdüğü adımı gösterir (SC-37).
- [ ] K32 (eski Faz 6 K15 kısmı): Adım sürerken açık mola varken yapılan iptalde Kesildi adımın süresi, molanın müdahale anına kadarki kısmı düşülerek yazılır (SH-17, SH-01).
- [ ] K33 (eski Faz 6 K17 kısmı): Yönetici adım sürerken açılmış molayı bitirince adım "Devam ediyor" kalır ve görevlisi adımı bitirebilir (BR-40).

**Kapsam dışı:** Yeni talep bildirimi (Faz 8), düzeltme (Faz 8).

---

## Faz 8: Acil müdahale, bildirim ve düzeltme (Salı sonrası)

**Amaç:** Operatörün acil müdahaleyi tek ekrandan açıp başlatması, yöneticilerin bildirim alması ve yöneticinin yanlış bilgiyi orijinali koruyarak düzeltmesi.
Takvim: Salı sonrası (O-03). Faz 6A'ya taşınanlar (PL-56): acil açma servisi (kalem 2), E-27, `/emergencies/*` sözleşmesi, K1-K5, K14 ve K8'in
liste ve detay kısmı. Bu fazda kalan: acil açılışında ve yeni talepte bildirim (Faz 6A acil servisine eklenir), panoda acil bölümü (Faz 7 panosu), düzeltme.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Bildirim ve kişi başına okundu tabloları; düzeltme tablosu (hedef, orijinal, yeni değer, gerekçe, yapan, zaman) | backend-engineer talebi, dbadmin |
| 2 | BE | Faz 6A'ya taşındı (PL-56). Acil açma servisi: yalnız Acil türü, eşleme aranmaz, açıklama zorunlu, zorunlu malzeme aynı formda, açılış ve ilk adım tek transaction ve aynı sunucu anı, kayıt no `A` türü, saha referansı yok; makine ve kişi kısıtları aynen (BR-50, K-44) | backend-engineer |
| 3 | BE | Bildirim servisi: acil açılışında ve yeni talepte her aktif Yönetici'ye bildirim; okundu kişi başına (BR-51); kabuktaki okunmamış sayısı için view component (`Controllers/ViewComponents/`) | backend-engineer |
| 4 | BE | Düzeltme servisi: hedefler lot, görevli listesi, iş emri, açıklama (K-16); zaman alanı hedef olamaz; düzeltme olay olarak zincire girer; görevli düzeltmesinde efor düzeltilmiş sayıyla ayrıca gösterilir, orijinal korunur (PL-09, SH-02 yorumu güncellenir) | backend-engineer |
| 5 | FE | E-27 Acil müdahale (tek ekran), E-28 Bildirimler ve kabukta okunmamış sayısı, E-29 Düzeltme formu; detayda düzeltmeler (orijinal ve yeni değer birlikte); liste, detay ve panoda "Acil" etiketi, panoda acil müdahaleler bölümü; İşlerim'de "Acil müdahale" düğmesi | frontend-engineer |

**Paralel:** Evet. Gerekçe: yeni tablolar, mevcut kayıt akışı servisleri yeniden kullanılır; sözleşme aşağıda.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| Faz 6A'ya taşındı: GET, POST `/emergencies/create` | `EmergenciesController.Create` | Operator | `EmergencyCreateViewModel { CommandId, FacilityId?, LineId?, MachineId?, CleaningTypeId?, Description, Materials[] { MaterialLotId, ExpiredReason }, Pin, Facilities[], Lines[], Machines[] { Id, Label, IsAvailable, BusyReason }, EmergencyTypes[] { Id, Name, VersionNo, MaterialRequired }, ErrorMessage, ErrorUrl? }`; başarıda `/records/run/{id}` | `Views/Emergencies/Create.cshtml` |
| Faz 6A'ya taşındı: GET `/emergencies/machines?lineId=` | `EmergenciesController.Machines` | Operator | JSON `[{ id, label, isAvailable, busyReason }]` | - |
| GET `/notifications` | `NotificationsController.Index` | Manager | `NotificationListViewModel { Items[] { Id, KindLabel, RecordId, RecordNo, MachineLabel, Summary, CreatedAtLocal, IsRead, Url }, UnreadOnly, Page, PageSize, Total }` | `Views/Notifications/Index.cshtml` |
| POST `/notifications/mark-read/{id}`, `/notifications/mark-all-read` | `.MarkRead`, `.MarkAllRead` | Manager | - | - |
| view component | `NotificationCountViewComponent` | Manager | `NotificationCountViewModel { UnreadCount }` | `Views/Shared/Components/NotificationCount/Default.cshtml` |
| GET, POST `/corrections/create?recordId=` | `CorrectionsController.Create` | Manager | `CorrectionCreateViewModel { CommandId, RecordId, RecordNo, Target, Targets[] { Value, Label }, TargetItemId?, TargetItems[] { Id, Label, CurrentValue }, NewTextValue?, NewMaterialLotId?, NewParticipantUserIds[], Reason, Pin }` | `Views/Corrections/Create.cshtml` |
| Mevcut ekranlara ek alanlar | `Details`, `Index`, `Dashboard`, `MyWork` | - | Details: `Corrections[] { TargetLabel, OriginalValue, NewValue, Reason, ByName, AtLocal }`, `CanCorrect`; Index: `Kind` filtresi ve etiketi; Dashboard: `Emergencies[] { RecordId, RecordNo, MachineLabel, OwnerName, OpenedAtLocal, StatusLabel }`; MyWork: `CanOpenEmergency` | mevcut görünümler |

`Target` değerleri: `Lot`, `Participants`, `WorkOrder`, `Note`.

**Dosya sınırı:** frontend `Views/Emergencies/**`, `Views/Notifications/**`, `Views/Corrections/**`, `Views/Shared/Components/NotificationCount/**`, mevcut görünümlerde yeni bölümler;
backend `Controllers/EmergenciesController.cs`, `Controllers/NotificationsController.cs`, `Controllers/CorrectionsController.cs`, `Controllers/ViewComponents/**`, ilgili servisler, `ViewModels/Emergencies/**`, `ViewModels/Notifications/**`, `ViewModels/Corrections/**`.

**Kabul kriterleri**

- K1 (Faz 6A'ya taşındı, PL-56): Acil müdahale ekranında yalnız Acil türünde, yürürlükte versiyonu olan tipler listelenir; makine eşlemesi aranmaz (BR-50).
- K2 (Faz 6A'ya taşındı, PL-56): Gönderimde kayıt açılır ve ilk adım başlar; açılış zamanı ile ilk adım başlangıcı aynı değerdir; kayıt no `...-A<Yıl>-<Sıra>`; saha referansı alanı "yok" yazar (SC-02).
- K3 (Faz 6A'ya taşındı, PL-56): Açıklama boşken kayıt açılmaz; malzeme zorunlu acil tipte malzemesiz gönderim reddedilir (SC-02, BR-11).
- K4 (Faz 6A'ya taşındı, PL-56): Makine kilitliyse ya da operatörün aktif kaydı varsa acil açılmaz; mesaj kilidi tutan kaydı, sahibini ve durumunu gösterir (K-44).
- K5 (Faz 6A'ya taşındı, PL-56): Acil kayıtta fazın son adımı gerekçe istenmeden bitirilir (minimum yok).
- [ ] K6: Acil açılınca ve yeni talep açılınca her aktif Yönetici'nin kabuğundaki okunmamış sayısı bir artar; bildirimi okundu işaretleyen yöneticinin sayısı azalır, diğerininki değişmez (BR-51).
- [ ] K7: `/notifications` Operatör ve Denetçi'de 403.
- [ ] K8: Panoda acil kayıt metinli "Acil" etiketiyle ayrı bölümde görünür; liste ve detay kısmı Faz 6A K6'da (FR-15, BR-10).
- [ ] K9: Yönetici yanlış lotu gerekçe ve PIN'le düzeltir; detayda orijinal ve yeni değer birlikte, gerekçe, yapan ve zamanla görünür; düzeltme olay günlüğündedir (BR-29).
- [ ] K10: Düzeltme formunda zaman alanı hedef olarak yoktur; istekle gönderilen zaman hedefi reddedilir (SC-13).
- [ ] K11: Görevli düzeltmesinden sonra detay orijinal ve düzeltilmiş efor değerini birlikte gösterir (PL-09).
- [ ] K12: Düzeltme sonrası bütünlük rozeti "Doğrulandı" kalır.
- [ ] K13: `/corrections/*` Operatör ve Denetçi'de 403.
- K14 (Faz 6A'ya taşındı, PL-56): Acil müdahale ekranı 360 px'te yatay kaydırmasız, gönder düğmesi 44 px.

**Kapsam dışı:** E-posta ve SMS (K-43), acilin başka kaydı zaman aşımına düşürmesi (K-44 alternatifi).

---

## Faz 9: Raporlar, denetim çıktısı ve sistem ayarları (Salı sonrası)

**Amaç:** Makine ve faz bazında süre, efor ve minimum uyumunun raporlanması, lot izlenebilirliği, denetimde sunulacak yazdırılabilir kayıt
çıktısı ve sistem ayarlarının ekrandan değiştirilmesi (K-47).
Takvim: Salı sonrası (O-03). Faz 6A'ya taşınanlar (PL-56): süre raporu ve malzeme izlenebilirliğinin sorgu ve tabloları (kalem 2, kalem 3'ün
sorgu kısmı, E-30 tablosu, E-31), SH-15, K1-K5 ve K10. Denetim çıktısının içeriği Faz 6 detayının yazdırma çıktısındadır (Faz 6 K14). Bu fazda
kalan: E-30 SVG grafiği (K9), CSV dışa aktarımları (K7), düzeltmeleri de içeren ayrı `/records/audit/{id}` adresi (K6, K8), bekleme ve düzeltme
kolonlarının raporlara eklenmesi, Sistem ayarları (K12-K15), son regresyon (K11).

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Rapor sorguları için index'ler (backend'in sorgu listesine göre); yeni tablo yok | backend-engineer talebi, dbadmin |
| 2 | BE | Süre raporu sorgusu: makine ve faz bazında ortalama brüt, mola, bekleme, net, efor; yalnız Tamamlandı kayıtlar, faz ortalaması versiyon içinde (SH-15); durum sayıları ayrı; süre altı sayısı; en uzun adımlar (FR-40); tür filtresi | backend-engineer |
| 3 | BE | Lot izlenebilirliği: lotla ve makineyle sorgu (FR-37); denetim çıktısı sorgusu; CSV (UTF-8 BOM, `;` ayırıcı); `IDurationCalculator` SH-15 | backend-engineer |
| 4 | BE | Ayar servisi yazma: değer pozitif tam sayı; `<AzamiKilitSüresi>` aktif planlı tiplerin varsayılan kilit süresinin altına inemez, engelleyen tipler listelenir; her değişiklik aynı transaction'da geçmişe yazılır; PIN ister; etkiler PL-18 (açık kayıtların kilit bitişi değişmez, mevcut PIN'ler geçerli); `<PinUzunluğu>` küçültülemez, yalnız aynı kalır ya da artar (PL-66) | backend-engineer |
| 5 | FE | E-30 Süre ve efor raporu (SVG grafik ve aynı verinin tablosu), E-31 Malzeme izlenebilirliği, E-32 Denetim çıktısı (yazdırma CSS'i, kabuksuz); detay ve listeye denetim çıktısı ve CSV bağlantıları; E-33 Sistem ayarları (birimli alanlar, PIN, engelleyen tipler, değişiklik geçmişi); menüde Yönetici ve Süper admin için Sistem ayarları | frontend-engineer |

**Paralel:** Evet. Gerekçe: raporlar salt okuma; ayar tablosu Faz 1'den beri var, yeni tablo yok; sözleşme aşağıda.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/reports/durations` | `ReportsController.Durations` | ManagerOrAuditor | `DurationReportViewModel { Filter { From?, To?, FacilityId?, LineId?, MachineId?, CleaningTypeId?, Kind? }, ByMachine[] { MachineLabel, CleaningTypeName, RecordCount, AvgGrossSeconds, AvgBreakSeconds, AvgHoldSeconds, AvgNetSeconds, AvgEffortSeconds }, ByPhase[] { CleaningTypeName, VersionNo, PhaseOrderNo, PhaseTitle, MinDurationSeconds?, RecordCount, AvgNetSeconds, BelowMinimumCount }, LongestSteps[] { CleaningTypeName, VersionNo, PhaseTitle, StepTitle, RecordCount, AvgDurationSeconds, MaxDurationSeconds }, StatusCounts[] { StatusLabel, Count }, Facilities[], Lines[], Machines[], CleaningTypes[] }` | `Views/Reports/Durations.cshtml` |
| GET `/reports/durations-csv` | `ReportsController.DurationsCsv` | ManagerOrAuditor | aynı filtre, CSV | - |
| GET `/reports/materials` | `ReportsController.Materials` | ManagerOrAuditor | `MaterialTraceViewModel { Mode (Lot, Machine), LotQuery, MaterialLotId?, MachineId?, From?, To?, LotUsages[] { RecordId, RecordNo, MachineLabel, OpenedAtLocal, StatusLabel, ExpiredAtEntry, IsCorrected }, MachineMaterials[] { EnteredAtLocal, RecordId, RecordNo, MaterialCode, MaterialName, LotNo, ExpiryDate, ExpiredAtEntry, IsCorrected } }` | `Views/Reports/Materials.cshtml` |
| GET `/reports/materials-csv` | `ReportsController.MaterialsCsv` | ManagerOrAuditor | aynı filtre, CSV | - |
| GET `/records/audit/{id}` | `RecordsController.Audit` | ManagerOrAuditor | `AuditReportViewModel { Detail (RecordDetailViewModel), Integrity, Corrections[], GeneratedAtLocal, GeneratedByName }` | `Views/Records/Audit.cshtml` |
| GET `/records/audit-csv/{id}`, `/records/export-csv` | `RecordsController.AuditCsv`, `.ExportCsv` | ManagerOrAuditor | CSV; liste dışa aktarımı `RecordListViewModel.Filter` ile | - |
| GET, POST `/settings` | `SettingsController.Index`, `.Update` | ManagerOrSuperAdmin | `SystemSettingsViewModel { SessionTimeoutMinutes, PinLength, PinMaxAttempts, MaxPhotoSizeMb, MaxVideoSizeMb, MaxLockMinutes, Pin, BlockingTypes[] { Id, Name, DefaultLockMinutes }, History[] { SettingLabel, OldValue, NewValue, ChangedByName, ChangedAtLocal }, StatusMessage }` | `Views/Settings/Index.cshtml` |

**Dosya sınırı:** frontend `Views/Reports/**`, `Views/Records/Audit.cshtml`, `Views/Settings/**`, menü, `Styles/` yazdırma katmanı, `wwwroot/js/SvgChart.js`; backend
`Controllers/ReportsController.cs`, `Controllers/SettingsController.cs`, `RecordsController` denetim action'ları, rapor, CSV ve ayar servisleri, `DurationCalculator`, `ViewModels/Reports/**`, `ViewModels/Settings/**`.

**Kabul kriterleri**

- K1 (Faz 6A'ya taşındı, PL-56): Süre raporu makine ve faz bazında ortalama brüt, mola, bekleme, net ve eforu ayrı kolonlarda gösterir (FR-21, FR-36).
- K2 (Faz 6A'ya taşındı, PL-56): Ortalamalara yalnız Tamamlandı kayıtlar girer; İptal, Yarıda kapatıldı, Zaman aşımı ve Beklemede sayıları ayrı gösterilir; QA bir iptal kaydının ortalamayı değiştirmediğini gösterir (SH-15).
- K3 (Faz 6A'ya taşındı, PL-56): Aynı tipin v1 ve v2 faz ortalamaları ayrı satırdır; süre altı faz sayısı faz satırında görünür (FR-22).
- K4 (Faz 6A'ya taşındı, PL-56): En uzun süren adımlar listelenir (FR-40); planlı ve acil kayıtlar tür filtresiyle ayrılır (FR-15).
- K5 (Faz 6A'ya taşındı, PL-56): Lotla arama lotun kullanıldığı kayıtları makine, tarih ve kayıt no ile; makineyle arama malzeme ve lotları tarih sırasıyla listeler; SKT geçmiş ve düzeltilmiş satırlar işaretlidir (SC-22, FR-37).
- [ ] K6: Denetim çıktısı 13 soruyu, olay günlüğünü, bütünlük sonucunu, versiyonu, düzeltmeleri, SKT işaretlerini ve süre altı gerekçelerini içerir; yazdırma önizlemesinde menü ve üst bar görünmez (SC-14, FR-35).
- [ ] K7: CSV dosyaları Excel'de Türkçe karakterler bozulmadan ve kolonlara ayrılmış açılır.
- [ ] K8: Operatör `/reports/*` ve `/records/audit/*` için 403 alır; Denetçi açar (BR-34, SC-24).
- [ ] K9: Her grafiğin aynı veriyi gösteren tablosu vardır; grafik gerçek veriden SVG ile çizilir (NFR-05).
- K10 (Faz 6A'ya taşındı, PL-56): SH-15 formülü `IDurationCalculator`'da XML yorumunda.
- [ ] K11: Son regresyon: Brief §8'deki SC-01..SC-38'in her biri raporda geçti ya da kaldı olarak işaretlenir.
- [ ] K12: Yönetici ve Süper admin `/settings`'te bir değeri PIN'le değiştirir; yanlış PIN'de değer değişmez; geçmişte eski değer, yeni değer, yapan ve zaman görünür. Operatör ve Denetçi `/settings` GET ve POST'ta 403 alır (FR-55).
- [ ] K13: Sıfır, negatif ve tam sayı olmayan değer alan altında nedeniyle reddedilir.
- [ ] K14: `<AzamiKilitSüresi>` aktif bir planlı tipin varsayılan kilit süresinin altına indirilemez; ekran engelleyen tipleri listeler. Değişiklik açık kayıtların kilit bitişini değiştirmez (PL-18).
- [ ] K15: `<PinUzunluğu>` artırılınca mevcut PIN'ler doğrulanmaya devam eder, yeni belirlenen PIN yeni uzunlukta istenir; küçültme isteği alan altında nedeniyle reddedilir, değer ve geçmiş değişmez (PL-66); `<OturumSüresi>` ve `<PinDenemeSınırı>` değişikliği sonraki istekte uygulanır; `<AzamiMedyaBoyutu>` yeni yüklemelerde uygulanır (PL-18).

**Kapsam dışı:** Personel ve vardiya kırılımı (K-36), sunucu tarafı PDF (K-28), İÖ önerileri.

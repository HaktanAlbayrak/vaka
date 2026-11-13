# Faz Planı

> Sahibi: analyst. Kullanıcı onayıyla donar; sonraki kararlar `Docs/Phases/Decisions.md` dosyasına eklenir.
> Durum: **ONAYLANDI** (2026-11-13, `Decisions.md` O-01). K-38, K-39, K-40, K-47 ve PL-05 kullanıcı yanıtlarıyla işlendi.
> Kimlikler: FR, NFR, BR, SC, SH, SK `Docs/Analysis/Brief.md`; E (ekran) `Docs/Analysis/Requirements.md §4`; K `Docs/Analysis/Kusur.md`; PL `Decisions.md`.

## Özet

| Faz | Başlık | Kullanıcının göreceği çıktı | Paralel | Durum |
|---|---|---|---|---|
| 0 | Analiz ve faz planı | Brief, Kusur, Requirements, plan | - | TAMAMLANDI (2026-11-13) |
| T | Tasarım DNA'sı ve kabuk prototipi | DNA kartı, demo veriyle 5 prototip sayfa | Hayır | BEKLİYOR |
| 1 | Giriş, PIN, kullanıcılar ve Süper admin | Giriş, ilk giriş, PIN, kullanıcı yönetimi, Süper admin | Evet | BEKLİYOR |
| 2 | Tesis yapısı, vardiya ve malzeme | Tesis, hat, makine, vardiya, malzeme ve lot ekranları | Evet | BEKLİYOR |
| 3 | Temizlik tipi ve prosedür versiyonu | Tip, makine eşleme, taslak düzenleyici, yayınlama | Evet | BEKLİYOR |
| 4 | Kayıt açma ve makine kilidi | Yeni kayıt, İşlerim, Açıldı hali, makine panosu, zaman aşımı | Evet | BEKLİYOR |
| 5 | Adım yürütme, görevli ve mola | Adım başlat/bitir, görevli PIN onayı, adım arasında ve adım sürerken mola, otomatik tamamlanma | Evet | BEKLİYOR |
| 6 | Kayıt listesi, detay ve yönetici panosu | 13 soruyu yanıtlayan detay, olay günlüğü, bütünlük, pano | Evet | BEKLİYOR |
| 7 | Talep, müdahale ve beklemeye alma | Talepler, Taleplerim, müdahale formu, bekleme ve devam | Evet | BEKLİYOR |
| 8 | Acil müdahale, bildirim ve düzeltme | Acil müdahale ekranı, bildirimler, düzeltme formu | Evet | BEKLİYOR |
| 9 | Raporlar, denetim çıktısı ve sistem ayarları | Süre ve efor raporu, lot izlenebilirliği, yazdırılabilir denetim çıktısı, Sistem ayarları ekranı | Evet | BEKLİYOR |

Durum değerleri: BEKLİYOR, GELİŞTİRMEDE, QA'DA, İADE, TESLİM ALINDI. Faz 0 QA'sız analiz fazıdır, kullanıcı onayıyla TAMAMLANDI olur.

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
| SH-17 adım molası | `CalculateStepBreaks` | 5 | Adımla kesişen mola süresi (K-39, PL-19) |
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
| SH-16 vardiyası bitmiş açık iş | `IsPastShiftEnd` | 6 | PL-07 |
| SH-05 faz beklemesi | `CalculatePhaseHolds` | 7 | |
| SH-10 kayıt beklemesi | `CalculateRecordHolds` | 7 | |
| SH-15 rapor ortalaması | `CalculateAverage` | 9 | Yalnız Tamamlandı kayıtlar, versiyon içinde |

**Her kod fazında ortak kabul kriterleri** (qa-tester her fazın raporunda ayrıca işaretler)

- [ ] G1: `dotnet build Src/Vaka.Web -warnaserror` 0 hata 0 uyarı; `npm run build:css --prefix Src/Vaka.Web` başarılı.
- [ ] G2: `CodingStandards.md §4` grep'leri 0 eşleşme; her controller `[Authorize(Roles = Roles.X)]`, `[Authorize]` ya da `[AllowAnonymous]` taşır; rol metni yalnız `Roles` sabitlerinden.
- [ ] G3: UI ekranları `UiDelivery.md` §2 matrisinde kırık hücresiz, §4 sıfır ihlal, konsol 0 hata; DNA kartından sapma yok.
- [ ] G4: Operatör ekranları 360 px'te yatay kaydırmasız; birincil düğmeler en az 44 px yüksekliğinde (NFR-04); durumlar renk dışında metinle de ayrılır (NFR-05).
- [ ] G5: Antiforgery token'sız POST 400 döner (NFR-12).
- [ ] G6: Görünür metin, seed ve dokümanlarda em-dash ve en-dash 0 eşleşme.
- [ ] G7: Migration `Up()` gövdelerinde `Drop` yok; ALTER varsa `Docs/Db/` kaydında kullanıcı onayı yazılı.
- [ ] G8 (Faz 4 ve sonrası): `Controllers/` ve `Views/` altında `TimeSpan` aritmetiği, `.Subtract(` ya da `AtUtc` alanları arasında çıkarma yok; süreler yalnız `IDurationCalculator`'dan.
- [ ] G9 (Faz 4 ve sonrası): Fazın her yeni komutu aynı `CommandId` ile iki kez gönderildiğinde tek etki ve tek olay üretir (BR-33).
- [ ] G10 (Faz 2 ve sonrası): Yalnız `SuperAdmin` rolüne sahip kullanıcı fazın yeni adreslerinin tümünde (GET ve POST) 403 alır ve menüde bu ekranları görmez; istisna Faz 9'daki `/settings` (BR-34, PL-15).

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
- [ ] TK6: Kabukta kişi adı, sicil no yeri ve "Kullanıcı değiştir" 390 ve 1440 px'te görünür; tarayıcı çevrimdışı moduna alınınca bağlantı göstergesi değişir.
- [ ] TK7: Açık ve koyu temada metin kontrastı AA; `prefers-reduced-motion` açıkken hareket kapanır (MOTION > 3 ise).
- [ ] TK8: Kullanıcı DNA kartını ve görüntüleri onaylar; analist onayı `Decisions.md`'ye işler. Onaysız Faz 1 başlamaz.
- [ ] TK9: `Authorization/Roles.cs` dört tekil ve dört birleşik sabiti içerir; birleşik sabitlerde tekil sabit dışında rol metni yoktur; projede `Roles.cs` dışında rol adı metni geçmez.
**Kapsam dışı:** Kimlik doğrulama, veritabanı, gerçek veri.

---

## Faz 1: Giriş, PIN, kullanıcılar ve Süper admin

**Amaç:** Kişinin sicil no ve parolayla girdiği, ilk girişte parola ve PIN belirlediği, Süper admin'in yönetici hesaplarını, yöneticinin
Operatör ve Denetçi hesaplarını yönettiği kimlik katmanı; sistem ayarlarının veritabanından okunması.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Kullanıcı ve kullanıcı rolleri tabloları (Brief §7.g `User`); sicil no benzersiz; PIN hatalı deneme sayacı, PIN kilit anı, parola değiştirme ve PIN belirleme bayrakları bu fazda. Seed: tek Süper admin (PL-05), geçici parola özetli, geliştirme parolası yalnız README'de; başka kullanıcı seed'lenmez | backend-engineer talebi, dbadmin |
| 2 | DB | Sistem ayarları tablosu (beş ayar, medya boyutu fotoğraf ve video ayrı; tamsayı değer) ve ayar değişiklik geçmişi tablosu (eski, yeni, yapan, zaman) (K-47, PL-17). Seed değerleri kullanıcıdan gelir (Requirements §9); gelmeden bu kalem kapanmaz | backend-engineer talebi, dbadmin |
| 3 | BE | Ayar servisi: beş değeri veritabanından okur; bu fazda yazma yok (ekran Faz 9). Oturum süresi ve PIN değerleri her istekte servisten okunur | backend-engineer |
| 4 | BE | Cookie kimlik doğrulama, sicil no + parola, işlemsizlikte `<OturumSüresi>` kayan süre; claim'ler: `UserId`, ad, sicil no, roller | backend-engineer |
| 5 | BE | İlk giriş kapısı: parola değiştirilmemiş ya da PIN'i olmayan kullanıcı `/account/first-login`, `/account/logout` ve statik dosya dışındaki her istekte ilk girişe yönlenir (BR-55). Yetki politikası değil, istek filtresi | backend-engineer |
| 6 | BE | PIN servisi: biçim kuralı yalnız yeni PIN belirlenirken (BR-49: yalnız rakam, `<PinUzunluğu>` hane, tüm haneleri aynı ya da ardışık artan/azalan dizi reddedilir), doğrulama uzunluktan bağımsız, `<PinDenemeSınırı>` hatalı denemede kilit ve oturum kapatma; PIN düz metin günlüklenmez (NFR-15) | backend-engineer |
| 7 | BE | Kullanıcı servisi: ekleme, düzenleme, rol atama, geçici parola, pasifleştirme ve yeniden etkinleştirme, PIN ve parola sıfırlama (PIN ister, PL-06). Kapsam kuralı SK-9: Yönetici yalnız `Manager` ve `SuperAdmin` rolü taşımayan hesapları yönetir ve yalnız `Operator`, `Auditor` atar; Süper admin tümünü; kimse kendini pasifleştiremez ve kendi rollerini değiştiremez; son aktif Süper admin pasifleştirilemez ve rolü kaldırılamaz (PL-05, PL-16). Kural serviste, işlemi yapanın ve hedefin rolleri veritabanından okunarak uygulanır; policy yok. Açık iş listesi arayüzü (`OpenWork`), bu fazda boş döner, Faz 4, 5, 7'de genişler; `Url` yalnız görüntüleyenin erişebildiği ekrana verilir | backend-engineer |
| 8 | BE | Ana sayfa yönlendirmesi: yalnız `SuperAdmin` rolü olan kullanıcı `/` adresinde `/users`'a yönlenir; diğer roller Faz 4 ve 6'ya kadar `HomeViewModel` görür | backend-engineer |
| 9 | FE | E-01 Giriş, E-02 Parola ve PIN (iki mod), E-03 Kullanıcılar, E-04 Kullanıcı formu ve pasifleştirme onayı, 403 sayfası; kabukta gerçek kişi adı ve rol bazlı menü (Süper admin menüsünde yalnız Kullanıcılar ve hesap) | frontend-engineer |

**Paralel:** Evet. Gerekçe: şema Brief §7.g'de belli (K-40 kararıyla kullanıcıya yönetici alanı eklenmez; roller ayrı tabloda); route ve ViewModel aşağıda; dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET, POST `/account/login` | `AccountController.Login` | anonim | `LoginViewModel { RegistryNo, Password, ReturnUrl, ErrorMessage }` | `Views/Account/Login.cshtml` |
| POST `/account/logout` | `AccountController.Logout` | Any | - | - |
| GET `/account/access-denied` | `AccountController.AccessDenied` | anonim | - | `Views/Account/AccessDenied.cshtml` |
| GET, POST `/account/first-login` | `AccountController.FirstLogin` | Any | `FirstLoginViewModel { FullName, MustChangePassword, MustSetPin, CurrentPassword, NewPassword, ConfirmPassword, NewPin, ConfirmPin, PinLength }` | `Views/Account/FirstLogin.cshtml` |
| GET `/account/security`; POST `/account/change-password`, `/account/change-pin` | `AccountController.Security`, `.ChangePassword`, `.ChangePin` | Any | `SecurityViewModel { PinLength, StatusMessage, Password { CurrentPassword, NewPassword, ConfirmPassword }, Pin { CurrentPin, NewPin, ConfirmPin } }` | `Views/Account/Security.cshtml` |
| GET `/` | `HomeController.Index` | Any | `HomeViewModel { FullName, Roles[] }` (Faz 4 ve 6'da role göre yönlendirmeye döner) | `Views/Home/Index.cshtml` |
| GET `/users` | `UsersController.Index` | ManagerOrSuperAdmin | `UserListViewModel { Items[] { Id, RegistryNo, FullName, Roles[], IsActive, IsPinLocked, MustChangePassword, CanManage }, Query, Status (Active, Inactive, All), Page, PageSize, Total }` | `Views/Users/Index.cshtml` |
| GET, POST `/users/create`, `/users/edit/{id}` | `UsersController.Create`, `.Edit` | ManagerOrSuperAdmin | `UserFormViewModel { Id?, RegistryNo, FullName, SelectedRoles[], AvailableRoles[] (işlemi yapanın atayabildiği roller), RolesLocked (kendi hesabı), TemporaryPassword (yalnız Create), IsActive, CanResetPin, CanResetPassword, CanDeactivate }` | `Views/Users/Form.cshtml` |
| GET, POST `/users/deactivate/{id}` | `UsersController.Deactivate` | ManagerOrSuperAdmin | `UserDeactivateViewModel { UserId, FullName, RegistryNo, OpenWork[] { Kind, Title, Url? }, CanDeactivate, BlockReason, Pin }` | `Views/Users/Deactivate.cshtml` |
| POST `/users/activate/{id}` | `UsersController.Activate` | ManagerOrSuperAdmin | - | - |
| POST `/users/reset-pin/{id}`, `/users/reset-password/{id}` | `UsersController.ResetPin`, `.ResetPassword` | ManagerOrSuperAdmin | `UserResetViewModel { UserId, TemporaryPassword (yalnız parola), Pin }` | `Views/Users/Form.cshtml` içinde |

Tüm `/users` istekleri SK-9 kapsamına göre serviste kontrol edilir; kapsam dışı hedefte istek işlem yapmaz ve 403 sayfası döner.

**Dosya sınırı:** frontend `Views/Account/**`, `Views/Users/**`, `Views/Home/**`, `Views/Shared/**`, `wwwroot/js/PinInput.js`; backend
`Controllers/AccountController.cs`, `Controllers/HomeController.cs`, `Controllers/UsersController.cs`, `Services/` (kullanıcı, PIN, parola, ayar okuma),
`ViewModels/Account/**`, `ViewModels/Users/**`, `Authorization/**`, `Program.cs`, `appsettings*.json`.

**Kabul kriterleri**

- [ ] K1: Oturumsuz istek `/users` için `/account/login?ReturnUrl=%2Fusers` adresine 302 döner.
- [ ] K2: Seed'de yalnız bir kullanıcı vardır ve rolü Süper admin'dir. Süper admin geçici parolayla girince `/account/first-login` açılır; parola ve PIN belirlenmeden `/`, `/users` ve `/account/security` istekleri ilk girişe yönlenir; bitince `/` adresi `/users`'a yönlenir (SC-34, SC-38).
- [ ] K3: İlk giriş ve PIN değiştirmede `<PinUzunluğu>` dışı uzunluk, rakam dışı karakter, tüm haneleri aynı ve ardışık artan ya da azalan PIN alan altında nedeniyle reddedilir.
- [ ] K4: Süper admin `/users/create` ile Yönetici rolünde kullanıcı açar; o Yönetici `/users/create` ile Operatör ve Denetçi açar; aynı sicil no ile ikinci kayıt "Bu sicil no kayıtlı" hatasıyla reddedilir; yeni kullanıcılar ilk girişte parola ve PIN belirler.
- [ ] K5: Operatör ve Denetçi rolündeki kullanıcı `/users`, `/users/create`, `/users/edit/{id}`, `/users/deactivate/{id}`, `/users/activate/{id}`, `/users/reset-pin/{id}`, `/users/reset-password/{id}` GET ve POST isteklerinde 403 alır; menüde Kullanıcılar yoktur (SC-17).
- [ ] K6: Yönetici ve Süper admin pasifleştirmede yanlış PIN girerse kullanıcı aktif kalır; doğru PIN'le pasif olur; pasif kullanıcı doğru parolayla giremez.
- [ ] K7: Art arda `<PinDenemeSınırı>` hatalı PIN'de PIN kilitlenir ve oturum kapanır; yeniden girişte PIN isteyen işlem "PIN kilitli" ile reddedilir. Kilitli Operatör'ün PIN'ini Yönetici, kilitli Yönetici'nin PIN'ini Süper admin sıfırlar; kullanıcı sonraki girişte ilk giriş ekranında yalnız yeni PIN belirler (SC-31 kimlik kısmı).
- [ ] K8: Son aktif Süper admin'in pasifleştirme ve Süper admin rolünü kaldırma isteği nedeniyle reddedilir; ikinci bir Süper admin açıldıktan sonra biri pasifleştirilebilir. Kullanıcı kendi hesabını pasifleştiremez ve kendi rollerini değiştiremez (PL-05, PL-16).
- [ ] K8a: Yönetici'nin formunda Yönetici ve Süper admin rolleri sunulmaz; istek değiştirilerek gönderilen `Manager` ya da `SuperAdmin` rolü reddedilir. Yönetici, `Manager` ya da `SuperAdmin` rolü taşıyan bir hesap için düzenleme, pasifleştirme, etkinleştirme, PIN ve parola sıfırlama isteği gönderince 403 alır ve hesap değişmez; listede bu hesaplarda işlem bağlantısı yoktur (SK-9).
- [ ] K8b: Süper admin rolü tek başına olan kullanıcının menüsünde yalnız Kullanıcılar ve hesap bağlantıları vardır.
- [ ] K9: Kullanıcı `/account/security`'de eski PIN'le yeni PIN belirler; eski PIN yanlışsa PIN değişmez ve hatalı deneme sayılır (K-42).
- [ ] K10: Veritabanında parola ve PIN düz metin olarak bulunmaz; uygulama günlüğünde girilen PIN geçmez (NFR-15).
- [ ] K11: Kabukta oturumdaki kişinin adı ve sicil no 360, 768 ve 1440 px'te görünür; "Kullanıcı değiştir" tek dokunuşla oturumu kapatıp giriş ekranını açar (NFR-09).
- [ ] K12: QA kopyasında ayar tablosundaki `<OturumSüresi>` kısa bir değere ayarlanır; o süre işlemsizlikten sonraki istek giriş sayfasına yönlenir. `<PinUzunluğu>` ve `<PinDenemeSınırı>` da ayar tablosundan okunur; kaynak kodda ve `appsettings*.json`'da bu değerler yoktur (K-47).
- [ ] K12a: Ayar tablosu beş ayarı (medya boyutu fotoğraf ve video ayrı) ve boş değişiklik geçmişi tablosunu içerir; seed değerleri Requirements §9'da kullanıcının verdiği değerlerdir.
- [ ] K13: Operatör ve Yönetici rollerine birlikte sahip kullanıcı iki rolün menüsünü birlikte görür (K-29).
- [ ] K14: Prototip kabuğu gerçek kabukla değişmiştir; prototip sayfaları Development'ta açılmaya devam eder (PL-10).

**Kapsam dışı:** Kullanıcıya tesis ya da yönetici bağı (K-20, K-40), kurumsal dizin, parola karmaşıklık politikası iş biriminden gelmedikçe ASP.NET varsayılanı,
Sistem ayarları ekranı (Faz 9), Süper admin hesabının uygulama dışından kurtarılması.

---

## Faz 2: Tesis yapısı, vardiya ve malzeme

**Amaç:** Kaydın seçimlerini besleyen tesis, hat, makine, vardiya, malzeme ve lot tanımlarını yöneticinin girmesi.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Tesis, hat, makine, vardiya, malzeme, lot tabloları; BR-54 benzersizlikleri; lot no malzeme içinde benzersiz; makinede kullanımdan kaldırılma anı. Seed: nötr demo tesis yapısı, vardiya, malzeme ve lotlar (V-13), en az bir SKT'si geçmiş lot | backend-engineer talebi, dbadmin |
| 2 | BE | Tanım servisleri: kod doğrulaması (tire yok, benzersizlik), kod ve makinenin hattı oluşturulduktan sonra değişmez (PL-04); pasif tesis ve hattın altındakiler yeni kayıtta seçilemez (BR-05) | backend-engineer |
| 3 | BE | Vardiya kuralları (K-38 kararı): aynı tesiste aktif vardiyalar çakışmaz; bitiş başlangıçtan küçükse ertesi gün biter; aralık başlangıç dahil bitiş hariçtir (bir vardiyanın bitişi sonrakinin başlangıcına eşit olabilir); pasif vardiya çakışma kontrolüne ve hesaplara girmez; saatler Türkiye saatidir. Bir anın vardiyasını bulan tek metot bu fazda yazılır, Faz 4 SH-13 ve Faz 6 SH-16 onu kullanır | backend-engineer |
| 4 | BE | Makine kullanımdan kaldırma; engelleyen kayıt kontrolü arayüzü bu fazda boş döner, Faz 4 ve 7'de dolar (BR-35) | backend-engineer |
| 5 | BE | SKT geçmiş kararı tek metotta: SKT < Türkiye saatine göre bugünün tarihi (BR-12) | backend-engineer |
| 6 | FE | E-05 Tesisler ve hatlar, E-06 Vardiyalar, E-07 Makineler (tesis ve hat filtreli, bağlı seçim), E-08 Malzemeler ve lotlar ("SKT geçmiş" rozeti) | frontend-engineer |

**Paralel:** Evet. Gerekçe: şema Brief §7.g'de belli, frontend ViewModel'e bağlı, dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/facilities` | `FacilitiesController.Index` | Manager | `FacilityListViewModel { Items[] { Id, Code, Name, IsActive, LineCount, ShiftCount }, ShowInactive }` | `Views/Facilities/Index.cshtml` |
| GET, POST `/facilities/create`, `/facilities/edit/{id}` | `FacilitiesController.Create`, `.Edit` | Manager | `FacilityFormViewModel { Id?, Code, Name, IsActive, CodeLocked }` | `Views/Facilities/Form.cshtml` |
| GET `/facilities/details/{id}` | `FacilitiesController.Details` | Manager | `FacilityDetailViewModel { Id, Code, Name, IsActive, Lines[] { Id, Code, Name, IsActive, MachineCount } }` | `Views/Facilities/Details.cshtml` |
| GET, POST `/lines/create?facilityId=`, `/lines/edit/{id}` | `LinesController.Create`, `.Edit` | Manager | `LineFormViewModel { Id?, FacilityId, FacilityName, Code, Name, IsActive, CodeLocked }` | `Views/Lines/Form.cshtml` |
| GET `/shifts?facilityId=` | `ShiftsController.Index` | Manager | `ShiftListViewModel { FacilityId, Facilities[] { Id, Name }, Items[] { Id, Name, StartTime, EndTime, EndsNextDay, IsActive } }` | `Views/Shifts/Index.cshtml` |
| GET, POST `/shifts/create?facilityId=`, `/shifts/edit/{id}` | `ShiftsController.Create`, `.Edit` | Manager | `ShiftFormViewModel { Id?, FacilityId, FacilityName, Name, StartTime, EndTime, IsActive }` | `Views/Shifts/Form.cshtml` |
| GET `/machines` | `MachinesController.Index` | Manager | `MachineListViewModel { Items[] { Id, Code, Name, FacilityName, LineName, IsActive, RetiredAtLocal? }, FacilityId?, LineId?, Query, ShowRetired, Facilities[], Lines[], Page, PageSize, Total }` | `Views/Machines/Index.cshtml` |
| GET, POST `/machines/create`, `/machines/edit/{id}` | `MachinesController.Create`, `.Edit` | Manager | `MachineFormViewModel { Id?, FacilityId, LineId, Code, Name, CodeLocked, Facilities[], Lines[] }` | `Views/Machines/Form.cshtml` |
| GET `/machines/lines?facilityId=` | `MachinesController.Lines` | Manager | JSON `[{ id, label }]` | - |
| GET, POST `/machines/retire/{id}` | `MachinesController.Retire` | Manager | `MachineRetireViewModel { MachineId, Code, Name, BlockingRecords[] { RecordNo, StatusLabel, Url }, CanRetire }` | `Views/Machines/Retire.cshtml` |
| GET `/materials` | `MaterialsController.Index` | Manager | `MaterialListViewModel { Items[] { Id, Code, Name, IsActive, LotCount, ExpiredLotCount }, Query, ShowInactive, Page, PageSize, Total }` | `Views/Materials/Index.cshtml` |
| GET, POST `/materials/create`, `/materials/edit/{id}` | `MaterialsController.Create`, `.Edit` | Manager | `MaterialFormViewModel { Id?, Code, Name, IsActive }` | `Views/Materials/Form.cshtml` |
| GET `/materials/details/{id}` | `MaterialsController.Details` | Manager | `MaterialDetailViewModel { Id, Code, Name, IsActive, Lots[] { Id, LotNo, ExpiryDate, IsExpired } }` | `Views/Materials/Details.cshtml` |
| GET, POST `/material-lots/create?materialId=`, `/material-lots/edit/{id}` | `MaterialLotsController.Create`, `.Edit` | Manager | `MaterialLotFormViewModel { Id?, MaterialId, MaterialName, LotNo, ExpiryDate }` | `Views/MaterialLots/Form.cshtml` |

**Dosya sınırı:** frontend `Views/Facilities/**`, `Views/Lines/**`, `Views/Shifts/**`, `Views/Machines/**`, `Views/Materials/**`, `Views/MaterialLots/**`,
`wwwroot/js/CascadeSelect.js`, menü; backend ilgili controller'lar, `Services/` tanım servisleri, `ViewModels/Definitions/**`.

**Kabul kriterleri**

- [ ] K1: Yönetici tesis, hat ve makine ekler; aynı tesis kodu, aynı tesiste aynı hat kodu, aynı hatta aynı makine kodu ve tire içeren kod alan altında nedeniyle reddedilir (BR-54).
- [ ] K2: Kaydedilmiş tesis, hat ve makinenin kod alanı düzenleme formunda değiştirilemez; istek değiştirilerek gönderilen yeni kod ya da makine için yeni hat reddedilir (PL-04).
- [ ] K3: Makine formunda hat listesi seçilen tesise göre gelir; JS kapalıyken form sunucu tarafında aynı sonucu verir.
- [ ] K4: Aynı tesiste çakışan iki aktif vardiya reddedilir; bir vardiyanın bitişiyle sonrakinin başlangıcı aynı saat olabilir; bitişi başlangıçtan küçük vardiya "ertesi gün biter" olarak listelenir; pasif vardiya çakışma kontrolüne girmez (K-38).
- [ ] K5: Yönetici makineyi kullanımdan kaldırır; liste "Kullanımdan kaldırıldı" ve anını gösterir; makine silinmez (FR-02, BR-05).
- [ ] K6: SKT'si bugünden önce olan lot listede metinli "SKT geçmiş" rozetiyle görünür; bugünkü SKT geçmiş sayılmaz.
- [ ] K7: Operatör ve Denetçi bu fazın tüm adreslerinde 403 alır (BR-34).
- [ ] K8: Liste ekranlarında sayfalama ve filtre sunucuda çalışır; 1440 px'te tablo, 390 px'te yatay kaydırmasız görünüm.

**Kapsam dışı:** Toplu içe aktarım (K-19), kullanıcıya tesis ataması (K-20).

---

## Faz 3: Temizlik tipi ve prosedür versiyonu

**Amaç:** Yöneticinin temizlik tiplerini tanımlayıp makinelerle eşlemesi, faz ve adımlardan oluşan prosedürü taslakta hazırlayıp versiyon olarak yayınlaması.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Temizlik tipi (tür: Planlı, Acil), makine eşlemesi, prosedür versiyonu, faz, adım, adım medyası tabloları; tip başına tek "Yürürlükte" ve tek "Taslak" için kısmi unique index; yayınlanmış versiyonun faz, adım ve medya satırlarında UPDATE ve DELETE'i reddeden tetikleyiciler (BR-04). Seed: bir planlı ve bir acil tip, yayınlanmış birer versiyon | backend-engineer talebi, dbadmin |
| 2 | BE | Tip servisi: varsayılan kilit süresi yalnız planlı tipte zorunlu ve `<AzamiKilitSüresi>`'ni (ayar servisinden) aşamaz (BR-45); tür oluşturulduktan sonra değişmez (PL-04); acil tip makine eşlemesi almaz (BR-50) | backend-engineer |
| 3 | BE | Versiyon servisi: yeni taslak yürürlüktekinin derin kopyasıdır (Brief §7.e); taslakta faz ve adım ekleme, düzenleme, sıralama, çıkarma (PL-03); yayınlama tek transaction'da eskisini Arşiv yapar, versiyon no sistemce artar, PIN ister; yayın koşulu: en az bir faz, her fazda en az bir adım, planlı tipte her fazda minimum süre (PL-14) | backend-engineer |
| 4 | BE | Medya: JPEG, PNG, WebP, MP4 (K-26), `<AzamiMedyaBoyutu>` (ayar servisinden, fotoğraf ve video ayrı), `wwwroot` dışında içerik özetiyle adlandırılmış dosya, yetkili action üzerinden sunum, video için aralıklı okuma (Brief §9.6) | backend-engineer |
| 5 | BE | Tip pasifleştirme; engelleyen kayıt kontrolü arayüzü bu fazda boş döner, Faz 4'te dolar (BR-35) | backend-engineer |
| 6 | FE | E-09 Tipler ve makine eşleme (tesis ve hat gruplu seçim), E-10 Tip detayı ve versiyon geçmişi, E-11 Taslak düzenleyici (faz ve adım listesi, sıralama, medya yükleme, yayınlama PIN diyaloğu), E-12 Versiyon görüntüleme | frontend-engineer |

**Paralel:** Evet. Gerekçe: şema Brief §7.g'de belli; düzenleyicinin her işlemi ayrı route olarak aşağıda; frontend JS'i yalnız bu route'ları çağırır.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/cleaning-types` | `CleaningTypesController.Index` | Manager | `CleaningTypeListViewModel { Items[] { Id, Code, Name, Kind, IsActive, CurrentVersionNo?, HasDraft, MachineCount }, Kind?, ShowInactive }` | `Views/CleaningTypes/Index.cshtml` |
| GET, POST `/cleaning-types/create`, `/cleaning-types/edit/{id}` | `CleaningTypesController.Create`, `.Edit` | Manager | `CleaningTypeFormViewModel { Id?, Code, Name, Kind, KindLocked, DefaultLockMinutes?, MaxLockMinutes, IsActive }` | `Views/CleaningTypes/Form.cshtml` |
| GET, POST `/cleaning-types/machines/{id}` | `CleaningTypesController.Machines` | Manager | `MachineMappingViewModel { CleaningTypeId, CleaningTypeName, Groups[] { FacilityName, LineName, Machines[] { Id, Code, Name, IsActive, IsSelected } }, SelectedMachineIds[] }` | `Views/CleaningTypes/Machines.cshtml` |
| GET `/cleaning-types/details/{id}` | `CleaningTypesController.Details` | Manager | `CleaningTypeDetailViewModel { Id, Code, Name, Kind, DefaultLockMinutes?, IsActive, MachineCount, Versions[] { Id, VersionNo, Status, PublishedAtLocal?, PublishedByName?, DocumentRef }, DraftId?, CanCreateDraft }` | `Views/CleaningTypes/Details.cshtml` |
| POST `/procedure-versions/create-draft` | `ProcedureVersionsController.CreateDraft` | Manager | form `{ CleaningTypeId }`, başarıda düzenleyiciye yönlenir | - |
| GET `/procedure-versions/edit/{id}` | `ProcedureVersionsController.Edit` | Manager | `ProcedureDraftViewModel { Id, CleaningTypeId, CleaningTypeName, Kind, VersionNo, MaterialRequired, DocumentRef, Phases[] { Id, OrderNo, Title, MinDurationMinutes?, Steps[] { Id, OrderNo, Title, Description, Media[] { Id, MediaType, OriginalFileName, Url } } }, PublishBlockers[] }` | `Views/ProcedureVersions/Edit.cshtml` |
| POST `/procedure-versions/update-header/{id}` | `.UpdateHeader` | Manager | `{ MaterialRequired, DocumentRef }` | - |
| POST `/procedure-versions/add-phase/{id}`, `/update-phase/{phaseId}`, `/move-phase/{phaseId}`, `/remove-phase/{phaseId}` | `.AddPhase`, `.UpdatePhase`, `.MovePhase`, `.RemovePhase` | Manager | `{ Title, MinDurationMinutes? }`; taşımada `{ Direction: Up, Down }` | - |
| POST `/procedure-versions/add-step/{phaseId}`, `/update-step/{stepId}`, `/move-step/{stepId}`, `/remove-step/{stepId}` | `.AddStep`, `.UpdateStep`, `.MoveStep`, `.RemoveStep` | Manager | `{ Title, Description }`; taşımada `{ Direction }` | - |
| POST `/procedure-versions/upload-media/{stepId}`, `/remove-media/{mediaId}` | `.UploadMedia`, `.RemoveMedia` | Manager | multipart `{ File }` | - |
| POST `/procedure-versions/publish/{id}` | `.Publish` | Manager | `{ Pin }`; engel varsa `PublishBlockers[]` ile düzenleyiciye döner | - |
| GET `/procedure-versions/details/{id}` | `.Details` | ManagerOrAuditor | `ProcedureVersionViewModel { Id, CleaningTypeName, Kind, VersionNo, Status, PublishedAtLocal?, PublishedByName?, DocumentRef, MaterialRequired, Phases[] { OrderNo, Title, MinDurationMinutes?, Steps[] { OrderNo, Title, Description, Media[] } } }` | `Views/ProcedureVersions/Details.cshtml` |
| GET `/media/{id}` | `MediaController.Show` | RecordReaders | dosya akışı, aralıklı okuma | - |

Düzenleyici POST'ları JS kapalıyken tam sayfa yönlendirmeyle, JS açıkken aynı route'a fetch ile çalışır; ikisinde de sonuç düzenleyicinin yeniden çizimidir.

**Dosya sınırı:** frontend `Views/CleaningTypes/**`, `Views/ProcedureVersions/**`, `wwwroot/js/ProcedureEditor.js`, `wwwroot/js/MediaViewer.js`;
backend `Controllers/CleaningTypesController.cs`, `Controllers/ProcedureVersionsController.cs`, `Controllers/MediaController.cs`, ilgili servisler, `ViewModels/Procedures/**`.

**Kabul kriterleri**

- [ ] K1: Yönetici planlı tip açar, varsayılan kilit süresi girer; `<AzamiKilitSüresi>` üstü değer reddedilir. Acil tip formunda kilit süresi alanı ve makine eşleme bağlantısı yoktur.
- [ ] K2: Kaydedilmiş tipin türü değiştirilemez (PL-04).
- [ ] K3: Planlı tip makinelerle eşlenir; pasif makine eşleme listesinde seçilemez.
- [ ] K4: Taslakta faz ve adım eklenir, sıralanır, çıkarılır; planlı tipte minimum süresi boş faz varken yayınlama "Faz <n> minimum süresi yok" engeliyle reddedilir (PL-14); fazsız ya da adımsız fazlı taslak yayınlanamaz.
- [ ] K5: Yayınlama yanlış PIN'le yapılmaz; doğru PIN'le versiyon "Yürürlükte", önceki "Arşiv" olur, versiyon no bir artar.
- [ ] K6: Yürürlükte bir versiyon varken yeni taslak açılır; taslak yürürlüktekinin faz, adım ve medyasını taşır; ikinci taslak açma isteği reddedilir.
- [ ] K7: Yayınlanmış versiyonun faz, adım ve medya satırı QA kopyasında SQL UPDATE ve DELETE ile değiştirilemez (tetikleyici hatası); düzenleme route'ları yayınlanmış versiyon için reddedilir (BR-04).
- [ ] K8: JPEG, PNG, WebP ve MP4 yüklenir; başka biçim ve `<AzamiMedyaBoyutu>` üstü dosya reddedilir; aynı dosya iki kez yüklenince diskte tek kopya kalır; video tarayıcıda ileri sarılabilir (NFR-10).
- [ ] K9: `/media/{id}` oturumsuz istekte giriş sayfasına yönlenir.
- [ ] K10: Denetçi `/procedure-versions/details/{id}` açar, düzenleme route'larında 403 alır; Operatör tüm bu fazın ekranlarında 403 alır.
- [ ] K11: Düzenleyici 1024 px'te ve klavyeyle kullanılabilir (sıralama düğmeleri odaklanabilir, odak görünür).

**Kapsam dışı:** Yayın için ikinci onay ve ileri tarihli yürürlük (K-24), adım sonuç alanları (K-27).

---

## Faz 4: Kayıt açma ve makine kilidi

**Amaç:** Operatörün makine ve tip seçerek kayıt açması, kaydın makineyi kilitlemesi, kilit süresi dolunca zaman aşımına düşmesi; olay günlüğünün ilk kez yazılması.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Temizlik kaydı, faz ve adım yürütmeleri, adım görevlileri, kayıt malzemeleri, olay günlüğü, işlenmiş komutlar ve numara sayaçları tabloları. Önden gelen kolonlar (PL-02): saha referansı, başlangıç, tamamlanma ve kapanış zamanları, brüt, mola, bekleme, net ve efor toplamları, acil açıklaması, faz süre altı alanları, adım zaman, adım molası (SH-17, PL-19), süre ve efor alanları, görevli "çalışıyor" bayrağı | backend-engineer talebi, dbadmin |
| 2 | DB | Kısmi unique index'ler: makine başına tek Açıldı, Devam ediyor ya da Beklemede kayıt (BR-23); sahip başına tek Açıldı ya da Devam ediyor kayıt (BR-37); kişi başına tek çalışılan adım (BR-24). Unique: kayıt no, saha referansı, komut kimliği, kayıt içi olay sırası. Tetikleyiciler: olay günlüğünde UPDATE ve DELETE ret (BR-31); adımda dolu başlangıç ve bitiş zamanının değişmesi ret (BR-28). SQLite WAL ve busy timeout | backend-engineer talebi, dbadmin |
| 3 | BE | Olay yazıcı: her durum değişikliği aynı transaction'da olay yazar; özet = HMAC-SHA256(anahtar, önceki özet + kanonik olay), anahtar ortam değişkeninden (BR-32, NFR-12); komut kimliğiyle tekrar istekte ilk sonuç döner (BR-33) | backend-engineer |
| 4 | BE | Kayıt açma servisi: BR-01, BR-02 sunucu doğrulaması; yürürlükteki versiyona bağlama ve faz, adım satırlarını Bekliyor olarak oluşturma, sahibi her adıma varsayılan görevli yazma (BR-03, BR-17); açılış anı değerleri (BR-06); kayıt no (BR-08); iş emri planlıda zorunlu (K-18); kilit süresi tipten, `<AzamiKilitSüresi>` sınırı (BR-45); çakışmada sahip adı, durum ve açılış saatiyle hata | backend-engineer |
| 5 | BE | `IDurationCalculator` ilk metotları: SH-13 ve zaman aşımı kararı; zaman aşımı kayda ya da makineye dokunan her istekte ve pano sorgusunda aynı transaction'da uygulanır, zaman aşımı anı kilit bitişidir (BR-26) | backend-engineer |
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

**Dosya sınırı:** frontend `Views/MyWork/**`, `Views/Records/Create.cshtml`, `Views/Records/Run.cshtml`, `Views/MachineBoard/**`, `wwwroot/js/CommandForm.js`,
`wwwroot/js/LotPicker.js`, `wwwroot/js/CascadeSelect.js`; backend `Controllers/MyWorkController.cs`, `Controllers/RecordsController.cs`,
`Controllers/MachineBoardController.cs`, `Services/` (kayıt açma, olay yazıcı, numara, `IDurationCalculator`), `ViewModels/Records/**`.

**Kabul kriterleri**

- [ ] K1: Yeni kayıtta hat listesi seçilen tesise, makine listesi seçilen hatta göre gelir; pasif ve kullanımdan kaldırılmış makine seçilemez; tip listesinde yalnız makineye eşli, yürürlükte versiyonu olan planlı tipler vardır (BR-01, BR-02).
- [ ] K2: İstek değiştirilerek başka hattın makinesi ya da eşlemesiz tip gönderilince kayıt açılmaz ve olay yazılmaz (BR-02).
- [ ] K3: Planlı iş emri boşken kayıt açılmaz (K-18).
- [ ] K4: Kayıt açılınca yürütme ekranı açılır; kayıt no `<TesisKodu>-<HatKodu>-<MakineKodu>-P<Yıl>-<Sıra>` biçimindedir, aynı makinede aynı yıl ikinci kayıtta sıra bir artar; durum "Açıldı"; saha referansı boştur (BR-08, BR-09).
- [ ] K5: Kilit bitişi: açılışı içeren vardiyası olan tesiste en erken (açılış + kilit süresi, vardiya bitişi), vardiyasız tesiste ve iki vardiya arasındaki boşlukta açılış + kilit süresi; QA iki tesiste dener, gece yarısını geçen vardiyayı da dener (SH-13, SC-36 kilit kısmı). Kayıt açıldıktan sonra vardiyanın bitiş saati değiştirilince açık kaydın kilit bitişi değişmez (K-38).
- [ ] K6: Kilit süresi `<AzamiKilitSüresi>` üstündeyse kayıt açılmaz (BR-45).
- [ ] K7: Makinede Açıldı ya da Devam ediyor kayıt varken ikinci açma reddedilir; mesajda sahip adı, durum ve açılış saati vardır. İki oturumdan eşzamanlı iki açma isteğinde tek kayıt oluşur (SC-04, BR-23).
- [ ] K8: Aktif kaydı olan operatör başka makinede kayıt açamaz; mesaj mevcut kaydın numarasını bağlantıyla gösterir; eşzamanlı iki istekte de tek kayıt oluşur (SC-29, BR-37).
- [ ] K9: Kilit bitişi geçmiş Açıldı kayıt, makine panosu açılınca "Zaman aşımı" olur ve makine Boş görünür; olay günlüğünde zaman aşımı anı kilit bitişidir; aynı kayda malzeme ekleme isteği reddedilir; başka operatör makine için kayıt açabilir (SC-15, BR-26, BR-46).
- [ ] K10: SKT geçmiş lot seçicide "SKT geçmiş" uyarısıyla görünür; gerekçesiz ekleme reddedilir; gerekçeyle eklenen satır işaretli görünür (SC-08).
- [ ] K11: Malzeme ekleme ve çıkarma yalnız sahibin PIN'iyle yapılır; başka operatörün doğrudan isteği reddedilir; ekleme kilit bitişini değiştirmez (BR-46, BR-47).
- [ ] K12: Aynı `CommandId` ile iki kez gönderilen açma isteği tek kayıt ve tek olay üretir (SC-23).
- [ ] K13: Tarayıcı çevrimdışıyken "Kaydı oluştur" işlemin yapılmadığını gösterir; çevrimiçi olunca yeniden deneme tek kayıt üretir (SC-33).
- [ ] K14: QA kopyasında olay günlüğü satırına SQL UPDATE ve DELETE tetikleyici hatasıyla reddedilir (BR-31).
- [ ] K15: Her olay önceki olayın özetini taşır; özet anahtarı `appsettings*.json` ve kaynak kodda yoktur (BR-32, NFR-12).
- [ ] K16: `IDurationCalculator` SH-13 ve zaman aşımı metotlarının XML yorumunda formül ve kimlik yazılıdır.
- [ ] K17: Kayıt yürürlükteki versiyona bağlanır; kayıt Açıldı iken yeni versiyon yayınlansa da kaydın versiyon no'su değişmez (BR-03, SC-20).
- [ ] K18: Makine durum panosu Operatör, Yönetici ve Denetçi'de açılır, yalnız Süper admin rolündeki kullanıcıda 403 döner; durum metinle yazılıdır; zaman aşımına düşmesi gereken kayıt panoda Açıldı görünmez.
- [ ] K19: Aktif kaydı olan makine kullanımdan kaldırılamaz ve tipi pasifleştirilemez; sahibi pasifleştirilemez; ekranlar engelleyen kaydı bağlantıyla listeler (BR-35, BR-36).
- [ ] K20: Operatör `/` adresinde `/my-work`'e yönlenir; aktif kaydı varsa kart numara, makine, durum ve kilit bitişiyle görünür; Yönetici rolü tek başına olan kullanıcı `/records/create` ve `/my-work` için 403 alır (K-29).

**Kapsam dışı:** Adım yürütme (Faz 5), kayıt detayı ve listesi (Faz 6), acil kayıt (Faz 8).

---

## Faz 5: Adım yürütme, görevli ve mola

**Amaç:** Operatörün adımları sırayla PIN'le başlatıp bitirmesi, görevlilerin kendi PIN'iyle katılması, iki adım arasında ya da adım sürerken
mola vermesi ve kaydın otomatik tamamlanması.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Mola tablosu: başlatan, molanın başladığı anda devam eden adım (boş olabilir, PL-19), zamanlar, bitiren, kapanış türü (Kişi, Yönetici, Müdahale), gerekçe; kayıt başına ve kişi başına tek açık mola için kısmi unique index (BR-40); dolu mola zamanlarının değişmesini reddeden tetikleyici | backend-engineer talebi, dbadmin |
| 2 | BE | Adım servisi: SK-1 (sahip ya da görevli), BR-14 katı sıra, BR-15 tek devam eden adım, BR-24 tüm görevliler boşta, BR-11 zorunlu malzeme, BR-46 kilit kontrolü, BR-39 kayıtta açık mola varken başlatma ve bitirme reddi; ilk adımda saha referansı (BR-09); bitirmede SH-17, SH-01, SH-02; faz son adımında SH-07 ve gerekçe (BR-21); faz ve kayıt otomatik tamamlanma, makine kilidinin kalkması (BR-22) | backend-engineer |
| 3 | BE | Görevli servisi: yalnız sahip, adım başlamadan ekler ve çıkarır (SK-2); eklenen kişinin PIN'i aynı istekte doğrulanır, hatalı deneme onun sayacına yazılır (SK-4, BR-53); pasif, PIN'i kilitli ya da molada kişi eklenemez; kendi kaydı devam eden kişiye uyarı bilgisi (K-41) | backend-engineer |
| 4 | BE | Mola servisi (K-39 kararı): BR-38 başlatma koşulu (kayıt Devam ediyor; iki adım arasında ya da adım sürerken; başlatan sahip ya da o anki adımın görevlisi; başlatan başka kayıtta devam eden adımda değil), devam eden adım varsa molaya bağlanır; BR-39 kayıt ve kişi kilidi (molada kişinin tüm yazma istekleri servis katmanında reddedilir, controller `/breaks/current`'a yönlendirir; kayıtta adım başlatma, bitirme, görevli ve malzeme reddedilir); BR-40 yalnız başlatan bitirir, adım "Devam ediyor" olarak sürer | backend-engineer |
| 5 | BE | `IDurationCalculator`: SH-17, SH-01, SH-02, SH-03, SH-04, SH-06 (bekleme terimsiz), SH-07, SH-08, SH-09, SH-11 (bekleme terimsiz), SH-12, SH-14 (açık molada değer artmaz); açık iş genişlemesi: tamamlanmamış adımda görevlilik ve açık mola pasifleştirmeyi engeller (BR-36) | backend-engineer |
| 6 | FE | E-15 Yürütme ekranının tam hali: sıradaki adım belirgin, kilitli adımlar ve kilit nedeni, adım açıklaması ve medyası, başlat ve bitir (PIN), faz minimumu ve canlı net süre, süre altı gerekçe alanı, mola düğmesi (adım sürerken de), açık mola bandı (molada olan kişi, başlangıç, "Sayaçlar durdu"), molalar listesi, tamamlanma özeti; E-17 Görevli paneli (kişi arama, katılım PIN'i, K-41 uyarısı); E-18 Mola ekranı (sürdüğü adım varsa adı); İşlerim'de görevli olduğum adımlar ve açık mola | frontend-engineer |
| 7 | FE | `LiveTimer.js` (sunucunun verdiği saniyeden gösterimi ilerletir, `TimersRunning` false iken ilerletmez, hesap yapmaz), `ParticipantPicker.js` | frontend-engineer |

**Paralel:** Evet. Gerekçe: ekranın tüm kararları ViewModel bayraklarından gelir (`CanStart`, `CanFinish`, `CanManageParticipants`, `CanStartBreak`, `TimersRunning`); route'lar aşağıda; dosyalar ayrık.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/records/run/{id}` | `RecordsController.Run` | Operator | Faz 4 alanlarına ek: `Phases[] { GrossSeconds?, BreakSeconds?, NetSeconds?, LiveNetSeconds?, BelowMinimum, BelowMinimumReason, Steps[] { Description, Media[] { Id, MediaType, Url }, StartedAtLocal?, StartedByName?, EndedAtLocal?, EndedByName?, BreakSeconds?, DurationSeconds?, EffortSeconds?, LiveElapsedSeconds?, IsLastOfPhase, Participants[] { UserId, FullName, RegistryNo, IsOwner, ConfirmedAtLocal }, CanStart, CanFinish, CannotStartReason, CannotFinishReason, CanManageParticipants } }, Breaks[] { StartedByName, StartedAtLocal, EndedAtLocal?, DurationSeconds?, EndKindLabel, DuringStepTitle? }, OpenBreak? { Id, StartedByName, StartedAtLocal, DuringStepTitle? }, TimersRunning, CanStartBreak, CannotBreakReason, Summary { GrossSeconds?, BreakSeconds?, NetSeconds?, EffortSeconds?, StartedAtLocal?, CompletedAtLocal? }, FirstStepStartedByName? ` | `Views/Records/Run.cshtml` |
| POST `/records/start-step/{stepId}` | `RecordsController.StartStep` | Operator | `{ CommandId, Pin }` | - |
| POST `/records/finish-step/{stepId}` | `RecordsController.FinishStep` | Operator | `{ CommandId, Pin, BelowMinimumReason? }`; süre altındaysa `FinishStepViewModel { StepId, PhaseTitle, PhaseNetSeconds, MinDurationSeconds, CommandId, BelowMinimumReason, Pin }` ile gerekçe formu döner | `Views/Records/FinishStep.cshtml` |
| GET `/records/people?q=&stepId=` | `RecordsController.People` | Operator | JSON `[{ userId, fullName, registryNo, isAvailable, unavailableReason, hasOwnActiveRecord }]` | - |
| POST `/records/add-participant/{stepId}` | `RecordsController.AddParticipant` | Operator | `{ CommandId, ParticipantUserId, OwnerPin, ParticipantPin }` | - |
| POST `/records/remove-participant/{stepId}` | `RecordsController.RemoveParticipant` | Operator | `{ CommandId, ParticipantUserId, Pin }` | - |
| POST `/breaks/start/{recordId}` | `BreaksController.Start` | Operator | `{ CommandId, Pin }` | - |
| GET `/breaks/current` | `BreaksController.Current` | Operator | `CurrentBreakViewModel { BreakId, RecordId, RecordNo, MachineLabel, StartedAtLocal, ElapsedSeconds, DuringStepTitle?, CommandId }` | `Views/Breaks/Current.cshtml` |
| POST `/breaks/end/{breakId}` | `BreaksController.End` | Operator | `{ CommandId, Pin }` | - |
| GET `/my-work` | `MyWorkController.Index` | Operator | Faz 4'e ek: `AssignedSteps[] { RecordId, RecordNo, MachineLabel, StepTitle, OwnerName, Url }`, `OpenBreak? { BreakId, RecordNo, StartedAtLocal }` | `Views/MyWork/Index.cshtml` |

Açık molası olan kullanıcı `/my-work` ve `/records/run/{id}` GET'lerinde `/breaks/current`'a yönlenir (Brief §7.c). Kayıtta mola açıkken
diğer kullanıcılar yürütme ekranını görür: `TimersRunning` false, `OpenBreak` dolu, `CanStart` ve `CanFinish` false, neden "Kayıtta açık mola var".
`DuringStepTitle` molanın başladığı anda devam eden adımın başlığıdır, iki adım arasındaki molada boştur.

**Dosya sınırı:** frontend `Views/Records/Run.cshtml`, `Views/Records/FinishStep.cshtml`, `Views/Records/_Participants.cshtml`, `Views/Breaks/**`,
`Views/MyWork/**`, `wwwroot/js/LiveTimer.js`, `wwwroot/js/ParticipantPicker.js`; backend `Controllers/RecordsController.cs`, `Controllers/BreaksController.cs`,
adım, görevli ve mola servisleri, `Services/DurationCalculator.cs`, `ViewModels/Records/**`, `ViewModels/Breaks/**`.

**Kabul kriterleri**

- [ ] K1: Sıradaki adım belirgin, sonraki adımlar kilitli ve nedeni yazılı; 2. adım bitmeden 3. adımı başlatma isteği reddedilir (SC-18, BR-14); bir adım devam ederken başka adım başlatılamaz (BR-15).
- [ ] K2: Görevli olmayan operatör yürütme ekranını görür, başlat ve bitir düğmesi görmez; doğrudan POST reddedilir, adım durumu değişmez (SC-09, BR-16).
- [ ] K3: Sahip görevli ekler, eklenen kişi kendi PIN'ini girer ve görevli olur; olay ekleyeni ve onaylayanı taşır; yanlış katılım PIN'inde kişi eklenmez ve hatalı deneme o kişinin sayacına yazılır (SC-32).
- [ ] K4: Pasif, PIN'i kilitli ya da açık molası olan kişi eklenemez, nedeni yazılır; adım başladıktan sonra ekleme ve çıkarma reddedilir; sahip olmayan görevlinin ekleme isteği reddedilir (BR-53, SK-2).
- [ ] K5: Sahip kendini bir adımdan çıkarır ve o adımı yine başlatıp bitirir; efor yalnız listedeki görevlilere yazılır (SC-19).
- [ ] K6: Görevlilerden biri başka kayıtta devam eden adımdaysa adım başlamaz, mesaj kişiyi ve kayıt numarasını gösterir; iki eşzamanlı başlatmada kişi tek devam eden adımda kalır (SC-05, BR-24).
- [ ] K7: Kendi kaydı Devam ediyor olan kişi başka kayıtta görevli eklenirken K-41 uyarısı görünür ve ekleme yapılabilir.
- [ ] K8: Malzeme zorunlu tipte malzemesiz ilk adım başlatma reddedilir ve olay üretmez (SC-07, BR-11).
- [ ] K9: Planlı kayıtta ilk adım başlayınca saha referansı `<TesisKodu>-SD-<Yıl>-<Sıra>` üretilir ve ekranda görünür, kayıt "Devam ediyor" olur; kilit bitişi geçmişse ilk adım başlamaz ve kayıt Zaman aşımı olur (BR-09, BR-46).
- [ ] K10: Molasız, iki görevlili adımda adım süresi = bitiş - başlangıç, efor = süre x 2; QA değerleri veritabanındaki zamanlarla karşılaştırır (SH-01, SH-02).
- [ ] K11: Fazın son adımı, faz net süresi minimumun altındayken gerekçesiz bitirilemez ve istek olay üretmez; gerekçeyle bitirince faz "Tamamlandı - süre altında" olur, bitiş zamanı gerekçeli isteğin sunucu saatidir, gerekçe ekranda görünür (SC-30, BR-21).
- [ ] K12: Son fazın son adımı bitince kayıt "Tamamlandı" olur, tamamlanma zamanı yazılır, makine panosunda makine Boş görünür (BR-22, SC-01 yürütme kısmı).
- [ ] K13: Mola "Devam ediyor" kayıtta hem iki adım arasında hem bir adım devam ederken başlatılır; "Açıldı" kayıtta mola düğmesi yoktur ve istek reddedilir. Sahip ya da o anki adımın görevlisi başlatabilir; kayıtta görevi olmayan operatörün isteği reddedilir (BR-38, SK-6).
- [ ] K13a: Mola açıkken kayıtta adım başlatma ve devam eden adımı bitirme istekleri "Kayıtta açık mola var" ile reddedilir, olay üretmez; devam eden adım "Devam ediyor" kalır; görevli ve malzeme istekleri reddedilir (BR-39, SC-26).
- [ ] K14: Molayı başlatan kişinin başka kayıtta adım başlatma, yeni kayıt açma, görevli ekleme ve malzeme istekleri reddedilir ve `/breaks/current`'a yönlenir; molayı bitirebilir ve oturumu kapatabilir; reddedilen istekler olay üretmez (SC-26).
- [ ] K14a: Başka bir kayıtta devam eden adımda görevli olan kişinin mola başlatma isteği reddedilir (BR-38, SC-37 ek koşul).
- [ ] K15: Molayı başlatandan başka operatörün mola bitirme isteği reddedilir (BR-40). Adım sürerken açılan mola başlatan tarafından bitirilince adım "Devam ediyor" olarak sürer ve görevlisi bitirebilir.
- [ ] K16: Mola süresi faz ve kayıt net süresinden düşülür, brüt süreden düşülmez; mola verilmeden geçen boşluk net süreye dahildir (SC-06 a ve b, SC-25).
- [ ] K16a: Adım sürerken verilen mola adım süresinden ve eforundan düşülür. QA iki görevlili bir adımda adım sürerken mola verip bitirir; veritabanındaki zamanlarla adım molası = mola süresi, adım süresi = bitiş - başlangıç - adım molası, efor = adım süresi x 2 olduğunu gösterir. Ayrıca QA, SC-37 zamanlarını (adım 10:00:00-10:50:00, mola 10:04:00-10:44:00, iki görevli) `DurationCalculator` kodundaki formüle uygular ve 40 dk, 10 dk, 20 dk bulur (SH-17, SH-01, SH-02).
- [ ] K16b: Adım içi mola içeren fazda faz net = faz brüt - faz molası ve bu değer fazın adım süreleri toplamı ile adımlar arasındaki molasız boşlukların toplamına eşittir; faz minimumu bu değerle kontrol edilir (SH-06, SH-07).
- [ ] K17: Canlı sayaç sayfa yenilenince sunucudan gelen değerle başlar; JS dosyalarında tarih farkı hesabı yoktur (SH-14). Kayıtta mola açıkken adım, faz ve kayıt sayaçları ilerlemez ve ekranda metinle "Sayaçlar durdu" görünür; mola bitince sayfa yenilendiğinde kaldığı değerden devam eder.
- [ ] K18: Tamamlanmamış adımda görevli ya da açık molası olan kullanıcı pasifleştirilemez; ekran bu işleri bağlantıyla listeler (BR-36).
- [ ] K19: QA kopyasında dolu adım başlangıç zamanı SQL UPDATE ile değiştirilemez (BR-28).
- [ ] K20: `IDurationCalculator`'da SH-17, SH-01, SH-02, SH-03, SH-04, SH-06, SH-07, SH-08, SH-09, SH-11, SH-12, SH-14 kimliklerinin her biri formülüyle XML yorumunda bulunur.
- [ ] K21: Adımın fotoğrafı ve videosu yürütme ekranında açılır; yürütme ekranı 360 px'te yatay kaydırmasız, başlat ve bitir 44 px (FR-06, NFR-04).

**Kapsam dışı:** Yöneticinin molayı bitirmesi ve beklemeye alma (Faz 7), kayıt detayı (Faz 6), kişi düzeyi mola ve adımın ayrıca duraklatılması (K-39, K-21).

---

## Faz 6: Kayıt listesi, detay ve yönetici panosu

**Amaç:** Kayıtların aranması, tek kayıtta 13 temel sorunun yanıtlanması, bütünlüğün doğrulanması ve yöneticinin aktif işleri tek ekranda görmesi.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Liste filtreleri için index'ler (backend'in sorgu listesine göre); yeni tablo yok | backend-engineer talebi, dbadmin |
| 2 | BE | Kayıt sorgu servisi: filtre, arama, sunucu sayfalama; Operatör'de varsayılan "benim kayıtlarım" (sahip ya da görevli) (K-17, PL-08) | backend-engineer |
| 3 | BE | Detay sorgusu: Brief §4.3'teki 13 soru, açılış anı değerleri, versiyon içeriği (FR-09), zaman çizelgesi, olay günlüğü | backend-engineer |
| 4 | BE | Bütünlük doğrulama: (1) özet zinciri, (2) durum tablolarındaki zaman, kişi ve malzeme alanlarının olay değerleriyle aynı olması; ilk tutarsız olay sırası (PL-11, SC-21) | backend-engineer |
| 5 | BE | Pano sorgusu; SH-16 (başladığı vardiya bitmiş açık adım ve açık mola); ana sayfa Yönetici ve Denetçi'yi `/dashboard`'a yönlendirir; `PrototypeController` ve prototip görünümleri kaldırılır (PL-10) | backend-engineer, frontend-engineer |
| 6 | FE | E-19 Kayıtlar, E-20 Kayıt detayı (zaman çizelgesi, görevliler ve onay zamanları, molalar, malzemeler, olay günlüğü, bütünlük rozeti), E-21 Bütünlük doğrulama, E-22 Yönetici panosu | frontend-engineer |

**Paralel:** Evet. Gerekçe: salt okuma ekranları; veri yapısı Faz 4-5'te oluştu; sözleşme aşağıda.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET `/records` | `RecordsController.Index` | RecordReaders | `RecordListViewModel { Items[] { Id, RecordNo, Kind, KindLabel, StatusLabel, FieldBookRef?, FacilityName, LineName, MachineLabel, CleaningTypeName, VersionNo, OwnerName, OpenedAtLocal, CompletedAtLocal?, NetSeconds?, HasBelowMinimum, HasExpiredMaterial, Url }, Filter { Query, FacilityId?, LineId?, MachineId?, Kind?, Status?, From?, To?, MineOnly }, Facilities[], Lines[], Machines[], Page, PageSize, Total }` | `Views/Records/Index.cshtml` |
| GET `/records/details/{id}` | `RecordsController.Details` | RecordReaders | `RecordDetailViewModel { Header { RecordNo, Kind, KindLabel, StatusLabel, FieldBookRef?, Facility, Line, Machine, CleaningTypeName, VersionNo, VersionUrl?, WorkOrderNo, Note, EmergencyDescription? }, People { OwnerName, OwnerRegistryNo, FirstStepStartedByName?, Helpers[] }, Times { OpenedAtLocal, StartedAtLocal?, CompletedAtLocal?, ClosedAtLocal?, LockExpiresAtLocal? }, Summary { GrossSeconds?, BreakSeconds?, HoldSeconds?, NetSeconds?, EffortSeconds? }, Phases[] { OrderNo, Title, MinDurationSeconds?, StatusLabel, GrossSeconds?, BreakSeconds?, NetSeconds?, BelowMinimum, BelowMinimumReason, Steps[] { OrderNo, Title, Description, Media[], StatusLabel, StartedAtLocal?, StartedByName?, EndedAtLocal?, EndedByName?, BreakSeconds?, DurationSeconds?, EffortSeconds?, Participants[] { FullName, RegistryNo, AddedByName, ConfirmedAtLocal } } }, Breaks[] { StartedByName, StartedAtLocal, EndedAtLocal?, DurationSeconds?, EndKindLabel, EndReason, DuringStepTitle? }, Materials[] { MaterialCode, MaterialName, LotNo, ExpiryDate, ExpiredAtEntry, ExpiredReason, EnteredByName, EnteredAtLocal }, Events[] { Sequence, OccurredAtLocal, ActorName, EventLabel, Summary }, Integrity { IsValid, FirstBrokenSequence?, CheckedAtLocal, ReportUrl? }, RunUrl? }` | `Views/Records/Details.cshtml` |
| GET `/records/integrity/{id}` | `RecordsController.Integrity` | ManagerOrAuditor | `IntegrityReportViewModel { RecordId, RecordNo, IsValid, CheckedAtLocal, Events[] { Sequence, EventLabel, OccurredAtLocal, HashShort, IsValid, Problem? }, StateMismatches[] { Field, StoredValue, EventValue, Sequence } }` | `Views/Records/Integrity.cshtml` |
| GET `/dashboard` | `DashboardController.Index` | ManagerOrAuditor | `DashboardViewModel { ActiveRecords[] { RecordId, RecordNo, MachineLabel, StatusLabel, OwnerName, OpenedAtLocal, LockExpiresAtLocal?, CurrentStepTitle? }, NeedsAttention[] { KindLabel, RecordId, RecordNo, MachineLabel, PersonName, StartedAtLocal, ShiftName }, BelowMinimum[] { RecordId, RecordNo, PhaseTitle, NetSeconds, MinDurationSeconds, Reason, CompletedAtLocal }, RecentCompleted[] { RecordId, RecordNo, MachineLabel, CompletedAtLocal, NetSeconds, EffortSeconds }, OpenCount, InProgressCount, CompletedTodayCount, OnHold[] (Faz 7), PendingRequests[] (Faz 7), Emergencies[] (Faz 8) }` | `Views/Dashboard/Index.cshtml` |

**Dosya sınırı:** frontend `Views/Records/Index.cshtml`, `Views/Records/Details.cshtml`, `Views/Records/Integrity.cshtml`, `Views/Dashboard/**`, `Views/Prototype/**` (kaldırma);
backend `Controllers/RecordsController.cs` (okuma action'ları), `Controllers/DashboardController.cs`, `Controllers/PrototypeController.cs` (kaldırma), sorgu ve bütünlük servisleri, `ViewModels/Records/**`, `ViewModels/Dashboard/**`.

**Kabul kriterleri**

- [ ] K1: Tamamlanmış planlı kayıtta Brief §4.3'teki 13 soru detayda yanıtlıdır; QA her birini ayrı işaretler (FR-31, SC-01).
- [ ] K2: "Kaydı açan (sorumlu)" ile "İlk adımı başlatan" ayrı satırlardır (K-35); süreler brüt, mola, net ve efor olarak ayrı etiketlidir (FR-21). Adım içinde mola olan adımın satırında adım molası ayrıca görünür; mola satırı sürdüğü adımı gösterir (SC-37).
- [ ] K3: v2 yayınlandıktan sonra v1 kaydının detayı v1'in faz minimumunu, adım açıklamasını ve medyasını gösterir (SC-11, FR-09).
- [ ] K4: Makine adı sonradan değiştirildiğinde eski kayıt açılış anındaki adı gösterir (BR-06).
- [ ] K5: Operatör `/records`'u yalnız sahibi ya da görevlisi olduğu kayıtlarla açar; filtreyi kaldırınca tüm kayıtları görür (PL-08); tesis, hat, makine, tür, durum, tarih aralığı ve kayıt no, saha referansı, iş emri araması birlikte çalışır; sayfalama sunucuda.
- [ ] K6: Olay günlüğü tüm olayları sırayla, yapan kişi ve sunucu zamanıyla listeler (FR-34, BR-30).
- [ ] K7: Değiştirilmemiş kayıtta rozet "Doğrulandı" yazar. QA kopyasında kaydın iş emri ya da bir malzeme lot değeri SQL ile değiştirilince rozet "Bütünlük ihlali" olur ve doğrulama ekranı alanı, saklanan ve olaydaki değeri gösterir; olay değeri değiştirilmiş kopyada ilk tutarsız olay sırası gösterilir (SC-21, PL-11).
- [ ] K8: `/records/integrity/{id}` ve `/dashboard` Yönetici ve Denetçi'de açılır, Operatör'de 403; Denetçi panoda yazma düğmesi görmez (SC-24).
- [ ] K9: Başladığı vardiya bitmiş açık adım ve açık mola panoda "Müdahale bekleyenler"de kişi, kayıt no, başlangıç ve vardiya adıyla görünür; vardiyası sürenler görünmez (SH-16).
- [ ] K10: Süre altında tamamlanan fazlar panoda gerekçesiyle listelenir.
- [ ] K11: Yönetici ve Denetçi `/` adresinde `/dashboard`'a yönlenir; makine panosundaki kayıt bağlantısı onlarda detayı açar. Yalnız Süper admin rolündeki kullanıcı `/records` ve `/records/details/{id}` için 403 alır.
- [ ] K12: `SH-16` formülü `IDurationCalculator`'da XML yorumunda.
- [ ] K13: `Views/Prototype` ve `PrototypeController` repoda yoktur.

**Kapsam dışı:** Denetim çıktısı ve CSV (Faz 9), müdahale düğmeleri (Faz 7).

---

## Faz 7: Talep, müdahale ve beklemeye alma

**Amaç:** Sahibin yöneticiye gerekçeli talep açması, yöneticinin talepleri karara bağlaması ya da doğrudan müdahale etmesi, kaydın beklemeye alınıp devam ettirilmesi.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Talep ve bekleme tabloları; kayıt başına tek bekleyen talep ve tek açık bekleme için kısmi unique index (BR-41) | backend-engineer talebi, dbadmin |
| 2 | BE | Talep servisi: SK-5, BR-41; türler kayıt durumuna göre (Açıldı: İptal; Devam ediyor: üçü; Beklemede: İptal ve Yarıda kapatma; Brief §6.1); geri çekme; onay işlemi aynı transaction'da uygular, koşul sağlanmazsa talep Bekliyor kalır; ilk karar geçerli (K-40); ret gerekçesi zorunlu; kayıt başka yoldan kapanınca talep Geçersiz (BR-42) | backend-engineer |
| 3 | BE | Müdahale servisi: iptal, yarıda kapatma, beklemeye alma (BR-27, BR-43); devam eden adım Kesildi, açık mola müdahale anında kapanır, makine kilidi iptal ve yarıda kapatmada kalkar; yönetici açık molayı gerekçeyle bitirir (BR-40); sahip beklemedeki kaydı PIN'le devam ettirir (BR-44) | backend-engineer |
| 4 | BE | `IDurationCalculator`: SH-05, SH-10; SH-06, SH-11, SH-14 bekleme terimiyle güncellenir, yorumlar aynı commit'te; açık iş genişlemesi: bekleyen talep ve Beklemede kayıt pasifleştirmeyi, Beklemede kayıt makine kaldırmayı engeller | backend-engineer |
| 5 | FE | E-23 Talep formu (yürütme ekranı ve detayda sahibe), E-24 Taleplerim, E-25 Talepler ve karar formu, E-26 Müdahale formu (iptal, yarıda kapatma, beklemeye alma, mola bitirme); detayda talepler ve beklemeler; panoda Beklemede ve bekleyen talepler; İşlerim'de beklemedeki kayıtlar ve "Devam et"; makine panosunda Beklemede | frontend-engineer |

**Paralel:** Evet. Gerekçe: tablolar yeni, mevcut tabloya dokunulmaz (PL-02); route ve ViewModel aşağıda.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET, POST `/requests/create?recordId=` | `RequestsController.Create` | Operator | `RequestCreateViewModel { CommandId, RecordId, RecordNo, Type, AllowedTypes[], Reason, Pin, PendingRequest? { Id, TypeLabel, RequestedAtLocal } }` | `Views/Requests/Create.cshtml` |
| POST `/requests/withdraw/{id}` | `RequestsController.Withdraw` | Operator | `{ CommandId, Pin }` | - |
| GET `/requests/mine` | `RequestsController.Mine` | Operator | `MyRequestListViewModel { Items[] { Id, RecordId, RecordNo, MachineLabel, TypeLabel, Reason, StatusLabel, RequestedAtLocal, DecidedByName?, DecidedAtLocal?, DecisionNote?, CanWithdraw, CommandId } }` | `Views/Requests/Mine.cshtml` |
| GET `/requests` | `RequestsController.Index` | ManagerOrAuditor | `RequestListViewModel { Items[] { Id, RecordId, RecordNo, MachineLabel, RecordStatusLabel, TypeLabel, Reason, RequestedByName, RequestedAtLocal, StatusLabel, DecidedByName?, DecidedAtLocal?, DecisionNote?, CanDecide }, Status (varsayılan Pending), Page, PageSize, Total }` | `Views/Requests/Index.cshtml` |
| GET `/requests/decide/{id}`; POST `/requests/approve/{id}`, `/requests/reject/{id}` | `RequestsController.Decide`, `.Approve`, `.Reject` | Manager | `RequestDecisionViewModel { CommandId, RequestId, RecordId, RecordNo, TypeLabel, Reason, RequestedByName, RequestedAtLocal, RecordStatusLabel, DecisionNote, Pin, BlockReason? }` | `Views/Requests/Decide.cshtml` |
| GET, POST `/interventions/create?recordId=&type=` | `InterventionsController.Create` | Manager | `InterventionViewModel { CommandId, RecordId, RecordNo, RecordStatusLabel, Type, AllowedTypes[], Reason, Pin, PendingRequest?, BlockReason? }` | `Views/Interventions/Create.cshtml` |
| GET, POST `/interventions/end-break/{breakId}` | `InterventionsController.EndBreak` | Manager | `EndBreakViewModel { CommandId, BreakId, RecordId, RecordNo, StartedByName, StartedAtLocal, Reason, Pin }` | `Views/Interventions/EndBreak.cshtml` |
| POST `/records/resume/{id}` | `RecordsController.Resume` | Operator | `{ CommandId, Pin }` | - |
| Mevcut ekranlara ek alanlar | `Run`, `Details`, `Dashboard`, `MyWork`, `MachineBoard` | - | Run: `CanRequest, PendingRequest?, CanResume, Holds[]`; Details: `Requests[] { TypeLabel, Reason, StatusLabel, RequestedByName, RequestedAtLocal, DecidedByName?, DecisionNote? }, Holds[] { StartedAtLocal, EndedAtLocal?, DurationSeconds?, StartedByName, EndedByName? }, CanIntervene, CanEndBreak`; Dashboard: `OnHold[]`, `PendingRequests[]`; MyWork: `HeldRecords[] { Id, RecordNo, MachineLabel, HeldAtLocal, CanResume }`; MachineBoard: `State = OnHold` | mevcut görünümler |

`Type` değerleri: `Cancel`, `Abort`, `Hold`. Görünür adları: İptal, Yarıda kapatma, Beklemeye alma.

**Dosya sınırı:** frontend `Views/Requests/**`, `Views/Interventions/**`, mevcut `Run`, `Details`, `Dashboard`, `MyWork`, `MachineBoard` görünümlerinde yeni bölümler;
backend `Controllers/RequestsController.cs`, `Controllers/InterventionsController.cs`, `RecordsController.Resume`, talep ve müdahale servisleri, `DurationCalculator`, `ViewModels/Requests/**`, `ViewModels/Interventions/**`.

**Kabul kriterleri**

- [ ] K1: Sahip olmayan operatörün talep isteği reddedilir; gerekçesiz talep açılmaz; bekleyen talep varken ikinci talep açılmaz ve bekleyen talep gösterilir (BR-41, SK-5).
- [ ] K2: Operatör hiçbir ekranda kaydı kendisi iptal etme ya da kapatma düğmesi görmez; `/interventions/*` Operatör'de 403 (K-05).
- [ ] K3: Açıldı kayıtta talep türü yalnız İptal'dir; Devam ediyor kayıtta üç tür, Beklemede kayıtta İptal ve Yarıda kapatma sunulur; istekle gönderilen uygunsuz tür reddedilir.
- [ ] K4: Talepler ekranı varsayılan "Bekliyor" filtresiyle açılır; Denetçi listeyi görür, karar düğmesi görmez, karar POST'u 403 (SC-24).
- [ ] K5: Gerekçesiz ret reddedilir; gerekçeyle ret sonrası talep "Reddedildi", kayıt değişmez, sahip Taleplerim'de karar ve gerekçeyi görür; geri çekilen talep "Geri çekildi" olur ve yönetici listesinde karar düğmesi taşımaz (SC-28).
- [ ] K6: Devam eden adım varken beklemeye alma onayı yapılmaz, neden gösterilir, talep Bekliyor kalır; adım bitince onay yapılır (SC-35).
- [ ] K7: İki yönetici aynı talebi art arda onaylarsa ikincisi "Talep zaten karara bağlandı" mesajı alır ve ikinci olay yazılmaz (K-40).
- [ ] K8: İptalde kayıt "İptal edildi", devam eden adım "Kesildi" ve bitişi müdahale anı, açık mola kapanış türü "Müdahale" ile kapanır, makine Boş olur; kayıt silinmez, kayıt no yeniden kullanılmaz (BR-27, SC-10). Adım sürerken açık mola varken yapılan iptalde Kesildi adımın süresi, molanın müdahale anına kadarki kısmı düşülerek yazılır (SH-17, SH-01).
- [ ] K9: Yarıda kapatmada kayıt "Yarıda kapatıldı", makine Boş olur.
- [ ] K10: Beklemeye alınan kayıt "Beklemede", makine panosunda Beklemede ve kilitli; sahip başka makinede kayıt açabilir; sahip "Devam et"le PIN'le devam ettirir; başka aktif kaydı varken devam reddedilir (SC-27, BR-43, BR-44).
- [ ] K11: Detayda bekleme ayrı satırdır; Net = Brüt - mola - bekleme (SH-10, SH-11) ve faz neti beklemeyi düşer (SH-05, SH-06).
- [ ] K12: Yönetici doğrudan müdahale ettiğinde aynı kayıttaki bekleyen talep "Geçersiz" olur (BR-42).
- [ ] K13: Yönetici açık molayı gerekçe ve PIN'le bitirir; bitiş işlem anıdır; mola satırında kapanış türü "Yönetici" ve gerekçe görünür (BR-40). Mola adım sürerken açılmışsa adım "Devam ediyor" kalır, görevlisi adımı bitirebilir, molayı başlatan kişinin yazma kilidi kalkar.
- [ ] K14: Bekleyen talebi ya da Beklemede kaydı olan kullanıcı pasifleştirilemez; Beklemede kaydı olan makine kullanımdan kaldırılamaz (BR-35, BR-36).
- [ ] K15: SC-16 uçtan uca: yönetici kaydı yarıda kapatır, diğer kaydın sahibi kişiyi adımdan çıkarır, ardından pasifleştirme yapılır; sahip hiçbir aşamada değişmez.
- [ ] K16: Olay günlüğünde talep açma, geri çekme, onay, ret, geçersizlik, beklemeye alma, devam ve müdahale olayları yapan ve zamanla yer alır (BR-30).
- [ ] K17: SH-05 ve SH-10 yeni, SH-06, SH-11 ve SH-14 güncellenmiş formülleriyle XML yorumundadır.

**Kapsam dışı:** Yeni talep bildirimi (Faz 8), düzeltme (Faz 8).

---

## Faz 8: Acil müdahale, bildirim ve düzeltme

**Amaç:** Operatörün acil müdahaleyi tek ekrandan açıp başlatması, yöneticilerin bildirim alması ve yöneticinin yanlış bilgiyi orijinali koruyarak düzeltmesi.

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Bildirim ve kişi başına okundu tabloları; düzeltme tablosu (hedef, orijinal, yeni değer, gerekçe, yapan, zaman) | backend-engineer talebi, dbadmin |
| 2 | BE | Acil açma servisi: yalnız Acil türü, eşleme aranmaz, açıklama zorunlu, zorunlu malzeme aynı formda, açılış ve ilk adım tek transaction ve aynı sunucu anı, kayıt no `A` türü, saha referansı yok; makine ve kişi kısıtları aynen (BR-50, K-44) | backend-engineer |
| 3 | BE | Bildirim servisi: acil açılışında ve yeni talepte her aktif Yönetici'ye bildirim; okundu kişi başına (BR-51); kabuktaki okunmamış sayısı için view component (`Controllers/ViewComponents/`) | backend-engineer |
| 4 | BE | Düzeltme servisi: hedefler lot, görevli listesi, iş emri, açıklama (K-16); zaman alanı hedef olamaz; düzeltme olay olarak zincire girer; görevli düzeltmesinde efor düzeltilmiş sayıyla ayrıca gösterilir, orijinal korunur (PL-09, SH-02 yorumu güncellenir) | backend-engineer |
| 5 | FE | E-27 Acil müdahale (tek ekran), E-28 Bildirimler ve kabukta okunmamış sayısı, E-29 Düzeltme formu; detayda düzeltmeler (orijinal ve yeni değer birlikte); liste, detay ve panoda "Acil" etiketi, panoda acil müdahaleler bölümü; İşlerim'de "Acil müdahale" düğmesi | frontend-engineer |

**Paralel:** Evet. Gerekçe: yeni tablolar, mevcut kayıt akışı servisleri yeniden kullanılır; sözleşme aşağıda.

**Sözleşme**

| Route | Action | Rol | ViewModel (alanlar) | Görünüm |
|---|---|---|---|---|
| GET, POST `/emergencies/create` | `EmergenciesController.Create` | Operator | `EmergencyCreateViewModel { CommandId, FacilityId?, LineId?, MachineId?, CleaningTypeId?, Description, Materials[] { MaterialLotId, ExpiredReason }, Pin, Facilities[], Lines[], Machines[] { Id, Label, IsAvailable, BusyReason }, EmergencyTypes[] { Id, Name, VersionNo, MaterialRequired }, ErrorMessage, ErrorUrl? }`; başarıda `/records/run/{id}` | `Views/Emergencies/Create.cshtml` |
| GET `/emergencies/machines?lineId=` | `EmergenciesController.Machines` | Operator | JSON `[{ id, label, isAvailable, busyReason }]` | - |
| GET `/notifications` | `NotificationsController.Index` | Manager | `NotificationListViewModel { Items[] { Id, KindLabel, RecordId, RecordNo, MachineLabel, Summary, CreatedAtLocal, IsRead, Url }, UnreadOnly, Page, PageSize, Total }` | `Views/Notifications/Index.cshtml` |
| POST `/notifications/mark-read/{id}`, `/notifications/mark-all-read` | `.MarkRead`, `.MarkAllRead` | Manager | - | - |
| view component | `NotificationCountViewComponent` | Manager | `NotificationCountViewModel { UnreadCount }` | `Views/Shared/Components/NotificationCount/Default.cshtml` |
| GET, POST `/corrections/create?recordId=` | `CorrectionsController.Create` | Manager | `CorrectionCreateViewModel { CommandId, RecordId, RecordNo, Target, Targets[] { Value, Label }, TargetItemId?, TargetItems[] { Id, Label, CurrentValue }, NewTextValue?, NewMaterialLotId?, NewParticipantUserIds[], Reason, Pin }` | `Views/Corrections/Create.cshtml` |
| Mevcut ekranlara ek alanlar | `Details`, `Index`, `Dashboard`, `MyWork` | - | Details: `Corrections[] { TargetLabel, OriginalValue, NewValue, Reason, ByName, AtLocal }`, `CanCorrect`; Index: `Kind` filtresi ve etiketi; Dashboard: `Emergencies[] { RecordId, RecordNo, MachineLabel, OwnerName, OpenedAtLocal, StatusLabel }`; MyWork: `CanOpenEmergency` | mevcut görünümler |

`Target` değerleri: `Lot`, `Participants`, `WorkOrder`, `Note`.

**Dosya sınırı:** frontend `Views/Emergencies/**`, `Views/Notifications/**`, `Views/Corrections/**`, `Views/Shared/Components/NotificationCount/**`, mevcut görünümlerde yeni bölümler;
backend `Controllers/EmergenciesController.cs`, `Controllers/NotificationsController.cs`, `Controllers/CorrectionsController.cs`, `Controllers/ViewComponents/**`, ilgili servisler, `ViewModels/Emergencies/**`, `ViewModels/Notifications/**`, `ViewModels/Corrections/**`.

**Kabul kriterleri**

- [ ] K1: Acil müdahale ekranında yalnız Acil türünde, yürürlükte versiyonu olan tipler listelenir; makine eşlemesi aranmaz (BR-50).
- [ ] K2: Gönderimde kayıt açılır ve ilk adım başlar; açılış zamanı ile ilk adım başlangıcı aynı değerdir; kayıt no `...-A<Yıl>-<Sıra>`; saha referansı alanı "yok" yazar (SC-02).
- [ ] K3: Açıklama boşken kayıt açılmaz; malzeme zorunlu acil tipte malzemesiz gönderim reddedilir (SC-02, BR-11).
- [ ] K4: Makine kilitliyse ya da operatörün aktif kaydı varsa acil açılmaz; mesaj kilidi tutan kaydı, sahibini ve durumunu gösterir (K-44).
- [ ] K5: Acil kayıtta fazın son adımı gerekçe istenmeden bitirilir (minimum yok).
- [ ] K6: Acil açılınca ve yeni talep açılınca her aktif Yönetici'nin kabuğundaki okunmamış sayısı bir artar; bildirimi okundu işaretleyen yöneticinin sayısı azalır, diğerininki değişmez (BR-51).
- [ ] K7: `/notifications` Operatör ve Denetçi'de 403.
- [ ] K8: Liste, detay ve panoda acil kayıt metinli "Acil" etiketiyle ayrılır; liste türe göre filtrelenir (FR-15, BR-10).
- [ ] K9: Yönetici yanlış lotu gerekçe ve PIN'le düzeltir; detayda orijinal ve yeni değer birlikte, gerekçe, yapan ve zamanla görünür; düzeltme olay günlüğündedir (BR-29).
- [ ] K10: Düzeltme formunda zaman alanı hedef olarak yoktur; istekle gönderilen zaman hedefi reddedilir (SC-13).
- [ ] K11: Görevli düzeltmesinden sonra detay orijinal ve düzeltilmiş efor değerini birlikte gösterir (PL-09).
- [ ] K12: Düzeltme sonrası bütünlük rozeti "Doğrulandı" kalır.
- [ ] K13: `/corrections/*` Operatör ve Denetçi'de 403.
- [ ] K14: Acil müdahale ekranı 360 px'te yatay kaydırmasız, gönder düğmesi 44 px.

**Kapsam dışı:** E-posta ve SMS (K-43), acilin başka kaydı zaman aşımına düşürmesi (K-44 alternatifi).

---

## Faz 9: Raporlar, denetim çıktısı ve sistem ayarları

**Amaç:** Makine ve faz bazında süre, efor ve minimum uyumunun raporlanması, lot izlenebilirliği, denetimde sunulacak yazdırılabilir kayıt
çıktısı ve sistem ayarlarının ekrandan değiştirilmesi (K-47).

**Kapsam**

| # | Tür | İş kalemi | Sahibi |
|---|---|---|---|
| 1 | DB | Rapor sorguları için index'ler (backend'in sorgu listesine göre); yeni tablo yok | backend-engineer talebi, dbadmin |
| 2 | BE | Süre raporu sorgusu: makine ve faz bazında ortalama brüt, mola, bekleme, net, efor; yalnız Tamamlandı kayıtlar, faz ortalaması versiyon içinde (SH-15); durum sayıları ayrı; süre altı sayısı; en uzun adımlar (FR-40); tür filtresi | backend-engineer |
| 3 | BE | Lot izlenebilirliği: lotla ve makineyle sorgu (FR-37); denetim çıktısı sorgusu; CSV (UTF-8 BOM, `;` ayırıcı); `IDurationCalculator` SH-15 | backend-engineer |
| 4 | BE | Ayar servisi yazma: değer pozitif tam sayı; `<AzamiKilitSüresi>` aktif planlı tiplerin varsayılan kilit süresinin altına inemez, engelleyen tipler listelenir; her değişiklik aynı transaction'da geçmişe yazılır; PIN ister; etkiler PL-18 (açık kayıtların kilit bitişi değişmez, mevcut PIN'ler geçerli) | backend-engineer |
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

- [ ] K1: Süre raporu makine ve faz bazında ortalama brüt, mola, bekleme, net ve eforu ayrı kolonlarda gösterir (FR-21, FR-36).
- [ ] K2: Ortalamalara yalnız Tamamlandı kayıtlar girer; İptal, Yarıda kapatıldı, Zaman aşımı ve Beklemede sayıları ayrı gösterilir; QA bir iptal kaydının ortalamayı değiştirmediğini gösterir (SH-15).
- [ ] K3: Aynı tipin v1 ve v2 faz ortalamaları ayrı satırdır; süre altı faz sayısı faz satırında görünür (FR-22).
- [ ] K4: En uzun süren adımlar listelenir (FR-40); planlı ve acil kayıtlar tür filtresiyle ayrılır (FR-15).
- [ ] K5: Lotla arama lotun kullanıldığı kayıtları makine, tarih ve kayıt no ile; makineyle arama malzeme ve lotları tarih sırasıyla listeler; SKT geçmiş ve düzeltilmiş satırlar işaretlidir (SC-22, FR-37).
- [ ] K6: Denetim çıktısı 13 soruyu, olay günlüğünü, bütünlük sonucunu, versiyonu, düzeltmeleri, SKT işaretlerini ve süre altı gerekçelerini içerir; yazdırma önizlemesinde menü ve üst bar görünmez (SC-14, FR-35).
- [ ] K7: CSV dosyaları Excel'de Türkçe karakterler bozulmadan ve kolonlara ayrılmış açılır.
- [ ] K8: Operatör `/reports/*` ve `/records/audit/*` için 403 alır; Denetçi açar (BR-34, SC-24).
- [ ] K9: Her grafiğin aynı veriyi gösteren tablosu vardır; grafik gerçek veriden SVG ile çizilir (NFR-05).
- [ ] K10: SH-15 formülü `IDurationCalculator`'da XML yorumunda.
- [ ] K11: Son regresyon: Brief §8'deki SC-01..SC-38'in her biri raporda geçti ya da kaldı olarak işaretlenir.
- [ ] K12: Yönetici ve Süper admin `/settings`'te bir değeri PIN'le değiştirir; yanlış PIN'de değer değişmez; geçmişte eski değer, yeni değer, yapan ve zaman görünür. Operatör ve Denetçi `/settings` GET ve POST'ta 403 alır (FR-55).
- [ ] K13: Sıfır, negatif ve tam sayı olmayan değer alan altında nedeniyle reddedilir.
- [ ] K14: `<AzamiKilitSüresi>` aktif bir planlı tipin varsayılan kilit süresinin altına indirilemez; ekran engelleyen tipleri listeler. Değişiklik açık kayıtların kilit bitişini değiştirmez (PL-18).
- [ ] K15: `<PinUzunluğu>` değişince mevcut PIN'ler doğrulanmaya devam eder, yeni belirlenen PIN yeni uzunlukta istenir; `<OturumSüresi>` ve `<PinDenemeSınırı>` değişikliği sonraki istekte uygulanır; `<AzamiMedyaBoyutu>` yeni yüklemelerde uygulanır (PL-18).

**Kapsam dışı:** Personel ve vardiya kırılımı (K-36), sunucu tarafı PDF (K-28), İÖ önerileri.

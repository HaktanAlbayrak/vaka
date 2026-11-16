# Faz 6D Teslim Raporu: Makine raporu ve malzeme kullanım oranı

| Alan | Değer |
|---|---|
| Faz | 6D |
| Tarih | 2026-11-16 |
| Tur | 4 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `970cacd`; Faz 6D kodu commit'lenmemiş, ön tur düzeltmeleri dahil). Kaynak test başında `.qa/f6d/src` altına kopyalandı ve oradan derlendi. Devir notları `Docs/Handover/backend-engineer-20261116-2330.md` ("Düzeltme" dahil), `Docs/Handover/frontend-engineer-20261116-2300.md` ("Ön tur düzeltmeleri" dahil) |
| Ortam | LOCAL, Release build, port 5797 (Production), DB: `.qa/` kopyası. Yayın `.qa/f6d/build`; `.qa/f6d/qa.db` = `.qa/f6c/qa.db` dosyasının SQLite yedekleme API kopyası (kaynak salt okunur açıldı). `EventChain__HmacKey` geliştirme user-secrets değerinden yalnız süreç ortamına verildi (diske ve çıktıya yazılmadı). Chromium (playwright-cli, oturum `f6d`). Sunucu PID 37476 (bizim) test sonunda kapatıldı; PID 32568 dokunulmadı |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 4: B10 kapandı, açık KRİTİK/YÜKSEK/ORTA yok; tur 3 ve tur 2 KALDI, tur 1 GEÇTİ) |
| Analist kararı | TESLİM ALINDI (tur 4, 2026-11-16). Tur 1-3: İADE (PL-97, PL-98, PL-99); B4 Faz 9'a, B6 süreç notu (PL-100) |

**Test verisinin kurulumu.** Kopyada makine 1, 2 ve 3'ün her birinde açık kayıt vardı (makine kilidi); yeni kayıt açılamıyordu. Tüm ek veri uygulamanın kendi formlarıyla kuruldu (sayfadaki form okunup kendi belirteciyle gönderildi, `.qa/f6d/j/lib.py`, `step*.py`); DB'ye doğrudan yazım yapılmadı.

- Yönetici `1001`, `/interventions/create?recordId=12` ile kayıt 12'yi (makine 2) gerekçeyle iptal etti.
- Operatör `2004` makine 2'de KSA kaydı 13'ü açtı: `L2027-001` eklendi, SKT'si geçmiş `L2026-001` gerekçeyle eklendi, `K2027A` eklenip "çıkar" ile çıkarıldı (`RemovedAtUtc` dolu); iki faz süre altı gerekçeyle bitirildi, kayıt Tamamlandı.
- `2004` kayıt 14'ü açtı (`K2026B`), birinci faz süre altı, ikinci faz 62 sn beklenerek minimumun üstünde bitirildi. `2003` kayıt 15'i malzemesiz açıp iki faz süre altı bitirdi.
- Son kayıtlar sınırı (K6) için `2003` makine 2'de 18 kayıt (16-33, PLN ve KSA dönüşümlü) açtı, her birini `1001` gerekçeyle iptal etti. Makine 2'de toplam 24 kayıt var.
- Makine 1'e dokunulmadı: Faz 6C kopyasındaki veri (KSA v1 ve v2 birlikte, PLN, ACL, zaman aşımı, devam eden kayıt, 21 sa 7 dk 3 sn net süreli kayıt 6) olduğu gibi ölçüldü.

## 1. Kapsam

- BE 1: `IMachineReportReader` / `MachineReportReader`: süzgeç doğrulaması, tip başına satır (kayıt, Tamamlandı, ortalama brüt, mola, net, efor, planlanan minimum, plan farkı, süre altı ve değerlendirilen faz), durum sayıları, son 20 kayıt, malzeme satırları.
- BE 2: `IMaterialUsageReader` / `MaterialUsageReader`, `MaterialTraceMode.Usage`, E-31 kullanım oranı sorgusu.
- BE 3: `IDurationCalculator.CalculateRecordPlannedMinimum` (SH-18), `CalculatePlanVariance` (SH-19).
- BE 4: `MachineDurationRowViewModel.MachineReportUrl` (makine, tarih, tür).
- FE 5: E-34 `Views/Reports/Machine.cshtml` (süzgeç, başlık, durum sayıları, tip tablosu, `_BarChart` grafiği, son kayıtlar, malzeme tablosu, boş durum), menüde "Makine raporu".
- FE 6: E-31 "Kullanım oranı" modu, ortak `_MaterialUsageTable.cshtml`; E-30 makine satırında "Makine raporu" bağlantısı.
- Ön tur düzeltmeleri: son kayıtlar 6 sütun ("Süreler" birleşik), tarih hatasında yalnız hata bildirimi, yalnız `machineId` ile gelen hatalı istekte seçimin korunması.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Yönetici ve Denetçi açar; Operatör ve yalnız Süper admin 403, menüde görmez | GEÇTİ | `Evidence/Faz-6D/YetkiVeSuzgec.txt`: `/reports/machine` (süzgeçli ve süzgeçsiz) Yönetici 1001 ve Denetçi 3001 200; Operatör 2004 ve Süper admin 0001 302 `/account/access-denied` > 403; oturumsuz 302 giriş. Menüde "Makine raporu" yalnız Yönetici ve Denetçide (`_Layout.cshtml:52`, `Manager` ya da `Auditor` koşulu) |
| K2 | Makine seçilmeden boş durum, tablo yok; olmayan ya da uyuşmayan tesis, hat, makine ve ters tarih `ErrorMessage` ve boş tablo | GEÇTİ (sapma: B1) | `YetkiVeSuzgec.txt`: süzgeçsiz açılış "Bir makine seçin", tablo 0. `facilityId=999` "Seçilen tesis bulunamadı.", `lineId=999` "Seçilen hat bulunamadı ya da tesise ait değil.", `machineId=999` "Seçilen makine bulunamadı ya da hatta ait değil.", ters tarih "Bitiş tarihi başlangıç tarihinden önce olamaz.", `kind=9`, `from=abc`, `machineId=abc` "Süzgeç değerlerinden biri okunamadı...", hepsinde tablo 0 ve makine başlığı yok. Uyuşmayan seçim (`facilityId=2&machineId=1`, `facilityId=1&lineId=2&machineId=1`) hata vermiyor, makine boşaltılıp "Bir makine seçin" gösteriliyor (bkz. B1) |
| K3 | Tip satırı kayıt, Tamamlandı, ortalama brüt, mola, net, efor DB sorgusuyla ve aynı süzgeçle `/reports/durations` ile aynı | GEÇTİ | `Evidence/Faz-6D/HesapKarsilastirma.txt`: 13 süzgeç bileşimi (makine 1-5; tür 1 ve 2; `from`, `to`, aralık) x tip satırı, durum sayısı, son kayıt ve malzeme alanları SQLite ile birebir, "toplam sapma 0" (Yönetici ve Denetçi). E-30 makine satırı ile makine raporu tip satırı 5 süzgeçte 15 satır aynı (Tamamlandı sayısı, brüt, mola, net, efor) |
| K4 | Ortalama planlanan minimum ve plan farkı; negatif işaretli; acilde "-"; QA elle hesap | GEÇTİ | `HesapKarsilastirma.txt` "Elle hesap": makine 1 KSA SH-18 120, 120, 360 -> 200 sn = 3 dk 20 sn, net 25419, fark +7 sa 19 sn; PLN 1500 sn = 25 dk, fark -22 dk 14 sn; ACL "-" ve "-"; makine 2 KSA 6 dk, fark -4 dk 50 sn. Sayfa aynı (`MakineRaporu-1440-Acik.png`). İşaret ayrı `span` içinde, süre `_Duration` ile (`Machine.cshtml:246`) |
| K5 | Süre altı ve değerlendirilen faz `RecordPhases` sayımıyla aynı | GEÇTİ | SQL (Tamamlandı kayıt, minimum ve net dolu faz): makine 1 KSA 6/2, PLN 3/3; makine 2 KSA 8/6. Sayfa "2 / 6 faz", "3 / 3 faz", "6 / 8 faz"; ACL "-" (değerlendirilen 0). Tanım `IMachineReportReader.cs:10-11` |
| K6 | Son kayıtlar en çok 20, yeniden eskiye, her satır detaya gider; durum sayıları tüm kayıtlardan | GEÇTİ | Makine 2 (24 kayıt): 20 satır, kayıt 33'ten 14'e, `OpenedAtUtc` ve `Id` azalan sırası DB ile aynı; 20 bağlantı Yönetici ve Denetçide 200. Açılış saati Türkiye saati (UTC+3) ile aynı. Durum sayıları (Tamamlandı 4, İptal edildi 20) süzgece uyan tüm kayıtlardan |
| K7 | SH-18, SH-19 `DurationCalculator`'da, kullanım oranı formülü `MaterialUsageReader`'da XML yorumunda | GEÇTİ | `DurationCalculator.cs:206-218` ve `IDurationCalculator.cs:215-227` (formül satırı ve `<remarks>`), kodla uyumlu (dolu değerlerin toplamı, hiçbiri dolu değilse `null`; `net - planlanan`). `MaterialUsageReader.cs:19-22` oran formülü, bir ondalık, `MidpointRounding.AwayFromZero` ile uyumlu |
| K8 | Kullanım oranı modunda kayıt, oran, lot, SKT geçmiş DB ile aynı; çıkarılmış sayılmaz; payda yazar; Tamamlandı yoksa boş durum | GEÇTİ | `HesapKarsilastirma.txt` 13 süzgeç, sapma 0. Yuvarlama: hat 1 + KSA %42,9 ve %28,6 (3/7, 2/7). Çıkarılan `K2027A` (kayıt 13) MLZ002 lot sayısına ve kayıt sayısına girmiyor (makine 2 MLZ002 lot 1). SKT geçmiş kayıt 13'ten 1 kullanım. Payda ve malzemeli kayıt sayısı panel başlığında ("Oran, 10 Tamamlandı kayıt üzerinden hesaplanır. 6 kayıtta malzeme girilmiş."). Tip 5 ve `from=2026-12-01`: "Tamamlanan kayıt yok" boş durumu, hata yok |
| K9 | Makine raporundaki malzeme tablosu aynı makine ve süzgeçle kullanım oranı moduyla aynı | GEÇTİ | Makine 1 %80,0 / %20,0 ve makine 2 %25,0 / %25,0 iki ekranda aynı; ikisi de aynı SQLite hesabıyla doğrulandı (tarih süzgeçli bileşimler dahil). İki ekran aynı `_MaterialUsageTable.cshtml` ve aynı `IMaterialUsageReader` ile çizilir |
| K10 | Grafik değerleri tabloyla aynı; SVG sunucu HTML'inde, JS kapalıyken çizilir; minimum renk dışında desen ya da metinle ayrılır | GEÇTİ | Grafik metni: "Acil müdahale 1 dk 45 sn planlanan minimum yok", "Kısa planlı temizlik 7 sa 3 dk 39 sn min 3 dk 20 sn", "Planlı temizlik 2 dk 46 sn min 25 dk Minimumun altında" = tablo. JS kapalı bağlamda SVG çubukları 693-720 x 20 px çizili; minimum işareti `line`, minimumun altı desenli dolgu (`url(#type-chart-hatch)`) ve "Minimumun altında" metni, açıklamada üç öğe (`ResponsiveVeG11.txt` "JS kapalı") |
| K11 | E-30 makine satırı bağlantısı aynı makine, tarih ve türle açar | GEÇTİ | `HesapKarsilastirma.txt` son bölüm: süzgeçsiz `/reports/machine?MachineId=1`, `&Kind=1`, `&Kind=2`, `&From=2026-11-15&To=2026-11-15`, `&From=2026-11-16`; açılan raporda seçim ve değerler E-30 satırıyla aynı. Tesis ve hat makineden doluyor |
| K12 | Süzgeç JS kapalıyken çalışır; tesis değişince hat ve makine sunucudan yenilenir | GEÇTİ | JS kapalı Chromium: tesis seç + Raporla -> hat (H1, H2) ve makine (4) seçenekleri geldi; hat H2 -> makine 2 seçenek; makine 3 -> rapor çizildi; tesis 2'ye geçince hat ve makine yeni tesise göre doldu. E-31 kullanım oranında da aynı. Seçenekler Faz 6A düzeninde olduğu gibi "Raporla" ile yenilenir (E-30 da `CascadeSelect` kullanmıyor) |

Ortak kriterler: G1 GEÇTİ, G2 GEÇTİ, G3 GEÇTİ (ORTA B2 açık), G4 GEÇTİ, G5 GEÇTİ (fazda POST yok; `POST /reports/machine` 405), G5a GEÇTİ (sayfalardaki tek `method="post"` form çıkış formu, kendi belirteciyle), G5b GEÇTİ (`type="hidden"` + `value="false"` 0), G6 GEÇTİ, G7 GEÇTİ (migration yok), G8 GEÇTİ, G10 GEÇTİ, G11 GEÇTİ, G12 GEÇTİ. G9 uygulanmaz (plan).

Ön tur bulgularının yeniden doğrulaması:

1. Son kayıtlar 1280 px taşması: kapandı. Tablo 6 sütun; 1280, 1300, 1366 ve 1440 px'te açık ve koyu temada tablo panelin iki iç kenarına bitişik (sol 0, sağ 0), hiçbir hücrede taşma ya da kırpılma yok, sayfa yatay kaydırmasız; "Net 21 sa 7 dk 3 sn" satırı sütunu genişletmiyor. Hücre iç boşluğu iki yanda 12 px: ilk hücrenin içeriği iç kenardan 12 px. Son sütundaki "Ayrıntı" düğmesinin sağ boşluğu 24 / 27 / 37 / 48 px, çünkü İşlem sütunu sola hizalı ve düğmeden geniş; aynı düzen E-30 (35 / 71 px), E-31 makine modu (37 / 64 px) ve `/machines` (148 / 214 px) tablolarında da var, G11 tablo modu için hizalama koşulu koymuyor. Tip tablosu (7 sütun) ve malzeme tablosu aynı genişliklerde temiz (`ResponsiveVeG11.txt`, `MakineRaporu-1280-Koyu.png`).
2. Tarih hatasında "Bir makine seçin": kapandı. Ters tarihte yalnız "Rapor hazırlanamadı" bildirimi var, boş durum ve tablo yok; bildirim odak alıyor (`TarihHatasi-360.png`).
3. Seçimin korunması: kapandı. `machineId=1` ile ters tarih, `kind=9` ve `from=abc`'de tesis T1, hat H1, makine M01 seçili ve seçenek listeleri dolu (tesis 2, hat 2, makine 2); ters tarihte tarih değerleri de korunuyor. `machineId=999`'da tesis listesi dolu (2), hat ve makine listesi boş.

Plandan sapmalar (karar analistin):

- Backend dosya sınırı dışı: `Services/IReportOptions.cs` ve `ReportOptions.cs`'e `ReconcileSelectionAsync` eklendi (`ReportOptions.cs:57-73`). Davranışı E-34 ve yalnız E-31 kullanım oranı modunda etkili; E-30 aynı girdide hâlâ hata veriyor. K2 ile ilişkisi B1'de.
- Sözleşmede olmayan `Kind`: `MachineReportFilterViewModel.Kind` sözleşmede var (`Filter { ..., Kind? }`); sözleşme dışı olan `MaterialUsageFilter.Kind`. E-31 kullanım oranı formunda tür alanı yok, `Kind` yalnız makine raporunun malzeme satırları için kullanılıyor ve K9 eşitliğini sağlıyor (tür süzgeçli makine raporu ile E-31 karşılaştırılamaz, çünkü E-31'de tür süzgeci yok).
- "Değerlendirilen faz" tanımı (minimum anlık değeri ve net süresi dolu, Tamamlandı kaydın fazı) `IMachineReportReader.cs:10-11` XML yorumunda; kod (`MachineReportReader.cs:240-247`) ve K5 sayımı bu tanımla aynı.
- Frontend: menü `Views/Shared/_NavList.cshtml` yerine `_Layout.cshtml`'e eklendi (menü listesi orada kuruluyor, `_Layout.cshtml:49-53`); dosya sınırında olmayan yeni ortak partial `Views/Shared/_MaterialUsageTable.cshtml` (iki ekran ortak kullanıyor, K9'u kolaylaştırıyor).

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`.qa/f6d/src` kopyasında); `dotnet publish -c Release -o .qa/f6d/build` başarılı |
| CSS | `npm run build:css` | Kopyada aynı Tailwind CLI (3.4.17) ile derlendi, çıktı repodaki `wwwroot/css/app.css` ile bayt bayt aynı (73780 bayt) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme; sihirli rol metni 0; `IsInRole("` 0; `[Authorize]`/`[AllowAnonymous]` taşımayan controller 0; MediatR, AutoMapper, generic repository 0; `Program.cs`'te `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: `MachineReportReader`, `IMachineReportReader`, `MaterialUsageReader`, `IMaterialUsageReader`, `MachineReportFilterViewModel`, `MachineTypeRowViewModel`, `MachineRecentRecordViewModel`, `MachineReportHeaderViewModel`, `MaterialUsageRowViewModel`, `MaterialUsageViewModel`, `MaterialUsageFilter`, `MachineReportLinks`, `MaterialTraceMode.Usage`, `Machine.cshtml`, `_MaterialUsageTable.cshtml`. Özellik adları sözleşmeyle aynı; özel alanlar `_db`, `_usage`, `_clock` |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (değişen `Machine.cshtml`, `Materials.cshtml`, `Durations.cshtml`, `_MaterialUsageTable.cshtml`, `_Layout.cshtml`; `Styles/app.css` değişmedi). Elle yazılmış `type="hidden"` alanlar yalnız `mode` ve `LotQuery` (otomatik tamamlama gerekmez) |
| Mekanik eval | `UiDelivery.md §3` | 154 hücreden 150'si beklenen değerlerde; `h1` 1, `skip` true, `imgNoAlt`/`imgNoSize`/`inputNoLabel`/`iconBtnNoName` 0, `emdash` false, font Hanken Grotesk (`fonts.check` true), `hScroll` false, `smallTargets` 0, `extScripts` [], üst bar 64 px, konsol 0, token dışı köşe 0. Kalan 4: 320 px'te SKT çipi 2 px (B2), E-30 1280 px süzgeç düğmeleri (B7). Kart modunda görsel olarak gizli başlık satırı (sr-only) taşma sayımından çıkarıldı (Faz 6C ile aynı yöntem) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Yeni migration yok; `Data/Migrations` çalışma ağacında değişmemiş, son `20261115152212_AddCleaningRecordListIndexes` |

Kod okuması (SOLID, §7, §8):

- Controller: `ReportsController.Machine` yalnız model bağlama, `ModelState` sonucu ve servis çağrısı (`ReportsController.cs:33-39`). Bağımlılıklar arayüzle constructor'dan; `new` ile servis ve servis bulucu yok. Arayüzler tek üyeli.
- Süre aritmetiği yalnız `IDurationCalculator`'da: ortalamalar `CalculateAverage`, kayıt planlanan minimumu SH-18, fark SH-19 (`MachineReportReader.cs:230-238`); görünümde süre aritmetiği yok (G8 grep 0; `Math.Abs` yalnız işaret gösterimi).
- §7: SH-18 ve SH-19 formülleri metodun üstünde ve kodla uyumlu; `IMachineReportReader` özeti ortalamaların SH-15 ile alındığını yazıyor, kodla uyumlu.
- §8: emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager` adı, kodu tekrar eden satır yorumu 0 (yeni ve değişen 22 dosya); yorumlar yalnız XML özet. Görünür metinde, devir notlarında em/en-dash 0. Bkz. B4 (dosya uzunluğu), B5 (boş dal).

## 4. Yetki matrisi

Rollere karşılık: Viewer = Denetçi (3001), Editor = Operatör (2004), Admin = Yönetici (1001); Süper admin (0001) ayrı yazıldı. 403 = 302 `/account/access-denied` > 403 (projenin düzeni).

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/reports/machine` | 302 | 200 | 403 | 200; Süper admin 403 |
| GET `/reports/machine?machineId=1` | 302 | 200 | 403 | 200; Süper admin 403 |
| GET `/reports/materials?mode=Usage` | 302 | 200 | 403 | 200; Süper admin 403 |
| GET `/reports/materials?mode=Usage&machineId=2` | 302 | 200 | 403 | 200; Süper admin 403 |
| GET `/reports/durations` (bağlantı eklendi) | 302 | 200 | 403 | 200; Süper admin 403 |
| GET `/records/details/{id}` (son kayıt bağlantıları, 20 adet) | - | 200 | - | 200 |
| POST `/reports/machine` | 405 | - | - | 405 (POST uç noktası yok) |

Menü: "Makine raporu" Yönetici ve Denetçide Raporlar altında, Operatör ve Süper admin menüsünde yok (Raporlar grubu da yok).

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/reports/machine` (boş durum) | Temiz | Temiz | Temiz | Temiz; 1280 temiz | Temiz | Temiz (tüm genişlikler) | Temiz | 320 ve 640 temiz | İlk Tab "İçeriğe geç" |
| `/reports/machine?machineId=1` (dolu, 21 sa kaydı, grafik) | Temiz | Temiz | Temiz | Temiz; 1280, 1300, 1366 temiz | Temiz | Temiz | Temiz; hareketli öğe 0 | 320 ve 640 temiz | Süzgeç, Raporla, Temizle, 7 "Ayrıntı"; odak halkası her öğede, üst bar örtmüyor |
| `/reports/machine?machineId=2` (20 son kayıt, SKT çipi) | Temiz | Temiz | Temiz | Temiz; 1280 temiz | Temiz | Temiz | Temiz | 640 temiz; 320'de SKT çipi 2 px (B2) | Temiz (360) |
| `/reports/machine?machineId=3` (malzemesiz) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - |
| `/reports/machine?machineId=4` (kullanımdan kaldırılmış, kayıtsız) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - |
| Tarih hatası | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Bildirim odak alıyor |
| Tamamlandı kaydı olmayan süzgeç | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - |
| `/reports/materials?mode=Usage` (dolu) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | 640 temiz; 320'de SKT çipi 2 px (B2) | Mod düğmeleri, süzgeç, Sorgula (768) |
| `/reports/materials?mode=Usage` (boş, malzemesiz) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - |
| `/reports/durations` (Makine raporu bağlantısı) | Temiz | Temiz | Temiz | Temiz; 1280'de süzgeç düğmeleri (B7) | Temiz | Temiz | Temiz | Temiz | - |

Konsol: 154 hücrede 0 hata ve uyarı; sunucu günlüğünde `fail`/`crit`/exception 0. Grafik metin kontrastı her hücrede 4,5:1 üstü (eval `lowC` boş). Dört durum: yükleniyor (gönderimde `_ListSkeleton` görünür, "Yükleniyor…", `role=status`, içerik gizli, form `aria-busy`, düğme pasif; E-34 ve E-31), boş (makine seçilmedi, süzgeçle kayıt yok, Tamamlandı kayıt yok, malzeme kullanılmamış, kullanım oranında Tamamlandı kayıt yok), hata ve dolu ölçüldü. Ek ekranlar: `/dashboard` 200, `/account/access-denied` 403, olmayan adres 404, eval temiz (konsoldaki tek ileti beklenen 403/404 kaynak durumu).

G11 imzaları (`ResponsiveVeG11.txt`; kart kusuru 0, 1280 ve üstü tablo, satır en az 56 px, kapsayıcı 10 px, başlık zemini köşeden taşmıyor):

- Tip tablosu: 360 ve 640 `Baş[Tip|-] Gövde[Brüt ve mola:1@1 Net:1@1 Efor:1@2 Planlanan min.:1@2 Plan farkı:1@3 Süre altı faz:1@3] Ayak[yok]`; 768 ve 1024 `Baş[Tip|-] Gövde[Brüt ve mola:1@1 Net:1@1 Efor:1@1 Planlanan min.:1@2 Plan farkı:1@2 Süre altı faz:1@2] Ayak[yok]`.
- Son kayıtlar: 360-1024 `Baş[Kayıt no|Durum] Gövde[Tip:tam@1 Açılış:1@2 Süreler:1@2] Ayak[İşlem]` (320'de rozet alt satıra iniyor, kurala uygun).
- Malzeme ve kullanım oranı tablosu: 360 `Baş[Malzeme|-] Gövde[Kayıt:1@1 Kullanım oranı:1@1 Lot:1@2 SKT geçmiş:1@2] Ayak[yok]`; 768 ve 1024 `Gövde[Kayıt:1@1 Kullanım oranı:1@1 Lot:1@1 SKT geçmiş:1@2]`.
- E-30 makine satırı: 360 `Baş[Makine ve tip|-] Gövde[Kayıt:1@1 Brüt:1@1 Mola:1@2 Net:1@2 Efor:1@3] Ayak[İşlem]`; 768 ve 1024 3 sütun.
- Ölçülen imzalar devir notundaki alan listesiyle ve G11 kuralıyla uyumlu; devir notunda genişlik başına `Alan:sütun@satır` biçimi yok (B6).

G12: E-34, E-31 kullanım oranı ve E-30 için 360, 768, 1024 ve 1440 px'te açık ve koyu temada kusur 0 (süzgeç satırında alan üst kenarı ve yüksekliği aynı, etiket ve alan sol kenarı aynı, select oku dikey ortalı, panel başlığı birinci düzey öğeleri iç boşluk çizgisinde, kart başlığı ve gövde sol kenarı aynı, ayak düğmesi sağ iç kenarda).

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Başlık Archivo, gövde Hanken Grotesk, süreler, kayıt no, oran ve sayılar JetBrains Mono `tabular-nums`.
- Palet: UYGUN. Yalnız tokenler; ham hex ve `slate/gray/zinc` sınıfı 0. Grafikte ortalama net `chart-1`, minimumun altı `chart-1` desenli, minimum işareti mürekkep rengi çizgi (Faz 6B eki 3 ile aynı).
- Köşe ölçeği: UYGUN. Panel ve tablo kapsayıcısı 10 px, rozet, çip ve düğme 6 px; token dışı köşe 0 (her hücre).
- Gezinme arketipi: UYGUN. Raporlar grubunda "Makine raporu", `factory` ikonu, etkin bağlantı işaretli.
- Sayfa iskeleti: UYGUN. Süzgeç paneli, makine başlığı ve durum sayıları, tip tablosu ve grafik, son kayıtlar, malzeme; dört eşit KPI kartı yok (durum sayıları tek panelde `kv` listesi).
- Veri dili: UYGUN. 2 px başlık çizgisi, sayı sütunları sağa hizalı mono, 1279 px ve altında kart; rozet ikon ve metinli; boş durum kesik çizgili kutu ve sonraki adım; hata kutusu sorun ve sonraki adım; iskelet durağan.
- Hareket imzası: UYGUN. Yeni hareket yok; reduced-motion'da hareketli öğe 0.
- İmza bileşen: UYGUN (değişmedi).
- Anti-slop (`DesignDna.md §4`): temiz. Gradient, gölge yığını, emoji, elle çizilmiş SVG yok (ikonlar Lucide sprite'ı, grafik veriden); em/en-dash 0; "…" doğru karakter; iç içe kart yok.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | ORTA | K2 "süzgeçle uyuşmayan tesis, hat, makine `ErrorMessage` ve boş tablolarla döner" diyor; uygulama uyuşmayan hat ve makineyi hata vermeden boşaltıyor (`ReconcileSelectionAsync`, backend sapması, koordinatör isteği) ve "Bir makine seçin" boş durumunu gösteriyor. Aynı girdide E-30 hâlâ hata veriyor ("Seçilen makine bulunamadı ya da hatta ait değil."); plan E-34 doğrulamasını "E-30 düzeninde" istiyor. Olmayan kimlik ve ters tarih K2'ye uygun | `YetkiVeSuzgec.txt`: `facilityId=2&machineId=1`, `facilityId=1&lineId=2&machineId=1`, `facilityId=2&lineId=1` hata yok; E-30 aynı adreslerde hata. `ReportOptions.cs:57-73`, `MachineReportReader.cs:32`, `MaterialTraceReader.cs:33-36` | analyst | Karar: K2 metni ve Decisions kaydı yeni davranışa göre güncellenir (E-30'un da aynı davranışa alınıp alınmayacağı dahil) ya da backend'e iade edilip uyuşmayan seçim `ErrorMessage` ile döner | KAPANDI (tur 2) |
| B2 | ORTA | %200 yakınlaştırmada (320 CSS px) malzeme kullanım tablosu kartında "SKT geçmiş" hücresindeki çip ("1 kullanım") hücresinden ve kartın iç kenarından 2 px taşıyor; E-34 makine 2 ve E-31 kullanım oranı modunda. 360 px ve üstünde temiz | `Evidence/Faz-6D/KullanimOrani-320-SktTasma.png`; eval `hücre taşma SKT geçmiş 2`, `taşma span.chip 2.0` (`ResponsiveVeG11.txt`); `_MaterialUsageTable.cshtml:26-34` (`whitespace-nowrap` hücre + çip) | frontend-engineer | Kart modunda hücrenin `whitespace-nowrap`'ı kaldırılsın ya da çip metni kırılabilir olsun; 320 ve 360 px'te açık ve koyu yeniden ölçülür | KAPANDI (tur 2) |
| B3 | DÜŞÜK | E-31 kullanım oranı modunda okunamayan değer (`mode=Usage&from=abc&machineId=1`) sayfayı "Lota göre" moduna düşürüyor; mod ve seçim kayboluyor, hata metni lot modunda görünüyor. E-34'te ön turda düzeltilen davranışın (seçimin korunması) E-31 karşılığı; makine modunda da Faz 6A'dan beri aynı | `YetkiVeSuzgec.txt` sonu ve kontrol: etkin mod "Lota göre", başlık "Bir lot seçin". `ReportsController.cs:45` `invalid ? new MaterialTraceViewModel() : request` | backend-engineer | `Machine` action'ı gibi bağlanabilen alanlar (en az `Mode`) korunsun, okunamayanlar null olsun | KAPANDI (tur 2) |
| B4 | DÜŞÜK | `MachineReportReader.cs` 254 satır; `CodingStandards.md §2` 200 satırı geçen servisi bölünmeye aday sayar (süzgeç ve seçim, tip satırı hesabı, son kayıt okuma aynı sınıfta) | `wc -l Services/MachineReportReader.cs` | backend-engineer | Tip satırı kurulumu (`BuildTypeRow`, planlanan minimum sözlüğü) ayrı bir sınıfa alınabilir; zorunlu değil | DEVREDİLDİ (Faz 9 kalem 3, analist kararı) |
| B5 | DÜŞÜK | Görünümde boş dal: `@if (machine is null && !string.IsNullOrEmpty(Model.ErrorMessage)) { }` yalnız boş durumu bastırmak için duruyor; `CodingStandards.md §8` kullanılmayan kod | `Machine.cshtml:127-129` | frontend-engineer | Koşul `else if (machine is null && string.IsNullOrEmpty(Model.ErrorMessage))` biçimine indirgensin | KAPANDI (tur 2) |
| B6 | DÜŞÜK | Frontend devir notunda G11 beklenen imzaları genişlik başına `Alan:sütun@satır` biçiminde yazılmamış (yalnız alan listesi); PL-33 bunu ister. Ölçülen imzalar kurala uygun | `Docs/Handover/frontend-engineer-20261116-2300.md` "G11 imzaları"; `PhasePlan.md` "UI fazlarında yerleşim (PL-33)" | frontend-engineer | Sonraki fazdan itibaren beklenen imza genişlik başına yazılır | DEVREDİLDİ (süreç, Faz 6E, analist kararı) |
| B7 | DÜŞÜK | E-30 süzgecinde 1280 px'te "Raporla" ve "Temizle" dördüncü sütunda alt alta, "Raporla" üst kenarı aynı satırdaki select'lerle aynı değil (332 / 388 px). Faz 6D'de süzgeç değişmedi; G12'nin ölçüm genişlikleri (360, 768, 1024, 1440) dışında | `Evidence/Faz-6D/SureRaporuSuzgec-1280.png`; eval `form düğme üst 332/388` (1280 açık ve koyu) | frontend-engineer | Düğme satırı tam satıra alınsın (`xl:col-span-*`) ya da 1280'de alanlarla hizalansın | KAPANDI (tur 2) |
| B8 | YÜKSEK | Tur 2 (A1 yeniden denetimi): `/records` 1280 ve 1300 px'te düzeltmeden sonra da tablo panelden geniş (1032 / 964 ve 1032 / 984 px); İşlem sütunundaki "Ayrıntı" düğmesi panelin `overflow: clip` sınırında 56 ve 36 px kırpılıyor, açık ve koyu temada, listedeki 25 satırın hepsinde (yalnız uzun adlı satırda değil). Düğme metni "A" olarak görünüyor; merkezden tıklanıyor, sağ kenarı tıklanmıyor. 1366 ve 1440 px temiz. Analistin şartı "tam görünür ve tıklanır" karşılanmadı. Tur 1'de tek uzun adlı satır kanıtıyla ORTA sayılmıştı; tüm satırlarda süren kırık yerleşim olduğu için YÜKSEK yazıldı (README şiddet tablosu) | `Evidence/Faz-6D/Kayitlar-1280-Kirpilma.png` (tur 2 görüntüsü), `Evidence/Faz-6D/Tur2Olcumler.txt` "A1". 1280 px sütunlar: Kayıt no 169, Makine ve yer 197, Sahip 188, Durum 154, Açılış 108, Net süre 81, İşlem 136 = 1033 px; `Records/Index.cshtml` `xl:max-w-xs` ve rozetlerin alt alta dizilmesi toplamı panel içine indirmiyor | frontend-engineer | Sütunların en küçük genişlik toplamı 1280 px'te panel içine (964 px) inmeli: ör. kayıt no sütununun kırılabilmesi, Sahip ve Makine sütunlarının daha dar sınırı, ya da 1280-1400 px'te İşlem düğmesinin yalnız ikonlu (aria-label korunarak) çizilmesi. Yeniden ölçüm 1280, 1300, 1366, 1440 açık ve koyu, tüm satırlarda düğme sağ kenarı panel iç kenarında ya da içinde | KAPANDI (tur 3) |
| B9 | DÜŞÜK | Tur 2: E-31 Makine modunda tesis ve hat alanı yok, ama adresteki `facilityId` ve `lineId` uzlaştırmaya giriyor: `mode=Machine&facilityId=2&machineId=1` makineyi hatasız boşaltıyor, buna karşılık `lineId=999` ve `facilityId=999` doğrulanmıyor, hata vermiyor. Analistin tur 2 şartı E-31 Makine modunda da `lineId=999` hatası istiyordu; arayüzden üretilemeyen adresler | `Tur2Olcumler.txt` "E-31 Machine" satırları; `MaterialTraceReader.cs:35` (uzlaştırma Usage ve Machine), `:77` (Machine modunda yalnız makine doğrulanıyor); backend devir notu "Not" | analyst | Karar: Makine modunda alanı olmayan `facilityId`/`lineId` ya hiç kullanılmaz (uzlaştırmaya girmez) ya da doğrulanır; K2'nin E-31 Makine modu kapsamı netleşir | KAPANDI (analist kararı: davranış kabul, K2 notu, PL-98) |
| B10 | YÜKSEK | Tur 3 gerilemesi: `/records` kart modunda yeni birleşik "Açılış ve net süre" hücresinde "Açılış 16.11.2026 21:55" satırı `whitespace-nowrap` olduğu için yarım genişlikteki gövde sütununa sığmıyor. 360 px'te zaman öğesi kartın iç kenarını 23,6 px aşıyor ve "21:5" olarak kesik görünüyor; 320 px'te (%200 yakınlaştırma) 43,6 px aşıyor ve sayfa yatay kayıyor (334 / 320 px). Açık ve koyu temada, 25 satırın hepsinde, arama ve sayfa 2'de de. 768 ve 1024 px temiz. Tur 2'de aynı genişliklerde temizdi (ayrı "Açılış" hücresi). `/records` operatörün de kullandığı liste (G4: 360 px'te yatay kaydırma yok) | `Evidence/Faz-6D/Kayitlar-360-Tur3.png`; `Evidence/Faz-6D/Tur3Olcumler.txt` "Kart modu" ve matris (`taşma time.mono 23.6`, 320'de `hScroll 334`); `Records/Index.cshtml` birleşik hücre `<span class="block whitespace-nowrap">` | frontend-engineer | Kart modunda satırların kırılabilmesi (`xl:whitespace-nowrap`), ya da kart modunda iç etiketin değerin üstüne alınması; yeniden ölçüm 320, 360, 768, 1024 açık ve koyu ve 1280-1440 B8 ölçümünün tekrarı | KAPANDI (tur 4) |

## 7. Geçenler

- K1-K12 kanıtlarıyla geçti; makine raporu, kullanım oranı ve süre raporu 13 + 13 + 5 süzgeç bileşiminde SQLite ile sapma 0 (Yönetici ve Denetçi).
- SH-18 ve SH-19 elle hesapla aynı; KSA v1 ve v2 karışık kayıtlarda anlık minimum değerleri doğru toplanıyor; acil tipte "-".
- Çıkarılmış malzeme, SKT geçmiş kullanım, lot sayısı ve bir ondalık yuvarlama (%42,9, %28,6) doğru; payda sayfada yazıyor.
- Son kayıtlar 24 kayıtlı makinede 20 satır, sıra ve bağlantılar doğru.
- Üç ön tur bulgusu tarayıcıda kapandı (1280-1440 px açık ve koyu taşma yok; tarih hatasında yalnız bildirim; hatalı istekte seçim korunuyor).
- JS kapalıyken süzgeç ve grafik çalışıyor; yükleniyor, boş, hata, dolu durumları ölçüldü.
- Rol matrisi ve menü G10 ile uyumlu; yetki yalnız rol (`Roles.ManagerOrAuditor`).
- 154 hücrelik matris, klavye turu, reduced-motion, konsol temiz (B2 ve B7 dışında).

## 8. Kapsam dışı / yapılamayan

- Ekran okuyucu ile sesli tur yapılmadı; erişilebilirlik ağacı yerine öznitelik ve odak ölçümüyle sınırlı kaldı.
- Büyük veride sorgu süresi ölçülmedi (kopyada 33 kayıt); backend `CleaningRecords(MachineId, OpenedAtUtc)` index ihtiyacını büyük veride ölçülecek diye yazdı.
- Yalnız `lineId` ile (tesis olmadan) elle yazılmış adreslerde hat listesi boş kalırken makine listesinin hatta göre süzüldüğü görüldü (`lineId=3&machineId=1`); arayüzden üretilemediği için bulgu yazılmadı.
- Mobil yatay yönelim ayrıca ölçülmedi; 640 ve 768 px genişlik hücreleri kapsadı.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | `/records` listesi 1280 px'te (1280-1400 arası) tablo panelden geniş (1032 / 968 px), İşlem sütunundaki "Ayrıntı" düğmesi panelin `overflow: clip` sınırında kırpılıyor; 1440'ta temiz. Uzun sahip adı ("Şükrüye İnceoğlu Karamustafapaşalıoğulları", Faz 6C verisi) ile görülüyor. Ekran Faz 6D'de değişmedi | frontend-engineer | Faz 6D kapsamı dışında, test sırasında görüldü; kanıt `Evidence/Faz-6D/Kayitlar-1280-Kirpilma.png`. Şiddet ve hangi faza alınacağı analistin kararı. Tur 2: analist iade maddesine aldı (PL-97); düzeltme sonrası ölçüm B8 |

## 10. Analist kararı

```
FAZ 6D: İADE
Tarih: 2026-11-16
Gerekçe: K1-K12 hesap ve kanıtla karşılandı. Kendim doğruladım: B1 (ReportOptions.cs:57-73 Reconcile yalnız MachineReportReader.cs:32
  ve MaterialTraceReader.cs:35'te Usage modunda; DurationReportReader.cs:35 aynı girdide FindInvalidSelectionAsync ile hata),
  B2 (_MaterialUsageTable.cshtml:26 hücre whitespace-nowrap + çip), B3 (ReportsController.cs:45), B5 (Machine.cshtml:127-129),
  B4 (MachineReportReader.cs 254 satır). ORTA B2 açık; UI fazı sıfır ihlalle teslim edilir (CLAUDE.md kural 8). Sunum ekranlarındaki
  küçük kalemler aynı turda toplandı, tek tur hedefi.
Sapmalar: IReportOptions.ReconcileSelectionAsync, MaterialUsageFilter.Kind, menünün _Layout.cshtml'de olması, ortak
  _MaterialUsageTable.cshtml kabul edildi (gerekçeleri raporda; sözleşmeyi bozmuyor).
İade maddeleri (tur 2):
  B1 analist kararı + backend-engineer: uyuşmayan alt seçimi boşaltma davranışı kabul; K2 güncellendi (PL-97): olmayan kimlik
    ve ters tarih ErrorMessage verir, var olan ama üst seçimle uyuşmayan hat ya da makine hatasız boşaltılır. Tutarlılık için aynı
    düzen E-30'a (DurationReportReader, FindInvalidSelectionAsync'ten önce ReconcileSelectionAsync) ve E-31 Makine moduna
    (MaterialTraceReader koşulu Usage ve Machine) alınır. Neden: JS kapalı süzgeçte seçenekler "Raporla" ile yenilenir; tesis
    değişince eski hat ya da makine kalırsa kullanıcı hata alır, E-30 sunumda bu yüzden hata verirdi.
  B2 ORTA, frontend-engineer: 320 px kart modunda SKT çipi taşmaz.
  B3 DÜŞÜK, backend-engineer: okunamayan süzgeçte Mode ve okunabilen seçim korunur, okunamayan alan boş kalır (E-34 düzeni).
  B5 DÜŞÜK, frontend-engineer: boş dal kaldırılır.
  B7 DÜŞÜK, frontend-engineer: E-30 süzgeç düğmeleri 1280 px'te alanlarla hizalı ya da tam satırda (sunum ekranı).
  A1 ORTA, frontend-engineer: /records 1280-1400 px'te tablo panele sığar, "Ayrıntı" kırpılmaz (uzun sahip adıyla). Kapsam dışı ama
    sunumda görünen liste ve tıklanabilir öğe kırpılıyor; ayrı D-2 commit'i ek QA turu isterdi, bu turda ölçülür (PL-97).
Devredilenler:
  B4 DÜŞÜK, backend-engineer: Faz 9 kalem 3 (raporlar ve CSV MachineReportReader'a dokunur); bölme zorunlu değil, şimdi gerileme riski.
  B6 DÜŞÜK, frontend-engineer: kod değil süreç; Faz 6E'den itibaren devir notunda G11 imzası genişlik başına Alan:sütun@satır.
Yeniden denetim şartı: B1 E-34, E-30 ve E-31 (Usage ve Machine) için facilityId=2&machineId=1, facilityId=1&lineId=2&machineId=1,
  facilityId=2&lineId=1 hatasız boşaltma; machineId=999, lineId=999, ters tarih hata (K2). B2 320 ve 360 px açık ve koyu. B3
  mode=Usage&from=abc&machineId=1 modu ve makineyi korur, hata metni görünür. B7 1280 ve 1440 px. A1 /records 1280, 1300, 1366, 1440
  px açık ve koyu, uzun adlı kayıtta "Ayrıntı" tam görünür ve tıklanır. Gerileme: K3, K8, K9, K11, K12, E-30 Faz 6A/6B kabul
  kriterlerinden süzgeç ve grafik, değişen dosyalarda mekanik eval ve grep, konsol 0.
```

```
FAZ 6D: İADE (tur 2)
Tarih: 2026-11-16
Gerekçe: K 12/12; B1, B2, B3, B5, B7 kapandı, gerileme temiz. Açık YÜKSEK B8: /records 1280 ve 1300 px'te "Ayrıntı" 25 satırın
  hepsinde kırpılıyor (sütunlar 169 + 197 + 188 + 154 + 108 + 81 + 136 = 1033 px, panel içi 964 px). Kendim okudum: Records/Index.cshtml
  7 sütun, Sahip hücresi "break-words" (overflow-wrap: break-word en küçük genişliği düşürmez, tek uzun sözcük sütunu genişletir),
  xl'de çip, rozet ve düğme nowrap (app.css:655-658).
Tur 3 (son tur; Workflow §3 iki tur sınırı nedeniyle kullanıcı onayı gerekir, koordinatör alır):
  B8 YÜKSEK, frontend-engineer. Çözüm yönü, ikisi birlikte:
    1. "Açılış" ve "Net süre" tek sütunda birleşir: başlık "Açılış ve net süre", iki satır (üstte açılış anı, altta net süre),
       sağa hizalı mono; kart modunda iki ayrı etiketli değer kalır. Emsal Faz 6D son kayıtlardaki birleşik "Süreler" sütunu.
    2. "Sahip" ve "Makine ve yer" hücreleri tablo modunda uzun veri hücresi kuralıyla kırılır (overflow-wrap: anywhere, PL-45
       ve .cell-long emsali); tek uzun ad sütunun en küçük genişliğini belirlemez.
    Yapılmayacaklar: kart eşiğini yalnız bu liste için değiştirmek (DNA ve G11 1280 px eşiği, tüm listeler), Sahip sütununu
    kaldırmak ya da gizlemek (listede sorumlu görünür kalır, K-35), düğmeyi küçültmek ya da taşmayı clip dışına almak.
    Hedef: 1280 px'te QA verisiyle (en uzun ad, iki çipli satır, en uzun kayıt no) sütun en küçük genişlikleri toplamı panel iç
    genişliğinin en az 24 px altında; frontend bunu teslimden önce kendisi ölçer ve devir notuna yazar.
  B6 notu: frontend tur 3 devir notunda beklenen G11 imzasını 768 ve 1024 px için 3 sütunla düzeltir (yerleşim değişmez).
Kararlar:
  B9 DÜŞÜK: E-31 Makine modunda formda tesis ve hat alanı yoktur; adresteki facilityId ve lineId yalnız uzlaştırmada kullanılır,
    doğrulanmaz ve hata vermez. Arayüzden üretilemez, davranış kabul; K2 notu eklendi (PL-98). Kod değişmez.
  E-30 okunamayan değerde süzgeç sıfırlanması (Faz 6A'dan): Faz 9 kalem 5'e devredildi (E-30 orada değişiyor), backend-engineer.
Yeniden denetim şartı (tur 3): /records 1280, 1300, 1366, 1440 px açık ve koyu, 25 satırda "Ayrıntı" tam görünür ve sağ kenarı
  dahil tıklanır, tablo panelden geniş değil; 360, 768, 1024 px kart modu ve G11 imzası (birleşik sütun iki etiketli); G12; konsol 0;
  kayıt listesi araması ve sayfalama gerilemesi. Yalnız değişen dosyalarda mekanik eval ve grep.
Kullanıcı tur 3'ü onaylamazsa: Records/Index.cshtml değişikliği 6D'den çıkarılır (HEAD haline döner), 6D bu raporla yeniden
  karara gelir ve B8 faz dışı düzeltme D-2 olarak açılır (PL-74 düzeni).
```

```
FAZ 6D: İADE (tur 3)
Tarih: 2026-11-16
Gerekçe: B8 ve B9 kapandı, gerileme temiz. Tur 3'ün getirdiği YÜKSEK B10 açık: /records kart modunda "Açılış ve net süre" satırları
  whitespace-nowrap (Records/Index.cshtml:194-195, kendim okudum); 360 px'te kart 23,6 px aşılıyor ve saat kesiliyor, 320 px'te sayfa
  yatay kayıyor. Tur 3 son turdu (O-12); devam kullanıcı kararıdır.
Kullanıcıya seçenekler:
  (a) Önerilen. Yalnız B10 için tek seferlik tur 4. Frontend, frontend-engineer: nowrap yalnız xl'de (xl:whitespace-nowrap), kart
      modunda satır kırılır; 320, 360, 768, 1280 ve 1440 px açık ve koyu kendisi ölçer, devir notuna yazar. QA yalnız /records'u
      ölçer: bu genişliklerde kart ve tablo modu, "Ayrıntı" tam görünür, yatay kaydırma yok, konsol 0. Başka değişiklik kabul edilmez.
  (b) Records/Index.cshtml ve app.css'teki A1/B8 değişikliği 6D'den çıkarılır, dosyalar HEAD haline döner; 6D B8 olmadan karara
      gelir (6D kapsamı K 12/12 ve kendi bulguları kapalı olduğu için TESLİM ALINDI beklenir); /records kırpılması D-2 olarak açılır.
Öneri (a). Gerekçe: düzeltme tek sınıf değişikliği ve dar ölçüm; (b) sunumun olası ekran genişliğinde (1280-1300 px) bilinen
  kırpılmayı geri getirir ve D-2 yine bir QA ölçümü ister, yani maliyet aynı, sonuç daha kötü. (a)'da da B10 kapanmazsa (b) uygulanır.
```

```
FAZ 6D: TESLİM ALINDI (tur 4)
Tarih: 2026-11-16
Gerekçe: K 12/12; B1, B2, B3, B5, B7, B8, B10 kapandı, B9 kararla kapandı; açık KRİTİK, YÜKSEK, ORTA yok. Tur 4 değişikliği yalnız
  Records/Index.cshtml:194-195 xl:whitespace-nowrap (kendim okudum); /records 320-1440 px açık ve koyu taşma 0, "Ayrıntı" 33 satırda
  tam görünür ve tıklanır, G11 imzası tur 3 ile aynı, konsol 0. Kullanıcı onayı O-13 kapsamında kaldı.
Açık kalemler (devir):
  B4 DÜŞÜK, backend-engineer: Faz 9 kalem 3 (PL-97).
  B6 DÜŞÜK, frontend-engineer: süreç; Faz 6E'den itibaren devir notunda G11 imzası genişlik başına (PL-97).
  E-30 okunamayan değerde süzgecin sıfırlanması, backend-engineer: Faz 9 kalem 3 (PL-98).
```

## Tur 2 (2026-11-16)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `970cacd`; tur 1 iade düzeltmeleri dahil). Kaynak `.qa/f6d/src2` altına kopyalanıp oradan derlendi. Devir notları: backend "Tur 1 İADE düzeltmesi (B1, B3, PL-97)", frontend "Tur 1 iade düzeltmeleri" |
| Ortam | LOCAL, Release build, port 5798 (Production), DB `.qa/f6d/qa.db` (tur 1 verisiyle, yeni veri kurulmadı). Yayın `.qa/f6d/build2`. Sunucu PID 6088 (bizim) sonda kapatıldı; PID 32568 dokunulmadı |
| Kapsam | Analistin yeniden denetim şartı: B1, B2, B3, B5, B7, A1; gerileme K3, K8, K9, K11, K12, E-30 süzgeç ve grafiği, değişen dosyalarda mekanik eval ve grep, konsol |
| QA önerisi | KALDI (B8 YÜKSEK açık) |

Derleme ve mekanik: `dotnet build -c Release -warnaserror` 0 uyarı 0 hata; `npm run build:css` çıktısı repodaki `app.css` ile bayt bayt aynı (73907 bayt). Policy, sihirli rol, `IsInRole("`, `Migrate()` grep 0; yeni migration yok. Değişen görünümlerde (`Machine`, `Materials`, `Durations`, `_MaterialUsageTable`, `_Layout`, `Records/Index`) `UiDelivery.md §4` grep 0 ihlal; em/en-dash 0 (görünümler, servisler, devir notları, bu rapor). §8 taraması değişen servis ve controller'da 0. `MachineReportReader.cs` 234 satır (B4 devredildi). Sunucu günlüğünde `fail`/`crit`/exception 0.

| Madde | Yeniden denetim şartı | Sonuç | Kanıt |
|---|---|---|---|
| B1 | E-34, E-30, E-31 Usage ve Machine: `facilityId=2&machineId=1`, `facilityId=1&lineId=2&machineId=1`, `facilityId=2&lineId=1` hatasız boşaltma; `machineId=999`, `lineId=999`, ters tarih hata (K2, PL-97) | GEÇTİ (E-31 Makine modunda `lineId=999` için bkz. B9) | `Tur2Olcumler.txt` "B1": E-34, E-30 ve E-31 Usage'da üç uyuşmazlık girdisi hata vermiyor, uyuşmayan hat ve makine boşalıyor, seçenekler yeni üst seçime göre doluyor (ör. tesis 2: hat 1 seçenek, makine 1 seçenek, seçili yok). E-30'da `facilityId=1&lineId=2&machineId=1` makine boşaltılıp H2 hattının raporu çiziliyor. `machineId=999`, `lineId=999`, `facilityId=999` ve ters tarih üç ekranda `ErrorMessage` ve tablo 0. E-31 Makine modunda `machineId=999` ve ters tarih hata, uyuşmazlık girdilerinde makine boşalıyor; `lineId=999` ve `facilityId=999` yok sayılıyor (B9). Yalnız `machineId=1` ile E-34, E-30 ve E-31 Usage'da tesis T1 ve hat H1 makineden doluyor. JS kapalı E-30: tesis 2'ye geçince hata yok, hat ve makine boşaldı, seçenekler T2'ye göre |
| B2 | 320 ve 360 px, açık ve koyu | GEÇTİ | Çip iki satıra kırılıyor, sağ kenarı kartın iç kenarında (290 / 290 px); E-34 makine 2 ve E-31 malzeme tablosu 320 ve 360 px açık ve koyu kart kusuru 0, hücre taşma 0 (`Tur2Olcumler.txt` "B2", matris) |
| B3 | `mode=Usage&from=abc&machineId=1` modu ve makineyi korur, hata metni görünür | GEÇTİ | Etkin mod düğmesi "Kullanım oranı" (`aria-current="page"`), makine 1, tesis T1, hat H1 seçili, "Süzgeç değerlerinden biri okunamadı..." görünür. `mode=Machine&from=abc&machineId=1` de modu ve makineyi koruyor |
| B5 | Boş dal kaldırılır | GEÇTİ | `Machine.cshtml:127` `@if (machine is null)` altında boş durum yalnız `ErrorMessage` boşken; ters tarihte yalnız bildirim, boş durum yok |
| B7 | E-30 süzgeç düğmeleri 1280 ve 1440 px'te alanlarla hizalı ya da tam satırda | GEÇTİ (tam satırda) | 1280 ve 1440 px açık ve koyu: "Raporla" ve "Temizle" alanların altında tek satır (üst 426 px, yükseklik 48 px), ilk düğmenin sol kenarı alanların sol kenarıyla aynı (0 px), alanlardan 16 px aşağıda. Matris G12 süzgeç denetimi E-30 her hücrede kusur 0 |
| A1 | `/records` 1280, 1300, 1366, 1440 px açık ve koyu, uzun adlı kayıtta "Ayrıntı" tam görünür ve tıklanır | KALDI (B8) | 1280: 56 px, 1300: 36 px kırpılma, 25 satırın hepsinde, iki temada; düğmenin sağ kenarı tıklanmıyor. 1366 ve 1440: düğme iç kenardan 12 px içeride, tam görünür, merkez ve sağ kenar tıklanıyor (`Tur2Olcumler.txt` "A1", `Kayitlar-1280-Kirpilma.png`) |

Gerileme:

- K3, K8, K9, K11: `Tur2Olcumler.txt` "Gerileme": makine raporu 13, kullanım oranı 13, E-30 makine satırı ve bağlantı 5 süzgeç bileşiminde SQLite ile sapma 0 (tur 1 ile aynı betikler). GEÇTİ.
- K12: JS kapalı Chromium'da E-34 ve E-31 Usage süzgeci tesis, hat ve makine adımlarıyla sunucudan yenileniyor; tesis değişince eski seçim hatasız düşüyor. GEÇTİ.
- E-30 süzgeç ve grafik (Faz 6A/6B): JS kapalıyken üç grafik çiziliyor (kayıt sayısı 7, ortalama süre 5, faz ortalaması 8 çubuk; minimumun altındaki 7 faz desenli dolgu ve "asgarinin altında" metniyle), tablo değerleri SQLite ile aynı, süzgeç tesis değişiminde seçenekleri yeniliyor. GEÇTİ.
- Matris: 11 adres (E-34 beş durum, E-31 Usage ve Machine, E-30 iki durum, `/records`) x 320, 360, 640, 768, 1024, 1280, 1440, 1920 açık ve koyu, 1920 reduced-motion = 154 hücre. 152 hücre beklenen değerlerde (konsol 0, `hScroll` false, `smallTargets` 0, `inputNoLabel` 0, token dışı köşe 0, G12 süzgeç kusuru 0); 2 kusurlu hücre `/records` 1280 açık ve koyu (B8). G11 imzaları tur 1 ile aynı.
- B6 (devredildi) notu: frontend devir notuna eklenen yeni imzalar 360, 768 ve 1024 için tek 2 sütunlu düzen yazıyor (ör. tip tablosu `Net:2@2`); ölçülen ve G11'in istediği 768 ve 1024 düzeni 3 sütun (`Brüt ve mola:1@1 Net:1@1 Efor:1@1`). Yerleşim doğru, yazılı beklenti eksik.

Kapsam dışı (tur 2): E-30'da okunamayan değer (`from=abc&machineId=1`) süzgeci hâlâ sıfırlıyor (tesis, hat, makine seçili değil); Faz 6A'dan kalma, analistin tur 2 kapsamında değil, backend devir notunda yazılı. Bulgu yazılmadı.

Tur 2 özeti: B1, B2, B3, B5, B7 kapandı; A1 düzeltmesi 1280 ve 1300 px'te tutmadı (B8, YÜKSEK); B9 (DÜŞÜK) analist kararı bekliyor. Kalan bölümler (2-5) tur 1 ölçümüdür; tur 2 sonuçları bu bölümdedir.

## Tur 3 (2026-11-16)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `970cacd`; tur 3 değişikliği `Views/Records/Index.cshtml`, `Styles/app.css`). Kaynak `.qa/f6d/src3` altına kopyalanıp oradan derlendi. Devir notu: frontend "Tur 3 (B8, B6)". Kullanıcı onayı O-12 |
| Ortam | LOCAL, Release build, port 5799 (Production), DB `.qa/f6d/qa.db` (tur 1 verisi; yeni veri kurulmadı). Yayın `.qa/f6d/build3`. Sunucu PID 20804 (bizim) sonda kapatıldı; PID 32568 dokunulmadı |
| Kapsam | Analistin tur 3 şartı: B8 (`/records` 1280, 1300, 1366, 1440 açık ve koyu), 360/768/1024 kart modu ve G11 imzası, G12, konsol, liste araması ve sayfalama, değişen dosyalarda mekanik eval ve grep; `.table-stack` kullanan başka listelerde gerileme |
| QA önerisi | KALDI (B10 YÜKSEK açık) |

Derleme ve mekanik: `dotnet build -c Release -warnaserror` 0 uyarı 0 hata; `npm run build:css` çıktısı repodaki `app.css` ile bayt bayt aynı (73960 bayt). `Records/Index.cshtml` ve `Styles/app.css`'te `UiDelivery.md §4` grep 0 ihlal, em/en-dash 0, ham renk 0, gizli `value="false"` 0; yeni kural `.table-stack td.wrap-anywhere { overflow-wrap: anywhere }` (`app.css:632-634`). Sunucu günlüğünde `fail`/`crit`/exception 0. Konsol 84 hücrede 0.

| Madde | Yeniden denetim şartı | Sonuç | Kanıt |
|---|---|---|---|
| B8 | `/records` 1280, 1300, 1366, 1440 px açık ve koyu, 25 satırda "Ayrıntı" sağ kenarı dahil tam görünür ve tıklanır, tablo panelden geniş değil | GEÇTİ | Sekiz ölçümün hepsinde tablo = panel içi (964, 984, 1050, 1124 px), kırpılan satır 0, "Ayrıntı" sağ kenarı panel iç kenarından 12 px içeride, düğmenin ortası ve sağ kenarı tıklanıyor (`elementFromPoint`), sayfa yatay kaydırmasız. Hücre taşma ve hücre dışına çıkan öğe 0. 1280 px sütunlar: Kayıt no 169, Makine ve yer 226, Sahip 90, Durum 154, Açılış ve net süre 190, İşlem 136 (`Tur3Olcumler.txt` "B8", `Kayitlar-1280-Kirpilma.png` tur 2 durumunu gösterir) |
| Kart modu ve G11 | 360, 768, 1024 px; birleşik sütunda iki etiket görünür | KALDI (B10) | İmza 360 `Baş[Kayıt no\|Durum(alt satır)] Gövde[Makine ve yer:tam@1 Sahip:1@2 Açılış ve net süre:1@2] Ayak[İşlem]`; 768 ve 1024 `Baş[Kayıt no\|Durum] Gövde[Makine ve yer:tam@1 Sahip:1@2 Açılış ve net süre:1@2] Ayak[İşlem]` (3 sütunlu gövde, son satırda boş hücre, kurala uygun). Birleşik hücrede etiket "Açılış ve net süre" üstte, içinde "Açılış" ve "Net süre" iç etiketleri iki satırda görünüyor (25/25 satır). 360 ve 320 px'te açılış satırı kartın dışına taşıyor (B10); 768 ve 1024 temiz |
| G12 | Hizalama | GEÇTİ (1280-1440) | Birleşik sütunda iki satırın sağ kenarı başlığın sağ kenarıyla aynı (1280: 1098 / 1098 / 1098 px; 1300 ve 1440 aynı biçimde); kart başlığında tanıtıcı ve gövde sol kenarı, rozet ve ayak düğmesi sağ iç kenarda (matris kart kusuru 0, 360'taki taşma dışında) |
| Arama ve sayfalama | Gerileme | GEÇTİ | 33 kayıt: sayfa 1 25 satır, sayfa 2 8 satır, sayfalama bağlantıları doğru; `query=IE-QA6D-0001` 1 sonuç (kayıt 13), kayıt no ile 1 sonuç, lot `L2026-001` ile 2 sonuç (kayıt 11 ve 13, DB ile aynı), `zzzz` boş durum; durum ve makine süzgeçli listeler DB sayısıyla aynı (20, 24). Her satırda birleşik hücre ve iki iç etiket (`Tur3Olcumler.txt` sonu) |
| `.table-stack` gerilemesi | Başka bir liste | GEÇTİ | `/machines` ve `/materials` 320-1920 px açık ve koyu: kart imzası `Baş[Kod\|Durum] Gövde[Ad:tam@1 ...] Ayak[İşlem(2)]` tur öncesiyle aynı, 1280 ve üstünde tablo panele sığıyor, kart kusuru 0. Makine raporu (E-34) üç tablosunun imzası tur 1 ve 2 ile aynı |

Not (bulgu yazılmadı): `/machines` "Kullanımdan kaldırılanları göster" ve `/materials` "Pasifleri göster" onay kutuları 20 x 20 px (`smallTargets`); `.table-stack` ile ilgisiz, Faz 2'den kalma, bu ekranlar tur 1 ve 2'de ölçülmemişti. Analistin bilgisi için.

B6 notu (devredildi): frontend tur 3 devir notunda tip tablosu için "360/768/1024 (kart, 3 sütun)" yazıyor; 768 ve 1024 doğru, 360 px gövdesi 2 sütun (ölçülen ve G11 ile uyumlu). Yerleşim doğru, yazılı beklenti 360 için eksik.

Kanıt dosyaları: tur 3 için `Tur3Olcumler.txt` ve `Kayitlar-360-Tur3.png` eklendi; 10 dosya sınırı için kapanmış B2 ve B7'nin tur 1 görüntüleri (`KullanimOrani-320-SktTasma.png`, `SureRaporuSuzgec-1280.png`) çıkarıldı, bu maddelerin kapanış ölçümleri `Tur2Olcumler.txt` içinde.

Tur 3 özeti: B8 kapandı (1280-1440 px açık ve koyu temiz). Tur 3 değişikliği `/records` kart modunda 320 ve 360 px'te yeni bir kırık yerleşim getirdi (B10, YÜKSEK). B9 analist kararıyla kapandı (PL-98). Kalan açık bulgu yalnız B10.

## Tur 4 (2026-11-16)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `970cacd`; tur 4 değişikliği yalnız `Views/Records/Index.cshtml:194-195`, `whitespace-nowrap` yerine `xl:whitespace-nowrap`). Kaynak `.qa/f6d/src4` altına kopyalanıp oradan derlendi. Devir notu: frontend "Tur 4 (B10)". Kullanıcı onayı O-13 |
| Ortam | LOCAL, Release build, port 5800 (Production), DB `.qa/f6d/qa.db` (yeni veri yok). Yayın `.qa/f6d/build4`. Sunucu PID 34332 (bizim) sonda kapatıldı; PID 32568 dokunulmadı |
| Kapsam | Yalnız `/records` (B10): 320 (%200 yakınlaştırma), 360, 768, 1024, 1280, 1300, 1440 px; açık ve koyu; sayfa 1 (25 satır) ve sayfa 2 (8 satır) |
| QA önerisi | GEÇTİ |

Derleme ve mekanik: `dotnet build -c Release -warnaserror` 0 uyarı 0 hata; `npm run build:css` çıktısı repodaki `app.css` ile bayt bayt aynı (73960 bayt). `Records/Index.cshtml`'de `UiDelivery.md §4` grep 0 ihlal, em/en-dash 0, ham renk 0. Sunucu günlüğünde `fail`/`crit`/exception 0.

| Madde | Yeniden denetim şartı | Sonuç | Kanıt |
|---|---|---|---|
| B10 | Kart ve tablo modunda hücre taşması yok, sayfa yatay kaymıyor | GEÇTİ | 28 ölçümün hepsinde (2 sayfa x 7 genişlik x 2 tema) içeriği hücre ya da kart iç kenarı dışına çıkan hücre 0. Kart modunda en büyük aşım 0 px (açılış satırı iki satıra kırılıyor: "Açılış 16.11.2026" / "21:55"), tablo modunda içerik iç kenardan en az 12 px içeride. `scrollWidth` her genişlikte pencere genişliğine eşit (320'de 320) (`Tur3Olcumler.txt` "Tur 4", `Kayitlar-360-Tur3.png` tur 3 durumunu gösterir) |
| "Ayrıntı" | Tam görünür ve tıklanır | GEÇTİ | 33 satır x 14 ölçüm: düğme kart ya da panel iç kenarının içinde, metni "Ayrıntı" tam; sol, orta ve sağ noktada `elementFromPoint` düğmeye düşüyor (görünmeyen 0, tıklanmayan 0) |
| İç etiketler ve G11 | İki iç etiket görünür; imza tur 3 ile aynı | GEÇTİ | Birleşik hücrede "Açılış" ve "Net süre" iç etiketleri her satırda görünür (sayfa 1'de 50/50, sayfa 2'de 16/16). İmza 320 ve 360 `Baş[Kayıt no\|Durum(alt satır)] Gövde[Makine ve yer:tam@1 Sahip:1@2 Açılış ve net süre:1@2] Ayak[İşlem]`; 640, 768, 1024 `Baş[Kayıt no\|Durum]` ve aynı gövde; 1280-1440 tablo, satır en az 85 px, kapsayıcı 10 px, tablo panelden geniş değil. Tur 3 ile aynı |
| Konsol ve mekanik eval | Konsol 0 | GEÇTİ | 32 hücrelik eval matrisi (320, 360, 640, 768, 1024, 1280, 1300, 1440 x açık ve koyu x 2 sayfa) kusurlu hücre 0, konsol 0 |

Kanıt: tur 4 ölçümleri 10 dosya sınırı nedeniyle `Tur3Olcumler.txt` dosyasının sonuna "Tur 4" başlığıyla eklendi; dosya sayısı 10.

Tur 4 özeti: B10 kapandı. Bulgu tablosunda açık KRİTİK, YÜKSEK ve ORTA bulgu yok. B1, B2, B3, B5, B7, B8 ve B10 kapandı; B9 analist kararıyla kapandı; B4 ve B6 analist kararıyla devredildi. K1-K12 GEÇTİ (tur 1; gerilemeler tur 2 ve tur 3'te temiz).

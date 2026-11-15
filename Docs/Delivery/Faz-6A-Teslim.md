# Faz 6A Teslim Raporu: Acil müdahale, süre ve efor raporu, malzeme izlenebilirliği

| Alan | Değer |
|---|---|
| Faz | 6A |
| Tarih | 2026-11-15 |
| Tur | 1 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `bdcc6b1`; Faz 6A kodu commit'lenmemiş, ön tur bulgusu ÖT-1 düzeltmesi dahil). Devir notları `Docs/Handover/backend-engineer-20261115-1845.md` ("Ek" dahil), `Docs/Handover/frontend-engineer-20261115-2130.md` |
| Ortam | LOCAL, Release build, port 5621 (Production), DB: `.qa/` kopyası. Ön tur `.qa/f6a/build`, ÖT-1 yeniden ölçümü ve teslim testi `.qa/f6a/build2`. `.qa/f6a/qa.db` = `App_Data/vaka.db` SQLite yedekleme API kopyası + `DemoData.sql` + uygulama akışıyla kurulan veri (`.qa/f6a/s0.py`-`s4.py`). `EventChain__HmacKey` yalnız süreçte üretilen rastgele 32 bayt (yazılmadı). Chromium (playwright-cli) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ |
| Analist kararı | TESLİM ALINDI (tur 1, 2026-11-16; DÜŞÜK B1, B2 D-1 düzeltme işine) |

## 1. Kapsam

- BE: acil açma servisi (yalnız Acil türü, eşleme aranmaz, açıklama zorunlu, malzeme aynı formda, PIN; açılış ve ilk adım tek işlemde, aynı sunucu anı; `A` kayıt no, saha referansı yok; makine ve kişi kısıtları), İşlerim `CanOpenEmergency`, süre ve efor raporu (makine ve faz ortalaması, yalnız Tamamlandı, SH-15 `CalculateAverage`, durum sayıları, süre altı, en uzun adımlar, tür süzgeci, geçersiz süzgeçte `ErrorMessage`), malzeme izlenebilirliği (lot ve makine), Faz 4-5 servislerinden ortak parçaların ayrılması.
- FE: E-27 Acil müdahale (İşlerim düğmesi, çağlayan, LotPicker, JS kapalı `refresh=true`), yürütme ve detayda acil açıklama ve "Saha defteri Yok", E-30 Süre ve efor (yalnız tablo), E-31 Malzeme izlenebilirliği (iki adımlı lot akışı, makine modu), Yönetici ve Denetçi menüsünde "Raporlar", `siren` ikonu.
- Ön tur bulgusu ÖT-1 düzeltmesi: `_PinInput.cshtml:17` `data-field`, `FormValidation.js:39` `closest("[data-field], .field")`.
- DB: değişiklik yok (yeni tablo, index ve migration yok; K-33).

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Yalnız Acil türünde, yürürlükte versiyonu olan tipler; makine eşlemesi aranmaz | GEÇTİ | `Evidence/Faz-6A/KabulKriterleri.txt` "K1": radyolar tip 2 ve 5 (DB'deki acil + yürürlükte kümesiyle aynı); yalnız taslak versiyonlu ACL3 listede yok; acil tiplerin `MachineCleaningTypes` eşlemesi 0 iken 1, 2, 3, 6, 7, 8, 9, 10 numaralı makinelerde kayıt açıldı; planlı tip kimliğiyle POST "Acil tipi seçin." |
| K2 | Kayıt açılır ve ilk adım başlar; açılış = ilk adım başlangıcı; `...-A<Yıl>-<Sıra>`; saha referansı "yok" | GEÇTİ | "K2" tablosu: 12 acil kayıtta `OpenedAtUtc` = `StartedAtUtc` = ilk adım başlangıcı; olay 1 `RecordOpened`, olay 2 `StepStarted`, aynı CommandId ve aynı an; `FieldBookRef`, kilit ve iş emri NULL; no `T1-H1-M01-A2026-3` gibi (sıra makine ve yıl başına, planlıyla ortak, BR-08). Yürütme ve detayda "Saha defteri Yok", planlı kayıtta `T1-SD-2026-3` |
| K3 | Açıklama boşken kayıt açılmaz; malzeme zorunlu tipte malzemesiz ret | GEÇTİ | Boş, yalnız boşluk, 1001 karakter, malzemesiz zorunlu tip ("Bu acil tip malzeme ister. En az bir malzeme ekleyin."), SKT geçmiş lot gerekçesiz, olmayan lot, kullanımdan kalkmış makine, hat-makine uyumsuz, yanlış PIN: hepsinde 200, kayıt +0, olay +0; CommandId korunuyor, PIN alanı boş; yanlış PIN sayacı 0 -> 1. Malzeme zorunlu tip Yönetici akışıyla kuruldu (tetikleyici düşürülmedi) |
| K4 | Makine kilitli ya da aktif kayıt varken acil açılmaz; mesaj kayıt, sahip ve durum | GEÇTİ | Devam eden acilde: "Makinede açık kayıt var. Sahip: QA Operatör 10, durum: devam ediyor, açılış: 15.11.2026 19:15" + "Kaydı görüntüle" `/records/run/12`; Açıldı planlıda aynı biçim `/records/run/14`; aktif kaydı olan operatör: "Açık kaydınız var: T1-H1-M06-A2026-1 (devam ediyor)..." + bağlantı. Molada POST 302 `/breaks/current`, kayıt +0. Eşzamanlı acil + planlı aynı makine: biri 302, diğeri 200 kilit mesajı, makinede tek açık kayıt; eşzamanlı iki acil: aynı sonuç |
| K5 | Acil kayıtta son adım gerekçesiz biter | GEÇTİ | Tüm acil kayıtlarda adım bitirme doğrudan 302 (gerekçe formu yok); tamamlanan acil fazlarında `BelowMinimum` 0, gerekçe NULL, minimum NULL |
| K6 | Liste ve detayda metinli "Acil"; tür süzgeci | GEÇTİ | `/records?kind=2&mineOnly=false` 12/12, `kind=1` 5/5, süzgeçsiz 17/17 DB ile aynı; detayda "Acil" rozeti, açıklama, "Bütünlük Doğrulandı" |
| K7 | E-27 360'ta kaydırmasız, gönder 44 px; aynı CommandId tek kayıt | GEÇTİ | 360 ve 320'de `scrollWidth` = `innerWidth`; "Kaydı aç ve başlat" 228x56, "Vazgeç" 109x56. G9: aynı CommandId iki POST, ikisi de 302 `/records/run/15`, kayıt +1, olay +2, `ProcessedCommands` 1. Tarayıcıda çift tıklama: tek POST, kayıt +1 (`JsDavranis.txt`) |
| K8 | Makine ve faz bazında ortalama brüt, mola, net, efor ayrı kolonlarda | GEÇTİ | `RaporHesap.txt`: makine tablosu Kayıt, Brüt, Mola, Net, Efor kolonları; 13 süzgeç bileşiminde makine, faz, adım ve durum tabloları DB'den bağımsız elle hesapla birebir |
| K9 | Yalnız Tamamlandı ortalamaya girer; iptal ortalamayı değiştirmez; iki tamamlanmış kaydın ortalaması elle hesapla aynı | GEÇTİ | Makine 1 Acil müdahale: kayıt 3 net 4 sn ve kayıt 6 net 8 sn -> ekranda "6 sn" (brüt 8, mola 2, efor 6). Makine 1'de ilk adımı bitmiş acil kayıt iptal edildi: makine, faz ve adım tabloları önce ve sonra aynı; "İptal edildi" 1 -> 2. Zaman aşımı ve Yarıda kapatıldı sayıları ayrı |
| K10 | Aynı tipin v1 ve v2 faz ortalamaları ayrı satır; süre altı sayısı faz satırında | GEÇTİ | "Acil müdahale, versiyon 1 Faz 1" (3 kayıt, 4 sn) ve "versiyon 2 Faz 1" (3 kayıt, 6 sn) ayrı; KSA ve PLN fazlarında "1 kayıt" süre altı çipi, acilde asgari "-" |
| K11 | En uzun adımlar; planlı ve acil tür süzgeciyle ayrılır | GEÇTİ | İlk 10 adım ortalama süreye göre DB ile aynı sırada; `kind=1` ve `kind=2` (düz ve `Filter.Kind=2` önekli) tabloları ve durum sayıları DB kümeleriyle aynı |
| K12 | Lotla arama: makine, tarih, kayıt no; makineyle arama: tarih sırası; SKT geçmiş işaretli | GEÇTİ | Lot 1-5 ve makine 1, 2, 3, 9, 10 sonuçları DB sorgusuyla birebir (sıra, kayıt no, makine, açılış/giriş saati, durum, `/records/details/{id}` bağlantısı, "SKT geçmiş"); çıkarılmış malzeme listelenmiyor; tarih süzgeci lot modunda açılışa, makine modunda girişe; boş ve "sorgu yok" durumları |
| K13 | Operatör `/reports/*` 403; Yönetici ve Denetçi açar; Yönetici ve Denetçi `/emergencies/*` 403 | GEÇTİ | `YetkiMatrisi.txt` ve bölüm 4 |
| K14 | SH-15 formülü `IDurationCalculator` XML yorumunda | GEÇTİ | `Services/IDurationCalculator.cs:205-213` "Ortalama = Σ değer / kayıt sayısı; saniyeye aşağı yuvarlanır (tam bölme); değer yoksa 0", remarks yalnız Tamamlandı ve versiyon içi; `DurationCalculator.cs:177-178` long toplam / adet, yorumla aynı |
| K15 | E-30 ve E-31 G11 imzası, üç ekran G12 hizalaması | GEÇTİ | `MekanikEval.txt`: rapor tabloları 198/198 beklenen imza (360 kart 2 sütun, 768/1024 kart 4 sütun, 1440 ve 1920 tablo, satır en az 67 px), uzun makine ve adım adıyla aynı; `G12Hizalama.txt`: E-27, E-30, E-31 dört genişlik açık ve koyu kusur 0 |
| G1 | `-warnaserror` 0/0; `build:css` | GEÇTİ | Bölüm 3 |
| G2 | §4 grep 0; öznitelikler; rol metni | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi, §4, konsol, DNA | GEÇTİ | 468 teslim hücresi + 160 gerileme hücresi; Faz 6A ekranlarında kırık hücre yok; konsol 0; §4 0; DNA sapması yok. 640x400'de Faz 6 ekranlarında DÜŞÜK B2 |
| G4 | 360'ta kaydırma yok, birincil 44 px, durum metinle | GEÇTİ | E-27, İşlerim, raporlar 360 ve 320'de `hScroll` false; "Acil müdahale aç" 292x56, "Kaydı aç ve başlat" 228x56; rozetler ikon + metin ("Acil", "SKT geçmiş", durumlar) |
| G5 | Belirteçsiz POST 400 | GEÇTİ | Operatör belirteçsiz POST `/emergencies/create` 400 |
| G5a | Her POST formu kendi belirtecini taşır | GEÇTİ | Teslim hücrelerinde 1547 POST formu sayıldı, belirteçsiz 0 |
| G5b | `bool` onay kutuları | GEÇTİ | Faz 6A formlarında onay kutusu yok; `Views/**` içinde `type="hidden"` + `value="false"` 0; yeni gizli alanlar yalnız `CommandId`, `mode`, `LotQuery` |
| G6 | em/en-dash 0 | GEÇTİ | `Src` (cs, cshtml, js, css, md, sql) 0 dosya; eval `emdash` false; kanıt dosyaları 0 |
| G7 | `Up()` Drop yok | GEÇTİ | Faz 6A'da `Data/Migrations` ve `Docs/Db` değişmedi |
| G8 | Controller ve görünümde süre aritmetiği yok | GEÇTİ | `Controllers/`, `Views/` 0 eşleşme; Faz 6A servislerinde `TimeSpan` ya da zaman çıkarması 0; süreler `IDurationFormatter` ve `CalculateAverage` üzerinden |
| G9 | Aynı CommandId tek etki | GEÇTİ | K7 satırı |
| G10 | Süper admin yeni adreslerde 403, menüde yok | GEÇTİ | `/emergencies/create` GET ve POST, `/emergencies/machines`, `/reports/durations`, `/reports/materials` 302>403; menüde rapor ve acil bağlantısı yok |
| G11 | Yerleşim imzası | GEÇTİ | Rapor tabloları 198/198; build2'de Faz 1-6 liste ekranları ve `/records` 127/127 (ön turda 112/112) |
| G12 | Hizalama kusuru 0 | GEÇTİ | ÖT-1 düzeltmesi sonrası 128 PIN/istemci hata hücresi ve 468 teslim hücresi dört genişlik, açık ve koyu temada kusur 0; göz düğmesi 44x48 ve alanın içinde, hata satırı alanın altında |

Kabul özeti: Faz 6A kriterleri K1-K15 15/15; ortak kriterler (G1-G12, G5a, G5b) 14/14; toplam 29/29.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental -o .qa/f6a/bchk` (ön tur) ve `.qa/f6a/bchk2` (ÖT-1 sonrası): 0 Uyarı, 0 Hata; `dotnet publish -c Release` iki kez başarılı |
| CSS | `npm run build:css` | QA çıktısı (`.qa/f6a/app.check2.css`) `wwwroot/css/app.css` ile bayt bayt aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme; sihirli rol metni 0 (`Authorize` ve `IsInRole`); `EmergenciesController` `[Authorize(Roles = Roles.Operator)]`, `ReportsController` `[Authorize(Roles = Roles.ManagerOrAuditor)]`; özniteliksiz iki dosya controller değil. MediatR, AutoMapper, generic repository, `new ...Service(` 0; `GetService` Program.cs dışında 0; `Migrate()` 0. Controller'lar yalnız model bağlama, servis çağrısı ve sonuç çevirisi yapıyor |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: `EmergencyOpener`, `EmergencyOpenValidator`, `DurationReportReader`, `MaterialTraceReader`, `MaterialTraceMode.Lot/Machine`, `ReportMessages`, `IRecordBlockFinder`, `StepStartApplier`, `ViewModels/Emergencies`, `ViewModels/Reports`, `Views/Reports/Durations.cshtml`. §8: Faz 6A dosyalarında `#region`, süs yorum, emoji, `Helper/Utils/Manager` 0. `DurationReportReader` 227 satır (200 üstü, bölünmeye aday; bulgu sayılmadı), `EmergencyOpener` 14 bağımlılık |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (tam tablo; yeni `<input>`larda `autocomplete`, ikon düğme yok) |
| Mekanik eval | `UiDelivery.md §3` | Ön tur 518 hücre (ÖT-1 dışında temiz); teslim 468 + gerileme 160 hücre: Faz 6A ekranları temiz; en düşük kontrast 5,28:1, üst bar 64 px, 1920'de ana alan 1600 px, reduced-motion'da animasyon 0, konsol 0 |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Faz 6A'da migration yok; `Data/Migrations` son değişikliği `8730e7e` (Faz 6), indeks 8 kayıt, değişmedi |

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Denetçi (3001), Editor = Operatör (2001), Admin = Süper admin (0001); Yönetici (1001) parantez içinde. Tam çıktı `Evidence/Faz-6A/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/emergencies/create` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 | 302>403 |
| POST `/emergencies/create` | 302 (giriş) | 302>403 (Yönetici 302>403) | belirteçli 200 nedenli ret / 302 başarı; belirteçsiz 400 | 302>403 |
| GET `/emergencies/machines?lineId=` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 JSON | 302>403 |
| GET `/reports/durations` | 302 (giriş) | 200 (Yönetici 200) | 302>403 | 302>403 |
| GET `/reports/materials` | 302 (giriş) | 200 (Yönetici 200) | 302>403 | 302>403 |

Menüde gizli olup adresle açılan sayfa yok. Geçersiz süzgeçlerde 18 durum (süre 11, malzeme 7) 200 ve "Rapor hazırlanamadı" uyarısı, tablo yok.

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/my-work` (acil paneli; aktif kayıtlı operatörde panel yok) | "Acil müdahale aç" tam genişlik 56 px | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | 320x640, 640x400 temiz | Ölçülmedi (Faz 4-5 sırası değişmedi) |
| `/emergencies/create` (GET, JS çağlayan, sunucu reddi, istemci boş gönderim, JS kapalı yenileme ve lot ekleme, 52 lotla kesik liste) | Ray altta, 56 px gönder | Ray altta | Yan yana (356/352) | Yan yana (756/352) | Temiz | Temiz | Temiz | Temiz | Atlama bağlantısı, tesis, hat, makine, tip, açıklama, lot arama; hepsi halkalı |
| `/records/run/12` (acil, açıklama, "Saha defteri Yok") | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Ölçülmedi |
| `/records?kind=2`, `/records/details/{3,8,11,12}` | Temiz | Temiz | Temiz | Tablo | 1600 ile sınırlı | Temiz | Temiz | 320 temiz; 640x400 G12[f] (B2) | Ölçülmedi |
| `/reports/durations` (dolu, `kind=2`, süzgeçli boş, hata, tesis+hat) | Kart 2 sütun | Kart 4 sütun | Kart 4 sütun | Tablo, satır 67 px | 1600 ile sınırlı | Temiz | Temiz | Temiz | Menü, süzgeç alanları sırayla halkalı |
| `/reports/materials` (sorgu yok, lot arama, lot sonucu, boş aralık, olmayan lot, makine modu, uzun ad, kesik liste uyarısı) | Kart 2 sütun | Kart 4 sütun | Kart 4 sütun | Tablo | Temiz | Temiz | Temiz | Temiz | Sekmeler, arama, seçim halkalı |
| Faz 1-6 gerilemesi (15 Yönetici, 5 Operatör, 3 Süper admin sayfası; 13 PIN ve istemci hata formu) | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | Ölçülmedi | Ölçülmedi | Ölçülmedi |

Dört durum: E-30 ve E-31'de yükleniyor (form gönderiminde iskelet görünür, içerik gizlenir, "Yükleniyor…"), boş (süzgeçli ve sorgusuz), hata (`ErrorMessage` uyarısı), dolu ölçüldü (`JsDavranis.txt`).

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Archivo başlık, Hanken Grotesk gövde, süre ve kayıt no mono; `document.fonts.check` true.
- Palet: UYGUN. Yeni ham renk ve token yok; acil vurgusu mevcut `danger` ailesi (düğme, rozet, İşlerim paneli); en düşük kontrast 5,28:1.
- Köşe ölçeği: UYGUN. Panel, alan, düğme 2 px.
- Gezinme arketipi: UYGUN. "Raporlar" grubu Yönetici ve Denetçi rayında Makine panosundan sonra; alt çubuk değişmedi; Operatör ve Süper admin menüsünde yok.
- Sayfa iskeleti: UYGUN. E-27 ana kolon + bağlam rayı (lg altında altta); raporlar süzgeç paneli + tablolar.
- Veri dili: UYGUN. 1280 altında satır kartı, imzalar beklenen; sayılar sağa hizalı mono; durum şeridi `kv` listesi (eşit KPI kartı değil); boş durumlar ne olduğunu ve nasıl dolduğunu söylüyor.
- Hareket imzası: UYGUN. Yeni hareket yok; reduced-motion hücrelerinde animasyon 0.
- İmza bileşen: UYGUN. Kayıt şeridi değişmedi; acil kayıtta tek fazlı şerit.
- Anti-slop: temiz. Gradient, glow, eyebrow, sürüm rozeti, emoji, em-dash, CDN, grafik yok.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | DÜŞÜK | Faz 3 `/cleaning-types/create` istemci doğrulamasında "Tür seçin." hata satırı ilk radyo seçeneğinin etiketinin içine ekleniyor ("Planlı Tür seçin."). Radyo grubunun atasında `data-field` ya da `.field` yok; `FormValidation.js:39` `parentElement` olan `label.choice`'a düşüyor. ÖT-1 düzeltmesinden önce de aynı kod yolu (gerileme değil); Faz 6A ekranlarında yok (E-27 tip grubu `fieldset.field data-field`) | `.qa/f6a/shots/ct-err-360.png`; `Evidence/Faz-6A/OnTur.txt` son not; `Views/CleaningTypes/Form.cshtml:45` | frontend-engineer | `Form.cshtml:45` `fieldset#Kind`'i `class="field" data-field` taşıyacak biçimde sarmak (hata satırı grubun altına gelir) | AÇIK |
| B2 | DÜŞÜK | 640x400 (yatay telefon, %200 zoom karşılığı) satır kartında Faz 6 ekranlarının hücreleri kart izini doldurmuyor: `/records` "Makine ve yer" 30-478 px, iz 30-610 px (132 px kısa), "Sahip" 90 px kısa; detay tablolarında 4 px. Görselde taşma ya da kırılma yok, yalnız metin erken sarıyor; G12 genişlikleri (360/768/1024/1440) dışında, Faz 6A ekranlarında yok | `Evidence/Faz-6A/G12Hizalama.txt` 640x400 satırı; `.qa/f6a/shots/rec-640.png`; `Views/Records/Index.cshtml:170` `max-w-md`, `:174` `max-w-[12rem]` | frontend-engineer | Liste için sınırları tablo kipine almak (`xl:max-w-md`, `xl:max-w-[12rem]`); detay tablolarındaki 4 px farkın kaynağı bu turda belirlenmedi, aynı ölçümle bakılmalı | AÇIK |

Ön tur bulgusu (rapor tablosuna alınmadı, aynı oturumda düzeltildi ve yeniden ölçüldü): ÖT-1 ORTA, istemci tarafı boş PIN gönderiminde göz düğmesi 44x68, alan 52 px, alt taşma 20 px, merkez farkı 10 px. E-27, `/records/create`, Run adım bitirme ve `/interventions/create`'te görüldü (`OnTur-PinGoz-Once.png`). Düzeltme sonrası 128 hücrede kusur 0, göz 44x48, hata satırları alanın altında ve sol kenarı aynı. Giriş, kullanıcı, tesis, hat, makine, vardiya, malzeme, lot, temizlik tipi formları ve Faz 3 düzenleyicinin yayın formunda gerileme yok (`OnTur.txt`).

## 7. Geçenler

- Acil kayıt tek işlemde açılıyor ve ilk adım aynı anda başlıyor; açılış, ilk adım ve iki olay aynı an ve aynı CommandId; saha referansı, kilit ve iş emri yok; kayıt no `A` türüyle, sıra makine ve yıl başına.
- Bütün ret yolları (açıklama, malzeme, SKT gerekçesi, lot, makine, hat, tip, PIN, makine kilidi, kişinin aktif kaydı, mola) kayıt ve olay bırakmıyor; mesajlar kilidi tutan kaydı, sahibini, durumunu ve bağlantısını veriyor.
- G9 HTTP'de ve tarayıcıda çift tıklamada tek etki; eşzamanlı acil + planlı ve iki acil gönderimde makinede tek açık kayıt.
- Acil kayıtlar Faz 5 akışıyla gerekçesiz bitiyor; 10 acil kayıtta bütünlük "Doğrulandı".
- Süre ve efor raporu 13 süzgeç bileşiminde DB'den bağımsız elle hesapla birebir; iptal ortalamayı değiştirmiyor; v1 ve v2 faz satırları ayrı; Türkiye tarihi sınırı (UTC 22:30 kaydı ertesi günde) doğru.
- Malzeme izlenebilirliği lot ve makine bazında DB ile birebir; çıkarılmış malzeme listelenmiyor; tarih süzgeçleri doğru alana uygulanıyor.
- Yetki matrisi, Süper admin 403, belirteçsiz POST 400; 18 geçersiz süzgeçte 200 ve açıklayıcı uyarı.
- Faz 4-5 yeniden düzenlemesinden sonra planlı açma, saha referansı, adım başlatma ve bitirme, mola ve malzeme çıkarma çalışıyor.
- Aktif kaydı olan operatör `/emergencies/create`'te dolu formu görüyor, ret gönderimde "Açık kaydınız var ..." mesajı ve bağlantıyla geliyor. Faz 4 `/records/create` de aynı: iki sayfa 200 ve form gösteriyor, ret yalnız gönderimde. Moladaki operatörde de iki sayfa GET 200, POST 302 `/breaks/current`. Davranış tutarlı, bulgu yazılmadı.

## 8. Kapsam dışı / yapılamayan

- Türkiye tarihi sınırı doğal veriyle denenemedi (test UTC 16:00 civarında yapıldı). Bunun için kopyaya iki satır doğrudan eklendi (`QA-SINIR-1/2`, olay ve faz yok). Bu satırlar yalnız süzgeç sınırı için kullanıldı; bütünlük ve detay sonuçları anlamsızdır.
- Production anahtarı süreç başına üretildiği için App_Data'dan gelen kayıt 1 ve 2 "Bütünlük ihlali" gösterir; bu QA ortamının sonucudur.
- "Henüz tamamlanmış kayıt yok" boş durumu, tamamlanmış kaydı hiç olmayan bir veritabanı gerektirdiği için denenmedi; süzgeçli boş durum ölçüldü.
- "Kişi başına tek iş"in görevli (sahip olmayan) varyantı denenmedi; sahip olunan aktif kayıt ve mola varyantları ölçüldü.
- Eşzamanlılık iki ikili gönderimle sınırlı; yük testi yapılmadı.
- Chromium dışı tarayıcı, dokunmatik cihaz ve ekran okuyucu ile tur yapılmadı.
- Yürütme ekranı ve kayıt detayı için klavye turu bu fazda tekrarlanmadı (Faz 5-6'da ölçüldü; bu fazdaki değişiklik metin satırı).

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | B1 ve B2 (DÜŞÜK, önceki fazların ekranları) | frontend-engineer | Faz 6A ekranlarını etkilemiyor; yazılı gerekçeyle ertelenebilir |
| A2 | Rapor sorguları index'siz (K-33) | backend-engineer | Ölçek sayıları gelince ölçülmeli; DB-2026-008 kullanıcı onayı bekliyor |
| A3 | `DurationReportReader` 227 satır | backend-engineer | 200 satır üstü, bölünmeye aday (§2 S); bulgu değil |

## 10. Analist kararı

```
FAZ 6A: TESLİM ALINDI
Tarih: 2026-11-16
Gerekçe: K1-K15 ve G1-G12, G5a, G5b (29/29) kanıtla eşleşti; KRİTİK, YÜKSEK, ORTA açık bulgu 0 (ön tur ÖT-1 ORTA kapandı); QA önerisi
         GEÇTİ. DÜŞÜK B1 ve B2 Faz 6A ekranlarında değil (Faz 3 tip formu, Faz 6 kayıt listesi); Workflow.md §3 ve §4.4 gereği faz commit'inden
         sonra ayrı düzeltme işi D-1 olarak, Sunum hazırlığından önce kapanır (PL-74).
Açık kalemler: B1, B2 -> D-1 (frontend-engineer, qa-tester hedefli ölçüm).
```

Analistin doğruladıkları (kanıt dışında): `Views/CleaningTypes/Form.cshtml:45` `fieldset#Kind` `field`/`data-field` sarmalı taşımıyor;
`Views/Records/Index.cshtml:170` `max-w-md` ve `:174` `max-w-[12rem]` kart kipinde de geçerli. İki bulgu kodla tutarlı.

D-1 düzeltme işi (PL-74): frontend B1 (tür hatası alan altında, radyo etiketinin içinde değil) ve B2 (genişlik sınırları yalnız `xl:` tablo
kipinde) düzeltir; qa-tester hedefli ölçer: `/cleaning-types/create` tür seçmeden gönderimde hata metni fieldset'in altında ve `aria-describedby`
ile bağlı (JS açık ve kapalı); `/records` 640x400, 768, 1024'te kart hücreleri G11 imzasıyla aynı, erken sarma yok, 1440'ta tablo değişmez;
G12 ve G5a gerilemesi. Sonuç `Docs/Phases/PhasePlan.md` "Sunum hazırlığı" bölümüne yazılır; commit `Workflow.md §4.4` faz dışı düzeltme düzeniyle.

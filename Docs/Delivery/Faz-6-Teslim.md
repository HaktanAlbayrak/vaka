# Faz 6 Teslim Raporu: Kayıt listesi, detay, bütünlük ve müdahale

| Alan | Değer |
|---|---|
| Faz | 6 |
| Tarih | 2026-11-15 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `08a4975`; Faz 6 kodu commit'lenmemiş, tur 1'de yeni migration yok, DB-2026-008 TALEP durumundaydı; tur 2'de `20261115152212_AddCleaningRecordListIndexes` uygulanmış kopya). Devir notları `Docs/Handover/backend-engineer-20261115-1930.md`, `Docs/Handover/frontend-engineer-20261115-1900.md` ("Güncelleme (QA ön tur bulguları)" dahil) |
| Ortam | LOCAL, Release build, port 5601 (Production), 5603 ve 5604 (Development), DB: `.qa/` kopyası. Ön tur 1 `.qa/f6/build`, düzeltme sonrası ön tur 2 ve teslim testi `.qa/f6/build2`; tur 2 `.qa/f6/build3` ve yeni kopyalar. `.qa/f6/qa.db` = `App_Data/vaka.db` SQLite yedekleme API kopyası + `DemoData.sql`; 5601'de `EventChain__HmacKey` yalnız süreçte üretilen rastgele 32 bayt (yazılmadı); 5603/5604 user-secrets anahtarı (K7 bozulmuş kopyaları). Chromium (playwright-cli) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2: B1 kapandı, açık bulgu yok; tur 1: GEÇTİ, B1 ORTA açık) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-16); tur 1: İADE (B1 ORTA) |

## 1. Kapsam

- BE: kayıt sorgu servisi (süzgeçler, kayıt no, saha referansı, iş emri ve lot no araması, sunucu sayfalama, Operatör'de "benim kayıtlarım"), detay sorgusu (13 soru, versiyon içeriği, olay günlüğü, kapanış), bütünlük doğrulama (özet zinciri ve durum alanları), müdahale servisi (iptal, yarıda kapatma, yönetici mola bitirme), `/` yönlendirmeleri, prototiplerin kaldırılması, F5-B1 tek yuvarlama, F5-A1 detay bağlantısı, oturum sertleştirmesi (bellek içi iptal listesi).
- FE: E-19 Kayıtlar, E-20 Kayıt detayı ve yazdırma katmanı (denetim çıktısı), E-21 Bütünlük doğrulama, E-26 Müdahale ve mola bitirme formları, kabuk menüsü "Kayıtlar", F5-B2 `--scrim` perde tokeni; ön tur düzeltmeleri (sürüm bağlantısı 44 px, `_Moment` ve olay günlüğü zamanının tarih/saat parçalı kırılması).
- DB: DB-2026-008 index talebi (uygulanmadı, kod index'e bağlı değil).

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Tamamlanmış planlı kayıtta 13 soru detayda yanıtlı | GEÇTİ | `Evidence/Faz-6/KabulKriterleri.txt` "K1 R1 detay metni" (kayıt 11, KSA v4, iki görevli, molalı, iki lotlu): (1) Tesis T1 - Ana tesis; (2) Hat H1, Makine M10 - QA makine M10 (açılış anı adı); (3) "Kaydı açan (sorumlu) QA Operatör 11" ve "İlk adımı başlatan QA Operatör 12"; (4) her adımda görevliler ve ekleyen (onay zamanı B1); (5)(6) adım başlangıç ve bitiş, olay günlüğünde saniyeli; (7) net 11, brüt 15, mola 4 sn, bekletme -; (8) efor 16 sn; (9)(10) MLZ001, L2027-002 SKT 30.09.2027 ve L2026-001 SKT 30.06.2026 "SKT geçmiş" + gerekçe; (11) Kısa planlı temizlik, versiyon 4, doküman PRS-TM-014 Rev.3, versiyon bağlantısı; (12) iş emri IE-2026-0610; (13) Tamamlandı, tamamlanma 17:54; ek: bütünlük "Doğrulandı". Değerler DB zamanlarıyla elle aynı (brüt 14:54:21.877-14:54:37.629 = 15, mola 4,27 -> 4, efor 6+2+6+2) |
| K2 | Kaydı açan ile ilk adımı başlatan ayrı; brüt, mola, net, efor ayrı etiketli | GEÇTİ | Aynı metin: "Kaydı açan (sorumlu)" ve "İlk adımı başlatan" ayrı satır; "Net süre", "Brüt süre", "Mola", "Bekletme", "Efor (kişi süresi)" |
| K3 | v2 yayınlandıktan sonra v1 kaydının detayı v1 minimumu, açıklaması ve medyası | GEÇTİ | Kayıt 3 (v2) v3 yayınından sonra "Minimum 1 dk", "v2 açıklaması..." ve 2 medya; v3'te 2 dk ve "v3 açıklaması" |
| K4 | Makine adı değişince eski kayıt açılış anındaki adı gösterir | GEÇTİ | T6 K4: makine 13 "Karıştırıcı yeni adı" yapıldı; detay ve listede "QA makine M10", yeni ad görünmüyor |
| K5 | Operatör varsayılanı kendi kayıtları, kaldırınca hepsi; süzgeçler ve dört arama birlikte; lot no araması; sunucu sayfalama | GEÇTİ | T6 K5: op11, op12 (görevli) ve op20 varsayılanı DB'deki sahip/görevli kümesiyle eşit; `mineOnly=false` 11/11. Durum, tür, makine, tesis+hat+makine+durum+tür, tarih aralığı, kayıt no, saha referansı, iş emri, lot no (`L2027-002` 4 kayıt), küçük harf lot + durum, `%` metin olarak: 12 sorgunun hepsi DB sorgusuyla eşit. `pageSize=2`: sayfa 1 ve 2 ayrık, bağlantılar süzgeci taşıyor. Boş durumlar: süzgeçli ve "benim kayıtlarım" |
| K6 | Olay günlüğü tüm olayları sırayla, yapan ve sunucu zamanıyla | GEÇTİ | Kayıt 11: DB 18 olay = detayda 18 satır, sıra 1-18, yapan kişi ve saniyeli zaman, olay türleri DB ile aynı sırada |
| K7 | Değişmemiş kayıtta "Doğrulandı"; iş emri ya da lot değişince ihlal, alan, saklanan ve olaydaki değer; olay değeri değişince ilk tutarsız sıra | GEÇTİ | T6: müdahaleli ve müdahalesiz 7 kayıtta "Doğrulandı". K7 (Development kopyaları, 5604): kontrol kopyası "Doğrulandı"; iş emri "İş emri no, olay 1, IE-DEV-9999 / IE-DEV-0701"; lot no "Malzeme, lot ...: lot no, olay 2, L2027-999 / L2027-002"; adımı başlatan "olay 4, Test Yönetici (1001) / Test Operatör 2 (2002)"; sahte olay eklenince "İlk tutarsız olay sırası 17", satır "Sorunlu"; rozet her durumda "Bütünlük ihlali". Olay yükü ve adım zamanı UPDATE'i tetikleyiciyle reddedildi (bölüm 8) |
| K8 | `/records/integrity/{id}` Yönetici ve Denetçi'de 200, Operatör'de 403 | GEÇTİ | `YetkiMatrisi.txt`: Yönetici 200, Denetçi 200, Operatör (sahip ve yabancı) 302>403 |
| K11 | Yönetici ve Denetçi `/`'da `/records`'a; pano bağlantısı detaya; Süper admin `/records`, detay ve `/interventions/*` 403 | GEÇTİ | `/`: Yönetici ve Denetçi 302 `/records`, Süper admin `/users`, Operatör `/my-work`; pano bağlantıları Yönetici/Denetçi `/records/details/{id}`, Operatör `/records/run/{id}`; Süper admin dokuz adreste 302>403 |
| K13 | `Views/Prototype` ve `PrototypeController` yok | GEÇTİ | Çalışma ağacında dosyalar yok; `/prototype/run`, `/prototype/my-work` Production ve Development'ta 404; kaynakta `Prototype`, `_DemoBar` referansı 0. Not: silmeler indekste değil (`git ls-files` hâlâ listeliyor); commit'te silmelerin de eklenmesi gerekir |
| K14 | Yazdırma önizlemesi: menü, üst bar, alt çubuk, eylemler gizli; 13 soru, olay günlüğü, bütünlük, oluşturulma anı; A4 dikey taşma yok; denetim çıktısı içeriği | GEÇTİ | `MekanikEval.txt` misc6c (`emulateMedia print`, koyu tema açıkken): menü, üst bar, alt çubuk gizli, görünür düğme 0, zemin beyaz, yatay taşma yok, tablolar panelin 2 px içinde; "Denetim çıktısı", oluşturan (Yönetici, Denetçi, Operatör ayrı) ve an, "Doğrulandı", doküman no PRS-TM-014 Rev.3, "SKT geçmiş girildi. Gerekçe: ...", süre altı gerekçesi, kapanış bilgisi (iptal ve yarıda kapatma kayıtlarında), 21 olay satırı; A4 PDF 4-5 sayfa. Yazdır düğmesi `window.print` çağırıyor |
| K15 | İptal: İptal edildi, devam eden adım Kesildi ve bitişi müdahale anı, açık mola "Müdahale", makine Boş; silinmez, no yeniden kullanılmaz; Açıldı iptal edilebilir | GEÇTİ | T6: Açıldı iptal 302, durum 6, kapanış türü 1, kapatan, gerekçe, an; aynı makinede yeni kayıt `-P2026-2`, eski kayıt duruyor. Devam eden adımlı iptal: adım 4 (Kesildi), bitiş = kapanış anı, süre 4, efor 8, fazlar 5, `IsWorking` 0, olaylar `StepInterrupted`, `RecordCancelled`, panoda makine Boş. Açık molalı iptal: mola EndKind 3, `BreakEnded` + `RecordCancelled`, molayı başlatanın kilidi kalktı |
| K16 | Yarıda kapatma: Yarıda kapatıldı, makine Boş; Açıldı kayıtta sunulmaz, istekle reddedilir | GEÇTİ | Devam ediyor kayıtta türler İptal ve Yarıda kapatma; yarıda kapatma 302, durum 7, makinede açık kayıt 0. Açıldı kayıtta yalnız İptal sunuluyor; POST "Açıldı durumundaki kayıt yarıda kapatılamaz, yalnız iptal edilir.", olay +0 |
| K17 | Yönetici açık molayı gerekçe ve PIN'le bitirir; bitiş işlem anı; satırda "Yönetici" ve gerekçe; başlatanın kilidi kalkar | GEÇTİ | T6: mola EndKind 2, bitiş 14:54:54.691 (istek 14:54:54.582 sonrası), `EndReason` yazıldı, olay `BreakEnded {"EndKind":"Manager",...,"Reason":...}`; detayda "Gerekçe: ..." ve "Yönetici bitirdi, Test Yönetici"; op18 `/my-work` önce 302 `/breaks/current`, sonra 200 ve sıradaki adımı başlattı. Tarayıcıda form akışı (js6) |
| K18 | Operatör iptal/kapatma düğmesi görmez; `/interventions/*` Operatör ve Denetçi'de GET ve POST 403; Denetçi müdahale bağlantısı görmez | GEÇTİ | `YetkiMatrisi.txt`: Operatör ve Denetçi GET ve POST 302>403. Detay düğmeleri: Yönetici "Müdahale et", "Molayı bitir"; Denetçi yalnız "Yazdır", "Kayıtlara dön", "Doğrulama ayrıntısı"; Operatör "Yazdır", "Kayıtlara dön", "Yürütme ekranına git" |
| K19 | Gerekçesiz ya da yanlış PIN'li müdahale ve mola bitirme değiştirmez, olay yok; yanlış PIN sayılır; kapanmış kayıtta ret; kilidi geçmiş Açıldı kayıtta zaman aşımı ve ret | GEÇTİ | T6: gerekçesiz, 501 karakter, türsüz, yanlış PIN: 200, olay +0, durum değişmedi, Yönetici PIN sayacı 0 -> 1; mola bitirmede de aynı. Tamamlandı, İptal edildi, Yarıda kapatıldı ve Zaman aşımı kayıtlarına doğrudan POST nedenli ret, olay +0. Kilit 1 dk, 63 sn sonra iptal: durum 5, `ClosedAtUtc` = kilit bitişi, olaylar `RecordOpened`, `RecordTimedOut`, ret "Kayıt zaman aşımına düştü." |
| K20 | SC-16: yarıda kapatma, başlamamış adımdan çıkarma, pasifleştirme; sahip değişmez | GEÇTİ | T6 K20: P'nin kaydı yarıda kapatıldı (durum 7), diğer kaydın sahibi P'yi başlamamış adımdan çıkardı, P pasifleştirildi (302, IsActive 0); iki kaydın sahibi önce ve sonra aynı (23, 13) |
| K21 | İptal, yarıda kapatma ve yönetici mola bitirme olayları yapan, gerekçe ve zamanla; bütünlük müdahaleden sonra "Doğrulandı" | GEÇTİ | T6 K21: `RecordCancelled`, `RecordAborted`, `BreakEnded` yüklerinde gerekçe, aktör Yönetici; detay olay günlüğünde satırlar; 7 kayıtta rozet ve doğrulama ekranı "Doğrulandı" |
| K22 | F5-B1 sayaçlar art arda yenilemede aynı, SH-14 yorumu kodla aynı; koyu tema perdesi koyu, kontrast AA; F5-A1 bağlantıları | GEÇTİ | T6 K22: açık molada altı ardışık GET'te faz sayacı 1, 1, 1, 1, 1, 1 (`data-running=false`). `DurationCalculator.cs:173-174` tick farkı tek yuvarlama, `IDurationCalculator.cs:189-196` aynı. Perde koyu temada Run dialogu ve medya dialogu rgba(4,8,12,0.7), çekmece 0.6; dialog zemini rgb(23,30,37), metin rgb(228,234,239). F5-A1: pasifleştirme ekranında Yönetici'ye `/records/details/4`, Süper admin'de bağlantı yok |
| K23 | Çıkıştan sonra eski çerez girişe yönlenir; aynı kullanıcının diğer oturumu etkilenmez; yeniden başlatma sonrası not | GEÇTİ | `YetkiMatrisi.txt` K23: aynı kullanıcı iki oturum; A çıkışından sonra A'nın eski çereziyle `/my-work` ve `/records` 302 `/account/login?...reason=signed-out`; B 200. Yeniden başlatma notu: süreç yeniden başlayınca A'nın çıkış yapılmış eski çerezi yeniden 200 alıyor (iptal listesi bellekte, PL-71 sınırı); backend devir notundaki "yeniden başlatma sonrası eski çerez yine girişe yönlendi" gözlemi bu ortamda tekrarlanmadı |
| G1 | `-warnaserror` 0/0; `build:css` | GEÇTİ | Bölüm 3 |
| G2 | §4 grep 0; öznitelikler; rol metni | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi, §4, konsol, DNA | GEÇTİ | Ön tur 1 bulguları düzeltildi; ön tur 2 ve teslimde 198 mekanik eval, 128 G12 hücresi temiz; konsol 0; §4 0 |
| G4 | 360'ta kaydırma yok, birincil 44 px, durum metinle | GEÇTİ | 360 ve 320'de `hScroll` false; "Müdahaleyi kaydet" ve "Molayı bitir" 56 px; sürüm bağlantısı 44 px; rozetler ikon + metin |
| G5 | Belirteçsiz POST 400 | GEÇTİ | Yönetici oturumunda iki müdahale adresi belirteçsiz 400 |
| G5a | Her POST formu kendi belirtecini taşır | GEÇTİ | Faz 6 sayfalarında 59 POST formu, belirteçsiz 0 |
| G5b | `bool` onay kutuları | GEÇTİ | Faz 6 formlarında onay kutusu yok (`mineOnly` iki seçenekli select); gizli `false` grep 0 |
| G6 | em/en-dash 0 | GEÇTİ | `Src` ve kanıt dosyaları 0; eval `emdash` false |
| G7 | `Up()` Drop yok | GEÇTİ | Faz 6'da yeni migration yok |
| G8 | Controller ve görünümde süre aritmetiği yok | GEÇTİ | `Controllers/`, `Views/` 0 eşleşme; Faz 6 servislerinde `TimeSpan`/zaman çıkarması 0 |
| G9 | Aynı CommandId tek etki | GEÇTİ | İptal iki gönderim: 1 `RecordCancelled`, 1 komut satırı; Yönetici mola bitirme iki gönderim: olay +1 |
| G10 | Süper admin yeni adreslerde 403, menüde yok | GEÇTİ | Dokuz adres 302>403; menüde `/records` ve `/interventions` yok |
| G11 | Yerleşim imzası | GEÇTİ | `MekanikEval.txt`: `/records` 360 `Makine:2@1 KayıtNo:1@2 Sahip:1@2 Durum:1@3 Açılış:1@3 Net:1@4 İşlem:1@4`, 768/1024 `KayıtNo:1@1 Makine:2@1 Sahip:1@1 Durum:2@2 Açılış:1@2 Net:1@2 İşlem:4@3`, 1440 tablo (satır 69-87 px); detay Molalar, Malzemeler, Olay günlüğü ve bütünlük iki tablosu devir notundaki beklenen imzalarla aynı, boşluk yok; Faz 1-4 dokuz tablo 72/72 aynı |
| G12 | Hizalama kusuru 0 | GEÇTİ | `G12Hizalama.txt`: ön tur 2'de 128 hücre kusur 0 (Faz 6 ekranları, Run, İşlerim, yeni kayıt, pano, pasifleştirme, sürüm geçmişi) |

Kabul özeti: Faz 6 kriterleri (K1-K8, K11, K13-K23; K9, K10, K12 Faz 7'de) 20/20; ortak kriterler (G1-G12, G5a, G5b) 14/14; toplam 34/34.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental -o .qa/f6/bchk` ve `.qa/f6/bchk2` (düzeltme sonrası): 0 Uyarı, 0 Hata; `dotnet publish -c Release` başarılı |
| CSS | `npm run build:css` | QA çıktısı (`.qa/f6/app.check2.css`) `wwwroot/css/app.css` ile bayt bayt aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme; sihirli rol metni 0; `InterventionsController` `[Authorize(Roles = Roles.Manager)]`, `RecordsController` sınıf düzeyi `RecordReaders`, eski action'lar `Roles.Operator`; özniteliksiz iki dosya controller değil. MediatR, AutoMapper, generic repository, `new ...Service(` 0; `GetService` Program.cs dışında 0; `Migrate()` 0. Controller'larda iş kuralı yok |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: `InterventionService`, `RecordIntegrity`, `RecordStateAudit`, `StateMismatchCollector`, `EventCanonical`, `SessionRevocations`, `InterventionType.Cancel/Abort`, `ViewModels/Interventions`; §8 taraması: `#region`, süs yorum, emoji, kalıp dil, `Helper/Utils/Manager` 0; satır içi yorumlar "neden" anlatıyor. `RecordDetailReader` 293, `InterventionService` 228 satır (200 üstü, bölünmeye aday; bulgu sayılmadı) |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (tam tablo) |
| Mekanik eval | `UiDelivery.md §3` | Ön tur 1: 360 hücre, iki bulgu (bölüm 6 notu). Ön tur 2 ve teslim: 198 hücre temiz; yalnız `label.choice` içindeki tür radyoları 20x20 (kabul). En düşük kontrast 5,28:1, üst bar 64 px, konsol 0 |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Faz 6'da migration yok; DB-2026-008 indekste TALEP |

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Denetçi (3001); Editor = Operatör (op11 kaydın sahibi; op2 yabancı aynı sonuç); Admin = Süper admin (0001); Yönetici (1001) parantez içinde. Tam tablo `Evidence/Faz-6/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/` | 302 (giriş) | 302 `/records` (Yönetici 302 `/records`) | 302 `/my-work` | 302 `/users` |
| GET `/records` | 302 (giriş) | 200 (Yönetici 200) | 200 (varsayılan kendi kayıtları) | 302>403 |
| GET `/records/details/{id}`, `/records/details/99999` | 302 (giriş) | 200, 404 (Yönetici 200, 404) | 200, 404 | 302>403 |
| GET `/records/integrity/{id}` | 302 (giriş) | 200 (Yönetici 200) | 302>403 | 302>403 |
| GET `/interventions/create?recordId=`, `/interventions/end-break/{id}` | 302 (giriş) | 302>403 (Yönetici 200; olmayan mola 404) | 302>403 | 302>403 |
| POST `/interventions/create?recordId=` | 302 (giriş) | 302>403 (Yönetici: belirteçli 200 nedenli ret, belirteçsiz 400) | 302>403 | 302>403 |
| POST `/interventions/end-break/{id}` | 302 (giriş) | 302>403 (Yönetici: 404, belirteçsiz 400) | 302>403 | 302>403 |
| GET `/prototype/run`, `/prototype/my-work` | 404 | 404 | 404 | 404 |

Menüde gizli olup adresle açılan sayfa yok.

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/records` (Yönetici, Operatör varsayılanı, lot araması, sayfa 2, iki boş durum) | Kart 2 sütun, imza beklenen | Kart 4 sütun | Kart 4 sütun | Tablo, satır 87 px | Ana 1600 ile sınırlı | Temiz | Temiz | 320x640 ve 640x400 temiz | Süzgeç alanları sırayla, halkalı |
| `/records/details/{id}` (tamamlandı, açık mola, iptal, yarıda kapatma, ihlal) | Ray altta; Zaman hücresi düzeltildi (önce taşıyordu) | Ray altta | Yan yana | Yan yana | 1132/384 | Temiz | Temiz | Temiz | Yazdır, sürüm bağlantısı, medya; halkalı |
| Yazdırma (Yönetici, Denetçi, Operatör) | - | 718 px temiz | - | - | - | Koyu temada açık çıktı | - | - | Yazdır düğmesi `window.print` |
| `/records/integrity/{id}` (Doğrulandı, ihlal) | Temiz | Temiz | Temiz | Tablo | Temiz | Temiz | Temiz | Temiz | Halkalı |
| `/interventions/create`, `/interventions/end-break/{id}` (Açıldı, Devam ediyor, engelli) | Temiz | Temiz | Yan yana | Yan yana | 1196/320 | Temiz | Temiz | Temiz | Tür, gerekçe, PIN, göz; boş gönderimde üç alan hatası, odak türde |
| Faz 1-5 gerilemesi (13 sayfa, Run, İşlerim, yeni kayıt, mola ekranı, pano) | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | Ölçülmedi | Ölçülmedi | Ölçülmedi |

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Olay günlüğü zamanı mono, süreler mono rakam ve sans birim.
- Palet: UYGUN. Yeni ham renk yok; tek yeni token `--scrim` (iki temada koyu); ihlal satırı `danger-soft`; en düşük kontrast 5,28:1.
- Köşe ölçeği: UYGUN. Panel, tablo, form 2 px.
- Gezinme arketipi: UYGUN. "Kayıtlar" Yönetici ve Denetçi'de İzleme grubunda, Operatör'de Makine panosundan sonra; Süper admin'de yok.
- Sayfa iskeleti: UYGUN. Detay ve müdahale formu ana kolon + bağlam rayı, lg altında ray altta; liste filtre paneli + tek tablo.
- Veri dili: UYGUN. 1280 altında satır kartı, imzalar beklenen; rozetler ikon + metin (bütünlük `integrity.*`); boş durumlar ne olduğunu ve nasıl dolduğunu söylüyor.
- Hareket imzası: UYGUN. Yeni hareket yok; reduced-motion hücreleri temiz.
- İmza bileşen: UYGUN. Değişmedi; F5-B2 perdesi düzeltildi.
- Anti-slop: temiz. Gradient, glow, eyebrow, sürüm rozeti, emoji, em-dash, CDN yok.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | ORTA | Detay ve denetim çıktısı görevli satırında gerçekleşmemiş bir "onay" zamanı gösteriyor. Görevlinin kendi PIN'iyle onayı Faz 7'de (PL-57, K-10); `StepParticipants.ConfirmedAtUtc` kayıt 11'in 6 görevli satırında NULL, ama `RecordPhaseReader` NULL'da eklenme olayının zamanını (sahip için açılış anını) `ConfirmedAtLocal` olarak veriyor ve görünüm bunu "onay 15.11.2026 17:54" diye yazıyor. Yazdırılan denetim çıktısında görevlinin PIN'le onay verdiği izlenimi doğuyor (Brief §4.3 soru 4 "PIN onay zamanları") | `Evidence/Faz-6/KabulKriterleri.txt` "K1 R1 detay metni" ("Ekleyen QA Operatör 11, onay 15.11.2026 17:54") ve DB notu; `Services/RecordPhaseReader.cs:117`, `Views/Records/Details.cshtml:264` | backend-engineer, frontend-engineer | `ConfirmedAtLocal`'ı yalnız `ConfirmedAtUtc` doluysa doldurmak (nullable), eklenme anını ayrı alan olarak vermek (ör. `AddedAtLocal`); görünümde "eklendi <an>" yazıp onay yoksa "onay yok" ya da hiç yazmamak. Sözleşme alanı değişeceği için analist kararı | KAPANDI (tur 2): onaysız görevlide "onay" metni yok; "Ekleyen X, eklendi <an>" eklenme olayının sunucu anıyla, açılıştaki sahipte açılış anıyla; detay ve yazdırma aynı (Tur 2, T2.1) |

Ön tur 1 bulguları (raporlanmadı, ön turda düzeltildi ve yeniden ölçüldü): "Versiyon içeriğini aç" 20 px dokunma hedefi (YÜKSEK, `Details.cshtml:75`) -> 44 px; olay günlüğü Zaman hücresi 320-390 px'te 37,6 / 17,6 / 2,6 px taşma (ORTA, `Details.cshtml:469`, `Integrity.cshtml:121`) -> taşma 0 (`MekanikEval.txt`, `OnTur-Zaman-360-Once.png`).

## 7. Geçenler

- 13 soru, iki görevli ve molalı gerçek kayıtta yanıtlı; süreler ve efor DB zamanlarıyla elle aynı; v2/v3 ve makine adı değişikliğinde açılış anı değerleri korunuyor.
- Liste: 12 süzgeç ve arama bileşimi DB sorgularıyla birebir; Operatör varsayılanı sahip ve görevli kümesi; sayfalama sunucuda.
- Bütünlük: değişmemiş ve müdahaleli kayıtlarda "Doğrulandı"; iş emri, lot no, adımı başlatan kişi ve sahte olay ayrı ayrı doğru alan, değer ve ilk tutarsız sırayla gösteriliyor.
- Müdahaleler: iptal (Açıldı, devam eden adımlı, açık molalı), yarıda kapatma, yönetici mola bitirme tek işlemde; makine kilidi kalkıyor; kayıt silinmiyor, no yeniden kullanılmıyor; ret yolları olay bırakmıyor; G9 tek etki; SC-16 uçtan uca sahip değişmeden.
- Yetki matrisi, `/` yönlendirmeleri, pano bağlantıları, prototip 404, oturum iptali (iki oturum), F5-B1, F5-B2, F5-A1.
- Yazdırma çıktısı üç rolde denetim çıktısı içeriğini taşıyor, A4 taşmasız.
- Ön tur iki bulgusu aynı oturumda düzeltildi; `_Moment` değişikliği sonrası Faz 4-5 ekranlarında gerileme yok.

## 8. Kapsam dışı / yapılamayan

- K7 "olay değeri değiştirilmiş kopya": olay yükü ve adım zamanı UPDATE'leri kopyada tetikleyicilerle reddedildi ("Olay günlüğü değiştirilemez.", "Başlamış ya da bitmiş adımın zamanı değiştirilemez."); tetikleyiciyi kaldırma (DROP TRIGGER) proje kancası tarafından engellendi, atlatılmadı. İlk tutarsız olay sırası sahte olay eklenmiş kopyada ölçüldü (sıra 17).
- Production anahtarı süreç başına üretildiği için 5601 yeniden başlatılınca önceki süreçte yazılmış kayıtlar (1-10) "Bütünlük ihlali" gösterir; bu QA ortamının sonucudur (gerçek kurulumda anahtar sabit). Bütünlük kanıtları aynı süreçte yazılan kayıtlarla ve Development kopyalarıyla alındı.
- "Hiç kayıt yok" boş durumu boş veritabanı gerektirdiği için denenmedi; diğer iki boş durum ölçüldü.
- Adım sürerken açık molalı iptal ve beklemedeki kayıt Faz 7 (PL-57, PL-46).
- Gerçek yazıcı önizlemesindeki sayfa sonu bölünmesi görsel olarak incelenmedi (PDF üretildi, sayfa sayısı ölçüldü).
- Chromium dışı tarayıcı, dokunmatik cihaz ve ekran okuyucu ile tur yapılmadı.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | K23 kalıcı oturum iptali: yeniden başlatmadan sonra çıkış yapılmış eski çerez yine geçerli (ölçüldü) | analyst | PL-71: kalıcı çözüm ALTER, Salı sonrası kullanıcı onayıyla |
| A2 | Prototip silmeleri indekste değil; commit'te silmelerin de eklenmesi | koordinatör | K13 "repoda yoktur" commit'le tamamlanır |
| A3 | DB-2026-008 index'leri | backend-engineer, dbadmin | Kullanıcı onayı bekliyor; kod index'e bağlı değil |

## 10. Analist kararı

```
FAZ 6: İADE
Tarih: 2026-11-16
Gerekçe: 34/34 kriter kanıtla eşleşti, KRİTİK ve YÜKSEK 0; ancak B1 ORTA açık ve ORTA aynı fazda kapanır (Workflow.md §3). Detay ve
         denetim çıktısı, görevli PIN onayı Faz 7'ye kadar olmadığı halde (PL-57) eklenme anını "onay" olarak gösteriyor; denetim
         belgesinde olmamış bir onay yazılamaz.
İade maddeleri: B1 backend-engineer (RecordPhaseReader) ve frontend-engineer (Details.cshtml); sözleşme PL-72
Yeniden denetim şartı: aşağıdaki "Tur 2 yeniden denetim" listesi
```

Sözleşme (PL-72): `RecordDetailParticipantViewModel.ConfirmedAtLocal` -> `DateTime?`, yalnız gerçek onay anı (Faz 5-6'da hep boş); yeni
`AddedAtLocal: DateTime` (eklenme olayının anı, açılıştaki sahip için açılış anı, PL-69). Görünüm "Ekleyen <ad>, eklendi <an>" yazar, onay doluysa
ayrıca "onay <an>".

Tur 2 yeniden denetim (qa-tester, aynı rapora "Tur 2" bölümü; önce yerleşim ön turu, O-06):

- B1: Görevli eklenmiş tamamlanmış ve devam eden kayıtta `/records/details/{id}` ve yazdırma önizlemesinde onaysız görevlide "onay" metni ve onay
  anı yok; eklenme anı veritabanındaki eklenme olayının sunucu zamanıyla (Türkiye saati), açılıştaki sahipte açılış anıyla aynı; ekleyen adı doğru.
- Bütünlük: rozet "Doğrulandı" kalır; K7 durum ve olay karşılaştırması (`/records/integrity/{id}`) değişmez, görevli alanlarında tutarsızlık yok.
- Gerileme: K1 (13 soru), K14 (denetim çıktısı içeriği), G11 ve G12 detay ve yazdırma görünümünde, G5a, G1, G2, §4 grep, konsol 0.

Açık kalemler hakkında: A1 PL-71'de yazılı bilinen sınırdır; backend devir notundaki "yeniden başlatmada da geçerli" ifadesi yanlıştır, QA
ölçümü doğrudur. Kalıcı çözüm (kullanıcı satırında oturum sürümü) ALTER'dir; Salı sonrası kullanıcıya sorulur, PhasePlan Faz 7 kalem 9 (PL-72).
A2 koordinatörün commit işidir.

### Tur 2 kararı

```
FAZ 6: TESLİM ALINDI
Tarih: 2026-11-16
Gerekçe: B1 tur 2 şartının her maddesiyle ölçülüp kapandı (onaysız görevlide onay metni yok, eklenme anı olaydan ve açılıştan doğru,
         bütünlük ve K7 değişmedi, K1 ve K14 yazdırma tam, G11, G12, G5a); 34/34 kriter tur 1-2 kanıtıyla eşleşti; açık bulgu 0;
         QA önerisi GEÇTİ.
Açık kalemler: A1 bilinen sınır, Faz 7 kalem 9 (PL-71, PL-72); A2 prototip silmeleri faz commit'ine girer (koordinatör, K13).
```

Analistin doğruladıkları (kanıt dışında): `Views/Records/Details.cshtml:264` "Ekleyen <ad>, eklendi <an>" yazıyor, "onay" yalnız
`ConfirmedAtLocal` doluyken çiziliyor.

## Tur 2 (2026-11-16)

Kapsam: analistin tur 2 yeniden denetim listesi (PL-72) ve DB-2026-008 sonrası K5 gerilemesi. Release `.qa/f6/build3` (`-warnaserror` 0 uyarı 0 hata; CSS QA çıktısı `wwwroot/css/app.css` ile bayt bayt aynı). Yeni kopyalar `App_Data/vaka.db` (migration `20261115152212_AddCleaningRecordListIndexes` uygulanmış, iki index mevcut) + `DemoData.sql`; 5601 Production, 5603 ve 5604 Development. Önce kısa ön tur (detay, yazdırma, liste, bütünlük, müdahale formu, Run, İşlerim) yapıldı, temiz çıktı. Kanıt `Evidence/Faz-6/Tur2.txt`.

| # | Madde | Sonuç | Kanıt |
|---|---|---|---|
| T2.1 | B1: tamamlanmış (kayıt 3) ve devam eden (kayıt 4) kayıtta detay ve yazdırma önizlemesinde onaysız görevlide "onay" metni ve onay anı yok; eklenme anı eklenme olayının sunucu zamanı (TR), açılıştaki sahipte açılış anı; ekleyen adı doğru | GEÇTİ | `Tur2.txt` B1: 13 görevli satırının hepsi beklenen "Ekleyen <ad>, eklendi <an>" metniyle aynı (beklenen DB'deki `ParticipantAdded` olayından ve açılış anından üretildi); 18:27'de eklenen görevli `datetime="2026-11-15T18:27"` (olay 15:27:04 UTC), sahipler açılış 18:25; `ConfirmedAtUtc` hepsinde NULL. Detay HTML'inde "onay" yalnız kabuğun gizli çevrimdışı şeridinde ve SKT gerekçesinde ("depo onayıyla"); yazdırmada görevli satırlarında "onay" 0, "eklendi" 12/10/4 |
| T2.2 | Bütünlük rozeti "Doğrulandı"; K7 durum ve olay karşılaştırması değişmez; görevli alanlarında tutarsızlık yok | GEÇTİ | Dört kayıtta (tamamlandı, devam ediyor, açıldı, iptal) "Doğrulandı", değişen alan bölümü yok. K7 (yeni Development kopyası, 5604): kontrol "Doğrulandı"; iş emri, lot no, adımı başlatan kişi ve sahte olay (ilk tutarsız sıra 17) sonuçları tur 1 ile aynı; olay yükü ve adım zamanı UPDATE'i tetikleyiciyle reddediliyor |
| T2.3 | K1 (13 soru) ve K14 (denetim çıktısı) gerilemesi | GEÇTİ | Yazdırma (Yönetici, Denetçi, Operatör): menü, üst bar, alt çubuk gizli, görünür düğme 0, taşma yok; denetim çıktısı, oluşturan ve an, "Doğrulandı", doküman no PRS-TM-014 Rev.3, SKT işareti ve gerekçesi, süre altı gerekçesi, kapanış bilgisi (iptal kaydı), 13 sorunun anahtar alanları eksiksiz; A4 PDF 4-5 sayfa |
| T2.4 | G11 ve G12 detay ve yazdırma görünümünde | GEÇTİ | G11: `/records`, detayın üç tablosu ve bütünlük tablosu 360/768/1024/1440'ta tur 1 ve beklenen imzalarla aynı, boşluk yok. G12: 64 hücre kusur 0 |
| T2.5 | G5a, G1, G2, §4 grep, konsol | GEÇTİ | 40 POST formu, belirteçsiz 0; derleme 0/0; policy, sihirli rol, G8, gizli `false` 0; §4 tam tablo 0; mekanik eval 80 hücre, konsol 0 (yalnız `label.choice` içindeki tür radyoları, tur 1 kabulü) |
| T2.6 | K5 gerilemesi (DB-2026-008 index'leri) | GEÇTİ | 14 süzgeç/arama bileşimi DB sorgularıyla hem küme hem sıra (`OpenedAtUtc desc, Id desc`) olarak eşit; Operatör varsayılanı sahip ve görevli kümesi; sayfalama ayrık. Migration `Up()` yalnız iki `CreateIndex`, `Drop` yok; DB-2026-008 kaydında kullanıcı onayı (O-07) yazılı |

Bulgu tablosu güncellemesi: B1 KAPANDI (tur 2). Yeni bulgu yok.

Kabul özeti (tur 2): yeniden denetim maddeleri 6/6; toplam 34/34 (tur 1 kriterleri değişmedi). Açık kalemler: A1 PL-71/PL-72 bilinen sınır (Faz 7 kalem 9), A2 koordinatörün commit işi (prototip silmelerinin indekse eklenmesi), A3 kapandı (DB-2026-008 uygulandı, LotNoSnapshot index'i gerekçeyle atlandı).

QA önerisi (tur 2): GEÇTİ.

Kendi süreçleri (5601, 5603, 5604) ve playwright oturumları kapatıldı; sunucu günlüklerinde fail/crit yok; 5080 ve 5181'e dokunulmadı; `App_Data/vaka.db`'ye yazılmadı.

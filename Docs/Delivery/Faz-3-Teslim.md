# Faz 3 Teslim Raporu: Temizlik tipi ve prosedür versiyonu

| Alan | Değer |
|---|---|
| Faz | 3 |
| Tarih | 2026-11-15 |
| Tur | 3 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `08c793d`; Faz 3 kodu, migration `20261114205904_AddCleaningTypesAndProcedureVersions`, `DemoData.sql` Faz 3 bölümü commit'lenmemiş). Devir notları `Docs/Handover/backend-engineer-20261115-0600.md`, `Docs/Handover/frontend-engineer-20261115-0500.md` ("Ön tur düzeltmeleri" dahil); `DB-2026-005` |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/f3/build`. Port 5301 Production, DB `.qa/f3/qa.db` (`App_Data/vaka.db` SQLite yedekleme API'siyle salt okunur kopyası, üzerine uygulama yoluyla QA verisi), medya kökü `.qa/f3/media`. Port 5302 Production, `.qa/f3/empty.db` (yalnız migration). `.qa/f3/fresh.db` sıfırdan `dotnet ef database update` + `DemoData.sql` (K12, K15). Video ileri sarma Google Chrome ile, diğer tarayıcı ölçümleri Chromium (playwright-cli) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 3: B3 ve B4 kapandı, açık bulgu yok; tur 1 ve tur 2: KALDI) |
| Analist kararı | TESLİM ALINDI (tur 3, kullanıcı onaylı, 2026-11-15); tur 1 ve tur 2: İADE |

## 1. Kapsam

- DB: `CleaningTypes`, `MachineCleaningTypes`, `ProcedureVersions`, `ProcedurePhases`, `ProcedureSteps`, `StepMedia`; 11 değişmezlik tetikleyicisi; ayar tablosuna `MaxLockMinutes` 120, `MaxPhotoMb` 10, `MaxVideoMb` 100; demo veride PLN (planlı) ve ACL (acil) tipleri, ikisinin Yürürlükte v1'i (DB-2026-005).
- BE: tip servisi (kod, tür kilidi, kilit süresi sınırı ayardan, acil tipte süre ve eşleme yok), makine eşleme (seçilemez makine kuralları), versiyon servisi (taslak, derin kopya, faz ve adım düzenleme, taşıma, yayın engelleri, PIN'li yayın), medya (imza denetimi, ayardan boyut sınırı, SHA-256 adlı tek kopya, `/media/{id}` aralıklı okuma, 413), `IDurationCalculator` yedi yeni metot.
- FE: E-09 tipler, tip formu ve makine eşleme; E-10 tip detayı ve versiyon tablosu; E-11 taslak düzenleyici (`ProcedureEditor.js`, rayda yayın paneli, PL-53); E-12 versiyon görüntüleme ve `MediaViewer.js`; FE 9: `_PinInput` ve parola göz düğmesi, G12 taraması, PL-55 kart modunda İşlem hücresi ve satır hover'ı (Faz 1-3 liste ve ray tabloları).

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Planlı tip ve kilit süresi sınırı (120 kabul, 121 red, ayar değişince sınır uyar); acil tipte süre alanı ve eşleme bağlantısı yok, gönderilen süre red | GEÇTİ | `Evidence/Faz-3/KabulKriterleri.txt` K1: 120 302, 121 "Varsayılan kilit süresi 1 ile 120 dakika arasında olmalı.", boş ve 0 red, tür seçilmeden red; ayar 60 yapılınca 61 red, 60 kabul, form ipucu ve `max` 60; ayar 120'ye döndü. Acil + 30 "Acil tipte kilit süresi girilmez."; acil düzenleme formunda `DefaultLockMinutes` yok, detayda eşleme bağlantısı yok; acil düzenlemede gönderilen süre red, DB değişmedi |
| K2 | Kaydedilmiş tipin türü değiştirilemez | GEÇTİ | `KabulKriterleri.txt` K2: planlıya `Kind=2`, acile `Kind=1` "Tür kayıttan sonra değiştirilemez.", DB'de tür aynı; kod değişikliği serbest (PL-51) |
| K3 | Seçilemez makineler nedeniyle; istekle gelen seçilemez makine eşlenmez; eşli seçilemez korunur; acil tip eşleme yazmaz | GEÇTİ | `KabulKriterleri.txt` K3: "Makine kullanımdan kaldırılmış." (M02), "Hat pasif." (MQ2 eşli, MQ3 eşsiz), "Tesis pasif." (M01 Depo asansörü eşli, MQ1); eşli ve seçilemez ikisi işaretli ve `disabled`. POST 1,2 + seçilemez eşsiz 8 ve 4: sonuç 1,2,5,7 (5 ve 7 korundu, 8 ve 4 eşlenmedi). Acil `/cleaning-types/machines/2` GET ve POST 302 tip detayı, "Acil tipe makine eşlenmez.", eşleme satırı 0 |
| K4 | Taslakta faz ve adım ekleme, taşıma, çıkarma; sıra no boşluksuz; yayın engelleri | KALDI | Sunucu kuralları geçerli belirteç elle eklenmiş isteklerle doğru (`KabulKriterleri.txt` K4: üç engel metni, sınırda taşıma sıra değiştirmiyor, faz çıkarma adımları da çıkarıyor, sıra no'lar 1'den boşluksuz). Arayüzden yapılamıyor: düzenleyicinin 48 POST formunun hiçbirinde antiforgery belirteci yok, her işlem 400 (B1, `Duzenleyici.txt` 1-3) |
| K5 | Yanlış PIN yayınlamaz ve deneme sayar; doğru PIN'le Yürürlükte, önceki Arşiv, versiyon no +1 | KALDI | Sunucu: yanlış PIN "PIN hatalı." ve sayaç 0 > 1, boş PIN "PIN girin." sayaç değişmez; doğru PIN 302 "v2 yayınlandı.", v1 Arşiv (`PublishedAtUtc` korundu), v2 Yürürlükte, yayınlayan 1001, sayaç 0 (`KabulKriterleri.txt` K5). Arayüzde yayın formu belirteçsiz, tarayıcıdan gönderim 400 (B1) |
| K6 | Yürürlükteyken yeni taslak; derin kopya; ikinci taslak reddi | KALDI | Sunucu: v3 taslağı v2'nin 3 faz, 6 adım, 8 medyasını taşıdı (içerik imzası aynı), malzeme zorunluluğu ve doküman no kopyalandı; ikinci istek "Bu tipin taslağı zaten var." (`KabulKriterleri.txt` K6). Arayüzde "Yeni taslak aç" formu belirteçsiz, tıklama 400 "Bir hata oluştu" (B1, `Duzenleyici.txt` 2) |
| K7 | Tetikleyiciler; düzenleme rotaları yayınlanmışta red, veri değişmez | GEÇTİ | `KabulKriterleri.txt` K7: kopya DB'de yayınlanmış faz, adım, medya UPDATE/DELETE/INSERT ve adımı taslak fazına taşıma "Yayınlanmış versiyon değiştirilemez."; versiyon `MaterialRequired`, `DocumentRef`, Arşiv > Yürürlükte ve DELETE red; Yürürlükte > Arşiv kabul; taslak serbest. 12 düzenleme rotası ve GET edit 302 versiyon görüntüleme, ileti aynı, v2 içeriği değişmedi |
| K8 | JPEG, PNG, WebP, MP4 kabul; GIF ve sahte uzantı red; 10 485 760 / 104 857 600 sınırları; ayar uyar; tek kopya; video ileri sarılır, 206 | KALDI | Sunucu ve sunum tamamen doğru (`Medya.txt`): dört biçim 302; GIF "Dosya biçimi desteklenmiyor..."; GIF içerikli `.png`, PNG içerikli `.jpg`, JPEG içerikli `.mp4` "Dosya içeriği uzantısıyla uyuşmuyor."; PNG ve JPEG 10 485 760 kabul, 10 485 761 "Fotoğraf en çok 10 MB olabilir."; MP4 104 857 600 kabul, +1 "Video en çok 100 MB olabilir."; ayar 1 MB yapılınca 1 383 556 baytlık JPEG red, düzenleyici sınırı 1; aynı içerik iki adımda diskte tek dosya; 106 MB istek 413 uygulama hata sayfası; Range 206 `Accept-Ranges: bytes`; Chrome'da 5,97 sn videoda 4. saniyeye atlama `seeked`, `currentTime` 4. Arayüzde yükleme formu belirteçsiz, tarayıcıdan yükleme 400 (B1) |
| K9 | `/media/{id}` oturumsuz giriş sayfasına | GEÇTİ | `Medya.txt` ve `YetkiMatrisi.txt`: 302 `/account/login?ReturnUrl=%2Fmedia%2F2` |
| K10 | Denetçi versiyonu açar, düzenlemede 403, taslakta 404; Operatör `/media` dışında 403 | GEÇTİ | `YetkiMatrisi.txt`: Denetçi `/procedure-versions/details/3` 200, `/details/5` (taslak) 404, `/procedure-versions/edit/5` ve 16 POST 302 > 403; Operatör 6 GET ve 16 POST 403, yayınlanmış medya 200, taslak medyası 404 |
| K11 | Düzenleyici 1024'te klavyeyle; fetch sonrası odak; JS kapalı tam sayfa | KALDI | JS kapalı tam sayfa ve JS açık fetch işlemlerinin hepsi 400 (B1, `Duzenleyici.txt` 2 ve 3A). Belirteç elle eklenince davranış doğru: taşıma düğmesi Shift+Tab ile odaklanıyor, halka 3 px; Enter sonrası odak aynı yön düğmesinde, sayfa yenilenmiyor; sunucu hatasında `role=alert` kutusu ilgili formda, görünür ve odakta (`Duzenleyici.txt` 3) |
| K12 | Ayar tablosunda altı satır; Faz 1 satırları ve geçmiş değişmez; migration'da UPDATE yok | GEÇTİ | `KabulKriterleri.txt` K12: sıfırdan migration sonrası 6 satır (4 `MaxLockMinutes` 120 dakika, 5 `MaxPhotoMb` 10 MB, 6 `MaxVideoMb` 100 MB), `SettingChanges` 0; `Up()` içinde `InsertData` 1, `UpdateData`/`DeleteData`/`DropTable`/`AlterColumn` 0 |
| K13 | Pasifleştirme ve yeniden etkinleştirme; "Pasif" rozeti | GEÇTİ | `KabulKriterleri.txt` K13: yalnız gizli `IsActive=false` ile 302, DB 0; varsayılan listede yok, `showInactive` satırında "Pasif"; yeniden etkinleştirme 302, DB 1 |
| K14 | E-09 G11 imzası, E-10 PL-39, E-11/E-12 form-detay kuralı, 360/390 kaydırma yok | GEÇTİ | `MekanikEval.txt` G11: 123 tablo ölçümü beklenen imzayla aynı; iskelet 360'ta ray altta, 1024 ve 1440'ta yan yana (ana 356/756, ray 352 px); 360 ve 390'da `hScroll` false |
| K15 | Demo veride planlı ve acil tip, Yürürlükte v1; eşleme; "Kurulum verisi"; ikinci uygulamada sayı değişmez | GEÇTİ | `KabulKriterleri.txt` K15: PLN ve ACL Yürürlükte v1 (yayınlayan NULL), PLN eşlemesi 1,2,3,5 = aktif ve kaldırılmamış makineler, ACL 0; üçüncü uygulamada 12 tablonun satır sayısı aynı; versiyon görüntüleme ve tip detayında "Kurulum verisi" |
| K16 | PIN göz düğmesi altı ekranda; Faz 1-3 G12 kusur 0; Faz 1-2 G11 ve eval aynı; PL-55 imzaları | KALDI | Göz düğmesi 176 ölçümde kenarlık içinde, merkez farkı 0 (`G12Hizalama.txt` a); PL-55 imzaları ve Faz 1-2 eval tutuyor. Faz 3 düzenleyicisinde 1024 px'te G12 kusuru var (B2) |
| G1 | `-warnaserror` 0/0; `build:css` | GEÇTİ | Bölüm 3 |
| G2 | §4 grep'leri 0; öznitelikler; rol metni | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi kırık hücresiz, §4 0, konsol 0, DNA sapması yok | GEÇTİ | 394 teslim ölçümü ve 338 G12 hücresinde kırık yerleşim yok, konsol 0 (sayfa yüklemelerinde), §4 0, DNA bölüm 5. Düzenleyicinin işlevsizliği B1'de, hizalama kusuru G12 altında |
| G4 | Operatör ekranları 360'ta kaydırmasız, 44 px | GEÇTİ | Fazda Operatör ekranı yok; Faz 3 ekranlarında 40 px altı etkileşimli öğe yok (onay kutuları ve radyolar en az 56 px'lik `.choice` etiketi içinde) |
| G5 | Antiforgery'siz POST 400 | GEÇTİ | `YetkiMatrisi.txt`: 16 yazma rotası Yönetici oturumunda gövdede `Code` yok, `Code=T9`, `Code=200` ile 400; çok parçalı yükleme 400 |
| G6 | em/en-dash 0 | GEÇTİ | `Src/Vaka.Web` (cs, cshtml, js, css, sql, json) ve `Docs` 0; 394 ölçümde `emdash` false |
| G7 | `Up()` Drop yok; ALTER onayı | GEÇTİ | Bölüm 3 |
| G10 | Süper admin tüm Faz 3 adreslerinde 403, menüde yok | GEÇTİ | `YetkiMatrisi.txt`: 13 GET ve 16 POST 302 > 403; `/` ve `/users` sayfasında `/cleaning-types` bağlantısı yok |
| G11 | Liste ekranları ve ray tabloları beklenen imzayla | GEÇTİ | `MekanikEval.txt` G11: 9 tablo, 360/390/768/1024/1280/1440/1920 açık, koyu ve reduced-motion; 123 ölçüm aynı, 0 farklı |
| G12 | Yeni ve değişen ekranlarda hizalama kusuru 0 | KALDI | 338 hücrelik genel ölçüm kusur 0; hedefli ölçümde düzenleyici faz formlarında 24 px (B2) |

G8 ve G9 Faz 4 ve sonrası. Kabul özeti: K1-K16 10/16 (K4, K5, K6, K8, K11, K16 KALDI); ortak kriterler (G1-G7, G10-G12) 9/10; toplam 19/26.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental -o .qa/f3/bchk`: 0 Uyarı, 0 Hata; `dotnet publish -c Release` başarılı |
| CSS | `npm run build:css` | Tailwind QA çıktısı `wwwroot/css/app.css` ve yayındaki css ile bayt bayt aynı (`cmp`) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme (policy 0, sihirli `[Authorize(Roles = "` 0, `IsInRole("` 0). `CleaningTypesController` `Roles.Manager`; `ProcedureVersionsController` sınıfta `Roles.ManagerOrAuditor`, `Details` dışındaki 15 action'da ayrıca `Roles.Manager`; `MediaController` `Roles.RecordReaders`. Özniteliksiz iki dosya controller değil (`NoFormBindingAttribute.cs` filtre, `SessionControllerExtensions.cs`). MediatR, AutoMapper, generic repository, `new ...Service(`, `GetService<` 0; `Program.cs` içinde `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: tablolar `CleaningTypes`, `MachineCleaningTypes`, `ProcedureVersions`, `ProcedurePhases`, `ProcedureSteps`, `StepMedia`; kolonlar `DefaultLockMinutes`, `MinDurationSeconds`, `PublishedByUserId`, `Sha256`, `SizeBytes`; index `IX_ProcedureVersions_OneCurrent`, tetikleyici `TR_ProcedurePhases_NoUpdateWhenPublished`; dosyalar `ProcedureVersionsController.cs`, `ProcedureEditor.js`, `MediaViewer.js`; ViewModel'ler `...ViewModel` sonekli, sözleşmedeki adlarla |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (transition-all, outline-none, user-scalable, onclick, CDN, em/en-dash, h-screen, görünür `...`, jquery/gsap/aos, slate/gray/zinc, görünümlerde ham hex 0; tek `<img>` `_MediaList.cshtml:15` `alt`, `width`, `height`, `loading="lazy"`; `autocomplete` eksik metin girdisi 0; ikonlar `aria-hidden`; tek animasyon `live-pulse`, reduced-motion'da kapalı) |
| Mekanik eval | `UiDelivery.md §3` | 394 ölçüm temiz: h1 1, skip var, imgNoAlt/imgNoSize/inputNoLabel/iconBtnNoName 0, emdash yok, hScroll 0, taşma 0, dış kaynak 0, yazı tipi doğru, üst bar 64 px, mobil gövde 16 px, en düşük kontrast 5,28:1, reduced-motion animasyon 0, konsol 0 (`MekanikEval.txt`) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Üç migration; Faz 3'ün `AddCleaningTypesAndProcedureVersions` (commit'lenmemiş) `DbChanges.md:11` DB-2026-005 DOĞRULANDI, Eklemeli. `Up()` satır 14-280: 6 `CreateTable`, index'ler, 1 `InsertData`, tetikleyici SQL'i; `Drop*`, `UpdateData`, `DeleteData`, `AlterColumn` 0 (`DropTable` ve `DeleteData` yalnız `Down()`). `dotnet ef migrations has-pending-model-changes --configuration Release --no-build`: "No changes have been made to the model since the last migration." Sıfırdan uygulama başarılı (`fresh.db`) |

Ek denetimler:

- §7: yedi yeni metodun (`MinutesToSeconds`, `SecondsToMinutes`, `IsMinimumMissing`, `IsLockWithinLimit`, `IsMediaWithinLimit`, `GetMediaLimitBytes`,
  `GetUploadBodyLimitBytes`) formülü `IDurationCalculator.cs:53-91` XML özetinde, uygulama `DurationCalculator.cs:105-118` ile uyuşuyor (dakika x 60,
  tam bölme, 0 < değer <= sınır, MB x 1 048 576, max(foto, video) MB x 1 048 576 + zarf). Kodda MB sabiti yok; sınırlar her istekte ayardan
  (K1 ve K8 ayar değişikliği denemeleri). Uyumsuzluk 0.
- §8: Faz 3'te yeni ya da değişen 102 dosyada emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager` adı, TODO 0.
- SOLID: controller'lar bağlama, servis çağrısı ve görünüm/yönlendirme seçimi yapıyor; kurallar servislerde (`CleaningTypeService`, `MachineMappingService`,
  `ProcedureDraftEditor`, `ProcedureStepEditor`, `ProcedureVersionPublisher`, `StepMediaService`, `DraftRules`); arayüzlerle ve constructor'dan. En büyük
  dosyalar `CleaningTypeService.cs` 302, `ProcedureVersionsController.cs` 246, `ProcedureVersionReader.cs` 214, `ProcedureVersionPublisher.cs` 205 satır
  (§2'nin 200 satır eşiği; bölünme adayı, bulgu değil, backend devir notunda gerekçe var).

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Operatör (2001) ve Denetçi (3001), farklı olduklarında ayrı yazıldı; Editor = Yönetici (1001); Admin = Süper admin (0001, yalnız
`SuperAdmin`). Değer: ham soket yanıtı, `302>` yönlendirme hedefi. POST satırlarında Viewer ve Admin geçerli antiforgery belirteciyle; Editor kolonunda
belirteçsiz sonuç. Tam tablo `Evidence/Faz-3/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/cleaning-types`, `?kind=1&showInactive=true`, `/create`, `/edit/1`, `/machines/1`, `/details/1` | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/procedure-versions/edit/5` (taslak) | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/procedure-versions/details/3` (Yürürlükte) | 302 (giriş) | Operatör 302>403, Denetçi 200 | 200 | 302>403 |
| GET `/procedure-versions/details/5` (taslak) | 302 (giriş) | Operatör 302>403, Denetçi 404 | 302 > düzenleyici | 302>403 |
| GET `/procedure-versions/details/9999` | 302 (giriş) | Operatör 302>403, Denetçi 404 | 404 | 302>403 |
| GET `/media/1` (yayınlanmış) | 302 (giriş) | 200 | 200 | 302>403 |
| GET `/media/9` (taslak), `/media/9999` | 302 (giriş) | 404 | 200 (taslak), 404 | 302>403 |
| POST `/cleaning-types/create`, `/edit/1`, `/machines/1` | 302 (giriş) | 302>403 | 400 (belirteçsiz; `Code=T9`, `Code=200` ile de 400) | 302>403 |
| POST `/procedure-versions/create-draft`, `update-header`, `add-phase`, `update-phase`, `move-phase`, `remove-phase`, `add-step`, `update-step`, `move-step`, `remove-step`, `upload-media`, `remove-media`, `publish` | 302 (giriş) | 302>403 | 400 (belirteçsiz; çok parçalı yükleme de 400) | 302>403 |

Menüde gizli olup URL ile açılan sayfa yok: Operatör, Denetçi ve Süper admin menüsünde `/cleaning-types` yok; Yönetici menüsünde "Temizlik tipleri" var.
Reddedilen POST'lardan sonra tipler, eşlemeler, versiyonlar, fazlar, adımlar, medya ve PIN sayacı aynı.

## 5. Responsive ve görsel tur

Her hücrede §3 eval, AA kontrast, reduced-motion animasyon sayımı ve tablo/kart imzası (`.qa/f3/m3.tmpl.js`); ayrıca G12 hizalama ölçümü
(`.qa/f3/g12.tmpl.js`, 338 hücre, açık ve koyu) ve satır hover'ı. Ekran görüntüleri yalnız şüpheli hücrelerde ve DNA incelemesinde alındı ve
`Read` ile açıldı (`.qa/f3/*.png`).

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/cleaning-types` (dolu, süzgeçli, pasif dahil, uzun kod ve ad; boş kopya) | Kart, `İşlem:2@5`, boş yarım hücre PL-55 istisnası | Kart 4 sütun, `Versiyon:2@2 İşlem:4@3` | Kart 4 sütun | Tablo, satır en az 69 px | Tablo | Temiz | Animasyon 0 | Temiz (320, 640) | Atlama bağlantısı ilk odak, Enter ile `main`, 8 Tab halka 3 px |
| `/cleaning-types/details/{id}` (planlı, acil, prosedürsüz, uzun ad) | Ray altta, versiyon kartı PL-39 | Ray altta, `İşlem:3@2` | Ray yanında, PL-39 2 sütun | Tablo, Yayın sütunu kalan alan | Tablo | Temiz | Temiz | Temiz | Bağlantılar halkalı |
| `/cleaning-types/create`, `/edit/{id}` (planlı, acil), `/machines/{id}` | Tek sütun, ray altta | Temiz | Ray yanında | Temiz | Temiz | Temiz | Temiz | Temiz | Boş gönderim: üç alan hatası, odak `Code`; Acil seçilince süre alanı gizleniyor, JS kapalıyken "Yalnız planlı tipte" ipucuyla görünüyor |
| `/procedure-versions/edit/{id}` (taslak, medya, yayın paneli) | Ray altta | Temiz | Ray yanında; faz formlarında 24 px hizasızlık (B2) | Temiz | Temiz | Temiz (1024'te B2) | Temiz | Temiz | Taşıma düğmeleri odaklanabilir; işlemler 400 (B1) |
| `/procedure-versions/details/{id}` (Yönetici, Denetçi) | Ray altta | Temiz | Ray yanında | Temiz | Temiz | Temiz | Temiz | Temiz | Medya önizleme düğmeleri halkalı; diyalog Esc ile kapanıyor, odak küçük resme dönüyor |
| Gerileme: Faz 1-2 listeleri ve ray tabloları (PL-55) | Beklenen imza | Beklenen imza | Beklenen imza | Tablo | ölçülmedi | G12 koyu temiz | ölçülmedi | ölçülmedi | Faz 1-2 akışı |
| Gerileme: Faz 1-2 formları, `/account/*` (giriş, ilk giriş, güvenlik), `/users/edit` (sıfırlama, etkinleştirme), `/users/deactivate` | Temiz, göz düğmesi hizalı | Temiz | Temiz | Temiz | ölçülmedi | Temiz | ölçülmedi | ölçülmedi | Faz 1 akışı |

Dört durum: boş (`/cleaning-types` boş kopyada "Henüz temizlik tipi yok" + "Temizlik tipi ekle"; prosedürsüz tip "Bu tipin prosedürü yok"; boş taslakta
yayın engeli "En az bir faz gerekir"), hata (tip formu alan altı ve özet; düzenleyicide `Error.Target` bölgesi, `KabulKriterleri.txt` K4 "BÖLGE DOĞRU" 14/14 ve medya hataları 7/7 yükleme formunda; sınırdaki üç taşma isteği hatasız 302, sıra değişmiyor),
yükleniyor (`data-busy-form` formlarda, düzenleyicide `aria-busy` ve düğmeler devre dışı, `ProcedureEditor.js:60-64`), dolu (uzun kod 16, ad 120, iki
versiyon çipi, 8 medya). Konsol: sayfa yüklemelerinde hata ve uyarı 0; tek konsol hatası B1'in 400 yanıtı.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Başlık Archivo, gövde Hanken Grotesk, kod ve versiyon no JetBrains Mono; `fonts.check` 394 ölçümde true.
- Palet: UYGUN. Tek accent; "Acil" tehlike, "Planlı" nötr, "Yürürlükte"/"Taslak" çip, "Aktif" başarı tonu; AA altı metin 0 (en düşük 5,28:1).
- Köşe ölçeği: UYGUN. Panel, düğme, çip ve medya küçük resmi 2 px köşe, 2 px kenarlık, gölge ve gradient 0.
- Gezinme arketipi: UYGUN. Yönetici rayında "Tanımlar" altında "Temizlik tipleri"; Denetçi, Operatör ve Süper admin menüsünde yok.
- Sayfa iskeleti: UYGUN. Form, detay, düzenleyici ve versiyon görüntüleme ana kolon + 22rem ray (lg ve üstü yan yana, altında ray altta); yayın paneli rayda.
- Veri dili: UYGUN. Kart modunda İşlem son hücre, düğmeler tek satır ve sağa hizalı; hover tonu satırda, hücreler şeffaf; 1280 ve üstü tablo.
- Hareket imzası: UYGUN. Yeni animasyon yok; reduced-motion hücrelerinde animasyon 0.
- İmza bileşen: kapsam dışı (Faz 4-5).
- Anti-slop (`DesignDna.md §4`): temiz. Gradient, glow, cam, dört eşit KPI, jenerik karşılama, emoji, CDN yok; demo veri alana uygun.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | YÜKSEK | Taslak düzenleyici ve "Yeni taslak aç" tarayıcıdan kullanılamıyor: action özniteliği yazılı POST formlarında antiforgery belirteci üretilmiyor. `/procedure-versions/edit/{id}` sayfasındaki 48 formun (üst bilgi, faz ve adım ekle/düzenle/taşı/çıkar, medya yükle/çıkar, yayınla) ve tip detayındaki iki "Yeni taslak aç" formunun hiçbiri belirteç taşımıyor; genel antiforgery filtresi her gönderimi 400 ile reddediyor. JS açıkken fetch 400, formda "İşlem tamamlanamadı. Bağlantıyı kontrol edip tekrar deneyin."; JS kapalıyken "Bir hata oluştu" sayfası. K4, K5, K6, K8 ve K11 arayüzden karşılanmıyor. Sunucu kuralları belirteç elle eklendiğinde doğru çalışıyor; mühendislerin curl doğrulaması belirteci başka formdan aldığı için hatayı göstermedi | `Evidence/Faz-3/Duzenleyici.txt` 1-3, `Duzenleyici-400.png`; `Views/ProcedureVersions/Edit.cshtml:38, 100, 108, 121, 146, 177, 185, 199, 215, 233, 250, 272, 334`, `Views/Shared/_MediaList.cshtml:26`, `Views/CleaningTypes/Details.cshtml:38, 58` | frontend-engineer | Bu 16 form tanımına `asp-antiforgery="true"` (Faz 1 `Security.cshtml:12`, `Users/Form.cshtml:133` düzeni) ya da form içine `@Html.AntiForgeryToken()`. Yeniden test: sayfa HTML'inde her POST formunda belirteç; JS açık ve JS kapalı tarayıcıda faz ekle, adım ekle, taşı, çıkar, medya yükle ve çıkar, taslak aç, yanlış ve doğru PIN'le yayın; K4, K5, K6, K8, K11 arayüzden | KAPANDI (tur 2): 278 POST formunun 278'i belirteçli (G5a); JS açık 21, JS kapalı 22 gerçek form gönderiminde 400 yok; K4, K5, K6, K8, K11 arayüzden geçti (Tur 2, T2.1) |
| B2 | ORTA | G12 (aynı satır kontrol): `/procedure-versions/edit/{id}` 1024 px'te (açık ve koyu) faz düzenleme formlarında ve "Faz ekle" formunda "Faz adı" ile "Minimum süre (dakika)" alanları yan yana; dar ana kolonda (356 px) ikinci etiket iki satıra kırılıyor (24 / 48 px), minimum alanı faz adı alanından 24 px aşağıda. 360, 768 ve 1440'ta kusur yok. Genel G12 ölçümü bu kaymayı yakalamadı (örtüşme eşiği), hedefli ölçümle bulundu | `Evidence/Faz-3/G12Hizalama.txt` "Hedefli ölçüm"; `Duzenleyici-400.png` (aynı form); `Views/ProcedureVersions/Edit.cshtml:123` ve `:274` (`grid gap-4 md:grid-cols-2`) | frontend-engineer | İki sütunu `xl:` eşiğinden başlatmak (lot formu düzeltmesindeki gibi) ya da etiketi tek satıra sığacak kadar kısaltmak. Yeniden test: `row.js` 360/768/1024/1440 açık ve koyu, kusur 0 | KAPANDI (tur 2): 1024'te faz formları tek sütun; row.js 12 form ekranı x 4 genişlik x 2 tema kusur 0 (T2.1) |
| B3 | YÜKSEK | (tur 2) Düzenleyicide "Kayıtta malzeme zorunlu" onay kutusu tarayıcıdan işaretlenemiyor: üst bilgi formunda gizli `MaterialRequired=false` alanı onay kutusundan önce geliyor, tarayıcı gövdesi `MaterialRequired=false&MaterialRequired=true`, model bağlama ilk değeri (false) alıyor. İşaretleyip kaydedince değer false kalıyor; yürürlükteki versiyondan kopyalanan taslakta işaretli gelen kutu, doküman no kaydedilirken sessizce false oluyor (tip 5 v1 `MaterialRequired` 1, kopyası v2 doküman no kaydından sonra 0). JS açık ve kapalı aynı | `Evidence/Faz-3/Duzenleyici.txt` "Tur 2" B3 ve "Veritabanı sonucu"; `Views/ProcedureVersions/Edit.cshtml:41-42` | frontend-engineer | Gizli `false` alanını onay kutusundan sonraya almak (etiket yardımcısının `asp-for` düzeni). Yeniden test: `mr.js` JS açık ve kapalı işaretle, kaydet, yeniden aç: işaretli ve DB 1; işareti kaldır: DB 0 | KAPANDI (tur 3): üç durum x iki başlangıç x JS açık/kapalı 8/8; kopyalanan taslakta işaretli değer dokunmadan kayıtta true kaldı, yayınlanan v3 görüntülemede "Kayıtta malzeme zorunlu" (Tur 3, T3.1) |
| B4 | YÜKSEK | (tur 2, Faz 2 ekranları) Aynı sıra hatası Faz 2 aktiflik onay kutularında: tesis, hat, malzeme ve vardiya düzenleme formlarında gizli `IsActive=false` onay kutusundan önce. Tarayıcıdan aktif bir kaydı hiçbir şeye dokunmadan kaydetmek onu pasifleştiriyor (hat 4: önce aktif, gövde `IsActive=false&IsActive=true`, sonra pasif); pasif kaydı "Aktif" işaretleyip kaydetmek etkinleştirmiyor (tesis 3). Kusur Faz 2 commit'inde (`c3a4fd7`) de var; Faz 2 testleri istekleri elle kurduğu için görülmedi. Faz 3 tip formu (`asp-for`) doğru | `Evidence/Faz-3/Duzenleyici.txt` "Tur 2" B4; `Views/Facilities/Form.cshtml:24-25`, `Views/Lines/Form.cshtml:29-30`, `Views/Materials/Form.cshtml:24-25`, `Views/Shifts/Form.cshtml:33-34` | frontend-engineer | Dört formda gizli `false` alanını onay kutusundan sonraya almak. Yeniden test: `bool.js`/`bool2.js` dört formda değiştirmeden kaydet (aktif kalır), pasifi işaretle ve kaydet (aktif olur), işareti kaldır (pasif olur), JS açık ve kapalı | KAPANDI (tur 3): tesis, hat, malzeme, vardiya ve tip formlarında 40/40 durum doğru; Faz 2 K1, K4, K8 aktiflik gerilemesi yok (T3.1) |

## 7. Geçenler

- K1, K2, K3, K7, K9, K10, K12, K13, K14, K15; tip formunda kilit süresi sınırı ayardan (ayar değişince form ipucu, `max` ve sunucu birlikte değişiyor).
- Sunucu tarafı versiyon kuralları (engel metinleri, sıra no, derin kopya, PIN sayacı, yayın işlemi) ve medya kuralları (imza, sınırlar, tek kopya, 413,
  aralıklı okuma) belirteç elle eklendiğinde eksiksiz; reddedilen düzenleyici POST'larında hata metni 21 reddin 21'inde ilgili formda (14 düzenleyici, 7 medya; tam sayfa yanıt) ve
  fetch akışında görünür, `role=alert` ve odakta.
- PL-53: belirteç elle eklendiğinde yayın paneli faz eklenince "Faz 4 adımı yok", "Faz 4 minimum süresi yok" ile devre dışı, adım ve minimum girilince
  sayfa yenilenmeden etkin ("Taslak yayına hazır."); `window` işaretçisi korundu, ana çerçeve gezinmesi olmadı (`Duzenleyici.txt` 3).
- Video Chrome'da ileri sarılıyor; JS ön denetimi 10 MB üstü fotoğrafı istek göndermeden reddediyor; medya önizleme diyaloğu Esc ile kapanıyor.
- Yetki matrisi: oturumsuz 302; Operatör, Denetçi ve Süper admin yazma rotalarında 403; Denetçi taslakta 404; menüde gizli olup açılan sayfa yok.
- G11 (PL-55) dokuz tabloda beklenen imza; ön tur bulguları (lot tablosu `Lot no:2@1`, tarih ve saat alanı 48 px, lot formu 1024'te tek sütun) kapandı.
- G12: göz düğmesi altı ekranda kenarlık içinde ve ortalı; 338 hücrede panel çizgisi, alan sol kenarı, düğme satırı ve satır kartı kusur 0 (B2 dışında).
- Mekanik eval 394 ölçümde temiz; §4 grep 0; Faz 1-2 gerilemesi yok.
- DB: migration indekste, `Up()` yalnız ekleme, model farkı yok, demo veri tekrar uygulamada çoğalmıyor, 11 tetikleyici beklenen kuralları uyguluyor.
- §7 formülleri kodla uyumlu; §8 temiz; PascalCase uygun; yasak mimari yok.

## 8. Kapsam dışı / yapılamayan

- K4, K5, K6, K8, K11'in arayüz akışı B1 nedeniyle gerçek kullanıcı yoluyla yürütülemedi; bu kriterler KALDI sayıldı. Belirteç elle eklenerek yapılan tarayıcı
  ölçümleri yalnız düzeltme sonrası beklenen davranışı göstermek içindir, geçti sayılmadı.
- Yalnız Chromium ve Google Chrome (video); Firefox, Safari ve gerçek mobil cihaz test edilmedi. %200 yakınlaştırma eşdeğer CSS genişliğiyle ölçüldü.
- Test videosu Chrome MediaRecorder ile üretilen 6 saniyelik MP4; uzun ve yüksek bit hızlı videoda ileri sarma denenmedi. Sınır dosyaları geçerli imza + sıfır
  dolgu (oynatılabilir içerik değil).
- Engelleyen kayıt kontrolü (`ICleaningTypeBlockProvider`) Faz 4'te doluyor; engelli pasifleştirme ekranı üretilemedi.
- PIN kilitlenmesi ile oturumun kapanması (yayında 5 hatalı deneme) denenmedi; sayaç artışı ve doğru PIN'le sıfırlanması ölçüldü.
- Ekran okuyucuyla dinleme yapılmadı; `role=alert`, `aria-label` ve odak ölçümle denetlendi.
- `UiDelivery.md §2` matrisindeki 1920 ve %200 hücreleri Faz 1-2 gerilemesinde yeniden ölçülmedi (değişen yalnız kart modu ve form yükseklikleri).

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | Tip pasifleştirmenin engelleyen kayıt listesi (`BlockingRecords`, `IsActive` alan hatası) çalışan uygulamada ölçülecek | qa-tester | `ICleaningTypeBlockProvider` Faz 4'te doluyor |
| A2 | Faz 2'den devreden F2-A1 (makine kaldırma engel listesi) ve F2-A2 (`FindShiftAt` çalışma zamanı) | qa-tester | PL-52; ilk çağıran ve sağlayıcı Faz 4 |

## 10. Analist kararı

```
FAZ 3: İADE
Tarih: 2026-11-15
Gerekçe: B1 YÜKSEK açık (taslak düzenleyici ve "Yeni taslak aç" tarayıcıdan kullanılamıyor); K4, K5, K6, K8, K11 arayüzden
         karşılanmıyor. B2 ORTA (G12, K16) aynı fazda kapanır. Diğer 19 kriter kanıtla eşleşti; sunucu kuralları belirteç
         elle eklendiğinde K4-K8 şartlarını karşılıyor.
İade maddeleri: B1, B2 frontend-engineer (backend'e iş yok)
Yeniden denetim şartı: aşağıdaki "Tur 2 yeniden denetim" listesi
```

Analistin doğruladıkları (kanıt dışında):

- B1: Çalışma ağacında 16 formun hepsi şimdi `asp-antiforgery="true"` taşıyor (`Edit.cshtml:38, 100, 108, 121, 146, 177, 185, 199, 215, 233,
  250, 272, 334`, `Shared/_MediaList.cshtml:26`, `CleaningTypes/Details.cshtml:38, 58`); düzeltme frontend tarafından başlatılmış, henüz
  ölçülmedi. `action` özniteliği olan ve `asp-*` taşımayan başka POST formu `Views/` altında yok; öznitelik ya da `action` taşımayan formlar
  (yalnız `Views/Prototype/**`) belirteci etiket yardımcısından alır.
- B2: `Edit.cshtml:123` ve `:274` çalışma ağacında `xl:grid-cols-2`; QA'nın önerdiği çözüm uygulanmış, ölçülmedi.

Tur 2 yeniden denetim (qa-tester, aynı rapora "Tur 2" bölümü; önce yerleşim ön turu, PL-35):

- B1: `/procedure-versions/edit/{id}`, `/cleaning-types/details/{id}` (taslaklı ve taslaksız) ve Faz 3'ün diğer tüm sayfalarında HTML'deki her
  `method="post"` formu kendi `__RequestVerificationToken` gizli alanını taşır (G5a, PL-60). Tarayıcıdan, JS açık ve JS kapalı: yeni taslak aç,
  üst bilgi kaydet, faz ekle, düzenle, yukarı ve aşağı taşı, çıkar; adım ekle, düzenle, taşı, çıkar; fotoğraf ve video yükle, medya çıkar;
  yanlış PIN'le ve doğru PIN'le yayın. Her işlem 400 değil beklenen sonucu verir; K4, K5, K6, K8 (yükleme ve sınır hatası arayüzde), K11
  (fetch sonrası odak dahil) ve PL-53 (engel giderilince düğme sayfa yenilenmeden etkin) arayüzden yeniden işaretlenir. G5 gerilemesi:
  belirteçsiz POST'lar yine 400.
- B2 / G12 / K16: `/procedure-versions/edit/{id}` faz düzenleme ve "Faz ekle" formları 360, 768, 1024, 1440 açık ve koyu temada hedefli ölçümle
  (`row.js`) kusur 0; düzenleyicinin diğer formları ve E-09..E-12 G12 genel ölçümü ve G11 imzaları tur 1 ile aynı.
- Gerileme: G1, G2, §4 grep, konsol 0; değişen görünümlerde mekanik eval.

### Tur 2 kararı

```
FAZ 3: İADE
Tarih: 2026-11-15
Gerekçe: B1 ve B2 kapandı, 27/27 kriter geçti; ancak iki yeni YÜKSEK açık: B3 (malzeme zorunluluğu tarayıcıdan işaretlenemiyor,
         K4-K6 ve doküman satır 22-23 fiilen karşılanmıyor) ve B4 (Faz 2 aktiflik onay kutuları kayıt dokunmadan kaydedilince
         pasifleştiriyor). Tur 2 son turdur; tur 3 kullanıcı onayı ister (Workflow.md §3).
İade maddeleri: B3, B4 frontend-engineer (backend'e iş yok)
Yeniden denetim şartı: aşağıdaki "Tur 3 yeniden denetim" listesi; tur 3 yalnız kullanıcı onaylarsa
```

Analistin doğruladıkları (kanıt dışında):

- B3: `Edit.cshtml:41-42` okundu: elle yazılmış `<input type="hidden" name="MaterialRequired" value="false" />` onay kutusundan önce. İşaretli
  gönderimde gövde `MaterialRequired=false&MaterialRequired=true` olur; `bool` bağlama ilk değeri aldığı için sonuç false. Bulgu doğru.
- B4: aynı kalıp `Facilities/Form.cshtml:24`, `Lines/Form.cshtml:29`, `Materials/Form.cshtml:24`, `Shifts/Form.cshtml:33` (`IsActive`).
  `Views/` altında elle yazılmış `value="false"` gizli alanı bu beş satırdan ibaret.

B4 Faz 2 kodundadır (commit `c3a4fd7`). Faz 2 teslim raporu değişmez; düzeltme ve Faz 2 formlarının yeniden denetimi Faz 3'ün tur 3'ündedir
(PL-42 düzeni, PL-61). Faz 2 K1-K9 aktiflik değişikliğini tarayıcıdan üç durumla ölçmemişti; tekrarı önleyen kural G5b (PL-61).

Düzeltme yönü (frontend-engineer): `bool` onay kutuları `asp-for` ile çizilir (etiket yardımcısı gizli `false` alanını formun sonuna koyar);
elle yazılmış gizli `false` alanı kaldırılır. `Views/**` içindeki tüm `bool` onay kutuları taranır.

Tur 3 yeniden denetim (qa-tester, kullanıcı onayıyla; aynı rapora "Tur 3" bölümü):

- B3 / G5b: `/procedure-versions/edit/{id}` üst bilgi formunda "Kayıtta malzeme zorunlu" tarayıcıdan, JS açık ve JS kapalı, üç durumla:
  işaretsizken işaretleyip kaydet -> DB true; işaretliyken kaldırıp kaydet -> DB false; dokunmadan kaydet -> değer değişmez (iki başlangıç
  değeriyle). Kopyalanan taslakta işaretli gelen değer dokunmadan kaydedilince true kalır. Malzeme zorunlu taslak yayınlanınca versiyon
  görüntülemede işaretli görünür.
- B4 / G5b: `/facilities/edit/{id}`, `/lines/edit/{id}`, `/materials/edit/{id}`, `/shifts/edit/{id}` ve Faz 3 `/cleaning-types/edit/{id}`
  `IsActive` aynı üç durumla, aktif ve pasif başlangıçla; DB'de doğrulanır. Faz 2 K1, K4 (pasif vardiya çakışma dışı ve yeniden aktif), K5,
  K8 aktiflik süzgeçleri gerileme olarak.
- G5b grep: `Views/**` içinde elle yazılmış `type="hidden"` ve `value="false"` birlikte 0 eşleşme; her `bool` onay kutusu `asp-for` ile.
- Gerileme: G5a form belirteci sayımı, G1, G2, §4 grep, konsol 0; değişen görünümlerde mekanik eval, G11 ve G12 tur 2 ile aynı.

### Tur 3 kararı

```
FAZ 3: TESLİM ALINDI
Tarih: 2026-11-15
Gerekçe: Kullanıcı onaylı tur 3. B3 ve B4 tur 3 şartının her maddesiyle ölçülüp kapandı (üç durum testi 48/48, kopya taslak ve
         yayın, Faz 2 gerilemesi, G5b grep 0); K1-K17 ve G1-G7, G5a, G5b, G10-G12 (29/29) tur 1-3 kanıtıyla eşleşti; açık bulgu 0;
         QA önerisi GEÇTİ.
Açık kalemler: bölüm 9 A1 (tip pasifleştirme engel listesi) Faz 4'e F3-A1 olarak; A2 Faz 4'te F2-A1, F2-A2 olarak zaten yazılı.
```

Analistin doğruladıkları (kanıt dışında): `Views/**` içinde `type="hidden"` ile `false` birlikte 0 eşleşme; `ProcedureVersions/Edit.cshtml:45`
ve `Facilities/Form.cshtml:24` onay kutusu `asp-for` ile çiziliyor. B4 Faz 2 kodundaydı; Faz 2 raporu değişmedi (PL-61).

## Tur 2 (2026-11-15)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `08c793d`); frontend devir notu `Docs/Handover/frontend-engineer-20261115-0500.md` "Teslim testi B1, B2"; bölüm 10 "Tur 2 yeniden denetim" listesi, PL-60 (G5a) |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/f3/build` (yeni yayın; Tailwind QA çıktısı `wwwroot/css/app.css` ve yayındaki css ile bayt bayt aynı). Port 5301 Production `.qa/f3/qa.db` (tur 1 verisinin devamı), 5302 `.qa/f3/empty.db`. Chromium (playwright-cli); hiçbir isteğe belirteç elle eklenmedi |
| QA önerisi | KALDI (B1, B2 kapandı; tur 2'de bulunan YÜKSEK 2 açık: B3, B4) |

Betikler ve ham çıktılar `.qa/f3/` altında (`g5a.py`, `ui.tmpl.js`, `uion.txt`, `uioff.txt`, `st.txt`, `ed2.txt`, `mr.txt`, `bool*.txt`, `row.txt`, `mx2.out`, `g5b.out`).
Teslimden önce yerleşim ön turu yapıldı (PL-35): G11, G12, G5a ve mekanik eval temiz çıktı.

### T2.1 İade edilen bulguların yeniden ölçümü (bölüm 10, tur 2 şartı)

| # | İstenen sonuç (bölüm 10) | Ölçüm | Sonuç |
|---|---|---|---|
| 1. B1 / G5a | Faz 3'ün tüm sayfalarında her `method="post"` formu kendi belirtecini taşır | `g5a.py`: Faz 1-3'ün 63 sayfası (Yönetici, Süper admin, Operatör, Denetçi, ilk giriş, oturumsuz; düzenleyici, taslaklı ve taslaksız tip detayları, boş kopya): 278 POST formu, belirteçsiz 0. `/procedure-versions/edit/5` 52 form, `/cleaning-types/details/3` ve `/4` (iki "Yeni taslak aç" formu) 5 form, hepsi belirteçli | GEÇTİ |
| 2. B1 / K4, K5, K6, K8, K11, PL-53, JS açık | Tarayıcıdan, formun kendi belirteciyle: yeni taslak aç, üst bilgi, faz ekle/düzenle/taşı/çıkar, adım ekle/düzenle/taşı/çıkar, fotoğraf ve video yükle, medya çıkar, yanlış ve doğru PIN'le yayın; 400 yok | `uion.txt`, `st.txt` (tip 5, başlıktaki "Yeni taslak aç"): 21 POST, 400 yanıtı 0, ana çerçeve gezinmesi yalnız taslak açma ve yayında. Taslak v1'in 2 fazını kopyaladı (K6). Her fetch işleminden sonra durum iletisi ("Faz eklendi.", "Adım taşındı.", "Medya eklendi." ...). Taşımada odak aynı yön düğmesinde, sınırdaysa karşı yönde; eklemede faz başlığında; hata kutusunda (`role=alert`). PL-53: min'siz faz eklenince yayın düğmesi devre dışı ("Faz 4 adımı yok; Faz 4 minimum süresi yok"), adım eklenip minimum girilince sayfa yenilenmeden etkin (sayfa işaretçisi korundu). 10 485 761 baytlık fotoğrafı JS ön denetimi istek göndermeden reddetti ("Fotoğraf en çok 10 MB olabilir.", odak dosya alanında); GIF içerikli `.png` sunucuda reddedildi, hata medya formunda ve odakta. Medya çıkarma 302, medya sayısı 8 > 7. Yanlış PIN "PIN hatalı." yayın formunda, sayaç arttı; doğru PIN 302 "v2 yayınlandı.", v1 Arşiv, v2 Yürürlükte, sayaç 0 (K5). Klavye (`ed2.txt`): taşıma düğmesine Shift+Tab ile ulaşıldı, halka 3 px, Enter sonrası odak aynı düğmede; konsol 0 | GEÇTİ |
| 3. B1, JS kapalı | Aynı işlemler tam sayfa | `uioff.txt`, `st.txt` (tip 4, boş durumdaki "Yeni taslak aç"): 22 POST, 400 yanıtı 0. Boş taslak "En az bir faz gerekir" engeliyle açıldı; her işlem 302 ve düzenleyiciye dönüş, görünür durum iletisi ("Faz eklendi."); 10 485 761 baytlık fotoğraf sunucuda reddedildi, "Fotoğraf en çok 10 MB olabilir." yükleme formunda; GIF içerikli `.png` reddi aynı formda; medya çıkarma 302; yanlış PIN yayın formunda hata; doğru PIN 302 "v1 yayınlandı." | GEÇTİ |
| 4. Sonuç DB'de | Arayüz işlemlerinin etkisi | `uidb.out`: tip 5 v2 (Yürürlükte) = v1'in 2 fazı + "UI Faz 1" (300 sn, "UI Adım 1 güncel", PNG ve MP4) + "UI Faz 2" (180 sn); "UI Adım 2" ve "UI Silinecek" çıkarıldı, sıra no'lar 1'den boşluksuz; tip 4 v1 (Yürürlükte) aynı yapı; yayınlayan 1001. Doküman no kaydedildi; malzeme zorunluluğu kaydedilmedi (B3) | GEÇTİ (B3 ayrı) |
| 5. G5 gerilemesi | Belirteçsiz POST'lar 400 | `YetkiMatrisi.txt` "Tur 2": Faz 3'ün 16 yazma rotası Yönetici oturumunda belirteçsiz 3 gövdeyle 400, çok parçalı yükleme 400; Faz 1-2'nin 22 rotası dört gövdeyle ve oturumsuz `/account/login` 400; 400 olmayan 0 | GEÇTİ |
| 6. B2 / G12 / K16 | Düzenleyici faz formları 360/768/1024/1440 açık ve koyu `row.js` kusur 0; diğer ekranların G12 genel ölçümü ve G11 imzaları tur 1 ile aynı | `row.js` 12 form ekranı x 4 genişlik x 2 tema: kusur 0; 1024'te faz formları tek sütun, alanlar alt alta. G12 genel ölçümü 338 hücre kusur 0 (düzenleyici ve tip detayları dahil); göz düğmesi 176 ölçüm merkez farkı 0 | GEÇTİ |

### T2.2 Kabul kriterleri (yeniden işaretlenen ve gerileme)

| # | Sonuç | Kanıt |
|---|---|---|
| K4 | GEÇTİ | T2.1 madde 2-4: faz ve adım ekleme, taşıma, çıkarma arayüzden; sıra no'lar boşluksuz; "En az bir faz gerekir", "Faz n adımı yok", "Faz n minimum süresi yok" engelleri panelde |
| K5 | GEÇTİ | T2.1 madde 2-3: yanlış PIN reddi ve sayaç; doğru PIN'le Yürürlükte, önceki Arşiv, versiyon no +1 |
| K6 | GEÇTİ | T2.1 madde 2: yürürlükteki versiyondan taslak arayüzden açıldı, fazlar ve adımlar kopyalandı; ikinci taslak isteği reddi tur 1'de ölçüldü (sunucu kuralı değişmedi) |
| K8 | GEÇTİ | T2.1 madde 2-3: fotoğraf ve video arayüzden yüklendi, sınır ve imza hataları düzenleyicide; sınır, tek kopya, 206 ve ileri sarma tur 1 ölçümleri geçerli |
| K11 | GEÇTİ | T2.1 madde 2-3: klavye, fetch sonrası odak, JS kapalı tam sayfa |
| K16 | GEÇTİ | T2.1 madde 6 |
| G5 | GEÇTİ | T2.1 madde 5 |
| G5a | GEÇTİ | T2.1 madde 1: 278 POST formunun 278'i belirteçli |
| G12 | GEÇTİ | T2.1 madde 6 |
| G10 | GEÇTİ | `YetkiMatrisi.txt` "Tur 2": Süper admin 13 GET ve 16 POST'ta 403, menüde `/cleaning-types` yok |
| G11 | GEÇTİ | Ön tur ve teslim: 9 tablo 360/768/1024/1440'ta 40 ölçüm beklenen imzayla aynı (`MekanikEval.txt` "Tur 2") |
| G1 | GEÇTİ | `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata; css çıktısı aynı |
| G2 | GEÇTİ | Policy grep 0; öznitelikler değişmedi |

Kabul özeti tur 2 sonunda: K1-K16 16/16; ortak kriterler (G1-G7, G5a, G10-G12) 11/11; toplam 27/27. B3 ve B4 kabul kriteri satırlarının dışında kalan işlev
hatalarıdır; YÜKSEK oldukları için öneri KALDI.

### T2.3 Mekanik eval (§3)

152 ölçüm (`MekanikEval.txt` "Tur 2"): düzenleyici, tip detayları ve versiyon görüntüleme 360/768/1024/1440/1920 açık, koyu, 390 reduced-motion,
320x640 ve 640x400; Faz 1-3 form ekranları ve listeler 360/768/1024/1440; giriş ve ilk giriş. Hepsi temiz (h1, skip, etiket, ikon düğme adı, em/en-dash,
hScroll, taşma, hedef boyutu, dış kaynak, yazı tipi, üst bar, kontrast en düşük 5,28:1, konsol 0). §4 grep tam tablo 0 ihlal (transition-all, outline-none,
user-scalable, onclick, CDN, em/en-dash, h-screen, görünür `...`, jquery/gsap/aos, slate/gray/zinc, ham hex, boyutsuz img, autocomplete eksiği, ikon
`aria-hidden`).

### T2.4 Klavye turu (değişen kısım)

Düzenleyicide taşıma düğmeleri Tab sırasında, halka 3 px; Enter ile taşıma sonrası odak aynı düğmede (sınırda karşı yön); sunucu hatasında odak
`role=alert` kutusunda; JS ön denetiminde dosya alanında. Ekleme ve çıkarmada odak faz başlığına ya da faza gidiyor (programatik odak, fare ile
tetiklendiğinde halka çizilmiyor; klavye akışı etkilenmiyor).

### T2.5 DNA uygunluğu (değişen maddeler)

- Sayfa iskeleti ve veri dili: UYGUN. Düzenleyici faz formu 1024'te tek sütun, 1440'ta iki sütun; diğer maddeler tur 1 sonucunda kalır.

### T2.6 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-3/` (10 dosya, yeni dosya yok): `Duzenleyici.txt`, `YetkiMatrisi.txt`, `G12Hizalama.txt`, `MekanikEval.txt` sonlarına
"Tur 2" bölümü eklendi. Diğerleri tur 1'den.

### T2.7 Yeni bulgular

| # | Şiddet | Özet | Sahibi |
|---|---|---|---|
| B3 | YÜKSEK | Düzenleyicide "Kayıtta malzeme zorunlu" tarayıcıdan işaretlenemiyor; kopyalanan taslakta işaretli gelen değer kayıtta false oluyor (gizli alan onay kutusundan önce) | frontend-engineer |
| B4 | YÜKSEK | Faz 2 tesis, hat, malzeme ve vardiya formlarında aynı sıra: aktif kayıt değiştirilmeden kaydedilince pasifleşiyor, pasif kayıt etkinleştirilemiyor (Faz 2 commit'inde de var) | frontend-engineer |

Ayrıntı bölüm 6. İkisinin kök nedeni aynı (`<input type="hidden" value="false">` onay kutusundan önce; MVC tek değerli `bool` bağlamada ilk değeri alır).
Faz 3 tip formu ve Faz 1 kullanıcı formu bu düzeni kullanmıyor.

### T2.8 Kapsam dışı / yapılamayan (tur 2)

- Vardiya formundaki B4 davranışı tarayıcıda denenmedi; işaretleme aynı (`Shifts/Form.cshtml:33-34`), tesis ve hat formunda ölçüldü. Malzeme formunda pasif kayıt
  değiştirilmeden kaydedildi (pasif kaldı); aktif malzemeyle denenmedi.
- B3 ve B4 için ikinci tur hakkı bu turla doldu (en çok 2 tur); karar analistte.
- QA dışı süreçler PID 34348 (5080) ve 2580 (5181) dokunulmadı; QA'nın açtığı 5301 ve 5302 süreçleri ve playwright oturumları kapatıldı.

### T2.9 Sonuç

B1 ve B2 kapandı; K4, K5, K6, K8, K11, K16, G5a ve G12 arayüzden geçti, kabul kriterleri 27/27. Tur 2'de arayüz akışı ilk kez uçtan uca
yürütülebildiği için iki yeni YÜKSEK bulgu çıktı (B3 Faz 3 düzenleyici, B4 Faz 2 formları); QA önerisi KALDI. Karar analistte.

## Tur 3 (2026-11-15)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `08c793d`); frontend devir notu `Docs/Handover/frontend-engineer-20261115-0500.md` "B3, B4" ve "G5b (PL-61)"; bölüm 10 "Tur 3 yeniden denetim" listesi; PL-61 (G5b, Faz 3 FE 10, K17). Tur 3 kullanıcı onaylı |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/f3/build` (yeni yayın; Tailwind QA çıktısı `wwwroot/css/app.css` ve yayındaki css ile bayt bayt aynı). Port 5301 Production `.qa/f3/qa.db` (tur 1-2 verisinin devamı), 5302 `.qa/f3/empty.db`. Chromium (playwright-cli); bütün gönderimler tarayıcının kendi formundan |
| QA önerisi | GEÇTİ (KRİTİK, YÜKSEK, ORTA, DÜŞÜK açık 0) |

Betikler ve ham çıktılar `.qa/f3/` altında (`tri.py`, `tri.tmpl.js`, `tri.out`, `cp.txt`, `reg.txt`, `g5a3.out`, `row.txt`, `g12-h3.json`, `r-s3*.json`).
Teslimden önce ön tur: G5a sayımı, G5b grep'i, değişen görünümlerde G11, G12 ve mekanik eval temiz çıktı.

### T3.1 İade edilen bulguların yeniden ölçümü (bölüm 10, tur 3 şartı)

| # | İstenen sonuç (bölüm 10) | Ölçüm | Sonuç |
|---|---|---|---|
| 1. B3 / G5b | "Kayıtta malzeme zorunlu" JS açık ve kapalı üç durumla: işaretle -> true, kaldır -> false, dokunmadan -> değişmez (iki başlangıç) | `tri.out` (taslak 8): 2 mod x 4 durum, 8/8 doğru. İşaretli gövde `MaterialRequired=true&MaterialRequired=false` (kutu önce, gizli `false` sonra), işaretsiz gövde `MaterialRequired=false`; DB ve yeniden açılan sayfadaki kutu beklenen değerde | GEÇTİ |
| 2. B3 kopya ve yayın | Kopyalanan taslakta işaretli gelen değer dokunmadan kaydedilince true kalır; yayınlanınca versiyon görüntülemede işaretli | `cp.txt`: PLN v3 (v2'den kopya, DB 1) açılışta işaretli; doküman no ile dokunmadan kayıt 302, yeniden çizimde işaretli, DB 1; engelli faz çıkarıldı, doğru PIN'le "v3 yayınlandı."; `/procedure-versions/details/5` "Malzeme: Kayıtta malzeme zorunlu"; DB v3 Yürürlükte, `MaterialRequired` 1, v2 Arşiv. Başlık hatasında (doküman no 65) kutu gönderilen durumla yeniden çiziliyor (`reg.txt`) | GEÇTİ |
| 3. B4 / G5b | Tesis, hat, malzeme, vardiya, tip `IsActive` üç durum, aktif ve pasif başlangıç, DB'de | `tri.out`: 5 form x 2 mod x 4 durum, 40/40 doğru; dokunmadan kayıt aktif kaydı aktif, pasif kaydı pasif bırakıyor; işaretleme etkinleştiriyor, kaldırma pasifleştiriyor | GEÇTİ |
| 4. Faz 2 gerilemesi | K1, K4 (pasif vardiya çakışma dışı ve yeniden aktif), K5, K8 aktiflik süzgeçleri | `reg.txt`: K4: işaretsiz eklenen 10:00-12:00 vardiyası "Pasif" eklendi; düzenlemede Aktif işaretlenince "Bu aralık "Gündüz" vardiyasıyla çakışıyor." ve kutu pasif kaldı; Gece pasife alınınca 01:00-02:00 aktif kaydedildi; Gece yeniden aktif edilince "QA T3 çakışan" ile çakışma reddi; geri alındı. K1: pasif tesise hat "Pasif tesise hat eklenemez.". K8: pasif tesis ve malzeme varsayılan listede yok, `showInactive` ile "Pasif" rozetiyle var. K5 (makine kaldırma) onay kutusu kullanmıyor, değişen dosyaya bağlı değil; tur 1 sonucunda kalır | GEÇTİ |
| 5. G5b grep | `Views/**` elle yazılmış gizli `false` 0; her `bool` onay kutusu `asp-for` | 0 eşleşme; `bool` kutuları `Facilities/Form.cshtml:24`, `Lines/Form.cshtml:29`, `Materials/Form.cshtml:24`, `Shifts/Form.cshtml:33`, `ProcedureVersions/Edit.cshtml:45`, `CleaningTypes/Form.cshtml:92` hepsi `asp-for` | GEÇTİ |
| 6. Gerileme | G5a, G1, G2, §4, konsol, değişen görünümlerde mekanik eval, G11 ve G12 tur 2 ile aynı | G5a: 64 sayfa, 297 POST formu, belirteçsiz 0. G1: `-warnaserror --no-incremental` 0 uyarı 0 hata. G2 policy grep 0. §4 grep tam tablo 0 ihlal. Mekanik eval 130 ölçüm temiz (değişen beş form ve düzenleyici 360/768/1024/1440/1920, koyu, reduced-motion, 320x640; listeler), konsol 0. G11 40/40 aynı. G12 128 hücre kusur 0; `row.js` 12 form ekranı x 4 genişlik x 2 tema kusur 0 | GEÇTİ |

### T3.2 Kabul kriterleri (yeniden işaretlenen ve gerileme)

| # | Sonuç | Kanıt |
|---|---|---|
| K4, K5, K6 | GEÇTİ | Tur 2 sonucu; doküman satır 22-23 (malzeme zorunluluğu) T3.1 madde 1-2 ile arayüzden karşılanıyor |
| K17 | GEÇTİ | T3.1 madde 1, 3, 5: 48/48 durum, G5b grep 0 |
| G5b | GEÇTİ | T3.1 madde 1, 3, 5 |
| G5a | GEÇTİ | T3.1 madde 6 |
| G1, G2, G3, G11, G12 | GEÇTİ | T3.1 madde 6 |

Kabul özeti tur 3 sonunda: K1-K17 17/17; ortak kriterler (G1-G7, G5a, G5b, G10-G12) 12/12; toplam 29/29.

### T3.3 Mekanik eval (§3)

130 ölçüm (`MekanikEval.txt` "Tur 3"): tesis, hat, malzeme, vardiya ekleme ve düzenleme, tip düzenleme ve düzenleyici 360, 768, 1024, 1440, 1920 açık,
1440 ve 360 koyu, 390 reduced-motion, 320x640; dört liste ve iki ray tablosu 360/768/1024/1440. Hepsi temiz; en düşük kontrast 5,2:1 (AA üstü).

### T3.4 Klavye turu (değişen kısım)

Değişiklik yalnız onay kutularının işaretlemesinde; kutular etiket içinde, Tab sırası ve odak halkası tur 2 ile aynı (G12 ve eval temiz).

### T3.5 DNA uygunluğu (değişen maddeler)

Değişen madde yok; onay kutusu tasarımı (`.choice`) aynı.

### T3.6 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-3/` (10 dosya, yeni dosya yok): `KabulKriterleri.txt`, `Duzenleyici.txt`, `G12Hizalama.txt`, `MekanikEval.txt` sonlarına
"Tur 3" bölümü eklendi.

### T3.7 Yeni bulgular

yok

### T3.8 Kapsam dışı / yapılamayan (tur 3)

- Onay kutusu üç durum testi düzenleme formlarında yapıldı; ekleme formlarında yalnız vardiya (işaretsiz ekleme "Pasif" kaydetti) denendi.
- K5 (makine kaldırma) yeniden yürütülmedi: onay kutusu kullanmıyor, tur 3'te değişen dosyalara bağlı değil.
- QA dışı süreçler PID 34348 (5080) ve 2580 (5181) dokunulmadı; QA'nın açtığı 5301 ve 5302 süreçleri ve playwright oturumları kapatıldı. Test PIN'i
  yalnız `.qa/f3/j/secret.json` dosyasında (gitignore); betiklerden temizlendi.

### T3.9 Sonuç

B3 ve B4 kapandı; G5b ve K17 geçti, kabul kriterleri 29/29. Yeni bulgu yok; KRİTİK, YÜKSEK, ORTA ve DÜŞÜK açık bulgu 0. QA önerisi GEÇTİ. Karar analistte.

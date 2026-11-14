# Faz 2 Teslim Raporu: Tesis yapısı, vardiya ve malzeme

| Alan | Değer |
|---|---|
| Faz | 2 |
| Tarih | 2026-11-14 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `59fbe80`; Faz 2 kodu, migration `AddFacilitiesMachinesShiftsMaterials`, `DemoData.sql` ve devir notları commit'lenmemiş). Devir notları: `backend-engineer-20261115-0030.md`, `frontend-engineer-20261115-0100.md`, G11 imzaları `Archive/frontend-engineer-20261114-2330.md` ve `-2359.md`; `DB-2026-004` |
| Ortam | LOCAL, Release build (`dotnet publish -c Release -o .qa/f2/build`). Port 5161 Production, DB `.qa/f2/qa.db` (`App_Data/vaka.db` kopyası, SQLite yedekleme API'siyle salt okunur alındı, üstüne `DemoData.sql`). Port 5162 Production, `.qa/f2/qa-empty.db` (aynı kopya, tanım tabloları boşaltılmış; yalnız boş durumlar). Port 5163 Development, `.qa/f2/qa.db` (yalnız prototip gerilemesi) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2: B1-B10 kapandı, açık bulgu yok; tur 1: KALDI) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-14); tur 1: İADE (2026-11-14; B1-B10 tur 2'de kapandı, PL-42..PL-45) |

## 1. Kapsam

- DB: `Facilities`, `ProductionLines`, `Machines` (`RetiredAtUtc`, `CK_Machines_RetiredInactive`), `Shifts` (`CK_Shifts_StartNotEnd`), `Materials`, `MaterialLots`; kod kolonları NOCASE, BR-54 benzersiz index'leri; migration `AddFacilitiesMachinesShiftsMaterials` (DB-2026-004). Demo veri migration'da değil, `Data/Seed/DemoData.sql` (`INSERT OR IGNORE`, SKT'si geçmiş `L2026-001` dahil).
- BE: tanım servisleri (kod doğrulama, tire yasağı, benzersizlik, PL-04/PL-37 değişmezlik, BR-05 pasif tesis ve hat); vardiya kuralları (K-38) ve `IDurationCalculator` Faz 2 metotları (`EndsNextDay`, `DoShiftsOverlap`, `FindShiftAt`, `IsLotExpired`); makine kullanımdan kaldırma (engel sağlayıcı boş, PIN yok); `/machines` ve `/materials` sunucuda 20'lik sayfalama; JS kapalı makine formu akışı.
- FE: E-05 Tesisler ve hatlar (liste, ayrıntı, tesis ve hat formu), E-06 Vardiyalar, E-07 Makineler (tesis ve hat filtresi, bağlı seçim `CascadeSelect.js`, JS kapalı yenileme düğmesi, kaldırma ekranı), E-08 Malzemeler ve lotlar ("SKT geçmiş" rozeti); `_ListSkeleton`, `_ValueField`; Yönetici menüsünde "Tanımlar" grubu; ön tur düzeltmeleri (`.table-stack-rail .stack-lead`, `.table-stack td`, `.page-title`, `.page-lead`).

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Yönetici tesis, hat, makine ekler; yinelenen tesis kodu, aynı tesiste hat kodu, aynı hatta makine kodu ve tireli kod alan altında nedenle reddedilir | GEÇTİ | `Evidence/Faz-2/KabulKriterleri.txt` K1: `T1`/`t1`, `H1`/`h1`, `M01`/`m01` (kaldırılmış `M02` dahil) `Code` alanında "kayıtlı"; `A-B`, `H-9`, `M-1` "tire içeremez"; 17 karakter reddedildi; `T5`, hat `H1` (başka tesiste serbest), `M09` eklendi, hedef ekranda durum metni. Pasif tesis ve pasif hat reddi (BR-05) |
| K2 | Kod ve makinenin hattı düzenlemede değişmez; değiştirilmiş istek reddedilir | GEÇTİ | `KabulKriterleri.txt` K2: düzenleme formlarında `Code` `readonly`, makinede tesis ve hat `disabled`; `T9`, `t5`, `H8`, `M10` `Code` hatası; makinede başka hat, başka tesis, başka tesisin hattı `LineId` hatası; boş ya da gönderilmeyen kodla ad değişikliği 302; DB'de kod ve hat değişmedi |
| K3 | Hat listesi seçilen tesise göre; JS kapalıyken yenileme düğmesi aynı listeyi getirir; başka tesisin hattıyla gönderim `LineId` hatası, gönderilen tesisin hatlarıyla yeniden çizim, kayıt yok | GEÇTİ | `KabulKriterleri.txt` K3: `/machines/lines` JSON ile `GET /machines/create?facilityId=` seçenekleri aynı (pasif tesis boş); JS kapalı Chromium akışı: yenileme kodu ve adı korudu, eski hatla kaydet `LineId` "Seçilen hat bu tesise ait değil." ve tesis 3 hatları, kayıt 0; doğru hatla 302 "Makine eklendi."; liste süzgecinde uyuşmayan hat yok sayıldı. JS açık: klavyeyle akış, 500 ve boş hat durumları. Not: yenileme düğmesi antiforgery belirtecini adres çubuğuna taşıyor (B7) |
| K4 | Çakışan iki aktif vardiya reddedilir; bitiş = sonraki başlangıç serbest; ertesi gün biter listelenir; pasif vardiya kontrole girmez | GEÇTİ | `KabulKriterleri.txt` K4: 06-14 ile 13-20 ve 05-06:30 "QA A vardiyasıyla çakışıyor"; 14-22 ve 22-06 (iki sınır) kabul; pasif 13-20 kabul, aktife alınınca red; A pasife alınınca 07-09 kabul; 10-10, boş ve çözülemeyen saat alan altında Türkçe; pasif tesise red; listede `22:00 06:00 Ertesi gün biter`, T1 `Gece 23:00 07:00 Ertesi gün biter` |
| K5 | Makine kullanımdan kaldırılır; liste "Kullanımdan kaldırıldı" ve anını gösterir; silinmez | GEÇTİ | `KabulKriterleri.txt` K5: POST 302, durum metni; varsayılan listede yok, `showRetired=true` ile "Kullanımdan kaldırıldı 14.11.2026 18:00" (UTC 15:00 + 3); satır DB'de `IsActive 0`, `RetiredAtUtc` dolu; ikinci deneme nedenli red; PIN alanı yok (PL-37). Kaldırılmış makinenin ekranlarında B3 ve B5 |
| K6 | Dünkü SKT "SKT geçmiş" rozetiyle; bugünkü geçmiş sayılmaz | GEÇTİ | `KabulKriterleri.txt` K6: `QA-DUN` 13.11.2026 ve seed `L2026-001` "SKT geçmiş", `QA-BUGUN` 14.11.2026 ve `QA-YARIN` "Kullanılabilir"; listede "6 lot SKT geçmiş 2"; rozet ikon + metin + kenarlık |
| K7 | Operatör ve Denetçi fazın tüm adreslerinde 403 | GEÇTİ | `Evidence/Faz-2/YetkiMatrisi.txt`: 22 GET ve 13 POST (geçerli belirteçle) 302 `/account/access-denied` > 403; menüde Faz 2 bağlantısı 0 |
| K8 | `/machines`, `/materials` sayfalama ve süzgeç, `/facilities` aktiflik, `/shifts` tesis seçimi sunucuda; G11 imzaları 360/768/1024/1440 beklenenle aynı; ayrıntı tabloları PL-39; 360 ve 390'da yatay kaydırma yok; 1440 tablo | GEÇTİ | `KabulKriterleri.txt` K8 (sorgu dizesiyle açılan adresler: 20 + 9 satır, `showRetired` ve `showInactive` sayfa bağlantısında korunuyor, süzgeçler sunucuda); `Evidence/Faz-2/MekanikEval.txt` G11 bölümü: dört liste ve iki ayrıntı tablosu dört genişlikte beklenen imzayla aynı, 360/390 `hScroll` false, 1280 ve üstü tablo, satır en az 69 px. Sayfa dışı adres B6 |
| K9 | Faz 2 metotlarında formül ve kural kimliği; çakışma ve SKT kararı yalnız bu metotlardan; controller ve görünümde karşılaştırma yok | GEÇTİ | `IDurationCalculator.cs:16-51`, `DurationCalculator.cs:20-100`: dört metodun formülü ve kimliği (K-38, FR-49, BR-12, K-12) kodla uyuşuyor; çağıranlar `ShiftService.cs:47, 158`, `MaterialService.cs:60, 118`; controller/görünüm grep'inde karşılaştırma 0 (`KabulKriterleri.txt` K9) |
| G1 | `-warnaserror` 0/0; `build:css` başarılı | GEÇTİ | Bölüm 3 |
| G2 | §4 grep'leri 0; her controller `[Authorize(Roles = ...)]`/`[AllowAnonymous]`; rol metni yalnız `Roles` | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi kırık hücresiz, §4 sıfır ihlal, konsol 0 hata, DNA'dan sapma yok | KALDI | §4 grep 0, konsol 0, 892 ölçümde 891 temiz; ancak tablo modunda kırık düğme ve rozet metni (B2) ve 320 px'te ray taşması (B4) |
| G4 | Operatör ekranları 360'ta kaydırmasız, birincil düğme 44 px, durum metinle | GEÇTİ | Fazda Operatör ekranı yok; Faz 2 ekranlarında 44 px altı etkileşimli öğe 0 (onay kutuları 52 px etiket içinde), birincil düğmeler 56 px; durumlar ikon + metin rozetle |
| G5 | Antiforgery token'sız POST 400 | KALDI | `Evidence/Faz-2/G5-Antiforgery.txt`: gövdesinde `Code` alanı olan her POST'ta durum 400 değil; `Code` sayısal değilse `HTTP/1.1 0`, sayısalsa gönderilen değer (200, 403, 404, 500). B1 |
| G6 | Görünür metin, seed, doküman em/en-dash 0 | GEÇTİ | `Src` ve `Docs` taraması 0; 892 ölçümde `emdash` false; `DemoData.sql` 0 |
| G7 | `Up()` içinde Drop yok; ALTER onayı | GEÇTİ | `20261114143133_AddFacilitiesMachinesShiftsMaterials.cs` `Up()`: yalnız `CreateTable` ve `CreateIndex` (Drop, Alter, Rename, AddColumn, Sql 0); sınıf Eklemeli |
| G10 | Yalnız SuperAdmin rolündeki kullanıcı fazın tüm adreslerinde 403 ve menüde görmez | GEÇTİ | `YetkiMatrisi.txt` Süper admin kolonu (GET 22, POST 13 geçerli belirteçle: 302 > 403); `/users` sayfasında Faz 2 bağlantısı 0 |
| G11 | Liste ekranları 360/768/1024/1440 imzası beklenenle aynı | GEÇTİ | `MekanikEval.txt` G11 bölümü ve karşılaştırma satırları |

G8 ve G9 Faz 4 ve sonrası: bu fazda uygulanmaz. Kabul özeti: K1-K9 9/9; ortak kriterler (G1-G7, G10, G11) 7/9; toplam 16/18.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental` ile 0 uyarı, 0 hata; `dotnet publish -c Release` başarılı |
| CSS | `npm run build:css` | Başarılı; çıktı çalışma ağacındaki `wwwroot/css/app.css` ile bayt bayt aynı (`cmp`) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme (policy 0, sihirli `[Authorize(Roles = "` 0, `IsInRole("` 0). Altı yeni controller sınıf düzeyinde `[Authorize(Roles = Roles.Manager)]`; `[Authorize]`'sız tek dosya `SessionControllerExtensions.cs` (statik uzantı sınıfı, controller değil). MediatR, AutoMapper, generic repository 0; `new ...Service(` ve `GetService` 0; `Program.cs` içinde `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: tablolar `Facilities`, `ProductionLines`, `Machines`, `Shifts`, `Materials`, `MaterialLots`; kolonlar `ProductionLineId`, `RetiredAtUtc`, `StartTime`, `ExpiryDate`, `LotNo`; index ve kısıtlar `IX_Machines_ProductionLineId_Code`, `CK_Shifts_StartNotEnd`; dosyalar `MaterialLotsController.cs`, `MachineRetirement.cs`, `CascadeSelect.js` (`class CascadeSelect`); ViewModel'ler `...ViewModel` sonekli ve sözleşmedeki alanlarla aynı; asenkron metotlar `Async`, özel alanlar `_camelCase` |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (`Views/**`, `Styles/app.css`, `wwwroot/js/**`: transition-all, outline-none, boyutsuz img, user-scalable, onclick, CDN, em/en-dash, h-screen, görünür `...`, jquery/gsap/aos, ham hex, slate/gray/zinc 0; Faz 2 görünümlerinde autocomplete/inputmode eksik `<input>` 0) |
| Mekanik eval | `UiDelivery.md §3` | 892 ölçüm (Faz 2: 35 adres ve durum, ayrıca JS kapalı ve G11 hücre tekrarları; gerileme: Faz 1 12 adres, 5 prototip): h1 1, skip var, imgNoAlt/imgNoSize/inputNoLabel/iconBtnNoName 0, emdash yok, smallTargets 0, dış kaynak 0, font Hanken Grotesk ve Archivo (`document.fonts.check` true), üst bar 64 px, mobil gövde 16 px, en düşük kontrast 5,28:1, reduced-motion animasyon 0. Tek başarısız hücre: `/materials/details/5` 320x640 `hScroll` (B4). Ayrıntı `Evidence/Faz-2/MekanikEval.txt` |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | İki migration, ikisi de indekste (`AddUsersRolesAndSettings`: DB-2026-001/002; `AddFacilitiesMachinesShiftsMaterials`: DB-2026-004). Faz 2 migration'ı commit'lenmemiş, klasörde ve indekste aynı ad. `Up()` Drop 0. `dotnet ef migrations has-pending-model-changes --configuration Release`: "No changes have been made to the model since the last migration." Yeni tablolara `HasData` yok (Designer'daki `HasData` yalnız Faz 1 `SystemSetting`, `User`, `UserRole`); demo veri `DemoData.sql`'de, QA kopyasına iki kez uygulandı, sayılar değişmedi (2/3/5/3/3/5) |

Ek denetimler:

- §7: `EndsNextDay`, `DoShiftsOverlap`, `FindShiftAt`, `IsLotExpired` XML özetinde formül ve kural kimliği var, kodla uyuşuyor (`DoShiftsOverlap` bölme ve `[başlangıç, bitiş)` karşılaştırması `DurationCalculator.cs:31-46, 97-100`; `FindShiftAt` üç durum `:54-80`). `ShiftService.ApplyAsync` özetinde K-38 ve çağrılan metot yazılı. Uyumsuzluk 0.
- §8: emoji 0, `#region` 0, süs yorum 0, `Helper`/`Utils`/`Manager`/`Processor` adı 0, kalıp sözcük 0, boş `<summary>` 0. Yeni dört satır `//` yorumunun dördü de "neden" anlatıyor. Kullanılmayan parametre: B9.
- SOLID: controller'lar yalnız bağlama, servis çağrısı ve görünüm seçimi yapıyor; iş kuralı servislerde (`DefinitionRules`, `*Service`, `MachineRetirement`); bağımlılıklar arayüzle ve constructor'dan (`Program.cs` 9 yeni kayıt); kaldırma engeli `IMachineBlockProvider` ile genişletilebilir (Faz 4/7). `MachineService.cs` 262 satır (B9).
- G5 nedeni (backend'in curl gözlemi): curl hatası araca özgü değil, sunucunun durum satırı geçersiz. Ham soket, PowerShell 5.1 (`Section=ResponseStatusLine`) ve Playwright (`Parse Error: Invalid status code`) aynı sonucu verdi. `Program.cs:69` `UseStatusCodePagesWithReExecute("/error/{0}")` 400 yanıtını aynı POST gövdesiyle `/error/400`'e yeniden yürütüyor; `ErrorController.Status(int code)` (`ErrorController.cs:18-21`) `code` parametresini MVC kuralı gereği önce form gövdesinden bağlıyor. Faz 2 formlarındaki `Code` alanı bu parametreyi eziyor; `T9` gibi sayısal olmayan değer `code=0` ve `Response.StatusCode = 0` üretiyor. Faz 1 `/users/create` formunda `Code` alanı yok; backend'in denemesi gövdeye `Code=` eklediği için aynı sonucu verdi.

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Operatör (2002) ve Denetçi (2003), ayrı ayrı ölçüldü, sonuçlar aynı; Editor = Yönetici (2001); Admin = Süper admin (0001, yalnız `SuperAdmin`). Değer: ilk yanıt > yönlendirme izlenince son yanıt. POST satırlarında Viewer ve Admin geçerli antiforgery belirteciyle (yetki denetimi önce çalışıyor); Editor kolonunda parantez içi belirteçsiz sonuç. Tam tablo `Evidence/Faz-2/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/facilities`, `/facilities?showInactive=true`, `/facilities/create`, `/facilities/edit/1`, `/facilities/details/1` | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/lines/create?facilityId=1`, `/lines/edit/1` | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/shifts`, `/shifts?facilityId=1`, `/shifts/create?facilityId=1`, `/shifts/edit/1` | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/machines`, `/machines/create`, `/machines/edit/1`, `/machines/lines?facilityId=1`, `/machines/retire/1` | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/materials`, `/materials/create`, `/materials/edit/1`, `/materials/details/1` | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/material-lots/create?materialId=1`, `/material-lots/edit/1` (PL-41 kebab rota) | 302 (giriş) | 302>403 | 200 | 302>403 |
| GET `/facilities/details/999`, `/machines/retire/999`, `/material-lots/edit/999`, `/lines/create`, `/shifts/create`, `/material-lots/create` (kimliksiz) | 302 (giriş) | ölçülmedi | 404 | ölçülmedi |
| GET `/MaterialLots/Create?materialId=1` | ölçülmedi | ölçülmedi | 404 | ölçülmedi |
| POST `/facilities/create`, `/facilities/edit/1`, `/lines/create`, `/lines/edit/1`, `/machines/create`, `/machines/edit/1`, `/materials/create`, `/materials/edit/1` | 302 (giriş) | 302>403 | 200/302 (400; gövdede `Code` varsa 0, B1) | 302>403 |
| POST `/shifts/create`, `/shifts/edit/1`, `/material-lots/create`, `/material-lots/edit/1` | 302 (giriş) | 302>403 | 200/302 (400) | 302>403 |
| POST `/machines/retire/1` | 302 (giriş) | 302>403 | 302 (400; `Code=200` eklenirse 200, B1) | 302>403 |
| GET `/prototype/*` (Production) | 404 | 404 | 404 | 404 |

Menüde gizli olup URL ile açılan sayfa yok: Viewer ve Admin Faz 2'nin 22 GET ve 13 POST adresinin tümünde 403; menülerinde Faz 2 bağlantısı 0, Yönetici menüsünde `/facilities`, `/machines`, `/shifts`, `/materials`. Reddedilen hiçbir POST veri yazmadı (`YetkiMatrisi.txt` son satır).

## 5. Responsive ve görsel tur

Her hücrede §3 eval, AA kontrast, reduced-motion animasyon sayımı ve tablo/kart imzası ölçüldü (`.qa/f2/m2.tmpl.js`). Hücreler: 360x800, 768x1024, 1024x768, 1440x900, 1920x1080 (açık ve koyu), 800x360 yatay, %200 karşılığı 320x640 (açık) ve 640x400 (koyu), reduced-motion 390x844 (açık) ve 1440x900 (koyu); G11 için ek 390, 1279, 1280. JS kapalı ayrı bağlamda 7 genişlik. Ekran görüntüsü yalnız başarısız ya da şüpheli hücrede alındı (`.qa/f2/shots/`, 21 dosya, hepsi `Read` ile açıldı).

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/facilities` (dolu, pasif dahil, uzun ad 120) | Kart, imza aynı | Kart 4 sütun | Kart 4 sütun | Tablo, 69 px | Tablo | Temiz | Animasyon 0 | Temiz (320 ve 640) | Atlama bağlantısı ilk odak, Enter ile `main`; 8 Tab halka 3 px, üst bar örtmüyor |
| `/shifts` (T1, T5 karışık aktif/pasif, ertesi gün, boş tesis) | Temiz | Temiz | Temiz | Tablo | Tablo | Temiz | Temiz | Temiz | Tesis seçimi süzgeci iskeleti açıyor |
| `/machines` (29/31 kayıt, sayfa 2, süzgeçli boş) | Temiz | Temiz | Temiz | Tablo; Hat süzgeci 6 px yukarıda (B8) | B8 | Temiz | Temiz | Temiz | 360 çekmece: `aria-expanded` değişiyor, Esc kapatıyor, odak "Menü"ye dönüyor |
| `/materials` (28/29 kayıt, 32 karakter kod, süzgeçli boş) | Temiz, kod taşmıyor | Temiz | Temiz | Tablo, kod tek satır | Tablo | Temiz | Temiz | Temiz | Temiz |
| `/facilities/details/{id}` (hatlı, uzun hat adı, hatsız, pasif tesis) | Kart, PL-39 imza | Kart 4 sütun | Ray yanında 2 sütun, ön tur bulgusu kapandı | Tablo (1280-1380 px'te B2) | Tablo | Temiz | Temiz | Temiz | Temiz |
| `/materials/details/{id}` (SKT geçmiş, 64 karakter lot, lotsuz, 32 karakter kod) | Kart, PL-39 imza | Kart | Ray yanında 2 sütun | B2 (64 karakter lot; 1280'de olağan veriyle de) | Tablo | Temiz | Temiz | B4 (320 px ray kod taşması) | Temiz |
| Formlar: tesis, hat, vardiya, makine (ekle, tesisli, düzenle, kaldırılmış), malzeme, lot (16 adres) | Temiz, tek sütun, ray altta | Temiz | Ray yanında | Temiz | Temiz | Temiz | Temiz | Temiz | Boş gönderimde satır içi hatalar ve ilk alana odak (6 form); sunucu hatasında özet + alan, değer korunuyor; kaydedilmemiş değişiklikte `beforeunload`; makine formu yalnız klavyeyle tamamlandı |
| `/machines/retire/{id}` (aktif, kaldırılmış) | Temiz | Temiz | Temiz | Temiz; kaldırılmış makinede yanlış neden (B3) | Temiz | Temiz | Temiz | Temiz | Kaldır düğmesi halkalı |
| JS kapalı `/machines/create`, `/machines/edit`, `/machines?facilityId=1` | Yedek düğme görünür (292x76) | Görünür | Görünür | Görünür | ölçülmedi | ölçülmedi | ölçülmedi | ölçülmedi | Form sunucu tarafında çalışıyor (K3); JS açıkken yedek düğme `hidden` |
| Gerileme: `/`, `/users` (uzun ad, 16 haneli sicil), `/users/create`, `/users/edit/5`, `/account/security` | Temiz, `/users` imza Faz 1 tur 3 ile aynı | Temiz | Temiz | Tablo (1280'de sicil kırılıyor, B2) | Temiz | Temiz | Temiz | Temiz | Faz 1 akışı |
| Gerileme: giriş, `reason=expired`, 403, 404, oturumsuz Faz 2 adresi | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Faz 1 akışı |
| Gerileme: 5 prototip (Development) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | ölçülmedi |

Ön tur bulgularının kapanışı: (1) `/facilities/details/{id}` hat tablosu 1024'te `Ad:2@1 Kod:1@2 Makine:1@2 Durum:1@3 İşlem:1@3`, beklenenle aynı; KAPANDI. (2) `/materials` 32 karakterlik kod 360, 768, 1024'te hücre taşması 0, `hScroll` false; KAPANDI. (3) 360 ve 390 px'te `/material-lots/edit/10` ve `/materials/details/5` başlık ve alt satırda yatay kaydırma 0; KAPANDI (320 px'te ray kod satırı ayrı bulgu B4). `overflow-wrap: anywhere` gerilemesi: 1440 liste tablolarında kod hücreleri tek satırda ve sütun genişlikleri değişmedi, ancak ayrıntı tablolarında ve 1280-1380 bandında düğme ve rozet metni kırılıyor (B2); Faz 1 `/users` 1440 ve 1920'de değişmedi, 1280'de 16 haneli sicil no iki satıra kırılıyor (B2 içinde).

Konsol: yalnız 403/404 sayfalarında tarayıcının belge durum kaydı (Faz 1 ile aynı); başka hata ve uyarı 0. Ağ: 26 sayfa ve hat isteğinde 4xx/5xx 0, dış istek 0. Dört durum: yükleniyor iskeleti 4 liste ve sayfa bağlantısında görünür ("Yükleniyor…", liste gizli); boş durumlar boş tanımlı kopyada dört listede birincil eylemli, süzgeçli boş ve alt tablolarda da var; hata form özeti + alan altı, hat isteği 500'de nedenli metin; dolu uzun veriyle. Ayrıntı `MekanikEval.txt`.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Başlık Archivo, gövde Hanken Grotesk, kod ve saat JetBrains Mono; `document.fonts.check` 892 ölçümde true; Türkçe karakterler doğru (görüntüler).
- Palet: UYGUN. Tek accent `rgb(33,82,160)` açık, `rgb(122,170,235)` koyu; "SKT geçmiş" hata tonu, "Aktif" başarı tonu, "Kullanılabilir" nötr; AA altı metin 0 (en düşük 5,28:1).
- Köşe ölçeği: UYGUN. Düğme ve panel 2 px köşe, 2 px kenarlık, gölge 0, gradient 0.
- Gezinme arketipi: UYGUN. Yönetici rayında "Tanımlar" grubu (Tesisler, Makineler, Vardiyalar, Malzemeler); lg altı alt çubuk ve çekmece; üst bar 64 px; Süper admin menüsünde Faz 2 yok.
- Sayfa iskeleti: UYGUN. Form, ayrıntı ve kaldırma ekranları ana kolon + sağ bağlam rayı, lg ve üstü yan yana, altında ray altta; liste ekranları süzgeç paneli + tek tablo.
- Veri dili: UYGUN, kırık hücre bulgusuyla. 2 px başlık çizgisi, `sticky top 64px`, zebra yok, 1280 ve üstü tablo, 1279 ve altı kart (768 altı 2, 768-1279 4 sütun; ray yanındaki tablolar PL-39 ile 2), rozet tek biçim (ikon + metin + 2 px kenarlık), boş kesik çizgili kutu + eylem, yükleniyor durağan iskelet. Tablo modundaki kelime kırılması DNA kararı değil yerleşim hatası: B2.
- Hareket imzası: UYGUN. Geçiş yalnız renk özellikleri; reduced-motion hücrelerinde animasyon 0.
- İmza bileşen: bu fazda kapsam dışı (Faz 4-5).
- Anti-slop (`DesignDna.md §4`): temiz. Gradient, glow, cam, dört eşit KPI, iç içe kart, jenerik karşılama, sürüm rozeti, emoji, CDN yok; demo veri alana uygun ve nötr (Ana tesis, Hazırlık hattı, Yüzey dezenfektanı).

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | YÜKSEK | G5 karşılanmıyor: gövdesinde `Code` alanı olan POST'ta antiforgery hatası (ve 404) 400/404 yerine istemcinin `Code` değerini durum kodu olarak döndürüyor. Sayısal olmayan kod (`T9`, `M01`) geçersiz durum satırı `HTTP/1.1 0` üretiyor: curl, PowerShell ve Playwright isteği protokol hatasıyla düşürüyor, Chromium "Bir hata oluştu" sayfasını durum 0 ile gösteriyor; `Code=200` ile 200 OK. Tesis, hat, makine ve malzeme formlarının hepsinde `Code` alanı var; paylaşılan tablette "Kullanıcı değiştir" sonrası eski sekmedeki formun gönderimi bu yola düşer. Eylem çalışmıyor, veri yazılmıyor | `Evidence/Faz-2/G5-Antiforgery.txt`; `Controllers/ErrorController.cs:18-21` (`Status(int code)`, `Response.StatusCode = code`); `Program.cs:69` | backend-engineer | `code` yalnız rotadan alınsın (`Status([FromRoute] int code)`) ya da durum `IStatusCodeReExecuteFeature`/yanıttan okunsun. Yeniden test: `G5-Antiforgery.txt` 1-5 adımları; tüm Faz 2 POST'ları `Code=T9` ve `Code=200` ile 400, var olmayan kimlikte 404 | KAPANDI (tur 2): Faz 2 13 ve Faz 1 10 POST dört gövdeyle 400, `/account/logout` K11a istisnası; `/error/0`, `/200`, `/999` 404 (Tur 2, T2.1 madde 1) |
| B2 | YÜKSEK | Ön tur düzeltmesindeki ortak `.table-stack td { overflow-wrap: anywhere }` tablo modunda (1280 px ve üstü) da geçerli; en küçük içerik genişliği düştüğü için tablo düzeni düğme ve rozet sütunlarını kelime genişliğinin altına daraltıyor. Olağan veriyle 1280-1380 px'te `/materials/details/{id}` ("Düzen/le", "Kullanılabi/lir"; 24 karakter lot no) ve `/facilities/details/{id}` ("Düzen/le"; 95 karakterlik boşluklu hat adı) her satırda kırık; satır 69 > 93 px. 1440'ta 64 karakterlik lot noyla aynı kırılma ("Düze/nle", "SKT/geçmiş"). 1280'de `/materials` 32 karakterlik kod ve Faz 1 `/users` 16 haneli sicil no iki satıra kırılıyor. `overflow-wrap: normal` ile aynı sayfalarda kırılma 0 | `Evidence/Faz-2/DetayTablo-1280.png`, `LotTablo-1440.png`, `OverflowWrap.txt` (şimdi ve normal karşılaştırması, 1280/1366/1400/1440/1920); `Styles/app.css:551-554` | frontend-engineer | Kuralı kart moduyla sınırlamak (`max-xl:` altında) ya da yalnız kod/lot hücresinin içeriğine vermek; tablo modunda düğme ve rozet `whitespace-nowrap`. Ön tur bulgu 2 ve 3 (360/768/1024 kod taşması, 360 başlık) korunmalı. Yeniden test: `OverflowWrap.txt` yöntemi 1280, 1366, 1440, 1920'de tüm satırlar "AYNI"; G11 imzaları dört genişlikte | KAPANDI (tur 2): 1280-1920 düğme, rozet, çip kırılması 0; sığan değer tek satır, sığmayan yalnız kendi hücresinde; panelden taşma 0 (T2.1 madde 2) |
| B3 | ORTA | Zaten kaldırılmış makinenin kaldırma ekranı yanlış neden gösteriyor: "Aktif ya da beklemedeki kaydı olan makine kullanımdan kaldırılamaz. Önce yukarıdaki kayıtları tamamlayın ya da kapatın." Yukarıda kayıt yok. Ekran kaldırılmış makinenin düzenleme formundaki "Kullanımdan kaldır" bağlantısıyla açılıyor (hata durumu metni yanlış) | `Evidence/Faz-2/KaldirilmisMakine-Retire.png` (`/machines/retire/4`); `Views/Machines/Retire.cshtml:50-54`; `Views/Machines/Form.cshtml:104-111` | frontend-engineer | `CanRetire` false ve `BlockingRecords` boşsa "Makine zaten kullanımdan kaldırıldı" ve listeye dönüş; kaldırılmış makinede formdaki bağlantı gösterilmesin (B5 kararına bağlı alan gerekebilir) | KAPANDI (tur 2): bilgi bildirimi, gönder düğmesi ve engel metni yok (T2.1 madde 4) |
| B4 | ORTA | `/materials/details/{id}` bağlam rayında 32 karakterlik malzeme kodu (`dd.mono`) 320 px'te (%200 yakınlaştırma) sığmıyor: `scrollWidth` 341 > 320, ray içeriği sağdan kesiliyor; Ad satırı da aynı genişliğe uzuyor. 360 px'te sığıyor (341 < 360) | `Evidence/Faz-2/MalzemeRay-320.png`; `MekanikEval.txt` "KALDI" satırı; `Views/Materials/Details.cshtml:84`; `.kv dd` `Styles/app.css:322` | frontend-engineer | `.kv dd` için `min-w-0` ve kırılma (`overflow-wrap: anywhere` burada uygun, ray tablo değil). Yeniden test: 320x640 ve 640x400'de `hScroll` false | KAPANDI (tur 2): 320x640 ve 640x400 `hScroll` false (T2.1 madde 3) |
| B5 | DÜŞÜK | Değerlendirme (koordinatör sorusu): kaldırılmış makinenin `/machines/edit/{id}` formu "Değişiklikleri kaydet" ile açılıyor ve ad değişikliği kaydediliyor (302). PL-04/PL-37 yalnız kod ve hattı kilitliyor; BR-05, FR-02, BR-35 yeni kayıtta seçimi ve kaldırma engelini kapsıyor; kaldırılmış makinenin adının düzenlenmesini yasaklayan ya da izin veren karar yok, davranış hiçbir kurala aykırı değil. Ancak form makinenin kaldırılmış olduğunu göstermiyor (`MachineFormViewModel`'de durum alanı yok) ve "Kullanımdan kaldır" bağlantısı sunuyor (B3) | `KabulKriterleri.txt` K5 son üç satır; `ViewModels/Definitions/MachineFormViewModel.cs`; `Services/MachineService.cs:208-240` | analyst | Kural kararı: kaldırılmış makinenin adı düzenlenebilir mi. Evetse forma durum bilgisi (backend VM alanı, frontend gösterim); hayırsa düzenleme reddi ve listede "Düzenle" gizlenir | KAPANDI (tur 2): PL-43 uygulandı; düzenleme salt okunur, POST nedenli red, DB değişmedi (T2.1 madde 4) |
| B6 | DÜŞÜK | Son sayfanın ötesindeki adres (`/machines?page=3`, 29 kayıt; `/machines?page=99`) "Henüz makine yok" ve "Makine ekle" gösteriyor; kayıt varken yanlış boş durum. `/materials` aynı koşullu yapıda | `KabulKriterleri.txt` K8 (`/machines?page=3 satir=0 durum=['Henüz makine yok']`); `Views/Machines/Index.cshtml:70-78`, `Views/Materials/Index.cshtml:47-55` | frontend-engineer | `Total > 0` iken "Bu sayfada kayıt yok" + 1. sayfa bağlantısı (ya da backend sayfayı son sayfaya sınırlar) | KAPANDI (tur 2): "Bu sayfada kayıt yok." ve süzgeçli 1. sayfa bağlantısı (T2.1 madde 5) |
| B7 | DÜŞÜK | JS kapalı "Seçilen tesisin hatlarını getir" düğmesi `formmethod="get"` ile tüm formu gönderiyor; antiforgery belirteci adres çubuğuna, tarayıcı geçmişine ve sunucu erişim günlüğüne düşüyor (`?FacilityId=3&LineId=&Code=M88&Name=...&__RequestVerificationToken=...`) | `KabulKriterleri.txt` K3 JS kapalı adım 2; `Views/Machines/Form.cshtml:86` | frontend-engineer | Yedek düğmeyi yalnız `facilityId`, `code`, `name` taşıyan ayrı GET formuna bağlamak (`form` özniteliği) | KAPANDI (tur 2): yenileme aynı POST formunda, adreste ve günlükte belirteç 0 (T2.1 madde 6) |
| B8 | DÜŞÜK | `/machines` süzgecinde xl ve üstünde (1440, 1920) "Hat" etiketi ve seçimi "Tesis" ve arama alanından 6 px yukarıda (etiket 238/244, seçim 268/274); boş `#line-status` paragrafı ve `xl:items-end` | `.qa/f2/align.js` çıktısı (1440: `L:Hat@1077,238`, `SELECT:lineId@1077,268`, `L:Tesis@751,244`); `Views/Machines/Index.cshtml:27, 51` | frontend-engineer | Hizalamayı `items-start` ile ya da durum satırını alanın dışına alarak eşitlemek | KAPANDI (tur 2): 1280, 1440, 1920 etiket ve alan üst kenarı eşit (T2.1 madde 7) |
| B9 | DÜŞÜK | `MachineService.cs` 262 satır (§2: 200 üstü bölünmeye aday; liste, form, ekleme, düzenleme ve seçenek listeleri tek sınıfta). `ListFacilitiesAsync(bool activeOnly, int? includeId, ...)` parametresi `includeId` her çağrıda `null` (§8, kullanılmayan parametre) | `Services/MachineService.cs:99, 144, 242-245` | backend-engineer | `includeId` kaldırılsın; seçenek listeleri ayrı küçük servise ayrılabilir | KAPANDI (tur 2): `includeId` 0; 269 satırın gerekçesi devir notunda (T2.1 madde 8) |
| B10 | DÜŞÜK | `/machines` süzgecinde tesis seçilmemişken hat etiketleri "T1-H1 Hazırlık hattı" biçiminde; sözleşme makine ekranları için "Kod - Ad" diyor. Biçim farklı tesislerdeki aynı hat kodlarını ayırmak için mantıklı, ancak sözleşme dışı | `Services/MachineService.cs:256`; PhasePlan Faz 2 "Ortak davranış" 1. madde | analyst | Biçim kabul edilip sözleşmeye yazılsın ya da "Kod - Ad"a çekilsin | KAPANDI (tur 2): PL-44 biçimi uygulandı (T2.1 madde 8) |

## 7. Geçenler

- K1-K9 kabul kriterlerinin tümü çalışan uygulamada geçti; BR-54 benzersizliği NOCASE ile büyük/küçük harf ayırmıyor; PL-04/PL-37 değişmezliği değiştirilmiş isteklerde korunuyor; BR-05 pasif tesis ve hat reddi.
- Yetki matrisi: oturumsuz 302, Operatör, Denetçi ve yalnız Süper admin 22 GET ve 13 POST'ta 403; menüde gizli olup açılan sayfa yok (G10). Kimliksiz ve yok kimlikli adresler 404.
- K-38 vardiya kuralları: sınır eşitliği, gece yarısı, pasif vardiya ve yeniden aktifleştirme; liste "Ertesi gün biter".
- BR-12 SKT kararı tek metotta, Türkiye tarihine göre; bugünkü SKT geçmiş sayılmıyor.
- K3 JS kapalı akış uçtan uca; JS açıkken bağlı seçimin yükleniyor, boş, hata ve dolu durumları; yarış belirteci.
- Sunucu tarafı sayfalama ve süzgeçler adres çubuğundan aynı sonucu veriyor; süzgeç sayfa bağlantılarında korunuyor.
- G11 imzaları dört liste ve iki ayrıntı tablosunda beklenenle birebir; ön turun üç bulgusu kapandı.
- Mekanik eval 892 ölçümde 891 temiz; §4 grep 0; konsol ve ağ temiz; klavye turu, kaydedilmemiş değişiklik uyarısı, çekmece.
- DB: migration indekste, `Up()` yalnız ekleme, model farkı yok, demo veri migration dışında ve tekrar çalıştırmada çoğalmıyor.
- §7 formül yorumları kodla uyumlu; §8 taraması temiz; PascalCase uygun; yasak mimari yok.
- Gerileme: Faz 1 giriş, ilk giriş, `/users` (imza Faz 1 tur 3 ile aynı), 403/404 ve prototipler temiz (1280'deki sicil kırılması B2'de).

## 8. Kapsam dışı / yapılamayan

- Yalnız Chromium (playwright-cli) kullanıldı; Firefox, Safari ve gerçek mobil cihaz test edilmedi.
- %200 yakınlaştırma tarayıcı yakınlaştırmasıyla değil, eşdeğer CSS genişliğiyle ölçüldü (320x640 = 640 px pencere, 640x400 = 1280 px pencere).
- Yükleniyor iskeleti gerçek ağ gecikmesiyle izlenemedi: localhost'ta Playwright rotası ve CDP gecikme taklidi gezinmeyi geciktirmedi. Durum, `FormBusy` gönderim işleyicisi çalıştıktan sonra gönderim durdurularak ölçüldü (`load4.js`); görünürlük ve metin doğrulandı.
- Kaldırmayı engelleyen kayıt listesi (BR-35) Faz 2'de boş sağlayıcıyla geliyor; engelli kaldırma ekranı çalışan uygulamada üretilemedi (Faz 4/7).
- `FindShiftAt` Faz 2'de çağıransız (PL-40); yalnız kod okunarak denetlendi, çalışma zamanında denenmedi.
- Eşzamanlı aynı kodlu ekleme (`DbUpdateException` yolu) eşzamanlı istekle denenmedi; sıralı yinelenen istekler reddedildi.
- Ekran okuyucu ile dinleme yapılmadı; erişilebilir ad, etiket, `role=status` ve odak ölçümle denetlendi.
- `/shifts` tesis yokken boş durumu ayrı kopyada (port 5162, tanım tabloları boşaltılmış) ölçüldü, ana kopyada değil.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | Makine kaldırma ekranının engelleyen kayıt listesi ve `Url` görünürlüğü çalışan uygulamada ölçülecek | qa-tester | BR-35 engel sağlayıcısı Faz 4 ve 7'de doluyor |
| A2 | `FindShiftAt` gece yarısını geçen vardiya ve sınır anlarıyla çalışma zamanında denenecek | qa-tester | İlk çağıran Faz 4 SH-13 (PL-40) |

## 10. Analist kararı

```
FAZ 2: İADE
Tarih: 2026-11-14
Gerekçe: K1-K9 kanıtla eşleşti; G5 (B1 YÜKSEK) ve G3 (B2 YÜKSEK, B4 ORTA) karşılanmadı, ORTA B3 açık. DÜŞÜK B5-B10
         Faz 2 dosyalarında ve sonraki fazlar bu dosyalara dönmediği için ertelenmez, tur 2'de kapanır (PL-45).
İade maddeleri: B1, B9, B10 backend-engineer; B2, B3, B4, B6, B7, B8 frontend-engineer; B5 ve B7 ikisi birlikte (sözleşme PL-43, PL-44)
Yeniden denetim şartı: aşağıdaki "Tur 2 yeniden denetim" listesi; yerleşim maddeleri G11 imzasıyla
```

Analistin doğruladıkları (kanıt dışında):

- B1: `ErrorController.cs:18-21` ve `Program.cs:69` okundu. `.qa/f2/build` ayrı portta (5191, `qa.db`'nin kopyası) çalıştırıldı: oturumsuz
  `POST /account/login` gövdesi `Code=T9&RegistryNo=1` durum satırı `HTTP/1.1 0`, `RegistryNo=1` ise `400`. Ek bulgu: doğrudan
  `GET /error/0` da `HTTP/1.1 0`, `GET /error/200` `200 OK` döndürüyor; hata POST'a bağlı değil, rotadaki her tam sayı durum kodu olarak
  yazılıyor. Sunucu kapatıldı.
- B2: `Styles/app.css:551-554` okundu; `overflow-wrap: anywhere` `max-xl:` önekisiz, tablo modunda da geçerli. Bulgu doğru.
- B10: `MachineService.cs:259` okundu; tesis seçilmemişken etiket `Kod-Kod Ad` biçiminde, `" - "` ayıracı yok.

Faz 1 etkisi: B1 Faz 1 kodundadır (`ErrorController`, `Program.cs`; commit `38d0b4e`). Faz 1 G5 o fazın formlarında `Code` alanı olmadığı için
GEÇTİ ölçüldü; aynı uç oturumsuz `/account/login` dahil Faz 1 POST'larında gövdeye `Code` eklenince bozuluyor (yukarıdaki doğrulama).
Faz 1 teslim raporu değişmez; düzeltme ve Faz 1 POST'larının yeniden denetimi bu fazın tur 2'sindedir (PL-42).

Düzeltme listesi. Dosyalar ayrık; backend-engineer ve frontend-engineer tek mesajda birlikte çağrılabilir (sözleşme `PhasePlan.md` Faz 2, PL-43, PL-44).

backend-engineer (`ErrorController.cs`, `MachinesController.cs`, `MachineService.cs`, `ViewModels/Definitions/Machine*ViewModel.cs`):

1. B1: `Status` parametresi yalnız rotadan bağlanır (`[FromRoute] int code`); re-execute'ta form gövdesi hata eylemine bağlanmaz. 400-599 dışındaki
   kod (`/error/0`, `/error/200`) 404 ve "Sayfa bulunamadı" görünümüyle döner; `Response.StatusCode`'a rota değeri denetimsiz yazılmaz.
   İstenirse asıl kod `IStatusCodeReExecuteFeature`'dan da okunabilir; sonuç aynı olmalı.
2. B5 (PL-43): kaldırılmış makinenin düzenleme POST'u servis tarafında nedenli reddedilir ("Kullanımdan kaldırılmış makine değiştirilemez."),
   kayıt yazılmaz. `MachineFormViewModel` ve `MachineRetireViewModel`'e `RetiredAtLocal?: DateTime` eklenir ve dolar.
3. B7 (PL-44): `POST /machines/create` gövdesinde `refresh=true` varsa kayıt yapılmaz, alan hatası üretilmez; form gönderilen tesisin aktif
   hatlarıyla, kod ve ad korunarak yeniden çizilir. `GET /machines/create?facilityId=` doğrudan adres olarak kalır.
4. B9: `ListFacilitiesAsync` `includeId` parametresi kaldırılır. Sınıfı bölmek zorunlu değil; 200 satırın üstünde kalırsa nedeni devir notunda.
5. B10 (PL-44): tesis seçilmemişken hat etiketi `<TesisKodu>-<HatKodu> - <HatAdı>` (ör. `T1-H1 - Hazırlık hattı`); tesis seçiliyken ve
   `/machines/lines` JSON'unda "Kod - Ad" değişmez.

frontend-engineer (`Styles/app.css`, `Views/Machines/**`, `Views/Materials/**`, gerekirse `Views/Facilities/Details.cshtml`):

1. B2: `overflow-wrap: anywhere` yalnız kart modunda (`max-xl:`) ve yalnız uzun veri hücrelerinde (kod, lot no, ad); düğme ve rozet metni
   iki modda da kelime ortasından kırılmaz. Tablo modunda uzun kod ya da lot no sığmıyorsa kırılma yalnız o hücrede olur; düğme ve rozet
   sütunu daralmaz. Kart modu imzaları (`MekanikEval.txt:120-125`) değişmez, ön tur bulgu 2 ve 3 kapalı kalır.
2. B4: `.kv dd` `min-w-0` ve kırılmalı; ray tablo değildir.
3. B3 + B5 (PL-43): `RetiredAtLocal` doluysa kaldırma ekranı "Makine <an> tarihinde kullanımdan kaldırıldı." bilgisini ve makine listesine
   dönüşü gösterir, gönder düğmesi ve engelleyen kayıt metni yoktur. Düzenleme ekranı salt okunurdur: alanlar değiştirilemez, kaydet düğmesi
   ve "Kullanımdan kaldır" bağlantısı yoktur, kaldırılma anı görünür. Listede kaldırılmış satırın eylemi "Görüntüle" (aynı adres).
4. B6: `Total > 0` ve sayfada satır yoksa "Bu sayfada kayıt yok." ve 1. sayfa bağlantısı (`/machines`, `/materials`, süzgeç korunur);
   `Total = 0` boş durumu değişmez.
5. B7 (PL-44): JS kapalı yenileme düğmesi `formmethod="get"` yerine aynı POST formunda `name="refresh" value="true"`; JS açıkken gizli kalır.
6. B8: `/machines` süzgecinde Tesis, Hat ve arama etiketleri ve alanları xl ve üstünde aynı hizada.

Tur 2 yeniden denetim (qa-tester, aynı rapora "Tur 2" bölümü):

- B1 / G5: `G5-Antiforgery.txt` 1-5 adımları. Faz 2'nin 13 POST'u ve Faz 1'in 11 POST'u (`/account/*` 5, `/users/*` 6) belirteçsiz,
  gövdede `Code` yok, `Code=T9`, `Code=200`, `code=404` ile: dördü de geçerli durum satırıyla 400. Geçerli belirteçle var olmayan kimlik
  (`/machines/edit/9999`, `/facilities/edit/9999`) `Code=M01` ile 404. `GET /error/0`, `/error/200`, `/error/999` 404; `/error/403`, `/error/404`,
  `/error/500` doğru görünüm ve durum. K7 ve G10 POST satırları (`YetkiMatrisi.txt`) yeniden; veri yazımı 0.
- B2 / G3 / G11: `OverflowWrap.txt` yöntemi 1280, 1366, 1440, 1920'de uzun veriyle (64 karakter lot no, 32 karakter kod, 95 karakter hat adı,
  16 haneli sicil no): düğme ve rozet metninde kırılma 0, `hScroll` false, satır en az 56 px. G11 imzası dört liste, iki ray tablosu ve Faz 1
  `/users` için 360, 768, 1024, 1440'ta tam olarak ölçülür ve `MekanikEval.txt:120-125` ile Faz 1 tur 3 `/users` imzasıyla aynıdır;
  360 ve 390'da `hScroll` false.
- B4: `/materials/details/{id}` (32 karakter kod) 320x640 ve 640x400'de `hScroll` false; `/machines/retire/{id}` ve lot formu rayında aynı ölçüm.
- B3, B5: kaldırılmış makinede `/machines/retire/{id}` doğru bilgi, gönder düğmesi yok; `/machines/edit/{id}` salt okunur, kaydet ve kaldır
  bağlantısı yok; geçerli belirteçle düzenleme POST'u nedenli red, DB değişmez; liste eylemi "Görüntüle". Aktif makinede K2 ve K5 gerilemesi.
- B6: `/machines?page=3`, `/machines?page=99`, `/materials?page=99` "Bu sayfada kayıt yok." ve 1. sayfa bağlantısı; boş kopyada boş durum aynı.
- B7 / K3: JS kapalı akış yeniden; adres çubuğunda ve sunucu günlüğünde `__RequestVerificationToken` yok, kod ve ad korunuyor, yenilemede
  kayıt 0, eski hatla kaydet `LineId` hatası, doğru hatla 302.
- B8: `.qa/f2/align.js` 1440 ve 1920'de Tesis, Hat ve arama etiketlerinin ve alanlarının üst kenarı eşit.
- B9, B10: `includeId` grep 0; `/machines` tesis seçilmemişken `T1-H1 - Hazırlık hattı`, tesis seçiliyken `H1 - Hazırlık hattı`, JSON değişmedi.
- Gerileme: G1, G2, §4 grep, Faz 2 ekranları ve Faz 1 `/users` için 360/768/1024/1440 mekanik eval, konsol 0.
- Tur 2 öncesi ön tur (PL-35): B2, B4, B8 yerleşim etkili; qa-tester teslim turundan önce G11 ve mekanik eval ön turunu bu ekranlarda yapar.

### Tur 2 kararı

```
FAZ 2: TESLİM ALINDI
Tarih: 2026-11-14
Gerekçe: K1-K9 ve G1-G7, G10, G11 (18/18) Tur 2 T2.1-T2.2 kanıtıyla eşleşti; B1-B10 tur 1 yeniden denetim şartının her
         maddesiyle ölçülüp kapandı; KRİTİK, YÜKSEK, ORTA, DÜŞÜK açık bulgu 0; QA önerisi GEÇTİ.
Açık kalemler: bölüm 9 A1, A2 (qa-tester, Faz 4); yeni devreden kalem yok.

Analistin doğruladıkları: ErrorController.cs:18-33 ([FromRoute] int code, 400-599 dışı 404); Styles/app.css:555-589
(xl ray yanı tabloda yalnız td.cell-long kırılır, diğer hücreler nowrap; overflow-wrap:anywhere diğer tablolarda yalnız
1280 px altı kart modunda; düğme ve rozet kırılmaz); MachineService.cs:221-224 (kaldırılmış makine düzenleme reddi);
dotnet build -c Release -warnaserror --no-incremental 0 uyarı 0 hata.
QA notları: Machines/Form.cshtml:20 boş data-unsaved-guard etkisiz (alanlar readonly/disabled, diyalog 0), bulgu
sayılmaz; sonraki Views/Machines değişikliğinde öznitelik koşullu yazılır. 640x400 Ad:1@1 tur 1 ile aynı, G11 dışı, kabul.
```

## Tur 2 (2026-11-14)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `59fbe80`; Faz 2 kodu commit'lenmemiş). Devir notları `Docs/Handover/backend-engineer-20261115-0200.md`, `Docs/Handover/frontend-engineer-20261115-0300.md` ("Ek tur: taşma gerilemesi (üçüncü sürüm)" dahil), `Archive/frontend-engineer-20261115-0200.md` |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/f2pre4/build6`. Port 5201 Production, DB `.qa/f2t2/qa.db` (ön tur kopyası: tur 1 `qa.db` verisi + uzun veriler: 16 karakterlik tesis, hat ve makine kodu, 119 ve 64 karakterlik hat adı, 32 ve 64 karakterlik lot no, 16 haneli sicil no); istek günlüğü açık (`Hosting.Diagnostics` Information). Port 5202 Production, `.qa/f2t2/qa-empty.db` (tur 1 boş kopyası). Port 5203 Development (yalnız prototip) |
| QA önerisi | GEÇTİ (KRİTİK, YÜKSEK, ORTA, DÜŞÜK açık 0) |

Betikler ve ham çıktılar `.qa/f2t2/` altında (`g5.py`, `b35.py`, `b6.py`, `k3.js`, `align.js`, `kbd.js`, `det.js`, `ovf.js`, `g11.py`, `suites.sh`;
`*.out`, `*.txt`, `r-*.json`). Parolalar yalnız `auth.py` sürecinde üretildi ve kopya DB'lere özet olarak yazıldı; düz metin dosyaya yazılmadı.
Teslim testinden önce yerleşim ön turu (PL-35) üç kez koşuldu. İlk iki sürümün bulguları (taşan tablo rayın altına giriyordu; `.cell-text`
sığan değeri kırıyordu) koordinatöre ön tur bulgusu olarak döndü; son sürümde (`td.cell-long`) ön tur temiz çıktı.

### T2.1 İade edilen bulguların yeniden ölçümü (bölüm 10, tur 2 şartı)

| # | İstenen sonuç (bölüm 10) | Ölçüm | Sonuç |
|---|---|---|---|
| 1. B1 / G5 | Faz 2'nin 13, Faz 1'in 11 POST'u belirteçsiz; gövdede `Code` yok, `Code=T9`, `Code=200`, `code=404`: geçerli durum satırıyla 400. Geçerli belirteçle `/machines/edit/9999`, `/facilities/edit/9999` `Code=M01` ile 404. `/error/0`, `/200`, `/999` 404; `/error/403`, `/404`, `/500` doğru görünüm ve durum. K7, G10 POST satırları, veri yazımı 0 | Ham soket (`g5.py`). Faz 2 13 POST x 4 gövde: 52/52 `HTTP/1.1 400 Bad Request`. Faz 1: `/account/login` (oturumsuz ve oturumlu), `/account/first-login`, `/account/change-password`, `/account/change-pin`, `/users/create`, `/users/edit/5`, `/users/deactivate/5`, `/users/activate/5`, `/users/reset-pin/5`, `/users/reset-password/5` x 4 gövde: 44/44 400. `/account/logout` 4/4 302 `/account/login?reason=signed-out` (K11a bilinçli istisnası, `PhasePlan.md:249`, Faz 1 T-B15). Var olmayan kimlik: üç istek 404 "Sayfa bulunamadı". `/error/0`, `/200`, `/999`, `/399`, `/600` 404 "Sayfa bulunamadı"; `/error/400` 400, `/403` 403 "Yetkiniz yok", `/404` 404, `/500` 500 "Bir hata oluştu", `/error` 500. `ErrorController.cs:18-24` `[FromRoute] int code`, 400-599 dışı 404. K7/G10: Operatör, Denetçi ve Süper admin geçerli belirteçle 13 POST'ta 302 `/account/access-denied` (403). Tanım tabloları, kullanıcılar, makine ve tesis adları önce ve sonra aynı | GEÇTİ |
| 2. B2 / G3 / G11 | 1280, 1366, 1440, 1920'de uzun veriyle düğme ve rozet kırılması 0, `hScroll` false, satır en az 56 px. G11 dört liste, iki ray tablosu ve Faz 1 `/users` için 360, 768, 1024, 1440'ta beklenen imzayla aynı; 360 ve 390'da `hScroll` false | `det.js`, `ovf.js`, 1280/1366/1440/1600/1920. Ray yanı tablolar (`td.cell-long`): tablo genişliği her hücrede panel genişliğine eşit (592/678/752/912/1160 px), panelden taşma 0, rayla çakışma 0, örtülü "Düzenle" 0, sayfa kaydırması yok. Sığan değer tek satır: 16 karakterlik `HATKODUUZUN16XYZ` beş genişlikte 1 satır (td 153 px); 32 karakterlik lot 1440 ve üstünde, 64 karakterlik lot 1920'de, 99 karakterlik hat adı 1920'de 1 satır. Sığmayan değer yalnız kendi hücresinde kırılıyor: 64 karakterlik lot 1280-1600'de (gereken 539 px, kalan 148-468), 32 karakterlik lot 1280 ve 1366'da (gereken 282, kalan 166 ve 252), 119 karakterlik hat adı 1280 ve 1366'da kelime içinden, 1440 ve üstünde boşluklardan. Diğer hücrelerde kelime içi kırılma 0; düğme, rozet, çip çok satır 0. Tam genişlik listeler (`/materials` 32 karakterlik kod, `/users` 16 haneli sicil no, `/shifts`, `/machines`, `/facilities`): 35 ölçümde taşma, kelime içi kırılma ve çok satırlı düğme/rozet 0, en kısa satır 56 px. G11 (`g11.py`): 360/390/768/1024/1280/1366/1440/1920 açık, 360/1440/1920 koyu, 390/1440 reduced-motion ve Faz 1 sayfalarında 195 tablo ölçümünün 195'i beklenen imzayla aynı (`MekanikEval.txt` "Beklenen imza", Faz 1 tur 3 `/users`); 360 ve 390'da `hScroll` false | GEÇTİ |
| 3. B4 | `/materials/details/{id}` (32 karakterlik kod) 320x640 ve 640x400'de `hScroll` false; `/machines/retire/{id}` ve lot formu rayında aynı | `tz1`, `tz2`: `/materials/details/1`, `/2`, `/5`, `/facilities/details/*`, 15 form (`/machines/retire/7`, `/4`, `/material-lots/create?materialId=1`, `/material-lots/edit/10` dahil) 320x640 ve 640x400: 62/62 `hScroll` false, taşma 0 | GEÇTİ |
| 4. B3, B5 (PL-43) | Kaldırılmış makinede `/machines/retire/{id}` doğru bilgi, gönder düğmesi yok; `/machines/edit/{id}` salt okunur, kaydet ve kaldır bağlantısı yok; geçerli belirteçle düzenleme POST'u nedenli red, DB değişmez; liste eylemi "Görüntüle". Aktif makinede K2, K5 gerilemesi | `b35.py`, `kbd.js`. `/machines/retire/4`: "Makine 30.09.2026 12:00 tarihinde kullanımdan kaldırıldı." (`role=status`), gönder düğmesi 0, engel metni yok, tek bağlantı "Makine listesine dön" (`/machines?showRetired=true`). `/machines/edit/4`: başlık "Makine", aynı bildirim, kod ve ad `readonly`, tesis ve hat `disabled`, gönder düğmesi 0, kaldırma bağlantısı yok. POST `/machines/edit/4` (ad, kod, tesis, hat ile ve yalnız ad ile): 200, özet "Makine kaydedilemedi Kullanımdan kaldırılmış makine değiştirilemez.", DB satırı önce ve sonra aynı. POST `/machines/retire/4`: 200 "Makine zaten kullanımdan kaldırılmış.". `showRetired` listesinde M02, M09 ve yeni QAT2K5 satırının tek eylemi "Görüntüle" (`/machines/edit/{id}`). K2: `/machines/edit/1` kod M99 `Code` hatası, hat 2 ve başka tesisin hattı `LineId` hatası, ad değişikliği 302, DB'de kod ve hat aynı. K5: yeni makine eklendi, kaldırma ekranında PIN yok, POST 302, normal listede yok, `showRetired`'de "Kullanımdan kaldırıldı 14.11.2026 23:16", ikinci deneme nedenli red, satır `IsActive 0`, `RetiredAtUtc` dolu | GEÇTİ |
| 5. B6 | `/machines?page=3`, `/machines?page=99`, `/materials?page=99` "Bu sayfada kayıt yok." ve 1. sayfa bağlantısı; boş kopyada boş durum aynı | `b6.py`. Üç adres ve süzgeçli `/machines?page=99&facilityId=1`, `&showRetired=true`, `/materials?page=99&query=MLZ`: satır 0, "Bu sayfada kayıt yok.", "1. sayfaya dön" bağlantısı süzgeci koruyor (`/machines?page=1&facilityId=1`). `/machines?page=2` 15 satır, `/materials?page=2` 8 satır. Boş kopya (5202): `/machines` ve `?page=99` "Henüz makine yok" + "Makine ekle", `/materials` ve `?page=99` "Henüz malzeme yok" + "Malzeme ekle", `/facilities` "Henüz tesis yok", `/shifts` "Önce tesis ekleyin" (tur 1 ile aynı) | GEÇTİ |
| 6. B7 / K3 (PL-44) | JS kapalı akış: adres çubuğunda ve sunucu günlüğünde `__RequestVerificationToken` yok, kod ve ad korunuyor, yenilemede kayıt 0, eski hatla kaydet `LineId` hatası, doğru hatla 302 | `k3.js`, Chromium JS kapalı, 390x844. Yedek düğme `name=refresh value=true`, `formmethod`/`formaction` yok, görünür. Yenileme: `POST /machines/create?facilityId=1`, gövde `FacilityId,LineId,Code,Name,refresh,__RequestVerificationToken`; tesis 3 hatları geldi, kod M88T2 ve ad korundu, hata 0, DB'de M88T2 0. Tesis 1 seçili, tesis 3'ün hattıyla kaydet: "Seçilen hat bu tesise ait değil." (özet ve alan), tesis 1 hatlarıyla yeniden çizim, kod ve ad korundu. Doğru hatla kaydet: `/machines` "Makine eklendi.", DB'de M91T2 1. Gezinme adreslerinde belirteç 0; `run-5201.log` (tur 2'nin tüm istekleri) içinde `RequestVerificationToken` 0 | GEÇTİ |
| 7. B8 | 1440 ve 1920'de Tesis, Hat ve arama etiketleri ve alanlarının üst kenarı eşit | `align.js`: 1280, 1440, 1920'de `/machines` ve `/machines?facilityId=1` etiketler 238 px, alanlar 268 px (tek değer); 768 ve 1024'te iki satırlı ızgara, her satırda eşit | GEÇTİ |
| 8. B9, B10 | `includeId` grep 0; tesis seçilmemişken `T1-H1 - Hazırlık hattı`, seçiliyken `H1 - Hazırlık hattı`, JSON değişmedi | `includeId` `Src/**/*.cs` 0. `/machines` hat seçenekleri `T1-H1 - Hazırlık hattı`, `T1-H2 - Dolum hattı`; `/machines?facilityId=1` ve `/machines/create?facilityId=1` `H1 - Hazırlık hattı`; `/machines/lines?facilityId=1` `{"id":1,"label":"H1 - Hazırlık hattı"}` (tur 1 biçimi). `MachineService.cs` 269 satır; bölünmeme gerekçesi backend devir notunda (bölüm 10 madde 4 buna izin veriyor) | GEÇTİ |
| 9. Gerileme | G1, G2, §4 grep; Faz 2 ekranları ve Faz 1 `/users` için 360/768/1024/1440 mekanik eval, konsol 0 | T2.2 ve T2.3 | GEÇTİ |

### T2.2 Kabul kriterleri (yeniden işaretlenen ve gerileme)

| # | Sonuç | Kanıt |
|---|---|---|
| G5 | GEÇTİ | T2.1 madde 1; `Evidence/Faz-2/G5-Antiforgery.txt` "Tur 2" |
| G3 | GEÇTİ | §2 matrisi 524 ölçümde kırık hücre 0 (T2.3), §4 grep 0 ihlal, konsol 0 hata, DNA veri dili UYGUN (T2.5); B2 ve B4 kapandı |
| G11 | GEÇTİ | T2.1 madde 2; `MekanikEval.txt` "Tur 2" |
| K2, K5 | GEÇTİ | T2.1 madde 4 (aktif makine gerilemesi) |
| K3 | GEÇTİ | T2.1 madde 6; JS açıkken yedek düğme `/machines/create` 360, 768, 1024, 1440, 1920'de `hidden` ve görünmez (`r-tf.json`); JS açık bağlı seçim akışı bu turda yeniden yürütülmedi, `CascadeSelect.js` tur 2 devir notlarında değişen dosyalar arasında yok |
| K7, G10 | GEÇTİ | T2.1 madde 1; `YetkiMatrisi.txt` "Tur 2" |
| K8 | GEÇTİ | Sayfa dışı adres düzeldi (B6); sayfalama ve süzgeç tur 1 ile aynı (`/machines?page=2` 15, `/materials?page=2` 8 satır) |
| G1 | GEÇTİ | `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata. Tailwind derlemesinin QA çıktısı `wwwroot/css/app.css` ve yayındaki css ile bayt bayt aynı (`cmp`) |
| G2 | GEÇTİ | Policy grep (`AddPolicy`, `Policy =`, `IAuthorizationHandler`, `IAuthorizationRequirement`, sihirli `[Authorize(Roles = "`, `IsInRole("`) 0; her controller `[Authorize(Roles = ...)]` ya da `[AllowAnonymous]` (tek istisna `SessionControllerExtensions.cs`, controller değil); MediatR, AutoMapper, generic repository, `new ...Service(`, `GetService<` 0 |
| G7 | GEÇTİ | Yeni migration yok (`Data/Migrations/` tur 1 ile aynı iki migration, ikisi de `DbChanges.md` indeksinde); `Up()` satır 12-171, `DropTable` yalnız `Down()` içinde (172 ve sonrası); `dotnet ef migrations has-pending-model-changes --configuration Release --no-build`: "No changes have been made to the model since the last migration."; `Program.cs` içinde `Migrate()` 0 |

Diğer kriterler tur 1 sonucunda kalır. Kabul özeti tur 2 sonunda: K1-K9 9/9, ortak kriterler (G1-G7, G10, G11) 9/9; toplam 18/18.

### T2.3 Mekanik eval (§3) ve responsive matris

524 ölçüm (`Evidence/Faz-2/MekanikEval.txt` "Tur 2"): 10 liste adresi, 6 ayrıntı, 3 sayfa dışı adres ve `/users` 360, 390, 768, 1024, 1280,
1366, 1440, 1920 açık; 15 form 360, 768, 1024, 1440, 1920; liste, ayrıntı ve formlar 360, 1440, 1920 koyu, 390 açık ve 1440 koyu
reduced-motion, 320x640 ve 640x400 (%200); Faz 1 sayfaları (`/`, `/users`, `/users/create`, `/users/edit/5`, `/users/deactivate/5`,
`/account/security`) 360, 768, 1024, 1440 açık ve 1920 koyu; Süper admin, oturumsuz, JS kapalı (`/machines/create`, `/machines/edit/7`,
`/machines?facilityId=1`) ve beş prototip (Development).

| Ölçüm | Sonuç |
|---|---|
| h1 = 1, atlama bağlantısı, imgNoAlt/imgNoSize/inputNoLabel/iconBtnNoName | 524/524 temiz |
| emdash, hScroll, taşma, dış kaynak, yazı tipi | 0, 0, 0, 0; Hanken Grotesk ve Archivo `fonts.check` 524/524 |
| 40 px altı etkileşimli hedef | 0 (onay kutuları etiket içinde, tur 1 ile aynı istisna) |
| AA kontrast | altı 0, en düşük 5,28:1 |
| Üst bar, 768 altı gövde yazısı | en çok 64 px; 16 px altı 0 |
| reduced-motion animasyon | 0 (62 ölçüm) |
| Konsol | hata ve uyarı 0 |
| JS kapalı yedek düğme | `/machines/create` 360: 292x76, 768 ve 1440: 311x56, görünür |

Yapışkan tablo başlığı (ön tur ölçümü, aynı yayın ve veri): 1280 ve 1440'ta `/machines`, `/users`, `/shifts` ve iki ayrıntı sayfasında başlık
kaydırmada 64 px'te duruyor, o noktada en üstteki öğe TH.

Not (bulgu değil): 640x400 (%200, 1280 px eşdeğeri) liste kartlarında ölçüm `Ad:1@1` ve 1. satırda boşluk gösteriyor; Ad hücresi `max-w-xs`
(320 px) ile iki izden (2x282 px) dar. Değer tur 1 `.qa/f2/r-lists.json` ile birebir aynı; G11 genişliği değil, gerileme değil.

### T2.4 Klavye turu (değişen kısım)

- `/machines/retire/4` (kaldırılmış): `main` içinde tek odaklanabilir öğe "Makine listesine dön", halka 3 px.
- `/machines/edit/4` (kaldırılmış): Tab sırası Kod, Ad (ikisi `readonly`), "Makine listesine dön", halka 3 px; menü bağlantısına tıklayınca
  `beforeunload` diyaloğu 0 ve sayfa değişiyor. Aktif `/machines/edit/7`'de ad değişince menü tıklaması diyalog 1 (tur 1 davranışı korunuyor).
- Not (bulgu değil): kaldırılmış makinenin formunda `data-unsaved-guard=""` öznitelik olarak hâlâ yazılıyor (`Views/Machines/Form.cshtml:20`;
  `<form>` etiket yardımcısı null değeri boş metne çeviriyor). Alanlar değiştirilemediği için gözlenen etkisi yok (diyalog 0); devir notundaki
  "data-unsaved-guard yok" ifadesiyle uyuşmuyor.
- Diğer klavye davranışları tur 1 sonucunda kalır.

### T2.5 DNA uygunluğu (değişen maddeler)

- Veri dili: UYGUN. 1280 ve üstünde ray yanı tablolarda tek uzun sütun (`td.cell-long`) kalan alanı alıyor, diğer hücreler içerik
  genişliğinde ve tek satır; sığmayan değer yalnız kendi hücresinde kırılıyor; düğme, rozet ve çip tek satır; tablo panel genişliğini aşmıyor.
  Kart modu imzaları değişmedi, 1280 ve üstünde satır en az 56 px.
- Sayfa iskeleti: UYGUN. Kaldırılmış makine ekranları ana kolon + ray, yalnız bilgi ve geri dönüş.
- Diğer maddeler tur 1 sonucunda kalır. Anti-slop: değişiklik yok.

### T2.6 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-2/` (10 dosya):

- `G5-Antiforgery.txt`, `YetkiMatrisi.txt`, `KabulKriterleri.txt`, `MekanikEval.txt`, `OverflowWrap.txt`: sonlarına "Tur 2" bölümü eklendi.
- `HatTablo-1280-Tur2.png` (yeni): `/facilities/details/1` 1280 px; 16 karakterlik hat kodu tek satır, 119 karakterlik hat adı yalnız kendi
  hücresinde kırılıyor, tablo panel içinde, "Düzenle" düğmeleri tam görünür (Read ile açıldı).
- `DetayTablo-1280.png`, `LotTablo-1440.png`, `MalzemeRay-320.png`, `KaldirilmisMakine-Retire.png`: tur 1 bulgu görüntüleri, değişmedi.

Ön tur görüntüleri `.qa/f2pre4/`, `.qa/f2pre5/`, `.qa/f2pre6/` altında.

### T2.7 Yeni bulgular

yok

Kod denetimi (bulgu değil): §7 bu turda süre hesaplayan yeni ya da değişen metot yok (`MachineRetirement` yalnız `ITurkeyClock.ToLocal`
ile gösterim anı üretiyor). §8 değişen dosyalarda (`ErrorController.cs`, `MachineService.cs`, `MachineRetirement.cs`, `MachinesController.cs`,
`Styles/app.css`, `Views/Machines/**`, `Views/Materials/**`, `Views/Facilities/Details.cshtml`) emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager`
adı 0. SOLID: yenileme dalı (`MachinesController.cs:49-53`) yalnız model durumunu temizleyip formu yeniden çiziyor, iş kuralı yok;
kaldırılmış makine reddi servis katmanında (`MachineService.cs:221-223`).

### T2.8 Kapsam dışı / yapılamayan (tur 2)

- Yalnız Chromium; Firefox, Safari ve gerçek cihaz test edilmedi. %200 yakınlaştırma eşdeğer CSS genişliğiyle ölçüldü (tur 1 ile aynı).
- 800x360 yatay hücre ve koyu temada 768/1024 hücreleri yeniden ölçülmedi; değişen CSS 1280 ve üstü tablo kuralları ile `.kv` (ray), bunlar
  ilgili genişliklerde ölçüldü.
- Bölüm 8'deki tur 1 maddeleri (BR-35 engel listesi, `FindShiftAt` çalışma zamanı, eşzamanlı ekleme, ekran okuyucu, yükleniyor iskeletinin
  gerçek gecikmeyle izlenmesi) aynen geçerli; bu turda yeniden denenmedi.
- QA dışı süreç PID 2580'e (port 5181) dokunulmadı. QA'nın açtığı 5201, 5202, 5203 süreçleri ve playwright oturumları kapatıldı.

### T2.9 Sonuç

İade edilen on bulgunun (B1-B10) tamamı kapandı; G3, G5 ve G11 geçti, kabul kriterleri 18/18. Tur 2'de yeni bulgu yok. KRİTİK, YÜKSEK, ORTA ve
DÜŞÜK açık bulgu 0; QA önerisi GEÇTİ. Karar analistte.

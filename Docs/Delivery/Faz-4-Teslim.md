# Faz 4 Teslim Raporu: Kayıt açma ve makine kilidi

| Alan | Değer |
|---|---|
| Faz | 4 |
| Tarih | 2026-11-15 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `64b9ad3`; Faz 4 kodu, migration `20261115021022_AddCleaningRecordsAndEventLog`, `DemoData.sql` Faz 4 bölümü commit'lenmemiş). Devir notları `Docs/Handover/backend-engineer-20261115-1000.md`, `Docs/Handover/frontend-engineer-20261115-0800.md` (tuple 500, PL-64 ve "Run adım satırı hizası (G12)" bölümleri); `DB-2026-006` |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/f4/build`. Port 5401 Production, DB `.qa/f4/qa.db` (`App_Data/vaka.db` SQLite yedekleme API'siyle WAL dahil tutarlı kopya + `DemoData.sql`), `EventChain__HmacKey` yalnız sunucu sürecinde üretilen rastgele 32 bayt (hiçbir yere yazılmadı). Port 5403 Development, `.qa/f4/qa-dev.db`, user-secrets anahtarı (HMAC'in bağımsız doğrulaması için). Port 5402 Production, `.qa/f4/empty.db` (yalnız migration, boş durumlar). `.qa/f4/fresh.db` sıfırdan migration + `DemoData.sql` (K21). Chromium (playwright-cli) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2: B1 kapandı, açık bulgu yok; tur 1: KALDI) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-15); tur 1: İADE |

## 1. Kapsam

- DB: `CleaningRecords`, `RecordPhases`, `RecordSteps`, `StepParticipants`, `RecordMaterials`, `CleaningEvents`, `ProcessedCommands`, `NumberSequences`; kısmi unique kilitler (makine, sahip, çalışılan adım); olay günlüğü ve sayaç tetikleyicileri; WAL; demo tip PLN3 (3 faz, 5/3/4 adım, 15'er dk) (DB-2026-006).
- BE: olay yazıcı (HMAC-SHA256 zinciri, `EventChain:HmacKey` başlangıç doğrulaması), komut tekrarı (`ProcessedCommands`), kayıt no sayacı, yazma kilitli işlem, kayıt açma (BR-01/02/03/06/08/12/17/23/37/45, K-18), SH-13 kilit bitişi ve zaman aşımı, malzeme ekleme ve çıkarma, engel sağlayıcıları (BR-35/36), İşlerim, pano, Operatör ana sayfa yönlendirmesi, süre biçimleyici, F1-A2.
- FE: E-13 İşlerim, E-14 Yeni kayıt (zincirli seçim, LotPicker, JS kapalı yedek akış), E-15 Yürütme ekranı Açıldı hali, E-16 Makine panosu, `CommandForm.js`, `LotPicker.js`, `TypePicker.js`, menü; ön tur düzeltmesi (adım satırı `min-h-11`).

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Hat tesise, makine hatta göre; pasif ve kaldırılmış makine seçilemez; tip listesinde yalnız eşli, yürürlükteki planlı tipler | GEÇTİ | `Evidence/Faz-4/KabulKriterleri.txt` K1: `/records/lines?facilityId=1` H1, H2; pasif tesis T2 boş ve tesis listesinde yok; `/records/machines?lineId=2` kaldırılmış M02'yi listelemiyor; dolu makineler `isAvailable:false` ve neden metniyle; makine 1 için PLN ve PLN3 (ACL yok), eşlemesiz M09 boş; JS açık zincirli seçimde dolu makineler `disabled` (`JsKapaliAcik.txt`) |
| K2 | Değiştirilmiş istekte kayıt ve olay yok | GEÇTİ | `KabulKriterleri.txt` K2: başka hattın makinesi, eşlemesiz tip, acil tip, kaldırılmış makine, pasif tesisin makinesi, başka tesisin hattı: 200 nedenli ret, kayıt +0, olay +0 |
| K3 | Planlı iş emri boşken kayıt açılmaz | GEÇTİ | Boş ve yalnız boşluklu iş emri "İş emri numarasını girin.", kayıt +0 |
| K4 | Yürütme ekranı; kayıt no biçimi, aynı makinede sıra +1; durum Açıldı; saha ref boş; iş emri ve açıklama görünür | GEÇTİ | `T1-H1-M03-P2026-1`, `T3-H1-A2-P2026-1` ...; A1'de zaman aşımından sonra ikinci kayıt `T3-H1-A1-P2026-2`; durum 1, `FieldBookRef` NULL, Run'da "Saha defteri Atanmadı", iş emri ve açıklama metni görünüyor |
| K5 | Kilit bitişi vardiyaya göre; vardiyasız tesis ve boşlukta açılış + süre; gece yarısını geçen vardiya; vardiya değişince açık kaydın bitişi değişmez | GEÇTİ | Gece vardiyası 23:00-07:00 içinde 05:46 açılış, 120 dk: bitiş 07:00 (en erken), 60 dk: açılış + 60. Vardiyasız T3: açılış + 120. Sınır: S1 [04:50, 05:50), S2 [05:50, 06:00); 05:50:00,42'de açılış bitişi 06:00 (S2, başlangıç dahil). T6 tek vardiya [04:51, 05:51); 05:51:00,26'da açılış bitişi açılış + 60 (bitiş hariç, boşluk). S2 bitişi 06:30 yapılınca açık kaydın bitişi değişmedi |
| K6 | 121 red, 120 kabul | GEÇTİ | 121 ve 0 "Kilit süresi 1 ile 120 dakika arasında olmalı."; 120 ile iki kayıt açıldı |
| K7 | Dolu makinede sahip, durum, açılış saatiyle ret; eşzamanlı iki açmada tek kayıt | GEÇTİ | Sıralı: "Makinede açık kayıt var. Sahip: QA Operatör 5, durum: açıldı, açılış: 15.11.2026 05:46". Eşzamanlı iki tur (A4, B3; iki operatör): her turda biri 302, diğeri nedenli 200; kayıt +1, olay +1, 500 yok, günlükte `fail`/`crit` 0 |
| K8 | Aktif kaydı olan operatör başka makinede açamaz; bağlantılı mesaj; eşzamanlıda tek kayıt | GEÇTİ | "Açık kaydınız var: T3-H1-A2-P2026-1 (açıldı)..." ve "Kaydı görüntüle" `/records/run/6`; aynı operatör iki makineye eşzamanlı iki turda tek kayıt, reddedilen istek numara tüketmedi (sayaçlar) |
| K9 | Kilit bitişi geçmiş kayıt panoda zaman aşımı, makine Boş; olay anı kilit bitişi; malzeme reddi; başkası açabilir | GEÇTİ | 1 dk kilitli kayıt: kilit bitişinden sonra istekte durum 5, `ClosedAtUtc` = `LockExpiresAtUtc`, olay `RecordTimedOut` aktörsüz; pano "Boş, Kayıt açılabilir."; malzeme ekleme "Kilit süresi doldu, kayıt Zaman aşımına düştü. Malzeme değiştirilemez."; başka operatör aynı makinede 302 |
| K10 | SKT geçmiş lot uyarılı görünür; gerekçesiz ekleme ret; gerekçeli satır işaretli | KALDI | JS açık ve HTTP'de geçti: LotPicker sonucu "SKT geçmiş", gerekçe alanı zorunlu çıkıyor; gerekçesiz "SKT geçmiş lot için gerekçe girin."; Run'da satır "SKT geçmiş" rozeti ve gerekçeyle. JS kapalı yeni kayıtta (PL-64) seçili lot satırı ve gerekçe alanı görünmüyor, gerekçesiz gönderim sessizce engelleniyor, gerekçeli gönderim kayıt açmıyor (B1) |
| K11 | Malzeme ekle ve çıkar yalnız sahibin PIN'iyle; başkası ret; ekleme kilidi uzatmaz | KALDI | Sahip ve PIN kuralları geçti: başka operatör ekleme ve çıkarma 403, yanlış PIN "PIN hatalı." ve sayaç +1, doğru PIN 302, kilit bitişi değişmedi, aynı lot tekrar reddi; JS kapalı Run'da `?q=`, ekleme, çıkarma ve PendingMaterial çalışıyor. PL-64'ün "JS kapalı tarayıcıda lot seçilip kayıt açılır" kısmı yeni kayıtta karşılanmıyor (B1) |
| K12 | Aynı CommandId ile tek kayıt tek olay | GEÇTİ | İki gönderim aynı `/records/run/7`, kayıt +1, olay +1, `ProcessedCommands` tek satır; aynı CommandId farklı gövdeyle de ilk sonuç |
| K13 | Çevrimdışı "işlem yapılmadı"; çevrimiçi yeniden deneme tek kayıt | GEÇTİ | `context.setOffline(true)`: istek 0, "İşlem yapılmadı, bağlantı yok. Bağlantı gelince aynı düğmeyle yeniden deneyin, işlem tek kez yapılır.", düğme etkin, CommandId aynı; çevrimiçi tek POST, aynı CommandId, tek kayıt tek olay |
| K14 | Olay günlüğünde SQL UPDATE ve DELETE ret | GEÇTİ | Kopya DB: UPDATE (payload, özet) ve DELETE "Olay günlüğü değiştirilemez."; sayaç silme ve azaltma ret, artırma serbest |
| K15 | Olay önceki özeti taşır; anahtar repoda yok; anahtarsız başlamaz; ortam değişkeniyle çalışır | GEÇTİ | Anahtarsız, 16 bayt ve Base64 olmayan değerde süreç başlamadı, günlükte `crit` ve neden (değer yazılmadan). Ortam değişkeniyle Production başladı. Development'ta 4 olayın HMAC-SHA256 zinciri user-secrets anahtarıyla bağımsız Python hesabıyla birebir aynı (kullanıcı DB'sinden gelen kayıt 1 dahil); Production'da 23 olayda önceki özet bağlantısı ve biçim doğru, sıra boşluksuz, payload'da PIN yok. `appsettings*.json`, kaynak ve repoda anahtar değeri yok |
| K16 | SH-13 ve zaman aşımı formülleri XML yorumunda | GEÇTİ | `IDurationCalculator.cs:93-109`, `DurationCalculator.cs:120-135`: "Kilit bitişi = en erken (Açılış + KilitSüresi, açılış anını içeren vardiyanın bitişi)", "Geçti: Şimdi >= Kilit bitişi"; kod `shift.EndAt < byLock ? shift.EndAt : byLock`, `now >= lockExpiresAt` |
| K17 | Kayıt Açıldı iken yeni versiyon yayınlansa da versiyonu değişmez | GEÇTİ | PLN3 v2 yayınlandı; kayıt 4 `ProcedureVersionId` 3 (v1) kaldı, Run "versiyon 1"; yeni kayıt formu v2 sunuyor |
| K18 | Pano Operatör, Yönetici, Denetçi'de; Süper admin 403; durum metinle; zaman aşımına düşmesi gereken kayıt Açıldı görünmez | GEÇTİ | `YetkiMatrisi.txt`: 200/200/200, Süper admin 302 > 403; durum metinleri (Boş, Açıldı); zaman aşımı kaydı Boş; kayıt bağlantısı yalnız Operatör'de |
| K19 | Aktif kayıtlı makine kaldırılamaz, tip pasifleştirilemez, sahip pasifleştirilemez; ekranlar engelleyen kaydı listeler | GEÇTİ | Makine 14 retire: kayıt no listede, gönder düğmesi yok, POST nedenli ret; PLN3 pasifleştirme `IsActive` hatası ve 13 engelleyen kayıt; op3 pasifleştirme ekranı açık işi listeliyor, PIN alanı ve düğme yok, POST ret |
| K20 | Operatör `/` -> `/my-work`; aktif kayıt kartı; Yönetici `/records/create`, `/my-work` 403 | GEÇTİ | `YetkiMatrisi.txt`; İşlerim kartında kayıt no, makine, durum ve kilit bitişi (mekanik eval sayfaları) |
| K21 | Demo PLN3 (3 faz, 5/3/4 adım, 15 dk), formda seçilir, ikinci uygulamada sayı aynı | GEÇTİ | `fresh.db`: PLN3 Yürürlükte v1, yayınlayan NULL, fazlar 900 sn, 5/3/4 adım, makine 1,2,3,5 eşli; ikinci uygulamada 15 tablonun sayısı aynı; yeni kayıt formunda PLN3 seçenek |
| T-B14 | Süre biçiminde rakam mono, birim sans | GEÇTİ | Run "15dk": rakam JetBrains Mono, birim Hanken Grotesk (`_Duration.cshtml:3`) |
| T-B22 | 1024-1279'da başlık değerleri en çok iki satır; kayıt no ve saha ref bölünmez | GEÇTİ | 1024, 1100: Yer 2, Tip 2, diğerleri 1 satır; 1279 ve 1280 tümü 1; açılış ve saha defteri `nowrap` |
| F1-A1 | Açık işi olan kullanıcının pasifleştirme ekranı işi listeler, düğme yok, bağlantı görüntüleyene göre | GEÇTİ | Yönetici ve Süper admin: kayıt no ve "Kullanıcının açık işi var.", PIN ve düğme yok, run bağlantısı yok |
| F1-A2 | PIN ve iş eylemi aynı işlemde; iş reddinde kısmi yazma yok; yanlış PIN sayacı yazılır | GEÇTİ | Doğru PIN + açık iş reddi: kullanıcı aktif, sayaç 0; PIN sıfırlamada yanlış PIN sayaç 1 ve hedef PIN değişmedi, doğru PIN 302, sayaç 0, hedef sıfırlandı |
| F2-A1 | Aktif kayıtlı makinenin kaldırma ekranı engelleyeni listeler | GEÇTİ | K19 makine satırı |
| F2-A2 | `FindShiftAt` gece yarısı ve sınırlarda doğru | GEÇTİ | K5 satırı (başlangıç dahil ve bitiş hariç, saniyenin onda/beşte birinde ölçüldü) |
| F3-A1 | Aktif kayıtlı tipin pasifleştirilmesi `IsActive` hatası ve engelleyen liste | GEÇTİ | K19 tip satırı; Yönetici'ye bağlantı verilmiyor |
| G1 | `-warnaserror` 0/0; `build:css` | GEÇTİ | Bölüm 3 |
| G2 | §4 grep 0; öznitelikler; rol metni | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi, §4, konsol, DNA | GEÇTİ | 154 eval ölçümü ve 120 G12 hücresinde kırık yerleşim yok, konsol 0, §4 0, DNA bölüm 5. JS kapalı işlev hatası B1'de |
| G4 | Operatör ekranları 360'ta kaydırmasız, birincil düğme 44 px, durum metinle | GEÇTİ | 360'ta `hScroll` false (4 ekran, açık ve koyu); birincil düğmeler 44/48/56 px; rozetler ikon + metin |
| G5 | Belirteçsiz POST 400 | GEÇTİ | Üç Faz 4 POST'u Operatör oturumunda gövdesiz, `Code=T9`, `Code=200` ile 400 |
| G5a | Her POST formu kendi belirtecini taşır | GEÇTİ | 19 sayfa, 79 POST formu, belirteçsiz 0 (ön tur) |
| G5b | `bool` onay kutuları | GEÇTİ | Faz 4 görünümlerinde `bool` onay kutusu yok; `Views/**` gizli `false` grep 0 |
| G6 | em/en-dash 0 | GEÇTİ | `Src` ve `Docs` 0; eval'de `emdash` false |
| G7 | `Up()` Drop yok | GEÇTİ | Bölüm 3 |
| G8 | Controller ve görünümde süre aritmetiği yok | GEÇTİ | `Controllers/`, `Views/` içinde `TimeSpan`, `.Subtract(`, `AtUtc -`, `AddMinutes` ... 0 eşleşme |
| G9 | Aynı CommandId tek etki tek olay | GEÇTİ | K12, K13 |
| G10 | Süper admin tüm Faz 4 adreslerinde 403, menüde yok | GEÇTİ | `YetkiMatrisi.txt`: 11 GET ve 3 POST 302 > 403; menüde bağlantı yok |
| G11 | Yerleşim imzası | GEÇTİ | Liste tablosu yok; pano ızgarası 360'ta 1, 768 ve 1024'te 2, 1440'ta 3 sütun (beklenen 1/2/3); form-detay iskeleti; Faz 1-3 dokuz tabloda 36/36 aynı |
| G12 | Hizalama kusuru 0 | GEÇTİ | 120 hücre genel ölçüm kusur 0; ön tur bulgusu (adım satırı) düzeltildi: sarılmayan satırda damga ile başlık ve rozet merkez farkı 0 |

G9'dan başka bu fazda yeni ortak kriter yok. Kabul özeti: K1-K21 19/21 (K10, K11 KALDI); T-B14, T-B22 2/2; F1-A1, F1-A2, F2-A1, F2-A2, F3-A1 5/5;
ortak kriterler (G1-G12, G5a, G5b) 14/14; toplam 40/42.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental -o .qa/f4/bchk`: 0 Uyarı, 0 Hata; `dotnet publish -c Release` başarılı |
| CSS | `npm run build:css` | Tailwind QA çıktısı `wwwroot/css/app.css` ile bayt bayt aynı (`cmp`) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme. `MyWorkController` ve `RecordsController` `Roles.Operator`, `MachineBoardController` `Roles.RecordReaders`; özniteliksiz iki dosya controller değil. MediatR, AutoMapper, generic repository, `new ...Service(` 0; `Program.cs` içinde `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: tablolar `CleaningRecords`, `RecordPhases`, `CleaningEvents`, `ProcessedCommands`, `NumberSequences`; index `IX_CleaningRecords_OneActivePerMachine`; tetikleyici `TR_CleaningEvents_NoUpdate`; servisler `RecordOpener`, `CleaningEventWriter`, `HmacEventDigest`; JS `CommandForm.js`, `LotPicker.js` |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (tam tablo; antiforgery'siz POST formu 0, gizli `false` 0) |
| Mekanik eval | `UiDelivery.md §3` | 154 ölçüm temiz (Faz 4 ekranları 360/768/1024/1440 açık ve koyu, 1280, 1920, 390 reduced-motion, 320x640, 640x400; boş kopya; Faz 1-3 gerilemesi 52), en düşük kontrast 5,28:1, konsol 0 (`MekanikEval.txt`) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Dört migration; Faz 4'ün `AddCleaningRecordsAndEventLog` (commit'lenmemiş) `DbChanges.md:12` DB-2026-006 DOĞRULANDI, Eklemeli. `Up()` satır 12-499 içinde Drop/Alter/Update/Delete 0. `has-pending-model-changes`: "No changes have been made to the model since the last migration." Sıfırdan uygulama başarılı |

Ek denetimler:

- §7: SH-13 ve BR-26 formülleri arayüz ve uygulamada XML özetinde, kodla uyumlu (K16). Önceki metotlar değişmedi.
- §8: Faz 4 controller ve servislerinde `#region`, `Helper`/`Utils`/`Manager`, emoji 0.
- SOLID: controller'lar bağlama ve yönlendirme (`RecordsController.cs:42-66`); kurallar `RecordOpenValidator`, `RecordOpener`, `RecordMaterialService`, `IRecordTimeouts`;
  `RecordOpener.cs` 256 ve `RecordMaterialService.cs` 201 satır (§2 eşiği; gerekçe backend devir notunda, bulgu değil).

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Denetçi (3001); Editor = Operatör (sahip op3 ve sahip olmayan op4); Admin = Süper admin (0001); Yönetici (1001) parantez içinde.
Tam tablo `Evidence/Faz-4/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/my-work`, `/records/create`, `/records/lines`, `/records/machines`, `/records/cleaning-types`, `/records/lots` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 | 302>403 |
| GET `/records/run/4` (op3'ün kaydı), `/records/run/9999` | 302 (giriş) | 302>403 | 200 (sahip olmayan salt okunur 200), 404 | 302>403 |
| GET `/machine-board`, `?facilityId=1` | 302 (giriş) | 200 (Yönetici 200) | 200 | 302>403 |
| GET `/` | 302 (giriş) | 200 (Yönetici 200) | 302 > `/my-work` | 302 > `/users` |
| POST `/records/create` (geçerli belirteç) | 302 (giriş) | 302>403 | 200 nedenli ret (op4 açık kaydı var); belirteçsiz 400 | 302>403 |
| POST `/records/add-material/4`, `/records/remove-material/4` | 302 (giriş) | 302>403 | sahip olmayan 403; belirteçsiz 400 | 302>403 |

Menü: Operatör İşlerim, Yeni kayıt, Makine panosu; Yönetici ve Denetçi yalnız Makine panosu; Süper admin hiçbiri. Reddedilen POST'lardan sonra kayıt, olay ve malzeme sayısı aynı.

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/my-work` (aktif kayıtlı, boş) | Temiz, kaydırma yok | Temiz | Ray yanında | Temiz | Temiz | Temiz | Temiz | Temiz | Bağlantılar halkalı |
| `/records/create` (dolu, dolu makineler, JS kapalı yedek, boş kopya) | Temiz, birincil 56 px | Temiz | Ray yanında | Temiz | Temiz | Temiz | Temiz | Temiz | Tab sırası alanlarla, halka 3 px; boş gönderimde altı alan hatası, odak `FacilityId`. JS kapalı lot akışı B1 |
| `/records/run/{id}` (Açıldı, malzemeli, SKT geçmiş satır) | Temiz; adım satırı blok damga üstünden | Adım satırı ortalı (fark 0) | Ortalı (sarılmayan), T-B22 en çok 2 satır | Ortalı | Temiz | Temiz | Temiz | Temiz | PIN göz düğmeleri hizalı; CommandForm ret odağı özet kutusunda |
| `/machine-board` (Operatör, Yönetici, Denetçi; boş kopya) | 1 sütun | 2 sütun | 2 sütun | 3 sütun | Temiz | Temiz | Temiz | Temiz | Süzgeç formu JS'siz çalışıyor |
| Gerileme: Faz 1-3 listeleri, tip ve makine düzenleme, pasifleştirme, güvenlik | Beklenen imza | Beklenen imza | Beklenen imza | Tablo | ölçülmedi | G12 temiz | ölçülmedi | ölçülmedi | Faz 1-3 akışı |

Dört durum: boş (İşlerim "Açık işiniz yok" + "Yeni kayıt aç"; pano "Gösterilecek makine yok"), yükleniyor ("Yükleniyor…" iskeleti İşlerim ve panoda),
hata (yeni kayıtta özet + alan altı, Run'da `role=alert` özet, CommandForm çevrimdışı iletisi), dolu. Konsol 0.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Kayıt no, saat ve süre rakamları mono, birimler sans (T-B14).
- Palet: UYGUN. Durum rozetleri ikon + metin + kenarlık ("Hazır" açık zemin, "Bekliyor" kesikli, "SKT geçmiş" hata tonu); AA altı 0.
- Köşe ölçeği: UYGUN. 2 px köşe ve kenarlık, gölge 0.
- Gezinme arketipi: UYGUN. Operatör alt çubuğunda İşlerim, Yeni kayıt, Makine panosu; Yönetici ve Denetçi İzleme grubunda pano.
- Sayfa iskeleti: UYGUN. Yürütme ana kolon adımlar, ray kayıt bağlamı (malzeme); form ana kolon + ray; pano tek süzgeç + kart ızgarası, dört eşit KPI yok.
- Veri dili: UYGUN. Durum metinle; boş kesikli kutu ve eylem; yükleniyor durağan iskelet.
- Hareket imzası: UYGUN. Yeni animasyon yok.
- İmza bileşen (kayıt şeridi): Açıldı hali faz hücreleri ve numaralı adım damgaları; adım satırı hizası düzeltildi (`AdimSatiri-768.png`). Çalışan adım Faz 5.
- Anti-slop: temiz.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | YÜKSEK | JS kapalı yeni kayıtta lot akışı (PL-64, K10, K11) çalışmıyor. (a) "Ekle" ve "Çıkar" düğmeleri `formaction="/records/create?refresh=true"` ile gönderildiği için sayfa adresi `?refresh=true` olarak kalıyor; formun `action` özniteliği olmadığından sonraki "Kaydı oluştur" da `/records/create?refresh=true` adresine gidiyor ve backend sorgudan `refresh` okuduğu için yalnız yenileme yapılıyor: kayıt açılmıyor, ileti yok (lot eklendikten sonra JS kapalı kayıt açmak imkânsız). (b) Seçili lot satırlarının kapsayıcısı JS kapalıyken gizli: `_LotPicker.cshtml:13` `data-lot-js="@(isMulti ? null : "true")"` `data-*` özniteliği olduğu için null'da kaldırılmıyor, `data-lot-js=""` yazılıyor ve `app.css:801` `html:not([data-js]) [data-lot-js]` onu gizliyor. Kullanıcı seçtiği lotu, gerekçe alanını ve "Çıkar" düğmesini görmüyor; SKT geçmiş lot gerekçesiz eklenince gizli `required` gerekçe alanı formu sessizce engelliyor (istek gönderilmiyor, görünür ileti yok). JS açık akış ve Run sayfasının JS kapalı akışı doğru | `Evidence/Faz-4/JsKapaliAcik.txt` (nojs2, nojs3), `JsKapali-YeniKayit.png` (lot eklenmiş, seçili satır yok, eklenen lot hâlâ "Ekle" listesinde); `Views/Records/Create.cshtml:29`, `Views/Shared/_LotPicker.cshtml:12-13`, `Views/Shared/_LotFallback.cshtml:41`, `Views/Shared/_LotRow.cshtml:16`, `Controllers/RecordsController.cs:42-50` | frontend-engineer | (a) Yeni kayıt formuna `action="/records/create"` (ya da "Kaydı oluştur" düğmesine `formaction="/records/create"`); Ekle/Çıkar `?refresh=true` kalabilir. (b) `data-lot-js` ve `data-lot-required` özniteliklerini `@if` ile yalnız gerektiğinde yazmak (çoklu modda hiç yazmamak). Yeniden test: JS kapalı tarayıcıda malzeme zorunlu tipte lot ara, ekle (seçili satır görünür), çıkar, SKT geçmiş lotta görünür gerekçe alanı ve gerekçesiz gönderimde görünür hata, gerekçeyle "Kaydı oluştur" 302 ve Run'da işaretli satır; JS açık akış gerilemesi | KAPANDI (tur 2): JS kapalı 360 ve 1440'ta lot süz/ekle/çıkar, seçili satır görünür, SKT gerekçesi görünür alanda, "Kaydı oluştur" 302 ve DB doğru; JS açık gerileme yok (Tur 2, T2.1) |

## 7. Geçenler

- Kayıt açma kuralları (BR-01/02/03/06/08/12/45, K-18), kilit bitişi (SH-13; gece yarısı, vardiyasız, sınır anları), zaman aşımı (BR-26, BR-46) ve versiyon bağlılığı (BR-03).
- Eşzamanlılık: makine ve sahip kilidi iki operatör ve aynı operatörün iki makinesi için iki tur, 500 yok, numara tüketilmiyor.
- Olay zinciri: HMAC bağımsız hesapla doğrulandı; anahtar doğrulaması başlangıçta; tetikleyiciler; aynı CommandId tek etki; çevrimdışı yeniden deneme.
- Malzeme: sahip ve PIN, SKT gerekçesi, kilit uzamaz, JS kapalı Run yolu (`?q=`, ekle, çıkar, PendingMaterial seçili radyo ve boş PIN).
- Engeller ve devredenler: K19, F1-A1, F1-A2, F2-A1, F2-A2, F3-A1.
- Yetki matrisi, G10, G5, G5a; menü role göre.
- Yerleşim: ön tur bulgusu (adım satırı hizası) düzeltildi ve ölçüldü; G11, G12, G4, mekanik eval, §4 temiz; Faz 1-3 gerilemesi yok.

## 8. Kapsam dışı / yapılamayan

- Vardiya sınırı saniyenin onda/beşte birinde ölçüldü; tam sınır anında (0 ms) istek üretilemez. Hesap kodu (`DurationCalculator.cs:120-125`) ve `FindShiftAt` kuralı okunarak destekleniyor.
- Malzeme zorunlu tipte malzemesiz kayıt açılabiliyor; BR-11 ilk adımın başlatılmasını engeller (Faz 5), bu fazda açılış reddi beklenmiyor. Ölçüldü (kayıt 16), bulgu sayılmadı.
- Run `?q=` listesinde kayda zaten eklenmiş lot da listeleniyor; seçilince "Bu lot kayda zaten eklenmiş." reddi geliyor. Bulgu sayılmadı.
- Yalnız Chromium; gerçek cihaz ve diğer tarayıcılar test edilmedi. %200 eşdeğer CSS genişliğiyle.
- Ağ kesintisinin istek gönderildikten sonra (yanıt kaybı) oluşması tarayıcıda üretilmedi; aynı CommandId tekrarı (K12) HTTP ile ölçüldü.
- Test operatörleri (2003-2016) QA kopyasına SQL ile eklendi (uygulamada Operatör oluşturma ilk giriş akışı ister); yalnız QA kopyasında.
- QA dışı süreçler: 5080 (kullanıcının uygulaması, test başında PID 18196, sonda dinlemiyordu) ve 5181 PID 2580 dokunulmadı. QA'nın açtığı 5401, 5402, 5403 süreçleri ve playwright oturumları kapatıldı.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | BR-11: malzeme zorunlu tipte malzemesiz kaydın ilk adımı başlatılamaz | qa-tester | Adım başlatma Faz 5'te |

## 10. Analist kararı

```
FAZ 4: İADE
Tarih: 2026-11-15
Gerekçe: B1 YÜKSEK açık: JS kapalı yeni kayıtta lot ekleme sonrası kayıt açılamıyor ve seçili lot görünmüyor; K10 ve K11'in PL-64
         kısmı karşılanmıyor. Diğer 40 kriter kanıtla eşleşti; JS açık akış ve Run'ın JS kapalı akışı doğru.
İade maddeleri: B1 frontend-engineer (backend'e iş yok)
Yeniden denetim şartı: aşağıdaki "Tur 2 yeniden denetim" listesi
```

Analistin doğruladıkları (kanıt dışında): `Create.cshtml:29` formda `action` yok ve `:121` yenileme düğmesi `name="refresh"`; `_LotPicker.cshtml:13`
`data-lot-js` koşullu yazılıyor (frontend düzeltmesi çalışma ağacında, ölçülmedi). Bulgunun iki parçası kodla tutarlı.

QA'nın bulgu saymadıkları:

- Malzeme zorunlu tipte malzemesiz kayıt açılması doğru davranıştır: K-11 "malzeme kayıt düzeyinde, ilk adıma kadar eklenir; zorunlu tipte ilk adım
  malzemesiz başlamaz". Doküman satır 22-23 ("atlayıp devam etmesini istemiyoruz") ilk adımın engellenmesiyle karşılanır; Faz 5 K8 bunu ölçer.
  A1 Faz 5'e F4-A1 olarak yazıldı.
- Run `?q=` aramasında kayda eklenmiş lotun listelenip seçilince nedenle reddedilmesi DÜŞÜK kullanılabilirlik konusudur; veri yanlışı yok. Run
  ekranı Faz 5'in dosyasıdır, Faz 5'e F4-A2 olarak devredildi: `AvailableLots` ve `/records/lots` (kayıt kimliğiyle) kayda eklenmiş lotu listelemez.

Tur 2 yeniden denetim (qa-tester, aynı rapora "Tur 2" bölümü; önce yerleşim ön turu, PL-35, O-06):

- B1 (JS kapalı Chromium, 360 ve 1440): `/records/create`'te tesis, hat, makine ve malzeme zorunlu tip seçilir; `LotQuery` ile süzülür; normal lot
  eklenir, seçili satır görünür; SKT geçmiş lot gerekçesiz eklenince görünür alan hatası, gerekçeyle eklenince işaretli satır; bir satır çıkarılır.
  Her yenilemede kayıt ve olay 0, `CommandId` ve girilen alanlar korunur, adres çubuğunda kalıcı `refresh` yok. Ardından "Kaydı oluştur" PIN'le
  kaydı açar, yürütme ekranı açılır, veritabanında malzeme satırları ve SKT işareti doğru. K10 ve K11 JS kapalı ve JS açık yeniden işaretlenir.
- Gerileme: JS açık `LotPicker.js` akışı, K12 (aynı `CommandId` tek kayıt), K13 (çevrimdışı), G5a, G5b, G11 ve G12 `/records/create` ve
  `/records/run/{id}` için; G1, G2, §4 grep, konsol 0.

### Tur 2 kararı

```
FAZ 4: TESLİM ALINDI
Tarih: 2026-11-15
Gerekçe: B1 tur 2 şartının her maddesiyle ölçülüp kapandı (JS kapalı lot akışı 360 ve 1440'ta, kayıt açma, yenilemelerde kayıt ve
         olay 0, ExpiredAtEntry doğru); K1-K21, T-B14, T-B22, F1-A1..F3-A1 ve G1-G12, G5a, G5b (42/42) tur 1-2 kanıtıyla eşleşti;
         açık bulgu 0; QA önerisi GEÇTİ.
Açık kalemler: Faz 5'e F4-A1 (malzemesiz kayıtta ilk adım engeli) ve F4-A2 (Run aramasında eklenmiş lot) (PL-67).
```

Analistin doğruladıkları (kanıt dışında): `Create.cshtml:29` formda `action="/records/create"`; `_LotPicker.cshtml:13` `data-lot-js` yalnız tek
seçimli modda yazılıyor. QA notları: test operatörlerinin QA kopyasına SQL ile eklenmesi yalnız test kurulumudur, ölçülen akışları etkilemez;
başsız tarayıcıda doğrulama balonu yerine `validationMessage` ve odak ölçümü kabul edildi.

## Tur 2 (2026-11-15)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `64b9ad3`); frontend devir notu `Docs/Handover/frontend-engineer-20261115-0800.md` "B1: JS kapalı lot akışı"; bölüm 10 "Tur 2 yeniden denetim" listesi |
| Ortam | LOCAL, yeni `dotnet publish -c Release -o .qa/f4/build` (Tailwind QA çıktısı `wwwroot/css/app.css` ile bayt bayt aynı; `-warnaserror --no-incremental` 0 uyarı 0 hata). Port 5401 Production `.qa/f4/qa.db` (tur 1 verisinin devamı, yeni süreç içi rastgele anahtar), 5402 boş kopya, 5403 Development. Chromium; tarayıcının kendi form gönderimi |
| QA önerisi | GEÇTİ (KRİTİK, YÜKSEK, ORTA, DÜŞÜK açık 0) |

Betikler ve ham çıktılar `.qa/f4/` altında (`b1.tmpl.js`, `b1a.txt`, `b1b.txt`, `b1v.tmpl.js`, `pend.tmpl.js`, `js1.tmpl.js`, `b1db.out`, `k12t2.out`, `r-t2e.json`, `g12-t2g.json`).
Teslimden önce yerleşim ön turu (PL-35): `/records/create`, `/records/run/{id}`, `/my-work` için G12 24 hücre kusur 0, mekanik eval 36 ölçüm temiz.

### T2.1 İade edilen bulgunun yeniden ölçümü (bölüm 10, tur 2 şartı)

| # | İstenen sonuç (bölüm 10) | Ölçüm | Sonuç |
|---|---|---|---|
| 1. B1, JS kapalı 360 ve 1440 | Tesis, hat, makine ve malzeme zorunlu tip seçilir; `LotQuery` ile süzülür; normal lot eklenir, seçili satır görünür; SKT geçmiş lot gerekçesiz eklenince görünür alan hatası, gerekçeyle eklenince işaretli satır; bir satır çıkarılır. Her yenilemede kayıt ve olay 0, `CommandId` ve alanlar korunur, kalıcı `refresh` yok. "Kaydı oluştur" PIN'le açar; DB'de malzeme satırları ve SKT işareti doğru | `b1.js`, iki genişlikte aynı sonuç (`JsKapaliAcik.txt` "Tur 2"). Form `action="/records/create"`, sayfada `data-lot-js` yalnız tek lot modunda (1 öğe, Run yedeği). Tesis, hat, makine "Seçimi uygula" ile yenilendi; tip PLN, iş emri, açıklama ve kilit 50 dk her yenilemede korundu, `CommandId` baştan sona aynı. `LotQuery` "L2027" yalnız L2027-001 ve L2027-002'yi, "L2026" yalnız L2026-001'i listeledi. Normal lot eklenince satır ve "Çıkar" görünür. SKT geçmiş lot gerekçesiz eklenince satır "SKT geçmiş" rozetiyle ve görünür, zorunlu gerekçe alanıyla geldi; gerekçesiz "Kaydı oluştur"da tarayıcı gönderimi durdurdu, odak gerekçe alanında, tarayıcı iletisi "Lütfen bu alanı doldurun."; tarayıcı doğrulaması atlanınca sunucu "SKT geçmiş lot için gerekçe girin." iletisini özet ve gerekçe alanının altında verdi (`b1v.js`). Üçüncü lot eklenip "Çıkar" ile kaldırıldı. "Ekle" ve "Çıkar" sonrası adres anlık `?refresh=true` oluyor, sonraki yenileme `/records/create`'e dönüyor; "Kaydı oluştur" her durumda `/records/create`'e gitti (kalıcı değil). Gizli `required` alan 0. Gerekçe yazılıp PIN'le "Kaydı oluştur": 302 `/records/run/18` ve `/run/19`; DB (`b1db.out`): önce kayıt/olay/malzeme 17/23/10, sonra 19/26/14; olay farkı 2 `RecordOpened` ve kayıt 14'ün süre aşımıyla oluşan 1 `RecordTimedOut`, refresh POST'larında kayıt ve olay üretilmedi; iki kayıt (`T3-H1-A8-P2026-1`, `T3-H1-A9-P2026-1`, PLN, iş emri `IE-T2-360` / `IE-T2-1440`, kilit 50), her birinde L2027-001 (`ExpiredAtEntry` 0) ve L2026-001 (`ExpiredAtEntry` 1, gerekçe yazılı), çıkarılan lot yok; Run'da "SKT geçmiş" rozeti ve gerekçe görünüyor. Görüntü: `JsKapali-Tur2-360.png` | GEÇTİ |
| 2. K10, K11 JS kapalı ve açık | Yeniden işaretlenir | JS kapalı: madde 1 ve Run yolu (tur 1, değişmedi). JS açık yeni kayıt (`js1.js`): zincirli seçim, varsayılan kilit 90, LotPicker sonucu "SKT geçmiş", gerekçe alanı görünür ve zorunlu, gerekçesiz gönderimde istek yok ve odak alanda; kayıt açıldı, malzeme işaretli ve gerekçeli. JS açık Run (`pend.js`): LotPicker ile L2027-002 seçildi, yanlış PIN "PIN hatalı.", PendingMaterial satırı şablondan yeniden çizildi (lot ve SKT gerekçesi korundu, PIN boş, odak uyarıda), doğru PIN 302; SKT geçmiş lotta gerekçesiz istek gitmedi. Sahip ve PIN kuralları tur 1 sonucunda kalır | GEÇTİ |
| 3. Gerileme | LotPicker.js, K12, K13, G5a, G5b, G11, G12 (`/records/create`, `/records/run`); G1, G2, §4, konsol | K12: aynı `CommandId` iki gönderim tek kayıt tek olay. K13: çevrimdışıyken istek 0 ve "İşlem yapılmadı, bağlantı yok..." iletisi, çevrimiçi yeniden deneme aynı `CommandId` ile tek kayıt. G5a: JS kapalı yeni kayıtta her adımda 4 form, Run'da 6 form, belirteçsiz 0; antiforgery'siz POST formu grep 0. G5b grep 0. G11 form-detay iskeleti ve G12 24 hücre kusur 0; mekanik eval 36 ölçüm temiz. G1 0/0, G2 policy 0, §4 tam tablo 0, G8 grep 0, konsol 0 | GEÇTİ |

### T2.2 Kabul kriterleri (yeniden işaretlenen ve gerileme)

| # | Sonuç | Kanıt |
|---|---|---|
| K10 | GEÇTİ | T2.1 madde 1-2 |
| K11 | GEÇTİ | T2.1 madde 1-2 |
| K12, K13, G9 | GEÇTİ | T2.1 madde 3 |
| G5a, G5b, G11, G12, G1, G2 | GEÇTİ | T2.1 madde 3 |

Kabul özeti tur 2 sonunda: K1-K21 21/21; T-B14, T-B22 2/2; F1-A1, F1-A2, F2-A1, F2-A2, F3-A1 5/5; ortak kriterler 14/14; toplam 42/42.

### T2.3 Mekanik eval (§3)

36 ölçüm (`MekanikEval.txt` "Tur 2"): `/records/create`, `/records/run/4`, `/records/run/4?q=K`, `/my-work` 360, 768, 1024, 1440, 1920 açık, 360 ve 1440 koyu,
390 reduced-motion, 320x640; hepsi temiz, en düşük kontrast 5,44:1, konsol 0.

### T2.4 Klavye turu (değişen kısım)

JS kapalı yeni kayıtta gerekçesiz gönderimde odak gerekçe alanına gidiyor; JS açık Run'da ret sonrası odak uyarı kutusunda. Diğerleri tur 1 sonucunda kalır.

### T2.5 DNA uygunluğu (değişen maddeler)

Değişen madde yok; JS kapalı seçili lot satırı JS açık satırla aynı bileşen (`_LotRow`), SKT geçmiş satırı hata tonlu kenarlık ve rozetle.

### T2.6 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-4/` (8 dosya): `JsKapaliAcik.txt` ve `MekanikEval.txt` sonlarına "Tur 2" bölümü eklendi; `JsKapali-Tur2-360.png` (yeni):
JS kapalı 360'ta iki seçili lot, SKT geçmiş satırda rozet ve görünür gerekçe alanı.

### T2.7 Yeni bulgular

yok

Not (bulgu değil): JS açık Run'da `data-lot-pending` şablonundan eklenen satırda JS kapalıya özgü "Çıkar" düğmesi (`RemoveMaterialIndex`, `formaction="/records/create?refresh=true"`)
gizli olarak bulunuyor; görünmüyor ve gönderilmiyor, etkisi yok.

### T2.8 Kapsam dışı / yapılamayan (tur 2)

- Tarayıcının yerel doğrulama iletisi ("Lütfen bu alanı doldurun.") başsız Chromium'da görsel olarak görüntülenemez; `validationMessage`, `:invalid` ve odak ölçüldü.
- Test operatörleri 2017-2020 QA kopyasına SQL ile eklendi.
- QA dışı süreç 5181 PID 2580 dokunulmadı; 5080 test boyunca dinlemiyordu. QA'nın açtığı 5401, 5402, 5403 süreçleri ve playwright oturumları kapatıldı.

### T2.9 Sonuç

B1 kapandı; K10 ve K11 JS kapalı ve JS açık geçti, kabul kriterleri 42/42. Yeni bulgu yok; KRİTİK, YÜKSEK, ORTA ve DÜŞÜK açık bulgu 0. QA önerisi GEÇTİ.
Karar analistte.

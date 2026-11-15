# Faz 5 Teslim Raporu: Adım yürütme, görevli ve mola

| Alan | Değer |
|---|---|
| Faz | 5 |
| Tarih | 2026-11-15 |
| Tur | 1 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `4c0a01d`; Faz 5 kodu commit'lenmemiş, migration `20261115032303_AddRecordBreaks`, `DemoData.sql` Faz 5 bölümü). Devir notları `Docs/Handover/backend-engineer-20261115-1700.md` (PL-69 dahil), `Docs/Handover/frontend-engineer-20261115-1500.md` ("Güncelleme" ve "FE 10" bölümleri); `DB-2026-007` |
| Ortam | LOCAL, Release build, port 5501 (Production) ve 5503 (Development), DB: `.qa/` kopyası. Ön tur `.qa/f5/build`, FE 10 (PL-70) sonrası teslim testi `.qa/f5/build2`. `.qa/f5/qa.db` = `App_Data/vaka.db` SQLite yedekleme API kopyası + `DemoData.sql`; 5501'de `EventChain__HmacKey` yalnız süreçte üretilen rastgele 32 bayt (yazılmadı); 5503 `.qa/f5/qa-dev.db`, user-secrets anahtarı (bağımsız HMAC doğrulaması için). Chromium (playwright-cli) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ |
| Analist kararı | TESLİM ALINDI (tur 1, 2026-11-15; DÜŞÜK B1, B2 Faz 6'ya) |

## 1. Kapsam

- DB: `RecordBreaks` (kayıt ve kişi başına tek açık mola kısmi unique index, zaman ve silme tetikleyicileri), demo tip KSA (2 faz, 2+2 adım, 60 sn minimum, malzemesiz) (DB-2026-007).
- BE: adım başlatma ve bitirme (SK-1, BR-14, BR-15, BR-24, BR-11, BR-46, BR-39, saha referansı, BR-21 süre altı gerekçe, BR-22 otomatik tamamlanma), görevli ekleme ve çıkarma (SK-2, K-41), mola (BR-38, BR-39, BR-40, SK-6), `IDurationCalculator` SH-17, SH-01..SH-04, SH-06..SH-09, SH-11, SH-12, SH-14; İşlerim görevli adımları ve açık mola; BR-36 genişlemesi; F4-A2 lot süzgeci; `RecordStepsController`, görünüm konumu genişletici (PL-69).
- FE: E-15 Yürütme ekranı tam hali, E-17 görevli paneli (JS açık `ParticipantPicker.js`, JS kapalı radyo listesi), E-18 `/breaks/current`, İşlerim görevli adımlar, `LiveTimer.js`; FE 8 PIN `maxlength` ve rakam süzgeci (K25), FE 9 göz düğmesi ve select rötuşu (K26), FE 10 süre altı gerekçe dialogu (PL-70, K27) ve JS kapalı `FinishStep` sayfası.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Sıradaki adım belirgin, kilitli adımlar nedenli; sıra dışı ve ikinci devam eden adım reddi | GEÇTİ | `Evidence/Faz-5/KabulKriterleri.txt` T1: 1. adım bitmeden 2. ve 3. adım 200 ret, olay +0; adım sürerken başka adım 200 ret, olay +0. T5: "Kilitli: Önceki adım tamamlanmadan başlatılamaz"; sıradaki adım "Hazır" kenarlıklı (`data-state=ready`) |
| K2 | Görevli olmayan ekranı görür, düğme görmez; POST reddi, durum değişmez | GEÇTİ | T3 K2: op2 Run 200, "Adımı başlat" ve "Adımı bitir" yok; başlat ve bitir POST 403, olay +0, adım durumu 1 ve 2 değişmedi |
| K3 | Sahip kendi PIN'iyle ekler; olay ekleyeni ve ekleneni taşır; yanlış PIN sayacı | GEÇTİ | T1: `ParticipantAdded` payload `{"StepId":39,"ParticipantUserId":14,"AddedByUserId":13}`, aktör 13; yanlış PIN 200, eklenmedi, op10 `PinFailedCount` 0 -> 1, olay +0 |
| K4 | Pasif ya da molada kişi eklenemez (nedenli); başlamış adımda ekleme/çıkarma reddi; sahip olmayan görevli reddi | GEÇTİ | T5: "QA Pasif Operatör pasif, görevli olarak eklenemez.", "QA Operatör 26 molada, görevli olarak eklenemez.", "Adım başladıktan sonra görevli eklenmez ya da çıkarılmaz."; T3: sahip olmayan görevli (op19) 403; hepsinde olay +0 |
| K5 | Sahip kendini çıkarır, yine başlatıp bitirir; efor yalnız listedekilere | GEÇTİ | T3 K5: kayıt 13 adım 1 listede yalnız op11; sahip başlattı ve bitirdi (başlatan/bitiren 19), süre 3, efor 3 (x1); son görevliyi çıkarma reddi |
| K6 | Görevli başka kayıtta devam eden adımdaysa ret, kişi ve kayıt no; eşzamanlı başlatmada tek adım | GEÇTİ | T3c: "QA Operatör 19 başka kayıtta devam eden adımda: T1-H1-M16-P2026-1.", olay +0. T3: ortak görevli op10 ile iki kayıtta `Barrier` ile eşzamanlı başlatma: RD 302, RC 200 ret; op10 `IsWorking=1` tek satır |
| K7 | K-41 uyarısı ve ekleme serbest | GEÇTİ | T3: `/records/people` op4 `hasOwnActiveRecord:true, isAvailable:true`, ekleme 302; JS kapalı listede "Kendi açık kaydı var, yine de eklenebilir." (`JsDavranis.txt` nojs5); JS açıkta uyarı kutusu görünür (pp) |
| K8 | Malzeme zorunlu tipte malzemesiz ilk adım reddi, olay yok | GEÇTİ | T3 K8: PLN kayıt 23, ekranda "Malzeme zorunlu", başlat formu yok; doğrudan POST 200 ret, olay +0, kayıt Açıldı |
| K9 | İlk adımda saha referansı, Devam ediyor; kilit bitişi geçmişse ret ve Zaman aşımı | GEÇTİ | T1: `T1-SD-2026-6`, durum 2, ekranda görünür. T3: kilit 1 dk, 63 sn sonra başlatma 200 ret; durum 5, `ClosedAtUtc` = kilit bitişi, olaylar `RecordOpened`, `RecordTimedOut` |
| K10 | Molasız iki görevlili adımda süre = bitiş - başlangıç, efor = süre x 2 | GEÇTİ | T1: 13:34:52.249-13:34:57.673 = 5,42 sn -> süre 5, efor 10, görevli 2, `IsWorking` kalan 0 |
| K11 | Süre altında gerekçesiz bitirilemez, olay yok; gerekçeyle "Tamamlandı - süre altında", bitiş gerekçeli isteğin saati, gerekçe ekranda | GEÇTİ | T1: gerekçesiz 200 form, olay +0, komut +0, PIN sayacı değişmedi; aynı CommandId ile gerekçeli: faz durumu 4, gerekçe kayıtlı, adım bitişi 13:35:06.38 > ilk istek 13:35:03.36; ekranda gerekçe. JS kapalı `FinishStep` sayfası (FE 10 sonrası) 8 hücre temiz, adım ret boyunca Devam ediyor |
| K12 | Son adımla kayıt Tamamlandı, tamamlanma zamanı; panoda makine Boş | GEÇTİ | T1: kayıt 8 durum 4, `CompletedAtUtc` = son adım bitişi; `/machine-board` "M09 QA makine M09 Boş Kayıt açılabilir." |
| K13 | Mola iki adım arasında; adım sürerken ve Açıldı kayıtta düğme yok ve ret; sahip ya da sıradaki adımın görevlisi; görevi olmayan reddi | GEÇTİ | T3: Açıldı kayıtta "Mola başlat" yok, istek 200 ret; T1: adım sürerken 200 ret; op19 (sıradaki adımın görevlisi) 302 `/breaks/current`; op2 403; olay sayıları doğru |
| K13a | Mola açıkken başlatma "Kayıtta açık mola var", olay yok; görevli ve malzeme reddi | GEÇTİ | T5: başlatma ve görevli ekleme "Kayıtta açık mola var.", olay +0; T3: görevli çıkarma ve malzeme 200 ret (malzeme, başlamış kayıtta Faz 4 kuralıyla da kapalı) |
| K14 | Molada kişinin başka kayıtta başlatma, kayıt açma, görevli, malzeme istekleri `/breaks/current`'a; molayı bitirir, çıkış yapar | GEÇTİ | T3 K14: dört istek 302 `/breaks/current`, olay +0, kayıt +0; `/my-work` ve Run GET 302 `/breaks/current`; çıkış 302, yeniden giriş, mola bitirme 302 |
| K14a | Başka kayıtta devam eden adımdaki kişinin mola reddi | GEÇTİ | T3c: op10 (sıradaki adımın görevlisi, başka kayıtta çalışıyor) "Başka kayıtta devam eden adımdasınız: T1-H1-M19-P2026-1. Mola verilemez.", olay +0 |
| K15 | Başlatandan başkasının mola bitirme reddi | GEÇTİ | T3: sahip op18'in op19'un molasını bitirmesi 403, olay +0 |
| K16 | Mola faz ve kayıt netinden düşülür, brütten düşülmez; molasız boşluk nete dahil; Ö-03 formül ve kısa sürelerle uygulama | GEÇTİ | `SureFormul.txt`: yayınlanan `DurationCalculator` ile SC-06: molasız faz net 40 dk 12 sn + d, 40 dk molalı 12 sn + d, 900 sn minimumla ikinci durum süre altı (d = 0, 30, 300). T2 (KSA, 60 sn): molasız brüt 64 = net 64, gerekçesiz 302; 50 sn molalı brüt 64, mola 50, net 14, gerekçe formu. Kayıt 4 gerçek 39 dk 41 sn mola: brüt 2582, mola 2381, net 201 |
| K17 | Canlı sayaç sunucu değeriyle başlar, JS'te tarih farkı yok; molada faz sayaçları durur, "Sayaçlar durdu"; mola bitince kaldığı yerden | GEÇTİ | `JsDavranis.txt` pp: yenilemede `data-seconds` = gösterim; 2,2 sn'de 1 -> 3 sn ilerledi. Ön tur: molada faz sayacı `data-running=false`, 2,3 sn'de değişmedi, bant ve "Sayaçlar durdu, kayıtta mola açık." görünür. T4: mola bitince faz net 2 -> 4 -> 5. `wwwroot/js` içinde `new Date`, `Date.now` 0 eşleşme. Açık molada değer 2/3 oynaması B1 (DÜŞÜK) |
| K18 | Tamamlanmamış adımda görevli ya da açık molalı kullanıcı pasifleştirilemez; ekran işleri listeler | GEÇTİ | T3 K18: op19 için "Açık işleri var": kayıt sahibi, iki görevli adım, "Açık mola"; PIN alanı yok, POST 200 ret, aktif kaldı. Bağlantı yok: bölüm 9 |
| K19 | Dolu adım başlangıcı SQL UPDATE ile değişmez | GEÇTİ | T4 (kopya `k19.db`): adım ve mola başlangıç/bitiş UPDATE'leri "Başlamış ya da bitmiş adımın zamanı değiştirilemez." ile reddedildi; başlamamış adım değişebiliyor |
| K20 | SH kimlikleri formülleriyle XML yorumunda | GEÇTİ | `IDurationCalculator.cs` Faz 5 bölümü: SH-17, SH-01, SH-02, SH-03, SH-04, SH-06, SH-07, SH-08, SH-09, SH-11, SH-12, SH-14 ve açık mola süresi; `DurationCalculator.cs:140-205` formüllerle uyuşuyor (yuvarlama notu B1) |
| K21 | Fotoğraf ve video yürütme ekranında açılır; 360'ta kaydırma yok, başlat/bitir 44 px | GEÇTİ | `JsDavranis.txt` k21: KSA v2 adım 1'de fotoğraf (`/media/1` 200 image/jpeg, 3840 px yüklendi) ve video (`/media/2` 206, readyState 4) dialogda açıldı, Esc kapattı; 360 ve 1440'ta kaydırma yok. `.run-action` düğmesi 56 px |
| K22 | Brüt ilk adım başlangıcından; açılış ayrı alanda; açılış ile ilk adım arası hiçbir süreye girmez | GEÇTİ | T4: açılış 13:34:43.8, ilk adım 13:34:52.2 (8,4 sn bekleme), brüt 24 = son bitiş - ilk adım (açılıştan 33 olurdu); ekranda "Açılış 16:34" başlıkta, özette "İlk adım başlangıcı 16:34" |
| K23 | Sahip adım arasında çıkıp girer; Devam ediyor, İşlerim'de, sıradaki adımdan devam; makine kilitli, başkası açamaz | GEÇTİ | T3 K23: çıkış 302; pano "M16 ... Devam ediyor ... Sahip QA Operatör 18"; op7 M16'da kayıt açma 200 "Kayıt açılamadı", kayıt +0; giriş sonrası İşlerim'de kayıt no ve "Devam ediyor", Run'da sıradaki adımın (86) başlat formu |
| K24 | KSA demo tipi; PLN, ACL, PLN3 değişmez; ikinci uygulamada sayı aynı | GEÇTİ | `KabulKriterleri.txt` K24: ikinci uygulamada fark yok; KSA 2 faz 60/60 sn, 2+2 adım, malzemesiz, v1 Yürürlükte, yayınlayan NULL, makine 1,2,3,5; `DemoData.sql` farkı yalnız ekleme; yeni kayıt tip listesinde KSA |
| K25 | Her PIN alanında 5. hane ve rakam dışı girilmez (yapıştırmada da); ayar 6 ile 6 hane; sabit yok; sunucu kuralı değişmedi | GEÇTİ | `JsDavranis.txt` k25: "12a3b45678" yazımı 1234, "98-76 54321x" yapıştırma 9876 (Run, mola, yeni kayıt, güvenlik, yayın); 5503'te ayar 6: `maxlength=6`, yazım 123456, yapıştırma 987654, belirleme modunda 6 hane göstergesi. Hedefli ölçümde 888 PIN alanı ölçümünün hepsi `maxlength=4`, `inputmode=numeric`. `Views`, `Services`, `wwwroot/js` içinde PIN uzunluğu sabiti 0; PIN servisleri değişmedi |
| K26 | Göz düğmesi ve select Faz 1-5, 4 genişlik, iki tema | GEÇTİ | `MekanikEval.txt` hedefli ölçüm: select 152 ölçüm, hepsi `.select-wrap` içinde, `appearance:none`, 48 px, ok dikey fark 0, alan içinde, tıklamayı geçiriyor; pasif select kesik kenarlık ve `sunken`. Göz düğmesi 1.056 ölçüm (888 PIN, 168 parola): 44 px, alan içinde, merkez farkı 0, dinlenmede dolgusuz, köşe 0 = 2 px - 2 px kenarlık; hover ve odakta `sunken`, odak halkası 3 px içeride, ikon kontrastı 6,48 (açık) / 6,54 (koyu) ve üzeri; reduced-motion'da geçiş 0,01 ms. Faz 1-4 G11 72/72 aynı |
| K27 | Süre altı gerekçe dialogu (PL-70) | GEÇTİ | `JsDavranis.txt` k27: adres değişmeden `<dialog>` modal, ad "Faz süre altında kalıyor", faz/net/minimum görünür, odak gerekçede, form belirteci 1, CommandId sayfadakiyle aynı; gerekçesiz gönderim istek atmadan "Gerekçeyi yazın."; gerekçe + yanlış PIN dialogda "PIN hatalı.", gerekçe korundu, olay +0, adım Devam ediyor; Esc ve "Vazgeç" kapatır, odak "Adımı bitir"e döner; Tab sayfa öğesine çıkmıyor (tab360); doğru PIN ile faz 4 "Tamamlandı - süre altında", olay +2. 320-1440, açık ve koyu, reduced-motion: kaydırma yok, düğmeler 56 px, alan sol farkı 0, göz düğmesi hizalı, kontrast 6,48 ve üzeri. Koyu tema perdesi B2 |
| F4-A1 | Malzeme zorunlu tipte malzemesiz kaydın ilk adımı başlamaz | GEÇTİ | K8 satırı |
| F4-A2 | Run aramasında kayda eklenmiş lot görünmez; gönderilirse nedenli ret | GEÇTİ | T3: `/records/lots?q=L20&recordId=14` yalnız L2026-001 (eklenen L2027-001, -002 yok), recordId'siz üçü; aynı lot POST 200 ret, olay +0. JS açık LotPicker isteği `recordId=14` ile, eklenmiş lot listede yok (f4js2) |
| T-B20 | "Molayı bitir" 320-1920 tek satır, 56 px, adım düğmeleriyle aynı ızgara | GEÇTİ | `MekanikEval.txt` misc5: 320/360/768/1024/1440/1920'de 56 px, 1 satır, taşma yok; `.run-action` (md üstünde 448 px) |
| G1 | `-warnaserror` 0/0; `build:css` | GEÇTİ | Bölüm 3 |
| G2 | §4 grep 0; öznitelikler; rol metni | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi, §4, konsol, DNA | GEÇTİ | 313 mekanik eval hücresi, 312 G12 hücresi, 352 hedefli ölçüm hücresi; kırık hücre yok, konsol 0 (QA'nın kestiği istek dışında), §4 0 |
| G4 | Operatör ekranları 360'ta kaydırmasız, birincil 44 px, durum metinle | GEÇTİ | 360 ve 320'de `hScroll` false (Run, İşlerim, mola, dialog); birincil düğmeler 44/48/56 px; rozetler ikon + metin |
| G5 | Belirteçsiz POST 400 | GEÇTİ | `YetkiMatrisi.txt`: altı Faz 5 POST'u Operatör oturumunda belirteçsiz 400 |
| G5a | Her POST formu kendi belirtecini taşır | GEÇTİ | Hedefli ölçüm: 1.768 POST formu (sayfa-hücre toplamı), belirteçsiz 0; dialog formu 1 belirteç |
| G5b | `bool` onay kutuları | GEÇTİ | Faz 5 formlarında onay kutusu yok; `Views/**` gizli `false` grep 0 |
| G6 | em/en-dash 0 | GEÇTİ | `Src` görünür metin, seed, Faz 5 dokümanları ve kanıt dosyaları 0; eval `emdash` false |
| G7 | `Up()` Drop yok | GEÇTİ | Bölüm 3 |
| G8 | Controller ve görünümde süre aritmetiği yok | GEÇTİ | `Controllers/`, `Views/` içinde `TimeSpan`, `.Subtract(`, `AtUtc -`, `DateTime.Now/UtcNow` 0; JS'te yalnız `performance.now` farkı |
| G9 | Aynı CommandId tek etki tek olay | GEÇTİ | T1: görevli ekle, başlat, bitir, gerekçeli bitir, mola başlat, mola bitir iki gönderim, her biri tek olay; T3: görevli çıkar iki gönderim tek `ParticipantRemoved` |
| G10 | Süper admin yeni adreslerde 403, menüde yok | GEÇTİ | `YetkiMatrisi.txt`: 5 GET ve 6 POST 302>403; menüde `/my-work`, `/breaks` yok |
| G11 | Yerleşim imzası | GEÇTİ | Yeni liste tablosu yok; Run iskeleti lg+ yan yana (20/24 rem ray), lg altı ray altta; İşlerim görevli adımlar 360'ta 1, md+ 2 sütun (beklenen 1/2); Faz 1-4 dokuz tabloda 72/72 aynı |
| G12 | Hizalama kusuru 0 | GEÇTİ | `G12Hizalama.txt`: 312 hücre kusur 0 (JS açık ve kapalı Run, İşlerim, mola, FinishStep, Faz 1-4); adım satırında sarılmayan satır merkez farkı 0 (sarılan satırlar Faz 4 kabulüyle aynı); dialog içi ölçüm k27 |

Kabul özeti: K1-K27 (K13a, K14a dahil, K16a ve K16b Faz 7'de) 29/29; F4-A1, F4-A2, T-B20 3/3; ortak kriterler (G1-G12, G5a, G5b) 14/14; toplam 46/46.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental -o .qa/f5/bchk` (ön tur) ve `.qa/f5/bchk2` (FE 10 sonrası): 0 Uyarı, 0 Hata; `dotnet publish -c Release` başarılı |
| CSS | `npm run build:css` | Tailwind QA çıktısı (`.qa/f5/app.check2.css`, FE 10 sonrası) `wwwroot/css/app.css` ile bayt bayt aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme; sihirli rol metni 0; `RecordStepsController` ve `BreaksController` `[Authorize(Roles = Roles.Operator)]`; özniteliksiz iki dosya controller değil. MediatR, AutoMapper, generic repository, `new ...Service(` 0; `GetRequiredService` yalnız `Program.cs:107` başlangıç anahtar doğrulaması (Faz 4'ten); `Program.cs` içinde `Migrate()` 0. Controller'larda iş kuralı yok (servis sonucu durum koduna çevriliyor) |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: tablo `RecordBreaks`, kolonlar `StartedByUserId`, `DuringStepId`, `EndKind`; tetikleyiciler `TR_RecordBreaks_NoTimeChange`, `TR_RecordSteps_NoTimeChange`; servisler `StepStarter`, `StepFinisher`, `BreakService`, `ParticipantDirectory`, `RunCompletion`; `Routing/RecordViewLocationExpander.cs`; JS `LiveTimer.js`, `ParticipantPicker.js`. §8: `#region`, süs yorum, emoji, kalıp dil, `Helper/Utils/Manager` 0; dört satır içi yorum "neden" anlatıyor |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (FE 10 sonrası tam tablo yeniden) |
| Mekanik eval | `UiDelivery.md §3` | 313 hücre; KALDI görünen 16 hücre JS kapalı radyo 20x20, `label.choice` (min-h-14) içinde, hedef etiket: kabul. En düşük kontrast 5,28:1, üst bar 64 px, konsol 0 (`MekanikEval.txt`) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | `20261115032303_AddRecordBreaks` DB-2026-007 olarak indekste (Eklemeli, DOĞRULANDI); `Up()`: 1 `CreateTable`, 5 `CreateIndex`, 1 `Sql` (tetikleyici); `Drop` yalnız `Down()`; ALTER yok |

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Denetçi (3001); Editor = Operatör (op 2001, kayda yabancı; kurallar için sahip ve görevli satırları bölüm 2); Admin = Süper admin (0001); Yönetici (1001) parantez içinde. Tam tablo `Evidence/Faz-5/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/records/run/3` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 (salt okunur, düğmesiz) | 302>403 |
| GET `/records/people?q=&stepId=` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 JSON | 302>403 |
| GET `/breaks/current` | 302 (giriş) | 302>403 (Yönetici 302>403) | 302 `/my-work` (açık mola yok); molada 200 | 302>403 |
| GET `/my-work` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200; molada 302 `/breaks/current` | 302>403 |
| GET `/records/lots?q=&recordId=` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 | 302>403 |
| POST `/records/start-step/{id}`, `/records/finish-step/{id}` | 302 (giriş) | 302>403 (Yönetici 302>403) | 403 (görevli değil); belirteçsiz 400 | 302>403 |
| POST `/records/add-participant/{id}`, `/records/remove-participant/{id}` | 302 (giriş) | 302>403 (Yönetici 302>403) | 403 (sahip değil); belirteçsiz 400 | 302>403 |
| POST `/breaks/start/{recordId}` | 302 (giriş) | 302>403 (Yönetici 302>403) | 403 (kayıtta görevi yok); belirteçsiz 400 | 302>403 |
| POST `/breaks/end/{breakId}` | 302 (giriş) | 302>403 (Yönetici 302>403) | 404 (yok), başkasının molası 403; belirteçsiz 400 | 302>403 |

Menüde gizli olup adresle açılan sayfa yok.

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/records/run/{id}` Açıldı, sahip, görevli paneli (JS açık ve kapalı, `?stepId=&people=`) | Temiz, ray altta | Temiz, ray altta | Yan yana 388/320 | Yan yana 724/384 | Ana 1600 ile sınırlı | Temiz | Temiz | 320x640 ve 640x400 temiz | İlk odak atlama bağlantısı, tüm öğeler halkalı |
| `/records/run/{id}` adım sürüyor (sahip, görevli, yabancı), son adım | Temiz, `.run-action` tam genişlik 264-292 px, 56 px | Temiz, 448 px | Temiz | Temiz, canlı sayaç ilerliyor | Temiz | Temiz | Geçiş ve animasyon 0,01 ms | Temiz | "Adımı bitir" halkalı |
| `/records/run/{id}` açık mola (başka operatör) | Bant, "Sayaçlar durdu." | Temiz | Temiz | Faz sayacı durdu, mola süresi ilerliyor | Temiz | Temiz | Temiz | Temiz | Temiz |
| `/records/run/{id}` tamamlandı, süre altı gerekçeli | Özet, formlar yok | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz |
| Süre altı dialogu (K27) | 322 px, kaydırma yok, düğmeler alt alta 56 px | 576 px, düğmeler yan yana | Temiz | Temiz | Ölçülmedi (1440 ile aynı en) | Temiz; perde açık renk (B2) | Temiz | 320x640 temiz | Odak içeride, Esc, dönüş "Adımı bitir" |
| `FinishStep` sayfası (JS kapalı) | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | Ölçülmedi | Ölçülmedi | Form alanları sırayla |
| `/breaks/current` | Temiz, "Molayı bitir" 56 px tek satır | 448 px | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | PIN, göz, "Molayı bitir" halkalı |
| `/my-work` görevli adımlar ve boş durum | 1 sütun | 2 sütun | 2 sütun | 2 sütun | Temiz | Temiz | Temiz | Temiz | "Adıma git" halkalı |
| Faz 1-4 gerilemesi (17 sayfa, select ve PIN içerenler; prototip ve ilk giriş 5503) | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | Ölçülmedi | Ölçülmedi | Ölçülmedi |

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. `document.fonts.check` Hanken Grotesk ve Archivo true; sayaç ve süreler mono rakam, sans birim.
- Palet: UYGUN. Yeni ham renk yok (grep 0); mola bandı uyarı kenarlığı, en düşük kontrast 5,28:1. Koyu tema dialog perdesi açık renk: B2 (DÜŞÜK, `_MediaDialog` ile aynı kalıp).
- Köşe ölçeği: UYGUN. Kart, select ve dialog 2 px köşe ve kenarlık; göz düğmesi iç köşe 0.
- Gezinme arketipi: UYGUN. Değişmedi; mola ekranı ve İşlerim alt çubukta "İşlerim" etkin.
- Sayfa iskeleti: UYGUN. Run ana kolon + bağlam rayı (Molalar, Malzeme), lg altında ray altta; mola ekranı tek kolon 42 rem.
- Veri dili: UYGUN. Rozetler ikon + metin; boş durumlar ("Bu kayıtta mola verilmedi.", "Başka bir kayıtta görevli olduğunuz bekleyen adım yok."); ParticipantPicker yükleniyor/boş/hata/dolu metinleri.
- Hareket imzası: UYGUN. Canlı nokta molada duruyor (`data-paused`), reduced-motion'da animasyon ve geçiş kapalı; dialogda animasyon yok.
- İmza bileşen (kayıt şeridi): UYGUN. Faz hücreleri, `<progress>` minimuma göre, canlı net; çalışan adım 56 px "Adımı bitir"; kilitli adım kesikli ve nedenli; mola açıkken bant, sayaçlar durmuş ve metinle "Sayaçlar durdu". Bantta "Mola süresi" sayacının ilerlemesi (frontend sapma notu) K17 ve K-39 ile uyumlu: kriter faz ve kayıt sayaçlarının durmasını ister, mola süresi bu sayaçlardan değil; bant metni "Faz ve adım süreleri ilerlemez" diye ayırıyor. Mola açıkken "Adımı bitir" devre dışı maddesi Faz 7 (adım içi mola, PL-57).
- Anti-slop taraması (`DesignDna.md §4`): temiz. Gradient, glow, cam yüzey (yalnız üst bar), eyebrow, sürüm rozeti, emoji, em-dash, CDN yok; demo veri alana uygun.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | DÜŞÜK | SH-14 canlı süre açık molada 1 sn oynuyor. `CalculateLiveElapsed` iki terimi ayrı ayrı aşağı yuvarlıyor (`WholeSeconds(now - start) - OverlapSeconds(...)`), açık mola süresince değer sunucu anının saniye kesrine göre 2 ya da 3 çıkıyor. Ekranda "Sayaçlar durdu" yazarken yenilemede faz sayacı 2 -> 3 -> 2 değişebiliyor; XML yorumu "fark sabit kalır: değer artmaz" diyor. Kapanmış süreler (adım, faz, kayıt) tutarlı | `Evidence/Faz-5/KabulKriterleri.txt` T4 (kayıt 4, altı ardışık GET: 2, 2, 2, 2, 3, 2); `SureFormul.txt` son satır; `Services/DurationCalculator.cs:173`, `Services/IDurationCalculator.cs:193` | backend-engineer | Canlı değerde farkı tick düzeyinde alıp tek kez yuvarlamak: `WholeSeconds((now - start) - TimeSpan.FromTicks(overlapTicks))` (örtüşme tick toplamını döndüren özel yardımcıyla); yorumdaki "değer artmaz" cümlesi kodla aynı kalır | AÇIK |
| B2 | DÜŞÜK | Koyu temada dialog perdesi açık renk: `backdrop:bg-ink/60` koyu temada `--ink` açık olduğu için sayfa %60 açık gri tülle örtülüyor (rgba(228,234,239,0.6)); açık temada koyu perde. Süre altı dialogunda ve medya dialogunda aynı | `Evidence/Faz-5/Dialog-768-Koyu.png`; `JsDavranis.txt` k27 `backdrop` değerleri; `Views/Records/Run.cshtml:590`, `Views/Shared/_MediaDialog.cshtml:1` | frontend-engineer | Perde için iki temada da koyu kalan bir token (ör. `--scrim`) ya da koyu temada ayrı değer; iki dialogda aynı sınıf | AÇIK |

## 7. Geçenler

- Uçtan uca KSA akışı iki görevliyle: adım, faz ve kayıt süreleri ile efor veritabanı zamanlarından elle hesapla birebir (kayıt 8: faz 14/0/14 ve 10/6/4, kayıt brüt 24, mola 6, net 18, efor 19).
- Ö-03 formülü yayınlanan `DurationCalculator` üzerinde, kısa sürelerle uygulamada ve gerçek 40 dk molayla.
- Ret kuralları (sıra, tek devam eden adım, BR-24, BR-11, BR-46, BR-38, BR-39, BR-40, SK-1, SK-2, SK-6): ret olay ve komut bırakmıyor; nedenler kişi ve kayıt no ile.
- Eşzamanlılık: ortak görevlili iki kayıtta başlatma ve aynı kayda iki mola, her ikisinde tek etki.
- G9: altı komut aynı CommandId ile tek etki ve tek olay.
- Olay zinciri: Development'ta 12 olay bağımsız HMAC-SHA256 hesabıyla birebir (anahtar bellekte okundu, yazdırılmadı); Production'da 138 olayda önceki özet bağlantısı doğru, sıra boşluksuz, payload'da PIN yok.
- JS: LiveTimer (`TimersRunning`), ParticipantPicker dört durum ve K-41 uyarısı, PinInput yazma ve yapıştırma, CommandForm ret ve başarı, K27 dialog; JS kapalı görevli ekleme ve PendingParticipant.
- App.js ve CommandForm değişikliği sonrası Faz 3 düzenleyici (medya yükleme, adım kaydı, yayınlama) ve Faz 4 yeni kayıt ile Run malzeme JS akışlarında gerileme yok.
- Ön tur: G11, G12, G5a, G5b, mekanik eval ve §4 temiz; FE 10 sonrası Run ve FinishStep yeniden ölçüldü.

## 8. Kapsam dışı / yapılamayan

- K16a, K16b (adım sürerken mola) ve mola açıkken "Adımı bitir" reddi: Faz 7 (PL-57). SC-37 formülü bilgi için uygulandı: 40 dk, 10 dk, 20 dk (`SureFormul.txt`).
- Ön tur ölçümleri (Faz 1-4 gerilemesi, İşlerim, mola ekranı) FE 10 öncesi yayında (`.qa/f5/build`) alındı; FE 10 yalnız Run sonu, FinishStep, CommandForm, App.js ve `.finish-dialog` CSS'ini değiştirdiği için bu sayfalar yeniden ölçülmedi, Run ve FinishStep yeni yayında yeniden ölçüldü. Ön turda eski FinishStep sayfası için alınan G12 ölçümü (koordinatör notu üzerine) değerlendirmeye alınmadı.
- 1920 px ve %200 zoom Faz 1-4 sayfalarında ölçülmedi (bu fazda değişmedi); dialog 1920'de ayrıca ölçülmedi (en 36 rem ile sınırlı).
- Chromium dışı tarayıcı, gerçek dokunmatik cihaz ve ekran okuyucu ile tur yapılmadı.
- Gözlem (Faz 5 kapsamı dışı, şiddet verilmedi): çıkıştan sonra eski oturum çerezi yeniden gönderilince `/my-work` 200 dönüyor (T3 K23 satırı, "eski çerezle /my-work -> (200, '')"); çerez kimlik doğrulaması sunucuda oturum iptali tutmuyor. Analistin Faz 1 kararına göre değerlendirmesi için yazıldı.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | K18 pasifleştirme ekranındaki açık işler (kayıt sahibi, görevli adım, açık mola) bağlantısız listeleniyor; ekranı kullanan Yönetici ve Süper admin `/records/run` adresine giremediği için bağlantı verilemiyor. Faz 4 F1-A1 kararıyla aynı ("bağlantı görüntüleyene göre"); kayıt detayı Faz 6'da gelince bağlantı eklenebilir | analyst | Kriter metni "bağlantıyla listeler" diyor; hedef ekran Faz 6'da |
| A2 | B1, B2 (DÜŞÜK) | backend-engineer, frontend-engineer | Yazılı gerekçeyle ertelenebilir; analist kararı |

## 10. Analist kararı

```
FAZ 5: TESLİM ALINDI
Tarih: 2026-11-15
Gerekçe: K1-K27, devreden kalemler ve ortak kriterler (46/46) kanıtla eşleşti; KRİTİK, YÜKSEK, ORTA açık bulgu 0; QA önerisi GEÇTİ.
         DÜŞÜK B1 ve B2 Workflow.md §3 gereği yazılı gerekçeyle Faz 6'ya devredildi.
Açık kalemler: B1 (backend-engineer), B2 (frontend-engineer), A1 (Faz 6) aşağıda; Faz 6'da F5-B1, F5-B2, F5-A1.
```

Analistin doğruladıkları (kanıt dışında): `DurationCalculator.cs:173` `WholeSeconds(now - start) - OverlapSeconds(...)` iki terimi ayrı aşağı
yuvarlıyor; `IDurationCalculator.cs:190` yorumu tek farkı anlatıyor. B1 doğru; değer kaydedilen süreleri etkilemiyor (bitişte yazılan süreler
SH-01, SH-06 ile ayrı hesaplanır, QA elle birebir doğruladı), yalnız açık moladaki canlı gösterim 1 sn oynuyor.

Devir gerekçesi: B1 ve B2'yi teslimden önce düzeltmek QA'nın ölçmediği kodu faz commit'ine sokar; B1 süre servisindedir. İkisi de Faz 6'nın ilk
işidir (Faz 6 yürütme ekranının dialoglarına ve süre servisine dokunuyor) ve Faz 6 teslim testinde ölçülür:

- F5-B1 (backend-engineer): `CalculateLiveElapsed` farkı tick düzeyinde alıp tek kez yuvarlar; XML yorumu kodla aynı. Ölçüm: açık molada Run
  yenilemelerinde faz ve kayıt sayacı değişmez.
- F5-B2 (frontend-engineer): dialog perdesi iki temada koyu kalan token; ölçüm: koyu temada perde rengi ve kontrast.
- F5-A1 (A1): pasifleştirme ekranındaki açık işlerde kayıt bağlantısı Faz 6 detayına gider (PL-16).

Kapsam dışı gözlem (Faz 1 kodu): çıkıştan sonra eski oturum çerezi yeniden gönderilince istek kabul ediliyor. Paylaşılan cihazda tarayıcı çerezi
siler, risk çerezin kopyalanmasına bağlıdır; ORTA olarak Faz 6'ya alındı (PL-71). Faz 1 raporu değişmez.

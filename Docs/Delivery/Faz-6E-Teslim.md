# Faz 6E Teslim Raporu: Görevli girişi ve adım içi katılım

| Alan | Değer |
|---|---|
| Faz | 6E |
| Tarih | 2026-11-17 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `d50be07`; tur 2 kopyası `.qa/f6e/t2/src`, FE tur 2 düzeltmeleri dahil; Faz 6E kodu commit'lenmemiş, yerleşim ön turunun yedi düzeltmesi dahil). Kaynak test başında `.qa/f6e/qasrc` altına kopyalandı, oradan derlendi. Devir notları `Docs/Handover/backend-engineer-20261116-2359.md`, `Docs/Handover/frontend-engineer-20261116-2359.md` |
| Ortam | LOCAL, Release build, port 5871 (Production), DB: `.qa/` kopyası. `.qa/f6e/qa.db` = `.qa/f6d/qa.db` dosyasının SQLite yedekleme API kopyası; `20261116200234_AddStepWorkIntervals` yalnız bu kopyaya `dotnet ef database update --connection` ile uygulandı. Göç öncesi karşılaştırma için ikinci kopya `.qa/f6e/pre6e.db`. `EventChain__HmacKey` geliştirme user-secrets değerinden yalnız süreç ortamına verildi. B1 karşılaştırması için Faz 6D QA yayını (`.qa/f6d/build4`) port 5872'de 6E öncesi kopyayla kısa süre çalıştırıldı. Chromium (Playwright 1.61). Gerçek `App_Data/vaka.db`'ye ve PID 32568'e dokunulmadı |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2; B1 ve B3 kapandı, B2 analist kararıyla kapandı; yeni DÜŞÜK B4 açık. Tur 1: GEÇTİ, ORTA B1 açıktı) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-17). Tur 1: İADE (PL-102). B4 Faz 6F'ye devredildi (PL-103) |

**Test verisinin kurulumu.** DB'ye doğrudan yazım yapılmadı. Her şey uygulamanın uç noktalarıyla kuruldu: sayfadaki form okunup kendi belirteciyle gönderildi (JS kapalı tarayıcının gönderdiği gövde, `.qa/f6e/j/sc.py`). K4, K17 ve ön tur 7 adımları tarayıcıda yapıldı. Kişiler: X = `2002` (sahip), Y = `2004` IŞIL ÖZGÜR, Z = `2003` Şükrüye İnceoğlu Karamustafapaşalıoğulları (uzun ad), `2006` Ahmet Yılmaz (Faz 6E öncesinden kayıt 10'da çalışan), `2001` Test Operatör 1 (molada). Bütün test kişilerinin PIN'i aynı olduğundan "sahibin PIN'i reddedilir" denemesi için Y kendi PIN'ini `/account/security` ekranından değiştirdi.

- Kayıt 34 (X, M02, planlı): K7 ve K5 gerilemesi (adım 1.1), SC-32 ile K1, K2, K3 ve K5 (adım 2.1), K4, K6, K8 ve K10b (adım 2.2), K9 (adım 2.3), K10 ve K11 (adım 3.1), K12 (Yönetici yarıda kapattı).
- Kayıt 9 (`2001`): mola bitirildi, Y adım 35'e eklendi. Y, kayıt 34'te girişini yapmamışken bu adımı başlattı (K10b). Sonra `2001` yeniden molaya girdi.
- Kayıt 10 (`2006`): Faz 6E öncesinden aralıksız devam eden adım 38 bitirildi.
- Kayıt 35: acil açılış (K14). Kayıt 36 (X, KSA): JS kapalı tam akış (K17), kapı ekranları ve ayrılmış kişinin bitirme denemesi.

## 1. Kapsam

- DB 1: `StepWorkIntervals` tablosu, FK'lar, kişi başına unique, değişmezlik tetikleyicileri; `StepWorkInterval` entity, `StepParticipant.WorkInterval` (DB-2026-009, Eklemeli).
- BE 2: `ParticipantService` ekleme (adım sürerken de) ve çıkarma (yalnız katılmamış kişi); `JoinAsync`, `LeaveAsync`; `ParticipantJoined`, `ParticipantLeft` olayları; komut kimliği.
- BE 3: başlatmada girişi bekleyen kişi engel değil; başlangıçta aralık açılması; bitirme kapısı (`StepFinisher`); bitişte (`StepFinished`) ve müdahalede (`Interrupted`) aralık kapanışı.
- BE 4: `IDurationCalculator.CalculateParticipantWork` (SH-02a) ve `CalculateStepEffort(IReadOnlyList<int>)` (SH-02), formüller XML yorumunda.
- BE 5: B5 (`IRecordEventReader`, `RecordEventReader`); okuyucularda görevli durumu, eylem bayrakları, `PendingNames`, `CannotFinishReason`, `AwaitingMyEntry`, detayda kişi aralıkları; çizelgede katılma ve ayrılma satırları.
- FE 6: B7 (`FormValidation.describe`); `_Participants.cshtml` kişi kartları, durum rozeti, kişinin adıyla Katıl ve Ayrıl PIN formu, "Listeden çıkar", neden metni; `Run.cshtml` pasif Bitir ve kapı uyarısı.
- FE 7: İşlerim "Girişiniz bekleniyor" kartı; detay görevli satırı (Eklenme, Katılma, Ayrılma, Çalışma, "Adım boyunca"); `_Timeline.cshtml` iki yeni tür; `Styles/app.css`.
- Ön tur düzeltmeleri (7 madde): tek sütun kart, 320 px'te kırılmayan rozet ve düğme, G11 metni, kapı uyarısı `.run-action` içinde, `data-state="Idle"`, PIN'i kilitli kişide listenin görünmesi, kart hata kutusu ve `#step-N-pU` kaydırması.

**Plandan sapmalar (karar analistin):**

- Backend, dosya sınırı dışı, `Services/**` içinde: `RecordStateAudit` ve `RecordStateRules` değişti; bütünlük denetimi adım sürerken katılanları tanıyor. Yeni dosyalar: `StepWorkCloser` ve `IStepWorkCloser`, `ParticipantStateRules`. Ayrıca `StepQueries` (`WorkInterval` include) ve `Program.cs` kayıtları değişti. Etkisi ölçüldü: 36 kaydın hepsi "Doğrulandı" (K14).
- Ayrılmış kişi adımı bitiremez (planda yok). Ölçüldü: ayrılan `2006`'nın ekranında Bitir formu yok, elle POST 403, olay yazılmadı (`Evidence/Faz-6E/AkisKaniti.txt`, son bölüm).
- `IStepStartApplier.CheckParticipantsAsync` imzası `(step, starterUserId, ownerUserId)` oldu, acil açılış da buna göre güncellendi. `IDurationCalculator.CalculateStepEffort(int, int)` kaldırıldı; yerine `CalculateStepEffort(IReadOnlyList<int>)` ve `CalculateParticipantWork` geldi.
- Frontend, dosya sınırı dışı: `wwwroot/js/App.js:54` ve `wwwroot/js/CommandForm.js:78` odak seçicisinin başına `[data-focus-first]` eklendi. Diğer formlarda gerileme yok; run sayfasındaki odak kaybı bu satırlardan değil, adım sürerken açılan ekleme panelinden geliyor (B1).

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | X, Y girişini yapmadan 2. fazın adımını başlatır; `StepStarted` görevlileri yalnız X; panelde Y "Girişi bekleniyor" | GEÇTİ | `AkisKaniti.txt` log-a2: adım 154 Durum 2, `StepStarted.ParticipantUserIds:[4]`. Y satırında `IsWorking=0` ve aralık yok. Panel metni "IŞIL ÖZGÜR 2004 Girişi bekleniyor, Kendi PIN'iyle katılınca çalışma süresi başlar." |
| K2 | Y girişsizken Bitir reddedilir, "Girişi bekleniyor: <Y>." görünür, `StepFinished` yok, elle POST reddedilir, sonraki adım başlatılamaz | GEÇTİ | Panelde Bitir pasif (`disabled`, `aria-describedby="finish-gate-N"`), uyarı aynı `.run-action` içinde. Elle POST 200 ret, metin "Girişi bekleniyor: Ahmet Yılmaz, Şükrüye İnceoğlu Karamustafapaşalıoğulları. Kişi kendi PIN'iyle katılsın ya da sahip listeden çıkarsın." (log-k2b), yeni olay yok. Sonraki adımın POST'u "Kayıtta devam eden bir adım var. Önce onu bitirin." ile reddedildi. Adım 154'te de olay yazılmadı (log-a2) |
| K3 | Aynı tablette Y'nin PIN'iyle katılma; aralık ve boş `ConfirmedAtUtc` katılma anı; olayın yapanı Y, `SessionUserId` X; sahibin PIN'i ve yanlış PIN reddedilir, sayaç Y'nin | GEÇTİ | Sahibin PIN'i: Y sayacı 0'dan 1'e, X sayacı 0'da kaldı. `0000`: Y sayacı 2, X 0. Doğru PIN ile `StartedAtUtc` = `ConfirmedAtUtc` = 20:55:11.1623065. Olay `ParticipantJoined` (yapan 7, `SessionUserId` 4, `BeforeStart` false). Başarıdan sonra Y sayacı 0 |
| K4 | Y kendi oturumunda İşlerim kartından panele gider ve kendi PIN'iyle katılır; `SessionUserId` Y | GEÇTİ | Tarayıcı, 390 px: kart "Girişiniz bekleniyor", bağlantı `/records/run/34#step-155`. Açılan sayfada adım görünür (üst 80 px). İpucu "Kendi PIN'inizi girin." Katıldıktan sonra kart "Çalışıyor". Olay `ParticipantJoined` (yapan 7, `SessionUserId` 7). Konsol 0 |
| K5 | Efor = adım süresi + Y'nin (bitiş - katılma); kişi `WorkSeconds` toplamına eşit; adımın tamamında iki kişide süre x 2; süre kişilerden bağımsız | GEÇTİ | SC-32 ölçekli (10 dk yerine 60 sn, 4. dakika yerine 24. sn): adım 154'te süre 59, X 59, Y 35 (20:55:11.16'dan 20:55:46.80'e), efor 94 = 59 + 35. Adımın tamamında iki kişi (adım 152): 19 x 2 = 38. SQL denetimi (`log-sql`): biten 10 adımda `EffortSeconds` = Σ `WorkSeconds`; 17 kapalı aralıkta yazılı süre = floor(min(ayrılma, bitiş) - max(katılma, başlangıç)); fark 0 |
| K6 | Y kendi PIN'iyle ayrılır, aralık ayrılma anında kapanır, yeniden katılamaz, çalışan son kişi ayrılamaz | GEÇTİ | Adım 155: Y aralığı `EndKind` 0 (Left), 21 sn, olay `ParticipantLeft` (`SessionUserId` 7, `WorkSeconds` 21). Yeniden katılma POST'u: "IŞIL ÖZGÜR adımdan ayrıldı, aynı adıma yeniden katılamaz." Son çalışan X'in ayrılma POST'u: "Adımda çalışan son kişi ayrılamaz. Adımı bitirin." Adım bitince Y'nin süresi 21 sn kaldı, efor 146 = 125 + 21 |
| K7 | Başlamadan katılan Z adım başlayınca çalışmaya başlar; çizelgede "X ve Z birlikte adıma başladı" | GEÇTİ | Adım 152: başlamadan önce Z "Katılacak", `ConfirmedAtUtc` dolu, aralık yok. Başlangıçta X ve Z aralığı aynı anda açıldı (20:53:31.8036522), `StepStarted.ParticipantUserIds:[4,6]`. Çizelge: "Test Operatör 2 ve Şükrüye İnceoğlu Karamustafapaşalıoğulları birlikte adıma başladı" (log-k15) |
| K8 | X adım sürerken girişsiz Z'yi çıkarır, adım bitirilebilir olur; katılmış Y'yi çıkarma reddedilir | GEÇTİ | Adım 155: `ParticipantRemoved` (Z), ardından Bitir başarılı. Y'yi çıkarma POST'u: "IŞIL ÖZGÜR adıma katıldı, listeden çıkarılamaz." |
| K9 | X adım sürerken Z'yi ekler; Z girişine kadar efor almaz ve kapıya takılır | GEÇTİ | Adım 156: başlangıçtan 3 sn sonra `ParticipantAdded`. Bitirme POST'u kapı metniyle reddedildi. Z katıldıktan sonra Z'nin süresi 4 sn (katılma 20:58:28.07, bitiş 20:58:32.09), efor 16 = 12 + 4 |
| K10 | PIN'i kilitli, molada ve başka kayıtta çalışan kişide "Katıl" yerine neden; elle POST reddedilir, olay yok; girişsiz Y başka kayıtta adım başlatabilir | GEÇTİ | Adım 157 panelinde: "Test Operatör 1 molada. Mola bitince katılabilir.", "Ahmet Yılmaz başka kayıtta devam eden adımda çalışıyor.", "Şükrüye İnceoğlu Karamustafapaşalıoğulları PIN'i kilitli. Yöneticiye başvurun." Üçünün doğru PIN'li POST'u aynı metinle reddedildi, yeni olay yok. Z'nin kilidi X tabletinden 5 yanlış denemeyle oluştu; sayaç Z'de 5, X'te 0. K10b: Y, kayıt 34 adım 155'te girişsizken kayıt 9 adım 35'i başlattı (`StepStarted` yapan 7). Bu sırada adım 155'te Y "başka kayıtta devam eden adımda çalışıyor" oldu, POST reddedildi |
| K11 | Sahip ya da kişinin kendisi olmayan Operatör 403; Yönetici ve Denetçi 403; bitmiş adıma katılma reddedilir; aynı komut kimliği tek olay | GEÇTİ | `2006`'nın Y adına katılma ve X adına ayrılma POST'ları 403. Yönetici, Denetçi ve Süper admin 302 ile `/account/access-denied`'e gidiyor, o sayfa 403 (`YetkiMatrisi.txt`). Açık kayıtta bitmiş adım 159: "Adım bitti, görevli işlemi yapılamaz.", olay yok. Aynı `CommandId` ile iki istek: ikisi de 302, tek `ParticipantJoined` (sıra 31) |
| K12 | Y çalışırken kayıt yarıda kapatılırsa Y'nin aralığı müdahale anında `Interrupted` ile kapanır, efor kapanan aralıklardan | GEÇTİ | Yönetici "Yarıda kapatma" (Tür 2): X ve Y aralıkları 21:04:48.8904688'de `EndKind` 2. Süreler X 349, Y 329; `StepInterrupted` efor 678 = 349 + 329; `RecordAborted` |
| K13 | Dolu zamanı değiştiren ve satır silen SQL tetikleyiciyle, ikinci aralık unique ile reddedilir | GEÇTİ | `K13Tetikleyici.txt` (ayrı kopya): StartedAtUtc ve EndedAtUtc güncellemesi "Çalışma aralığının zamanı değiştirilemez.", silme "Çalışma aralığı silinemez.", ikinci aralık `UNIQUE constraint failed: StepWorkIntervals.StepParticipantId` |
| K14 | Faz 6E öncesi kayıtlarda efor değişmez, "Adım boyunca", bütünlük "Doğrulandı"; acil açılış ve ilk adım değişmeden çalışır | GEÇTİ | `pre6e.db` ile karşılaştırma: kayıt efor, net ve brüt değeri değişen satır 0. Adım düzeyinde değişen tek satır, bu testte çalıştırılan adım 35. Göçten hemen sonra ve test sonunda 36 kaydın hepsi "Doğrulandı". Kayıt 8 detayında "Adım boyunca" 6 kez. Acil kayıt 35 ilk adımı otomatik başladı, X aralığıyla açıldı ve bitti; kayıt "Doğrulandı". Eski adım 38 (aralıksız) bitince başlangıçtan kapanan aralık yazıldı: süre = efor = 12224, kayıt 10 "Doğrulandı" |
| K15 | Detay görevli satırında katılma, ayrılma, çalışma süresi; çizelgede katılma ve ayrılma satırları | GEÇTİ | Detay 2.2, Y için: "Eklenme 23:56, Katılma 23:57, Ayrılma 23:58, Çalışma 21 sn". Ayrılmayan kişide "Adım bitişine kadar". Çizelgede "adıma katıldı", "adıma katılacağını onayladı", "adımdan ayrıldı" ve süre satırları var. Oturum başkasınınsa "Cihazdaki oturum: Test Operatör 2" yazıyor. `StepFinished` ayrıntısında "Görevliler: Test Operatör 2 (59 sn), IŞIL ÖZGÜR (35 sn)" (log-k15). Çizelge satırı hizası: işaret ve saat sol kenarı ile işaret-saat üst farkı diğer satırlarla aynı (6 genişlik x 2 tema) |
| K16 | Hata üç kez açılıp kapandıktan sonra hata kimliği en çok bir kez, hata yokken hiç; seçici, `/users/create`, `/materials/create` | GEÇTİ | `ErisilebilirlikKaniti.txt` K16: dört döngüde açıkken 1 ve görünür, kapalıyken 0, son listede hata kimliği yok. Üç formda da aynı, konsol 0 |
| K17 | JS kapalı ekleme, katılma, ayrılma, bitirme; panel, İşlerim, detay 360-1440 açık ve koyu yatay kaydırmasız; durum metinle; G12 kusuru 0; konsol 0 | GEÇTİ | JS kapalı tarayıcıda kayıt 36 adım 159 tamamlandı: ekleme (arama ve radyo), başlatma, kapı, katılma, ayrılma, bitirme; efor 4 = 2 + 2. Yatay kaydırma 0 (`ErisilebilirlikKaniti.txt`). Matris 150 hücre, 320-1920 px, iki tema, `hScroll` false; konsol 0 (`ResponsiveMatris.txt`). Durumlar ikon ve metinli rozetle ayrılıyor. G12: kart içi sol hizalar, tam genişlik özetler, rozet satırı, PIN göz düğmesi ve kapı uyarısı-düğme kenar farkı 0 |

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`.qa/f6e/qasrc`). `dotnet publish -c Release -o .qa/f6e/build` başarılı |
| CSS | `npm run build:css` | Aynı Tailwind CLI ile kopyada derlendi; çıktı repodaki `wwwroot/css/app.css` ile bayt bayt aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme. Sihirli rol metni 0. `[Authorize]` ya da `[AllowAnonymous]` taşımayan controller 0. MediatR, AutoMapper, generic repository 0. `Program.cs`'te `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun. Sınıf ve arayüzler: `StepWorkInterval`, `StepIntervalEndKind`, `StepWorkIntervalConfiguration`, `IStepWorkCloser`/`StepWorkCloser`, `ParticipantState`, `ParticipantStateRules`, `IRecordEventReader`/`RecordEventReader`, `JoinStepViewModel`, `LeaveStepViewModel`. Tablo ve kolonlar: `StepWorkIntervals` (`StepParticipantId`, `StartedAtUtc`, `EndedAtUtc`, `EndKind`, `EndedByUserId`, `WorkSeconds`). Tetikleyiciler: `TR_StepWorkIntervals_NoTimeChange`, `TR_StepWorkIntervals_NoDelete`. CSS `.step-person`, `.step-person-name` kebab-case |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (`Views/**`, `Styles/app.css`, `wwwroot/js/**`). Yeni PIN alanı `autocomplete="off"`, `inputmode="numeric"`, `spellcheck="false"` taşıyor |
| Mekanik eval | `UiDelivery.md §3` | 162 hücre (çalıştır, İşlerim, detay; 320, 360, 640, 768, 1024, 1280, 1440, 1920 px; açık ve koyu; reduced-motion). `h1` 1, `skip` true, `imgNoAlt`, `imgNoSize`, `inputNoLabel`, `iconBtnNoName` 0. `emdash` false. Font Hanken Grotesk (`fonts.check` true). `hScroll` false, `smallTargets` 0, 44 px altı hedef 0, `extScripts` [], konsol 0. Detayda 320 ve 360 px'teki `overflowing` (TR, TH) kart modunda görsel olarak gizli başlık satırı; Faz 6C ve 6D yöntemiyle sayımdan çıkarıldı. AA altı kontrast 0 |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Yeni migration `20261116200234_AddStepWorkIntervals` (çalışma ağacında, izlenmiyor), `DbChanges.md` DB-2026-009'da kayıtlı. `Up()` yalnız `CreateTable`, 2 `CreateIndex`, 2 `Sql` (tetikleyici); `Drop*` 0, ALTER yok (Eklemeli, onay gerekmez). Snapshot farkı yalnız ekleme |

Kod okuması (SOLID, §7, §8):

- `RecordStepsController.JoinStep` ve `LeaveStep` yalnız model bağlıyor, servisi çağırıyor ve sonucu HTTP'ye çeviriyor; iş kuralı serviste. Servisler constructor injection ile geliyor; `new` ile servis yok.
- `IParticipantService` dört üyeli ve amaca özel.
- `ParticipantService` 472 satır; §2'ye göre bölünme adayı. PL-101 (3) gereği yapısal bölünme bulgu sayılmadı.
- §7: `DurationCalculator.CalculateParticipantWork` (SH-02a: kişi başlangıcı = en geç (katılma, adım başlangıcı), kişi bitişi = en erken (ayrılma, adım bitişi), çalışma = fark - kesişen mola) ve `CalculateStepEffort` (SH-02: Σ kişi süreleri) formülleri XML yorumunda ve kodla uyuşuyor. `ParticipantService.LeaveAsync` aynı formülü yorumda yazıyor. `StepWorkCloser`, `StepFinisher` ve `InterventionService` süreyi hesaplayıcıya devrediyor; servislerde ayrıca süre aritmetiği yok.
- §8: değişen 49 kaynak dosyada emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager` adı, kalıp dil ve em/en-dash 0.

## 4. Yetki matrisi

Rollere karşılık: Viewer = Denetçi (3001), Editor = Operatör, Admin = Yönetici (1001); Süper admin (0001) ayrı yazıldı. Operatör sütunu üç oturumla ölçüldü: sahip (2002) / kişinin kendisi (2004) / başka operatör (2006). 403 = 302 `/account/access-denied` > 403 (projenin düzeni). POST'lar belirteçsiz 400 (dört route). Kanıt: `Evidence/Faz-6E/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| POST `/records/join-step/{stepId}` | 302 | 403 | sahip 200 (iş kuralı reddi) / kendisi değilse 403 / başka 403 | 403; Süper admin 403 |
| POST `/records/leave-step/{stepId}` | 302 | 403 | sahip 200 / kendisi değilse 403 / başka 403 | 403; Süper admin 403 |
| POST `/records/add-participant/{stepId}` | 302 | 403 | sahip 200 / 403 / 403 | 403; Süper admin 403 |
| POST `/records/remove-participant/{stepId}` | 302 | 403 | sahip 200 / 403 / 403 | 403; Süper admin 403 |
| GET `/records/run/{id}` | 302 | 403 | 200 / 200 / 200 | 403; Süper admin 403 |
| GET `/my-work` | 302 | 403 | 200 / 200 / 200 | 403; Süper admin 403 |
| GET `/records/details/{id}` | 302 | 200 | 200 / 200 / 200 | 200; Süper admin 403 |

Menüde gizli olup adresle açılan sayfa yok. Başka operatörün kişi adına katılma ve ayrılma isteği servis düzeyinde 403 (K11).

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/records/run/34` sahip, adım devam ediyor (çalışan, 3 nedenli, Çalışıyor, Ayrıldı, Katılacak kartları) | Temiz; 320 temiz (kart 224 px) | Temiz | Temiz | Temiz; 1280 temiz | Temiz | Temiz (tüm genişlikler) | Temiz; canlı nokta animasyonu 0 | 320 ve 640 temiz | İlk Tab "İçeriğe geç"; odak halkası her durakta, üst bar ve alt çubuk örtmüyor |
| `/records/run/34`, tüm `details` açık (PIN formları) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - | Temiz | Form içi input, göz ve gönder halkalı |
| `/records/run/36` kapı (girişi bekleyen ve PIN'i kilitli kişi, pasif Bitir) | Temiz; kapı uyarısı ve düğme sol/sağ farkı 0 | Temiz | Temiz | Temiz; 1280 temiz | Temiz | Temiz | - | 640 temiz | 3 kart özeti sırayla; 768'de 2006 oturumu temiz |
| `/records/run/34` Y ve Z oturumu (kendi kartı, neden) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - | Temiz | Temiz (1440) |
| `/my-work` "Girişiniz bekleniyor" | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | 640 temiz | Temiz (360 koyu) |
| `/records/details/34` görevli satırı ve çizelge | Temiz (sr-only başlık yanlış pozitifi hariç) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | 640 temiz | Temiz (360, 1440) |
| `/records/details/8` "Adım boyunca" | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | - | Temiz | - |
| Kart hata kutusu (Ayrıl formuna yanlış PIN) | JS açık: kutu görünüm içinde (üst 470), odak kutuda; JS kapalı: kaydırma var, odak yok (B2) | - | - | 1280: aynı | - | - | - | - | - |
| Run, "Adımı bitir" yanlış PIN | Odak BODY (B1) | - | - | - | - | - | - | - | - |

G11 imzası (devir notundaki düzeltilmiş metinle aynı, 320-1920 px, iki tema; kart listesi her genişlikte tek sütun):

- Ad:1@1 Rozet:1@2 Durum:1@3 (eylemsiz kart).
- Ad:1@1 Rozet:1@2 Durum:1@3 Eylem:1@4 (Katıl ya da Ayrıl).
- Ad:1@1 Rozet:1@2 Durum:1@3 Neden:1@4 Eylem:1@5 (nedenli bekleyen ve Çıkar).
- Rozet kırılması 0. 52 px üstü özet ya da düğme 0.
- Kart genişliği: 320'de 224, 360'ta 264, 768'de 580, 1024'te 248, 1280'de 424, 1440'ta 584, 1920'de 992 px.
- Detay kv: 320-1024 `Eklenme:1@1 Katılma:2@1 Ayrılma:1@2 Çalışma:2@2`; 1280 ve üstü `1@1 2@1 3@1 4@1`.
- Eski adım: 320-1024 `Eklenme:1@1 Çalışma:1@2`, xl `Çalışma:2@1` (3 sütun birleşik).

Ön tur düzeltmelerinin yeniden doğrulaması:

1. Tek sütun ve imza: tutuyor.
2. 320 px'te rozet ve düğme kırılmıyor; görevli bloğu damga sütununa taşıyor (kart 224 px): tutuyor.
3. G11 metni devir notunda düzeltilmiş.
4. Kapı uyarısı `.run-action` içinde, sol ve sağ fark her genişlikte 0.
5. Başlamamış adımda sahip kartı `data-state="Idle"`, kenarlık çizgi rengi (rgb 202,210,216).
6. PIN'i kilitli Z İşlerim'de kart görüyor; run sayfasında kartı ve "PIN'i kilitli. Yöneticiye başvurun." nedeni görünüyor.
7. Kart hata kutusu, açık `details` ve `#step-N-pU` kaydırması tutuyor. JS açıkken odak hata kutusunda; JS kapalıyken odak yok (B2).

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN (Hanken Grotesk gövde, `fonts.check` true; sicil no ve saatler mono).
- Palet: UYGUN (tek accent; Çalışıyor kenarlığı accent, bekleyen warn, diğerleri çizgi rengi; AA altı kontrast 0, iki tema).
- Köşe ölçeği: UYGUN (kart `rounded-card`, rozet ve düğme `rounded-ui`, 2 px kenarlık, gölge yok).
- Gezinme arketipi: UYGUN (değişmedi).
- Sayfa iskeleti: UYGUN (ana kolon + bağlam rayı; görevli kartları adım içinde).
- Veri dili: UYGUN (rozet tek biçim, ikon + metin + kenarlık; durum renkle değil metinle; hata kutusu düz hata kenarlıklı ve "İşlem yapılmadı" + neden).
- Hareket imzası: UYGUN (yeni hareket yok; canlı nokta reduced-motion'da duruyor).
- İmza bileşen: UYGUN (kayıt şeridinde çalışan adım, pasif "Adımı bitir" 56 px ve nedeni `aria-describedby` ile bağlı).
- Anti-slop taraması (`DesignDna.md §4`): temiz (gradient, glow, eyebrow yığını, sürüm rozeti, em/en-dash yok; ikonlar Lucide sprite'ından).

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | ORTA | Yürütme ekranında devam eden adımda "Adımı bitir" yanlış PIN ile reddedilince odak BODY'de kalıyor. Faz 6D'de odak `pin-finish-N` alanına gidiyordu (gerileme). Sunucu ModelState'teki `Pin` hatasını sayfadaki 9 PIN alanının hepsine `aria-invalid="true"` olarak yazıyor. Faz 6E'de ekleme paneli adım sürerken de açık olduğundan, belgede ilk eşleşen öğe kapalı `details` içindeki `pin-add-person-N`. `App.js:54` ve `CommandForm.js:78` ona odaklanamıyor. JS açıkken sayfa 1336 px'te kalıyor, üst bildirim görünüm dışında (-1170); alan hatası görünür. JS kapalıyken de aynı | `Evidence/Faz-6E/ErisilebilirlikKaniti.txt` (B1 karşılaştırması: 6E `focus: BODY`, 6D `focus: INPUT#pin-finish-38`), `Views/Records/Run.cshtml:388`, `Views/Records/_Participants.cshtml` ekleme paneli `_PinInput` | frontend-engineer | Odak seçicisinde kapalı `details` içindeki öğeleri atla (ör. `el.closest("details:not([open])")` olan adayı geç), ya da `aria-invalid`'i yalnız gönderilen komutun PIN alanına yaz (bekleyen komut kimliği ya da action ile). İlgisiz PIN alanlarının `aria-invalid` olması da böylece kalkar | KAPANDI (tur 2) |
| B2 | DÜŞÜK | JS kapalıyken Katıl, Ayrıl ya da Çıkar hatasında sayfa kart hata kutusuna kayıyor ve kutu görünüyor (üst 287 px, `role="alert"`), ama odak kutuya gitmiyor (BODY). Ekran okuyucu sayfa yüklemesinde uyarıyı okumayabilir. JS kapalı yol ikincil ve metin görünür olduğundan kapı etkisi yok | `ErisilebilirlikKaniti.txt` (err7, JS false; tur 1 ekran görüntüsü tur 2 kanıtına yer açmak için çıkarıldı), `Views/Records/_Participants.cshtml:117` | frontend-engineer | Kart hata kutusuna `autofocus` (zaten `tabindex="-1"`) eklenebilir; ya da JS kapalı yolda odak beklenmez diye analist kabul eder | KAPANDI (analist kararı, PL-102) |
| B3 | DÜŞÜK | Zaman çizelgesinde "<Ad> adımdan ayrıldı" satırındaki kişinin çalışma süresi "Süre" etiketiyle gösteriliyor. Aynı çizelgede "Süre" adım süresi; detayda aynı değer "Çalışma" adını taşıyor | `AkisKaniti.txt` log-k15 ("IŞIL ÖZGÜR adımdan ayrıldı ... Süre 21 sn"), `Views/Records/_Timeline.cshtml:77`, `Services/RecordTimelineBuilder.cs:146` | frontend-engineer | `ParticipantLeft` türünde etiket "Çalışma" olsun | KAPANDI (tur 2) |
| B4 | DÜŞÜK | (Tur 2) `FocusFirst.js:3` `element.checkVisibility()` desteğini sınamıyor, geri dönüşü yok. Bu yöntem olmayan tarayıcıda (Safari 17.4 öncesi; uygulamanın kullandığı `:has` Safari 15.4'ten beri var) her hata sonrası odak kayboluyor. İlk yüklemede (`App.js`) yakalanmamış `TypeError: element.checkVisibility is not a function` atılıyor; örnek giriş ekranı yanlış parola: odak BODY, konsolda 1 hata. Yürütme komutunda (`CommandForm.js:80`) hata `send()` içindeki `catch`'e düşüyor, `fail()` eski (DOM'dan çıkmış) formda çalışıyor; odak BODY, görünür etki yok. Faz 6D'de bu tarayıcılarda giriş hatası odağı çalışıyordu. Proje belgelerinde tarayıcı desteği hedefi yazılı değil | `Evidence/Faz-6E/Tur2Kaniti.txt` ("checkVisibility olmayan tarayıcı benzetimi": Bitir 360 ve 1440 `focus: BODY`; giriş `TypeError`), `wwwroot/js/FocusFirst.js:3` | frontend-engineer | `typeof element.checkVisibility === "function" ? element.checkVisibility() : element.getClientRects().length > 0 && !element.closest("details:not([open])")` gibi bir geri dönüş | AÇIK |

## 7. Geçenler

- K1-K17'nin hepsi; SC-32 ölçekli oynandı. SH-02a ve SH-02, 17 aralık ve 10 adımda SQLite sorgusuyla karşılaştırıldı, fark 0.
- Faz 6E öncesi 33 kaydın eforu, neti ve brütü değişmedi. Göçten hemen sonra ve test sonunda tüm kayıtlar (36) "Doğrulandı".
- Faz 6E öncesinden aralıksız devam eden adım 38 bitişte başlangıçtan kapanan aralığa dönüştü; efor = süre.
- Yetki matrisi: oturumsuz 302, rol dışı 403, belirteçsiz POST 400, başka operatörün kişi adına isteği 403.
- Ön tur düzeltmelerinin yedisi tarayıcıda yeniden ölçüldü (bölüm 5).
- Ortak `[data-focus-first]` değişikliğinde gerileme yok: giriş hatasında odak bildirimde, `/users/create` ve `/materials/create` boş gönderiminde ilk alanda. B1 bu değişiklikten değil.
- G5a: POST formlarında belirteç sayısı her formda 1 (run/36 sahip 9 form, 2006 4 form, İşlerim 3, detay 3).
- Görünür metinde em/en-dash 0 (eval `emdash` false, 162 hücre). Sunucu günlüğünde istisna 0.
- Dört durum: dolu (bütün kart durumları); boş ("Bu adıma görevli eklenmedi" metni Faz 6C'den değişmedi; sahip tek başınayken kendini çıkaramadığı için arayüzden kurulamadı); hata (kart hata kutusu, alan hatası, üst bildirim); yükleniyor (gönderimde düğme `disabled`, ağ hatasında `data-command-status` metni; yerleşim ön turunda ölçüldü, kod değişmedi).

## 8. Kapsam dışı / yapılamayan

- SH-02a'nın "kesişen kayıt molası" terimi kişi aralığıyla kesişen molayla denenemedi: adım sürerken mola kapsam dışı (O-14, K-39), adımlar arası mola aralıkla kesişmiyor. Formül kodda ve yorumda var.
- "Boş" görevli listesi arayüzden kurulamadı: sahip listede tek başınayken kendini çıkaramıyor (`CanRemove` birden çok kişi ister). Metin Faz 6C'den değişmedi.
- Yükleniyor durumu bu turda yeniden ölçülmedi. Yerleşim ön turunda ölçüldü (`data-command-form` düğmeyi pasifler, ağ hatasında metin görünür); `CommandForm.js`'deki tek değişiklik odak seçicisi.
- B1'in Faz 6D ile karşılaştırması 6D QA yayınıyla (`.qa/f6d/build4`) yapıldı; HEAD yeniden derlenmedi.
- Test sırasında QA kopyasında Y (`2004`, ön tur 7 ölçümünde) ve Z'nin (`2003`, K10 için bilerek) PIN'leri kilitlendi. Kaynak DB etkilenmedi.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| - | yok | - | Tur 2 sonunda yalnız DÜŞÜK B4 açık; erteleme ya da düzeltme kararı analistin |

## 10. Analist kararı

```
FAZ 6E: İADE
Tarih: 2026-11-17
Gerekçe: K 17/17; SC-32, SH-02a ve SH-02 SQLite ile fark 0; eski kayıtların eforu ve bütünlüğü değişmedi. ORTA B1 açık: Faz 6D'ye göre odak
  gerilemesi (UiDelivery §2). Kendim okudum: App.js:54 ve CommandForm.js:78 ilk [aria-invalid="true"] öğeye odaklanıyor, bu öğe kapalı
  details içindeki ekleme paneli PIN'i olunca odak BODY'de kalıyor; _Timeline.cshtml:77 her süreyi "Süre" etiketiyle yazıyor;
  StepFinisher.cs:134 sahip her durumda bitirebiliyor, ayrılmış görevli bitiremiyor.
Sapmalar kabul: RecordStateAudit ve RecordStateRules değişikliği (bütünlük denetiminin adım sürerken katılanı tanıması gerekiyordu;
  36 kayıt "Doğrulandı"), StepWorkCloser, ParticipantStateRules, StepQueries ve Program.cs; imza değişiklikleri (acil açılış uyarlandı);
  ayrılmış görevlinin bitirememesi (ayrılan çalışmayı bırakmıştır; sahibin SK-1 hakkı sürer, StepFinisher.cs:134); App.js ve
  CommandForm.js'e [data-focus-first] eklenmesi.
İade maddeleri (tur 2, tek tur):
  B1 ORTA, frontend-engineer: App.js ve CommandForm.js odak seçicisi kapalı details içindeki ya da görünmeyen adayları atlar ve görünür
    ilk hatalı alana odaklanır (alternatif: aria-invalid yalnız gönderilen formun PIN'ine). JS açık yol için şarttır; JS kapalı yolda
    odak beklenmez, alan hatası görünür olmalı (B2 ile aynı ilke).
  B3 DÜŞÜK, frontend-engineer: ertelenmez (sunumda görünen çizelge, tek etiket). ParticipantLeft satırında etiket "Çalışma".
  B2 DÜŞÜK: analist kararıyla kapandı. JS kapalı yolda odak beklenmez; hata kutusu görünür ve kaydırılmış, metin role="alert" (PL-102).
Yeniden denetim şartı (yalnız bunlar): B1 Run'da adım sürerken açık ekleme paneliyle "Adımı bitir" yanlış PIN: odak pin-finish-N, bildirim
  görünür, 360 ve 1440 px; aynı kontrol "Katıl" yanlış PIN'de ve bir başka sayfada (/users/deactivate/{id} boş PIN) gerileme olarak.
  B3 ayrılma satırında "Çalışma", bitiş satırında "Süre" ve "Efor" değişmemiş. Değişen dosyalarda grep tablosu, konsol 0.
```

```
FAZ 6E: TESLİM ALINDI (tur 2)
Tarih: 2026-11-17
Gerekçe: K 17/17; B1 ve B3 kapandı, B2 kararla kapandı, gerileme yok; açık KRİTİK, YÜKSEK, ORTA yok. Kendim okudum: FocusFirst.js:3
  element.checkVisibility() koşulsuz çağrılıyor (B4).
Açık kalemler (devir):
  B4 DÜŞÜK, frontend-engineer: Faz 6F'ye devredildi (FE kalem, K14). Gerekçe: projede tarayıcı hedefi yazılı değil ve hedef
    Chromium'da etki yok; Faz 6F giriş ekranına (E-01) dokunuyor ve son regresyonu (K13) kapsıyor; tur 3 sunum gününde kullanıcı
    onayı ve ek tur ister. Düzeltme: checkVisibility yoksa getClientRects().length > 0 ve kapalı details kontrolü.
```

## Tur 2 (2026-11-17)

**Kapsam.** Yalnız analistin yeniden denetim şartı: B1 (yürütmede adım sürerken açık ekleme paneliyle "Adımı bitir" yanlış PIN, 360 ve 1440 px), gerileme ("Katıl" yanlış PIN, `/users/deactivate/{id}` boş PIN), B3 (çizelge etiketleri), değişen dosyalarda grep tablosu ve konsol. Koordinatörün isteğiyle iki ek kontrol: FE'nin bir kez gördüğü soğuk başlatma durumu tekrarlandı; `checkVisibility()` desteği olmayan tarayıcıdaki davranış kod okumasıyla değerlendirildi ve benzetimle ölçüldü. B2 analist kararıyla kapandı (PL-102), yeniden test edilmedi.

**Ortam.**
- Çalışma ağacı `.qa/f6e/t2/src`'ye kopyalandı. Değişen dört dosya (`wwwroot/js/FocusFirst.js` yeni, `App.js`, `CommandForm.js`, `Views/Records/_Timeline.cshtml`) repodakiyle bayt bayt aynı.
- `dotnet build -c Release -warnaserror` 0 uyarı, 0 hata. CSS çıktısı repodakiyle bayt bayt aynı. Yayın `.qa/f6e/t2/build`, port 5881.
- DB: `.qa/f6e/qa.db`'nin yeni kopyası `.qa/f6e/t2/base.db`. Her soğuk denemede bu dosya `.qa/f6e/t2/qa.db` olarak yeniden kopyalandı ve sunucu yeniden başlatıldı.
- Kişiler: sahip `2002`, katılma denemesi için `2001` (molası arayüzden bitirildi, `2002` adım 160'a ekledi), Yönetici `1001`. IŞIL ve Şükrüye kullanılmadı.
- Yalnız kendi PID'lerim kapatıldı, 5881 boş; 32568'e dokunulmadı.

**Sonuçlar.**

| Madde | Ölçüm | Sonuç | Kanıt |
|---|---|---|---|
| B1, 360 px | Kayıt 36 adım 160 devam ediyor, ekleme paneli açık (`pin-add-person-160` kapalı `details` içinde ve `aria-invalid`). 5 soğuk başlatmada ikişer deneme (10 gönderim). Odak her seferinde `INPUT#pin-finish-160`; alan görünümde (üst 376 px); alan hatası "PIN hatalı." görünür. Üst bildirim "İşlem yapılmadı PIN hatalı." DOM'da, sayfa alana kaydığı için görünüm üstünde (-1947 px). Konsol 0 | KAPANDI | `Tur2Kaniti.txt` B1 |
| B1, 1440 px | Aynı 5 soğuk başlatmada: odak `pin-finish-160` (üst 596 px), alan hatası görünür, konsol 0 | KAPANDI | `Tur2Kaniti.txt` B1 |
| B1, soğuk başlatma yarışı | 3 soğuk başlatma, CPU x6 ve 400 ms gecikme. Düğme DOM'a gelir gelmez, JS bağlanmadan gönderildi (yerel POST). Yanıt sayfasında `App.js` odağı `pin-finish-160`'a verdi, alan görünümde, bildirim 1, konsol 0. FE'nin bir kez gördüğü "odak BODY, bildirim yok" durumu 9 soğuk başlatmadaki toplam 20 gönderimde (17 normal, 3 yarış) oluşmadı; sebep bulunamadı | Gözlenmedi | `Tur2Kaniti.txt` B1 yarış |
| Gerileme, Katıl | `2001` kartında "Adıma katıl" yanlış PIN: odak karttaki hata kutusunda ("İşlem yapılmadı PIN hatalı."), kutu görünümde (360 px üst 470, 1440 px üst 502), `details` açık, konsol 0 | GEÇTİ | `Tur2Kaniti.txt` gerileme |
| Gerileme, `/users/deactivate/11` | Boş PIN (istemci doğrulaması): odak `Pin` alanında, "Bu alanı doldurun.", `aria-describedby` "Pin-hint Pin-error" (360 px üst 407, 1440 px üst 359). Yanlış PIN (sunucu, 360 px): odak `Pin` alanında, görünümde. Kişi pasifleşmedi. Konsol 0 | GEÇTİ | `Tur2Kaniti.txt` gerileme |
| B3 | "IŞIL ÖZGÜR adımdan ayrıldı ... Çalışma 21 sn", "Ahmet Yılmaz adımdan ayrıldı ... Çalışma 2 sn". Bitiş satırlarında "Süre" ve "Efor" aynı ("Süre 59 sn, Efor 1 dk 34 sn"), "Adım kesildi" satırında da öyle. Kaynak: `_Timeline.cshtml:77` yalnız `ParticipantLeft` türünde "Çalışma" yazıyor | KAPANDI | `Tur2Kaniti.txt` B3 |
| Grep tablosu | Değişen dört dosyada `UiDelivery.md §4` kalıpları 0 (transition-all, outline-none, h-screen, onclick, CDN, jquery, em/en-dash, ham renk, `<img`) | 0 ihlal | `grep -nE` çıktısı boş (tur 2 komutu) |
| Eval ve konsol | Run 36 (kapalı ve tüm `details` açık), detay 34 ve 36, `/users/deactivate/11`, İşlerim; 360 ve 1440 px; açık ve koyu (24 hücre). `UiDelivery.md §3` beklenen değerlerde, konsol 0. Detayda 360 px `TR, TH` görsel olarak gizli başlık satırı (tur 1 ile aynı yanlış pozitif) | GEÇTİ | `Tur2Kaniti.txt` eval |
| `checkVisibility()` desteği olmayan tarayıcı | Kod okuması: `FocusFirst.js:3` yöntemi doğrudan çağırıyor, özellik sınaması ya da geri dönüş yok. Benzetim (`Element.prototype.checkVisibility` silindi): Bitir yanlış PIN sonrası 360 ve 1440 px'te odak BODY (hata `CommandForm.send` `catch`'inde yutuluyor, konsol 0). Giriş ekranında yanlış parola sonrası odak BODY ve `TypeError: element.checkVisibility is not a function` | Yeni bulgu B4 (DÜŞÜK) | `Tur2Kaniti.txt` benzetim |

Not: devam eden adımda bir hata sonrası `aria-invalid` hâlâ 4 PIN alanında duruyor (ekleme paneli, katıl, çıkar ve bitir). Analistin B1 şartında bu bir seçenekti; odak seçicisi düzeltildiği için şart karşılandı. Ekran okuyucu ilgisiz alanları geçersiz okuyabilir; ayrı bulgu yazılmadı.

**Kanıt dosyaları.** `Evidence/Faz-6E/Tur2Kaniti.txt` eklendi. Toplam 10 dosyada kalmak için tur 1'deki `KartHatasiJsKapali360.png` (kapanan B2) çıkarıldı.

**Tur 2 sonucu.** B1 ve B3 kapandı, B2 analist kararıyla kapandı. Yeni bulgu B4 (DÜŞÜK, eski Safari'de odak geri dönüşü yok) açık. KRİTİK, YÜKSEK ve ORTA açık bulgu yok. QA önerisi: GEÇTİ.

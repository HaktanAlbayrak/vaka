DEVİR: frontend-engineer -> koordinatör   Faz: 6E (FE 6, 7)   Dosya: Docs/Handover/frontend-engineer-20261116-2359.md

Yapıldı:
- `_Participants.cshtml`: görevli listesi kişi kartı oldu (`.step-person`). Durum rozeti metin ve ikonla: Girişi bekleniyor, Başlatınca çalışır (adım başlamamış sahip), Katılacak, Çalışıyor (adım bitmişse "Çalıştı"), Ayrıldı. Satırda Katıldı, Ayrıldı, Çalışma (WorkSeconds, `_Duration`). `CanJoin` ve `CanLeave` için kişinin adıyla etiketli PIN formu (`/records/join-step/{id}`, `/records/leave-step/{id}`, alanlar `CommandId`, `ParticipantUserId`, `Pin`); ipucu "Bu PIN <Ad> adına girilir; <Ad> kendisi girsin.", oturum kişinin kendisiyse "Kendi PIN'inizi girin.". Neden varsa (`BlockedReason`) düğme yerine metin. `CanRemove` kişi kartında "Listeden çıkar" (PIN). Ekleme paneli adım sürerken de açılır; ipucuna "Eklenen kişi kendi PIN'iyle katılınca çalışmaya başlar." JS kapalı çalışır (details, düz POST).
- `Run.cshtml`: katıl ve ayrıl yetkisi olan kişi varsa kilitli adımda da liste görünür; `PendingNames` doluyken Bitir pasif düğme (`disabled`, `aria-describedby`) ve uyarı kutusu, metin `CannotFinishReason` (boşsa "Girişi bekleniyor: <Ad>." kurulur).
- `MyWork/Index.cshtml`: `AwaitingMyEntry` kartında "Girişiniz bekleniyor" rozeti, açıklama, birincil "Katılmak için aç" düğmesi, bağlantı `Url#step-{StepId}`. Kart `border-warn`.
- `Details.cshtml`: görevli satırı dl: Eklenme, Katılma, Ayrılma ("Adım bitişine kadar" boşsa), Çalışma; `HasInterval` false ise "Adım boyunca" (WorkSeconds varsa yanında süre); ilk giriş onayı satırı.
- `_Timeline.cshtml`: `ParticipantJoined` (user-check, accent), `ParticipantLeft` (arrow-left, neutral).
- `Styles/app.css`: `.step-person` ailesi (kart, durum kenarlığı 2 px, kırılan ad, dar kartta tam genişlik düğme). `.person-name` kabuktaki kullanıcı adı sınıfı olduğundan ad çakışması `step-person-name` ile çözüldü (kabuk `nowrap` miras veriyordu, uzun ad taşıyordu).
- B7 `FormValidation.js`: `mark` hata açılınca kimliği listede yoksa ekler, hata silinince çıkarır (`describe`); sunucu çizdiği hata kimliği de yinelenmez.

Dört durum: dolu ve boş (görevli yok metni), hata (`ErrorMessage` bildirimi, form hatası alan altında), yükleniyor (mevcut `data-command-form` ve `_RecordSkeleton`). Yeni veri yükleyen ekran yok.

Kabul kriterleri (FE payı): K1/K9 durum metni ve rozet; K2 kapı metni ve pasif Bitir; K3, K4 form ve İşlerim kartı; K6 ayrıl formu; K8 `CanRemove` yalnız katılmamışta; K10 neden metni; K15 detay ve çizelge türleri (çizelge ikonu ölçülmedi); K16 ölçüldü; K17 aşağıda.

Ölçüm (kendi derlememle `.qa/f6e/bin`, `.qa/f6d/qa.db` kopyası `.qa/f6e/qa.db`, port 5821, Playwright 1.61 betikleri `.qa/f6e/*.mjs`; ilk kullanıcılar 2006, 2003, 1001, 0001, parola Faz 6C kaydındaki). Backend okuyucuları (`RunProgressReader`, `MyWorkReader`, `RecordPhaseReader`) henüz yeni alanları doldurmadığı için ölçüm derlemesi `.qa/f6e/src` kopyasındadır ve yalnız orada sahte durum verir (yeniden gruplanan Çalışıyor, Bekleniyor, Ayrıldı, Katılacak, kilitli neden, 6 kişiye tamamlama, uzun ad "Şükrüye İnceoğlu Karamustafapaşalıoğulları"); repoya girmez. Ölçülemeyen: gerçek servis akışı (katıl, ayrıl, kapı reddi), zaman çizelgesindeki iki yeni satırın görünümü, JS kapalı tam akış, G12 hizalama turu.
- (İlk tur, sahte durumlu) Çalıştır sayfası `/records/run/10` (2006), tüm `details` açık, 11 kişi kartı, açık ve koyu: 320, 360, 768, 1280, 1440 px'te sayfa yatay kaydırması yok, kart içi taşma 0, 44 px altı dokunma hedefi 0. 320 px'te kart genişliği 168 px (adım ızgarası), düğmeler kırılır (`whitespace-normal`).
- `/records/details/8` (1001), görevli satırı: 5 genişlikte taşma 0, kaydırma yok. 320 px'te ayrıca `TR`, `TH` 1 hücre viewport dışı (açık ve koyu); görevli bölümü dışında, önceki fazdan, sayfa kaydırması yok.
- `/my-work` (2003, kart sahte işaretli): 5 genişlik, iki tema, taşma 0.
- Bitir kapısı 360 px: uyarı kutusu ve pasif düğme (264x56) görünür.
- K16 (`.qa/f6e/k.mjs`, üç döngü hata aç-kapa): görevli seçicide, `/users/create` ve `/materials/create`'te hata açıkken kimlik 1, kapalıyken 0, her döngüde. Konsol hata 0.

G11 imzası: aşağıdaki "Ön tur düzeltmeleri" bölümünde (ilk metin yanlıştı).

Doğrulama: `npm run build:css` temiz; `dotnet build -o .qa/f6e/bin2` 0 uyarı 0 hata (bin klasörüne dokunulmadı); em/en-dash, transition-all, outline-none, h-screen, onclick, CDN, `...`, ham renk grep 0. Kendi dotnet sürecim kapatıldı (port 5821 boş); 32568'e dokunulmadı.

Backend'den beklenen / sapma:
- `RunProgressReader` (ya da ilgili okuyucu) `StepParticipantViewModel.State`, `JoinedAtLocal`, `LeftAtLocal`, `WorkSeconds`, `CanJoin`, `CanLeave`, `CanRemove`, `BlockedReason` ve `RecordStepViewModel.PendingNames`, `CannotFinishReason`, `CanManageParticipants` (adım sürerken true) değerlerini doldurmalı; ben yalnız ViewModel'e güvenerek çizdim. `MyWorkReader` `AwaitingMyEntry` ve `StepId` doldurmalı (görünüm `Url#step-{StepId}` kullanır). `RecordPhaseReader` `HasInterval`, `JoinedAtLocal`, `LeftAtLocal`, `WorkSeconds` doldurmalı; `ConfirmedAtLocal` ayrı gösterilir ("İlk giriş onayı").
- Sahip kendi satırında adım başlamadan `State = Pending` ise "Başlatınca çalışır" yazılır; sahip başlamadan `Confirmed` olarak dönerse "Katılacak" yazar. Hangisini yapacağın görünümü etkiler, bildir.
- `CannotFinishReason` kapı metnini taşımalı ("Girişi bekleniyor: <Ad>..."); `PendingNames` doluyken `CanFinish` false olmalı.
- Katıl ya da ayrıl hatasında dönüş `/records/run/{id}?stepId=` ve `ErrorMessage`; PIN korunmaz.
- `_PinInput.cshtml` ipucu sabit ("Kendi PIN'inizi girin."), dosya sınırı dışı olduğundan `_Participants` içinde yerel PIN alanı çizildi (aynı kancalar: `data-pin`, `data-pin-input`, `data-pin-reveal`). `_PinInput`'a isteğe bağlı ipucu parametresi eklenirse yerel kopya kaldırılır.

Değişen dosyalar: Views/Records/{_Participants,Run,Details,_Timeline}.cshtml, Views/MyWork/Index.cshtml, Styles/app.css, wwwroot/css/app.css (yeniden üretildi), wwwroot/js/FormValidation.js.
DB talebi: yok

Ön tur düzeltmeleri (7 madde):
1. Kart listesi her genişlikte tek sütun (`xl:grid-cols-2` kalktı). Rozet adın altında kendi satırında; imza her genişlikte aynı.
2. Rozet `whitespace-nowrap`. 768 altında görevli bloğu `max-md:-ml-14` ile adım damgası sütununa taşar (320 px'te kart 168 yerine 224 px). Düğme ve özet `min-h-12 px-2 text-base`, tool-body `p-2`. 320 px'te hiçbir düğme ya da rozet iki satıra kırılmıyor.
3. G11 imzası düzeltildi (aşağıda, ölçülü).
4. Bitir kapısı: uyarı kutusu `.run-action` içine, pasif düğmenin üstüne alındı; sol ve sağ kenar farkı 0 (320-1440).
5. Sahip kartı başlamamış adımda `data-state="Idle"`: warn kenarlığı yok (ölçülen kenarlık çizgi rengi). Warn yalnız sahip dışı, girişi beklenen kişide.
6. `Run.cshtml` koşulu `BlockedReason` doluysa da listeyi gösterir. PIN'i kilitli kişinin İşlerim kartı adımda kartını ve "PIN'i kilitli. Yöneticiye başvurun." nedenini gösteriyor (ölçüldü, `/records/run/10#step-38`).
7. Join, leave ve remove hatasında başarısız kişinin kartında `role="alert"` hata kutusu (`data-form-summary data-focus-first`), ilgili `details` açık, kart `id="step-N-pU"`. Form action'ları `#step-N-pU` parçası taşır, JS kapalıyken de karta kaydırır. Üstteki genel hata kutusu bu durumda `data-step-error` alır, odağı çalmaz. Odak BODY'de kalıyordu: `_PinInput` her PIN alanına ModelState hatasını `aria-invalid` yapıyor ve ilk eşleşen, kapalı bir `details` içindeydi. `App.js` ve `CommandForm.js` odak seçicisine en başa `[data-focus-first]` eklendi (iki satır, dosya sınırı dışı; geri alınırsa odak yine BODY'de kalır). Ölçüm, yanlış PIN: JS açıkken odak karttaki hata kutusunda ve kutu görünüm içinde (360 px'te üst 470/800, 1280 px'te 502/800). JS kapalıyken sayfa karta kaydırılır (kutu üst 307 ve 287 px); odak yok (JS yok).

Ölçüm düzeni: gerçek backend, derleme `.qa/f6e1/bin`, `.qa/f6d/qa.db` kopyası `.qa/f6e1/qa.db` (migration `AddStepWorkIntervals` kopyaya SQL betiğiyle uygulandı), port 5831. Kayıt 10: sahip 2006, 2003 çalışıyor, 2004 PIN'i kilitli, 2901 girişi bekliyor. Betikler `.qa/f6e1/*.mjs`. Açık ve koyu tema aynı sonuç.

| Genişlik | Kart | Sayfa kaydırma | Rozet kırılması | Düğme kırılması | Kart taşması | Kapı sol/sağ fark |
|---|---|---|---|---|---|---|
| 320 | 224 | yok | 0 | 0 | 0 | 0 / 0 |
| 360 | 264 | yok | 0 | 0 | 0 | 0 / 0 |
| 768 | 580 | yok | 0 | 0 | 0 | 0 / 0 |
| 1024 | 248 | yok | 0 | 0 | 0 | 0 / 0 |
| 1280 | 424 | yok | 0 | 0 | 0 | 0 / 0 |
| 1440 | 584 | yok | 0 | 0 | 0 | 0 / 0 |

İşlerim (2004) ve çalıştır sayfası, tüm details açık, 6 genişlik x 2 tema (24 satır): taşma 0, sayfa kaydırması 0, 44 px altı hedef 0.

G11 imzası, `step-person` kartı, 320-1440 px'in hepsinde aynı, liste tek sütun (ölçüldü):
- Çalışıyor kartı: Ad:1@1, Rozet:1@2, Durum:1@3, Eylem:1@4.
- Girişi bekleyen, nedeni olan kart (PIN kilitli): Ad:1@1, Rozet:1@2, Durum:1@3, Neden:1@4, Eylem (Çıkar):1@5.
- Nedensiz bekleyen kart: Ad:1@1, Rozet:1@2, Durum:1@3, Katıl:1@4, Çıkar:1@5 (beklenen; bu veride Katıl yalnız Çalışıyor kartının eylemi ve nedenli kartın Çıkar'ı ölçüldü).
- Başarısız işlem sonrası: Hata kutusu Durum'dan sonra gelir, eylemler bir satır aşağı kayar.
Eylemler alt alta, yan yana değil. Detay görevli satırı kv: 320-1024 Eklenme:1@1, Katılma:2@1, Ayrılma:1@2, Çalışma:2@2; 1280 ve üstü 1@1 ... 4@1 (768 ve 1024 ölçülmedi).

Doğrulama: `npm run build:css` temiz; `dotnet build -o .qa/f6e1/bin2` 0 uyarı 0 hata; dash, transition-all, outline-none, h-screen, onclick grep 0. Sunucu süreci kapatıldı (5831 boş), 32568'e dokunulmadı.

Tur 2 (B1, B3):
- B1: odak seçicisi yeni `wwwroot/js/FocusFirst.js` (`focusFirstError(root, action)`). Önce gönderilen formun (aynı `action`) görünür `aria-invalid` alanına, yoksa sırayla `[data-focus-first]`, `aria-invalid`, `[data-form-summary]` adaylarından görünür olan ilkine odaklanır; kapalı `details` içindeki adaylar `checkVisibility()` ile atlanır. `App.js` (ilk yükleme) ve `CommandForm.js` (`replace`) bu işlevi kullanır; önceki `[data-focus-first]` satırları kalktı. `_PinInput` ve ModelState tarafına dokunulmadı.
- B3: `_Timeline.cshtml` "adımdan ayrıldı" satırında süre etiketi "Çalışma"; bitiş satırında "Süre" ve "Efor" aynı.
- Ölçüm (gerçek backend, `.qa/f6e1/qa.db` kopyası `.qa/f6e2/qa.db`, migration uygulanmış, port 5841, sahip 2006, görevli 2003; IŞIL ve Şükrüye kullanılmadı; betikler `.qa/f6e2/*.mjs`):
  - Bitir yanlış PIN, ekleme paneli açık, adım sürerken: 360 px'te odak `pin-finish-38`, alan görünümde (üst 374/800), bildirim görünür; 1440 px'te odak `pin-finish-38` (üst 406), bildirim görünür. Aynı akış iki tekrarda aynı sonuç. İlk soğuk başlatmada 360 px'te bir kez odak BODY'de kaldı ve bildirim yoktu; yeniden denemelerde (3 kez) oluşmadı, sebep bulunamadı (sunucu ilk istek, JS modülü yüklenmeden form gönderilmiş olabilir).
  - Gerileme: "Adıma katıl" yanlış PIN: odak kartın hata kutusunda (`data-form-summary`), 360 ve 1440 px'te görünümde. `/users/deactivate/11` boş PIN (Yönetici 1001): odak `Pin` alanında, 360 (üst 407) ve 1440 (üst 359) px.
  - Konsol hatası 0 (360 ve 1440, iki oturum).
  - B3: 2003 katılıp ayrılınca detay çizelgesinde "ayrıldı" satırı "Çalışma 2 sn" gösteriyor.
- Not: ölçümde 2006'nın PIN'i yanlış denemelerle kilitlendi, kopyada sıfırlandı (kopya veri, repoda yok).
- Doğrulama: `npm run build:css` temiz; `dotnet build -o .qa/f6e2/bin2` 0 uyarı 0 hata; dash, transition-all, outline-none, h-screen, onclick grep 0. Sunucu kapatıldı, 32568'e dokunulmadı.
Değişen dosyalar (tur 2): wwwroot/js/FocusFirst.js (yeni), wwwroot/js/App.js, wwwroot/js/CommandForm.js, Views/Records/_Timeline.cshtml.

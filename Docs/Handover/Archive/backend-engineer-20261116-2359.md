# Backend devir notu: Faz 6E (görevli girişi ve adım içi katılım)

## Route ve rol
| Route | Rol | Açıklama |
|---|---|---|
| POST `/records/join-step/{stepId}` | Operator (servis: sahip ya da kişinin kendisi, değilse 403) | `JoinStepViewModel { CommandId, ParticipantUserId, Pin }`; PIN kişinin PIN'i. Başarıda `/records/run/{id}?stepId={stepId}`, ret 200 `Run` görünümü ve `ErrorMessage` |
| POST `/records/leave-step/{stepId}` | Operator (aynı) | `LeaveStepViewModel` aynı alanlar, aynı dönüş |
| POST `/records/add-participant/{stepId}`, `/records/remove-participant/{stepId}` | Operator (yalnız sahip) | Alanlar aynı; ekleme adım Bekliyor ya da Devam ediyorken, çıkarma yalnız hiç katılmamış kişide |

Oturumsuz 302 giriş; Yönetici ve Denetçi 302 access-denied (403 sayfası); sahip ya da kişinin kendisi olmayan Operatör 403.

## Karar: sahip adım başlamadan hangi State ile döner
`Pending`. Sahip girişsiz çalışır (plan: "adım başladığında girişsiz çalışmaya başlar"); `Confirmed` yalnız sahip dışı ve girişini yapmış kişi içindir. Sahip satırında `CanJoin=false`, `BlockedReason=null`; ekranda "Başlatınca çalışır" yazılır. Sahip dışı `Pending` adım Bekliyor iken "Girişi bekleniyor". `RecordStepViewModel.PendingNames` ve bitirme kapısı sahibi saymaz.

## ViewModel alanları (dolu gelenler)
- `StepParticipantViewModel`: `State`, `JoinedAtLocal` (aralık başlangıcı), `LeftAtLocal` (yalnız `Left`), `WorkSeconds` (aralık kapanmışsa), `CanJoin`, `CanLeave`, `CanRemove`, `BlockedReason`.
  - `State`: aralığı varsa `Left` (ayrıldı) ya da `Working` (adım bitmiş olsa da; o zaman `LeftAtLocal` boş, `WorkSeconds` dolu); aralığı yoksa Faz 6E öncesinden çalışan ve bitmiş eski adımda `Working` (`JoinedAtLocal` boş); başlamamış adımda girişini yapmış sahip dışı `Confirmed`; kalanı `Pending`.
  - `CanJoin` ve `BlockedReason` yalnız sahip ya da kişinin kendisine (ve `Pending`, sahip dışı, adım düzenlenebilir) hesaplanır; neden varsa `CanJoin=false` ve `BlockedReason` dolu (pasif, PIN kilitli, molada, başka adımda çalışıyor).
  - `CanLeave`: `Working`, adım Devam ediyor, sahip ya da kişinin kendisi, çalışan birden çok. Son kişide `CanLeave=false`, `BlockedReason="Adımda çalışan son kişi ayrılamaz. Adımı bitirin."`.
  - `CanRemove`: yalnız sahip, `Pending`/`Confirmed`, listede birden çok kişi.
- `RecordStepViewModel`: `PendingNames` (yalnız Devam eden adımda, sahip dışı `Pending`), `CannotFinishReason` kapı metnini taşır (`Girişi bekleniyor: Ad, Ad. Kişi kendi PIN'iyle katılsın ya da sahip listeden çıkarsın.`), `CanFinish=false` olur, `CanManageParticipants` sahip için adım Bekliyor ve Devam ederken true. Ayrılmış kişi artık adımı bitiremez (`isMember` dışı).
- `AssignedStepViewModel`: `StepId`, `AwaitingMyEntry` (Bekliyor adımda `ConfirmedAtUtc` boş, ya da Devam eden adımda aralıksız ve çalışmayan); girişi bekleyenler listede önce gelir.
- `RecordDetailParticipantViewModel`: `HasInterval` (false yalnız "Adım boyunca": eski adım ya da Faz 6E öncesinden çalışan), `JoinedAtLocal`, `LeftAtLocal`, `WorkSeconds`. Hiç katılmamış kişi `HasInterval=true`, `JoinedAtLocal` boş, bitmiş adımda `WorkSeconds=0`. `TimelineEntryKind.ParticipantJoined`, `ParticipantLeft`.

## Kurallar özeti
- Başlatma: çalışmaya başlayanlar = başlatan + listedeki sahip + girişini yapmış uygun kişiler (aktif, molada değil, başka adımda çalışmıyor). Başlatan ve sahip uygunluğu eskisi gibi denetlenir; uygunsuz girişli kişi aralıksız kalır (`ConfirmedAtUtc` değişmez). Çalışan olmazsa ret. `StepStarted.ParticipantUserIds` çalışanlardır. Sahip dışı başlatan girişsizse `ConfirmedAtUtc` yazılır.
- Katılma: adım Bekliyor ise yalnız `ConfirmedAtUtc`; Devam ediyorsa aralık, `IsWorking`, `ConfirmedAtUtc ??= an`. Olay `ParticipantJoined` (yapan kişi, `SessionUserId` oturum). Ayrılan yeniden katılamaz.
- Ayrılma: aralık `Left` ile kapanır (`EndedByUserId` ayrılan kişi), `WorkSeconds` SH-02a, `ParticipantLeft` olayı; çalışan son kişi ayrılamaz.
- Bitirme kapısı sahip dışı girişsiz kişide; bitişte açık aralıklar `StepFinished`, müdahalede `Interrupted` ile kapanır. Faz 6E öncesinden çalışan, aralığı olmayan görevli için kapanışta adım başlangıcından başlayan kapalı aralık eklenir (adımın tamamında çalışmış). Efor = kişi sürelerinin toplamı. Olay yükü: `ParticipantUserIds` çalışanlar, `ParticipantWorkSeconds` aynı sıra saniyeler.
- Sahip adım sürerken kendini eklerse aynı istekte katılır (`ParticipantAdded` + `ParticipantJoined`). Çıkarma: aralığı olan ya da çalışan kişi çıkarılamaz.
- Bütünlük: adım "görevliler" karşılaştırması artık çalışanlar (aralığı olan, çalışan, eski biten adımda hepsi) ile `StepStarted` görevlileri artı adım sürerken `ParticipantJoined` kişileridir.

## Sapmalar
- Dosya sınırı dışı ama `Services/**` içinde: `StepWorkCloser` ve `IStepWorkCloser` (yeni), `ParticipantStateRules` (yeni), `RecordStateAudit`, `RecordStateRules` (bütünlük karşılaştırması), `StepQueries` (`WorkInterval` include), `Program.cs` kayıtları.
- `IStepStartApplier.CheckParticipantsAsync` imzası `(step, starterUserId, ownerUserId)` oldu (acil açılış da güncellendi).
- `IDurationCalculator.CalculateStepEffort(int, int)` kaldırıldı; yerine `CalculateStepEffort(IReadOnlyList<int>)` ve `CalculateParticipantWork`.
- B5: `RecordDetailReader`'dan olay okuma `IRecordEventReader` / `RecordEventReader`'a taşındı (davranış aynı, `SessionUserId` ad sözlüğüne eklendi).
- Ayrılmış kişi adımı bitiremez (planda yok, tutarlılık için).

## DB talebi
DB-2026-009 (`StepWorkIntervals`) dbadmin'ce uygulandı; yeni talep yok. Gerçek `App_Data/vaka.db`'ye migration henüz uygulanmadı (koordinatör zamanlar).

## Doğrulama (kopya DB: `.qa/be/f6e/qa.db`, boş port 5611)
Oturumsuz 302 giriş, Yönetici ve Denetçi access-denied, başkası adına katılma 403, aynı CommandId tek olay, PIN yanlışında sayaç kişinin hesabına, tüm kayıtlar "Doğrulandı", eski devam eden adım müdahalede aralığa dönüştü ve efor süreye eşit, kapı/çıkarma/yeniden katılma/son kişi ayrılamaz senaryoları, MyWork `AwaitingMyEntry`, tetikleyiciler (güncelleme ve silme reddi).

DEVİR: backend-engineer -> koordinatör (frontend-engineer, dbadmin)   Faz: 5 aşama 1   Dosya: Docs/Handover/backend-engineer-20261115-1400.md

Önceki not: Archive/backend-engineer-20261115-1000.md. Sözleşmeden sapma yok.

ViewModel'ler hazır. Derleme: `dotnet build -warnaserror -o .qa/be/s5build` 0 uyarı 0 hata. Action'lar ve servisler aşama 2'de (dbadmin sonrası).

Yeni ve değişen tipler (koleksiyonların hepsi T[]):
- `Services/BreakEndKind { Person = 1, Manager = 2, Intervention = 3 }`.
- `ViewModels/Records` (Vaka.Web.ViewModels.Records):
  - `RecordRunViewModel` ek: `Breaks[]`, `OpenBreak?`, `TimersRunning`, `CanStartBreak`, `CannotBreakReason`, `Summary`, `FirstStepStartedByName`, `ParticipantStepId?`, `ParticipantQuery?`, `ParticipantOptions[]` (en çok 50), `ParticipantOptionsTruncated`, `PendingParticipant?`.
  - `RecordPhaseViewModel` ek: `GrossSeconds?`, `BreakSeconds?`, `NetSeconds?`, `LiveNetSeconds?`, `BelowMinimum`, `BelowMinimumReason`.
  - `RecordStepViewModel` ek: `Description`, `Media[]` (mevcut `Procedures.StepMediaViewModel`: Id, MediaType, MediaTypeLabel, OriginalFileName, Url; `_MediaList` yeniden kullanılabilir), `IsLastOfPhase`, `StartedAtLocal?`, `StartedByName?`, `EndedAtLocal?`, `EndedByName?`, `BreakSeconds?`, `DurationSeconds?`, `EffortSeconds?`, `LiveElapsedSeconds?`, `Participants[]`, `CanStart`, `CanFinish`, `CannotStartReason`, `CannotFinishReason`, `CanManageParticipants`. `IsNext` ve `Status` önceki gibi.
  - Yeni: `StepParticipantViewModel { UserId, FullName, RegistryNo, IsOwner, AddedAtLocal }`, `RecordBreakViewModel { StartedByName, StartedAtLocal, EndedAtLocal?, DurationSeconds?, EndKindLabel, DuringStepTitle? }`, `OpenBreakViewModel { Id, StartedByName, StartedAtLocal, DuringStepTitle? }`, `RecordSummaryViewModel { GrossSeconds?, BreakSeconds?, NetSeconds?, EffortSeconds?, StartedAtLocal?, CompletedAtLocal? }`, `ParticipantOptionViewModel { UserId, FullName, RegistryNo, IsAvailable, UnavailableReason?, HasOwnActiveRecord }`, `PendingParticipantViewModel { StepId, ParticipantUserId }`, `AssignedStepViewModel { RecordId, RecordNo, MachineLabel, StepTitle, OwnerName, Url }`, `MyWorkBreakViewModel { BreakId, RecordNo, StartedAtLocal }`.
  - `MyWorkViewModel` ek: `AssignedSteps[]`, `OpenBreak?`.
  - Komut modelleri: `StartStepViewModel { CommandId, Pin }`, `FinishStepViewModel { StepId, RecordId, PhaseTitle, PhaseNetSeconds, MinDurationSeconds, CommandId, BelowMinimumReason?, Pin, ErrorMessage? }` (hem POST gövdesi hem gerekçe formunun modeli; `RecordId` yürütme ekranına dönüş bağlantısı için sözleşmeye eklenen tek alan), `AddParticipantViewModel { CommandId, ParticipantUserId, Pin }`, `RemoveParticipantViewModel { CommandId, ParticipantUserId, Pin }`.
- `ViewModels/Breaks` (Vaka.Web.ViewModels.Breaks): `CurrentBreakViewModel { BreakId, RecordId, RecordNo, MachineLabel, StartedAtLocal, ElapsedSeconds, DuringStepTitle?, CommandId, ErrorMessage? }` (`RecordId` ve `ErrorMessage` sözleşmeye ek), `StartBreakViewModel { CommandId, Pin }`, `EndBreakViewModel { CommandId, Pin }`.

Route'lar sözleşmedeki gibi; Run GET'e `stepId` ve `people` sorgu parametreleri aşama 2'de bağlanır (ParticipantStepId, ParticipantQuery). Mevcut `RecordsController` `[Route("[controller]/[action]")]` düzeninde: `start-step`, `finish-step`, `add-participant`, `remove-participant`, `people` aynı kalıpla gelecek; `/breaks/start|current|end`. Boş action imzası eklemedim (görünümler ViewModel'lere bağlanır, route'lar yok iken POST hedefi 404 olur).

F4-A2 yapıldı: `ILotCatalog.FindAsync(query, includeLotId, excludeRecordId, ct)`; kayda eklenmiş (çıkarılmamış) lot ne listede ne seçili-lot eklemesinde yer alır. `GET /records/lots?q=&recordId=` (recordId isteğe bağlı) ve yürütme ekranı `AvailableLots` buna bağlandı. Aynı lotu yine de POST ile göndermek mevcut `AddMaterial` ile nedenli reddedilir (Faz 4, kısmi unique index). Create ekranı (recordId yok) değişmedi.

Frontend'in bilmesi gerekenler:
- Ret 200 görünüm, başarı 302 `/records/run/{id}`; PIN ve gövde kuralı Faz 4'teki gibi. Reddedilen görevli eklemede `PendingParticipant` dolu, PIN boş. Süre altı gerekçe: gerekçesiz bitirme faz minimumunun altındaysa 200 `FinishStep.cshtml` (model `FinishStepViewModel`), aynı `CommandId` korunur.
- `TimersRunning` false iken `LiveNetSeconds` ve `LiveElapsedSeconds` sabit değerdir; JS ilerletmez, hesap yapmaz.

DB talebi: DB-2026-007 (TALEP, Eklemeli, ALTER yok, DROP yok). Mola tablosu `RecordBreaks` (kalem 1) ve KSA demo tipi (kalem 1b: tip 4, versiyon 4, faz 8-9 her biri 60 sn, adım 20-23, PLN/PLN3 ile aynı 4 demo makinesine eşli). `DbChanges.md` satırı eklendi. Koordinatör dbadmin'i çağırabilir; aşama 2 onun "Backend'in bilmesi gerekenler" notundan sonra.

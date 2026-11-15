DEVİR: backend-engineer -> koordinatör (dbadmin, sonra backend-engineer)   Faz: 3 aşama 1   Dosya: Docs/Handover/backend-engineer-20261115-0400.md

ViewModel'ler hazır (BE 6). Frontend derleyebilir: `dotnet build -warnaserror` 0 uyarı 0 hata.

Yapıldı:
- Enum'lar `Services/`: `CleaningTypeKind`, `ProcedureVersionStatus`, `StepMediaType`. `SettingKeys`'e `MaxLockMinutes`, `MaxPhotoMb`, `MaxVideoMb`.
- `ViewModels/Procedures/` (ad alanı `Vaka.Web.ViewModels.Procedures`), sözleşmedeki adlarla: `MoveDirection`, `CleaningTypeListViewModel`, `CleaningTypeListItemViewModel`, `CleaningTypeFormViewModel`, `MachineMappingViewModel`, `MachineMappingGroupViewModel`, `MachineMappingItemViewModel`, `CleaningTypeDetailViewModel`, `ProcedureVersionItemViewModel`, `CreateDraftViewModel`, `ProcedureDraftViewModel`, `DraftPhaseViewModel`, `DraftStepViewModel`, `StepMediaViewModel`, `DraftFormErrorViewModel`, `DraftHeaderViewModel`, `PhaseFormViewModel`, `StepFormViewModel`, `MoveViewModel`, `MediaUploadViewModel`, `PublishViewModel`, `ProcedureVersionViewModel`, `VersionPhaseViewModel`, `VersionStepViewModel`. `SelectOptionViewModel` ve `BlockingRecordViewModel` Faz 2'den (`ViewModels/Definitions`).
- DB talebi açıldı: Docs/Db/DB-2026-005.md (Eklemeli, ALTER yok, kullanıcı onayı gerekmez), `DbChanges.md` TALEP satırı eklendi.

Sözleşmeden tek ek (ekleme, daralma yok): `DraftFormErrorViewModel.MaterialRequired` (bool?). Üst bilgi formunda hata olunca onay kutusu gönderildiği haliyle dolsun diye; frontend yoksayabilir, boşsa `ProcedureDraftViewModel.MaterialRequired` kullanılır.

Frontend'in bilmesi gerekenler:
- Tüm ViewModel'ler düz özellik sınıfı; `Kind` tip listesi süzgecinde ve tip formunda `CleaningTypeKind?`. `MachineMappingViewModel.SelectedMachineIds` POST alanı (tekrarlı). `MediaUploadViewModel.File` multipart alan adı `File`.
- Servis ve controller'lar henüz yok; route'lar DB tabloları gelince aşama 2'de. Sözleşmedeki adresler değişmeyecek.

DB talebi: Docs/Db/DB-2026-005.md. dbadmin uygulayınca (entity'ler, migration, tetikleyiciler, ayar seed, DemoData.sql) servis ve controller'lar (aşama 2) yazılır; entity olmadan derlenemedikleri için aşama 2 dbadmin'den sonra.
Açık: önceki notun maddeleri değişmedi.

DEVİR: backend-engineer -> koordinatör (frontend-engineer, dbadmin)   Faz: 4 aşama 1   Dosya: Docs/Handover/backend-engineer-20261115-0700.md

ViewModel'ler hazır. `dotnet build -warnaserror` 0 uyarı 0 hata. Servis, controller ve olay yazıcı aşama 2'de (dbadmin sonrası).

Yapıldı:
- Ad alanı `Vaka.Web.ViewModels.Records`, klasör `ViewModels/Records/`: `MyWorkViewModel`, `ActiveRecordViewModel`, `RecordCreateViewModel`, `MachineOptionViewModel`, `CleaningTypeOptionViewModel`, `MaterialEntryViewModel`, `LotOptionViewModel`, `RecordRunViewModel`, `RecordMaterialViewModel`, `RecordPhaseViewModel`, `RecordStepViewModel`, `AddMaterialViewModel`, `RemoveMaterialViewModel`, `MachineBoardViewModel`, `MachineBoardGroupViewModel`, `MachineBoardItemViewModel`. `Facilities[]` ve `Lines[]` Faz 2 `SelectOptionViewModel` (`Vaka.Web.ViewModels.Definitions`).
- Enum'lar `Services/`: `RecordStatus`, `PhaseStatus`, `StepStatus`, `MachineState` (değerler sözleşmedeki gibi). `Services/RecordLabels.cs`: enum -> Türkçe metin (kayıt: Açıldı, Devam ediyor, Beklemede, Tamamlandı, Zaman aşımı, İptal edildi, Yarıda kapatıldı; faz: Bekliyor, Devam ediyor, Tamamlandı, Tamamlandı - süre altında, Kesildi; adım: Bekliyor, Devam ediyor, Tamamlandı, Kesildi; makine: Boş, Açıldı, Devam ediyor, Beklemede). ViewModel `Status`/`State` ve `...Label` birlikte dolar.
- `IDurationFormatter.Format(int seconds)` -> `DurationPart[] { Value, Unit }` (`sa`, `dk`, `sn`), uygulaması `DurationFormatter`, Program.cs'de singleton (görünümde `@inject IDurationFormatter`). Sıfır birim atlanır, toplam 0 ise tek parça `0 sn`, negatif ArgumentOutOfRange. `Vaka.Web.csproj`'a `UserSecretsId` eklendi.

Sözleşmeden sapma yok; yalnız eklemeler (frontend yok sayabilir):
- `MyWorkViewModel` yalnız Faz 4 alanlarını taşır (AssignedSteps, HeldRecords, OpenBreak Faz 5 ve 7'de eklenir).
- `ActiveRecordViewModel` ve `RecordRunViewModel` ek: `KindLabel`, `ActiveRecordViewModel.Status`.
- `MaterialEntryViewModel` ek alanlar (`MaterialCode`, `MaterialName`, `LotNo`, `ExpiryDate`, `IsExpired`): formdan okunmaz, reddedilen istek yeniden çizilirken sunucu seçili lotların görünmesi için doldurur.
- JSON uçları ViewModel ile döner, sistem JSON'u camelCase: `Lines` -> `SelectOptionViewModel` (`id`, `label`), `Machines` -> `MachineOptionViewModel` (`id`, `label`, `isAvailable`, `busyReason`), `CleaningTypes` -> `CleaningTypeOptionViewModel` (`id`, `name`, `versionNo`, `materialRequired`, `defaultLockMinutes`), `Lots` -> `LotOptionViewModel` (`id`, `materialCode`, `materialName`, `lotNo`, `expiryDate` "yyyy-MM-dd", `isExpired`).

Frontend için:
- `CommandId` alanları `Guid`; GET'te sunucu doldurur, form gizli alanla geri gönderir. Reddedilen istek 200 ile aynı görünümü çizer ve `CommandId` aynı kalır (reddedilen komut kayıt bırakmaz, aynı kimlik yeniden kullanılabilir). Başarıda her komut `/records/run/{id}`'ye 302 verir; aynı `CommandId` ile tekrar istek aynı yönlendirmeyi döner.
- Form alan adları: create `CommandId, FacilityId, LineId, MachineId, CleaningTypeId, WorkOrderNo, Note, LockMinutes, Materials[i].MaterialLotId, Materials[i].ExpiredReason, Pin` (JS kapalı yenileme `refresh=true`); `add-material`: `CommandId, MaterialLotId, ExpiredReason, Pin`; `remove-material`: `CommandId, RecordMaterialId, Pin`. Her form kendi antiforgery alanını taşır (G5a), bool alanı yok.
- Controller'lar aşama 2'de gelir; o zamana kadar bu route'lar 404 verir, görünümler yalnız model tipine bağlanarak derlenir.

DB talebi: DB-2026-006 (Faz 4, Eklemeli): sekiz tablo (`CleaningRecords`, `RecordPhases`, `RecordSteps`, `StepParticipants`, `RecordMaterials`, `CleaningEvents`, `ProcessedCommands`, `NumberSequences`), önden gelen Faz 5-7 kolonları, kısmi unique index'ler (makine BR-23, sahip BR-37, çalışan kişi BR-24, aktif lot), olay günlüğü UPDATE/DELETE ve adım zamanı tetikleyicileri, sayaç tetikleyicisi, WAL, üçüncü demo tipi `PLN3` (3 faz, 5-3-4 adım, 900 sn, Yürürlükte v1, makine 1, 2, 3, 5). ALTER yok. Dikkat: demo betiğindeki tek `UPDATE` (Taslak -> Yürürlükte, yalnız betiğin kendi satırı) Faz 3 düzenidir; dbadmin onay gerektiren işlem sayarsa koordinatör onayı alır. İndeks satırı `Docs/Db/DbChanges.md`'de (TALEP).

Aşama 2 planı (dbadmin sonrası): servisler (kayıt açma, olay yazıcı, numara, kilit ve zaman aşımı, malzeme, pano, engel sağlayıcıları, vardiya bulucu), controller'lar, `EventChain:HmacKey` doğrulaması (dev anahtarı user-secrets'a, repoya yazılmaz), F1-A2 için PIN doğrulaması ve iş eylemini tek işlemde çalıştıran ortak yürütücü (Faz 1 hesap işlemleri dahil), Program.cs, bağlantı dizesinde `Default Timeout`. Açık sorular yok.

Güncelleme: PL-64 alanları hazır (build 0 uyarı 0 hata).
- `RecordCreateViewModel` ve `RecordRunViewModel`: `LotQuery?`, `AvailableLots[]` (`LotOptionViewModel`), `AvailableLotsTruncated`.
- `RecordCreateViewModel`: `AddMaterialLotId?`, `AddExpiredReason?`, `RemoveMaterialIndex?` (refresh=true dalı).
- `RecordRunViewModel.PendingMaterial?` yeni tip `PendingMaterialViewModel { MaterialLotId, ExpiredReason }`.
Aşama 2 notları: refresh=true dalında lot ekle/çıkar (kayıt açılmaz, olay yazılmaz, CommandId ve alanlar korunur, LotQuery süzer); `GET /records/run/{id}?q=`; ret her zaman 200 görünüm, yönlendirme yok; yetki reddi 403; `/records/lots?q=` boş q ile de en çok 50 sonuç; refresh ve ret yanıtlarında CleaningTypes[] seçili makineye göre dolu; ModelState anahtarları FacilityId, LineId, MachineId, CleaningTypeId, WorkOrderNo, LockMinutes, Pin, Materials[i].ExpiredReason; genel hata ErrorMessage.

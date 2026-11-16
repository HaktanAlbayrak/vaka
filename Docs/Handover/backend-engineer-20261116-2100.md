DEVİR: backend-engineer -> koordinatör   Faz: 6C (BE 1, 2, 5)   Dosya: Docs/Handover/backend-engineer-20261116-2100.md

Yapıldı:
- BE 1: `IRecordTimelineBuilder` / `RecordTimelineBuilder` (Program.cs'te singleton). `ParsedEvent` listesinden olay başına bir satır, `Sequence` sırasıyla, Türkiye gününe göre gruplu `RecordTimelineDayViewModel[]`. Satır metinleri plandaki tabloyla aynı. Süre ve efor yalnız olay yükündeki saklı saniyelerdir (adım bitişi: `DurationSeconds`, `EffortSeconds`; faz: `NetSeconds`; kayıt: `NetSeconds`, `EffortSeconds`; mola bitişi: `DurationSeconds`); yeni süre hesabı yok, formül açıklaması builder'ın XML yorumunda. Yapanı olmayan olayda kişi "Sistem". Bilinmeyen tür `RecordEventText.Label` ile `Other` satırı.
- BE 2: `RecordDetailViewModel.Timeline` `RecordDetailReader` içinde dolar; ad, adım etiketi (`Faz n / Adım m - başlık`) ve faz sırası sözlükleri okuyucudan gelir.
- BE 5: `GET /records/people?q=&stepId=` -> `{ items, truncated }`. `stepId` yoksa 400, adım yoksa 404, sahip değilse 403 (`IParticipantDirectory.SearchForOwnerAsync`; 403 projedeki Forbid yoluyla access-denied yönlendirmesi olarak görünür). Yalnız aktif Operatörler, adımda zaten görevli olanlar çıkar, molada olan "Molada" nedeniyle `IsAvailable=false`. Arama ad ve sicil no'da, `tr-TR` `CompareInfo.IndexOf(IgnoreCase)` ile bellekte (SQLite Türkçe harfte duyarlı). Boş arama ada göre ilk 50. `Run` JS kapalı yolu aynı `SearchAsync`'i kullanır. `ParticipantService.CheckCandidateAsync` değişmedi, zaten aynı kuralları sunucuda uyguluyor.

Düzeltme (ön tur 500'ü): `RecordDetailReader` faz sırası sözlüğü `ToDictionaryAsync(phase => ..., phase => phase.ProcedurePhase.OrderNo)` ile kuruluyordu; ifade istemci tarafında çalıştığı için `ProcedurePhase` null idi. Şimdi önce `Select(phase => new { phase.Id, phase.ProcedurePhase.OrderNo })` + `ToListAsync`, sonra `ToDictionary`. Okuyucudaki diğer sözlükler kontrol edildi: kullanıcılar (`Id`, `FullName` düz alan) ve adımlar (zaten `Select` sonrası bellek) sorunsuz.

Değişen dosyalar (hepsi `Src/Vaka.Web/`):
- Yeni: `Services/TimelineEntryKind.cs`, `Services/IRecordTimelineBuilder.cs`, `Services/RecordTimelineBuilder.cs`, `ViewModels/Records/RecordTimelineEntryViewModel.cs`, `ViewModels/Records/RecordTimelineDayViewModel.cs`, `ViewModels/Records/ParticipantSearchResponseViewModel.cs`
- Değişen: `ViewModels/Records/RecordDetailViewModel.cs`, `Services/RecordDetailReader.cs`, `Services/IParticipantDirectory.cs`, `Services/ParticipantDirectory.cs`, `Controllers/RecordStepsController.cs` (`People`), `Program.cs`

Öz denetim:
- `dotnet build -warnaserror` (kaynağın scratchpad kopyasında; ana dizinde çalışan başka bir süreç exe'yi kilitliyor): 0 uyarı, 0 hata. Test projesi yok.
- Çalıştırma: `.qa/f6c0/qa.db` kopyası (scratchpad), boş port 5873, yalnız kendi PID'im kapatıldı. Oturumlu (1001) `/records/details/1..7` 200 ve çizelge bölümü var, 8 yok (404, kayıt yok); log'da exception 0.
- `/records/people`: 2002 `stepId`siz 400, olmayan adım 404; 2001 (kayıt 1'in sahibi) 200: boş arama 50 satır ve `truncated: true`, "IŞIL" 1, "şükr" 1 (büyük harfli, Türkçe), sicil "2903" 1, pasif operatör 0; 1001 (Operatör değil) access-denied yönlendirmesi.
- Builder ve Türkçe eşleşme ayrıca scratchpad konsol denemesiyle sınandı (gün geçişi, çok görevli başlangıç, süre altı faz, zaman aşımı, bilinmeyen tür).

Frontend'in bilmesi gerekenler:
- Alanlar sözleşmeyle aynı, sapma yok. `DurationSeconds` ve `EffortSeconds` `int?` ham saniyedir, `_Duration` ile biçimlenir. `Kind` `Vaka.Web.Services.TimelineEntryKind`.
- `people` yanıtı JSON camelCase: `items[]` (`userId`, `fullName`, `registryNo`, `isAvailable`, `unavailableReason`, `hasOwnActiveRecord`), `truncated`. Hata durumunda 400/404/403.

DB talebi: yok

Açık/ertelenen:
- Oturumlu 403 ve K15'in "i/İ" çiftleri gerçek istekle yalnız kısmen ölçüldü; QA tam ölçsün.

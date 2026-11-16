# Backend devir notu: Faz 6D (makine raporu, kullanım oranı)

## Route ve rol
| Route | Rol | ViewModel |
|---|---|---|
| GET `/reports/machine` | ManagerOrAuditor | `MachineReportViewModel` (sözleşmedeki alanlarla aynı) |
| GET `/reports/materials?mode=Usage` | ManagerOrAuditor | `MaterialTraceViewModel`: `Mode=Usage (3)`, `FacilityId`, `LineId`, `CleaningTypeId`, `Facilities`, `Lines`, `CleaningTypes`, `Usage: MaterialUsageViewModel` |
| GET `/reports/durations` | ManagerOrAuditor | `MachineDurationRowViewModel.MachineReportUrl` (makine, tarih, tür) |

Oturumsuz 302 giriş, Operatör 302 access-denied (mevcut 403 sayfası düzeni).

## Frontend'in bilmesi gerekenler
- Makine seçilmemişse `Machine == null`, `ErrorMessage` boş, tablolar boş; geçersizse `ErrorMessage` dolu.
- Uyuşmayan alt seçim hata değildir: tesis/hat değişince hat ve/veya makine sunucuda null olur (`IReportOptions.ReconcileSelectionAsync`), seçenekler yeni üst seçime göre dolar. Olmayan kimlik hâlâ hata.
- Makine seçili ve tesis/hat boşsa (durations bağlantısı) tesis ve hat makineden doldurulur.
- Usage modunda `Machines` tesis/hat seçimine göre (tesis yoksa boş); diğer modlarda tüm makineler. `Usage` yalnız hatasızsa dolu, seçim gerekmez; `CompletedRecordCount` 0 ise `Rows` boş.
- Ortalamalar `int?`: Tamamlandı yoksa null; acilde `AvgPlannedMinSeconds` ve `PlanVarianceSeconds` null; `PlanVarianceSeconds` negatif olabilir.
- Değerlendirilen faz: minimum anlık değeri ve net süresi dolu, Tamamlandı kayıtların fazı. `ExpiredUseCount` süresi geçmiş girişli satır sayısı.
- Kullanım oranı `decimal`, bir ondalık, 0,05 yukarı.

## Sapmalar
- Dosya sınırı dışı: `Services/IReportOptions.cs`, `Services/ReportOptions.cs` (`ReconcileSelectionAsync`, koordinatör isteği); yeni `ViewModels/Reports/MaterialUsageFilter.cs`, `MachineReportLinks.cs` (sınır içi klasör).
- Sözleşmede olmayan ek: `MachineReportFilterViewModel` ile `MaterialUsageFilter` içinde `Kind`.

## DB talebi
Yok. Index gerekmedi (küçük veri; büyük veride `CleaningRecords(MachineId, OpenedAtUtc)` ölçülür, onay ister).

## Düzeltme (yerleşim ön turu, DÜŞÜK)
- `MachineReportReader.FillParentsAsync`: makine var ve tesis ya da hat boşsa doğrulamadan önce makineden doldurulur; tarih ve tür hatasında da seçim ve seçenekler korunur. Olmayan makinede tesis listesi dolu, hat ve makine listesi boştur.
- `ReportsController.Machine`: okunamayan değerde süzgeç sıfırlanmaz; bağlanabilen alanlar korunur, okunamayanlar null olur, `ErrorMessage` yine dolar.

## Tur 1 İADE düzeltmesi (B1, B3, PL-97)
- `IReportOptions.ReconcileSelectionAsync` artık `(FacilityId, LineId, MachineId)` döner: uyuşmayan hat ve makine boşaltılır, makine kalıyorsa boş tesis ve hat makineden doldurulur. `MachineReportReader.FillParentsAsync` kaldırıldı.
- `DurationReportReader` (E-30) `FindInvalidSelectionAsync`'ten önce çağırır; `MaterialTraceReader` Kullanım oranı ve Makine modunda çağırır.
- `ReportsController.Materials`: okunamayan değerde istek sıfırlanmaz (mod ve okunabilen seçim korunur), hata metni yine dolar.
- Not: E-31 Makine modunda tesis ve hat alanı yoktur; `lineId=999` o modda yok sayılır (hata vermez). E-30'da okunamayan değer süzgeci hâlâ sıfırlar (istenmedi).

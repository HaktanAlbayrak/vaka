DEVİR: backend-engineer -> koordinatör (dbadmin, sonra backend devamı)   Faz: 2 (ilk yarı)   Dosya: Docs/Handover/backend-engineer-20261114-2300.md

Yapıldı:
- 22 ViewModel sözleşmedeki adlarla `ViewModels/Definitions/` altında yazıldı (frontend derlemesi bağlı; derleniyor).
- DB talebi `Docs/Db/DB-2026-004.md` (Eklemeli, altı yeni tablo, nötr demo seed, SKT'si geçmiş lot `L2026-001`), `DbChanges.md` indeksine TALEP satırı. Mevcut tabloya dokunan ihtiyaç doğmadı, ALTER yok.
- Süre ve tarih kararları: `IDurationCalculator` içine `EndsNextDay`, `DoShiftsOverlap`, `FindShiftAt` (anı içeren vardiya, gece yarısını geçen dahil, Faz 4 SH-13 ve Faz 6 SH-16 bunu kullanır), `IsLotExpired` (SKT < Türkiye bugünü, BR-12); hepsinde formül XML yorumunda. `ITurkeyClock`/`TurkeyClock` (Türkiye saat dilimi, `ToLocal`, `Today`); `DurationCalculator` artık `ITurkeyClock` alır. Repo dışı bir deneme konsoluyla doğrulandı: sınır anları (06:59:59, 07:00:00, 15:00:00, 23:00:00, 00:00), gece vardiyasının dünden başlaması, bitiş=başlangıç çakışmaması, SKT bugün=geçmedi.
- Servis arayüzleri ve istek kayıtları: `IFacilityService`, `ILineService`, `IShiftService`, `IShiftLocator`, `IMachineService`, `IMachineRetirement`, `IMachineBlockProvider` (+ boş döndüren `NoMachineBlockProvider`, Faz 4 ve 7'de dolar), `IMaterialService`, `IMaterialLotService`. Ortak doğrulama `DefinitionRules` (kod: boş, uzunluk, tire; ad; PL-04 kod değişimi).
- Altı controller, hepsi `[Authorize(Roles = Roles.Manager)]` (Süper admin, Operatör, Denetçi 403): `FacilitiesController`, `LinesController`, `ShiftsController`, `MachinesController`, `MaterialsController`, `MaterialLotsController`. Kayıt `Program.cs`: `ITurkeyClock`, `IMachineBlockProvider`.
- Doğrulama: `dotnet build Src/Vaka.Web -warnaserror -o .qa/be/build` 0 uyarı 0 hata; §4 grep'leri ve em/en-dash 0 eşleşme. Servis uygulamaları olmadığı için route'lar henüz çalıştırılmadı (curl doğrulaması kalanlar listesinde).

Route, rol, ViewModel (hepsi Manager; sözleşmeyle aynı):
- GET /facilities (showInactive) -> FacilityListViewModel; GET/POST /facilities/create, /facilities/edit/{id} -> FacilityFormViewModel (gönderilen: Code, Name, IsActive); GET /facilities/details/{id} -> FacilityDetailViewModel.
- GET/POST /lines/create?facilityId=, /lines/edit/{id} -> LineFormViewModel (POST'ta FacilityId gizli alan olarak gelmeli).
- GET /shifts?facilityId= -> ShiftListViewModel; GET/POST /shifts/create?facilityId=, /shifts/edit/{id} -> ShiftFormViewModel (POST'ta FacilityId gizli alan; düzenlemede tesis değişmez).
- GET /machines (facilityId, lineId, query, showRetired, page) -> MachineListViewModel; GET/POST /machines/create?facilityId=, /machines/edit/{id} -> MachineFormViewModel; GET /machines/lines?facilityId= -> JSON [{id,label}]; GET/POST /machines/retire/{id} -> MachineRetireViewModel (POST gövdesi yok, yalnız antiforgery).
- GET /materials (query, showInactive, page) -> MaterialListViewModel; GET/POST /materials/create, /materials/edit/{id} -> MaterialFormViewModel; GET /materials/details/{id} -> MaterialDetailViewModel.
- GET/POST /material-lots/create?materialId=, /material-lots/edit/{id} -> MaterialLotFormViewModel (POST'ta MaterialId gizli alan).

Sözleşmeye ek ve seçimler (analist/frontend görsün):
- Ek alan: `StatusMessage` (string?) liste ve ayrıntı VM'lerinde (Facility list/detail, Shift list, Machine list, Material list/detail); Users ekranlarındaki gibi yönlendirme sonrası başarı iletisi. Formlara eklenmedi.
- Tipler: `StartTime`/`EndTime` formda `TimeOnly?`, listede `TimeOnly`; `ExpiryDate` formda `DateOnly?`, listede `DateOnly`; `RetiredAtLocal` `DateTime?` (Türkiye saati). Çözülemeyen saat/tarih modelde boş gelir ve Türkçe alan hatası olur ("Başlangıç saatini girin" gibi).
- `Facilities[]`/`Lines[]` makine VM'lerinde `SelectOptionViewModel { Id, Label }`, etiket "Kod - Ad"; Shifts için `FacilityOptionViewModel { Id, Name }` (tüm tesisler, pasif dahil).
- PL-04 düzenleme: kod ya da hat alanı gönderilmezse (boş ya da 0) değişmemiş sayılır, dolu ve farklıysa "Code" (makinede hat için "LineId") alan hatasıyla reddedilir. Frontend kilitli alanı `disabled`/`readonly` gösterebilir.
- K3 (JS kapalı): `GET /machines/create?facilityId=N` hat listesini N tesisine göre doldurur; formda tesis seçimini GET ile yenileyen bir düğme gerekir. Aynı şekilde makine listesi filtresinde hat listesi yalnız seçili tesisin aktif hatlarını taşır (tesis seçilmemişse tüm hatlar, etiket "Kod - Ad"); JS kapalıyken tesis seçip filtreyi uygulamak listeyi yeniler.
- Başarı yönlendirmeleri: tesis ve malzeme eklenince Details, hat eklenince/düzenlenince tesis Details, vardiya Index(facilityId), makine Index, lot malzeme Details.
- Makine kullanımdan kaldırma PIN istemez (sözleşmede Pin alanı yok). BR-48'in kritik işlem listesinde yer alıp almadığı analist kararıdır; gerekirse VM'ye Pin eklenir.
- Hata dönüşleri: iş kuralı reddi form hatası (alan anahtarı `Code`, `Name`, `StartTime`, `EndTime`, `LotNo`, `ExpiryDate`, `LineId`, `FacilityId`; alana bağlı olmayan hata boş anahtar); kayıt yoksa 404.

Kalanlar (dbadmin sonucundan sonra, bu sırayla):
1. Entity adları DB-2026-004'teki gibi gelince: `DbContext`ten okuyan altı servis uygulaması: `FacilityService`, `LineService`, `ShiftService`, `MachineService`, `MachineRetirement`, `MaterialService`, `MaterialLotService` (+ `ShiftLocator`). Kurallar: kod doğrulaması ve benzersizlik (BR-54, `DbUpdateException` yakalanıp "Bu kod kayıtlı" alan hatası), PL-04, pasif tesis/hat altına yeni kayıt reddi (BR-05), vardiya çakışması (`DoShiftsOverlap`, yalnız aktif vardiyalar, mesajda çakışan vardiyanın adı), başlangıç=bitiş reddi, SKT geçmiş `IsLotExpired(.., clock.Today())`, retire (engel varsa reddet, `IsActive=false`, `RetiredAtUtc`). Liste: `AsNoTracking`, projeksiyon, `LIKE` sorgusu, sunucuda sayfalama (20), kod sırası.
2. `Program.cs`: yedi servis ve `IShiftLocator` kaydı.
3. Çalıştırma doğrulaması (`.qa/be/` kopya DB, boş port): her yeni route için oturumsuz 302, Operatör/Denetçi/yalnız Süper admin 403, Yönetici 200; K1, K2, K4 (çakışma, sınır, pasif), K5, K6 ve antiforgery'siz POST 400 curl ile. Yönetici hesabı için kopya DB'de geçici kullanıcı kopyada açılır; geliştirme DB'sine yazılmaz.
4. `IShiftLocator` Faz 2'de çağıran olmadan kalır; Faz 4 SH-13 ve Faz 6 SH-16 kullanır. Kullanılmayan kod sayılmaması için analist onayı gerekirse kalem 1'den çıkarılıp Faz 4'e bırakılır.

Frontend/QA için:
- ViewModel derleme bağı hazır; servisler gelene kadar route'lar DI hatası verir (controller oluşturulamaz). Görünümler seed gelince çalışan uygulamada denenir.
- Menüde Faz 2 adresleri yalnız `User.IsInRole(Roles.Manager)` için görünmeli (G10).

DB talebi: DB-2026-004 (TALEP, Eklemeli). Koordinatör dbadmin'i çağırmalı.
Açık/ertelenen: önceki notun maddeleri (LIKE kaçışı, parola kilidi yok, PIN işlem ayrımı Faz 4) değişmedi.

DEVİR: backend-engineer -> koordinatör   Faz: 6B (BE 1-3)   Dosya: Docs/Handover/backend-engineer-20261115-2345.md

Yapıldı:
- BE 1: `HomeController.Index` Operatör (çoklu rolde de) `/my-work`; Yönetici ya da Denetçi `/dashboard`; yalnız Süper admin `/users`.
- BE 2: `GET /dashboard`, `DashboardController.Index`, `[Authorize(Roles = Roles.ManagerOrAuditor)]`, `IDashboardReader` / `DashboardReader` (Program.cs'te scoped). Okuma önce `TimeoutScope.Everything` uygular; sorgular `AsNoTracking` + projeksiyon. Yeni süre formülü yok, süreler saklı saniyelerdir.
- BE 3: İş emri no isteğe bağlı. Boş ya da yalnız boşluk ise planlı kayıtta aynı transaction içinde `NumberSequence` kapsamı `WorkOrder:<TesisId>` ve Türkiye yılıyla `<TesisKodu>-IE-<Yıl>-<Sıra>` atanır (`RecordNumbers.FormatWorkOrder`). Dolu ise trim edilip saklanır, sayaç ilerlemez. Sistem biçimine uyan değer (`^.+-IE-[0-9]{4}-[0-9]+$`, büyük/küçük harf duyarsız) ve 64 karakteri aşan değer `WorkOrderNo` alan hatasıyla reddedilir (uzunluk önce denetlenir, tek hata döner). Açılış olayı saklanan (atanmış ya da girilen) değeri taşır. Acil açılış ve eski kayıtlar değişmedi. `RecordCreateViewModel` değişmedi (zaten `[Required]` taşımıyordu; zorunluluk doğrulayıcıdaydı, kalktı).

Değişen dosyalar (hepsi `Src/Vaka.Web/`):
- Yeni: `Controllers/DashboardController.cs`, `Services/IDashboardReader.cs`, `Services/DashboardReader.cs`, `ViewModels/Dashboard/` (DashboardViewModel, DashboardActiveRecordViewModel, DashboardBelowMinimumViewModel, DashboardCompletedViewModel, DashboardDayCountViewModel)
- Değişen: `Controllers/HomeController.cs`, `Services/RecordNumbers.cs` (WorkOrderScope, FormatWorkOrder, LooksLikeSystemWorkOrder, sınıf `partial`), `Services/RecordOpenValidator.cs`, `Services/RecordOpener.cs`, `Program.cs` (tek satır kayıt)

Öz denetim:
- `dotnet build -warnaserror`: 0 uyarı, 0 hata. Not: `bin\Debug\...\Vaka.Web.exe` başka bir süreç (PID 32180, benim değil) tarafından kilitliydi; derlemeyi `-o` ile scratchpad'e aldım, o süreci kapatmadım.
- CodingStandards §4 grep'leri 0 eşleşme; yeni dosyalarda em/en-dash 0.
- Çalıştırma: kullanıcının `App_Data/vaka.db` dosyasının kopyası, `--urls` ile 5481 ve 5482, işim bitince yalnız bu iki PID kapatıldı. Kopyada şifre/PIN yalnız test için sıfırlandı.
  - `/` : Yönetici 302 `/dashboard`, Denetçi 302 `/dashboard`, Süper admin 302 `/users`, Operatör (PIN kapısı geçilmeden) 302 first-login; oturumsuz 302 login.
  - `/dashboard`: oturumsuz 302 login; Operatör ve Süper admin 302 access-denied (projedeki mevcut 403 yönlendirmesi); Yönetici 200 ve Denetçi 200 (görünüm henüz yoksa 500 `Views/Dashboard/Index.cshtml` bulunamadı; 200'ü FE görünümü gelmeden geçici JSON görünümüyle, scratchpad'deki proje kopyasında doğruladım, repoya girmedi).
  - Pano verisi kopya DB ile örtüştü: Açıldı 0, Devam ediyor 1, bugün tamamlanan 8 (durum 4 ve bugün), Son 7 gün 7 satır, bugün Planlı 10 Acil 0, süre altı satırları gerekçeli, listeler en yeni önce.
  - İş emri: boş -> `T1-IE-2026-1`, yalnız boşluk -> `...-2`, `ERP-778` aynen saklandı ve sayaç ilerlemedi, sonraki boş -> `...-3`; aynı `CommandId` ile iki gönderim tek kayıt (aynı kayıt id, sayaç artmadı). `T1-IE-2026-5`, `xx-ie-2026-0005` ve 65 karakter: 200 + alan hatası, kayıt sayısı değişmedi.
  - Doğrulanamadı: ikinci tesiste sıranın ayrı başlaması HTTP ile denenemedi (kopyada ikinci tesis makinesi "Tesis seçilen makineye uygun değil" verdi, mevcut doğrulama); kapsam tesis kimliğine bağlı olduğundan koddan ayrı sayaç satırı oluşur. QA K16'da denesin.

Sonrakinin bilmesi gerekenler (frontend):
- Görünüm modeli `Vaka.Web.ViewModels.Dashboard.DashboardViewModel`: `OpenCount`, `InProgressCount`, `CompletedTodayCount` (int); `ActiveRecords[]`: `RecordId`, `RecordNo`, `Kind` (CleaningTypeKind), `KindLabel`, `Status` (RecordStatus), `StatusLabel`, `MachineLabel` ("M01 - Karıştırıcı"), `OwnerName`, `OpenedAtLocal` (DateTime), `LockExpiresAtLocal?` (yalnız Açıldı durumunda dolu), `CurrentStepTitle?` (devam eden adım yoksa null), `Url` (`/records/details/{id}`); `BelowMinimum[]`: `RecordId`, `RecordNo`, `MachineLabel`, `PhaseTitle`, `NetSeconds`, `MinDurationSeconds`, `Reason` (boş olabilir), `CompletedAtLocal`, `Url`; `RecentCompleted[]`: `RecordId`, `RecordNo`, `Kind`, `KindLabel`, `MachineLabel`, `CompletedAtLocal`, `NetSeconds`, `EffortSeconds`, `Url`; `Last7Days[]` (her zaman 7, eski önce): `Date` (DateOnly), `DateLabel` ("9 Kas Pzt"), `PlannedCount`, `EmergencyCount`. Alan adları sözleşmeyle aynı, sapma yok.
- `ActiveRecords` en eski açılış önce; `BelowMinimum` ve `RecentCompleted` en yeni önce, en çok 10; yalnız Tamamlandı kayıtlar (Zaman aşımı, İptal, Yarıda kapatma sayılmaz). Kayıt sahibi Operatör için bağlantı yok, pano yalnız Yönetici ve Denetçi.
- Yeni kayıt formunda `WorkOrderNo` alan hatası anahtarı `WorkOrderNo` (D-1 hata yeri); hata metinleri: "Bu biçim sistemin atadığı numaraya ayrılmıştır. Alanı boş bırakın ya da dış iş emri numarasını girin." ve "İş emri numarası en çok 64 karakter olabilir." Sunucu artık boş değeri kabul eder; formdaki `required` istemci tarafında FE'nin işi.
- Gömülü sayı: pano süre metinleri için `IDurationFormatter` görünümde FE'nin mevcut yolundan kullanılır (ViewModel ham saniye taşır).

DB talebi: yok

Açık/ertelenen:
- Pano Faz 7 ve 8 alanları (`NeedsAttention`, `OnHold`, `PendingRequests`, `Emergencies`) eklenmedi (planlandığı gibi).

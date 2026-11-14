DEVİR: backend-engineer -> koordinatör (dbadmin, sonra backend-engineer)   Faz: 1   Dosya: Docs/Handover/backend-engineer-20261114-0100.md

Yapıldı:
- BE 9 (frontend derlemesi için): `ViewModels/Account|Home|Users/**` sözleşmedeki adlarla, `Authorization/VakaClaimTypes.cs` (`UserId`, `RegistryNo`), `Roles.Label`. Sözleşmeden sapma yok. Derleme geçiyor.
- DB talepleri açıldı: `Docs/Db/DB-2026-001.md` (Users, UserRoles, tek Süper admin seed) ve `Docs/Db/DB-2026-002.md` (SystemSettings, SettingChanges, üç ayar satırı). `DbChanges.md` indeksine iki TALEP satırı eklendi. Süper admin parolası koda, appsettings'e ve talebe girmez: dbadmin geliştirme parolası seçer, `PasswordHasher<string>` V3 özetini seed'e sabit yazar, parolayı yalnız koordinatöre iletir (README'ye koordinatör işler). Seed'de `MustChangePassword` ve `MustSetPin` true.
- DB'den bağımsız kısımlar yazıldı. `Services/`: `CommandResult`, `SettingKeys`, `ISystemSettingsReader`, `ICredentialHasher` + `CredentialHasher`, `PasswordRules`, `IPinFormatValidator` + `PinFormatValidator` (BR-49), `IPinService`, `IAccountService`, `IUserDirectory`, `IUserAdministration`, `ManagementScope` (SK-9), `IOpenWorkProvider` + `NoOpenWorkProvider`, `IDurationCalculator` + `DurationCalculator.IsSessionIdle` (formül yorumlu). `Authorization/`: `AccountPrincipal`, `SessionEndReason`, `VakaCookieEvents` (son istek anına göre işlemsizlik), `FirstLoginGateFilter` (yetki denetiminden önce çalışan istek filtresi).
- Controller'lar: `AccountController`, `HomeController`, `UsersController`, `SessionControllerExtensions` (çıkış yönlendirmesi, ModelState hata ekleme). T-B15: `Program.cs`'e genel `AutoValidateAntiforgeryTokenAttribute`; `PrototypeController` action'ları `[HttpGet]`; `ErrorController`'a `[IgnoreAntiforgeryToken]` eklendi (hata sayfası yeniden çalıştırması POST'ta token doğrulamasına takılmasın; plandaki "yalnız gerekirse" kapsamında).
- `Program.cs` yalnız DB'den bağımsız kayıtları taşıyor: antiforgery filtresi, `SuppressImplicitRequiredAttributeForNonNullableReferenceTypes`, `TimeProvider`, hasher, süre hesaplayıcı, `NoOpenWorkProvider`.

Doğrulama:
- `dotnet build Src/Vaka.Web -warnaserror`: 0 uyarı, 0 hata.
- §4 grep'leri 0 eşleşme; Controllers/Services/ViewModels/Authorization/Docs/Db içinde em/en-dash 0.
- Development, 127.0.0.1:5291: `/prototype/run` 200, `/error/403` 403. `/users` 500: kimlik doğrulama henüz bağlı değil (aşağıda). Süreç kapatıldı.
- Giriş, ilk giriş, PIN, kullanıcı yönetimi, oturum zaman aşımı ve 403/302 akışı çalışan uygulamada henüz doğrulanamadı; DB olmadan doğrulanamaz.

dbadmin sonucundan sonra yapılacaklar (backend-engineer, sırayla):
1. `Data/` ve entity adlarını dbadmin notundan al; `AddDbContext<VakaDbContext>` ve `appsettings.json` bağlantı dizesi (yalnız SQLite dosya yolu).
2. DB'ye bağlı sınıfları yaz (`Services/`): `SystemSettingsReader` (satır yoksa `InvalidOperationException`), `PinService` (art arda `PinDenemeSınırı` hatada `PinLockedAtUtc`; doğru PIN sayacı sıfırlar; kilitliyken `Locked`), `AccountService` (giriş: pasif ve hatalı parola aynı sonuç; ilk giriş; parola ve PIN değiştirme; `GetStateAsync`), `UserDirectory`, `UserAdministration` (SK-9, PL-16, PL-27, son aktif Süper admin, PIN en son doğrulanır).
3. `Program.cs` bağlama: `AddAuthentication` + cookie (LoginPath `/account/login`, AccessDeniedPath `/account/access-denied`, `EventsType = typeof(VakaCookieEvents)`, `SlidingExpiration = false`, oturum çerezi, `ExpireTimeSpan` yalnız teknik tavan), scoped kayıtlar, `Filters.Add<FirstLoginGateFilter>()`. Bu parça DB'ye bağlı servisler kayıtlı olmadan eklenemedi: olay sınıfı her istekte çözüldüğü için prototip sayfaları da düşerdi.
4. Öz denetim: her yeni route için oturumsuz / yetkisiz rol / yetkili rol curl; K1-K14, G5 (token'sız POST 400), K12 (QA kopyasında kısa oturum süresi).

Sonrakinin bilmesi gerekenler (frontend):
- Görünüm adları: `Account/Login`, `Account/AccessDenied`, `Account/FirstLogin`, `Account/Security`, `Home/Index`, `Users/Index`, `Users/Form` (Create, Edit ve Activate/ResetPin/ResetPassword hata durumu), `Users/Deactivate`. `@model` tipleri sözleşmedeki ViewModel'ler.
- Route'lar: `/`, `/account/login`, `/account/logout` (POST), `/account/access-denied`, `/account/first-login`, `/account/security`, `/account/change-password` (POST), `/account/change-pin` (POST), `/users?query=&status=&page=`, `/users/create`, `/users/edit/{id}`, `/users/deactivate/{id}`, POST `/users/activate/{id}`, `/users/reset-pin/{id}`, `/users/reset-password/{id}`. `status` değerleri `Active|Inactive|All` (harf duyarsız).
- Yetki reddi (rol ya da SK-9 kapsamı) `Forbid()` ile verilir: tarayıcı `/account/access-denied`'a yönlenir, son yanıt 403. Oturumsuz: `/account/login?ReturnUrl=...` 302.
- Oturum bitiş nedeni: `/account/login?reason=expired|inactive|pin-locked|signed-out`; metin controller'da `LoginViewModel.StatusMessage` olur. Giriş hatası `ErrorMessage`'dadır; alan hataları ModelState'te (`RegistryNo`, `Password`).
- "Kullanıcı değiştir": `POST /account/logout`, antiforgery doğrulaması kapalı (K11a); form yine de token taşıyabilir. Diğer tüm POST'larda token zorunlu (form tag helper'ı otomatik ekler).
- Form alan adları: Login `RegistryNo`, `Password`, `ReturnUrl`. FirstLogin `CurrentPassword`, `NewPassword`, `ConfirmPassword` (yalnız `MustChangePassword` ise), `NewPin`, `ConfirmPin` (yalnız `MustSetPin` ise); sunucu bayrakları veritabanından yeniden okur. Security `Password.CurrentPassword|NewPassword|ConfirmPassword`, `Pin.CurrentPin|NewPin|ConfirmPin`; hata anahtarları aynı önekli. Users create: `RegistryNo`, `FullName`, `SelectedRoles` (aynı adlı onay kutuları), `TemporaryPassword`; edit: `FullName`, `SelectedRoles` (`RolesLocked` ise gönderilse de yok sayılır; `RegistryNo` yok sayılır). Deactivate: alan adı `Pin`. Activate/ResetPin/ResetPassword: `Activate.Pin`, `ResetPin.Pin`, `ResetPassword.TemporaryPassword`, `ResetPassword.Pin`. Alana bağlı olmayan hata (yanlış PIN, kapsam kuralı, açık iş) boş anahtarda gelir: `asp-validation-summary="ModelOnly"`.
- Başarı iletisi TempData'dan `StatusMessage` alanına dolar; frontend TempData okumaz. Başarısız `Activate/ResetPin/ResetPassword` ve `Create/Edit` yanıtı `Users/Form` + tam `UserFormViewModel`.
- Rol alanları `Roles` sabit değerini taşır; yazı için `Roles.Label(role)`. Kabukta ad `User.Identity.Name`, sicil no `User.FindFirst(VakaClaimTypes.RegistryNo)`.
- Hata sonrası yeniden çizimde ModelState gönderilen değeri geri yazabilir; PIN ve parola alanlarında `value` basılmasın (görünüm tarafı).
- İlk giriş kapısı `/account/first-login`, `/account/login`, `/account/logout`, `/account/access-denied`, `/error*` dışındaki her istekte çalışır (plandaki listeye `/account/login` eklendi: ilk girişi bitirmemiş oturumlu kişi giriş sayfasını açabilsin).
- `PrototypeController` yalnız GET; formlar yok.

Koordinatör maddeleri (frontend devir notundan):
1. PIN reddi alan anahtarıyla gelir: servisler `CommandResult.FromPin(status, "Pin")` döner (`IUserAdministration` yorumunda yazılı), controller önek ekler: `ResetPin.Pin`, `ResetPassword.Pin`, `Activate.Pin`, pasifleştirmede `Pin`. Kilitlenme (`PinLocked`) oturumu kapatıp girişe yönlendirir. Servis gövdesi DB sonrası yazılacak; bu kural orada uygulanır.
2. `UserFormViewModel.DeactivateBlockReason` (string?) eklendi: `CanDeactivate` false iken nedeni (kendi hesabı, son aktif Süper admin, zaten pasif) servis doldurur. Sözleşmeye ek alan; mevcut alanları değiştirmez, frontend bağlar. Açık iş engeli yine `UserDeactivateViewModel.BlockReason`'dadır.
3. K11a: `POST /account/logout` zaten `[AllowAnonymous]` + `[IgnoreAntiforgeryToken]`; oturumu düşmüş kişinin token'ı eski kimlikle üretildiğinden doğrulanamaz, 400 yerine girişe yönlenir (`?reason=signed-out`). T-B15 ile çelişmez: genel `AutoValidateAntiforgeryTokenAttribute` durur, yalnız bu action'da istisna vardır; logout'un zorla tetiklenmesi veri değiştirmez ve en kötü sonucu oturumun kapanmasıdır. Diğer tüm POST'lar token ister.
4. Login POST: kimlik hatasında yalnız `ErrorMessage` doldurulur, ModelState'e hata eklenmez; ModelState hatası (boş alan) olduğunda `ErrorMessage` boştur. İkisi birlikte dolmaz.

DB talebi: Docs/Db/DB-2026-001.md, Docs/Db/DB-2026-002.md (ikisi de Eklemeli, TALEP). 001 önce uygulanmalı, 002 Users'a FK verir.
Açık/ertelenen:
- Seed sicil no ve ad: analist belirlemedi; dbadmin nötr değer seçer (öneri `0001`, `Süper Admin`), koordinatör README'ye işler.
- Çerez `ExpireTimeSpan` teknik tavan olacak; işlemsizlik kararı yalnız ayardan ve `IsSessionIdle`'dan gelir. Tavan Program.cs'te adlandırılmış sabit olur, ayar sayılmaz. Karşı görüş varsa analist bildirsin.
- PIN doğrulama ile iş eylemi bu fazda iki ayrı kayıt işlemidir (sayaç ayrı kaydedilir); BR-48 "aynı transaction" kuralı olay günlüğünün geldiği Faz 4'te tek işleme alınır.
- Parola için deneme sınırı ya da kilit istenmedi, yazılmadı.

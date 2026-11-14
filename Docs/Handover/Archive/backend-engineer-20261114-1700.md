DEVİR: backend-engineer -> koordinatör (qa-tester)   Faz: 1   Dosya: Docs/Handover/backend-engineer-20261114-1700.md

Yapıldı:
- BE 9 (ViewModel'ler, `VakaClaimTypes`, `Roles.Label`) ve dbadmin sonrası kalan her şey tamamlandı. `appsettings.json` bağlantı dizesi (`ConnectionStrings:Default`), `AddDbContext` (göreli SQLite yolu içerik köküne çevrilir), cookie kimlik doğrulama (`VakaCookieEvents`, `SlidingExpiration = false`, `ExpireTimeSpan` 30 gün teknik tavan, işlemsizlik kararı ayardan), `FirstLoginGateFilter` (Order -1000, yetki denetiminden önce).
- DB'ye bağlı servisler: `SystemSettingsReader`, `PinService`, `AccountService`, `UserDirectory`, `UserAdministration`, `UserQueries` (iç yardımcı: roller, son aktif Süper admin, pasifleştirme engeli).
- Önceki turdaki koordinatör maddeleri uygulandı: PIN reddi alan anahtarıyla (`Pin`, controller önekler: `ResetPin.Pin`, `ResetPassword.Pin`, `Activate.Pin`); `UserFormViewModel.DeactivateBlockReason` servis tarafından doluyor (kendi hesabı, son aktif Süper admin, zaten pasif); logout antiforgery istisnası; Login'de `ErrorMessage` ile ModelState hatası birlikte dolmaz.
- `ErrorController`'a `[IgnoreAntiforgeryToken]`, `PrototypeController` action'ları `[HttpGet]`, genel `AutoValidateAntiforgeryTokenAttribute` (T-B15).

Doğrulama (kendi DB kopyam `.qa/be/vaka.db`, geliştirme DB'sine dokunulmadı; port 5292, kapatıldı):
- `dotnet build Src/Vaka.Web -warnaserror`: 0 uyarı, 0 hata. §4 grep'leri ve em/en-dash: 0 eşleşme. Geliştirme parolası hiçbir dosyada yok.
- K1: oturumsuz `/users` `/account/login?ReturnUrl=%2Fusers` 302. `/`, `/users/create`, `/account/security` aynı.
- K2: seed kullanıcısı geçici parolayla girince `/`, `/users`, `/account/security`, `/users/create` `/account/first-login`'e 302; bitince `/` -> `/users` 302.
- K3: 3 ve 5 haneli, `12a4`, `1111`, `1234`, `4321` alan altında nedenli reddedildi; `0482` kabul edildi ve sonraki doğrulamada geçti (ilk giriş, PIN değiştirme, sonraki `reset-pin` onayı); `1357` kabul.
- K4: SA yönetici açtı, yönetici operatör ve denetçi açtı; aynı sicil "Bu sicil no kayıtlı."; edit isteğinde `RegistryNo=9999` gönderildi, kayıt `2001` kaldı.
- K5: Operatör `/users`, `/users/create`, `/users/edit/3`, `/users/deactivate/3` GET ve `activate`, `reset-pin`, `reset-password`, `create`, `deactivate` POST: hepsi son yanıt 403 (`/account/access-denied`). Operatör menüsünde `/users` bağlantısı 0. Denetçi için ayrı tur yapılmadı, aynı `Roles.ManagerOrSuperAdmin` kuralı.
- K6: yanlış PIN'le pasifleştirme ve etkinleştirme reddedildi, durum değişmedi; doğru PIN'le değişti; pasif kullanıcı girişi "pasif" mesajıyla reddedildi.
- K7: 4 hatalı PIN sonrası oturum açık; doğru PIN sayacı sıfırladı; yeniden 4 hata sonrası açık; 5. hata oturumu kapattı ve `/account/login?reason=pin-locked` (nedeni gösteriyor). Kilitliyken işlem "PIN kilitli..." ile reddedildi. SA PIN'i sıfırlayınca kullanıcı sonraki girişte yalnız yeni PIN alanını gördü.
- K8: kendi hesabı pasifleştirme reddedildi; ikinci Süper admin açılınca biri pasifleştirildi; kendi rolleri değiştirilemedi (`RolesLocked`, istek yok sayıldı). "Son aktif Süper admin" kuralı kodda var; başka bir aktif Süper admin olmadan o kural tetiklenemediği için yalnız kendi-hesap kuralı üzerinden dolaylı görüldü.
- K8a: Yönetici, `Manager`/`SuperAdmin` hesaplarına edit, deactivate GET ve activate, reset-pin, reset-password POST'ta 403; listede bu hesapta düzenleme bağlantısı yok; `Manager` ya da `SuperAdmin` rolü atama isteği "Seçilen rolü atayamazsınız." ile reddedildi.
- K9: yanlış eski PIN reddedildi ve hatalı deneme sayıldı; doğru eski PIN + yeni PIN değişti.
- K10: DB dosyasında dört test parolası düz metin olarak yok, hash'ler `AQAAAAIAAYag` (PasswordHasher V3); uygulama günlüğünde PIN yok.
- K12: oturum süresi 1 dakikaya çekildi (kopyada, 2 yerine en küçük pozitif değer): 40 sn aralıklı 4 istek (toplam 120 sn) açık kaldı; 65 sn işlemsizlikten sonra `...&reason=expired` 302. `PinUzunluğu` 5 ve `PinDenemeSınırı` 2 değerleri sonraki istekte uygulandı (4 haneli yeni PIN reddi, 2. hatalı denemede kilit).
- K12a: ayar tablosunda tam üç satır (30, 4, 5), geçmiş boş; `Value = 0` ve `Value = -1` `CK_SystemSettings_Value` ile reddedildi.
- K13: Operatör + Yönetici rollü kullanıcı `/users` 200, menüde bağlantı; Yönetici rolü alınınca aynı oturum (yeniden giriş olmadan) sonraki istekte 403 aldı. Yalnız Operatör 403.
- K11a: oturumu düşmüş `POST /account/logout` token'sız `/account/login?reason=signed-out` 302 verdi (400 değil).
- G5: oturumlu token'sız `POST /users/create`, `/account/change-pin` 400; token'sız `POST /account/login` 400. Oturumsuz token'sız POST önce giriş yönlendirmesi (302) alır. `/account/logout` bilinçli istisna.
- K14 (backend payı): `/prototype/run` Development'ta 200.
- Yapılmadı: K8b ve K11 görünüm kısımları (menü, 360/768/1440 görünürlük) ve tarayıcı turu QA'ya ait; benim ölçtüğüm yalnız operatör ve `/users` bağlantı varlığı.

Sonrakinin bilmesi gerekenler (QA, frontend):
- QA kopyasında oturum süresi için `SystemSettings.Value` doğrudan güncellenir (`SessionMinutes`); en küçük değer 1 (CHECK). Seed'li geliştirme DB'sinde Süper admin `0001` hâlâ ilk giriş bekliyor.
- Yetki reddi 302 + `/account/access-denied` 403'tür; curl'de POST yönlendirmesini izlemek için `-X POST` kullanma (`-d` ile bırak, curl 302'de GET'e döner), yoksa 405 görünür.
- Form alan adları ve `reason` değerleri bir önceki notla aynıdır (Archive/backend-engineer-20261114-0100.md).
- Rol değişikliği çerezdeki kimliğe sonraki istekte yansır (olay sınıfı veritabanından yeniler); pasifleşen kişinin oturumu sonraki istekte `reason=inactive` ile düşer.
- Yeni DB talebi gerekmedi.

DB talebi: yok (DB-2026-001, DB-2026-002 uygulanmış kullanıldı)
Açık/ertelenen:
- Geliştirme DB'sinde (`App_Data/vaka.db`) hiçbir test verisi bırakılmadı; QA kendi kopyasını almalı.
- PIN doğrulama ile iş eylemi iki ayrı kayıt işlemi (sayaç ayrı kaydedilir); BR-48 "aynı transaction" Faz 4'te olay günlüğüyle tek işleme alınır.
- Parola için deneme sınırı/kilit planda yok, yazılmadı. LIKE joker karakterleri (`%`, `_`) arama metninde kaçışlanmıyor (düşük etki, Faz 1 liste araması).
- Son aktif Süper admin kuralının rol kaldırma kolu yalnız kod okumasıyla doğrulandı (yukarıdaki gerekçe).

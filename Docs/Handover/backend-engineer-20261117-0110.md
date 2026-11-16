# Backend devir notu: Faz 6F (PIN ile giriş ve giriş deneme sınırı)

## Route ve rol
| Route | Rol | Açıklama |
|---|---|---|
| GET `/account/login?mode=pin\|password` | anonim | `mode` yoksa `vaka-login-mode` çerezi, çerez yoksa PIN. Geçersiz `mode` çereze düşer |
| POST `/account/login` | anonim | `LoginViewModel { Mode, RegistryNo, Password, Pin, ReturnUrl, ErrorMessage, StatusMessage, LockedUntilLocal }` |
| POST `/users/unlock-login/{id}` | ManagerOrSuperAdmin (servis: SK-9 kapsamı) | `UserResetViewModel`, önek `UnlockLogin`; hata anahtarı `UnlockLogin.Pin`; başarıda `/users/edit/{id}` ve "Giriş kilidi açıldı." |
| GET `/users/edit/{id}` | ManagerOrSuperAdmin | `UserFormViewModel`: `LoginFailedCount`, `LoginLockedUntilLocal` (yalnız kilit sürüyorsa), `CanUnlockLogin` (kilit sürüyor ve kapsam içinde), `UnlockLogin` |

Oturumsuz 302 giriş; kapsam dışı hedefte ya da Operatör/Denetçi ile 302 access-denied (403 sayfası).

## Giriş akışı
- Doğrulama sunucuda: PIN modunda `RegistryNo` + `Pin`, parola modunda `RegistryNo` + `Password` zorunlu (alan hatası `Pin` ya da `Password` anahtarıyla). Başarısız POST'ta `Mode` ve `RegistryNo` korunur, parola ve PIN boşaltılır.
- Hata metinleri (`ErrorMessage`): PIN "Sicil no ya da PIN hatalı.", parola "Sicil no ya da parola hatalı.", ilk giriş eksikse "İlk girişinizi parola ile yapın.". Kilitte `ErrorMessage` boş, `LockedUntilLocal` Türkiye saatiyle dolu (metni görünüm kurar).
- PIN girişi yalnız tüm rolleri Operatör olan aktif hesapta. Uygun olmayan hesap, bilinmeyen sicil no ve pasif hesap aynı hata, PIN doğrulanmaz, hiçbir sayaç yazılmaz; doğrulama süresi sahte özetle eşitlenir (`ICredentialHasher.VerifyDecoy`).
- PIN modunda işlem PIN'i kilitli (`PinLockedAtUtc` dolu) uygun operatör PIN doğrulanmadan `SignInStatus.PinLocked` alır; `ErrorMessage` "PIN'iniz kilitli. Parola ile girin; PIN'i yöneticiniz sıfırlar.", `LockedUntilLocal` boş, iki sayaç değişmez, parola girişi açık (PL-104 B1). Uygun olmayan ya da bilinmeyen hesap yine "Sicil no ya da PIN hatalı.".
- Parola ya da PIN yanlışı `Users.LoginFailedCount`'u artırır (tek SQL güncellemesi, eşzamanlı denemede kayıp yok). Sınır `PinAttemptLimit` (5); sınırda `LoginLockedUntilUtc = şimdi + LoginLockMinutes`. Kilitliyken doğru parola ve PIN de kilit metnini alır, sayaç değişmez. Süre dolunca sayaç sıfırdan başlar, başarılı giriş sayacı sıfırlar. `PinFailedCount` ve `PinLockedAtUtc` hiç yazılmaz.
- Çıkış ve "Kullanıcı değiştir" `/account/login?mode=Pin`'e döner (`SessionControllerExtensions`). Başarılı girişte `vaka-login-mode` çerezi (`pin`/`password`, HttpOnly, Lax, 1 yıl) yazılır.
- Kilit açma: hedefte kilit ya da sayaç yoksa "Hesapta giriş kilidi yok."; yönetenin PIN'i en son doğrulanır (hatalı deneme yönetenin işlem sayacına yazılır). Ayrı olay kaydı yok (kapsam dışı: giriş denemelerinin denetim günlüğü).

## Dosyalar
`Services/`: `SignInResult.cs`, `ILoginAttemptGuard.cs`, `LoginAttemptGuard.cs`, `AccountService.cs`, `IAccountService.cs`, `ICredentialHasher.cs`, `CredentialHasher.cs`, `UserAdministration.cs`, `IUserAdministration.cs`, `UserDirectory.cs`, `SettingKeys.cs`; `Data/Seed/SettingSeed.cs` (tek satır, anahtar sabitle); `Controllers/`: `AccountController.cs`, `UsersController.cs`, `ReportsController.cs`, `SessionControllerExtensions.cs`; `ViewModels/Account/`, `ViewModels/Users/UserFormViewModel.cs`; `Program.cs` (guard kaydı).

## Kalem 5
E-30 `Durations` okunamayan süzgeçte boş süzgeç yerine bağlanan süzgeci kullanır (E-34 ile aynı); hata metni aynı, tesis, hat, makine seçili kalır.

## Ortam
Migration `20261116214722_AddLoginLockToUsers` gerçek `App_Data/vaka.db`'ye UYGULANMADI; bu kod gerçek DB'de kolonlar yokken çalışmaz.
Sınama kopya DB ile (`.qa/f6e/qa.db` kopyası); sınamada operatör PIN'ini ve yöneticinin PIN'ini kopyada elle eşitlemek gerekti (QA PIN'leri bilinmiyor).

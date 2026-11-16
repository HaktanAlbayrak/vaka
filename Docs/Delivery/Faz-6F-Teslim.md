# Faz 6F Teslim Raporu: PIN ile giriş ve giriş deneme sınırı

| Alan | Değer |
|---|---|
| Faz | 6F |
| Tarih | 2026-11-17 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `b21db41`; tur 2 kopyası `.qa/f6fqa/t2/src`, BE tur 2 düzeltmeleri dahil; Faz 6F kodu commit'lenmemiş, ön tur sonrası kilit metni düzeltmesi dahil). Kaynak test başında `.qa/f6fqa/src` altına kopyalandı, oradan derlendi. Devir notları `Docs/Handover/frontend-engineer-20261117-0055.md`, `Docs/Handover/backend-engineer-20261117-0110.md`; DB talebi `Docs/Db/DB-2026-010.md` |
| Ortam | LOCAL, Release build, port 5901 (Production), DB: `.qa/` kopyası. `.qa/f6fqa/qa.db` = `.qa/f6e/qa.db` dosyasının SQLite yedekleme API kopyası (DB-2026-009 uygulanmış); `20261116214722_AddLoginLockToUsers` yalnız bu kopyaya `dotnet ef database update --configuration Release --connection` ile uygulandı. Göç öncesi karşılaştırma kopyası `.qa/f6fqa/pre6f.db`. K12 karşılaştırması için Faz 6E yayını (`.qa/f6e/t2/build`) aynı DB'nin ayrı kopyasıyla port 5902'de kısa süre çalıştırıldı. `EventChain__HmacKey` user-secrets değerinden yalnız süreç ortamına verildi. Chromium (Playwright 1.61). Gerçek `App_Data/vaka.db`'ye ve PID 20552'ye dokunulmadı |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2; B1 ve B2 kapandı, açık bulgu yok. Tur 1: GEÇTİ, iki DÜŞÜK) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-17). Tur 1: İADE (PL-104). Planın son fazı (PL-105) |

**Test verisinin kurulumu.** Giriş durumları uygulamanın uç noktalarıyla kuruldu: sayfadaki form okunup kendi belirteciyle gönderildi (`.qa/f6fqa/j/t.py`). K8 JS kapalı yol, K11, K14 ve K13 menü taraması tarayıcıda yapıldı. DB'ye tek doğrudan yazım K5 içindir: plan gereği QA kopyasında `LoginLockMinutes` 15'ten 1'e indirildi, ölçümden sonra 15'e döndü.

- Süper admin `/users/create` ile "Çoklu Rol Deneme" (`2970`, Operatör + Yönetici) açtı. Yönetici `1001` "Yeni Operatör Deneme"yi (`2971`, yalnız Operatör) açtı. İkisi de geçici parolayla `/account/first-login`'den geçti.
- Yönetici `2001`'in PIN'ini E-04'ten sıfırladı (K7).
- Planlı kayıt 37'yi (KSA) `2002` açtı; görevliler `2006` ve `2971`. Acil kayıt 38'i de `2002` açtı. Makine 2'yi boşaltmak için Yönetici, Faz 6E'den kalan kayıt 36'yı iptal etti.

## 1. Kapsam

- DB 1: `Users.LoginFailedCount` (INTEGER NOT NULL DEFAULT 0), `Users.LoginLockedUntilUtc` (TEXT NULL), `SystemSettings` `LoginLockMinutes` = 15 (DB-2026-010, ALTER + Eklemeli, kullanıcı onaylı).
- BE 2: `ILoginAttemptGuard`/`LoginAttemptGuard` (giriş sayacı, süreli kilit, formül XML yorumunda); `IAccountService.SignInAsync` ve `SignInWithPinAsync`; `ICredentialHasher.VerifyDecoy`; `SignInResult`.
- BE 3: `AccountController.Login` mod yolu, `vaka-login-mode` çerezi; çıkış ve "Kullanıcı değiştir" `?mode=Pin`'e döner; `UsersController.UnlockLogin` (SK-9 kapsamı).
- FE 4: `Login.cshtml` "PIN ile" / "Parola ile" seçimi, PIN modunda sicil no + PIN, kilit metni, alttaki açıklama; E-04'te "Giriş kilitli" rozeti, sayaç satırı, "Giriş kilidini aç" paneli; `.login-mode` stilleri; `hash` ve `lock-open` ikonları.
- BE 5: E-30 okunamayan süzgeçte bağlanan alanlar korunur (`ReportsController.Durations`, tek satır).
- FE 6: `FocusFirst.js` `checkVisibility` yedek yolu (Faz 6E B4).

**Plandan sapmalar (karar analistin):**

- Kilit metni plandaki "Giriş HH:mm'e kadar kapalı" kalıbı yerine "Giriş HH:mm saatine kadar kapalı" yazıyor. Sebep Türkçe ek uyumu: "01:16'e" yanlış. E-04 metni de aynı biçimde. Gerçek kilitle görüldü: "Çok sayıda hatalı giriş. Giriş 01:33 saatine kadar kapalı; yöneticiniz erken açabilir." (`GirisKaniti.txt`, `GirisKilit360Koyu.png`).
- `Styles/BuildAssets.mjs` dosya sınırı dışında; ikon listesine `hash` ve `lock-open` eklendi (FE devir notunda yazılı).
- PIN modundaki alt açıklamaya plan metninden sonra "İlk girişinizi parola ile yapın." cümlesi eklendi.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Yalnız Operatör, ilk girişi tamam `2002` PIN modunda girer, İşlerim'e yönlenir | GEÇTİ | `GirisKaniti.txt` K1: 302 `/`, ardından 302 `/my-work`; `vaka-login-mode=pin` yazıldı; sayaç (0, -, 0, -). JS kapalı tarayıcıda da PIN girişi `/my-work`'e gitti |
| K2 | Yönetici, Denetçi, Süper admin ve Operatör+Yönetici PIN modunda giremez; metin "Sicil no ya da PIN hatalı."; `LoginFailedCount` ve `PinFailedCount` değişmez; parola ile girerler | GEÇTİ | `1001`, `3001` ve `0001` doğru PIN'le "Sicil no ya da PIN hatalı." aldı, sayaçlar önce ve sonra (0, -, 0, -). `2970` (Operatör + Yönetici) ilk giriş öncesinde ve sonrasında aynı metni aldı, sayaç değişmedi. Dördü de parola ile 302 aldı. Uygun olmayan hesaplarda yanlış PIN denemesi öncesi ve sonrası `Users` tablosu özeti aynı |
| K3 | Sınır kadar yanlış PIN: sayaç her denemede artar; sınırda kilit = son deneme + 15 dk; işlem PIN sayacı değişmez; kilitliyken doğru PIN ve parola reddedilir, metin bitiş saatini yazar | GEÇTİ | `2006`: sayaç 1, 2, 3, 4, 5. 5. denemede `LoginLockedUntilUtc` 22:33:18.475; son istek 22:18:18.374; fark 900,1 sn. `PinFailedCount` 0, `PinLockedAtUtc` boş. Kilitliyken doğru PIN ve doğru parola kilit metnini aldı, sayaç 5'te kaldı. Ekranda "01:33 saatine kadar" yazıyor (UTC+3 ile aynı). Bağımsızlık ters yönde de ölçüldü: Run ekranında 2 yanlış işlem PIN'i sonrası `2002` için `PinFailedCount` 2, `LoginFailedCount` 0 |
| K4 | Yönetici yanlış parolayla kilitlenir; Süper admin E-04'te PIN'le açar, yönetici hemen girer; yönetici kapsamı dışında düğme yok, elle POST 403 | GEÇTİ | `1001` 5 yanlış parolayla kilitlendi; kilitliyken doğru parola kilit metnini aldı. Süper admin `/users/edit/2`'de "Giriş kilitli" ve açma formunu gördü. Yanlış PIN'de alan hatası çıktı ve Süper admin'in işlem sayacı 1 oldu. Doğru PIN'le kilit açıldı (0, -); `1001` hemen 302 ile girdi. Kapsam: `1001` için `/users/edit/10` (kilitli başka Yönetici) ve `/users/edit/1` (Süper admin) 403 sayfasına gidiyor; aynı hedeflere elle POST da 403 sayfası, `1002` kilitli kaldı. `1001` kapsamındaki operatör `2006`'nın kilidini açtı ("Giriş kilidi açıldı."), `2006` hemen girdi (`GirisKaniti.txt` K3-K4) |
| K5 | Süre dolunca doğru bilgiyle giriş; başarılı giriş sayacı sıfırlar | GEÇTİ | `LoginLockMinutes` = 1 yapıldı. `2971` 5 yanlış PIN'le kilitlendi (bitiş 22:19:31); süre dolmadan doğru PIN kilit metni aldı. 66 sn sonra bir yanlış PIN denendi: süresi dolan kilit temizlendi ve sayaç sıfırdan 1 oldu. Ardından doğru PIN 302, sayaç (0, -). Ayar 15'e geri alındı |
| K6 | Olmayan sicil no ile metin yanlış parola ya da PIN ile aynı, hiçbir satır değişmez; pasif kullanıcı hiçbir modla giremez | GEÇTİ | `9999`: PIN modunda "Sicil no ya da PIN hatalı.", parola modunda "Sicil no ya da parola hatalı."; ikisi de gerçek kullanıcının yanlış PIN ve parola metniyle aynı. Pasif `2005` iki modda da aynı metni aldı. Bu denemelerden önce ve sonra `Users` tablosunun SHA-256 özeti aynı (`8daf750ab2ae1f15`). Yanıt süresi medyanı: olmayan sicil 71,4 ms, uygun olmayan hesap 68,6 ms, uygun hesapta yanlış PIN 73,5 ms, doğru PIN 72,8 ms |
| K7 | Yeni ya da PIN'i sıfırlanmış operatör PIN moduyla giremez, "İlk girişinizi parola ile yapın."; parola ile BR-55 ekranı | GEÇTİ | Yeni `2971` ve E-04'ten PIN'i sıfırlanan `2001` (`MustSetPin` 1, `PinHash` boş) "İlk girişinizi parola ile yapın." aldı; sayaç yazılmadı. `2971` ve `2970` parolayla girince 302 ile `/account/first-login`'e gitti; tamamlayınca 302 `/`. Seed'deki `2901` da aynı metni aldı |
| K8 | Mod seçimi JS kapalı çalışır; son seçim hatırlanır, çerez yoksa PIN; çıkış ve "Kullanıcı değiştir" PIN modunda boş alanlarla açar; `autocomplete="off"`, PIN `inputmode="numeric"` | GEÇTİ | JS kapalı tarayıcı: ilk açılış "PIN ile" seçili, alanlar boş. "Parola ile" düz GET ile `?mode=password` açıyor. Parola girişi `/dashboard`'a gitti. Çıkış `/account/login?reason=signed-out&mode=Pin`'e döndü, PIN modu, alanlar boş. Aynı tarayıcıda `mode` vermeden açılış parola modunu hatırladı. Çerez başlığı: `vaka-login-mode=password; max-age=31536000; path=/; samesite=lax; httponly`. "Kullanıcı değiştir" aynı `/account/logout` formu (`_SwitchUser.cshtml:24,33`). PIN modunda form, `RegistryNo` ve `Pin` `autocomplete="off"`; `Pin` `inputmode="numeric"`, `type="password"`; GET'te ikisi de boş, 120 hücrede aynı |
| K9 | PIN oturumu parola oturumuyla aynı rol ve `<OturumSüresi>` | GEÇTİ | `2002` PIN ve parola oturumunda yetki aynı: `/my-work` 200, `/records/create` 200, `/records` 200, `/users` 302 (403 sayfası), `/reports/durations` 302 (403 sayfası). Kimlik çerezi öznitelikleri aynı. Kod: iki yol da `AccountPrincipal.Create(result.State!)` ve `VakaCookieEvents.CreateProperties(...)` ile oturum açıyor (`AccountController.cs:85-88`); işlemsizlik süresi `OturumSüresi`'nden (`Program.cs:39-41`) |
| K10 | Migration yalnız iki `AddColumn` ve `LoginLockMinutes` = 15; mevcut satırlar 0 ve NULL; Guard onay istemi; onay DB kaydında | GEÇTİ | `Up()` (satır 12-34): `AddColumn<int> LoginFailedCount`, `AddColumn<DateTime> LoginLockedUntilUtc`, `InsertData SystemSettings`; `Drop*` yalnız `Down()`'da. Kopyada satır sayısı değişen tablo yalnız `__EFMigrationsHistory` (7 > 8) ve `SystemSettings` (6 > 7). 61 kullanıcı 0 ve NULL; `integrity_check` ok, FK hatası yok. `Guard.mjs`'e `dotnet ef database update` girdisi verildi, yanıt `permissionDecision: "ask"` ("ALTER onayı gerekli ... AddColumn"). `DB-2026-010.md` "Onay" satırı: 2026-11-17, "Onaylıyorum" (`K10Migration.txt`) |
| K11 | Giriş ve E-04 360-1440 açık ve koyu yatay kaydırmasız; hata yerinde; G12 kusuru 0; konsol 0 | GEÇTİ | 120 hücre (320, 360, 768, 1024, 1280, 1440 px; iki tema; 7 giriş ve 3 E-04 durumu): §3 eval beklenen değerlerde, `hScroll` false, taşma 0, G12 kusuru 0, konsol 0. Hata yerleri: istemci hatası alan altında ve odak `RegistryNo`'da; sunucu hatası ve kilit metni yöntem seçiminden sonra, alanlardan önce; odak bildirimde. E-04 açma hatası panel açık, alan altında "PIN hatalı.", odak `UnlockLogin-Pin`'de (`ResponsiveErisilebilirlik.txt`) |
| K12 | E-30 `from=abc&machineId=1` hata metni gösterir, tesis, hat, makine seçili; E-34 ve E-31 ile aynı; geçerli süzgeçte rapor değişmez | GEÇTİ | `/reports/durations`, `/reports/machine` ve `/reports/materials?mode=Usage` aynı sorguyla aynı sonucu veriyor: "Rapor hazırlanamadı. Süzgeç değerlerinden biri okunamadı.", tesis T1, hat H1, makine M01 seçili, tarih boş. Geçerli süzgeçte (`machineId=1`, süzgeçsiz, `kind=Planned&facilityId=1`) `main` içeriği Faz 6E yayınıyla bayt bayt aynı (`K12Rapor.txt`) |
| K13 | Son regresyon: her rolün menü ekranları 360 ve 1440, kaydırma yok, konsol 0, G10; uçtan uca planlı ve acil kayıt; detayda 13 soru, çizelge, "Doğrulandı"; makine raporu ve kullanım oranı; sunum kayıtlarının bütünlüğü | GEÇTİ | Menü taraması 5 hesapla (Operatör, Yönetici, Denetçi, Süper admin, Operatör+Yönetici), 38 menü bağlantısı x 2 genişlik = 76 ziyaret: hepsi 200, h1 1, `hScroll` false, taşma 0, konsol 0, dash 0. G10: Süper admin menüsünde yalnız `/users`; `/records`, `/dashboard`, `/my-work`, `/records/create`, `/machine-board`, üç rapor, `/facilities` ve kayıt detayı 403 sayfası. Planlı kayıt 37 (KSA, malzeme L2027-001, iki görevli): `2006` başlamadan katıldı ("Test Operatör 2 ve Ahmet Yılmaz birlikte adıma başladı"); `2971` adım sürerken katıldı ve ayrıldı (3 sn); mola 3 sn; iki fazda süre altı gerekçesi; Tamamlandı. Brüt 29, mola 3, net 26, efor 25 = 19 + 2 + 2 + 2. Acil kayıt 38 açıldı, ilk adım otomatik başladı, bitti. Kayıt 37 detayında 13 sorunun 13'ü yanıtlı (`RegresyonKaniti.txt`); zaman çizelgesinde katılma ve ayrılma satırları var; "Doğrulandı". `/reports/machine?machineId=2` kayıt 37'yi listeliyor; kullanım oranı ekranı açılıyor. Kopyadaki 38 kaydın hepsi "Doğrulandı" |
| K14 | `checkVisibility` silinmiş sayfada giriş ekranı konsol hatasız, Run'da yanlış PIN sonrası odak `pin-finish-N` | GEÇTİ | `Element.prototype.checkVisibility` silinmiş ve silinmemiş bağlamda sonuç aynı: giriş açılışında hata yok, boş gönderimde odak `RegistryNo`, sunucu hatasında odak bildirimde. Run'da (`/records/run/36`) "Adımı bitir" yanlış PIN sonrası odak `pin-finish-160`, alan görünümde. Konsol ve sayfa hatası 0 (`GirisKaniti.txt`, K14) |

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`.qa/f6fqa/src`). `dotnet publish -c Release -o .qa/f6fqa/build` başarılı |
| CSS | `npm run build:css` | Kopyada aynı Tailwind CLI ile derlendi; çıktı repodaki `wwwroot/css/app.css` ile bayt bayt aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme. Sihirli rol metni 0. `[Authorize]` ya da `[AllowAnonymous]` taşımayan controller 0. `Program.cs`'te `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun. Yeni dosyalar: `ILoginAttemptGuard.cs`, `LoginAttemptGuard.cs`, `SignInResult.cs`, `LoginMode.cs`. Kolonlar `LoginFailedCount`, `LoginLockedUntilUtc`; ayar anahtarı `LoginLockMinutes`, sabiti `SettingKeys.LoginLockMinutes`. CSS `.login-mode`, `.login-mode-item` kebab-case |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (`Login.cshtml`, `Users/Form.cshtml`, `FocusFirst.js`, `Styles/app.css` farkı). PIN alanı `autocomplete="off"`, `inputmode="numeric"`, `spellcheck="false"` |
| Mekanik eval | `UiDelivery.md §3` | E-01 ve E-04'te 120 hücre ve regresyonda 76 ziyaret beklenen değerlerde. `h1` 1, `skip` true, `imgNoAlt`, `imgNoSize`, `inputNoLabel`, `iconBtnNoName` 0. `emdash` false, font Hanken Grotesk, `hScroll` false, `extScripts` []. 44 px altı hedef yalnız E-04 rol onay kutuları (20x20); dokunma hedefleri onları saran etiket (292x74 px), Faz 1'den beri değişmedi. Konsol 0, AA altı kontrast 0 |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Yeni migration `20261116214722_AddLoginLockToUsers` (çalışma ağacında, izlenmiyor) `DbChanges.md` DB-2026-010'da kayıtlı. ALTER (`AddColumn`) için kayıtta kullanıcı onay satırı var. `Up()` içinde `Drop*` 0 |

Kod okuması (güvenlik, SOLID, §7, §8):

- **Sabit zamanlı karşılaştırma:** `CredentialHasher.Verify` Identity `PasswordHasher.VerifyHashedPassword` (PBKDF2) kullanıyor; özet karşılaştırması çerçeve içinde sabit zamanlı.
- **VerifyDecoy:** `AccountService.SignInWithPinAsync` olmayan, pasif ve uygun olmayan hesapta `_hasher.VerifyDecoy(pin)` çağırıyor; sahte özet süreç başında bir kez üretiliyor (`CredentialHasher.cs:12`). Süre ölçümü K6'da.
- **Atomik sayaç:** `LoginAttemptGuard.RegisterFailureAsync` artışı SQL'de `LoginFailedCount + 1` olarak `ExecuteUpdateAsync` ile yapıyor. Kilit ayrı ve koşullu bir güncellemeyle yazılıyor (`LoginFailedCount >= limit && LoginLockedUntilUtc == null`); eşzamanlı denemede ikinci kilit yazılmaz.
- **Formül yorumu:** kilit formülü `ILoginAttemptGuard.cs`'te ("Kilit bitişi = sınıra ulaşan hatalı denemenin sunucu anı + `LoginLockMinutes` dakika"; kilit açık = `LoginLockedUntilUtc > şimdi`) ve kodla uyuşuyor; K3'te ölçülen fark 900,1 sn. Yorumdaki "tek SQL güncellemesi" ifadesi koda uymuyor (B2).
- **Controller'lar:** `AccountController.Login` doğrulama, servis çağrısı ve çerezle sınırlı; iş kuralı serviste. `UsersController.UnlockLogin` servise devrediyor.
- **Satır sayıları:** `AccountService` 238, `UserAdministration` 332 satır; §2 bölünme adayı. PL-101 (3) gereği bulgu sayılmadı.
- **G8:** `Controllers/` ve `Views/` altında süre aritmetiği yok. Tek `TimeSpan` kullanımı çerez ömrü `TimeSpan.FromDays(365)` (`AccountController.cs:214`); süre hesabı değil.
- **§8:** değişen 27 kaynak dosyada emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager` adı, kalıp dil ve em/en-dash 0.

## 4. Yetki matrisi

Rollere karşılık: Viewer = Denetçi (3001), Editor = Operatör (2002), Admin = Yönetici (1001); Süper admin (0001) ayrı yazıldı. 403 = 302 `/account/access-denied` > 403 (projenin düzeni). Belirteçsiz POST 400 (`/account/login`, `/users/unlock-login/8`).

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/account/login?mode=pin\|password` | 200 | 302 ana sayfaya (oturum açık; kod okuması `AccountController.cs:33`) | 302 ana sayfaya | 302 ana sayfaya |
| POST `/account/login` (PIN modu) | Yalnız Operatör'ün tüm rolleri olan aktif hesap girer | ret, sayaç yazılmaz | 302 (giriş) | ret, sayaç yazılmaz; Süper admin ve Operatör+Yönetici aynı |
| POST `/account/login` (parola modu) | Her aktif hesap | 302 | 302 | 302; Süper admin 302 |
| GET `/users/edit/{id}` (operatör hedef) | 302 | 403 | 403 | 200, kilitliyse açma paneli; Süper admin 200 |
| GET `/users/edit/{id}` (Yönetici ya da Süper admin hedef) | 302 | 403 | 403 | 403 (SK-9); Süper admin 200 |
| POST `/users/unlock-login/{id}` | 302 | 403 | 403 | kapsam içinde 302 ve "Giriş kilidi açıldı."; kapsam dışında 403; Süper admin 302 |

G10: Süper admin yalnız kullanıcı ekranlarını görüyor. Bu fazın yeni POST'u `/users/unlock-login` plan gereği ManagerOrSuperAdmin; Süper admin'e açık olması K4'ün gereği.

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| E-01 PIN modu (boş, istemci hatası, yönetici reddi, ilk giriş uyarısı, kilit) | Temiz; 320 temiz | Temiz | Temiz | Temiz; 1280 temiz | Ölçülmedi | Temiz (tüm genişlikler) | Ölçülmedi (yeni hareket yok; `.login-mode-item` yalnız renk geçişi) | 320 temiz | İlk Tab "İçeriğe geç", sonra tema, "PIN ile", "Parola ile", sicil no, PIN, göz, "Giriş yap"; her durakta odak halkası |
| E-01 parola modu (boş, hata) | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | - | 320 temiz | Aynı sıra |
| E-04 kilitli (rozetler, kilit metni, açma paneli kapalı ve açık) | Temiz; iki rozet aynı satırda | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | - | 320 temiz | Halkasız durak 0; açma hatasında odak `UnlockLogin-Pin` |
| E-04 sayaç satırı | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | - | 320 temiz | - |
| Regresyon: her rolün menü ekranları (38 bağlantı) | Temiz | - | - | Temiz | - | - | - | - | - |

G11 imzası (FE devir notuyla aynı, 320-1440 px, iki tema):

- PIN modu: Yöntem:1@1 Sicil no:1@2 PIN:1@3 Giriş yap:1@4.
- Bildirimli: Yöntem:1@1 Bildirim:1@2 Sicil no:1@3 PIN (ya da Parola):1@4 Giriş yap:1@5.
- E-04 açma paneli: Açıklama:1@1 PIN:1@2 Düğme:1@3.

G12: "PIN ile" ve "Parola ile" üst kenar, yükseklik ve genişlik farkı 0. Yöntem, bildirim, alanlar ve düğme panel iç kenarlarına oturuyor. Etiket ve alan solu aynı. Göz düğmesi alanın içinde ve dikeyde ortalı.

Dört durum:

- Dolu: formlar.
- Hata: bildirim, kilit metni, alan hatası.
- Yükleniyor: gönderimde form `aria-busy="true"`, düğme pasif.
- Boş: form ekranı olduğu için yok.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN (kilit saati ve sayaç mono).
- Palet: UYGUN (seçili mod accent kenarlıklı; kilit bildirimi hata renginde; AA altı kontrast 0, iki tema).
- Köşe ölçeği: UYGUN (`.login-mode` ve öğeleri `rounded-ui` 6 px; kart ve panel 10 px; 2 px kenarlık, gölge yok).
- Gezinme arketipi: UYGUN (genel kabuk kipinde giriş ekranı: marka, bağlantı göstergesi, tema).
- Sayfa iskeleti: UYGUN (E-04 ana kolon + bağlam rayı; açma paneli rayda).
- Veri dili: UYGUN ("Giriş kilitli" rozeti ikon + metin + kenarlık; hata kutusu sorun + sonraki adım).
- Hareket imzası: UYGUN (yalnız 150 ms renk geçişi).
- İmza bileşen: UYGUN (değişmedi; regresyonda Yürütme ekranı temiz).
- Anti-slop taraması (`DesignDna.md §4`): temiz (ikonlar Lucide sprite'ından üretildi, em/en-dash 0).

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | DÜŞÜK | İşlem PIN'i kilitli operatör (`PinLockedAtUtc` dolu, `PinFailedCount` 5) aynı PIN'le giriş ekranından PIN moduyla girebiliyor. Plan bu durumu tanımlamıyor: PIN girişini engelleyen koşullar `MustChangePassword`, `MustSetPin` ve PIN yokluğu. İşlem PIN'i kilitli kişi girer, işlem yapamaz (BR-36); yönetici sıfırlayana kadar PIN girişi açık kalır | `GirisKaniti.txt` "İşlem PIN i kilitli 2004 PIN girişi 302 /", sayaç (0, -, 5, 2026-11-16 21:02:01); `Services/AccountService.cs` `SignInWithPinAsync` koşulları | analyst | Kabul edilirse kayıt altına alınır; edilmezse `PinLockedAtUtc` doluyken PIN girişi "İlk girişinizi parola ile yapın." ya da ayrı bir metinle reddedilir (backend-engineer) | KAPANDI (tur 2) |
| B2 | DÜŞÜK | `ILoginAttemptGuard` özeti "Sayaç ve kilit tek SQL güncellemesiyle yazılır" diyor. Kod artışı ve kilidi iki ayrı `ExecuteUpdateAsync` ile yazıyor. Davranış doğru (artış atomik, kilit koşullu), yorum kodla uyuşmuyor (§7, §8) | `Services/ILoginAttemptGuard.cs:5`, `Services/LoginAttemptGuard.cs` `RegisterFailureAsync` (iki güncelleme) | backend-engineer | Yorum "Artış tek SQL güncellemesiyle, kilit ayrı ve koşullu güncellemeyle yazılır" olarak düzeltilir | KAPANDI (tur 2) |

## 7. Geçenler

- K1-K14'ün hepsi.
- Rol bazında giriş matrisi: Operatör PIN ve parola ile girer. Yönetici, Denetçi, Süper admin ve Operatör+Yönetici PIN modunda reddediliyor ve sayaçları değişmiyor; parola ile giriyorlar.
- İlk giriş (yeni hesap, PIN sıfırlama), kilit oluşması, süre dolması ve kilitliyken doğru girişin reddi.
- İşlem PIN sayacının bağımsızlığı iki yönde DB sorgusuyla ölçüldü.
- Kilit açma yetki matrisi (Yönetici kapsamı, başka Yönetici, Süper admin hedefi); olmayan sicil no ve pasif hesapta aynı metin, satır değişmiyor.
- Çerez, çıkış ve "Kullanıcı değiştir" yönlendirmesi; JS kapalı mod seçimi.
- Son regresyon: 76 menü ziyareti, uçtan uca planlı ve acil kayıt, 13 soru, zaman çizelgesi ve 38 kaydın bütünlüğü; K12 karşılaştırması 6E ile bayt bayt aynı.
- G5 (belirteçsiz POST 400), G5a (giriş formu 1, E-04 6 form, her birinde belirteç 1), G6 (görünür metin ve seed'de dash 0), G7, G8, G10, G12. Sunucu günlüğünde istisna 0.

## 8. Kapsam dışı / yapılamayan

- 1920 px ve reduced-motion E-01 ile E-04'te ölçülmedi. Plan K11 360-1440 istiyor; ekranlara yeni hareket eklenmedi.
- Eşzamanlı yanlış giriş (yarış) yükü üretilmedi; atomiklik kod okumasıyla değerlendirildi (bölüm 3).
- K5'te süre 15 dakika beklenmedi; plan gereği QA kopyasında `LoginLockMinutes` 1'e indirildi.
- Test sonunda QA kopyasında kalanlar:
  - `2006` girişi kilitli: kontrast ölçümü için yeniden kilitlendi, 15 dakikada kendiliğinden açılır.
  - `2001` ilk girişini tamamlamadı.
  - `2970` ve `2971` hesapları eklendi; kayıt 36 iptal edildi.
  - Kaynak DB etkilenmedi.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| - | yok | - | Plan Faz 6F ile biter (O-14); tur 2 sonunda açık bulgu yok |

## 10. Analist kararı

```
FAZ 6F: İADE
Tarih: 2026-11-17
Gerekçe: K 14/14, son regresyon (K13) temiz. B1 bir kural boşluğu ve son faz olduğu için devredilemez; kararım reddetmek. Kendim okudum:
  AccountService.cs:49 PIN girişini yalnız MustChangePassword, MustSetPin ve PIN yokluğunda reddediyor, PinLockedAtUtc'ye bakmıyor;
  ILoginAttemptGuard.cs:5 "tek SQL güncellemesi" diyor.
B1 kararı: işlem PIN'i kilitli hesap PIN ile giremez. Gerekçe: BR-49'da kilit oturumu kapatır ve PIN'i yönetici sıfırlayana kadar geçersiz
  kılar; aynı PIN'in girişte kabul edilmesi kilidin amacına aykırı. Girilen oturum zaten hiçbir PIN'li işlem yapamaz, yalnız kafa karıştırır.
Sapmalar kabul: kilit metni "HH:mm saatine kadar" (Türkçe ek her saate uymuyordu; plan metni düzeltildi), BuildAssets.mjs'e iki ikon,
  PIN modu açıklamasına "İlk girişinizi parola ile yapın." cümlesi.
İade maddeleri (tur 2, tek tur, ikisi backend-engineer):
  B1: SignInWithPinAsync'te PinLockedAtUtc doluysa PIN doğrulanmadan reddedilir; giriş sayacı değişmez. Yeni durum SignInStatus.PinLocked,
    görünür metin "PIN'iniz kilitli. Parola ile girin; PIN'i yöneticiniz sıfırlar." (AccountController eşlemesi). Parola ile giriş açık kalır.
  B2: ILoginAttemptGuard.cs:5 özeti koda uyar: artış tek SQL güncellemesiyle, kilit ayrı ve koşullu güncellemeyle yazılır.
Yeniden denetim şartı (yalnız bunlar): işlem PIN'i kilitli operatör doğru ve yanlış PIN'le PIN moduyla giremez, metin yukarıdaki, LoginFailedCount
  ve PinFailedCount değişmez, parola ile girer; kilitsiz operatör PIN'le girer (K1), K3 ve K7 gerilemesi; yorum satırı kod okumasıyla;
  derleme ve konsol 0.
```

```
FAZ 6F: TESLİM ALINDI (tur 2)
Tarih: 2026-11-17
Gerekçe: K 14/14, son regresyon (K13) temiz; B1 ve B2 kapandı, açık bulgu yok. Kendim okudum: AccountService.cs:54-56 PinLocked kontrolü
  PIN doğrulamasından önce, AccountController.cs:77 metin eşlemesi; ILoginAttemptGuard.cs özeti iki güncellemeyi doğru anlatıyor.
  DB-2026-010 DOĞRULANDI; gerçek veritabanına teslimden önce, kullanıcının O-10 onayı ve yedekle uygulandı.
Açık kalem: yok. Faz 6F planın son fazıdır (O-14); proje kapanır.
```

## Tur 2 (2026-11-17)

**Kapsam.** Yalnız analistin yeniden denetim şartı:
- B1: işlem PIN'i kilitli operatör PIN moduyla giremez, metin doğru, sayaçlar değişmez, parola ile girer.
- B2: yorum satırı, kod okumasıyla.
- Gerileme: K1, K3, K7; yönetici ve olmayan sicil no.
- Metnin 360 ve 1440 px'te taşmasız görünmesi.
- Derleme, konsol ve değişen dosyalarda grep.

Analist üç sapmayı kabul etti (bölüm 10).

**Ortam.**
- Çalışma ağacı `.qa/f6fqa/t2/src`'ye kopyalandı. Değişen dört dosya (`Services/AccountService.cs`, `Controllers/AccountController.cs`, `Services/SignInResult.cs`, `Services/ILoginAttemptGuard.cs`) repodakiyle bayt bayt aynı.
- `dotnet build -c Release -warnaserror` 0 uyarı, 0 hata. Yayın `.qa/f6fqa/t2/build`, port 5911.
- DB `.qa/f6fqa/qa.db`'nin yeni kopyası `.qa/f6fqa/t2/qa.db`.
- Yalnız kendi PID'im kapatıldı, 5911 boş; 20552'ye dokunulmadı. Sunucu günlüğünde istisna 0.

**Sonuçlar.**

| Madde | Ölçüm | Sonuç | Kanıt |
|---|---|---|---|
| B1, kilidin kurulması | `2971` PIN ile girdi (kilitsiz, K1 gerilemesi temiz). `/account/security` "PIN değiştir" işlem formunda 5 kez yanlış mevcut PIN girdi: `PinFailedCount` 1'den 5'e çıktı, 5. denemede `PinLockedAtUtc` yazıldı ve oturum `/account/login?reason=pin-locked&mode=Pin`'e kapandı. Giriş sayacı 0 kaldı | Arayüzden kuruldu | `Tur2Kaniti.txt` B1 |
| B1, PIN modu | `2971` doğru ve yanlış PIN'le aynı metni aldı: "PIN'iniz kilitli. Parola ile girin; PIN'i yöneticiniz sıfırlar." Kilit saati görünmüyor. Sayaçlar önce ve sonra (0, -, 5, dolu); iki denemede `Users` tablosu özeti aynı. Faz 6E'den kilitli kalan `2004` de aynı metni aldı | KAPANDI | `Tur2Kaniti.txt` B1 |
| B1, parola | `2971` parola ile 302 aldı, giriş açık | KAPANDI | `Tur2Kaniti.txt` B1 |
| B1, kod sırası | `AccountService.SignInWithPinAsync` sırası: uygunluk (değilse `VerifyDecoy` + Invalid), `PasswordRequired`, `PinLocked`, deneme. `PinLocked` durumunda PIN doğrulanmıyor ve sayaç yazılmıyor. Eşleme `AccountController.cs:77`. Yeni durum `SignInStatus.PinLocked` | Uygun | kod okuması |
| Gerileme | K1: `2002` PIN ile 302. K3: `2002` 5 yanlış PIN ile kilitlendi (bitiş 22:55:27), kilitliyken doğru PIN ve parola kilit metni ("01:55 saatine kadar") aldı. K7: `2001` (PIN'i sıfırlanmış) ve `2901` "İlk girişinizi parola ile yapın." aldı. `1001` ve `9999` PIN modunda "Sicil no ya da PIN hatalı." aldı; `Users` tablosu değişmedi | GEÇTİ | `Tur2Kaniti.txt` gerileme |
| Arayüz | 320, 360 ve 1440 px'te açık ve koyu temada: metin tek bildirimde, panel iç kenarlarına oturuyor (sol ve sağ fark 0). İç taşma, sayfa kaydırması ve viewport dışı öğe 0. Odak bildirimde, PIN alanı boş, sicil no korunuyor. h1 1, dash yok, konsol 0 | GEÇTİ | `Tur2Kaniti.txt` arayüz |
| B2 | `ILoginAttemptGuard.cs` özeti: "Sayaç artışı tek atomik SQL güncellemesidir; kilit ayrı ve koşullu (sayaç sınırda ve kilit boş) bir güncellemeyle yazılır." `LoginAttemptGuard.RegisterFailureAsync`'teki iki `ExecuteUpdateAsync` ile uyuşuyor | KAPANDI | kod okuması |
| Grep | Değişen dört dosyada em/en-dash, `#region`, süs yorum ve emoji 0 | 0 ihlal | komut çıktısı boş |

**Not:** `PinLocked` yolunda `VerifyDecoy` çağrılmıyor. Yanıt metni zaten ayrı (planda karar verilmiş görünür metin), bu yüzden süre farkı ek bilgi sızdırmıyor; bulgu yazılmadı.

**Kanıt dosyaları.** `Evidence/Faz-6F/Tur2Kaniti.txt` eklendi; toplam 10 dosya. 360 px ekran görüntüsü `.qa/f6fqa/t2/pinkilitli-360-light.png` olarak `.qa/` altında kaldı.

**Tur 2 sonucu.** B1 ve B2 kapandı. Gerileme yok. Açık bulgu yok. QA önerisi: GEÇTİ.

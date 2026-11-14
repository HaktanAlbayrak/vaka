# Faz 1 Teslim Raporu: Giriş, PIN, kullanıcılar ve Süper admin

| Alan | Değer |
|---|---|
| Faz | 1 |
| Tarih | 2026-11-14 |
| Tur | 3 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `83c4415`; Faz 1 kodu, migration, `Dna.md` ve devir notları commit'lenmemiş). Tur 2: devir notları `frontend-engineer-20261114-2000.md`, `backend-engineer-20261114-2100.md`, `DB-2026-003`. Tur 3: `frontend-engineer-20261114-2200.md` (B7) |
| Ortam | LOCAL, Release build. Tur 1: `.qa/build`, port 5111 (Production) ve 5112 (Development), DB `.qa/qa.db`. Tur 2: `.qa/t2f1/build`, port 5121 (Production) ve 5122 (Development, yalnız prototip), DB `.qa/t2f1/qa2.db` (`App_Data/vaka.db` kopyası). Tur 3: `.qa/f1t3/build`, port 5131 (Production) ve 5132 (Development, yalnız prototip), DB `.qa/f1t3/qa3.db` |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 3: B7 kapandı, açık bulgu yok). Tur 2: KALDI. Tur 1: KALDI |
| Analist kararı | TESLİM ALINDI (tur 3, 2026-11-14). Tur 2: kullanıcı onaylı sınırlı ek tur (PL-31). Tur 1: İADE (2026-11-14) |

## 1. Kapsam

- DB: `Users`, `UserRoles` (tek Süper admin seed), `SystemSettings` (3 satır), `SettingChanges`; migration `AddUsersRolesAndSettings` (DB-2026-001, DB-2026-002).
- BE: ayar okuma servisi; cookie kimlik doğrulama ve son istek anına göre işlemsizlik; ilk giriş kapısı; PIN servisi (biçim, deneme sınırı, kilit); kullanıcı servisleri (SK-9 kapsamı, son aktif Süper admin, kendi hesabı kuralları); ana sayfa yönlendirmesi; `VakaClaimTypes`, `Roles.Label`; T-B15 genel antiforgery filtresi.
- FE: E-01 Giriş, E-02 Parola ve PIN (iki mod), E-03 Kullanıcılar, E-04 Kullanıcı formu (sıfırlama ve etkinleştirme), pasifleştirme onayı, 403 sayfası; üç kipli kabuk (tam, odak, genel), rol bazlı menü, "Kullanıcı değiştir" `POST /account/logout`; T-B16, T-B21.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Oturumsuz `/users` 302 `/account/login?ReturnUrl=%2Fusers` | GEÇTİ | `Evidence/Faz-1/KabulAkisi.txt` K1; `YetkiMatrisi.txt` anon kolonu |
| K2 | Seed tek Süper admin; ilk giriş kapısı; bitince `/` -> `/users` | GEÇTİ | `KabulAkisi.txt` başlangıç ve K2 (seed: Users 1 satır, rol `SuperAdmin`; `/`, `/users`, `/account/security`, `/users/create` 302 ilk girişe; sonra `/` 302 `/users`) |
| K3 | PIN biçim kuralları (3/5 hane, `12a4`, `1111`, `1234`, `4321` alan altında red; `1357`, `0482` kabul, `0482` sonraki doğrulamada geçer) | GEÇTİ | `KabulAkisi.txt` K3 (her red `NewPin-error` altında nedenli; reddedilen denemede yarım kayıt yok) |
| K4 | SA Yönetici açar, Yönetici Operatör/Denetçi açar; tekrar sicil red; ilk girişte belirleme; sicil no değişmez | GEÇTİ | `KabulAkisi.txt` K4 (`RegistryNo=9999` gönderildi, DB `3001` kaldı; alan `readonly`) |
| K5 | Operatör ve Denetçi tüm `/users` GET/POST'larında son yanıt 403; menüde Kullanıcılar yok | GEÇTİ | `YetkiMatrisi.txt` (op, au kolonları 302>403); `KabulAkisi.txt` K5 (hedef değişmedi; `href="/users"` 0) |
| K6 | Pasifleştirme/etkinleştirme yanlış PIN'le değişmez, doğru PIN'le değişir; pasif kullanıcı giremez | GEÇTİ | `KabulAkisi.txt` K6 (Yönetici ve Süper admin ayrı ayrı; oturumu açık pasif kişi `reason=inactive`) |
| K7 | 4 hata kilitlemez, 5. kilitler ve oturumu kapatır, neden gösterilir; doğru PIN sayacı sıfırlar; "PIN kilitli"; Yönetici Operatör'ü, SA Yönetici'yi sıfırlar; sonraki girişte yalnız PIN | GEÇTİ | `KabulAkisi.txt` K7 (DB `PinFailedCount` 4 -> 0 -> 5 kilitli; kullanıcı işlemi sırasında kilitlenme de denendi; Yönetici'nin Yönetici PIN sıfırlaması 403) |
| K8 | Son aktif SA pasifleştirilemez ve rolü kaldırılamaz; iki SA ile biri pasifleştirilir; kendi hesabını pasifleştiremez, kendi rollerini değiştiremez | GEÇTİ | `KabulAkisi.txt` K8 (iki SA ile pasifleştirme ve rol kaldırma kolu sıralı istekle denendi). Eşzamanlı istekle kural aşılıyor: B2 |
| K8a | Yönetici formunda Yönetici/SA rolü yok; değiştirilmiş istek red; Yönetici/SA hesabına her işlem 403, hesap değişmez; listede işlem bağlantısı yok | GEÇTİ | `KabulAkisi.txt` K8a; `YetkiMatrisi.txt` m1 kolonu (`/users/edit/2`, `/users/deactivate/2`, `/users/reset-pin/2` 403) |
| K8b | SA menüsünde yalnız Kullanıcılar ve hesap; hiçbir rolde var olmayan adrese bağlantı ya da bağlantısız kalem yok | GEÇTİ | `MekanikEval.txt` menü kolonu (SA: `/users /account/security`; diğer rollerde yalnız `/`, `/users`, `/account/security`; `aria-disabled` kalem 0) |
| K9 | Güvenlik ekranında eski PIN'le yeni PIN; eski yanlışsa değişmez ve sayılır | GEÇTİ | `KabulAkisi.txt` K9 (hash aynı, `PinFailedCount=1`) |
| K10 | Parola ve PIN DB'de düz metin değil; günlükte PIN yok | GEÇTİ | `KabulAkisi.txt` K10 (4 parola metni 0 eşleşme, hash V3 84 karakter; `run.log` 12 PIN değeri 0 eşleşme) |
| K11 | Ad ve sicil 360/768/1440'ta görünür; "Kullanıcı değiştir" tek dokunuşla girişe götürür | GEÇTİ | `MekanikEval.txt` (tüm oturumlu hücrelerde kişi + sicil; 360: 92x44, 768: 118x44, lg+: rayda 222x44); `KlavyeTuru.txt` "switch 360" |
| K11a | Oturum düştükten sonra "Kullanıcı değiştir" 400 değil giriş ekranı | GEÇTİ | `KlavyeTuru.txt` son satır (tarayıcı, 70 sn işlemsizlik, 302 -> `/account/login?reason=signed-out`); `KabulAkisi.txt` K11a (curl) |
| K12 | Oturum son istekten `<OturumSüresi>` sonra kapanır; kısa aralıklar açık tutar; PIN değerleri tablodan; kaynakta sabit yok | GEÇTİ | `KabulAkisi.txt` K12 (2 dk: 4 x 50 sn açık, 125 sn sonra `reason=expired`; PinLength 5 ve PinAttemptLimit 2 sonraki istekte uygulandı; satır yokken açık hata, varsayılan uydurulmadı) |
| K12a | Ayar tablosu tam 3 satır (30, 4, 5), geçmiş boş, 0 ve negatif DB kısıtıyla red | GEÇTİ | `KabulAkisi.txt` başlangıç ve K12a (`CK_SystemSettings_Value`) |
| K13 | Operatör+Yönetici iki menüyü görür ve `/users`'a erişir; yalnız Operatör görmez | GEÇTİ | `YetkiMatrisi.txt` mo kolonu; `MekanikEval.txt` mo satırları |
| K14 | Prototip kabuğu gerçek kabukla değişti; prototip sayfaları Development'ta açılır | GEÇTİ | `KabulAkisi.txt` K14/TK2 (Development 5 sayfa 200; Production 404; oturumlu kişi gerçek kabuğu görür) |
| T-B15 | Genel antiforgery filtresi; bu fazın POST'ları token'sız 400; prototip action'ları `[HttpGet]` | GEÇTİ | `Program.cs:17`; `PrototypeController.cs` beş action `[HttpGet]`; `YetkiMatrisi.txt` tokensiz kolonlar (yetkili rolde 400; `/account/logout` bilinçli istisna, K11a) |
| T-B16 | Production oturumsuz hata ve 403 sayfalarında demo kişi, sicil, `/prototype` yok | GEÇTİ | `KabulAkisi.txt` T-B16; `MekanikEval.txt` (oturumsuz 105 hücrede kişi alanı 0, prototype bağlantısı 0) |
| T-B21 | `Dna.md` veri dili satırındaki kart sütun sayısı ve satır yüksekliği uygulamayla aynı; E-03 bu satıra göre | KALDI | Satır yüksekliği ve 1280 eşiği uyuyor (1440: tablo, en kısa satır 56 px), kart sütunları uymuyor: B1 |
| G1 | `-warnaserror` derleme 0/0; `build:css` başarılı | GEÇTİ | Bölüm 3 |
| G2 | §4 grep'leri 0; her controller `[Authorize]`/`[AllowAnonymous]`; rol metni yalnız `Roles` | GEÇTİ | Bölüm 3; rol metni istisnası B5 (DÜŞÜK) |
| G3 | §2 matrisi kırık hücresiz, §4 sıfır ihlal, konsol 0 hata, DNA'dan sapma yok | KALDI | Matris, grep ve konsol temiz (`MekanikEval.txt`); DNA veri dili sapması B1 |
| G4 | Operatör ekranları 360'ta yatay kaydırmasız, birincil düğmeler 44 px, durumlar metinle | GEÇTİ | `MekanikEval.txt` (hScroll 0, smallTargets 0); ek ölçüm: 360 ve 1440'ta 44 px altı etkileşimli öğe 0; rozetler ikon + metin |
| G5 | Token'sız POST 400 | GEÇTİ | `YetkiMatrisi.txt` (oturumlu ve yetkili tüm POST'lar 400; oturumsuz ve yetkisiz rolde yetki denetimi önce çalışır: 302) |
| G6 | Görünür metin, seed, doküman em/en-dash 0 | GEÇTİ | Kaynak ve Docs taraması 0; 450 hücrede `emdash` false |
| G7 | `Up()` içinde Drop yok; ALTER onayı | GEÇTİ | `20261114121621_AddUsersRolesAndSettings.cs` `Up()` Drop 0, AlterColumn/AddColumn/Rename 0; sınıf Eklemeli |

G8, G9 Faz 4 ve sonrası, G10 Faz 2 ve sonrası: bu fazda uygulanmaz.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental` ile 0 uyarı, 0 hata |
| CSS | `npm run build:css` | Başarılı; çıktı depodaki `wwwroot/css/app.css` ile bayt bayt aynı (`cmp`) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme (policy grep 0, sihirli `[Authorize(Roles = "` 0); 5 controller'ın 5'i sınıf düzeyinde `[Authorize(Roles = Roles.X)]` ya da `[AllowAnonymous]`; MediatR, AutoMapper, generic repository 0; `new ...Service(` ve `GetService` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: tablolar `Users`, `UserRoles`, `SystemSettings`, `SettingChanges`; kolonlar `RegistryNo`, `PinFailedCount`, `PinLockedAtUtc`, `ChangedByUserId`; dosyalar `UsersController.cs`, `FirstLoginGateFilter.cs`, `UnsavedGuard.js`; arayüzler `I` önekli; asenkron servis metotları `Async` sonekli; özel alanlar `_camelCase` |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (`Views/**`, `Styles/app.css`, `wwwroot/js/**`: transition-all, outline-none, img, user-scalable, onclick, CDN, dash, h-screen, `...`, jquery/gsap/aos, ham hex, slate/gray/zinc 0; tek animasyon `prefers-reduced-motion` ile kapanıyor; çok satırlı `<input>` taraması: Faz 1 görünümlerinde autocomplete/inputmode eksik 0) |
| Mekanik eval | `UiDelivery.md §3` | 450 ölçüm (30 sayfa/durum x 15 hücre): h1 1, skip var, imgNoAlt/imgNoSize/inputNoLabel/iconBtnNoName 0, emdash yok, hScroll yok, smallTargets 0, dış kaynak 0, font Hanken Grotesk (`document.fonts.check` true), üst bar 64 px, mobil gövde 16 px, en düşük metin kontrastı 5,28:1. `overflowing` yalnız 360/320 px kart görünümünde görünmez `sr-only` başlık satırı (yanlış pozitif). `Evidence/Faz-1/MekanikEval.txt` |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Tek migration `AddUsersRolesAndSettings`, DbChanges'te DB-2026-001 ve 002 satırlarında kayıtlı; `Up()` Drop 0; `Program.cs` içinde `Migrate()` 0 |

Ek: §7 formül yorumu `Services/IDurationCalculator.cs:5-13`, `Services/DurationCalculator.cs:5-11` ve `Authorization/VakaCookieEvents.cs:33-38`'de var ve kodla uyuşuyor (`now - lastRequestAt >= TimeSpan.FromMinutes(sessionMinutes)`). §8 taraması: emoji 0, `#region` 0, süs yorum 0, `Helper`/`Utils`/`Manager` adı 0, kalıp sözcük 0; altı satır yorumun altısı da "neden" anlatıyor.

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Operatör ve Denetçi (iki rol ayrı ayrı ölçüldü, sonuçlar aynı), Editor = Yönetici ve Yönetici+Operatör (ayrı ayrı ölçüldü, aynı), Admin = Süper admin. Değer: ilk yanıt > yönlendirme izlenince son yanıt. POST satırları token'lı ve durum değiştirmeyen geçersiz veriyle; token'sız sonuç parantezde. Hedef 5 = Operatör hesabı, 2 = Yönetici hesabı. Ayrıntı ve altı kolonlu tam tablo: `Evidence/Faz-1/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/` | 302>200 (giriş) | 200 | 200 | 302>200 (`/users`) |
| GET `/account/login` | 200 | 302>200 (`/`) | 302>200 | 302>200 |
| GET `/account/access-denied` | 403 | 403 | 403 | 403 |
| GET `/account/first-login` | 302 (giriş) | 302>200 (`/`, ilk giriş bitmiş) | 302>200 | 302>200 |
| GET `/account/security` | 302 (giriş) | 200 | 200 | 200 |
| GET `/users`, `/users?status=All`, `/users/create` | 302 (giriş) | 302>403 | 200 | 200 |
| GET `/users/edit/5`, `/users/deactivate/5` | 302 (giriş) | 302>403 | 200 | 200 |
| GET `/users/edit/2`, `/users/deactivate/2` | 302 (giriş) | 302>403 | 302>403 (SK-9) | 200 |
| GET `/users/edit/999` | 302 (giriş) | 302>403 | 404 | 404 |
| GET `/error/404`, `/error/403` | 404, 403 | 404, 403 | 404, 403 | 404, 403 |
| GET `/prototype/run` (Production) | 404 | 404 | 404 | 404 |
| POST `/account/login` | 400 token'sız | 200 (400) | 200 (400) | 200 (400) |
| POST `/account/first-login` | 302 (302) | 302>200 (400) | 302>200 (400) | 302>200 (400) |
| POST `/account/change-password`, `/account/change-pin` | 302 (302) | 200 (400) | 200 (400) | 200 (400) |
| POST `/users/create`, `/users/edit/5`, `/users/deactivate/5`, `/users/activate/5`, `/users/reset-pin/5`, `/users/reset-password/5` | 302 (302) | 302>403 (302>403) | 200 (400) | 200 (400) |
| POST `/users/edit/2`, `/users/deactivate/2`, `/users/reset-pin/2` | 302 (302) | 302>403 (302>403) | 302>403 (400) | 200 (400) |
| POST `/account/logout` (bilinçli istisna, K11a) | 302 (302) | 302 (302) | 302 (302) | 302 (302) |

Menüde gizli olup URL ile açılan sayfa yok: Operatör ve Denetçi `/users` adreslerinin tümünde 403, Yönetici Yönetici/SA hesaplarında 403. Matristeki hiçbir istek hedef hesapları değiştirmedi (DB kontrolü dosyanın sonunda).

## 5. Responsive ve görsel tur

Her hücrede §3 eval ve AA kontrast ölçümü çalıştı. Hücreler: 360x800, 768x1024, 1024x768, 1440x900, 1920x1080 (açık ve koyu), 800x360 mobil yatay, %200 yakınlaştırma karşılığı 320x640 (açık) ve 720x450 (koyu), reduced-motion 390x844 (açık) ve 1440x900 (koyu). "Ölçüm temiz" = §3 beklenen değerler ve AA kontrast. Ekran görüntüsü yalnız B1 hücreleri ve görsel DNA kontrolü için alındı (`.qa/f1/shots/`, 18 dosya, hepsi `Read` ile açıldı).

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/account/login` (boş, `reason=expired`, `reason=pin-locked`, yanlış parola) | Temiz | Temiz | Temiz | Temiz | Temiz (içerik 576 px) | Temiz | Animasyon 0 | Temiz | Atlama bağlantısı ilk odak; boş gönderimde iki satır içi hata ve ilk alana odak; yanlış parolada uyarı kutusuna odak, parola alanı boşaltılıyor |
| `/account/first-login` parola + PIN | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Odak halkası 3 px; "Kullanıcı değiştir" her genişlikte üst barda (odak kipi) |
| `/account/first-login` yalnız PIN | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | PIN `maxlength` 4 ve 4 hane göstergesi |
| `/account/security` | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Yeni PIN `maxlength` = PinLength, mevcut PIN sınırsız |
| `/` (Operatör, Yönetici, Yönetici+Operatör) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Çekmece: Enter ile açılır, `aria-expanded` true, ana içerik `inert`, Tab çekmece içinde döner, Esc kapatır ve odak "Menü"ye döner |
| `/users` dolu (29 kayıt, 2 sayfa), sayfa 2 | Kart, B1 | Kart, B1 | Kart, B1 | Tablo, satır 56 px | Tablo | Temiz | Temiz | Kart, B1 | Süzgeç ve sayfa bağlantısı iskeleti açıyor (`Yükleniyor…`) |
| `/users` süzgeçli boş, pasif süzgeci, uzun ad (108 karakter) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Boş durum kesik çizgili kutu ve "Filtreleri temizle"; uzun ad satır kırıyor |
| `/users/create` (boş, sunucu hatası) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Boş gönderimde 3 satır içi hata, ilk alana odak; sunucu hatasında özet + alan hataları, ilk hatalı alana odak, geçici parola alanı boş |
| `/users/edit/{id}` (rol kilitli, pasif, Operatör) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Kaydedilmemiş değişiklikle menüye tıklayınca `beforeunload` uyarısı, vazgeçince alan korunuyor; sıfırlama bölümleri `details` |
| `/users/deactivate/{id}` (normal, engelli) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Boş PIN gönderiminde PIN alanına odak |
| `/account/access-denied`, `/users` 403 (oturumlu ve oturumsuz) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Oturumsuz genel kipte kişi alanı yok |
| `/error/404`, `/error/403`, `/error` | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Yapışkan üst bar odağı örtmüyor (390 px'te 40 Tab, örtme 0, halkasız odak 0) |

Konsol: her yeni sayfada tek tür kayıt, belge durum kodu (`/account/access-denied` 403, `/error/403` 403, `/error/404` 404, `/error` 500; Faz T'de de beklenen sayıldı). Başka hata ve uyarı 0; 4xx/5xx alt kaynak 0. Çevrimdışı: bağlantı şeridi açılıyor, `data-requires-online` düğmeler 3/3 devre dışı, bağlantı gelince açılıyor (B6 istisnası). Koyu tema en düşük kontrast 5,28:1.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Gövde Hanken Grotesk, başlık Archivo, sicil no JetBrains Mono; `document.fonts.check` 450 hücrede true; ğ ş İ ı doğru (görüntüler).
- Palet: UYGUN. Tek mavi accent, başarı/uyarı/hata ayrı; açık ve koyu temada AA altı metin 0.
- Köşe ölçeği: UYGUN. 2 px köşe, 2 px kenarlık, gölge ve gradient yok.
- Gezinme arketipi: UYGUN. lg ve üstü daraltılabilir ray (altında kişi, sicil, rol, "Kullanıcı değiştir" 222x44); lg altı alt çubuk + çekmece; üst bar tek satır 64 px; lg altında "Kullanıcı değiştir" üst barda çekmece açılmadan, metinli, 44 px (TK6/PL-20 korunuyor); odak kipinde her genişlikte görünüyor.
- Sayfa iskeleti: UYGUN. İlk giriş, kullanıcı formu, pasifleştirme ve ana sayfada ana kolon + sağ bağlam rayı, lg altında ray alta iniyor.
- Veri dili: SAPMA. Tablo kısmı uygun (2 px başlık çizgisi, yapışkan başlık, zebra yok, 1280 ve üstü tablo, satır en az 56 px, 1279 ve altı kart). Kart sütunları kartla aynı değil: 768 altında iki, 768-1279 arasında dört sütun yazılı, uygulamada beş alandan dördü tam satır kaplıyor (B1). Rozet tek biçim (ikon + metin + 2 px kenarlık), boş durum kesik çizgili kutu + eylem, hata düz hata kenarlıklı kutu + sonraki adım, yükleniyor durağan iskelet + "Yükleniyor…": uygun.
- Hareket imzası: UYGUN. Yalnız renk geçişi; reduced-motion hücrelerinde çalışan animasyon 0.
- İmza bileşen: bu fazda kapsam dışı (Faz 4-5).
- Anti-slop (`DesignDna.md §4`): temiz. Gradient, glow, cam yüzey, dört eşit KPI, iç içe kart, jenerik karşılama, sürüm rozeti, emoji, CDN yok. Büyük harfli küçük etiketler yalnız ray grup başlıkları ve kart alan etiketleri; bölüm üstü eyebrow yok.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | YÜKSEK | T-B21 karşılanmıyor: E-03 kart görünümü `Dna.md` veri dili satırıyla aynı değil. Kart "768 px altında iki, 768-1279 px arasında dört sütunlu ızgara" diyor; ızgara tanımlı (`Styles/app.css:547`) ama `Views/Users/Index.cshtml:102,103,111,124` Ad soyad, Roller, Durum ve İşlem hücrelerine `stack-wide` (tam satır) veriyor. Sonuç: her genişlikte tek sütunluk dikey liste, 1024 px'te kart başına 268-286 px yükseklik, kartın sağ dörtte üçü boş. Devir notu "veri dili satırı uygulamayla aynı" diyor | `Evidence/Faz-1/MekanikEval.txt` son bölüm (1024: grid 4 sütun, hücre genişlikleri 163, 320, 700, 700, 700); `KullanicilarKart-1024.png`, `KullanicilarKart-360.png` | frontend-engineer | Roller, Durum ve İşlem hücrelerinden `stack-wide`'ı kaldırıp ızgaraya yerleştirmek (uzun alan Ad soyad 2-3 sütun kaplayabilir) ya da kart satırını değiştirmek; ikincisi kimlik değişikliği olduğu için analist kararı ister (`DesignDna.md §5.5`) | KAPANDI (tur 2): 768-1279 px'te Ad soyad 2 sütun, diğerleri birer sütun, tam satır hücre yok; T-B21 geçti. 360 px'teki yerleşim kalıntısı B7 |
| B2 | ORTA | Son aktif Süper admin kuralı eşzamanlı istekte aşılıyor (PL-05). İki Süper admin aynı anda birbirini pasifleştirince ikisi de 302 alıyor ve aktif Süper admin sayısı 0 oluyor; 3 denemenin 3'ünde. Uygulama içinden geri dönüş yolu yok (Süper admin kurtarma kapsam dışı). Kontrol (`UserAdministration.cs:150`) ile yazma (`:167-168`) arasında PIN doğrulaması var ve kilit ya da yeniden denetim yok; rol kaldırma kolu (`:113-137`) aynı okuma-sonra-yazma biçiminde. Sıralı istekte kural tutuyor (K8 geçti) | `Evidence/Faz-1/SuperAdminYarisi.txt` | backend-engineer | Denetim ve yazmayı tek yazma kilitli işlemde yapmak (SQLite'ta işlemi `BEGIN IMMEDIATE` ile açıp sayımı işlem içinde yeniden yapmak) ya da kayıttan sonra aynı işlemde aktif Süper admin sayısını sayıp 0 ise geri almak | KAPANDI (tur 2): 12 eşzamanlı denemenin 12'sinde sonda tek aktif Süper admin, 500 yok |
| B3 | DÜŞÜK | Yönetici bir kullanıcının adını ya da rollerini değiştirince o kullanıcının açık formundaki antiforgery token'ı geçersizleşiyor; sonraki POST 400 ve genel "Bir hata oluştu" sayfası. Kimlikte kalıcı tekil claim (`NameIdentifier`) olmadığı için token tüm claim'lerin özetine bağlanıyor (`Authorization/AccountPrincipal.cs:11-16`) | `Evidence/Faz-1/KabulAkisi.txt` "Ek gözlemler" B3 | backend-engineer | `AccountPrincipal.Create` içine `ClaimTypes.NameIdentifier` = kullanıcı kimliği eklemek; antiforgery bu claim'e bağlanır, ad ve rol değişiminden etkilenmez | KAPANDI (tur 2) |
| B4 | DÜŞÜK | Production günlüğünde her SQL komutu Information düzeyinde: test boyunca 1771 "Executed DbCommand" kaydı, 14311 satır. Parametreler gizli, sızıntı yok; günlük gürültüsü | `Evidence/Faz-1/KabulAkisi.txt` "Ek gözlemler" B4; `appsettings.json:3` | backend-engineer | `Logging:LogLevel:Microsoft.EntityFrameworkCore.Database.Command` = `Warning` | KAPANDI (tur 2) |
| B5 | DÜŞÜK | Rol adları `Roles.cs` dışında metin olarak yazılmış: `Data/Configurations/UserRoleConfiguration.cs:12-14` CHECK ifadesi `'Operator', 'Manager', 'Auditor', 'SuperAdmin'` (TK9 kuralı: projede `Roles.cs` dışında rol adı metni geçmez). Üretilmiş migration dosyasındaki metin istisna sayılabilir | `UserRoleConfiguration.cs:12-14` | dbadmin (talep backend-engineer üzerinden) | CHECK ifadesini `Roles` sabitlerinden kurmak; üretilen SQL aynı kalır, yeni migration gerekmez | KAPANDI (tur 2) |
| B6 | DÜŞÜK | Çevrimdışıyken "Kullanıcı değiştir" (`POST /account/logout`) düğmeleri etkin kalıyor, diğer POST düğmeleri kapanıyor. Devir notu "tüm POST düğmeleri `data-requires-online`" diyor. Paylaşılan cihazda bağlantı yokken basılırsa tarayıcı hata sayfası açılır, oturum kapanmaz (PL-20 riski) | `Evidence/Faz-1/KlavyeTuru.txt` "offline" satırı (`postBtnsWithoutAttr=Kullanıcı değiştir` x3); `Views/Shared/_SwitchUser.cshtml:25,34` | frontend-engineer | Çevrimdışı davranışını analistle netleştirmek: düğmeyi kapatıp nedenini göstermek ya da açık bırakıp şeritte "çıkış da yapılamaz" demek; devir notunu ölçümle hizalamak | KAPANDI (tur 2) |
| B7 | ORTA | (tur 2) 768 px altında kullanıcı kartı yerleşiminde boşluk; tur 2 şartı 1'in 360 kısmı ("Ad soyad dışındaki dört hücre ikişer yan yana") karşılanmıyor. Ad soyad 2 sütun kapladığı ve Sicil no'dan sonra geldiği için ızgara onu 2. satıra itiyor: Sicil no 1. satırda tek (sağ yarı boş), Roller ve Durum 3. satırda yan yana, İşlem 4. satırda tek. Kart dört satır, 360'ta 234-285 px (tur 1: 286). 320 (%200) aynı. 768 ve üstü doğru | `Evidence/Faz-1/MekanikEval.txt` "Tur 2" (360x800 ve 320x640 satırları: `Sicil no:1@1 Ad soyad:2@2 Roller:1@3 Durum:1@3 İşlem:1@4`); `Evidence/Faz-1/KullanicilarKart-360.png`; `Views/Users/Index.cshtml:101-102` hücre sırası, `Styles/app.css:546-548, 559-561` | frontend-engineer | 768 altında Ad soyad'ı kartın ilk satırına almak (yalnız `max-md` kırılımında `order`) ya da ızgaraya `grid-auto-flow: dense` vermek; sonuç 360'ta üç satır (Ad soyad / Sicil + Roller / Durum + İşlem). 768-1279 yerleşimi değişmemeli | KAPANDI (tur 3): 320-767 px'te `Ad soyad:2@1 Sicil no:1@2 Roller:1@2 Durum:1@3 İşlem:1@3`, boş hücre 0; 768+ tur 2 ile aynı (bölüm Tur 3) |

## 7. Geçenler

- K1-K14 (K8a, K8b, K11a, K12a dahil) çalışan uygulamada bağımsız olarak geçti; K7 hem PIN değiştirmede hem kullanıcı işlemi sırasında kilitlenmeyle, K12 kopyada 2 dakikayla, K11a curl ve tarayıcıda denendi.
- K8 rol kaldırma kolu iki Süper adminle sıralı denendi: iki aktif varken kaldırma geçiyor, Yönetici'ye düşen eski Süper admin diğerine dokunamıyor (403), tek kalan kendi rolünü kaldıramıyor. "Son aktif Süper admin" mesajı sıralı istekte tetiklenemiyor, çünkü tek aktif Süper admin'i hedefleyebilecek tek kişi kendisi.
- Yetki matrisi: menüde gizli olup URL'den açılan sayfa yok; SK-9 kapsamı GET ve POST'ta 403; yetkisiz isteklerin hiçbiri veri değiştirmedi; G5 yetkili rollerde 400.
- DB: tek migration kayıtlı, `Up()` Drop 0, `Migrate()` yok; parola ve PIN V3 hash; ayar kısıtı çalışıyor; ayar satırı yoksa varsayılan uydurulmuyor.
- Kod: §4 grep'leri 0, beş controller'ın beşi yetki özniteliği taşıyor, controller'lar ince (model bağlama, servis çağrısı, sonuç eşleme), servisler arayüzle ve constructor injection ile geliyor, yasak mimari yok. §7 işlemsizlik formülü metodun üstünde ve kodla aynı. §8 taraması temiz. Not: `Services/UserAdministration.cs` 308 satır (`CodingStandards.md §2` "200 satırı geçen servis bölünmeye adaydır"; ihlal değil, sonraki fazlarda büyürse bölünmeli).
- UI: 450 hücrede §3 değerleri temiz, §4 grep 0, konsolda yalnız beklenen belge durum kodları; dört durum (yükleniyor iskeleti, süzgeçli boş, hata sayfası, dolu ve uzun ad) liste ekranında, form ekranlarında alan hatası + özet + ilk hatalı alana odak; T-B16 Production'da temiz; kabuk kipleri (tam, odak, genel) kartla aynı.
- Güvenlik ek kontrolleri: çerez `httponly`, `samesite=lax`, kalıcı değil; `ReturnUrl=//dış-alan` yerel adrese düşüyor; pasif ve hatalı parola aynı mesaj.
- Gerileme: Faz T prototip sayfaları Development'ta 200, Production'da 404.

## 8. Kapsam dışı / yapılamayan

- `/users/deactivate/{id}` "açık iş" durumu: `NoOpenWorkProvider` bu fazda boş döndüğü için uygulamada üretilemedi; yalnız "engelli" (kendi hesabı) ve normal durum ölçüldü. Faz 4'te yeniden ölçülmeli.
- `/users` süzgeçsiz boş durumu ("Henüz kullanıcı yok"): listeyi açan kişi her zaman etkin listede olduğu için üretilemedi; süzgeçli boş ölçüldü.
- K12'nin 30 dakikalık seed değeri gerçek süreyle beklenmedi; mekanizma kopyada 2 ve 1 dakikayla doğrulandı, seed değeri tabloda 30.
- B2'nin rol kaldırma kolu eşzamanlı olarak ayrıca üretilemedi: rolü değişen tarafın isteği antiforgery nedeniyle 400 aldı (B3). Kod aynı okuma-sonra-yazma biçiminde.
- Test sırasında QA dışı bir süreç (`dotnet run`, PID 2644 ve 14080, 15:48'de başlamış) `Src/Vaka.Web/App_Data/vaka.db` dosyasını açtı ve yanında `-wal`/`-shm` dosyaları oluştu. QA bu dosyaya yazmadı ve bu süreçlere dokunmadı; `vaka.db` md5 değeri test başı ve sonunda aynı (`615db04b...`). Yayınlanan yapıdan sonra `Src/` altında kaynak dosya değişmedi.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| 1 | Pasifleştirme ekranının "açık iş" durumu (liste, bağlantı yalnız görüntüleyenin erişebildiği ekrana) çalışan uygulamada ölçülecek | qa-tester | `IOpenWorkProvider` Faz 4, 5, 7'de dolacak (PhasePlan Faz 1 kalem 7) |
| 2 | BR-48 "PIN doğrulama ve iş eylemi aynı işlemde" | backend-engineer | Devir notunda Faz 4'e bırakıldı (olay günlüğüyle) |

## 10. Analist kararı

```
FAZ 1: İADE
Tarih: 2026-11-14
Gerekçe: 28 kriterin 26'sı kanıtla geçti; T-B21 ve G3, B1 (YÜKSEK) nedeniyle karşılanmıyor. B2 (ORTA) son aktif Süper admin
kuralını eşzamanlı istekte aşıyor ve uygulama içinden geri dönüşü yok; ORTA aynı fazda kapanır. B3-B6 küçük ve aynı sahiplere
döndüğü için bu turda kapanır (Decisions.md PL-28, PL-29, PL-30).
İade maddeleri: B1, B6 frontend-engineer; B2, B3, B4 backend-engineer; B5 dbadmin (backend-engineer talebiyle).
Yeniden denetim şartı: aşağıdaki tur 2 listesi; tur 2 bu fazın son turudur (Workflow.md §3).
```

Analist doğrulaması: B1 (`Views/Users/Index.cshtml:102,103,111,124` dört `stack-wide`; `Styles/app.css` `.table-stack .stack-wide` = `col-span-full`),
B3 (`Authorization/AccountPrincipal.cs:11-16` içinde `NameIdentifier` yok), B4 (`appsettings.json:3`), B5 (`UserRoleConfiguration.cs:12-14`, migration
ve snapshot aynı metni taşıyor) dosyadan okundu, raporla aynı. B2 kanıtı (`SuperAdminYarisi.txt`) ve `UserAdministration.cs` `DeactivateAsync`,
`UpdateAsync` okuma-sonra-yazma biçimi raporla aynı.

**frontend-engineer**

1. B1 (T-B21, G3): uygulama DNA'ya uyar; `Dna.md` veri dili satırı değişmez, kimlik değişikliği yok. `Views/Users/Index.cshtml`'de Roller,
   Durum ve İşlem hücrelerinden `stack-wide` kaldırılır. Tek uzun alan Ad soyad'dır: 768 px altında tam satır kaplayabilir, 768-1279 px arasında
   en çok iki sütun kaplar. Diğer hücreler birer sütundur. Uzun ad (108 karakter) satır kırar, taşmaz. 1280 px ve üstü tablo değişmez.
2. B6: "Kullanıcı değiştir" düğmesinin üç yeri (üst bar, ray, çekmece; `_SwitchUser.cshtml` iki form) `data-requires-online` taşır, çevrimdışıyken
   diğer POST düğmeleri gibi devre dışıdır. Çevrimdışı şerit metni kullanıcı değiştirmenin de bağlantı gelince yapılabileceğini söyler. Devir notu
   ölçülen davranışla aynı yazılır.

**backend-engineer**

1. B2: son aktif Süper admin kuralı, aktif Süper admin sayısını azaltabilen her yolda (pasifleştirme ve Süper admin rolünü kaldırma) tek bir
   yerde uygulanır. Denetim ve yazma, başında yazma kilidi alan tek işlemde yapılır (SQLite'ta `BEGIN IMMEDIATE`; `BeginTransactionAsync`
   varsayılanının bunu verdiği doğrulanıp devir notuna yazılır). Kilit alındıktan sonra hedef, işlemi yapan ve aktif Süper admin sayısı
   veritabanından yeniden okunur; işlemi yapan artık aktif değilse ya da kapsamı kalmadıysa istek değişiklik yapmaz. Kilidi bekleyen istek 500
   vermez. PIN doğrulaması (hatalı deneme sayacının yazılması dahil) işlemin önünde kalabilir; BR-48 Faz 4'e devrediyor. Şema değişikliği
   beklenmiyor; gerekirse backend DB talebi açar, talep ALTER sınıfıdır ve uygulamadan önce kullanıcı onayı ister, koordinatör o noktada durur.
2. B3: `AccountPrincipal.Create` içine `ClaimTypes.NameIdentifier` = kullanıcı kimliği. B2'nin rol kaldırma kolunun eşzamanlı testi buna bağlı.
3. B4: `appsettings.json` içinde `Microsoft.EntityFrameworkCore.Database.Command` düzeyi `Warning`.
4. B5 talebi: `Docs/Db/DB-2026-003.md` açılır. İstenen: `CK_UserRoles_Role` ifadesi `Roles` sabitlerinden kurulur, üretilen SQL aynı kalır, şema
   değişikliği yok.

**dbadmin** (yalnız DB-2026-003 ile): `UserRoleConfiguration.cs` CHECK ifadesi `Roles` sabitlerinden kurulur. `dotnet ef migrations
has-pending-model-changes` değişiklik bildirmez; yeni migration dosyası oluşmaz; mevcut migration ve snapshot dosyalarındaki metin üretilmiş
geçmiş kayıt olarak istisnadır ve elle değiştirilmez. Model farkı çıkarsa dbadmin durur, migration üretmez, B5 Faz 2'ye ertelenir.

Paralellik: frontend ve backend dosyaları ayrık, birlikte çağrılabilir; dbadmin backend'in talebinden sonra.

**Tur 2 yeniden denetim şartı (qa-tester)**

1. B1, T-B21, G3: `/users` dolu, sayfa 2, uzun ad ve süzgeçli boş durumları 360, 768, 1024, 1440 px ve %200 yakınlaştırma (320) hücrelerinde §3
   eval ile; hücre genişliği ölçümü (`MekanikEval.txt` son bölüm yöntemi): 768 ve 1024'te Ad soyad dışında tam satır hücre yok ve Ad soyad en çok
   iki sütun; 360'ta Ad soyad dışındaki dört hücre ikişer yan yana; yatay kaydırma ve 44 px altı hedef 0; 1440'ta tablo ve 56 px satır aynı.
2. B2: `SuperAdminYarisi.txt` senaryosu üç deneme karşılıklı pasifleştirme, üç deneme pasifleştirme ile rol kaldırmanın eşzamanlısı. Her denemenin
   sonunda aktif ve `SuperAdmin` rollü en az bir hesap kalır, isteklerden biri değişiklik yapmaz, 500 yok. K8 sıralı gerilemesi.
3. B3: Yönetici bir kullanıcının adını ve rollerini değiştirdikten sonra o kullanıcının önceden açık formundan gönderilen POST 400 almaz; G5
   token'sız POST hâlâ 400.
4. B4: Production turunun günlüğünde "Executed DbCommand" Information kaydı 0; K10 PIN taraması yeniden.
5. B5: `Src/` altında `Roles.cs` ve `Data/Migrations/` dışında `'Operator'`, `'Manager'`, `'Auditor'`, `'SuperAdmin'` metni 0; `Migrations/` altında
   yeni dosya yok; `has-pending-model-changes` değişiklik yok.
6. B6: çevrimdışıyken üç "Kullanıcı değiştir" düğmesi devre dışı ve şerit metni görünür; bağlantı gelince etkin, K11 tek dokunuş.
7. Gerileme: G1, G2, G5, K8a, K11, K11a; prototip sayfaları Development'ta 200, Production'da 404.

**Tur 2 ve tur 3 kararı**

```
FAZ 1: TESLİM ALINDI
Tarih: 2026-11-14
Gerekçe: Tur 2'de B1-B6 kapandı, kabul kriterleri 28/28; B7 (ORTA) açık kaldığı için Workflow.md §3 gereği kullanıcıya soruldu,
kullanıcı B7 ve gerileme ile sınırlı 3. turu onayladı (Decisions.md PL-31). Tur 3'te B7 kapandı, gerileme temiz, açık KRİTİK, YÜKSEK,
ORTA ve DÜŞÜK bulgu yok, QA önerisi GEÇTİ. Faz T'den devreden T-B15, T-B16, T-B21 kapandı.
Devreden: bölüm 9 madde 1 (açık iş durumu ölçümü, qa-tester) ve madde 2 (BR-48 PIN ile iş eylemi aynı işlemde, backend-engineer)
Faz 4'ün "Faz 1'den devreden" satırlarına yazıldı (PL-32).
```

Kabul kriterleri eşlemesi: K1-K14 (K8a, K8b, K11a, K12a dahil) bölüm 2 ve T2.2; T-B15, T-B16 bölüm 2; T-B21, G3 T2.2 ve T3.2; K8 T2.1 madde 2;
G1, G2, G6 T3.2; G4, G5, G7 bölüm 2 ve T2.2. 28 kriterin her biri sonuç ve kanıtla eşleşti.

Analist doğrulaması (tur 3):

- B7: `Styles/app.css:562-564` `.table-stack .stack-lead` = `max-md:order-first`; derlenen `wwwroot/css/app.css` içinde `.stack-lead{order:-9999}`
  tek eşleşme; `stack-lead` yalnız `Views/Users/Index.cshtml:102` Ad soyad hücresinde, prototip `Records.cshtml:122` yalnız `stack-wide`. Rapor ile aynı.
- T-B15: `Program.cs:17` `AutoValidateAntiforgeryTokenAttribute` genel filtre; `PrototypeController.cs` beş action `[HttpGet]`;
  `[IgnoreAntiforgeryToken]` yalnız `AccountController.cs:73` (logout, K11a) ve `ErrorController.cs:7` (hata sayfaları). Rapor ile aynı.
- B3, B5 gerilemesi: `AccountPrincipal.cs:14` `NameIdentifier`; `Src/` altında tırnaklı rol adı metni `Roles.cs` ve `Data/Migrations/` (üretilmiş, PL-30)
  dışında 0.

## Tur 2 (2026-11-14)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `83c4415`); devir notları `Docs/Handover/frontend-engineer-20261114-2000.md`, `Docs/Handover/backend-engineer-20261114-2100.md`; `Docs/Db/DB-2026-003.md` (UYGULANDI) |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/t2f1/build`; Production 5121, Development 5122 (yalnız prototip); DB `.qa/t2f1/qa2.db` = `App_Data/vaka.db` kopyası (md5 `615db04b...`), veri uygulama üzerinden kuruldu: 0001 ilk girişi, 26 hesap (ikinci Süper admin 1002, Yönetici 2001, Yönetici+Operatör 2002, Operatör 3001, Denetçi 3002, 21 dolgu hesabı, 108 karakterlik ad 3199) |
| QA önerisi | KALDI (B7 ORTA açık; KRİTİK ve YÜKSEK yok) |

Betikler ve ham çıktılar `.qa/t2f1/` altında (`b1.tmpl.js`, `m.tmpl.js`, `b6.tmpl.js`, `race.sh`, `race2.sh`, `b3.sh`, `k8.sh`, `k11b.sh`; `*.json`, `*.out`).
Parola ve PIN değerleri dosyaya yazılmadı, betiklere ortam değişkeniyle verildi. Kaynak dosyaların md5 listesi test başında ve sonunda aynı
(127 dosya, `.qa/t2f1/src-start.md5` = `src-end.md5`).

### T2.1 İade edilen bulguların yeniden ölçümü (bölüm 10, tur 2 şartı)

| # | İstenen sonuç (bölüm 10) | Ölçüm | Sonuç |
|---|---|---|---|
| 1. B1, T-B21, G3 | `/users` dolu, sayfa 2, uzun ad, süzgeçli boş x 360, 768, 1024, 1440, 320; 768 ve 1024'te Ad soyad dışında tam satır hücre yok, Ad soyad en çok 2 sütun; 360'ta diğer dört hücre ikişer yan yana; yatay kaydırma ve 44 px altı hedef 0; 1440 tablo ve 56 px satır | `b1.tmpl.js`, ek olarak 1279 ve 1280. 768/1024/1279: 4 sütun; Ad soyad `grid-column-end: span 2`, Sicil no, Roller, Durum, İşlem birer sütun; kart iki satır (Sicil, Ad soyad, Roller / Durum, İşlem); 1024 kart 140-160 px (tur 1: 268-286), uzun ad 194 px. 1280/1440: tablo, en kısa satır 69 px. Uzun ad 360'ta 3, 320'de 4, 768-1440'ta 3 satıra kırılıyor, hücre taşması 0. Süzgeçli boş: tablo yok, boş durum kutusu. Tüm hücrelerde `hScroll` false, `main` içinde 44 px altı hedef 0. 360 ve 320: 2 sütun, Ad soyad tam satır; ancak Sicil no 1. satırda tek, Roller + Durum 3. satırda yan yana, İşlem 4. satırda tek | KISMEN: 768-1279, 1280+, uzun ad, boş durum, %200 GEÇTİ; 360 ikili yerleşim KALDI (B7) |
| 1a. PL-28, prototip Records | Tam satırı yalnız tek uzun metin alanı | Development `/prototype/records`: 360/320'de Makine tam satır, kayıt no (etiketsiz kart başlığı) 1. satırda, diğer altı hücre ikişer yan yana; 768-1279'da Makine 2 sütun, diğerleri birer; 1280 ve üstü tablo. `hScroll` false, 44 px altı 0 | GEÇTİ |
| 2. B2 | Üç karşılıklı pasifleştirme, üç pasifleştirme + rol kaldırma eşzamanlı; sonda en az bir aktif Süper admin, biri değişiklik yapmaz, 500 yok; K8 sıralı gerilemesi | `race.sh`, `race2.sh`. A1-A3 (0001 önce) ve A4 (1002 önce) karşılıklı pasifleştirme: her seferinde biri 302 > `/users`, diğeri 403 "Yetkiniz yok"; kazanan iki yönde de görüldü. B1-B3 pasifleştirme + rol kaldırma (yönler değiştirilerek): rol kaldırma kazandı, pasifleştirme 403. Gecikmeli 5 deneme (0,015-0,035 sn): pasifleştirme kazanınca düzenleme isteği ya oturumu düşürüp `reason=inactive` ile girişe gitti ya da (0,025 sn) 0,16 sn kilit bekleyip 403 aldı. 12 denemenin 12'sinde sonda aktif Süper admin tam 1. Günlükte `fail`/`crit` 0. Geri alma uygulama içinden (etkinleştirme, rol verme). Kod: `Services/UserAdministration.cs:193-227` `RunExclusiveAsync` (`:201` işlem, `:204` işlemi yapanın aktifliği kilit içinde, `:161-170` pasifleştirme engeli kilit içinde yeniden, `:103-135` rol güncelleme ve `:115` son aktif Süper admin denetimi kilit içinde). K8 sıralı: iki aktif Süper admin ile pasifleştirme ve rol kaldırma 302; kendi hesabını pasifleştirme "Kendi hesabınızı pasifleştiremezsiniz."; kendi rol değişikliği yok sayıldı (rol `SuperAdmin` kaldı) | GEÇTİ |
| 3. B3 | Yönetici ad ve rolleri değiştirdikten sonra kullanıcının açık formundan POST 400 almaz; G5 hâlâ 400 | `b3.sh`: 3001 `/account/security` formunu açar, 2001 adını değiştirir, eski token'la PIN formu 200 (alan hatası "PIN'ler aynı değil."); 2001 rollerini Operatör + Denetçi yapar, eski token'la gerçek PIN değişikliği 302 ve `PinHash` değişti; ad + rol birlikte değişince parola formu 200 (alan hatası). 400 yok. `Authorization/AccountPrincipal.cs:14` `NameIdentifier`. Token'sız POST 400 | GEÇTİ |
| 4. B4 | Production günlüğünde "Executed DbCommand" 0; K10 PIN taraması | `logs/run-prod.log` (tur 2'nin tamamı): "Executed DbCommand" 0, satır 8 (yalnız başlangıç kayıtları); kullanılan 3 PIN ve 4 parola değeri 0 eşleşme. `appsettings.json:7` `Warning` | GEÇTİ |
| 5. B5 | `Roles.cs` ve `Data/Migrations/` dışında rol adı metni 0; yeni migration yok; `has-pending-model-changes` temiz | Tek ve çift tırnaklı rol adı metni `Src/` altında iki istisna dışında 0; `Data/Migrations/` içindeki 3 eşleşme üretilmiş dosyalarda (PL-30 istisnası). `Data/Configurations/UserRoleConfiguration.cs:13-15` ifade `Roles` sabitlerinden. `Data/Migrations/` 3 dosya, zaman damgası tur 1 ile aynı (15:16). `dotnet ef migrations has-pending-model-changes --configuration Release --no-build`: "No changes have been made to the model since the last migration." `DbChanges.md:9` DB-2026-003 kayıtlı | GEÇTİ |
| 6. B6 | Çevrimdışıyken üç "Kullanıcı değiştir" devre dışı, şerit metni; bağlantı gelince etkin; K11 tek dokunuş | `b6.tmpl.js` (`context.setOffline`): 360, 768, 1440 ve odak kipi (ilk giriş) 360, 1440. Çevrimdışı: üst bar, ray ve çekmece düğmeleri `disabled` (çekmece açıkken 291x44 ve 326x44 devre dışı), `data-requires-online` 3/3 (security 5/5, ilk giriş 2/2), şerit "Bağlantı yok. Kayıt, onay ve kullanıcı değiştirme yapılamaz. Bağlantı gelince bu uyarı kapanır, işlemi yeniden yapın." Bağlantı gelince hepsi etkin, şerit gizli. Özniteliksiz POST düğmesi 0. K11: 360 üst bar ve 1440 ray tek dokunuşla `/account/login?reason=signed-out` | GEÇTİ |

### T2.2 Kabul kriterleri (yeniden işaretlenen ve gerileme)

| # | Sonuç | Kanıt |
|---|---|---|
| T-B21 | GEÇTİ | `Dna.md:34` "768 px altında iki, 768-1279 px arasında dört sütunlu ızgara", "1280 px ve üstünde satır en az 56 px": ölçülen 2 / 4 sütun, 1280+ tablo, en kısa satır 69 px; PL-28 tam satır kuralı uyuyor. 360 yerleşim boşluğu DNA metnine aykırı değil, B7 olarak ayrı |
| G3 | GEÇTİ | §4 grep 0 ihlal, konsol yalnız belge durum kodları, §3 eval temiz (T2.3), DNA veri dili UYGUN (T2.5) |
| K8 | GEÇTİ | Sıralı kurallar (T2.1 madde 2) ve eşzamanlı istekte kural tutuyor (B2 kapandı) |
| G1 | GEÇTİ | `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata (CoreCompile çalıştı). `npm run build:css` başarılı, çıktı depodaki `wwwroot/css/app.css` ile aynı (md5 `8de7134b...` önce ve sonra) |
| G2 | GEÇTİ | Policy grep 0, sihirli `[Authorize(Roles = "` 0; 5 controller'ın 5'i sınıf düzeyinde `[Authorize]` ya da `[AllowAnonymous]`; MediatR, AutoMapper, generic repository 0; `new ...Service(` 0; rol adı metni yalnız `Roles.cs` ve üretilmiş migration'da |
| G5 | GEÇTİ | Token'sız POST: Süper admin oturumunda Faz 1'in 9 POST'u 9/9 400, Operatör oturumunda 2/2 400, oturumsuz `/account/login` 400 |
| K8a | GEÇTİ | Yönetici formunda yalnız `Operator`, `Auditor`; `SelectedRoles=SuperAdmin` ve `=Manager` istekleri "Seçilen rolü atayamazsınız.", kayıt oluşmadı; Yönetici'nin Süper admin (Id 1, 2) ve Yönetici+Operatör (Id 4) hesaplarına GET ve POST (edit, deactivate, reset-pin) 302 > 403, hesaplar değişmedi; listede yalnız Operatör/Denetçi hesaplarına (Id 5-20) düzenle bağlantısı |
| K11 | GEÇTİ | Kişi ve sicil üst barda 320, 360, 390, 768'de, 1024 ve üstünde rayda (T2.3; `KullanicilarTablo-1440.png`); "Kullanıcı değiştir" 360: 92x44, 768: 118x44, rayda; tek dokunuş girişe (T2.1 madde 6) |
| K11a | GEÇTİ | Oturum süresi kopyada 1 dk: 66 sn işlemsizlik sonra eski token'la logout 302 `/account/login?reason=signed-out`; aynı sürede başka oturumun GET'i `reason=expired` (oturumun gerçekten düştüğü); tarayıcıda 360 px, 70 sn sonra üst bar düğmesi girişe. Ayar 30'a geri alındı |
| K14 (prototip) | GEÇTİ | Development: `/prototype/my-work`, `/run`, `/new-record`, `/records`, `/dashboard` 200; Production 5/5 404 |

Diğer kriterler tur 1 sonucunda kalır. Kabul kriterleri tur 2 sonunda 28/28 GEÇTİ.

### T2.3 Mekanik eval (§3), küçük ekran yeniden

B1 CSS'i (`.table-stack .stack-wide` = `max-xl:col-span-2`) ortak olduğu için Faz 1 sayfaları ve prototipler yeniden ölçüldü: 360x800, 768x1024,
1024x768 açık ve koyu, 320x640 (%200), 390x844 reduced-motion; 6 grup (oturumsuz, Operatör, Süper admin, Yönetici, Yönetici+Operatör,
prototip), 31 sayfa/durum, 248 ölçüm (`Evidence/Faz-1/MekanikEval.txt` "Tur 2").

| Ölçüm | Sonuç |
|---|---|
| h1 = 1, atlama bağlantısı, imgNoAlt/imgNoSize/inputNoLabel/iconBtnNoName | 248/248 temiz |
| emdash, hScroll, dış kaynak, yazı tipi | 0, 0, 0; gövde Hanken Grotesk, `fonts.check` 248/248 |
| smallTargets (<40 px), <24 px | 0, 0 |
| AA kontrast | altı 0, en düşük 5,28:1 (açık ve koyu) |
| Üst bar, 768 altı gövde yazısı | 64 px 248/248; 16 px altı 0 |
| reduced-motion animasyon | 0 |
| overflowing | yalnız 360 ve 320 `/users` kartında `sr-only` thead (tur 1 ile aynı yanlış pozitif) |
| Konsol | yalnız belge durum kodları (`/account/access-denied` 403, `/error/404` 404, `/error/403` 403, `/error` 500); başka hata ve uyarı 0 |

### T2.4 Klavye turu (değişen kısım)

Çevrimdışıyken üç "Kullanıcı değiştir" düğmesi `disabled` olduğu için Tab sırasından çıkıyor; bağlantı gelince yeniden etkin ve odaklanabilir.
Çekmece açık ve çevrimdışıyken çekmecedeki düğme devre dışı görünüyor (`disabled:opacity-50`, `cursor-not-allowed`), şerit metni nedenini
söylüyor. Diğer klavye davranışları tur 1 sonucunda kalır (`Evidence/Faz-1/KlavyeTuru.txt` "Tur 2").

### T2.5 DNA uygunluğu (değişen maddeler)

- Veri dili: UYGUN. 768 altında iki, 768-1279 px arasında dört sütunlu ızgara; tam satırı yalnız tek uzun metin alanı (Ad soyad, prototipte
  Makine) kaplıyor, 768-1279'da en çok iki sütun (PL-28); 1280 ve üstü tablo, satır en az 56 px. Kalıntı: 768 altında kart dört satır ve iki
  yarım satır boş (B7); DNA metnine aykırı değil, yoğunluk sorunu.
- Gezinme arketipi: UYGUN. Çevrimdışı "Kullanıcı değiştir" durumu PL-30 ile aynı: devre dışı ve şeritte nedeni yazılı.
- Diğer maddeler tur 1 sonucunda kalır. Anti-slop: değişiklik yok.

### T2.6 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-1/` (10 dosya):

- `KullanicilarKart-360.png`, `KullanicilarKart-1024.png`, `KullanicilarTablo-1440.png`: tur 2 görüntüleriyle değiştirildi (Read ile açıldı).
  360: Sicil no tek, Ad soyad tam satır, Roller + Durum yan yana, İşlem tek (B7). 1024: Sicil, Ad soyad, Roller / Durum, İşlem. 1440: tablo.
- `KullanicilarUzunAd-320.png` (yeni): 108 karakterlik ad 320 px'te dört satıra kırılıyor, taşma yok.
- `MekanikEval.txt`, `SuperAdminYarisi.txt`, `KabulAkisi.txt`, `KlavyeTuru.txt`: sonlarına "Tur 2" bölümü eklendi.
- `KullaniciFormu-RolKilitli-1440.png`, `YetkiMatrisi.txt`: tur 1'den, değişmedi.

Diğer görüntüler `.qa/t2f1/shots/` altında (9 dosya; 768 kart, uzun ad 1024, prototip Records 360, 768, 1280).

### T2.7 Yeni bulgular

| # | Şiddet | Özet | Sahibi |
|---|---|---|---|
| B7 | ORTA | 768 px altında kullanıcı kartında Sicil no ve İşlem tek başına bir satır kaplıyor, Ad soyad ikinci satıra itiliyor; tur 2 şartı 1'in 360 kısmı karşılanmıyor (ayrıntı bölüm 6) | frontend-engineer |

Not (bulgu değil): `Services/UserAdministration.cs` 365 satır (tur 1: 308). `CodingStandards.md §2` "200 satırı geçen servis bölünmeye
adaydır"; ihlal değil, sonraki fazlarda büyürse bölünmeli. §7: bu turda süre hesaplayan yeni metot yok. §8: emoji, `#region`, süs yorum,
`Helper`/`Utils`/`Manager` adı 0; `RunExclusiveAsync` XML yorumu "neden"i anlatıyor.

### T2.8 Kapsam dışı / yapılamayan (tur 2)

- Kilidi bekleyen isteğin zaman aşımı yolu ("Sistem şu an meşgul. İşlemi birkaç saniye sonra yeniden deneyin.") üretilmedi: yazma kilidinin 30 sn
  tutulması gerekiyor, uygulama içinden bu kadar uzun tutan işlem yok. Kod `UserAdministration.cs:223-226`; ölçülmedi, geçti sayılmadı.
- Tur 2'de 1920 px, koyu tema 1440 ve 800x360 hücreleri yeniden ölçülmedi: değişen CSS yalnız `max-xl` (1279 ve altı) kırılımında. 1440 tablo
  ölçüldü (T2.1).
- QA'nın tur 2 başında `App_Data/vaka.db` üzerinde yaptığı salt okunur SQLite sorgusu (`mode=ro`) dosyanın yanında `vaka.db-wal` (0 bayt) ve
  `vaka.db-shm` oluşturdu (16:14). `vaka.db` md5 değeri test başında ve sonunda aynı (`615db04b...`); dosyalar `.gitignore` kapsamında. QA bu
  dosyaları silmedi, çünkü `App_Data` yazma alanı dışında ve aynı veritabanını QA dışı süreç (PID 14080, port 5080) açık tutuyor olabilir.
- QA dışı süreç PID 14080'e dokunulmadı. QA'nın açtığı 5121 ve 5122 süreçleri ve playwright oturumu kapatıldı.

### T2.9 Sonuç

İade edilen altı bulgudan B2, B3, B4, B5, B6 kapandı; B1'in DNA ve PL-28 kısmı kapandı (T-B21, G3 geçti, kabul kriterleri 28/28). Tur 2 şartı
1'in 360 px kısmı karşılanmadı ve B7 (ORTA) olarak açıldı. KRİTİK ve YÜKSEK bulgu yok; ORTA aynı fazda kapanır (`Workflow.md §3`), bu nedenle
QA önerisi KALDI. Tur 2 bu fazın son turu; karar analistte.

## Tur 3 (2026-11-14)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `83c4415`); devir notu `Docs/Handover/frontend-engineer-20261114-2200.md` (B7). Kapsam: yalnız B7 ve gerileme (kullanıcı onaylı sınırlı ek tur, 2026-11-14) |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/f1t3/build`; Production 5131, Development 5132 (yalnız prototip); DB `.qa/f1t3/qa3.db` = `App_Data/vaka.db` kopyası (md5 `615db04b...`), 27 hesap uygulama üzerinden kuruldu (tur 2 ile aynı küme, 108 karakterlik ad 3199 dahil) |
| QA önerisi | GEÇTİ (B7 kapandı; açık KRİTİK, YÜKSEK, ORTA, DÜŞÜK yok) |

Betikler ve ham çıktılar `.qa/f1t3/` altında (`b7.tmpl.js`, `kbd.tmpl.js`, `m.tmpl.js`, `an.py`; `b7.json`, `kbd.json`, `r-*.json`, `m.out`).
Parola ve PIN değerleri dosyaya yazılmadı (betiklere ortam değişkeniyle; `.qa/f1t3` altındaki JSON ve günlüklerde 0 eşleşme). Kaynak dosyaların md5
listesi test başında ve sonunda aynı (127 dosya, `.qa/f1t3/src-start.md5` = `src-end.md5`). `App_Data/vaka.db` md5 başta ve sonda aynı.

### T3.1 B7 yeniden ölçümü

Değişiklik: `Styles/app.css:562-564` `.table-stack .stack-lead { @apply max-md:order-first; }`, derlenen CSS'te
`@media not all and (min-width:768px){.table-stack .stack-lead{order:-9999}}`; `Views/Users/Index.cshtml:102` Ad soyad hücresinde `stack-lead`.
Ölçüm `b7.tmpl.js`: `/users`, `/users?page=2&status=Active`, `/users?query=Abdurrahman&status=All`, `/users?status=All` x 320, 360, 390, 768, 1024, 1279,
1280, 1440 x açık ve koyu (64 ölçüm); imza görsel sıradır (satır, sonra sol kenar).

| İstenen | Ölçüm | Sonuç |
|---|---|---|
| 768 altı imza `Ad soyad:2@1 Sicil no:1@2 Roller:1@2 Durum:1@3 İşlem:1@3` | 320, 360, 390 açık ve koyu, 4 sayfanın tüm satırlarında tek imza, beklenenle aynı; 640x400 (1280 px %200) de aynı | GEÇTİ |
| Kartta boş hücre yok | 768 altı satır başına doluluk 2+2+2, boş hücre 0 (tüm satırlar). Kart 360'ta 186-221 px (tur 2: 234-285), uzun ad 261 px (tur 2: 325) | GEÇTİ |
| 108 karakterlik ad satır kırar | 320'de 4, 360 ve 390'da 3, 768-1440'ta 3 satır; hücre taşması 0, `hScroll` false | GEÇTİ |
| 768-1279 tur 2 ile aynı | 768/1024: 4 sütun, `Sicil no:1@1 Ad soyad:2@1 Roller:1@1 Durum:1@2 İşlem:1@2`, kart 140-160 px, uzun ad 194 px; 1279: 4 sütun, kart 140 px, uzun ad 174 px. Tur 2 `b1.out` değerleriyle (iz genişliği 161 / 163 / 227 px, Ad soyad 320 px) bire bir aynı | GEÇTİ |
| 1280+ tablo değişmedi | 1280 ve 1440 tablo, satır 69-85 px, en kısa 69 (tur 2 ile aynı) | GEÇTİ |
| Açık / koyu tema | Zemin açıkta `rgb(236,240,242)`, koyuda `rgb(15,20,25)`; imza ve ölçüler iki temada aynı; koyu 360 görüntüsü Read ile açıldı (`.qa/f1t3/shots/users-360-dark.png`) | GEÇTİ |
| Klavye odak sırası görsel sırayla çelişmiyor | Kartta odaklanabilir öğe her satırda yalnız İşlem bağlantısı (`a:İşlem`, tüm genişliklerde). `main` içinde Tab turu 360, 320, 640x400, 768: 13 durak, yukarı geri sıçrama 0, odak halkasız durak 0, görünür alan dışında kalan durak 0 (`KlavyeTuru.txt` "Tur 3") | GEÇTİ |
| %200 zoom | 320x640 ve 640x400: 2 sütun, beklenen imza, yatay kaydırma 0 | GEÇTİ |

Devir notundaki not doğrulandı: DOM sırası Sicil no, Ad soyad olarak kaldı; ekran okuyucu 768 altında Sicil no'yu önce okur, hücre etiketleri
(`data-label`) yazılı. Odak sırasını etkilemediği için bulgu değildir.

### T3.2 Gerileme

| Kontrol | Sonuç | Kanıt |
|---|---|---|
| G1 derleme | GEÇTİ | `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata. `npm run build:css` başarılı, çıktı depodaki `wwwroot/css/app.css` ile aynı (md5 `84fce5fe...` önce ve sonra); yayındaki CSS aynı md5 |
| §3 mekanik eval | GEÇTİ | Faz 1 sayfaları ve prototipler: 360x800, 768x1024, 1024x768 açık ve koyu, 320x640 (%200), 390x844 reduced-motion; 6 grup, 31 sayfa/durum, 248 ölçüm. `an.py` çıktısı tur 2 ile satır satır karşılaştırıldı: tek fark `/users` kart yüksekliği 360/320'de düştü. h1, atlama bağlantısı, görsel/alan/ikon adı, yatay kaydırma, dış kaynak, yazı tipi, 40 ve 24 px altı hedef, kontrast (en düşük 5,28:1), üst bar 64 px, reduced-motion animasyonu: 0 ihlal |
| Prototip Records | GEÇTİ | 360: kayıt no 1. satır, Makine tam satır, diğerleri ikişer yan yana (tur 2 ile aynı); `stack-lead` yalnız `Views/Users/Index.cshtml:102`, prototipte yok |
| §4 grep tablosu | GEÇTİ | `Views/**`, `Styles/app.css`, `wwwroot/js/**`: `transition-all`, `user-scalable`/`maximum-scale`, `div/span onclick`, CDN, `h-screen`, jquery/gsap/aos, `slate-/gray-/zinc-`, `focus-visible`'sız `outline-none`, boyutsuz `<img`, görünür `...`, görünümde ham hex: 0 |
| G2 (`CodingStandards.md §4`) | GEÇTİ | Policy grep 0, `[Authorize(Roles = "` 0, MediatR/AutoMapper/generic repository 0 |
| G6 em/en-dash | GEÇTİ | `Views`, `Styles`, `wwwroot/js`, `Controllers`, `Services`, derlenen CSS ve devir notunda 0; §3 eval `emdash` 248/248 temiz |
| Konsol | GEÇTİ | 56 kayıt, yalnız belge durum kodları (`/account/access-denied` 403, `/error/404` 404, `/error/403` 403, `/error` 500), tur 2 ile aynı sayı; sunucu günlüklerinde `fail`/`crit` 0, Production günlüğünde "Executed DbCommand" 0 |
| DB | GEÇTİ | `Data/Migrations/` 3 dosya, değişiklik yok; bu turda şema değişikliği yok |

Kabul kriterleri tur 3 sonunda 28/28 GEÇTİ (değişen kriter yok; T-B21 ve G3 yeniden doğrulandı).

### T3.3 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-1/` (10 dosya, sayı değişmedi):

- `KullanicilarKart-360.png`: tur 3 görüntüsüyle değiştirildi (Read ile açıldı). Her kartta Ad soyad tam satır, Sicil no + Roller, Durum + İşlem; boşluk yok.
- `KullanicilarUzunAd-320.png`: tur 3 görüntüsüyle değiştirildi (Read ile açıldı). 108 karakterlik ad 4 satır, altında Sicil no + Roller, Durum (Etkin, İlk giriş bekliyor) + İşlem.
- `MekanikEval.txt`, `KlavyeTuru.txt`: sonlarına "Tur 3" bölümü eklendi.
- Diğer altı dosya değişmedi.

Ek görüntüler `.qa/f1t3/shots/` altında (`users-360-dark.png`, `records-360.png`).

### T3.4 Yeni bulgular

yok

Not (bulgu değil, tur 2 ile aynı): 1279 px'te Ad soyad hücresi `grid-column-end: span 2` alıyor ama `max-w-xs` nedeniyle 320 px genişlikte
kalıyor (iki iz 470 px); ölçüm tur 2 ile aynı, B7 kapsamı dışında ve görsel taşma yok.

### T3.5 Kapsam dışı / yapılamayan (tur 3)

- Yetki matrisi, B2-B6 akışları, çevrimdışı durum ve 1920 px yeniden ölçülmedi: bu turda değişen yalnız `Styles/app.css` (`max-md` kırılımı) ve
  `Views/Users/Index.cshtml` içinde tek sınıf; tur 2 sonuçları geçerli.
- %200 zoom tarayıcı yakınlaştırmasıyla değil, eşdeğer CSS genişliğiyle ölçüldü (320x640 = 640 px pencere, 640x400 = 1280 px pencere).
- Test kurulumunda iki QA sunucusu aynı yayın klasöründen çalıştığı için 5131 oturum çerezi 5132'de de geçerli oldu; ilk prototip görüntüsü bu
  nedenle ilk giriş sayfasına düştü, çerezsiz yeniden alındı. Mekanik eval'in prototip grubu zaten çerezsiz çalışıyor. Ürün bulgusu değildir.
- QA dışı süreç PID 14080'e ve `App_Data`'ya dokunulmadı. QA'nın açtığı 5131 ve 5132 süreçleri (PID 10576, 12012) ve playwright oturumu kapatıldı.

### T3.6 Sonuç

B7 kapandı: 768 px altında kullanıcı kartı üç satır, boş hücre yok; 768-1279 ve 1280+ yerleşimi tur 2 ile aynı; odak sırası görsel sırayla
çelişmiyor. Ortak CSS değişikliği Faz 1 sayfalarında ve prototiplerde gerileme üretmedi. Açık bulgu yok, kabul kriterleri 28/28; QA önerisi GEÇTİ.
Karar analistte.

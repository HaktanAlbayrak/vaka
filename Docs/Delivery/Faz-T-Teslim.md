# Faz T Teslim Raporu: Tasarım DNA'sı ve kabuk prototipi

| Alan | Değer |
|---|---|
| Faz | T |
| Tarih | 2026-11-13 |
| Tur | 3 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `04eb3d8`; `Src/` ve devir notları commit'lenmemiş, `Docs/Design/Dna.md` değişmiş) |
| Ortam | LOCAL, Release build, port 5101 (Production) ve 5102 (Development), ayrıca `dotnet run --no-launch-profile` ile 5103 (Production); DB: yok (Faz T veritabanı içermez) |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 3) |
| Analist kararı | TESLİM ALINDI (tur 3, 2026-11-13; tur 1 İADE). Commit şartı: `Dna.md:21` durum satırı (bölüm 10, tur 3 kararı) |

## 1. Kapsam

- BE 1: `Src/Vaka.Web` projesi, `Program.cs` (MVC, küçük harf ve kebab URL, statik dosya, hata ve 403 route'ları), `Authorization/Roles.cs` (dört tekil, dört birleşik sabit).
- BE 2: `PrototypeController` (`[AllowAnonymous]`), yalnız Development'ta yanıt, diğer ortamlarda 404.
- FE 3: `Docs/Design/Dna.md` (ön blok, okuma satırı, kadranlar, DNA kartı, kaçınılanlar).
- FE 4: Tailwind CLI, token tabanlı açık ve koyu tema, self-host yazı tipleri, `_Layout` kabuğu (rol bazlı menü, kişi adı, sicil no, "Kullanıcı değiştir", bağlantı göstergesi).
- FE 5: Beş prototip sayfa (`/prototype/my-work`, `/run`, `/new-record`, `/records`, `/dashboard`) ve demo durum/sahne varyantları (toplam 26 URL).
- FE 6: Ortak bileşenler `_Badge`, `_PinInput` (iki mod), `_State`, `_PhaseStrip`, `_PhaseTrack`, `_BarChart`, `_NavList`, `_DemoBar`.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| TK1 | Dna.md ön blok ve kart dolu, kadranlar gerekçeli, yazı tipi üçlüsü Inter değil, latin-ext ile ğ ş İ ı | GEÇTİ | `Docs/Design/Dna.md:1-17` ön blok, `:25` kadranlar, `:27-36` sekiz eksen gerekçeli. Archivo / Hanken Grotesk / JetBrains Mono; `Inter` grep 0. 420 matris hücresinin tümünde `document.fonts.check('16px "<aile>"','ğşİı')` üç aile için true; `document.fonts` altı yüzün altısı `loaded` (latin + latin-ext). `body` fontu `Hanken Grotesk` |
| TK2 | Beş prototip Development'ta açılır, Production'da 404 | GEÇTİ | Development (5102): 26 URL'nin tümü 200, her birinde `<h1` 1, em/en-dash 0, dış kaynak 0. Production yayını (5101): beş sayfa ve `?scene=break` 404. `dotnet run --no-launch-profile` + `ASPNETCORE_ENVIRONMENT=Production` (5103): beş sayfa 404, günlükte `Hosting environment: Production` |
| TK3 | 1440 / 768 / 390 görüntüleri, özgünlük kapısı, anti-slop temiz | GEÇTİ | 15 tam sayfa görüntü `.qa/shots/<sayfa>-<genişlik>.png` alındı ve tek tek incelendi; ayrıntı bölüm 5 "Özgünlük kapısı". Temsilî 10 dosya `Docs/Delivery/Evidence/Faz-T/` |
| TK4 | Yürütme ve İşlerim 360 px'te yatay kaydırmasız, birincil düğmeler en az 44 px | GEÇTİ | 360x800 açık ve koyu, 320x640: Run altı sahne ve MyWork dört durumda `hScroll` false, `overflowing` 0. "Adımı bitir", "Yürütmeye git", "Molayı bitir" 56 px; `smallTargets` 0 (Run, MyWork). Not: alt çubuk konumu B3 |
| TK5 | Yürütme prototipinde kayıt, faz, adım durumlarının her biri görünür ve metinle ayrılır; tek rozet biçimi | KALDI | Durum sözlüğü (Run `details`) 7 kayıt, 5 faz, 5 adım, 2 mola, 5 talep durumunun tümünü içerir; rozet tüm sayfalarda tek `_Badge` partial'ından (`class="badge` partial dışında 0). Ancak faz şeridinde rozet metni kesiliyor: 1024 ve 1280 px'te dört rozet "Ta…", "De…", "Be…" (Tamamlandı ile Tamamlandı - süre altında metinle ayrılmıyor), 768 ve 1440 px'te "Tamamlandı - sü…" (B2) |
| TK6 | Kişi adı, sicil no ve "Kullanıcı değiştir" 390 ve 1440 px'te görünür; çevrimdışında gösterge değişir | KALDI | 1440: ad, sicil, "Kullanıcı değiştir" görünür. 360 / 390 / 768: ad ve sicil üst çubukta görünür, `[data-switch-user]` görünür sayısı 0 (yalnız Menü çekmecesinde) (B5). Çevrimdışı: `context.setOffline(true)` sonrası çip "Bağlantı yok", şerit `display:flex` `role="alert"`, Run'da "Adımı bitir" ve "Mola ver", New-record'da "Kaydı aç" `disabled`; çevrimiçine dönünce üçü de geri açıldı. Kanıt `.qa/shots/offline-new-record-390.png` |
| TK7 | Açık ve koyu temada kontrast AA; reduced-motion açıkken hareket kapanır | GEÇTİ | Matris betiği her görünür metin öğesinin rengini etkin zeminle WCAG formülüyle ölçtü: 420 hücrede (28 URL x 15 hücre, açık ve koyu) 4.5:1 (büyük metinde 3:1) altı 0. `prefers-reduced-motion: reduce` hücrelerinde `.live-dot` `animation-name: none`, diğer hücrelerde `live-pulse 1.6s`. MOTION 3 olduğu için zorunlu değil, yine de kapanıyor |
| TK8 | Kullanıcı DNA kartını ve görüntüleri onaylar | Kullanıcı onayladı (2026-11-13) | Tur 3: kullanıcı DNA kartını ve tur 2 görüntülerini 2026-11-13'te onayladı (koordinatör iletisi). `Decisions.md` kaydı ve `Dna.md:21` durum satırı tur 3 sırasında henüz güncellenmemişti (bkz. Tur 3, T3.2) |
| TK9 | Roles.cs dört tekil, dört birleşik sabit; birleşiklerde rol metni yok; Roles.cs dışında rol adı metni yok | GEÇTİ | `Src/Vaka.Web/Authorization/Roles.cs:5-13`. `grep -rnwE "Operator\|Manager\|Auditor\|SuperAdmin"` (cs, cshtml, js, json) `Roles.X` kullanımı ve `Roles.cs` dışında 0 |
| G1 | `dotnet build -warnaserror` 0/0; `npm run build:css` başarılı | GEÇTİ | `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata. `npm run build:css`: "Done in 516ms", çıktı depodaki `wwwroot/css/app.css` ile bayt bayt aynı (`cmp`) |
| G2 | §4 grep 0, her controller yetki niteliği taşır, rol metni yalnız sabitlerden | GEÇTİ | Policy grep 0, sihirli rol grep 0; `ErrorController` ve `PrototypeController` `[AllowAnonymous]` |
| G3 | UiDelivery §2 matrisi kırık hücresiz, §4 sıfır ihlal, konsol 0 hata, DNA sapması yok | KALDI | §4 grep 0 ihlal. Kırık hücreler: Records 768-1100 px (B1), Run faz şeridi 768-1440 px (B2), lg altı tüm sayfalarda alt çubuk (B3), New-record hata özeti (B4). Konsolda `favicon.ico` 404 (B7) |
| G4 | Operatör ekranları 360 px yatay kaydırmasız, birincil düğmeler 44 px, durumlar metinle | GEÇTİ | TK4 kanıtı; 360 px'te faz şeridi tek kolon ve rozetler tam metin (`.qa/shots/fail-run-strip-390.png`). Hata özeti bağlantıları birincil düğme değil, B4'te ayrıca |
| G5 | Antiforgery token'sız POST 400 | Uygulanmaz | Faz T'de durum değiştiren uç nokta yok. Prototip action'ları her metodu kabul ediyor: token'sız `POST /prototype/new-record` Development'ta 200 ("istek işlenmedi" bildirimi). Formlar token üretiyor ama doğrulama yok (B15, Faz 1'e hazırlık) |
| G6 | Görünür metin ve dokümanda em/en-dash 0 | GEÇTİ | `Views`, `Dna.md`, iki devir notu: 0. 420 hücrede `innerText` içinde 0 |
| G7 | Migration `Up()` içinde Drop yok | Uygulanmaz | `Migrations/` yok, `Program.cs` içinde `Migrate` 0, `DbChanges.md` indeksi boş |
| G8 | Süre aritmetiği yalnız IDurationCalculator | Uygulanmaz | Faz 4 ve sonrası |
| G9 | CommandId idempotens | Uygulanmaz | Faz 4 ve sonrası |
| G10 | SuperAdmin 403 | Uygulanmaz | Faz 2 ve sonrası. Menü varyantı kontrol edildi: `?demo=SuperAdmin` menüsünde yalnız Kullanıcılar ve Sistem ayarları (`.qa/shots/mywork-superadmin-1440.png`) |

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`--no-incremental`); `dotnet publish -c Release -o .qa/build` başarılı |
| CSS | `npm run build:css` | Başarılı; çıktı depodaki derlenmiş CSS ile aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme (policy grep 0, sihirli rol grep 0) |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: `Authorization/Roles.cs`, `Routing/KebabCaseParameterTransformer.cs`, `Controllers/PrototypeController.cs`, `Styles/BuildAssets.mjs`, `wwwroot/js/ShellNav.js` (`class ShellNav`), `Views/Shared/_PhaseStrip.cshtml`, `wwwroot/fonts/HankenGrotesk-LatinExt.woff2`; JS fonksiyon ve değişkenler camelCase, CSS sınıfları kebab-case. Tablo/kolon yok |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (`transition-all`, `outline-none`, `<img`, `user-scalable`, `onclick`, CDN, dash, `h-screen`, jquery/gsap/aos, görünümde ham hex ve slate/gray/zinc sınıfı: 0; `...` yalnız JS spread; tüm görünür `<input>` `autocomplete` ya da `inputmode` taşıyor; tek animasyon `prefers-reduced-motion` ile kapanıyor; `backdrop-blur` yalnız üst çubukta) |
| Mekanik eval | `UiDelivery.md §3` | 420 hücre: `h1` 1, `skip` true, `imgNoAlt` 0, `inputNoLabel` 0, `emdash` false, `extScripts` 0, `dupIds` 0, üst çubuk 64 px, mobil gövde 16 px, font `Hanken Grotesk`. Sapmalar: `iconBtnNoName` her sayfada 1 (B10), `hScroll` true Records 768/1024/800x360 (B1), `smallTargets` 4 New-record `?state=error` tüm hücrelerde (B4) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Migration yok, indeks boş; `Program.cs` içinde `Migrate()` yok |
| Yasak mimari | `grep MediatR\|AutoMapper\|IRepository<\|UnitOfWork` | 0 |
| Üslup §8 | emoji, `#region`, süs yorum, `Helper/Utils/Manager/Processor/DataService` adı, pazarlama dili | 0 eşleşme. Kaynakta yalnız iki yorum var (`ShellNav.js:28`, `ThemeToggle.js:15`), ikisi de "neden" yorumu |
| Süre formülü §7 | süre hesaplayan C# metodu | Yok (Faz T). `LiveCounter.js` yalnız sunucu değerinden gösterimi ilerletiyor |
| Zaman giriş alanı (NFR-01) | `grep type="(time\|date\|datetime-local)"` | 0 |
| Production yüzeyi | `curl` 5101 | `/prototype/*` 404, `/error` 500, `/error/403` 403, `/error/404` 404; `/package.json`, `/appsettings.json` 404; varlıklar 200 (`font/woff2`, `image/svg+xml`) |

## 4. Yetki matrisi

Faz T'de kimlik doğrulama yok; tüm route'lar `[AllowAnonymous]`. Rol kolonları uygulanmaz; kabuk menüsü `?demo=` ile çizilir.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| `GET /prototype/my-work`, `/run`, `/new-record`, `/records`, `/dashboard` (Development) | 200 (beklenen 200) | uygulanmaz | uygulanmaz | uygulanmaz |
| Aynı beş route (Production) | 404 (beklenen 404) | uygulanmaz | uygulanmaz | uygulanmaz |
| `POST /prototype/new-record` token'sız (Development) | 200, işlem yok (B15) | uygulanmaz | uygulanmaz | uygulanmaz |
| `GET /error`, `/error/403`, `/error/404` | 500, 403, 404 (beklenen) | uygulanmaz | uygulanmaz | uygulanmaz |

## 5. Responsive ve görsel tur

Her hücrede §3 eval ve kontrast ölçümü 28 URL üzerinde çalıştı (26 prototip URL + `/error/403`, `/error/404`). Mobil yatay 800x360, %200 zoom 320x640 (açık) ve 720x450 (koyu), reduced-motion 390x844 (açık) ve 1440x900 (koyu).

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| İşlerim (4 durum) | KALDI (B3) | KALDI (B3) | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ (B3 dışında) | KALDI (B6, B8) |
| Yürütme (6 sahne) | KALDI (B3) | KALDI (B2, B3) | KALDI (B2) | KALDI (B2) | GEÇTİ (sözlükte B2) | GEÇTİ | GEÇTİ | KALDI (B2 720 px) | KALDI (B6, B8) |
| Yeni kayıt (5 durum) | KALDI (B3, B4) | KALDI (B3, B4) | KALDI (B4) | KALDI (B4) | KALDI (B4) | GEÇTİ | GEÇTİ | KALDI (B4) | KALDI (B6, B8) |
| Kayıtlar (4 durum + sayfa 2) | KALDI (B3) | KALDI (B1, B3) | KALDI (B1) | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ (320 kart görünümü) | KALDI (B6, B8) |
| Pano (4 durum, Yönetici) | KALDI (B3) | KALDI (B3) | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | KALDI (B6, B8) |
| 403 / 404 | KALDI (B3) | KALDI (B3) | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ | GEÇTİ |

Klavye turu ayrıntısı: ilk Tab atlama bağlantısı, Enter ile odak `main`; odak halkası her öğede `outline: 3px solid` görünür; 1440'ta yapışkan üst bar odağı örtmüyor. Ray "Daralt" Enter ile `aria-expanded` true/false değişiyor, genişlik 72 px, etiketler 1x1 erişilebilir metin olarak kalıyor. Çekmece: Menü Enter ile açılır, `aria-expanded="true"`, `.shell` ve `.bottom-bar` `inert`, odak "Menüyü kapat"a gider; Esc kapatır, `aria-expanded="false"`, odak Menü bağlantısına döner; kapat düğmesi de odağı geri verir. Odak tuzağı yok (B6). Boş form gönderimi (New-record) URL değişmeden 5 satır içi hata ve `aria-invalid` üretir, odak ilk hatalı alana (`#machine`) gider. 390 px'te odaklanan "Kaydı aç" ve "Vazgeç" 56 px'in 30 px'i alt çubuğun altında kalıyor (B8).

1920'de içerik `max-w-page` (1600 px) ile sınırlı. Konsol: yalnız `favicon.ico` 404 (B7) ve hata sayfalarının kendi 403/404 durumu (beklenen); Run'daki demo PIN alanı için Chromium `VERBOSE` notu (hata ya da uyarı değil).

**Özgünlük kapısı (`DesignDna.md §5`)**

1. Kart dolu ve gerekçeli, kadranlar yazılı: UYGUN.
2. Kabuk prototipi (gezinme + üst bar + pano + tablo + form) Türkçe, alana uygun demo veriyle çalışıyor (BR-08 biçiminde kayıt no, makine kodları, saha defteri referansı, lot ve SKT): UYGUN.
3. Görüntüler (15 tam sayfa + 11 varyant) incelendi. AdminLTE, CoreUI, Tailwind UI ya da shadcn varsayılanıyla aynı iskelet değil: köşeler 2 px keskin, 2 px çerçeve, gölge yok, kayıt no ve sayaçlar mono, panoda tek özet şeridi (bir büyük, iki küçük sayı, eşit genişlikte değil), faz şeridi (6 px durum çizgili hücreler, kesikli çerçeveli kilitli adımlar) ve mini sürümü İşlerim ile panoda. Sol ray + üst bar arketipi yaygındır ama görsel dil (fiş/etiket) şablon görünümü vermiyor. Sonuç: GEÇTİ.
4. Kullanıcı onayı: bekliyor (TK8).

**Anti-slop taraması (`DesignDna.md §4`)**: neon/glow 0 (tek `box-shadow` adım çubuğu için `inset`), saf siyah/beyaz token yok, gradient 0, cam yalnız üst çubukta (1 yüzey), dört eşit KPI yok, satırlarda renkli nokta yok (yalnız çalışan adımın canlı noktası), bölüm üstü eyebrow yok (büyük harfli küçük metinler alan etiketi ve menü grup başlığı), sürüm rozeti ve jenerik karşılama yok, uydurma kusursuz sayı yok, jenerik isim (John Doe, Acme) ve lorem 0, ikonlar tek aile (Lucide 1.72.0 sprite), grafik veriden üretilen SVG (`_BarChart.cshtml`), emoji 0, em/en-dash 0. Aynı niyetli iki etiket: 1 (B12).

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Uygulanan `body` fontu Hanken Grotesk, başlıklar Archivo dar (`font-stretch 88%`), sayılar JetBrains Mono; altı woff2 self-host ve yüklü.
- Palet: UYGUN. `_Theme.cshtml` değerleri kartla aynı (accent `33 82 160` = `#2152A0`, koyu `122 170 235` = `#7AAAEB`); kontrast ölçümü 0 ihlal.
- Köşe ölçeği: UYGUN. `--radius-ui` ve `--radius-card` 2 px; `rounded-full` yalnız canlı nokta ve PIN hane noktası.
- Gezinme arketipi: SAPMA. lg+ daraltılabilir ray UYGUN; lg altı alt çubuk ekranın altına oturmuyor, 64 px yukarıda yüzüyor (B3). Kart "Kullanıcı değiştir"i mobilde çekmeceye koyuyor, TK6 bunu 390 px'te görünür istiyor (B5).
- Sayfa iskeleti: UYGUN. Ana kolon + sağ bağlam rayı (Run, New-record), panoda özet şeridi + asimetrik iki kolon, tablo ekranında filtre paneli + tek tablo.
- Veri dili: SAPMA. Tek rozet biçimi, zebra yok, 768 altı satır kartı, üç durum bileşeni UYGUN; ancak 768-1100 px'te tablo sayfayı yatay kaydırıyor (B1) ve rozet metni faz şeridinde kesiliyor (B2), kart "durum metinle ayrılır" diyor.
- Hareket imzası: UYGUN. Yalnız renk geçişi 150 ms, atlama bağlantısı `transform`, canlı nokta 1.6 sn ve mola açıkken sabit; reduced-motion hepsini kapatıyor.
- İmza bileşen (kayıt şeridi): SAPMA. Mola sahnesinde sayaç gri ve "Sayaç durdu, mola açık", adım "Devam ediyor" kalıyor, "Adımı bitir" nedeniyle devre dışı (K-39) UYGUN; 768-1440 px'te hücre rozetleri kesik, 1024'te "Dezenfek/siyon" kelime ortasından bölünüyor (B2).

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | YÜKSEK | Kayıtlar tablosu 768-1100 px aralığında sayfayı yatay kaydırıyor; son kolonlar (Net süre, Efor) kesik, kayıt no 4 satıra bölünüyor | 768: `scrollWidth` 865, 1024: 1113, 1100: 1113 (tablo 839 px, sarmalayıcı 716-800 px); 800x360 yatayda da `hScroll` true. Sarmalayıcı `overflow-x-auto md:overflow-visible` (`Views/Prototype/Records.cshtml:102`). `Docs/Delivery/Evidence/Faz-T/Bulgu-Records-Tasma-1024.png`, `.qa/shots/fail-records-768.png` | frontend-engineer | md-xl arasında tablo kart görünümünde kalsın ya da sarmalayıcı yatay kaydırmayı kendi içinde tutsun; kayıt no kelime içinde bölünmesin | KAPANDI (tur 2) |
| B2 | YÜKSEK | Faz şeridi (imza bileşen) rozet metinleri kesiliyor: 1024 ve 1280 px'te dört rozet "Ta…", "De…", "Be…"; 768 ve 1440 px'te "Tamamlandı - sü…". Tamamlandı ile Tamamlandı - süre altında metinle ayrılmıyor (TK5, NFR-05). Run durum sözlüğünde de "Tamamlandı - süre altında" 390 ve 1440 px'te kesik | `.badge > span { truncate }` (`Styles/app.css:302-304`), `md:grid-cols-4` (`Styles/app.css:367-368`). Ölçüm `scrollWidth > clientWidth`: Run'ın altı sahnesinde 768/1024/1280/1440. `Docs/Delivery/Evidence/Faz-T/Bulgu-Run-FazSeridi-1024.png`, `Run-1440.png` | frontend-engineer | Şerit hücresinde rozet satır kaydırsın (kesme yok) ya da şerit md-xl arasında iki kolona insin; sözlük satırındaki 11rem kolonu rozeti tam göstersin | KAPANDI (tur 2) |
| B3 | YÜKSEK | lg altı tüm sayfalarda alt çubuk ekranın altına oturmuyor, 64 px yukarıda duruyor; altında içerik görünüyor, sayfa sonunda son satır çubuğun arkasında kalıyor | Hesaplanan `bottom: 64px`. Kök neden: `tailwind.config.js:44` spacing anahtarı `bar`, Tailwind'in `bottom-bar` yardımcı sınıfını üretiyor (`.bottom-bar{bottom:var(--bottom-bar)}`), bileşen sınıfı `.bottom-bar` (`Styles/app.css:190-194`) ile çakışıyor. 360: çubuk 672-736 / görünüm 800; 390: 716-780 / 844; 768: 896-960 / 1024. `.qa/shots/fail-bottombar-gap-390.png` | frontend-engineer | Spacing anahtarı ya da bileşen sınıfı yeniden adlandırılsın, çakışma kalmasın | KAPANDI (tur 2) |
| B4 | YÜKSEK | Yeni kayıt hata özetindeki dört bağlantı (Makine, Temizlik tipi, Lot, PIN) 18 px yüksekliğinde; saha formunda eldivenle hedef (UiDelivery §3: 24 px altı YÜKSEK, NFR-04) | `smallTargets` 4: `a[Makine] 45x18`, `a[Temizlik tipi] 73x18`, `a[Lot] 20x18`, `a[PIN] 21x18`, tüm genişliklerde. `Views/Prototype/NewRecord.cshtml:79-84`. `.qa/shots/newrecord-error-390.png` | frontend-engineer | Bağlantılar en az 44 px yükseklikte blok hedef olsun | KAPANDI (tur 2) |
| B5 | YÜKSEK | "Kullanıcı değiştir" 360, 390 ve 768 px'te ekranda görünmüyor, yalnız Menü çekmecesinde (TK6; Faz 1 K11 "tek dokunuşla") | `[data-switch-user]` görünür sayısı: 360/390/768 = 0, 1024/1440 = 1. Ad ve sicil no üst çubukta görünüyor. `Views/Shared/_Layout.cshtml:139-164` üst çubukta düğme yok; `Dna.md:32` mobilde çekmeceye koyuyor | frontend-engineer | Üst çubukta ya da alt çubukta lg altı görünür bir "Kullanıcı değiştir" hedefi; DNA kartı TK6 ile çelişiyorsa analistin kararıyla biri güncellensin | KAPANDI (tur 2) |
| B6 | ORTA | Çekmece açıkken odak tuzağı yok: son öğeden sonra Tab odağı belgeye, ardından çekmece dışındaki "İçeriğe geç" bağlantısına taşıyor (`aria-modal="true"` iletişim kutusu) | 390 px Tab izi: 4 "Kullanıcı değiştir" (çekmece) -> 5 BODY -> 6 "İçeriğe geç" (`inDrawer=false`) -> 7 "Menüyü kapat". `wwwroot/js/ShellNav.js:10` ve `:64` yalnız `.shell` ve `.bottom-bar`'ı `inert` yapıyor; `.skip-link` body çocuğu | frontend-engineer | Atlama bağlantısı da `inert` olsun ya da Tab/Shift+Tab çekmece içinde dönsün | KAPANDI (tur 2) |
| B7 | ORTA | Konsolda hata: `favicon.ico` 404, her yeni tarayıcı oturumunun ilk sayfasında | `.qa/.playwright-cli/console-2026-11-13T19-44-57-628Z.log`: `[ERROR] Failed to load resource: 404 @ /favicon.ico`. `_Layout.cshtml` içinde `rel="icon"` yok | frontend-engineer | Self-host favicon ve `<link rel="icon">` | KAPANDI (tur 2) |
| B8 | ORTA | 390 px'te Tab ile odaklanan öğe sabit alt çubuğun altında kalıyor; `scroll-padding-bottom` yok (yalnız `scroll-padding-top: 5rem`) | New-record 390: "Kaydı aç" ve "Vazgeç" 690-746, çubuk 716-780, 56 px'in 30 px'i örtülü. `Styles/app.css:59` | frontend-engineer | lg altında `scroll-padding-bottom` alt çubuk yüksekliği kadar; B3 düzelince yeniden ölçülmeli | KAPANDI (tur 2) |
| B9 | ORTA | Yürütme ekranında "boş" durum yok (açık işi olmayan operatör "Açık iş"e girince); UiRules §3 dört durum | Run sahneleri: devam ediyor, adımlar arası, mola açık, Açıldı, yükleniyor, hata; `Views/Prototype/Run.cshtml:108-130` içinde `"empty"` `_State` yok. Diğer dört sayfada dört durum var | frontend-engineer | Run'a "Açık işiniz yok" boş durumu ve "Yeni kayıt aç" eylemi | KAPANDI (tur 2) |
| B10 | DÜŞÜK | Mekanik eval `iconBtnNoName` her sayfada 1: metinsiz ve adsız çekmece örtüsü bağlantısı | `<a class="drawer-scrim" href="#main" tabindex="-1" aria-hidden="true" data-drawer-close>` (`Views/Shared/_Layout.cshtml:218`). `aria-hidden` ve odaklanamaz olduğu için ekran okuyucu etkisi yok | frontend-engineer | Örtü `button` ya da `div` + JS olsun ya da eval beklentisine uyacak biçimde ad alsın | KAPANDI (tur 2) |
| B11 | DÜŞÜK | lg altı üst çubuktaki marka bağlantısı 40x40 px; DNA "tüm düğme ve alanlar en az 44 px" | eval: `a.brand-mark 40x40` 360/390/768/800/720 hücrelerinde (`_Layout.cshtml:140`, `Styles/app.css` `.brand-mark h-10 w-10`) | frontend-engineer | Bağlantı hedefi 44 px | KAPANDI (tur 2) |
| B12 | DÜŞÜK | Aynı niyetli iki etiket: filtre panelinde "Temizle", boş durumda "Filtreleri temizle"; ikisi de `/prototype/records` (`DesignDna.md §4` kilit) | `Views/Prototype/Records.cshtml:70` ve `:93` | frontend-engineer | Tek etiket | KAPANDI (tur 2) |
| B13 | DÜŞÜK | Pano "Son 7 gün" çubuklarında değer etiketi iki satıra bölünüyor ("10 (1" / "acil)"), 390, 768 ve 1440 px'te | `_BarChart.cshtml` `grid-cols-[4.25rem_minmax(0,1fr)_5.5rem]`; `Dashboard-1440.png` | frontend-engineer | Değer kolonu metne göre genişlesin ya da "acil" ayrı satır olarak tasarlansın | KAPANDI (tur 2) |
| B14 | DÜŞÜK | Süre metinleri birimleriyle mono yazıldığı için "1  sa  04  dk", "Net Başlamadı" geniş aralıkla görünüyor; Süper admin rol etiketi rayda "Süper ad…" kesik | `MyWork-390.png` (Bugün listesi), `.qa/shots/mywork-superadmin-1440.png` | frontend-engineer | Yalnız rakam mono, birim sans; rol etiketi kesilmesin | Rol etiketi KAPANDI (tur 2); süre biçimi ERTELENDİ (Faz 4, PL-21) |
| B15 | DÜŞÜK | Prototip action'ları her HTTP metodunu kabul ediyor ve antiforgery doğrulaması yok; genel `AutoValidateAntiforgeryToken` filtresi eklenmemiş (G5 Faz 1'de ilk POST ile zorunlu) | `curl -X POST /prototype/new-record -d x=1` -> 200 (Development). `Program.cs` içinde antiforgery filtresi yok; `PrototypeController.cs` action'larında metot niteliği yok | backend-engineer | Faz 1 ile birlikte genel antiforgery filtresi; prototip action'ları `[HttpGet]` ya da POST'u doğrulayan biçim | ERTELENDİ (Faz 1, PL-21) |
| B16 | DÜŞÜK | Production hata sayfaları (404/403) kabuğu demo kişiyle çiziyor: "Murat Çelik, Sicil 30517", "Prototip sayfası" notu ve Production'da 404 dönen `/prototype/*` bağlantıları | `curl :5101/error/404`: `person-name` "Murat Çelik" x3, `Sicil 30517` x3, `href="/prototype/run?demo=Operator"` x3 | frontend-engineer | Faz 1 K14 ile gerçek kimliğe geçerken oturumsuz Production görünümünde demo kişi ve prototip bağlantıları çizilmesin | ERTELENDİ (Faz 1, PL-21) |
| B17 | DÜŞÜK | sm altında bağlantı çipi çevrimiçi durumda erişilebilir metin taşımıyor ("Bağlı" `hidden sm:inline`, ikon `aria-hidden`); `role="status"` boş okunuyor. Çevrimdışı metni görünür | `Views/Shared/_Layout.cshtml:150-154` | frontend-engineer | Gizli ama okunur metin (`sr-only`) | KAPANDI (tur 2) |
| B18 | YÜKSEK | (tur 2) Yürütme ekranında çalışan ve sıradaki adımın eylem düğmeleri ("Adımı bitir", "Mola ver", "Adımı başlat") 1024-1079 px aralığında adım kartının (`li.step`) dışına taşıyor; 1024'te 56 px dışarıda, bağlam rayı kolonuna giriyor, düğme 126x76 px ve "Adımı / bitir" iki satır | Ayrıntı ve ölçüm: "Tur 2" bölümü, B18. `.qa/t2/ev/B18-Run-1024.png`, `.qa/t2/shots/Run-1024.png` | frontend-engineer | Adım eylem ızgarası adım gövdesinin dar olduğu genişlikte tek kolona insin ya da kolonlar kart içinde kalsın; 1024-1279 aralığı yeniden ölçülsün | KAPANDI (tur 3) |
| B19 | ORTA | (tur 2) `Dna.md` veri dili satırı "768 px altında satır kartına dönüşür" diyor; uygulama B1 düzeltmesiyle 1280 px altında kart, 1280 ve üstünde tablo | Ayrıntı: "Tur 2" bölümü, B19. `Docs/Design/Dna.md:34` | frontend-engineer | Kart satırı uygulamadaki kırılımı (xl, 1280 px) yazsın; TK8 onayı güncel kartla istensin | KAPANDI (tur 3) |
| B20 | DÜŞÜK | (tur 3) Mola panelindeki "Molayı bitir" düğmesi 1024-1080 px aralığında daralıyor: 108-164 px genişlik, 76 px yükseklik, metin iki satır ("Molayı / bitir"). Panel çerçevesi içinde kalıyor, PIN alanıyla örtüşme yok; B18'deki eski ızgara kalıbı bu formda duruyor | `.qa/t3/endbreak.out`: 1024 108x76 L2, 1040 124x76, 1060 144x76, 1079 163x76, 1080 164x76; 1000 ve 1100+ 56 px tek satır; açık ve koyu aynı. `Views/Prototype/Run.cshtml:189` `sm:grid-cols-[minmax(0,14rem)_minmax(0,1fr)]`. `.qa/t3/ev/Run-1079-Break.png` | frontend-engineer | Mola formu da `.step-action` ile aynı akışkan ızgarayı kullansın (56 px, tek satır) | AÇIK |
| B21 | DÜŞÜK | (tur 3) `Dna.md:34` veri dili satırının iki ayrıntısı uygulamadan farklı: "iki sütunlu ızgara" yazıyor, kart görünümü 768-1279 px'te dört sütun; "satır 56 px" yazıyor, tek satırlık tablo satırı 53 px (1440, 1920), 1280'de en az 65 px | `.qa/t3/b19.out`: 390 ve 767 `gridCols` 2; 768, 1024, 1279 `gridCols` 4; satır yükseklikleri 1440 en az 53, 1920 en az 53, 1280 en az 65. `Styles/app.css:522` `md:max-xl:grid-cols-4` | frontend-engineer | Kart satırı "768 px altında iki, 768-1279 px dört sütun" ve gerçek satır yüksekliğini yazsın ya da uygulama karta uysun | AÇIK |
| B22 | DÜŞÜK | (tur 3) Yürütme kayıt başlığındaki alan ızgarası (`md:grid-cols-4`) iki ray birlikte açıkken çok daralıyor: 1024 px'te kolon 74 px, "SD-2026-0412" üç satıra, "Ürün değişimi temizliği, versiyon 4" beş satıra, "Murat Çelik" iki satıra bölünüyor. Tur 2 görüntüsünde de aynı (gerileme değil) | `.qa/t3/kv.out`: 1024 kolon 74 px, SD 3 satır; 1079-1100 kolon 88-93 px, SD 2 satır; 1279 ve 1280+ tek satır. `Views/Prototype/Run.cshtml:153`. `.qa/t3/ev/Run-1024.png`, `.qa/t2/shots/Run-1024.png` | frontend-engineer | lg-xl arasında alan ızgarası iki kolon kalsın; kimlik değerleri (`SD-...`) bölünmesin | AÇIK |

## 7. Geçenler

- Derleme 0 uyarı / 0 hata, CSS derlemesi depodaki çıktıyla aynı, Release yayını çalışıyor.
- TK1, TK2, TK3, TK4, TK7, TK9; G1, G2, G4, G6.
- Production'da beş prototip 404 (yayın ve `dotnet run --no-launch-profile` ile).
- Rol sabitleri ve rol metni denetimi, policy yasağı, yasak mimari, üslup §8: 0 eşleşme.
- UiDelivery §4 grep tablosu 0 ihlal.
- 420 hücrede kontrast AA (açık ve koyu), font yükleme ve `ğşİı` denetimi, `h1`, atlama bağlantısı, etiketli alanlar, yinelenen id 0, dış betik 0.
- Çevrimdışı modu: gösterge, `role="alert"` şeridi, `data-requires-online` düğmelerinin devre dışı kalması ve geri açılması.
- Çekmece Esc ile kapanır, odak tetikleyiciye döner, `aria-expanded` değişir, arka plan `inert`.
- Boş form gönderimi satır içi hata ve ilk hatalı alana odak.
- PIN bileşeni: doğrulama modunda `maxlength` yok, belirleme modunda `maxlength="6"` ve hane göstergesi, `inputmode="numeric"`.
- Mola sahnesi (K-39): sayaç durur ve metinle söylenir, adım "Devam ediyor" kalır, "Adımı bitir" nedeniyle devre dışı.
- Özgünlük kapısı ve anti-slop taraması.

## 8. Kapsam dışı / yapılamayan

- Yetki matrisi rol kolonları: Faz T'de kimlik doğrulama yok (faz planı "Kapsam dışı: Kimlik doğrulama").
- DB kopyası ve migration denetimi: Faz T'de veritabanı yok.
- `dotnet test`: test projesi yok.
- Gerçek dokunmatik cihaz ve eldivenle deneme yapılmadı; hedef boyutları ölçümle denetlendi.
- %200 zoom tarayıcı yakınlaştırmasıyla değil, eşdeğer CSS genişliğiyle (320 ve 720 px) denendi.
- JS kapalıyken davranış (çekmecenin `#menu` hedefiyle açılması) denenmedi.
- TK8 kullanıcı onayı: testin konusu değil.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | Genel antiforgery filtresi ve G5 denetimi (B15) | backend-engineer | Faz 1'de ilk durum değiştiren POST'lar geliyor |
| A2 | Oturumsuz Production kabuğunda demo kişi ve prototip bağlantıları (B16) | frontend-engineer | Faz 1 K14 gerçek kabuk değişimi |
| A3 | Kullanıcı onayı ve `Decisions.md` kaydı (TK8) | analyst | Faz 1 başlangıç şartı |

## 10. Analist kararı

```
FAZ T: İADE
Tarih: 2026-11-13
Gerekçe: TK5, TK6 ve G3 karşılanmadı; beş YÜKSEK (B1-B5) ve dört ORTA (B6-B9) bulgu açık, QA önerisi KALDI.
         TK8 (kullanıcı onayı) bu kusurlu görüntülerle istenmez; tur 2 görüntüleriyle istenir.
İade maddeleri: B1-B9, B10-B13, B14 (rol etiketi), B17: frontend-engineer. Backend'e bu turda madde yok.
Yeniden denetim şartı: tur 2 (aşağıda).
```

**Kriter eşlemesi.** TK1, TK2, TK3, TK4, TK7, TK9, G1, G2, G4, G6 rapordaki kanıtla karşılanıyor. TK5 (B2), TK6 (B5), G3 (B1-B4, B7)
karşılanmıyor. TK8 kullanıcı onayı bekliyor. G5, G7-G10 bu fazda uygulanmaz; gerekçeler yerinde.

**Analist doğrulaması.** Dört maddeyi kaynakta doğruladım:

- B2: `Styles/app.css:302-304` `.badge > span` üzerinde `truncate`; şerit `md:grid-cols-4` (`:367-368`). Bulgu doğru.
- B3: `tailwind.config.js:44` spacing anahtarı `bar`; derlenmiş `wwwroot/css/app.css` içinde `.bottom-bar{bottom:var(--bottom-bar)}`
  yardımcı sınıfı üretilmiş, `Styles/app.css:190-194` bileşen sınıfıyla aynı ad. Bulgu ve kök neden doğru.
- B4: `Views/Prototype/NewRecord.cshtml:80-83` bağlantılar metin satırı içinde, blok hedef değil. Bulgu doğru.
- B5: `Views/Shared/_Layout.cshtml:139-164` üst çubukta "Kullanıcı değiştir" yok; yalnız ray ve çekmecede. Bulgu doğru.

**B5 kararı (PL-20).** TK6 geçerli, DNA kartı güncellenir. lg altında "Kullanıcı değiştir" çekmece açılmadan görünür ve tek dokunuşla
çalışır. Gerekçe `Docs/Phases/Decisions.md` PL-20'de; TK6 metni PhasePlan'da netleştirildi.

**Frontend-engineer düzeltme listesi (bu fazda kapanır)**

| Bulgu | Şiddet | İstenen sonuç |
|---|---|---|
| B1 | YÜKSEK | Kayıtlar 768-1100 px ve 800x360'ta sayfa yatay kaymaz (`hScroll` false); kayıt no kelime ortasından bölünmez |
| B2 | YÜKSEK | Faz şeridi ve durum sözlüğünde hiçbir rozet metni kesilmez, 360-1920 tüm hücrelerde; "Tamamlandı" ile "Tamamlandı - süre altında" tam metinle ayrılır; "Dezenfeksiyon" kelime ortasından bölünmez |
| B3 | YÜKSEK | Alt çubuk lg altı tüm sayfalarda görünüm altına oturur (çubuğun alt kenarı = görünüm yüksekliği); spacing anahtarı ile bileşen sınıfı ad çakışması kalmaz |
| B4 | YÜKSEK | Hata özeti bağlantıları en az 44 px yükseklikte; `smallTargets` 0 |
| B5 | YÜKSEK | PL-20: lg altı 360, 390, 768 px'te "Kullanıcı değiştir" çekmece açılmadan görünür, görünür metin etiketi taşır (yalnız ikon değil), erişilebilir adı "Kullanıcı değiştir" içerir, hedef en az 44 px, tek dokunuşla tetiklenir, 320 px'te yatay kaydırma yaratmaz. Yer (üst çubuk ya da alt çubuk) frontend'in seçimi. `Docs/Design/Dna.md` gezinme satırı aynı yeri yazar |
| B6 | ORTA | Çekmece açıkken Tab ve Shift+Tab çekmece dışına çıkmaz |
| B7 | ORTA | Konsol 0 hata; self-host favicon ve `<link rel="icon">` |
| B8 | ORTA | lg altında Tab ile odaklanan öğe alt çubuğun altında kalmaz (B3 düzeldikten sonra ölçülür) |
| B9 | ORTA | Yürütme ekranında boş durum sahnesi ("Açık işiniz yok" ve yeni kayıt eylemi) |
| B10 | DÜŞÜK | Çekmece örtüsü `iconBtnNoName` 0 verir. Gerekçe: B6 ile aynı dosya, ayrı tur gerektirmez |
| B11 | DÜŞÜK | Marka bağlantısı en az 44 px. Gerekçe: DNA'nın kendi 44 px kuralı, onaya sunulacak kabukta |
| B12 | DÜŞÜK | Kayıtlar ekranında filtre temizleme tek etiket. Gerekçe: tek satırlık değişiklik, `DesignDna.md §4` kilidi |
| B13 | DÜŞÜK | Pano çubuk değer etiketi 390, 768, 1440 px'te bölünmez. Gerekçe: kullanıcı onayına giden pano görüntüsünde |
| B14 (rol etiketi) | DÜŞÜK | Rayda "Süper admin" rol etiketi kesilmez. Gerekçe: B2 ile aynı kesme sorunu |
| B17 | DÜŞÜK | Bağlantı çipi sm altında çevrimiçiyken okunur metin taşır (`sr-only`). Gerekçe: aynı dosya (`_Layout`), erişilebilirlik |

**Ertelenen DÜŞÜK bulgular**

| Bulgu | Sahibi | Hedef faz | Gerekçe |
|---|---|---|---|
| B14 (süre biçimi: rakam mono, birim sans) | frontend-engineer, backend-engineer | Faz 4 | Süre metni Faz 4'te gelen tek biçimleyiciden gelir; rakam ve birimin ayrı işaretlenmesi o sözleşmeyle birlikte kurulur, prototipte yerel çözüm atılacak iş olur |
| B15 (antiforgery filtresi, prototip action metotları) | backend-engineer | Faz 1 | Faz T'de durum değiştiren uç nokta yok; Faz 1 ilk POST'larla G5'i zorunlu kılıyor (açık kalem A1) |
| B16 (Production hata sayfalarında demo kişi) | frontend-engineer | Faz 1 | Faz 1 K14 prototip kabuğunu gerçek kimlikli kabukla değiştiriyor (açık kalem A2); Production'da prototip yayımlanmadığı için bugün kullanıcıya ulaşan bir yüzey yok |

**Tur 2 yeniden denetim şartı (qa-tester)**

1. B1-B13, B14 (rol etiketi), B17 tek tek yeniden ölçülür; her biri bu bölümdeki "İstenen sonuç" cümlesine göre GEÇTİ ya da KALDI.
2. B3 tüm sayfaların lg altı yerleşimini değiştirdiği için §3 mekanik eval 28 URL üzerinde 360, 390, 768, 800x360 ve 320x640 hücrelerinde
   yeniden çalışır; B8 B3'ten sonra ölçülür.
3. TK5, TK6 (PhasePlan'daki yeni metinle: 360, 390, 768, 1440) ve G3 yeniden işaretlenir; G1, §4 grep, G6 ve kontrast ölçümü
   gerileme için yeniden çalışır.
4. `Docs/Design/Dna.md` gezinme satırının uygulamayla aynı olduğu kontrol edilir (DNA sapması yok).
5. `Docs/Delivery/Evidence/Faz-T/` görüntüleri düzeltilmiş halden yenilenir (en çok 10 dosya); TK8 kullanıcı onayı bu görüntülerle istenir.
6. Sonuç aynı rapora "Tur 2" bölümü olarak eklenir. Tur 2'de YÜKSEK ya da ORTA kalırsa koordinatör durur ve kullanıcıya sorar
   (`Workflow.md §3`). Tur 2'de yalnız DÜŞÜK kalırsa iade sebebi olmaz, gerekçeyle açık kaleme devreder.
7. TESLİM ALINDI kararı tur 2 QA önerisi GEÇTİ ve TK8 kullanıcı onayı birlikte geldiğinde verilir.

**Tur 2 sonucu.** QA önerisi KALDI (B18 YÜKSEK, B19 ORTA). Madde 6 gereği koordinatör kullanıcıya sordu; kullanıcı B18, B19 ve gerileme ile
sınırlı 3. turu seçti (`Decisions.md` PL-22).

**Tur 3 kararı**

```
FAZ T: TESLİM ALINDI
Tarih: 2026-11-13
Gerekçe: TK1-TK9 ve G1, G2, G3, G4, G6 karşılandı; G5, G7-G10 bu fazda uygulanmaz. KRİTİK, YÜKSEK ve ORTA açık bulgu yok,
         QA önerisi GEÇTİ (tur 3). TK8: kullanıcı DNA kartını ve tur 2 görüntülerini 2026-11-13'te onayladı (Decisions.md O-02).
Açık: B20, B21, B22 (DÜŞÜK, yeni) ve PL-21 ile ertelenen B14 süre biçimi, B15, B16; hedef faz ve sahibi aşağıda (PL-23).
Commit şartı: frontend-engineer Docs/Design/Dna.md:21 durum satırını onaylı hale getirir (A3, aşağıda). QA turu gerektirmez.
```

**Kriter eşlemesi (tur 3 itibarıyla)**

| Kriter | Sonuç | Dayanak |
|---|---|---|
| TK1, TK2, TK3, TK4, TK7, TK9 | Karşılandı | Bölüm 2 (tur 1); tur 2 ve tur 3 gerileme ölçümünde bozulma yok (T2.2, T3.3) |
| TK5, TK6 | Karşılandı | T2.1 B2 ve B5, T2.2 |
| TK8 | Karşılandı | Kullanıcı onayı 2026-11-13 (T3.2); `Decisions.md` O-02 bu kararla işlendi |
| G1, G2, G4, G6 | Karşılandı | T3.3 |
| G3 | Karşılandı | T3.1 (B18 KAPANDI, kap taşması 0/390), T3.2 (B19 KAPANDI), T3.3 (§4 grep 0, konsol yalnız beklenen 403/404) |
| G5, G7, G8, G9, G10 | Uygulanmaz | Gerekçeler bölüm 2'de; G5 için B15 Faz 1'e |

**Analist doğrulaması (tur 3).** Altı maddeyi kaynakta ve komutla doğruladım:

- B18: `Styles/app.css:444-449` `.step-action` `grid-cols-[repeat(auto-fit,minmax(min(100%,14rem),1fr))]`, `.step-action .btn`
  `w-full min-w-0 whitespace-nowrap`; derlenmiş `wwwroot/css/app.css` bu kuralı içeriyor (1 eşleşme). Rapordaki değişiklik doğru.
- B19 ve B21: `Dna.md:34` "1279 px ve altında satır kartına dönüşür (1280 px ve üstü tablo)" ve aynı satırda "iki sütunlu ızgara", "satır 56 px";
  `Styles/app.css:522` `max-xl:grid-cols-2 ... md:max-xl:grid-cols-4`. B19 kapalı, B21 doğru.
- B20: mola formu `Views/Prototype/Run.cshtml:190` (raporda `:189`, bir satır kayık) `sm:grid-cols-[minmax(0,14rem)_minmax(0,1fr)]`, `.step-action`
  kullanmıyor. Bulgu doğru.
- B22: `Views/Prototype/Run.cshtml:153` `dl.kv grid-cols-2 md:grid-cols-4`, lg-xl arasında ara kırılım yok. Bulgu doğru.
- G1: `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata. Kanıt klasörü 10 dosya.
- A3: `Dna.md:21` hâlâ "Durum: **KULLANICI ONAYI BEKLİYOR**. Kullanıcı onayı: yok". QA notu doğru.

**A3 kararı (commit öncesi, frontend-engineer).** `Dna.md` Faz 1 ve sonrasının tasarım sözleşmesidir; faz commit'i onu "onay bekliyor" durumuyla
dondurursa TK8 ve "onaysız Faz 1 başlamaz" şartı belgede çelişir, Faz 1'i alan frontend kartı onaysız okur. Bu yüzden satır commit'ten önce
değişir. İstenen: yalnız satır 21, "Durum: **ONAYLANDI** (2026-11-13). Kullanıcı onayı: 2026-11-13, `Docs/Phases/Decisions.md` O-02." anlamında;
kartın başka satırı değişmez (B21 bu commit'e girmez, aşağıda Faz 1). Tek satırlık doküman değişikliği görünüme dokunmadığı için QA turu
gerekmez; koordinatör commit'ten önce `Dna.md` içinde "ONAYI BEKLİYOR" 0 eşleşme ve em/en-dash 0 olduğunu görür. Satır 21 dışında değişiklik
varsa koordinatör analiste döner. Bölüm 9 A3 bununla kapanır.

**Açık kalemler (tur 3 kararı, PL-21 ve PL-23)**

| Bulgu | Sahibi | Hedef faz | Gerekçe ve kapanış şartı |
|---|---|---|---|
| B21 (`Dna.md:34` sütun sayısı ve satır yüksekliği) | frontend-engineer | Faz 1 | Kullanıcının onayladığı kart metni bu commit'te değişmez. Faz 1 ilk gerçek tabloyu (E-03 Kullanıcılar) karttan kurar; kart satırı uygulamayla (768 altı iki, 768-1279 dört sütun; ölçülen satır yüksekliği) ya da uygulama kartla aynı olur. Faz 1 G3 "DNA sapması yok" bunu denetler |
| B22 (Yürütme başlık alan ızgarası 1024-1279 px) | frontend-engineer | Faz 4 | Yürütme ekranı başlığı Faz 4'te `Views/Records/Run.cshtml` olarak gerçek veriyle kurulur (E-15 Açıldı hali); prototipte düzeltme atılacak iş olur. Kapanış: 1024-1279 px'te iki ray açıkken kayıt başlığı değerleri en çok iki satır, `SD-...` ve kayıt no bölünmez |
| B20 (Mola paneli "Molayı bitir" 1024-1080 px) | frontend-engineer | Faz 5 | Mola paneli Faz 5'te (E-15 tam hali, E-18) gerçek formla yazılır. Kapanış: "Molayı bitir" 320-1920 px'te tek satır ve 56 px, adım eylemleriyle aynı ızgara |
| B14 (süre biçimi) | frontend-engineer, backend-engineer | Faz 4 | PL-21, değişmedi |
| B15 (antiforgery, prototip action metotları) | backend-engineer | Faz 1 | PL-21, değişmedi (A1) |
| B16 (Production hata sayfası kabuğu) | frontend-engineer | Faz 1 | PL-21, değişmedi (A2) |

Bu kalemler PhasePlan'da ilgili fazın "Faz T'den devreden" satırına yazıldı; o fazın QA'sı kabul kriteri olarak işaretler.

## Tur 2 (2026-11-13)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `04eb3d8`; `Src/` commit'lenmemiş), devir notu `Docs/Handover/frontend-engineer-20261113-2330.md` |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/build` yeniden alındı; Development 5102, Production 5101; DB: yok |
| QA önerisi | KALDI (B18 YÜKSEK, B19 ORTA açık) |

Ölçümler `.qa/t2/` altındaki betiklerle yapıldı (`s1.js`, `s2.js`, `l1.js`, `l2.js` matris; `strip.js`, `records.js`, `inter.js`, `kbd.js`,
`contain.js`); ham çıktılar aynı klasörde `*.out`.

### T2.1 İade edilen bulguların yeniden ölçümü

| # | İstenen sonuç (bölüm 10) | Ölçüm | Sonuç |
|---|---|---|---|
| B1 | Kayıtlar 768-1100 px ve 800x360'ta `hScroll` false; kayıt no kelime ortasından bölünmez | `records.js`, `records` ve `?page=2`: 320, 360, 390, 768, 800x360, 1024, 1100, 1279 px kart görünümü; 1280, 1440, 1920 tablo. Tüm genişliklerde belge `scrollWidth` = görünüm genişliği, sarmalayıcı `scrollWidth` = `clientWidth`, kesik hücre 0; ilk on kayıt no'nun tümü tek satır. Matris: Records beş URL'de `hScroll` false (20 hücre). 768-1279 kart görünümü göz kontrolü (`.qa/t2/shots/Records-768.png`, `Records-1024.png`, `Records-1279.png`): kart içinde taşma ya da kesik etiket yok | GEÇTİ |
| B2 | Faz şeridi ve durum sözlüğünde 360-1920 hiçbir rozet kesilmez; "Tamamlandı" ile "Tamamlandı - süre altında" tam metin; "Dezenfeksiyon" bölünmez | `strip.js`: Run yedi sahne x 320, 360, 390, 768, 800, 1024, 1100, 1280, 1440, 1920, sözlük (`details`) açık. Her genişlikte kesik rozet 0, kabının dışına taşan rozet 0, kelime ortasından bölünen sözcük 0. Şerit rozetleri "Tamamlandı, Tamamlandı - süre altında, Devam ediyor, Bekliyor" tam metin; sözlükte 27 rozet (kayıt 7, faz 5, adım 5, mola 2, talep 5, tür ve lot 3) tam metin. Şerit 320-390 tek kolon, 768 ve 1024-1440 iki kolon, 800 ve 1920 dört kolon. "Dezenfeksiyon ve montaj" 800, 1024, 1280'de iki satır, kırılım kelime arasında | GEÇTİ |
| B3 | lg altı tüm sayfalarda çubuğun alt kenarı = görünüm yüksekliği; ad çakışması yok | Matris, 29 URL: 360 (736-800 / 800), 390 (780-844 / 844), 768 (960-1024 / 1024), 800x360 (296-360 / 360), 320x640 (576-640 / 640), 720x450 (386-450 / 450); hesaplanan `bottom` 0 px, sapma 0. `tailwind.config.js` içinde `bar` spacing anahtarı yok. `.qa/t2/shots/Viewport-MyWork-390.png` | GEÇTİ |
| B4 | Hata özeti bağlantıları en az 44 px; `smallTargets` 0 | 360, 390, 768, 1440: Makine 45x44, Temizlik tipi 73x44, Lot 44x44, PIN 44x44. `new-record?state=error` 20 hücrede `smallTargets` 0. "Lot" tıklanınca `#lot` odak alır | GEÇTİ |
| B5 | PL-20: 360, 390, 768'de çekmece açılmadan görünür, metin etiketli, ad "Kullanıcı değiştir", 44 px, tek dokunuş, 320'de yatay kaydırma yok | Üst çubukta `a.switch-btn`: 320, 360, 390 92x44; 768 118x44; görünür metin "Kullanıcı değiştir", erişilebilir ağaçta `link "Kullanıcı değiştir"`; çekmece kapalı. Tek tıkla `?demo=Manager`, kişi "Murat Çelik" -> "Selin Aksoy" (320, 360, 390, 768, 1440). 320: `hScroll` false, marka bağlantısı gizli. JS kapalıyken de `href` ile çalışıyor. Not: metin 12 px kalın (`text-xs font-bold`), kontrast AA içinde | GEÇTİ |
| B6 | Çekmece açıkken Tab ve Shift+Tab dışarı çıkmaz | 390 ve 768: 20 Tab ve 20 Shift+Tab, odak dışarıda 0/40; döngü İşlerim > Açık iş > Yeni kayıt > Kullanıcı değiştir > Menüyü kapat > İşlerim. Açıkken `.skip-link` `inert`; Esc sonrası odak "Menü", `aria-expanded` false, `inert` kalkıyor | GEÇTİ |
| B7 | Konsol 0 hata; self-host favicon ve `<link rel="icon">` | Yeni tarayıcı oturumu, ilk sayfa: konsol 0 hata 0 uyarı; istekler `/favicon.svg` 200 `image/svg+xml`, `/favicon.ico` isteği yok (`_Layout.cshtml:102`). Matrisin 580 sayfa yüklemesinde konsol hatası yalnız `/error/404` ve `/error/403` sayfalarının kendi durum kodu (beklenen) | GEÇTİ |
| B8 | lg altında Tab ile odaklanan öğe alt çubuğun altında kalmaz | 360, 390, 768, 800x360, 320x640 x New-record, New-record hata, Run, MyWork, Records, Dashboard: sayfa başına 60 Tab, odaklanan 20-45 öğe, alt ya da üst çubukla örtüşen 0. 390 "Kaydı aç" ve "Vazgeç" 394-450, çubuk üst kenarı 780. `scroll-padding-bottom` lg altı 80 px, lg+ 16 px | GEÇTİ |
| B9 | Yürütmede boş durum ("Açık işiniz yok" ve yeni kayıt eylemi) | `/prototype/run?scene=empty` 200, `h1` 1, "Açık işiniz yok", "Yeni kayıt aç" -> `/prototype/new-record`, 155x56; demo çubuğunda "Boş". Production'da 404 | GEÇTİ |
| B10 | `iconBtnNoName` 0 | 20 hücre x 29 URL: 0 | GEÇTİ |
| B11 | Marka bağlantısı en az 44 px | 360, 390, 768, 1023: 44x44; 359 ve altında gizli (Dna.md ile aynı) | GEÇTİ |
| B12 | Filtre temizleme tek etiket | `records`: "Filtreleri temizle"; `records?state=empty`: iki bağlantı, ikisi de "Filtreleri temizle" -> `/prototype/records` | GEÇTİ |
| B13 | Pano çubuk değeri 390, 768, 1440'ta bölünmez | 390, 768, 1024, 1440, 1920: yedi değerin tümü tek satır ("10 (1 acil)" dahil); çubuklar her genişlikte aynı sol kenar ve genişlikte | GEÇTİ |
| B14 (rol etiketi) | Rayda "Süper admin" kesilmez | `?demo=SuperAdmin` 1024 ve 1440 ray: "Süper admin" `scrollWidth` 79 = `clientWidth` 79; çekmece 390 aynı | GEÇTİ |
| B17 | sm altında çevrimiçiyken okunur metin | 390: "Bağlı" `clip: rect(0 0 0 0)`, 1 px (sr-only); erişilebilir ağaçta `status: Bağlı` | GEÇTİ |

### T2.2 Kabul kriterleri (yeniden işaretlenen ve gerileme)

| # | Sonuç | Kanıt |
|---|---|---|
| TK5 | GEÇTİ | Durum sözlüğü 7 kayıt, 5 faz, 5 adım, 2 mola, 5 talep durumunu tam metinle gösteriyor; şeritte "Tamamlandı" ve "Tamamlandı - süre altında" 320-1920 tam metin (B2). Rozet yalnız `_Badge` partial'ından |
| TK6 (yeni metin) | GEÇTİ | 360, 390, 768: ad, "Sicil N" ve "Kullanıcı değiştir" üst çubukta, çekmece kapalı; 1440: rayda ad, sicil, rol ve "Kullanıcı değiştir" 222x44. Hedef 44 px, metin etiketli, tek tık (B5). Çevrimdışı (390, `setOffline`): çip "Bağlantı yok", şerit görünür, "Adımı bitir" ve "Mola ver" devre dışı, "Kullanıcı değiştir" görünür kalıyor; çevrimiçine dönünce geri açılıyor (`.qa/t2/offline-run-390.png`) |
| TK7 | GEÇTİ | 20 hücre x 29 URL = 580 sayfa: WCAG oranı 4.5:1 (büyük metinde 3:1) altı 0, açık ve koyu. Reduced-motion hücrelerinde canlı nokta `animation-name: none` |
| G1 | GEÇTİ | `dotnet build Src/Vaka.Web -c Release -warnaserror --no-incremental`: 0 Uyarı, 0 Hata. `npm run build:css`: "Done in 509ms", çıktı depodaki `wwwroot/css/app.css` ile bayt bayt aynı (`cmp`) |
| G2 | GEÇTİ | Policy grep 0, sihirli rol grep 0, `Roles.cs` dışında rol adı metni 0, MediatR / AutoMapper / generic repository 0 |
| G3 | KALDI | §4 grep 0 ihlal, konsol 0 hata, matrisin mekanik değerleri temiz; ancak Run 1024-1079 px adım eylem düğmeleri kart dışına taşıyor (B18) ve `Dna.md` veri dili satırı uygulamayla çelişiyor (B19) |
| G6 | GEÇTİ | `Views`, `Styles`, `wwwroot/js`, `Dna.md`, devir notu: em/en-dash 0; 580 sayfanın `innerText`'inde 0 |
| TK8 | Kullanıcı onayı bekliyor | Görüntüler yenilendi (T2.6). B19 kapanmadan kartın veri dili satırı uygulamayı anlatmıyor |

Diğer kriterler (TK1-TK4, TK9, G4) tur 1 sonucunda kalır; gerileme ölçümünde bozulma yok (360 px Run ve MyWork `hScroll` false, `smallTargets` 0).

### T2.3 Mekanik eval (§3), 29 URL

28 tur 1 adresi + `run?scene=empty`. Analistin istediği hücreler: 360 açık/koyu, 390 açık/koyu/reduced-motion, 768 açık/koyu, 800x360, 320x640
açık/koyu. Gerileme ve kontrast için ayrıca 1024 açık/koyu, 1100, 1280, 1440 açık/koyu/reduced-motion, 1920 açık/koyu, 720x450 koyu. Toplam 20 hücre, 580 sayfa.

| Değer | Sonuç (580 sayfa) |
|---|---|
| `h1` 1, `skip` true, `imgNoAlt` 0, `inputNoLabel` 0, `iconBtnNoName` 0, `extScripts` 0, `dupIds` 0 | Sapma 0 |
| `hScroll` false, `overflowing` 0 | Sapma 0 |
| `smallTargets` 0; 44 px altı etkileşimli öğe | 0 ve 0 |
| Kesik metin (gizli taşma), kelime ortasından bölünme | 0 ve 0 |
| Üst çubuk 64 px, mobil gövde 16 px, font Hanken Grotesk, `ğşİı` üç ailede | Sapma 0 |
| Alt çubuk alt kenarı = görünüm yüksekliği (lg altı 7 hücre) | Sapma 0 |
| Kontrast AA (açık ve koyu) | İhlal 0 |
| Konsol | Yalnız `/error/404` 404 ve `/error/403` 403 (beklenen) |

Ek ölçüm (`contain.js`): 13 veri ekranı x 12 genişlik (320-1920); her düğme, bağlantı, rozet, alan ve başlığın çerçeveli kabı içinde kalıp
kalmadığı. Tek sapma B18.

`UiDelivery.md §4` grep tablosu (`Views`, `Styles/app.css`, `wwwroot/js`): 0 ihlal. Üslup §8 (emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager`
sınıf adı): 0; kaynakta iki "neden" yorumu tur 1 ile aynı.

### T2.4 Klavye turu (gerileme)

390 ve 1440: ilk Tab atlama bağlantısı (`outline: 3px solid`), Enter ile odak `#main`; 40 Tab boyunca halkasız odak yok (yalnız döngü sonunda
`BODY`). Boş form gönderimi URL değişmeden 5 `aria-invalid`, odak `#machine`. Ray "Daralt" Enter: `aria-expanded` false, 72 px; tekrar: true, 248 px.
Çekmece B6'da. JS kapalıyken Menü `#menu` hedefiyle çekmeceyi açıyor (`.qa/t2/nojs-menu-390.png`); tur 1'de yapılamayan bu test bu turda yapıldı.

### T2.5 DNA uygunluğu (değişen maddeler)

- Gezinme arketipi: UYGUN. `Dna.md:32` ile uygulama aynı: lg+ daraltılabilir ray (kişi, sicil, "Kullanıcı değiştir"); lg altı alt çubuk ekranın
  altında, 64 px, Menü ile çekmece; üst çubuk her boyutta tek satır 64 px, kişi adı ve sicil, lg altında metin etiketli 44 px "Kullanıcı değiştir",
  359 px ve altında marka bağlantısı gizli, bağlantı göstergesi, tema.
- Veri dili: SAPMA (B19). Rozet tek biçim ve tam metin, zebra yok, kart dönüşümü ve iki sütunlu ızgara UYGUN; kart "768 px altında satır
  kartına dönüşür" diyor, uygulama 1279 px'e kadar kart.
- İmza bileşen (kayıt şeridi): SAPMA (B18). Şerit rozetleri tam metin, kolon sayısı genişliğe göre 1-4, mola sahnesi UYGUN; 1024-1079 px'te
  çalışan adımın 56 px eylem düğmeleri kart dışına taşıyor ve 76 px'e çıkıyor.
- Yazı tipi, palet, köşe, sayfa iskeleti, hareket: değişmedi, UYGUN (ölçüm T2.3).
- Anti-slop (`DesignDna.md §4`): yeni favicon Lucide droplets yolu (ikon ailesiyle aynı); aynı niyetli iki etiket kalmadı (B12); diğer maddeler tur 1 ile aynı, temiz.

### T2.6 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-T/` tur 1 görüntüleri kaldırıldı, düzeltilmiş halden 10 dosya alındı (görünüm sayfa yüksekliğine büyütülerek; sabit
alt çubuk sayfanın altında). Beş sayfa x telefon ve masaüstü, açık ve koyu dönüşümlü; reduced-motion açık (canlı nokta sabit):

| Dosya | Sayfa | Genişlik | Tema |
|---|---|---|---|
| `MyWork-390.png` | İşlerim | 390 | Açık |
| `MyWork-1440-Koyu.png` | İşlerim | 1440 | Koyu |
| `Run-1440.png` | Yürütme | 1440 | Açık |
| `Run-390-Koyu.png` | Yürütme | 390 | Koyu |
| `NewRecord-390.png` | Yeni kayıt | 390 | Açık |
| `NewRecord-1440-Koyu.png` | Yeni kayıt | 1440 | Koyu |
| `Records-1440.png` | Kayıtlar (Yönetici) | 1440 | Açık |
| `Records-390-Koyu.png` | Kayıtlar (Yönetici) | 390 | Koyu |
| `Dashboard-1440.png` | Pano (Yönetici) | 1440 | Açık |
| `Dashboard-390-Koyu.png` | Pano (Yönetici) | 390 | Koyu |

Dosyalar `Read` ile açılıp incelendi. B18 görüntüsü 10 dosya sınırı nedeniyle `.qa/t2/ev/B18-Run-1024.png` altında.

### T2.7 Yeni bulgular

**B18 (YÜKSEK, frontend-engineer).** Yürütme ekranında adım eylem düğmeleri 1024-1079 px aralığında adım kartının dışına taşıyor.

- Ölçüm (`contain.js`; ayrıca 1000-1110 px arası 4 px adımla): "Adımı bitir" sağ kenarı `li.step` sağ kenarından 1024'te +56, 1040'ta +40,
  1060'ta +20, 1080'de 0 px; 1000-1020 ve 1084+ px'te kart içinde. Aralıkta düğme 126x76 px, metin "Adımı / bitir" ve "Mola / ver" iki satır;
  DNA 56 px diyor. Aynı taşma `?scene=between` ("Adımı başlat", "Mola ver") ve `?scene=opened` ("Adımı başlat") sahnelerinde. Sayfa yatay
  kaymıyor (`hScroll` false); tur 1 ve tur 2 matrisi yalnız görünüm taşmasını ölçtüğü için yakalamadı.
- Kaynak: `Styles/app.css:444-446` `.step-action` `sm:grid-cols-[minmax(0,14rem)_minmax(0,1fr)]`, `md:col-start-2`; 1024 px'te sol ray ve bağlam
  rayı birlikte açıkken adım gövdesi kolonu 284 px'e iniyor (`grid-template-columns: 44px 284px`). Devir notuna göre bu kural tur 2'de değişmedi;
  tur 1'de de vardı, tur 1 ölçümünde kap taşması denetlenmediği için kaçtı.
- Neden YÜKSEK: imza bileşenin birincil saha eylemi, 1024 px yatay tablet genişliğinde kart çerçevesini kırıyor (README "kırık yerleşim").
- Kanıt: `.qa/t2/ev/B18-Run-1024.png`, `.qa/t2/shots/Run-1024.png`, `.qa/t2/contain.js`.

**B19 (ORTA, frontend-engineer).** `Docs/Design/Dna.md:34` veri dili satırı "768 px altında **satır kartına dönüşür**" diyor; B1 düzeltmesiyle
uygulama 1279 px'e kadar kart, 1280 ve üstünde tablo (`records.js`: 1279 kart, 1280 tablo). Kullanıcıdan onay istenecek kart (TK8) uygulamayı
anlatmıyor; sonraki fazlar tabloyu karttan okuyup 768 px'ten kurarsa B1 yeniden doğar. G3 "DNA kartından sapma yok" maddesi.

### T2.8 Kapsam dışı / yapılamayan (tur 2)

- Gerçek dokunmatik cihaz ve eldiven: yapılmadı, hedefler ölçümle denetlendi.
- %200 zoom tarayıcı yakınlaştırmasıyla değil, eşdeğer CSS genişliğiyle (320 ve 720 px).
- `.qa/t2/shots/` altındaki tam sayfa görüntülerde sabit alt çubuk ilk görünümün altında duruyor; bu Playwright tam sayfa çekiminin sonucu,
  uygulama hatası değil (`Viewport-MyWork-390.png` ve ölçüm çubuğun ekranın altında olduğunu gösteriyor). Kanıt dosyaları bu yüzden görünüm
  sayfa yüksekliğine büyütülerek alındı.

### T2.9 Sonuç

- İade edilen 16 madde (B1-B13, B14 rol etiketi, B17): 16/16 GEÇTİ, KAPANDI.
- TK5 ve TK6 GEÇTİ; G3 KALDI (B18, B19). Gerileme: G1, G2, G6, TK7 (kontrast) GEÇTİ.
- Yeni: B18 YÜKSEK, B19 ORTA. Ertelenen DÜŞÜK'ler (B14 süre biçimi, B15, B16) PL-21'e göre açık kalemde.
- **QA önerisi: KALDI.** Bölüm 10 madde 6'ya göre tur 2'de YÜKSEK ve ORTA kaldığı için karar kullanıcıya gider.

## Tur 3 (2026-11-13)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `04eb3d8`; `Src/` commit'lenmemiş), devir notu `Docs/Handover/frontend-engineer-20261113-2355.md` |
| Ortam | LOCAL, `dotnet publish -c Release -o .qa/build` yeniden alındı; Development 5102 (PID 34780, test sonunda kapatıldı); DB: yok |
| Kapsam | Kullanıcı onaylı sınırlı yeniden denetim: B18, B19 ve paylaşılan CSS değişikliği nedeniyle gerileme |
| QA önerisi | GEÇTİ (KRİTİK, YÜKSEK ve ORTA açık yok; yeni üç DÜŞÜK: B20, B21, B22) |

Betikler ve ham çıktılar `.qa/t3/` altında (`b18.js`, `contain.js`, `b19.js`, `endbreak.js`, `kv.js`, `evidence.js`; `*.out`). Tur 2 matris
betikleri (`.qa/t2/s1.js`, `s2.js`, `l1.js`, `l2.js`, `records.js`, `sum.mjs`) değiştirilmeden yeniden çalıştırıldı.

### T3.1 B18 (adım eylem düğmeleri)

Değişiklik (devir notu): `Styles/app.css:444-449` `.step-action` `grid-cols-[repeat(auto-fit,minmax(min(100%,14rem),1fr))]`, `.step-action .btn`
`w-full min-w-0 whitespace-nowrap`.

`b18.js`: `/prototype/run`, `?scene=opened`, `?scene=between`, `?scene=break` x 320, 360, 768, 1024, 1040, 1079, 1080, 1280, 1440, 1920 (açık ve koyu,
80 hücre); ek olarak 390, 800, 1100, 1180, 1279 (20 hücre), 1000-1300 px 4 px adımla (304 hücre) ve ray daraltılmış 1024, 1040, 1079, 1080, 1280
(20 hücre). Her hücrede ölçülen: düğmenin `li.step` ve `.step-action` kenarına göre taşması, metin aralığının düğme dışına çıkması, `scrollWidth`
kırpması, metin satır sayısı, yükseklik, düğme ile PIN alanı (`[data-pin]`) kutusunun kesişmesi, belge yatay kaydırması.

| Sahne | Ölçüm (10 istenen genişlik) | Sonuç |
|---|---|---|
| Devam ediyor | "Adımı bitir" 56 px, "Mola ver" 44 px, ikisi de tek satır; sağ kenar kartın 18-22 px içinde. Izgara 320-360 ve 1024-1080 tek kolon (PIN üstte, düğmeler altta), 768, 1280, 1440, 1920 iki kolon. 1024: düğme 284x56 (tur 2: 126x76, kart dışına +56) | GEÇTİ |
| Açıldı (`opened`) | "Adımı başlat" 56 px tek satır, kartın 14-18 px içinde | GEÇTİ |
| Adımlar arası (`between`) | "Adımı başlat" 56 px, "Mola ver" 44 px, tek satır, kart içinde | GEÇTİ |
| Mola açık (`break`) | Devre dışı "Adımı bitir" 56 px tek satır, açıklama (`#break-reason`) kartın 18-22 px içinde | GEÇTİ |

Başarısız hücre: 0 / 444 (istenen 80, ek 364). PIN ile örtüşme 0, metin taşması 0, kırpma 0, `hScroll` 0.

Kap ölçümü (tur 2 `contain.js`, açık ve koyu): 13 veri ekranı (Run sahnelerinden beşi dahil) x 15 genişlik (320, 360, 390, 768, 800, 1024, 1040,
1079, 1080, 1100, 1180, 1279, 1280, 1440, 1920) = her temada 195 hücre. Çerçeveli kabının dışına taşan düğme, bağlantı, rozet, alan ya da başlık:
açık 0, koyu 0 (`.qa/t3/contain.out`).

Göz kontrolü: `.qa/t3/ev/Run-1024.png` (1024 açık), `Run-1024-Koyu-Between.png`, `Run-1079-Break.png`, `Run-1440.png`, `Run-390-Koyu.png` alındı;
`Run-1024.png`, `Run-1079-Break.png` ve `Run-1440.png` `Read` ile açıldı: adım kartı çerçevesi bütün, düğmeler PIN alanının altında ya da yanında, tek satır.

**B18: KAPANDI.**

Aynı turda görülen ayrı sorun (B20, DÜŞÜK): mola panelindeki "Molayı bitir" düğmesi `.step-action` değil, kendi formundaki eski ızgara kalıbını
kullanıyor (`Run.cshtml:189`); 1024-1080 px'te 108-164 px genişliğe ve iki satıra iniyor, panel çerçevesi içinde kalıyor. Bu turun değişikliğinden
kaynaklanmıyor (form ve sınıfları devir notunda değişmemiş).

### T3.2 B19 ve TK8

| Madde | Ölçüm | Sonuç |
|---|---|---|
| B19: `Dna.md:34` kırılımı | Kart satırı "1279 px ve altında satır kartına dönüşür (1280 px ve üstü tablo)". `records.js`: `records` ve `?page=2` 320, 360, 390, 768, 800x360, 1024, 1100, 1279 kart; 1280, 1440, 1920 tablo. Uygulama `Styles/app.css:518-538` `max-xl:` (1280 px) kuralları. Belge `scrollWidth` = görünüm genişliği, kesik hücre 0, çok satırlı kayıt no 0 (22 hücre) | GEÇTİ, **B19 KAPANDI** |
| Aynı satırın diğer ayrıntıları | "iki sütunlu ızgara": 768 altı 2, 768-1279 px 4 sütun. "satır 56 px": tek satırlık tablo satırı 53 px (`b19.out`) | B21 (DÜŞÜK) |
| TK8 | Kullanıcı DNA kartını ve tur 2 görüntülerini 2026-11-13'te onayladı (koordinatör iletisi) | Kullanıcı onayladı |

Not (TK8): `Dna.md:21` durum satırı hâlâ "KULLANICI ONAYI BEKLİYOR. Kullanıcı onayı: yok" ve `Decisions.md` içinde TK8 onay kaydı yok
(`PhasePlan.md:163` "analist onayı `Decisions.md`'ye işler"). Açık kalem A3 kapsamında; analist ve frontend-engineer işi, QA bulgusu değil.

### T3.3 Gerileme

| Kontrol | Komut / ölçüm | Sonuç |
|---|---|---|
| G1 derleme | `dotnet build -c Release -warnaserror --no-incremental` | 0 Uyarı, 0 Hata |
| G1 CSS | `npm run build:css` ("Done in 427ms"), `cmp` depodaki `wwwroot/css/app.css` ile | Bayt bayt aynı; yayındaki `app.css` de aynı |
| Mekanik eval §3, 29 URL | Tur 2 betikleri aynen: 360 açık/koyu, 390 açık/koyu/reduced, 768 açık/koyu, 800x360, 320x640 açık/koyu, 1024 açık/koyu, 1100, 1280, 720x450 koyu, 1440 açık/koyu/reduced, 1920 açık/koyu. 20 hücre, 580 sayfa | `h1`, `skip`, `imgNoAlt`, `inputNoLabel`, `iconBtnNoName`, `emdash`, font ve `ğşİı`, `hScroll`, `overflowing`, `smallTargets`, `extScripts`, üst çubuk 64 px, `dupIds`, kontrast AA, kesik metin, kelime ortası bölünme, alt çubuk konumu, mobil gövde 16 px: her biri 0 sapma |
| Konsol | Oturum günlüğü `.qa/.playwright-cli/console-2026-11-13T20-37-08-493Z.log` | 40 hata, tümü `/error/404` 404 ve `/error/403` 403 (20 hücre x 2, beklenen); başka hata ve uyarı 0 |
| §4 grep (`UiDelivery.md`) | `Views`, `Styles/app.css`, `wwwroot/js` | 0 ihlal. `autocomplete`/`inputmode` taşımayan tek `<input>` `NewRecord.cshtml:120` `type="radio"` (kural metin alanları için; tur 1 ve 2 ile aynı) |
| §4 grep (`CodingStandards.md`) | policy kalıbı, `[Authorize(Roles = "`, `Roles.cs` dışında rol adı | 0, 0, 0 |
| G6 | em/en-dash: `Views`, `Styles/app.css`, `wwwroot/js`, `Dna.md`, tur 3 devir notu; 580 sayfa `innerText` | 0 |
| Üslup §8 | emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager` adı | 0 |

Gerileme yok.

### T3.4 DNA uygunluğu (değişen maddeler)

- İmza bileşen (kayıt şeridi): UYGUN. Çalışan adımda 56 px "Adımı bitir" ve "Mola ver" kart içinde, 320-1920 tek satır (B18). Mola panelindeki
  "Molayı bitir" 1024-1080 px'te iki satır ve 76 px (B20, DÜŞÜK); hedef 44 px üstü, çerçeve içinde.
- Veri dili: UYGUN (kırılım). Kart dönüşümü 1279/1280 kartla aynı (B19). Sütun sayısı ve satır yüksekliği ayrıntısı B21 (DÜŞÜK).
- Diğer maddeler: tur 2 ile aynı; matris ölçümünde değişiklik yok.

### T3.5 Kanıt dosyaları

`Docs/Delivery/Evidence/Faz-T/Run-1440.png` yeni düzenle (PIN ve düğme kolonları eşit genişlikte) yeniden alındı ve `Read` ile incelendi; dosya
sayısı 10. Diğer dokuz dosya tur 2'deki gibi (390 px'te adım ızgarası eskisi gibi tek kolon; diğer sayfalarda `.step-action` yok). 1024 px kanıtı
10 dosya sınırı nedeniyle `.qa/t3/ev/Run-1024.png` altında.

### T3.6 Yeni bulgular

B20, B21, B22; üçü de DÜŞÜK, sahibi frontend-engineer; ayrıntı ve kanıt bölüm 6 tablosunda. Üçü de bu turun değişikliğinden kaynaklanmıyor
(B22 tur 2 görüntüsünde `.qa/t2/shots/Run-1024.png` de var; B20'nin formu `Run.cshtml:189` bu turda değişmedi; B21 tur 2'de UYGUN işaretlenen "iki sütunlu ızgara" ifadesinin 768-1279
aralığında tutmadığını gösteriyor). Erteleme kararı analistin.

### T3.7 Kapsam dışı / yapılamayan (tur 3)

- Tur 3 kapsamı B18, B19 ve gerileme ile sınırlı; klavye turu, çevrimdışı modu ve Production 404 denetimi yeniden yapılmadı (devir notuna göre bu
  turda görünüm, JS ve controller değişmedi; yalnız `app.css` ve `Dna.md`).
- Gerçek dokunmatik cihaz ve %200 tarayıcı yakınlaştırması: tur 1 ve 2'deki gibi ölçüm ve eşdeğer CSS genişliği.

### T3.8 Sonuç

- B18 KAPANDI, B19 KAPANDI. G3 GEÇTİ (§4 grep 0, konsol 0 beklenmeyen hata, kap taşması 0, DNA kartı kırılımı uygulamayla aynı).
- TK8: kullanıcı onayladı (2026-11-13).
- Gerileme: G1, §4 grep, G6, 580 sayfalık mekanik eval ve kontrast temiz.
- Açık: B20, B21, B22 (DÜŞÜK); ertelenen B14 (süre biçimi), B15, B16 (PL-21).
- Kabul kriterleri: TK1-TK9 9/9 (TK8 kullanıcı onayıyla), G1, G2, G3, G4, G6 GEÇTİ (5/5); G5, G7-G10 bu fazda uygulanmaz.
- **QA önerisi: GEÇTİ.**

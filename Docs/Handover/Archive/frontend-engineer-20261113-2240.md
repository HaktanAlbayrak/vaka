DEVİR: frontend-engineer -> qa-tester   Faz: T   Dosya: Docs/Handover/frontend-engineer-20261113-2240.md

Yapıldı:
- `Docs/Design/Dna.md`: ön blok, okuma satırı, kadranlar (VARIANCE 5, MOTION 3, DENSITY 6), sekiz eksenli DNA kartı, kaçınılanlar. Yazı tipi Archivo + Hanken Grotesk + JetBrains Mono (self-host, latin + latin-ext), tek accent mavi `#2152A0`, köşe 2 px keskin, gezinme: lg+ daraltılabilir yan ray, lg altı alt çubuk + çekmece. İmza bileşen: kayıt şeridi.
- Altyapı: `Src/Vaka.Web/package.json` (tailwindcss 3.4.17, fontsource-variable x3, lucide-static 1.72.0; sabit sürüm), `tailwind.config.js` (renkler yalnız token kanallarına eşli, ham renk sınıfı üretilemez), `Styles/app.css`, `Styles/BuildAssets.mjs` (`npm run build:assets`: yazı tiplerini `wwwroot/fonts/` altına, ikon sprite'ını `wwwroot/lib/icons/Icons.svg` olarak üretir), derlenmiş `wwwroot/css/app.css`. `Views/Shared/_Theme.cshtml` açık ve koyu token'lar (RGB kanal), FOUC'suz tema betiği `_Layout` head'inde.
- Kabuk `_Layout`: atlama bağlantısı, ray (daralt/genişlet, tercih saklanır), tek satır 64 px üst çubuk (kişi adı, sicil no, bağlantı göstergesi, tema), çevrimdışı şeridi (`role="alert"`; `data-requires-online` düğmeleri devre dışı), alt çubuk, çekmece (JS yokken `#menu` hedefiyle açılır; JS ile `inert`, Esc, odak iadesi). Menü Operatör, Yönetici ve Süper admin varyantı; yalnız `Roles` sabitleri ve `User.IsInRole`. Kimlik yokken demo kişi: `?demo=<Roles değeri>`; "Kullanıcı değiştir" prototipte sıradaki demo kişiye geçer.
- Beş sayfa: `/prototype/my-work`, `/run`, `/new-record`, `/records`, `/dashboard`. Her biri `?state=` ya da Run için `?scene=` ile dört durumu gösterir (alt kısımdaki "Demo durumu" bağlantıları). Run sahneleri: devam ediyor, adımlar arası (Hazır adım), mola açık, Açıldı (kilit bitişi), yükleniyor, hata. New-record: doğrulama hatası (satır içi + ilk hatalı alana odak), makine kilitli, yükleniyor, boş. Records: 10 satır, 7 kayıt durumunun tümü, uzun makine adı, sayfalama (`?page=`), 768 px altında satır kartı. Dashboard: özet şeridi, sahada listesi, son 7 gün (veriden üretilen SVG çubuk), süre altında kapanan fazlar, talepler, vardiya.
- Ortak bileşenler `Views/Shared/`: `_Badge` (kayıt 7, faz 5, adım 5, mola 2, talep 5 durum + Planlı/Acil + SKT geçmiş; tek sözlük), `_State` (boş/hata), `_PinInput` (+ `wwwroot/js/PinInput.js`; `set` modunda `maxlength` = `PinLength`, `verify` modunda sınır yok; Run sayfasındaki "Durum sözlüğü ve PIN bileşeni" bölümünde iki mod ve tüm rozetler görünür), `_PhaseStrip`, `_PhaseTrack`, `_BarChart`, `_NavList`, `_DemoBar`. Hata görünümleri (`Views/Error/*`) kabuğa uyarlandı.
- JS (vanilla ES modülü, `App.js` girişi): `ThemeToggle`, `ShellNav`, `ConnectionStatus`, `PinInput`, `LiveCounter`, `FormValidation`. JS kapalıyken gezinme, formlar ve sayfalar sunucu tarafında çalışır.

Doğrulama:
- `dotnet build Src/Vaka.Web -warnaserror`: 0 uyarı, 0 hata. `npm run build:css --prefix Src/Vaka.Web`: başarılı. `node --check` yedi JS dosyasında temiz.
- Development, 127.0.0.1:5291: beş sayfa 200 ve tüm durum/sahne varyantları 200 (22 URL); her sayfada `<h1` 1, em/en-dash 0, `<img` 0, dış kaynak 0, `...` 0, yinelenen id 0. CSS, woff2 (`font/woff2`), sprite ve JS dosyaları 200.
- Production (`--no-launch-profile` ile; `dotnet run` launchSettings'in Development değerini ezer): beş prototip 404, `/error/403` 403, `/error` 500.
- UiDelivery §4 grep tablosu kaynak dosyalarda: ihlal 0 (`transition: all`, `outline-none`, `h-screen`, `user-scalable`, `<img`, `onclick`, CDN, jquery/gsap/aos, ham hex ve slate/gray sınıfı yok; `...` yalnız JS spread söz dizimi). Rol adı metni `Views/` altında yalnız `Roles.*` sabitleriyle geçer.
- Kontrast hesabı (WCAG): açık ve koyu temada tüm metin/zemin çiftleri en az 4.5:1 (en düşük 5.2:1, kalınlaştırılmış çerçeve rengi 3.2:1); `Docs/Design/Dna.md`'de özeti var.
- Tarayıcı kullanılmadı (görevim gereği): görsel tur, 360/768/1440 ölçümleri, `document.fonts.check`, çevrimdışı modu, çekmece odak turu ve reduced-motion görünümü qa-tester'a kalır.

DNA uygunluğu (kart maddeleri):
- Yazı tipi: Archivo / Hanken Grotesk / JetBrains Mono, Inter yok, serif yok; `Styles/app.css` @font-face latin ve latin-ext ayrı dosya (ğ ş İ ı latin-ext'te). `body` Hanken Grotesk.
- Palet: tek accent `#2152A0`, anlamsal renkler ayrı, saf siyah/beyaz yok, gradient yok; açık ve koyu aynı token adlarıyla `_Theme.cshtml` içinde.
- Köşe: `--radius-ui` ve `--radius-card` 2 px; tailwind yalnız `rounded-ui`, `rounded-card`, `rounded-full` (yalnız canlı nokta ve PIN hane noktası).
- Gezinme: lg+ daraltılabilir ray, lg altı alt çubuk + çekmece; üst çubuk tek satır 64 px.
- Sayfa iskeleti: ana kolon + sağ bağlam rayı; panoda dört eşit kart yok.
- Veri dili: tek rozet biçimi; tablo 768 altında satır kartı (`data-label`); sayılar mono, sağa hizalı; üç durum bileşeni.
- Hareket: yalnız renk geçişi (150 ms), `transform` ile atlama bağlantısı, canlı nokta `opacity` + `transform`; `prefers-reduced-motion` ile kapanır.
- İmza bileşen: kayıt şeridi `Run` sayfasında; mini sürüm İşlerim ve panoda.
- Kapsam notu: TK5 için Run'da faz durumlarından Bekliyor, Devam ediyor, Tamamlandı, Tamamlandı - süre altında; adım durumlarından Bekliyor, Hazır (adımlar arası sahnesi), Devam ediyor, Tamamlandı; kayıt Açıldı ve Devam ediyor sahneleri görünür. Kesildi, Beklemede, Zaman aşımı, İptal edildi, Yarıda kapatıldı ve talep durumları Run'daki "Durum sözlüğü" bölümünde ve Records / Dashboard / İşlerim listelerinde rozetle görünür.

Sonrakinin bilmesi gerekenler:
- Dosyalar: `Src/Vaka.Web/{package.json,package-lock.json,tailwind.config.js,Styles/,wwwroot/css|fonts|js|lib/,Views/Shared/,Views/Prototype/,Views/Error/}`, `Docs/Design/Dna.md`. `node_modules/` gitignore'da; temiz kurulumda `npm ci --prefix Src/Vaka.Web` sonra `npm run build:assets` ve `build:css` gerekir (çıktılar depoda).
- Demo kancaları: `?demo=Operator|Manager|SuperAdmin` (kabuk rolü), `?state=` (my-work, new-record, records, dashboard), `?scene=` (run). `Views/Prototype/_ViewStart.cshtml` POST isteğinde "istek işlenmedi" bildirimi gösterir (formlar `method="post"` ile aynı adrese gider). `Views/Prototype/` ve `_DemoBar` Faz 1'de gerçek ekranlar gelince de kalır (PL-10), kabuğun demo kişi kolları Faz 1'de gerçek kimlikle değişir.
- Faz 1 için: `_Layout` kimlik varken `User.Identity.Name` ve `User.IsInRole` okur; sicil no şu an demo sabiti. `VakaClaimTypes` claim'i gelince `registryNo` o claim'den okunmalı (frontend yapar). "Kullanıcı değiştir" şu an demo geçişi; Faz 1'de `POST /account/logout` formuna çevrilir.
- Bilinen kısıtlar: canlı sayaç `mm:ss` biçimini JS'te üretir (yalnız görüntü, sunucu değerinden ilerler, SH-14); backend biçimleyicisi geldiğinde sayaç dışı süre metinleri onunla gelmeli. Faz minimumu göstergesi `<progress value max>`; ilerleme sayaçla birlikte JS'te güncellenir, `max` sunucudan gelir.
- Menüde henüz ekranı olmayan bağlantılar `aria-disabled` olarak görünür (başlık "Sonraki fazda"); ilgili faz ekranı gelince `_Layout` içindeki `navItems` listesinde `Href` verilir.

Backend'den beklenen/eksik:
- Sözleşme gerekmedi; Faz T'de ViewModel kullanılmadı, demo veri görünümlerde yerel. Faz 4-6 için görünümlerin beklediği alanlar: faz için `NetDisplay`, `MinimumDisplay`, `NetSeconds`, `MinimumSeconds` (ilerleme göstergesi için tam sayı + metin), adım ve faz için durum anahtarı (`_Badge` anahtarları: `record.*`, `phase.*`, `step.*`, `break.*`, `request.*`), açık molada `BreakStartedByName`, `BreakStartedAtLocal`, `BreakSeconds`, kayıt için `LockExpiresAtLocal` metni, `RecordNo`, `FieldRegisterRef`, sayfalı liste için `Total/Page/PageSize`. Süre metinleri tek biçimleyiciden (`1 sa 04 dk`) gelmeli.
- Rol kısayolu: menü görünürlüğü için ek ViewModel gerekmiyor (`User.IsInRole`).

DB talebi: yok
Açık/ertelenen:
- Özgünlük kapısı ve kullanıcı onayı (TK3, TK8): qa-tester ekran görüntülerini (1440/768/390) alır, analist `Decisions.md`'ye işler. Sahibi: qa-tester, analist.
- Yazı tipi dosyaları (üç aile, altı woff2, toplam yaklaşık 290 KB) tek seferlik yükleme; Archivo iki dosyası (`wdth` ekseni) tek başına yaklaşık 176 KB. Yükleme süresi sorun olursa `wdth` ekseni bırakılıp yalnız `wght` dosyalarına geçilir. Sahibi: frontend-engineer, onay sonrası.

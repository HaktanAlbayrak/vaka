DEVİR: frontend-engineer -> qa-tester   Faz: T (tur 2 düzeltme)   Dosya: Docs/Handover/frontend-engineer-20261113-2330.md

Yapıldı (B1-B13, B14 rol etiketi, B17; tarayıcı kullanılmadı, ölçüm qa-tester'da). Yollar `Src/Vaka.Web/` altında.

| # | Ne değişti | Dosya:satır | QA nasıl ölçer |
|---|---|---|---|
| B1 | `.table-stack` kart yerleşimi md yerine xl altında (768-1279 px dört kolonlu kart, altı iki kolon); tablo yalnız xl+. Sarmalayıcı `xl:overflow-visible`. `.record-no` `break-all` kaldırıldı (yalnız tire yerinden kırılır), tablo hücresinde `xl:whitespace-nowrap` | `Styles/app.css:517-545`, `:296` (`.record-no`); `Views/Prototype/Records.cshtml:102`, `:121` | Records 768, 800, 1024, 1100, 800x360: `scrollWidth <= clientWidth`; kayıt no tek parça (satır sonunda yalnız tireden) |
| B2 | `.badge > span` truncate yok, normal kelime kaydırma. Faz şeridi `repeat(auto-fit, minmax(11rem, 1fr))`, `md:-ml-0.5` örtüşmesi kalktı, `.phase-title` `break-words` yok. Durum sözlüğü satırı mobilde tek kolon, `sm+` 12rem + 1fr | `Styles/app.css:310-314`, `:379-382`, `:406-410`; `Views/Prototype/Run.cshtml:335` | Run (6 sahne + sözlük) 360-1920: her `.badge > span` için `scrollWidth <= clientWidth`; "Tamamlandı" ve "Tamamlandı - süre altında" tam metin; "Dezenfeksiyon" tek satırda kelime bölünmeden |
| B3 | Kök neden: `tailwind.config.js` spacing anahtarı `bar` silindi (kullanılmıyordu); `.bottom-bar` bileşen sınıfı artık `bottom: 0` alır. Derlenmiş CSS'te `.bottom-bar{...bottom:0...}` | `tailwind.config.js:44` (satır silindi); `Styles/app.css:195-199` | 360/390/768 px: çubuğun alt kenarı = `innerHeight`, hesaplanan `bottom` 0 |
| B4 | Hata özeti bağlantıları `inline-flex min-h-11 min-w-11 items-center`, madde işareti kalktı | `Views/Prototype/NewRecord.cshtml:79-84` | New-record `?state=error`: dört bağlantı yükseklik >= 44, `smallTargets` 0 |
| B5 | Seçilen yer: üst çubuk. Yeni `.switch-btn` (metin etiketi "Kullanıcı değiştir" iki satır, `min-h-11`, `lg:hidden`), `href` ile JS yokken de çalışır. 400 px altında ikon gizli, 360 px altında marka bağlantısı gizli (320 px'te sığması için). `Dna.md` gezinme satırı güncellendi | `Views/Shared/_Layout.cshtml:152-155`; `Styles/app.css:159-161`; `Docs/Design/Dna.md:32` | 320, 360, 390, 768: `[data-switch-user]` üst çubukta görünür, yükseklik >= 44, tek tıkla `?demo=` değişir, `hScroll` false; lg+ üst çubukta gizli (ray kullanır) |
| B6 | Atlama bağlantısı da `inert`; `Tab`/`Shift+Tab` çekmece içinde döner (`trapFocus`) | `wwwroot/js/ShellNav.js:10`, `:55-60`, `:62-75` | 390 px çekmece açık, 15+ Tab ve Shift+Tab: `document.activeElement` hep `.drawer` içinde |
| B7 | `favicon.svg` (Lucide droplets yolu, accent zemin) ve `<link rel="icon">` | `wwwroot/favicon.svg`; `_Layout.cshtml:102` | Yeni oturumda ilk sayfa: konsol hatası 0, `/favicon.svg` 200 |
| B8 | `scroll-padding-bottom` lg altında çubuk yüksekliği + güvenli alan + 1rem, lg+ 1rem | `Styles/app.css:60-65` | New-record 390: Tab ile "Kaydı aç" ve "Vazgeç" odaklanınca kutu alt çubuğun üst kenarının üstünde |
| B9 | Run `?scene=empty`: "Açık işiniz yok" boş durumu ve "Yeni kayıt aç" eylemi; demo çubuğuna "Boş" eklendi | `Views/Prototype/Run.cshtml:128-131`, `:358` | `/prototype/run?scene=empty` 200, `h1` 1, eylem `/prototype/new-record`, 44 px üstü |
| B10 | Örtü bağlantısı `aria-label="Menüyü kapat"`, `aria-hidden` kalktı (`tabindex="-1"`) | `_Layout.cshtml:229` | Eval `iconBtnNoName` 0 |
| B11 | Üst çubuk marka bağlantısı `h-11 w-11` | `_Layout.cshtml:146` | `a.brand-mark` 44x44 (lg altı, 359 px üstü) |
| B12 | Boş durumdaki ve filtredeki etiket "Filtreleri temizle" | `Records.cshtml:70`, `:93` | Sayfada tek etiket |
| B13 | Çubuk listesi ortak ızgara (`grid-cols-[4.25rem_minmax(0,1fr)_auto]` + alt ızgara), değer `whitespace-nowrap` | `Views/Shared/_BarChart.cshtml:14`, `:18`, `:25` | Dashboard 390/768/1440: değer etiketi tek satır, çubuklar hizalı |
| B14 | Rol etiketi ayrı satır (`Sicil N` ve rol iki `person-meta`), ray ve çekmecede | `_Layout.cshtml:124-126`, `:249-251` | `?demo=SuperAdmin` 1440 ray: "Süper admin" `scrollWidth <= clientWidth` |
| B17 | Çevrimiçi "Bağlı" metni `max-sm:sr-only` | `_Layout.cshtml:162` | 390 px: `.conn-online` içinde okunur metin "Bağlı" |

DNA uygunluğu (kart maddeleri):
- Gezinme arketipi: UYGUN. lg+ ray; lg altı alt çubuk (ekranın altına oturur) + çekmece, üst çubukta "Kullanıcı değiştir" (Dna.md ile aynı).
- Veri dili: UYGUN. Tek rozet biçimi tam metin; tablo xl altında kart, kayıt no kelime ortasından bölünmez.
- İmza bileşen: UYGUN. Faz şeridi hücre genişliğine göre 1-4 kolon, rozetler kesilmez.
- Diğer maddeler (yazı tipi, palet, köşe, hareket, iskelet): değişmedi.

Öz denetim: `dotnet build -warnaserror` 0 uyarı 0 hata; `npm run build:css` başarılı (wwwroot/css/app.css yenilendi); `node --check` ShellNav.js temiz; Development 127.0.0.1:5331 beş sayfa + `run?scene=empty` + `new-record?state=error` 200, em/en-dash 0, `/favicon.svg` 200, `data-switch-user` Run'da 3 (ray, üst çubuk, çekmece); UiDelivery §4 grep: ihlal 0 (`translate-` eşleşmesi `slate-` kalıbının yanlış pozitifi); Views'ta ham hex 0. Yalnız kendi PID'im kapatıldı.

Sonrakinin bilmesi gerekenler:
- `Styles/app.css` ve bazı görünümler satır sonu LF'e normalize edildi (içerik değişmedi).
- Ertelenenler dokunulmadı: B14 süre biçimi, B15, B16.
- Controller, Program.cs, Routing'e dokunulmadı.

Backend'den beklenen/eksik: yok.
DB talebi: yok
Açık/ertelenen: B14 süre biçimi (Faz 4), B16 (Faz 1). Tur 2 görüntüleri ile TK8 kullanıcı onayı: qa-tester, analist.

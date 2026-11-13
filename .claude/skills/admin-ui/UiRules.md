# UI Kuralları (Razor + Tailwind CLI + vanilla JS)

> Okur: frontend-engineer. Kimlik `Docs/Design/Dna.md` içindedir; bu dosya uygulama standartlarıdır.
> Tam arayüz kural metni: `../web-design-guidelines/command.md`.

## 1. Tema ve Tailwind

- Tek kaynak: `Views/Shared/_Theme.cshtml` CSS değişkenleri (`--font-*`, `--accent`, `--surface`, `--line`, `--muted`, `--radius-ui`, `--radius-card`,
  anlamsal renkler). `tailwind.config.js` bu değişkenlere eşlenir. Görünümlerde ham `slate-*`, `rounded-xl`, hex renk yerine **token sınıfı**.
- Renkler RGB kanal değişkeniyle (`rgb(var(--accent) / <alpha-value>)`) tanımlanır ki `bg-accent/10` çalışsın.
- Açık ve koyu tema aynı token adlarıyla; `color-scheme` `<html>` üstünde; tema seçimi kullanıcı tercihi + `prefers-color-scheme`, FOUC'suz (head içi küçük inline betik).
- Tailwind CLI: `Styles/app.css` -> `wwwroot/css/app.css` (`npm run build:css`, `watch:css`). Derlenmiş CSS depoda. Sınıf adı Razor'da birleştirilmez
  (`text-@(color)-600` derlenmez); tam sınıf adı yazılır ya da safelist'e eklenir.
- Varlıklar self-host: yazı tipleri `wwwroot/fonts/` (woff2, `font-display: swap`, kritik olan `preload`), ikon seti ve grafik kütüphanesi
  `wwwroot/lib/` altında sabit sürümle. `unpkg`, `jsdelivr`, `googleapis`, `@latest` yok.

## 2. Bileşen sistemi

- Tekrarlanan her parça partial ya da ViewComponent'tir: `_StatTile`, `_DataTable`, `_Pagination`, `_EmptyState`, `_Modal`, `_Toast`, `_FormField`, `_Badge`.
  Aynı işaretleme iki görünümde kopyalanmışsa bileşene çıkarılır.
- İkon: tek aile, tek çizgi kalınlığı, self-host; ikon-only buton `aria-label` + ikon `aria-hidden="true"`.
- Grafik: tek kütüphane (self-host, sabit sürüm) ya da veriden üretilen SVG. Grafik renkleri DNA paletinden, eksen/etiket okunur, grafiğin
  metin karşılığı (tablo ya da `aria-label` özeti) var, reduced-motion'da giriş animasyonu kapalı. Div'den sahte grafik yok.
- Kart yalnız gerçek hiyerarşi için; iç içe kart yok. Gruplama için önce boşluk ve ince ayraç.

## 3. Her ekranın dört durumu

Yalnız "dolu ve başarılı" durum teslim edilmez. Her veri ekranında:

| Durum | Kural |
|---|---|
| Yükleniyor | Son düzenin biçiminde iskelet (skeleton); jenerik dönen halka yok. Metin `Yükleniyor…` |
| Boş | Ne olduğunu ve nasıl doldurulacağını söyler, birincil eylem butonu içerir |
| Hata | Sorun + sonraki adım; form hataları alanın altında satır içi, ilk hatalı alana odak |
| Dolu | Uzun metin (`truncate` / `line-clamp` / `break-words`), çok satır (sayfalama), kısa veri ile bozulmuyor |

Yıkıcı eylem (silme, pasife alma) onay penceresi ya da geri al süresi ister. Başarı/hata bildirimi `aria-live="polite"` bölgesinde.

## 4. Formlar ve tablolar

- Etiket alanın **üstünde**, yardım metni altında, hata en altta; placeholder etiket yerine kullanılmaz.
- Doğru `type`, `inputmode`, `autocomplete`, anlamlı `name`; e-posta/kod alanında `spellcheck="false"`; yapıştırma engellenmez.
- Gönder butonu istek başlayana dek etkin, istek sırasında meşgul durumu; buton etiketi özgül ("Kullanıcıyı Kaydet", "Devam" değil).
- Kaydedilmemiş değişiklikle sayfadan çıkışta uyarı.
- Tablolar gerçek `<table>` + `<th scope>`; sayı kolonları sağa hizalı ve `tabular-nums`; yapışkan başlık; sıralama, filtre ve sayfa **URL sorgu
  parametresinde** (yenileyince ve paylaşınca aynı görünüm); tarih/sayı `Intl.*` ya da sunucuda kültüre göre, sabit biçim yok.
- Mobilde tablo: `overflow-x-auto` sarmalayıcı (ilk kolon yapışkan) ya da kart listesine dönüşüm; hangisi olduğu DNA "veri dili" satırında yazılı.

## 5. Responsive (zorunlu taban)

- **Mobile-first:** taban stiller mobil; `sm 640 / md 768 / lg 1024 / xl 1280 / 2xl 1536` ile genişler. Keyfi medya sorgusu yazılmaz.
- Test genişlikleri: **360, 768, 1024, 1440, 1920** (mobilde dikey + yatay). Hiçbirinde yatay kaydırma, kırpılma, çakışma yok.
- Gezinme: `lg` altında çekmece/alt bar (hamburger `aria-expanded`, Esc ile kapanır, odak tuzağı, arka plan `inert`); `lg` üstünde DNA arketipi.
  Üst bar tek satır, en çok 72px.
- Izgara: CSS Grid (`grid-cols-1 md:grid-cols-2 xl:grid-cols-4`), flex yüzde hesabı yok. Her çok kolonlu düzenin `< 768px` karşılığı aynı bileşende açıkça yazılır.
- İçerik 1920'de sınırsız yayılmaz (`max-w-[1600px]` gibi) ya da ek kolonla anlamlı dolar. Tam yükseklik için `min-h-[100dvh]`, `h-screen` değil.
- Dokunma hedefi **en az 44px**; mobilde gövde metni en az 16px; `env(safe-area-inset-*)` tam genişlik çubuklarda.
- %200 yakınlaştırma / 320 CSS px'te içerik yeniden akar, işlev kaybolmaz.
- Flex çocuklarında `min-w-0`; modal/çekmecede `overscroll-behavior: contain`.

## 6. Hareket

Yalnız `transform` + `opacity`; `transition: all` yasak (özellik sayılır); süre 150-250ms, giriş için yumuşak çıkış eğrisi.
Her animasyon `prefers-reduced-motion` ile kapanır; JS kapalıyken içerik görünür. Scroll dinleyicisi yok (IntersectionObserver / CSS).
Sonsuz döngü animasyonu yalnız gerçek "canlı" durum göstergesinde. Her hareketin tek cümlelik gerekçesi DNA "hareket imzası" ile uyumlu.

## 7. Erişilebilirlik

Atlama bağlantısı (`#main`), tek `h1` ve hiyerarşik başlıklar, landmark'lar (`nav`, `main`, `aside`), görünür `:focus-visible` halkası
(`outline-none` yalnız yerine halka varsa), eylem `<button>`, gezinme `<a>`, `<div onclick>` yok, kontrast AA (4.5:1; büyük metin 3:1),
durum yalnız renkle anlatılmaz (ikon ya da metin eşlik eder), yapışkan üst bar odaktaki öğeyi örtmez, tüm işlev klavyeyle yapılır.

## 8. JavaScript

Vanilla ES modülleri, `defer`; jQuery, GSAP, AOS yok. Dosya = tek sorumluluk, PascalCase (`SidebarNav.js`, `DataTable.js`, `ThemeToggle.js`),
`data-*` kancalarıyla bağlanır (CSS sınıfı kanca değildir). Render sırasında layout okuması (`getBoundingClientRect`) yok; okuma/yazma gruplanır.
Konsolda hata ve uyarı 0. İlerleyen geliştirme: JS yüklenmese de gezinme, formlar ve tablolar sunucu tarafında çalışır.

## 9. Öz denetim

`UiDelivery.md §1` hafif tur + `§4` grep tablosu sıfır ihlal olmadan devir notu yazılmaz.

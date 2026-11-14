---
navigation: collapsible-rail         # lg ve üstü; lg altı alt çubuk + menü çekmecesi
layout: main-plus-context-rail       # ana kolon + sağ bağlam rayı; panoda özet şeridi + asimetrik iki kolon
fonts:
  display: Archivo
  sans: Hanken Grotesk
  mono: JetBrains Mono
accent: "#2152A0"                    # koyu temada #7AAAEB
radius:
  ui: 2px
  card: 2px
dials:
  variance: 5
  motion: 3
  density: 6
signature: "Kayıt şeridi (faz ve adım şeridi)"
---

# Tasarım DNA'sı

> Sahibi: frontend-engineer (Faz T). Kurallar: `.claude/skills/admin-ui/DesignDna.md`. Durum: **ONAYLANDI** (2026-11-13). Kullanıcı onayı: 2026-11-13, `Docs/Phases/Decisions.md` O-02.

**Okuma:** Üretim tesisi makine temizlik kayıt paneli, sahada ayakta ve eldivenle tablet ya da telefonla günde birkaç kez kullanan operatör ile masa başında geniş ekranda çalışan yönetici ve denetçi için, net, sakin, güvenilir bir dille, kayıt fişi ve iş emri etiketi ailesi: soğuk gri taban, tek mavi işaret rengi, kalın keskin çerçeveler, mono sayılar.

**Kadranlar:** VARIANCE 5 (veri ekranlarında düzen sabit kalır; kimlik kabukta, başlıkta, kayıt şeridinde ve panoda asimetrik iki kolonda) · MOTION 3 (yalnız geri bildirim: renk geçişi 150 ms ve çalışan adımdaki canlı nokta; süs hareketi yok) · DENSITY 6 (operatör ekranları 5: büyük hedef, az öğe; yönetici tablosu ve panosu 7: bir ekranda on kayıt)

| Eksen | Seçim | Gerekçe |
|---|---|---|
| Yazı tipi üçlüsü | Display **Archivo** (değişken, `font-stretch` 88 %: dar, endüstriyel başlık) · sans **Hanken Grotesk** · mono **JetBrains Mono** (kayıt no, sayaç, süre, PIN). Üçü de self-host (`wwwroot/fonts/`, woff2, latin ve latin-ext), `font-display: swap`; sayılar `tabular-nums` | Archivo dar genişlikte başlığı 360 px'te tek satırda tutar, makine etiketi gibi okunur. Hanken Grotesk küçük boyutta açık harf aralığıyla eldivenle uzaktan bakışta okunur. JetBrains Mono rakamları ayırt edilir (0/O, 1/l), canlı sayaç ve kayıt no için gerekli. ğ ş İ ı latin-ext dosyalarından gelir. Inter ve serif yok. cms-codebase kayıt defterinde bu üçlü yok (Depa Makine yalnız JetBrains Mono'yu mono olarak kullanmış) |
| Palet (taban, accent, anlamsal, grafik) | Açık: taban `#ECF0F2`, yüzey `#F7F9FA`, çukur `#E0E6EA`, mürekkep `#141E28`, soluk metin `#4F5C69`, çizgi `#CAD2D8` / güçlü çizgi `#788794`. Koyu: taban `#0F1419`, yüzey `#171E25`, mürekkep `#E4EAEF`. **Tek accent** mavi `#2152A0` (koyu `#7AAAEB`), doygunluk %66. Anlamsal: başarı `#166E42`, uyarı `#924A06`, hata `#AA2016`, bilgi = accent; her biri açık zemin tonuyla birlikte. Grafik: planlı `#2152A0`, acil `#C4700E`, nötr `#606C78` (en çok 3 renk, mavi/turuncu renk körlüğüne dayanıklı, ayrıca sayı ve metin eşlik eder) | Fabrika zemininde parlak yüzey göz yorar; soğuk gri taban ışığı sakinleştirir, mavi "temiz, doğrulanmış" çağrışımı verir ve uyarı turuncusundan, hata kırmızısından ayrılır. Metin kontrastları ölçüldü: gövde 14.7:1, soluk metin en az 5.4:1; tüm anlamsal metin/zemin çiftleri en az 5.2:1 (açık), 6.0:1 (koyu). Saf siyah/beyaz, gradient, mor, bej+pirinç yok |
| Köşe ölçeği | Keskin: `--radius-ui` 2 px, `--radius-card` 2 px. Kenarlık 2 px | Kayıt fişi ve etiket görünümü; kalın sınır, eldivenle bakışta düğme, alan ve kart sınırını net verir. Yuvarlak köşe "uygulama" hissi verir, burada denetim belgesi hissi aranır |
| Gezinme arketipi (masaüstü / mobil) | Masaüstü (lg+): **daraltılabilir yan ray**, gruplu (İzleme, Tanımlar, Yönetim), altında kişi adı, sicil no, "Kullanıcı değiştir". Mobil ve tablet dikey (lg altı): **alt çubuk** (rolün en çok üç birincil bağlantısı + Menü) ve Menü ile açılan **çekmece** (tüm menü, kişi, kullanıcı değiştir; ikincil yol). Üst çubuk her boyutta tek satır, 64 px: kişi adı ve sicil no, lg altında **"Kullanıcı değiştir" düğmesi** (görünür metin etiketli, en az 44 px, çekmeceye girmeden tek dokunuş; 360 px altında marka bağlantısı yerini ona bırakır), bağlantı göstergesi, tema | Yönetici 10'dan fazla bölüme gider: ray gruplayabilir. Operatör üç yer arasında geçer (İşlerim, Açık iş, Yeni kayıt) ve tek elle, başparmakla çalışır: alt çubuk 64 px yüksekliğinde, bağlantılar 44 px üstü. Menü bağlantısı JS yokken `#menu` hedefiyle çekmeceyi açar |
| Sayfa iskeleti | Saha ve form ekranları: **ana kolon + sağ bağlam rayı** (lg altında rayın içeriği ana kolonun altına iner). Pano: üstte tek bir özet şeridi (bir büyük, iki küçük sayı), altında geniş kolon (şu an sahada, son 7 gün, süre altında kalan fazlar) ve dar bağlam rayı (talepler, vardiya). Tablo ekranı: filtre paneli + tek tablo | Operatörün sorusu "şimdi ne yapacağım": yürütmede ana kolon adımlar, ray kayıt bağlamı (malzeme, görevli, olaylar). Dört eşit KPI kartı yok; şerit tek bileşen ve sayılar eşit değil |
| Veri dili (tablo, rozet, durumlar) | **Tablo:** 2 px güçlü başlık çizgisi, yapışkan başlık (`top-16`), zebra yok, satır arası ince çizgi, sayı kolonları sağa hizalı mono. Yoğunluk orta (1280 px ve üstünde satır en az 56 px, uzun içerikte büyür). 1279 px ve altında **satır kartına dönüşür** (1280 px ve üstü tablo): aynı `<table>` işaretlemesi, `data-label` ile alan etiketi; 768 px altında iki, 768-1279 px arasında dört sütunlu ızgara. **Rozet:** tek biçim, ikon + metin + 2 px kenarlık; durum renkle değil metin ve ikonla ayrılır, ek olarak kenarlık stili (dolu = Devam ediyor, kesikli = Bekliyor, açık zemin = Hazır/Açıldı). Beş durum grubu (kayıt 7, faz 5, adım 5, mola 2, talep 5) `_Badge` içinde tek yerde. **Boş:** kesik çizgili kutu, ne olduğunu ve nasıl dolduğunu söyler, birincil eylem. **Hata:** düz hata kenarlıklı kutu, sorun + sonraki adım. **Yükleniyor:** son düzenin biçiminde sabit iskelet bloklar + "Yükleniyor…" metni | Denetimde kanıt olan kayıt listesinde her satırın sahibi, durumu ve süresi tek bakışta okunmalı. Mobilde yatay kaydırma eldivenle zor, kart dönüşümü tek elle okutur. İskelet bloklar durağan: döngü animasyonu yalnız gerçek canlı göstergede |
| Hareket imzası | Geri bildirim: düğme ve bağlantı rengi 150 ms geçer; atlama bağlantısı `transform` ile iner; çalışan adımın yanındaki canlı nokta `opacity` + `transform` ile 1,6 sn'de atar ve mola açıkken durur. Yalnız `transform` ve `opacity`. `prefers-reduced-motion` hepsini kapatır | Hareket süs değil bilgi: "bu adım şu an sayıyor". Mola açıkken nokta kehribar ve sabit olur |
| İmza bileşen | **Kayıt şeridi** (Yürütme ekranı): fazlar yan yana hücreler (üstte 6 px durum çizgisi, rozet, adım sayısı, `<progress>` ile net süre / faz minimumu, canlı net süre); altında geçerli fazın adımları, her biri numaralı damga, rozet, görevli çipleri. Çalışan adım sol 6 px mavi çubuk, 48 px mono canlı sayaç, PIN alanı ve 56 px "Adımı bitir" / "Mola ver"; sıradaki adım "Hazır", kilitli adımlar kesikli çerçeve ve "Önceki adım bitmeden başlatılamaz". Mola açıkken: üstte mola paneli, sayaçlar gri ve durmuş ("Sayaç durdu, mola açık"), adım "Devam ediyor" kalır, "Adımı bitir" nedeniyle devre dışı. Mini sürümü (`_PhaseTrack`) İşlerim ve panoda | Bu ürünün farkı faz minimumu, adım sırası ve mola kuralının tek ekranda görünmesi. Şerit hem "neredeyim" hem "ne yapabilirim" sorusunu cevaplar; metin karşılığı olmadan renk tek başına anlam taşımaz |

**Ortak bileşenler (Faz T):** `_Badge` (durum sözlüğü), `_State` (boş ve hata), `_PinInput` (iki mod: doğrulama modunda uzunluk sınırı yok; belirleme modunda `maxlength` = sunucudan gelen `PinLength`, hane göstergesi; `inputmode="numeric"`, göster/gizle), `_PhaseStrip`, `_PhaseTrack`, `_BarChart` (veriden üretilen SVG, metin karşılığı satırlarda), `_NavList`, `_Field` (etiket, ipucu, hata; parola alanında göster/gizle), `_FormSummary` (form hata özeti), `_Pagination`, `_AccessDenied` (403 sayfası ve `/error/403` ortak), `_SwitchUser`, `_TopbarTools`, `_DemoBar` (yalnız prototip). Bağlantı göstergesi ve çevrimdışı şeridi kabukta; çevrimdışıyken `data-requires-online` düğmeleri devre dışı kalır.

**Kabuk kipleri (Faz 1):** tam (ray, alt çubuk, çekmece), odak (ilk giriş: yalnız üst çubuk, kişi adı, sicil no ve her boyutta "Kullanıcı değiştir"), genel (oturumsuz giriş, 403 ve hata sayfaları: yalnız marka, bağlantı göstergesi ve tema; demo kişi yok).

**Bilinçli olarak kaçınılanlar:**

- Dört eşit KPI kartı satırı ve altında iki grafik: panoda yerine tek özet şeridi, sahada listesi ve etiketli yatay çubuklar var.
- Yuvarlak köşeli, gölgeli, açık gri-mavi kart yığını (AdminLTE / shadcn varsayılanı): çerçeve 2 px keskin, gölge yok, gruplama boşluk ve ince çizgiyle.
- Mor, neon, gradient ve dış glow: tek mavi accent, anlamsal renkler ayrı.
- Her satırda renkli durum noktası: durum yerine ikon + metinli rozet.
- Yatay kaydırmalı mobil tablo: satır kartına dönüşüm.
- "Hoş geldin" karşılaması, sürüm rozeti, bölüm üstü küçük etiketler: sayfa başlığı doğrudan işi söyler.
- Zaman giriş alanı (NFR-01): sayaçlar ve süreler yalnız sunucudan gelen değerin gösterimidir.
- Küçük dokunma hedefi: tüm düğme ve alanlar en az 44 px, birincil saha eylemleri 56 px.
- Serif, Inter, emoji, elle çizilmiş SVG: ikonlar tek aileden (Lucide 1.72.0, `wwwroot/lib/icons/Icons.svg` sprite'ı `npm run build:assets` ile üretilir).

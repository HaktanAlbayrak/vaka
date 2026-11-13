# Tasarım DNA'sı (admin dashboard)

> Faz T'de frontend-engineer doldurur (`Docs/Design/Dna.md`), kullanıcı onaylar, qa-tester her UI fazında karta göre denetler.
> Amaç: "hazır admin şablonu" görünümünden çıkıp **bu isteğe özgü** bir arayüz üretmek. Şablon görünümüne dönüş = DNA yazılmamış = Faz T geçmez.

## 1. Design Read (koddan önce tek satır)

`Dna.md` başına yaz: **"Okuma: <ürün/alan> yönetim paneli, <kullanıcı rolü ve kullanım sıklığı> için, <his> bir dille, <tasarım ailesi>."**
Kaynaklar: analistin `Requirements.md` dosyası (alan, roller, en sık yapılan iş), kullanıcının hava kelimeleri ve referansları, varsa marka varlıkları.
Belirsizse koordinatöre **tek** soru dön; çoklu anket yapma.

## 2. Üç kadran (0-10)

| Kadran | Anlam | Dashboard aralığı |
|---|---|---|
| `VARIANCE` | 1 simetrik ... 10 asimetrik | 4-6 (veri ekranında düzen önce gelir; kimlik kabukta, başlıkta, imza bileşende) |
| `MOTION` | 1 statik ... 10 sinematik | 3-5 (geri bildirim ve durum geçişi; süs animasyonu yok) |
| `DENSITY` | 1 ferah ... 10 kokpit | 5-8 (role göre: yönetici özeti 5-6, operasyon ekranı 7-8) |

Kadranlar `Dna.md` içine gerekçesiyle yazılır. MOTION > 3 ise `prefers-reduced-motion` zorunludur.

## 3. DNA kartı

| Eksen | Seçim kuralı |
|---|---|
| **Yazı tipi üçlüsü** | Display + sans + mono (sayılar için). **Inter varsayılan olamaz**; dashboard'da serif yok. Self-host (`wwwroot/fonts/`), latin-ext (ğ ş İ ı) doğrulanır. Sayılar `tabular-nums`. Öneri havuzu: Archivo + Hanken Grotesk + JetBrains Mono · Bricolage Grotesque + Manrope + IBM Plex Mono · Outfit + Figtree + Spline Sans Mono · Space Grotesk + DM Sans + Space Mono · Epilogue + Onest + Red Hat Mono · Urbanist + Plus Jakarta Sans + Fira Code. cms-codebase registry'sinde kullanılmış üçlü seçilmez. |
| **Palet** | Nötr taban + **tek accent** (doygunluk < %80) + anlamsal renkler (başarı, uyarı, hata, bilgi) ayrı ve accent'ten ayırt edilir. Saf `#000` / `#fff` yok. AI-mor/neon gradient ve bej+pirinç ailesi yok. Açık ve koyu tema aynı token adlarıyla tanımlanır. Grafik paleti ayrıca yazılır (kategorik en çok 6 renk, renk körlüğüne dayanıklı, açık/koyu ikisinde de AA). |
| **Köşe ölçeği** | Tek ölçek: keskin (0-2px) / yumuşak (8-12px) / hap. Karışık yalnız yazılı kuralla (`--radius-ui`, `--radius-card`). |
| **Gezinme arketipi** | Biri seçilir, gerekçesi yazılır: daraltılabilir yan ray · çift ray (ikon rayı + bağlam paneli) · üst bar + sekmeler · yüzen dok · komut paleti öncelikli (Ctrl+K) üst bar. Mobil karşılığı (çekmece, alt bar) aynı satırda tanımlanır. |
| **Sayfa iskeleti** | Özet ekranı için düzen ailesi: bento (asimetrik hücreler) · ana kolon + sağ bağlam rayı · bölünmüş ana/detay · tek kolon akış. **Dört eşit istatistik kartı + altında iki grafik** kalıbı varsayılan olamaz. |
| **Veri dili** | Tablo stili (satır yoğunluğu, zebra yok/var, yapışkan başlık), sayı biçimi, durum gösterimi (rozet biçimi tek), boş/yükleniyor/hata durumlarının görsel dili. |
| **Hareket imzası** | Tek cümle gerekçe: geri bildirim / durum geçişi / hiyerarşi. Yalnız `transform` + `opacity`. |
| **İmza bileşen** | Bu paneli tanımlayan **tek** özgün parça (örn. zaman çizelgeli etkinlik şeridi, canlı durum halkası, komut paleti). Elle yazılır; geri kalan bileşenler sistemden gelir. |

## 4. Anti-slop yasakları (mekanik taranır)

**Görsel:** neon/dış glow, saf siyah, büyük başlıkta gradient metin, tam sayfa cam yüzey, her kartta gölge + kenarlık + gradient üst üste,
süs amaçlı çizgi/nokta desenleri. Cam (backdrop-blur) yalnız üst bar ya da çekmecede, en çok 2 yüzey.
**Düzen:** dört eşit KPI kartı satırı (varsayılan olarak), iç içe kart, her satırda renkli durum noktası, içerik genişliğinde amaçsız boş alan,
her bölüm başlığının üstünde küçük büyük harfli etiket (eyebrow en çok 3 bölümde 1).
**Etiket ve metin:** sürüm rozeti (`v2.0`, `BETA`), "Welcome back, John" tarzı jenerik karşılama, filler fiil (seamless, elevate, unleash),
uydurma kusursuz sayı (`%99.99`), jenerik isim/marka (John Doe, Acme), lorem ipsum. Demo veri alana uygun ve gerçekçi olur.
**Kaynak:** elle çizilmiş SVG ikon/illüstrasyon (ikonlar tek aileden, self-host), div'lerden sahte grafik (grafik gerçek veriyle çizilir),
`@latest` / CDN bağlantısı, emoji ikon yerine.
**Yazım:** em-dash ve en-dash yok (başlık, gövde, buton, seed, `<title>`); tire `-`. Üç nokta `…` karakteriyle.
**Kilitler:** renk, köşe ve tema kilidi tüm ekranlarda aynı; aynı niyetli iki farklı buton etiketi yok ("Kaydet" + "Güncelle" aynı işlem için).

## 5. Özgünlük kapısı (Faz T çıkışı)

1. Kart dolu ve gerekçeli; kadranlar yazılı.
2. Kabuk prototipi (gezinme + üst bar + özet ekranı iskeleti + bir tablo ekranı + bir form) gerçekçi demo veriyle çalışıyor.
3. qa-tester 1440 / 768 / 390 ekran görüntüsü alır; **bilinen admin şablonlarıyla (AdminLTE, CoreUI, Tailwind UI varsayılanı, shadcn varsayılanı)
   aynı iskelet/aynı görünüm ise ret**. Anti-slop taraması temiz, uygulanan yazı tipi kartla aynı (`document.fonts.check`).
4. Kullanıcıya DNA kartı + ekran görüntüleri sunulur; **kullanıcı onayı** Faz T kapısıdır. Onaysız ekran fazına geçilmez.
5. Onaydan sonra kimlik değişikliği yalnız analist kararıyla (`Docs/Phases/Decisions.md`).

## 6. `Dna.md` ön bloğu

Dosya YAML ön bloğuyla başlar; `_Theme.cshtml` token'ları bundan yazılır, qa-tester uygulanan değerleri bununla karşılaştırır.

```yaml
---
navigation: collapsible-rail      # §3 gezinme arketipi
layout: bento                     # §3 sayfa iskeleti
fonts:
  display: Bricolage Grotesque
  sans: Manrope
  mono: IBM Plex Mono
accent: "#0F766E"
radius:
  ui: 8px
  card: 12px
dials:
  variance: 5
  motion: 4
  density: 6
signature: "<imza bileşenin adı>"
---
```

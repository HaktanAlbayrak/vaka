DEVİR: frontend-engineer -> koordinatör   Faz: 6B (FE 5-11)   Dosya: Docs/Handover/frontend-engineer-20261116-0010.md

Yapıldı:
- FE 5 kart listesi: `Styles/app.css` `.table-stack` yeniden yazıldı (yalnız `screen`, 1279 px ve altı). Satır = flex-wrap kart: yüzey rengi, 2 px dört kenar, `--radius-card`, kartlar arası 14 px. Baş `stack-lead` (tanıtıcı) + `stack-badge` (rozet, sağa, uzunsa alt satıra), baş ile gövde arası `tr::after` ince çizgi, gövde 2 sütun (<768) / 3 sütun (768-1279), `stack-wide` gövdenin ilk satırında tam satır, ayak `cell-action` (üstte çizgi, sağa hizalı, `action-row` tek satır). Rayın yanındaki gömülü tablo 1024-1279'da 2 sütun. `.panel:has(> .table-stack)` kart modunda şeffaf ve kenarlıksız, 1280+'da `overflow: clip` (başlık zemini köşeden taşmaz). `data-state="bad"` satırı tehlike kenarlıklı. Yazdırmada kart yok (kurallar screen). Etkisiz `col-span` sınıfları 14 görünümden temizlendi.
- FE 6 köşe: ui 6 px, card 10 px, ui-inner = ui - 2 px (`_Theme.cshtml`, `Dna.md` ön blok ve köşe satırı gerekçeli). Avatar tam yuvarlak; phase-cell, machine-tile, lot-results, drawer paneli (sağ köşeler) token köşe aldı. Görünümlerde sabit köşe değeri yok.
- FE 7 kabuk: `.rail` ve çekmece paneli `overflow-hidden`; yalnız `.rail-nav` kayar (logo ve alt blok sabit, çekmecede başlık ve `.drawer-foot` sabit). `ShellNav.revealCurrent` etkin öğeyi menü listesinde görünür alana getirir (sayfa yüklemesi ve çekmece açılışı). Daralma `grid-template-columns` 220 ms, etiketler opacity 150 ms ve ray sınırında kırpılır (satır kırılması yok, ikonlar yerinde kalır); çekmece `transform` 220 ms + perde opacity 200 ms, `visibility` gecikmeli kapanır (kapalıyken odak almaz). Geçişler `html[data-js]` altında (ilk boyamada ve kayıtlı daraltmada animasyon yok), `prefers-reduced-motion`'da süre 0.01 ms ve gecikme 0. DNA MOTION 3 -> 4.
- FE 8 tam ekran: `_TopbarTools` düğmesi (`hidden` çizilir), `wwwroot/js/Fullscreen.js` (App.js içinden), `fullscreenchange` ile `aria-pressed`, `aria-label`, simge; reddedilen istek sessiz. 768 altında gizli (`max-md:hidden`, 360 px üst bar yer darlığı). `maximize` ve `minimize` ikonları `BuildAssets.mjs`'e eklendi, sprite yeniden üretildi.
- FE 9 grafik: `_BarChart.cshtml` genelleştirildi (sunucuda SVG, yüzde genişlik, desen ve marker, lejant, metin değerleri; kütüphane yok). E-30: durum dağılımı, makine (net + mola yığılmış, değer metni net ve mola), faz (ortalama net çubuğu, asgari işareti, ortalaması asgarinin altındaki faz desenli ve "Ortalama asgarinin altında" metni). Her grafiğin tablosu aynı sayfada (durum için mevcut sayı listesi). Boş durumda grafik çizilmez (mevcut boş durum kutusu).
- FE 10 pano: `Views/Dashboard/Index.cshtml` (özet şeridi: bir büyük iki küçük sayı; Şu an sahada, Süre altında tamamlanan fazlar, Son tamamlananlar tabloları; Son 7 gün grafiği + tablosu, 2xl'de sağ ray, altında üstte). Menüde ve alt çubukta "Ana sayfa" `/dashboard`, `NavKey` home, `aria-current="page"` panoda.
- FE 11: Yeni kayıt "İş emri no (isteğe bağlı)", `required` yok, yardım metni "Boş bırakırsanız kayıt açılınca sistem numara atar"; hata D-1 `_Field` hata yerinde.

Değişen dosyalar: Styles/app.css, Styles/BuildAssets.mjs, wwwroot/css/app.css, wwwroot/lib/icons/Icons.svg, wwwroot/js/{App,ShellNav,Fullscreen}.js, Views/Shared/{_Layout,_Theme,_TopbarTools,_BarChart}.cshtml, Views/Dashboard/Index.cshtml, Views/Records/{Create,Index,Details,Integrity}.cshtml, Views/Reports/{Durations,Materials}.cshtml, Views/{Users,Facilities(2),Machines,Shifts,Materials(2),CleaningTypes(2)}/..., Docs/Design/Dna.md.

Beklenen G11 imzaları (360 | 768 ve 1024; 1440 tablo). Baş[Tanıtıcı|Rozet] Gövde[Alan:sütun@satır] Ayak[İşlem]:
- Machines/Index: 360 Baş[Kod|Durum] Gövde[Ad:2@1 TesisHat:1@2] Ayak[İşlem]; 768/1024 Gövde[Ad:3@1 TesisHat:1@2]
- Users/Index: Baş[Sicil no|Durum] Gövde[Ad soyad:tam@1 Roller:1@2]; Ayak[İşlem] (Yalnız görüntüleme çipi de ayakta)
- Facilities/Index: Baş[Kod|Durum] Gövde[Ad:tam@1 Hat:1@2 Vardiya:1@2]; Ayak[İşlem]
- Facilities/Details (ray): Baş[Kod|Durum] Gövde[Ad:tam@1 Makine:1@2] Ayak[İşlem]; 1024 ve 2 sütun
- Shifts/Index: Baş[Ad|Durum] Gövde[Başlangıç:1@1 Bitiş:1@1] Ayak[İşlem]
- Materials/Index: Baş[Kod|Durum] Gövde[Ad:tam@1 Lotlar:1@2] Ayak[İşlem]
- Materials/Details (ray): Baş[Lot no|Durum] Gövde[SKT:1@1] Ayak[İşlem]
- CleaningTypes/Index: Baş[Kod|Durum] Gövde[Ad:tam@1 Tür Versiyon Makine] Ayak[İşlem]
- CleaningTypes/Details (ray): Baş[Versiyon|Durum] Gövde[Yayın:tam@1 Doküman no:1@2] Ayak[İşlem veya yok]
- Records/Index: Baş[Kayıt no|Durum (tür + durum rozetleri)] Gövde[Makine ve yer:tam@1 (uyarı çipleri bu hücrede) Sahip Açılış Net süre] Ayak[İşlem]
- Records/Details Molalar: Baş[Mola|-] Gövde[Başlangıç Bitiş Süre Kapanış] Ayak[yok]; Malzemeler: Baş[Malzeme|-] Gövde[Lot no SKT Giren Giriş zamanı]; Olay günlüğü: Baş[Olay|-] Gövde[Özet:tam@1 Sıra Zaman Yapan] Ayak[yok]
- Records/Integrity: Değişen alanlar Baş[Alan ve olay|-] Gövde[Saklanan Olaydaki]; Olay zinciri Baş[Olay|Durum] Gövde[Sıra Zaman Özet kodu]
- Reports/Durations: Makine Baş[Makine ve tip|-] Gövde[Kayıt Brüt Mola Net Efor]; Faz Baş[Tip ve faz|-] Gövde[Asgari Kayıt Ort. net Süre altı]; Adım Baş[Adım ve tip|-] Gövde[Faz Kayıt Ort. süre En uzun]
- Reports/Materials: Lot Baş[Kayıt no|Durum] Gövde[Makine:tam@1 Açılış] Ayak[İşlem]; Makine Baş[Malzeme|-] Gövde[Giriş Kayıt no Lot no SKT] Ayak[İşlem]
- Dashboard: Şu an sahada Baş[Kayıt no|Durum (Acil + durum, kilit bitişi)] Gövde[Makine:tam@1 Sahip Şu anki adım Açılış] Ayak[İşlem]; Süre altı Baş[Kayıt no|-] Gövde[Faz ve makine:tam@1 Gerekçe:tam Net Asgari Tamamlanma] Ayak[İşlem]; Son tamamlananlar Baş[Kayıt no|Tür] Gövde[Makine:tam@1 Tamamlanma Net Efor] Ayak[İşlem]
- Son 7 gün tablosu küçük özet tablosudur (kart olmaz, 3 kısa sütun); G11 kapsamı dışı sayıldı, karşı görüş varsa analist belirtsin.

DNA uygunluğu (kart maddeleri): köşe 6/10 px token; gölge yok; tek accent; grafik en çok 3 renk (chart-1/2/3), renk dışında desen ve metin; Inter ve serif yok; dört eşit KPI yok (özet şeridi 1 büyük + 2 küçük); kart ayrımı yüzey + 2 px kenarlık + boşluk; MOTION 4 gerekçesi Dna.md'de.

UiDelivery öz denetimi: `npm run build:css` temiz; `dotnet build` uyarı 0, hata 0 (çıktı scratchpad'e, bin kilidi nedeniyle); kopya veritabanı (`vaka.db` kopyası), port 5187, Yönetici 1001 ile 16 sayfa 200, h1 = 1, em/en-dash 0, dış kaynak 0; oturumsuz `/dashboard` 302; `/dashboard` menüde `aria-current="page"` (3 yerde: rail, alt çubuk, çekmece). §4 grep tablosu (transition-all, outline-none, boyutsuz img, user-scalable, onclick, CDN, dash, h-screen, kütüphane, ham renk, sabit köşe) 0 ihlal; `rounded-full` yalnız avatar, canlı nokta, PIN hanesi. Yalnız kendi PID'im (10608) kapatıldı.

Backend'e bağımlı açık noktalar:
- Yok; sözleşmedeki alanlar aynen kullanıldı, ViewModel dosyalarına dokunulmadı. Pano hata durumu: ViewModel hata alanı yok, sorgu hatası genel hata sayfasına düşer (ayrı hata kutusu yok); boş durumlar var.
- Operatör hesabıyla `/records/create` formu çalışan uygulamada görülmedi (operatör parolası elimde yok); derleme ve işaretleme temiz.

Doğrulanamayanlar (tarayıcı yok, FE playwright kullanmaz; QA ön turuna): 1440x600 ve 768x600 kısa yükseklikte sabit blok ölçümü, daraltılmış ray geçişinin görsel kalitesi (ikon konumu, etiket kırpması), kartların 360/768/1024 ölçümleri, koyu tema grafik okunurluğu, tam ekran davranışı. Ray geçişi grid-template-columns animasyonuna dayanır (Chromium, Firefox, Safari güncel sürümlerde destekli).

## Ön tur düzeltmesi
- B1 (YÜKSEK): `_BarChart.cshtml` `FillClass` artık ton -> tam sınıf adı eşlemesi (`fill-chart-1/2/3` kaynakta açık yazılı); `.fill-chart-1/2/3`, `.stroke-ink`, `.fill-surface`, `.fill-sunken` derlenmiş CSS'te var (doğrulandı). Renkler tema tokenlerinden olduğundan açık ve koyu temada otomatik değişir (koyu: chart-1 mavi, chart-2 turuncu, chart-3 gri, hepsi sunken zemine karşı belirgin). Görsel doğrulama QA'da.
- B2 (ORTA): `td[data-label]:not(...)::before` seçicisine `.cell-action` dışlaması eklendi; ayakta etiket yok. `cell-action` `items-center justify-end`: düğmeler sağa hizalı, tek satır, dikeyde ortalı.
- B3 (ORTA): `prefers-reduced-motion: reduce` altında `.drawer`, `.drawer-scrim`, `.drawer-panel` ve `html[data-js] .shell` için `transition: none !important`. Görünürlük anında açıldığından `open()` odağı ilk öğeye verir; normal ayarda davranış değişmedi.
- N1: dokunulmadı (sr-only thead, görünür taşma yok).
- `npm run build:css` ve `dotnet build` temiz (0 uyarı, 0 hata). Uygulama bu turda çalıştırılmadı, PID açılmadı.

## Tur 2
- B1 (ORTA): `td.stack-badge` kart modunda dikey sütun (`flex-col items-end`): rozetler kartın sağ iç kenarında, "Kilit bitişi" rozetin altına iner (`Styles/app.css`). Tek hücre kuralı olduğundan 360/768/1024 ve rayda aynı.
- B2 (ORTA): reduced-motion bloğunda `.drawer` ve tüm alt öğeleri (`.drawer *`) `transition: none !important`; kök neden (alt öğelerde `all` geçişiyle visibility gecikmesi) kalktı. Ek güvence: `ShellNav.open()` odak ilk öğeye geçmediyse bir sonraki karede yeniden dener.
- B3 (ORTA): `Reports/Durations.cshtml` durum bölümü koşulu "tüm sayılar 0" (`StatusCounts.All(Count == 0)`). Boş süzgeçte (`from=2030-01-01&to=2030-01-31`) boş durum metni çıkıyor ve durum grafiği çizilmiyor (kopya DB'de doğrulandı: `<rect` 0).
- B4 (DÜŞÜK): `_BarChart` lejantında `-hatch` tonu için desenli kare (faz grafiğinde 4 çubuk + 1 lejant deseni).
- B6 (DÜŞÜK): `--chart-2` açık tema `196 112 14` -> `180 96 8` (`_Theme.cshtml`); hesap: göreli aydınlık 0,181, `sunken` (224,230,234) karşısında yaklaşık 3,6:1, panel zemininde daha yüksek. Koyu tema değişmedi. `Dna.md` palet satırı `#B46008` olarak güncellendi.
- B5 (DÜŞÜK, PL-84): Operatör + Yönetici/Denetçi kullanıcıda ray ve çekmecede "İzleme" grubu "Pano" (`/dashboard`, anahtar home, panoda `aria-current="page"`) ile başlar, Makine panosu ve Kayıtlar bu grupta; alt çubuk bu öğeyi almaz; tek rollü menüler ve `/` yönlendirmesi değişmedi.
- Öz denetim: `npm run build:css` temiz, `dotnet build` 0 uyarı 0 hata; kopya DB, port 5188, Yönetici ile boş süzgeç ve hatch kontrolü; PID 30920 kapatıldı. Doğrulanamayanlar: çoklu rol menüsü (kopya DB'de 4001 ile giriş olmadı, mantık kodda okundu), B1 ölçümü ve çekmece odağı (hesaplanmış stil ve odak ölçümü tarayıcı gerektirir, QA turunda).

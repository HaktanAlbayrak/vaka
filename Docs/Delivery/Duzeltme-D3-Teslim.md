# Düzeltme D-3 Teslim Raporu: Adım medyasının yeri

> Faz dışı düzeltme (PL-108, PL-109, O-15; PL-74 düzeni). D-2 raporunun biçimi: `_Template.md`'nin kısa hali. Yalnız PhasePlan "Düzeltme D-3" bölümündeki QA ölçüm şartı ölçüldü; yeniden üretim kullanıcının veritabanının kopyasında yapıldı.

| Alan | Değer |
|---|---|
| Faz | D-3 (faz dışı düzeltme) |
| Tarih | 2026-11-17 |
| Tur | 1 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `e390bf6`). Değişen dosyalar `Views/ProcedureVersions/Edit.cshtml`, `Views/ProcedureVersions/Details.cshtml`, `Views/Records/Details.cshtml`, `wwwroot/js/ProcedureEditor.js`, `Services/StepMediaService.cs` (ve yeniden üretilen `wwwroot/css/app.css`). Kopyası `.qa/d3/src`, repodakiyle bayt bayt aynı |
| Ortam | LOCAL, Release build, port 5941 (Production). DB: `.qa/d3/vaka.db` = `App_Data/vaka.db` dosyasının SQLite yedekleme API kopyası; kopyada yalnız Süper admin parola ve PIN özeti yazıldı, `1001`, `2002` ve `3001` arayüzden sıfırlandı (Faz 6C düzeni). `Media__RootPath=C:/projects/vaka/.qa/d3/Media`. Chromium (Playwright 1.61). Gerçek DB'ye, `App_Data/Media`'ya ve PID 20552'ye dokunulmadı; kendi PID'im (11828) kapatıldı |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (KRİTİK, YÜKSEK, ORTA yok; bir DÜŞÜK) |
| Analist kararı | TESLİM ALINDI (2026-11-17, PL-110); B1 kabul |

## 1. Kapsam

- FE `Edit.cshtml`:
  - Medya bölümü adım kartında panel açmadan görünüyor: başlık "Fotoğraf ve video (n)", boşsa "Bu adımda medya yok.", medya listesi ve yükleme formu.
  - "Adımı düzenle" panelinde yalnız başlık, açıklama ve çıkarma kaldı.
  - Adım başlığında sayı rozeti var.
- FE `ProcedureEditor.js`: adım eklenince odak yeni adımın medya başlığına gidiyor (`last-media:`).
- FE `ProcedureVersions/Details.cshtml`: taslak olmayan versiyonda yalnız Yönetici bilgi kutusunu ve "Tip ayrıntısına git" bağlantısını görüyor; sayı rozeti var.
- FE `Records/Details.cshtml` (D3-2): `_MediaDialog` eklendi.
- BE `StepMediaService.cs` (D3-1):
  - 0 baytlık dosyada "Dosya boş. İçeriği olan bir fotoğraf ya da video seçin."
  - Dosyasız gönderimde "Dosya seçin."

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| D3-Y1 | PLN v2 taslağında bir adım kartından, panel açmadan JPEG ve MP4 yükleme; başlık "Fotoğraf ve video (2)" | GEÇTİ | `Evidence/D-3/D3Olcum.txt` "Düzenleyici". Adım 33'te başlangıçta "Fotoğraf ve video", "Bu adımda medya yok."; dosya alanı görünür ve kapalı panelde değil (`checkVisibility` true), "Adımı düzenle" kapalı. JPEG: 302, başlık "(1)", rozet 1. MP4: 302, başlık "(2)", rozet 2. Panel kapalı kaldı (`DuzenleyiciAdimKarti360Acik.png`) |
| D3-Y2 | Yeni adım ekleyip aynı sayfada hemen ona medya yükleme | GEÇTİ | "QA medya adımı" eklendi (adım 39). Odak `step-39-media-title`'a ("Fotoğraf ve video") gitti, adres aynı kaldı. Aynı sayfada adım 39'a JPEG yüklendi: 302, başlık "(1)" |
| D3-Y3 | PIN'le yayın; Operatör yeni kayıtta o adımın iki medyasını açar, video oynar ve ileri sarılır; kayıt detayında görünür | GEÇTİ | Yayın 302, versiyon 7 durumu 2. `2002` kayıt 12'yi açtı (versiyon 7). Run'da adım 1.1'in iki medyası 1440 ve 360 px'te önizlemede açıldı. Fotoğraf yüklendi. Video oynadı (`currentTime > 0`); `currentTime` 2,2 sn'ye kuruldu, 3 sn'lik video sonuna ulaştı; hata yok. `/media/1` 200 `image/jpeg`, `/media/2` 206 `video/mp4`. Kayıt detayında aynı iki medya ve adım 39'unki görünüyor |
| D3-2 | Kayıt detayında önizleme açılır, Escape ile kapanır, odak tıklanan öğeye döner, video oynar; JS kapalı bağlantı çalışır | GEÇTİ | Kayıt detayı, 1440 ve 360 px: fotoğraf ve video önizlemesi `dialog` içinde açılıyor ve viewport'a sığıyor. Video oynuyor ve ileri sarılıyor. Escape kapatıyor; odak "Fotoğraf önizle: adim.jpg" ve "Video önizle: v_vp09.mp4" küçük resimlerine dönüyor. JS kapalıyken bağlantılar `/media/1` (200 `image/jpeg`) ve `/media/2` (200 `video/mp4`) açıyor. Konsol 0 |
| D3-R | GIF ve sınır üstü dosya nedenli reddedilir (Faz 3 K8 gerilemesi) | GEÇTİ | JS açık: GIF için alanın altında "Bu dosya türü kabul edilmiyor. Seçici yalnız izin verilen biçimleri gösterir."; 11 MB fotoğraf için "Fotoğraf en çok 10 MB olabilir."; istek gitmiyor, odak alanda. JS kapalı (sunucu): GIF için kartta "Dosya biçimi desteklenmiyor. İzinli biçimler: JPEG, PNG, WebP, MP4.". İçeriği uzantısıyla uyuşmayan `.jpg` için "Dosya içeriği uzantısıyla uyuşmuyor." |
| D3-1 | 0 bayt dosyada "Dosya boş..." alanın hata yerinde; dosyasız gönderimde "Dosya seçin." | GEÇTİ (yer için B1) | 0 bayt: 200, "Dosya boş. İçeriği olan bir fotoğraf ya da video seçin." Dosyasız ham POST: 200, "Dosya seçin." İkisi de adım kartındaki yükleme formunun hata kutusunda (`role="alert"`, odak kutuda, görünümde, alanın 42 px üstünde). Alanın altındaki hata satırında değil (B1) |
| D3-B | Yayınlanmış versiyonda bilgi kutusu Yönetici'de görünür, bağlantı tip ayrıntısını açar; Denetçi'de görünmez | GEÇTİ | `1001` için `/procedure-versions/details/7` (yayında) ve `/details/1` (arşiv): "Yayınlanmış versiyon değiştirilemez. Fotoğraf ya da video eklemek için tip ayrıntısından taslağı açın." ve "Tip ayrıntısına git" (`/cleaning-types/details/1`, 200). `3001` için aynı sayfalarda kutu ve bağlantı yok (`YayinlanmisSurumBilgi360Koyu.png`) |
| D3-G | 320 (%200), 360 ve 1440 px, açık ve koyu: taşma yok, G12 kusuru 0, konsol 0; değişen dosyalarda grep tablosu 0 | GEÇTİ | Düzenleyici (`/procedure-versions/edit/8`, 7 medya bölümü), versiyon ayrıntısı ve kayıt detayı; 18 hücre. `hScroll` false; h1 1; atlama bağlantısı var; img ve etiket ihlali 0; dash yok; konsol 0. Medya bölümünde başlık, etiket, alan ve düğme solu bölüm iç kenarında, bölüm dışına çıkan öğe 0 (G12). İki eşleşme yanlış pozitif: düzenleyicide `MaterialRequired` onay kutusu 20x20 (hedef saran etiket, 292x94 px, Faz 3'ten); kayıt detayında 320 ve 360 px'te görsel olarak gizli tablo başlığı (`thead` `position:absolute`, `clip: rect(0)`). `UiDelivery.md §4` kalıpları değişen beş dosyada 0. Release derlemesi 0 uyarı, 0 hata; CSS repodakiyle bayt bayt aynı; sunucu günlüğünde istisna 0 |
| D3-Gr | Gerileme: başlık ve açıklama kaydı, adım taşıma ve çıkarma, JS kapalı yükleme | GEÇTİ | "Adımı düzenle" ile adım 34 "Ürün kalıntısını boşalt (QA)" ve "QA açıklama satırı." olarak kaydedildi. Adım 39 yukarı taşındı (`step-33,step-34,step-39` > `step-33,step-39,step-34`), odak taşıma düğmesinde. Eklenen adım 40 "Adımı çıkar" ile çıkarıldı, odak faz kartında. JS kapalıyken JPEG yüklemesi başarılı (adım 36, başlık "(1)") |

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`.qa/d3/src`) |
| CSS | `npm run build:css` | Kopyada derlenen çıktı repodaki `wwwroot/css/app.css` ile bayt bayt aynı |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | Değişmedi (yalnız `StepMediaService.cs` iletisi) |
| Adlandırma (PascalCase) | örneklem denetimi | Yeni dosya yok |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal |
| Mekanik eval | `UiDelivery.md §3` | 18 hücre beklenen değerlerde (iki yanlış pozitif yukarıda) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Yeni migration yok |

## 4. Yetki matrisi

Route değişmedi. Bilgi kutusu görünürlüğü rol bazlı ölçüldü: Yönetici görür, Denetçi görmez (D3-B).

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| Düzenleyici (adım kartı medya bölümü) | Temiz | Ölçülmedi | Ölçülmedi | Temiz | Ölçülmedi | Temiz | Ölçülmedi | 320 temiz | Adım eklenince odak yeni adımın medya başlığında; yükleme sonrası odak adım kartında; hata sonrası odak hata kutusunda ya da alanda |
| Versiyon ayrıntısı (bilgi kutusu) | Temiz | Ölçülmedi | Ölçülmedi | Temiz | Ölçülmedi | Temiz | Ölçülmedi | 320 temiz | - |
| Kayıt detayı ve Run (önizleme) | Temiz | Ölçülmedi | Ölçülmedi | Temiz | Ölçülmedi | Temiz | Ölçülmedi | 320 temiz | Escape kapatıyor, odak küçük resme dönüyor |

Tasarım DNA'sı uygunluğu: UYGUN. Medya bölümü `rounded-card`, 2 px kenarlık; sayı rozeti ikon + metinli `badge`; bilgi kutusu mevcut `notice` diliyle. 360 px'te başlıktaki "(2)" alt satıra iniyor (`flex-wrap`); taşma değil.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | DÜŞÜK | Sunucu tarafı yükleme hataları ("Dosya boş...", "Dosya seçin.", biçim ve içerik uyuşmazlığı) alanın altındaki hata satırında (`[data-media-error]`) değil, formun hata kutusunda (`_EditorError`, `role="alert"`) görünüyor. Kutu alanın 42 px üstünde, odak kutuda; alan `aria-invalid` almıyor, `aria-describedby` yalnız ipucunu gösteriyor. PhasePlan kapsam tablosu "Yükleme ya da çıkarma hatası kartta görünür" diyor; bu karşılanıyor. QA şartı "alanın hata yerinde" diyor; bu karşılanmıyor | `D3Olcum.txt` "D3-1 yerleşim" (`boxAboveInput: true`, `gapPx: 42`, `inputAriaInvalid: null`, `focusIsBox: true`); `Views/ProcedureVersions/Edit.cshtml` yükleme formu `_EditorError` | analyst (karar), frontend-engineer | Kart kutusu kabul edilirse kayıt altına alınır. Edilmezse sunucu iletisi alanın hata satırına yazılır ve alan `aria-invalid` ile hataya bağlanır | AÇIK |

## 7. Geçenler

D3-Y1, D3-Y2, D3-Y3, D3-1 iletileri, D3-2, D3-R, D3-B, D3-G ve D3-Gr. Yeniden üretimde hata yok.

## 8. Kapsam dışı / yapılamayan

- Test MP4'ü VP9 kodlu bir MP4 (OpenCV ile üretildi). Playwright'ın Chromium sürümü H.264 çözmediği için telefon videosunun tipik H.264 MP4'ü bu ortamda oynatılamadı. Sunucu kodeke bakmıyor; `video/mp4` ve `ftyp` imzası yeterli.
- 768, 1024, 1920 px ve reduced-motion ölçülmedi; şart 320 (%200), 360 ve 1440 px.
- QA kopyasında kalanlar: versiyon 7 yayınlandı, v3 taslağı açıldı, kayıt 12 açık. Kaynak DB etkilenmedi.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| - | yok | - | B1 için karar analistin |

## 10. Analist kararı

```
D-3: TESLİM ALINDI
Tarih: 2026-11-17
Gerekçe: Şart 9/9 kanıtla; kullanıcının veritabanı kopyasında panel açmadan yükleme, yeni adıma hemen medya, yayın, oynatma ve detay
  önizlemesi çalışıyor; gerileme yok.
B1 DÜŞÜK, kabul: taslak düzenleyicinin tüm formları sunucu hatasını kartın form hata kutusunda gösterir (Edit.cshtml'de 7 _EditorError,
  Faz 3 deseni); yalnız medya formuna alan hatası koymak düzenleyici içinde tutarsızlık yaratır. Kutu alanın hemen üstünde, role="alert",
  odak kutuda, metin görünür ve okunur. PhasePlan'daki D3-1 ölçüm metni "alanın hata yerinde" yanlış yazılmıştı, "kartın hata kutusunda"
  olarak düzeltildi (PL-110). Açık kalem: yok.
```

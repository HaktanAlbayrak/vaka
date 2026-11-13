# Frontend devir notu: Faz T tur 3

## Değişenler
- `Src/Vaka.Web/Styles/app.css:444-449`: `.step-action` ızgarası `repeat(auto-fit, minmax(min(100%, 14rem), 1fr))`; kolon iki sütuna yetmezse PIN ve düğmeler alt alta. Yeni `.step-action .btn` kuralı (`w-full min-w-0 whitespace-nowrap`): metin tek satır, taşma yok; `btn-lg` 56 px yüksekliği korunur.
- `Docs/Design/Dna.md:34`: kart dönüşümü "1279 px ve altında satır kartı, 1280 px ve üstü tablo".
- Görünüm değişmedi; `wwwroot/css/app.css` derlendi.

## Öz denetim
`npm run build:css` tamam; `dotnet build -warnaserror` 0 uyarı 0 hata; `/prototype/run` ve `scene=between|break|opened` 200; em/en-dash 0 (Views, Styles, Dna.md).

## QA ölçmesi gereken hücreler
- `/prototype/run` (devam ediyor) ve `scene=opened`, `scene=between` (Adımı başlat): genişlik 320, 360, 768, 1024, 1040, 1079, 1080, 1280, 1440, 1920.
- Her birinde: Adımı bitir / Adımı başlat / Mola ver düğmesi sağ kenarı adım kartı kenarını aşmıyor, yükseklik 56 px (Mola ver en az 44 px), metin tek satır, PIN alanı ile örtüşme yok. Özellikle 1024-1079.
- `scene=break`: devre dışı "Adımı bitir" ve açıklama aynı kontrol.
- Not: çalışmayan tarayıcı turu yapılmadı; ölçüm QA'da.

## DNA uygunluğu
Dokunma hedefi ve 56 px birincil eylem maddeleri korunuyor; veri dili satırı uygulamaya eşitlendi.

## Backend'den beklenen/eksik
Yok.

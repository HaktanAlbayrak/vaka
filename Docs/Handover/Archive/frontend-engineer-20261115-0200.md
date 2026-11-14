# Devir notu: frontend-engineer, Faz 2 tur 2

Önceki not: Archive/frontend-engineer-20261115-0100.md. Backend `RetiredAtLocal` alanlarını iki ViewModel'e ekledi (okundu, kullanıldı).

## Düzeltmeler
- B2 (`Styles/app.css`): `overflow-wrap: anywhere` iki yerde. Kart modunda (`max-xl`) tüm `.table-stack td` için (imza değişmez). Tablo modunda yalnız içinde `.btn` ya da `.badge` bulunmayan hücreler için (`td:not(:has(.btn, .badge))`). `.table-stack .btn`, `.badge`, `.badge > span` her genişlikte `overflow-wrap: normal; word-break: normal`. Düğme ve rozet sütunu iki modda da kelime genişliğinin altına inmez. Faz 1 `/users` dahil ortak kural.
- B4: `.kv > *` `min-w-0`, `.kv dd` `overflow-wrap: anywhere` (eski `break-words` grid öğesinin en küçük genişliğini düşürmüyordu).
- B3, B5: `Machines/Retire.cshtml` `RetiredAtLocal` doluysa "Makine <an> tarihinde kullanımdan kaldırıldı." (bilgi bildirimi, `role=status`) ve "Makine listesine dön" (`/machines?showRetired=true`); engel listesi, neden metni ve gönder düğmesi yok. `Machines/Form.cshtml` kaldırılmışsa: başlık "Makine", aynı bilgi bildirimi, tesis, hat, kod ve ad salt okunur, kaydet düğmesi ve rayda "Kullanımdan kaldır" yok, `data-unsaved-guard` yok, bağlantı "Makine listesine dön". `Machines/Index.cshtml` kaldırılmış satırın eylemi "Görüntüle" (`/machines/edit/{id}`, göz ikonu), "Kaldır" yok.
- B6: `Machines/Index.cshtml` ve `Materials/Index.cshtml`: `Total > 0` ve sayfa boşsa `_State` "Bu sayfada kayıt yok." + "1. sayfaya dön" (`PageHref(1)`, süzgeç korunur). `Total = 0` dalları değişmedi.
- B7: `Machines/Form.cshtml` yenileme düğmesi `name="refresh" value="true"`, aynı POST formu, `formnovalidate` kaldı, `formmethod/formaction` yok. `CascadeSelect.js` `[data-cascade-fallback]` düğmesini JS açıkken gizlemeye devam ediyor (değişmedi).
- B8: `/machines` süzgeç ızgarasında `xl:items-end` yerine `xl:items-start`; Tesis, Hat ve arama etiketleri ve alanları aynı üst kenarda olmalı.

## Öz denetim
- `npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build` 0 uyarı 0 hata.
- §4 grep tablosunun tamamı `Views`, `Styles/app.css`, `wwwroot/js` üzerinde: transition-all, outline-none, user-scalable, onclick, CDN, em/en-dash, h-screen, jquery/gsap/aos, slate/gray/zinc, ham hex, boyutsuz img, görünür `...`, adsız ikon düğmesi: 0. `<input>` autocomplete/inputmode eksiği yalnız onay kutusu ve radyo (Faz 2 öncesi aynı, metin alanı 0).
- **Ölçülmedi:** tarayıcıda hiçbir şey (kural gereği). 1280-1920 tablo modunda uzun veriyle kırılma ve sicil no, 320/640 ray taşması, 1440 süzgeç hizası, G11 imzaları QA ön turunda ölçülecek. Risk: tablo modunda veri hücresine `anywhere` verildiği için olağanüstü uzun kodda sütun daralması olabilir; düğme ve rozet sütunu daralmaz.

## G11 beklenen imzası
- Liste ve ayrıntı tabloları 360/768/1024/1440: `MekanikEval.txt:120-125` ile Faz 1 tur 3 `/users` imzası değişmez (kart modu kuralı aynı, yalnız düğme ve rozet kırılmasız).
- Yeni durumlar: `/machines/retire/{kaldırılmış}` ve `/machines/edit/{kaldırılmış}` tek sütun (lg altı) ya da ana kolon + 22rem ray (lg ve üstü); `hScroll` false. Sayfa dışı liste: `_State` kutusu tek başına, tablo yok.

## Backend'den beklenen/eksik
Yok. Kaldırılmış makinede `POST /machines/edit` backend'de nedenli reddediliyor olmalı (PL-43); görünümde kaydet düğmesi olmadığından yalnız elle POST'ta görülür.

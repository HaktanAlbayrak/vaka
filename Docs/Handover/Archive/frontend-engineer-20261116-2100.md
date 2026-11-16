DEVİR: frontend-engineer -> koordinatör   Faz: 6C (FE 3, 4, 6)   Dosya: Docs/Handover/frontend-engineer-20261116-2100.md

Yapıldı:
- FE 3-4 zaman çizelgesi: `Views/Records/_Timeline.cshtml` (yeni), `Details.cshtml`'de "Olay günlüğü"nden önce. Gün başlığı `<h3><time>`, gün başına `<ol>`, saat `HH:mm` mono ve saniyeli `datetime`, cümle, adım etiketi, ayrıntı, süre ve efor `_Duration` ile. Tür-ikon-ton eşlemesi `Look` fonksiyonunda tek yerde. `IsWarning` satırı "Uyarı" etiketi ve ikonla metinle ayrılır. Bölüm `print:hidden`. Stiller `Styles/app.css` (`.tl-*`), tokenlerden.
- FE 6 görevli seçici: `_Participants.cshtml` ve `ParticipantPicker.js` WAI-ARIA combobox/listbox. Ok, Home, End, Enter, Esc, Tab; `aria-activedescendant`; durum canlı bölgede (yükleniyor, boş, hata, N kişi, kısaltma); molada olan `aria-disabled` ve "Eklenemez: neden"; seçilen kişi ve "Değiştir"; "Bir kişi seçin." alanın hata yerinde; eski yanıt `AbortController` ve istek sırasıyla atılır. JS kapalı yol durur.
- Ön tur düzeltmeleri: (1) `.tool > summary` artık `min-w-0 max-w-full whitespace-normal h-auto` ve `overflow-wrap:anywhere`; uzun görevli adı düğmeyi taşırmaz, JS kapalı yol dahil. (2) Listbox `tabindex="-1"`; Tab artık listeye değil sıradaki öğeye geçer. (3) Liste açıkken yükleniyor durumunda iskelet açılmaz; eski sonuçlar `opacity-60` ve `aria-busy` ile soluk kalır, yerleşim kaymaz (iskelet yalnız liste kapalıyken). (4) `.combo-option` tek dizilim: ad üstte, sicil no altta, not en altta (`flex-col`). (5) `.tl-mark` tam yuvarlaktan `rounded-ui` (6 px) kareye çevrildi.
- Karar (5): DNA tam yuvarlağı yalnız avatar ve nokta göstergelerine ayırıyor. `.tl-mark` içinde ikon taşıyan bir işaret, nokta göstergesi değil; ikonlu işaret için DNA köşe ölçeği `--radius-ui` geçerli. Bu yüzden düzeltildi.

Doğrulama: `npm run build:css` temiz; `dotnet build -o <scratchpad>` 0 hata, 0 uyarı (PID 32568 bin'i kilitliyor, dokunulmadı); em/en-dash, transition-all, outline-none, h-screen, onclick grep 0. Tarayıcıda denenmedi (FE playwright kullanmaz); 360-1440 ölçümü, Tab akışı ve combobox ARIA ağacı QA'da.

Sonrakinin bilmesi gerekenler:
- `GET /records/people` yanıtı `{ items, truncated }` camelCase; JS çıplak diziyi de kabul eder. `UnavailableReason` molada için "Molada" olmalı.
- Çizelge `RecordDetailViewModel.Timeline` (`RecordTimelineDayViewModel[]`) ve `TimelineEntryKind` ile çizilir; Kind eşlemesi `_Timeline.cshtml` başında.
- Değişen dosyalar: Views/Records/{_Timeline,Details,_Participants}.cshtml, wwwroot/js/ParticipantPicker.js, Styles/app.css, wwwroot/css/app.css.

DB talebi: yok
Açık/ertelenen: Liste ilk açılışta (liste kapalıyken) 2 satırlık iskelet ipucunun altında kısa süre yer tutar; sonuç gelince liste yerini alır. Ön turda sorun sayılırsa iskelet yerine yalnız durum metni (frontend-engineer, küçük).

Tur 1 iade düzeltmeleri (B1, B2, B3, B6):
- B1: Seçicinin denetimi FormValidation'ın sırasına katıldı. `FormValidation.js` `[required]` yanında `[data-choice-for]` alanlarını da gezer; boşluk ölçütü `data-choice-for` seçicisindeki alanın değeridir. Arama alanında `data-choice-for="[data-people-input]"` ve `data-required-message="Bir kişi seçin."` var. Seçicinin kendi submit dinleyicisi ve hazır hata `<p>`'si kaldırıldı; hata FormValidation'ın ürettiği `field-error` ile arama alanının `data-field` kabında, odak DOM sırasındaki ilk hatalı alanda (kişi, sonra PIN). Üç durum: ikisi boş odak kişide, yalnız kişi boş odak kişide, yalnız PIN boş odak PIN'de. Kişi seçilince hata ve `aria-invalid` temizlenir. Diğer formlar: yeni ayrım yalnız `data-choice-for` olan alana uygulanır, `required` yolu değişmedi.
- B2: `.tl-flag` kenarlığı `border-2`.
- B3: `StepLabel` görünümde " - " yerinde bölünür; "Faz n / Adım m" mono, adım başlığı gövde yazı tipi. ViewModel değişikliği gerekmedi.
- B6: görevli çipinde ad, sicil no ve ", sahip" tek `span` içinde; ", sahip" sicil no'ya bitişik.
- Doğrulama: `npm run build:css` temiz; `dotnet build -o <scratchpad>` hatasız. Tarayıcıda denenmedi.

# Devir notu: frontend-engineer, Faz 5 (FE 8, FE 9, ekranlar)

Önceki not: Archive/frontend-engineer-20261115-0800.md.

## FE 8 (K25)
- `Views/Shared/_PinInput.cshtml`: `@inject ISystemSettingsReader`, `SettingKeys.PinLength`; `maxlength` iki modda da güncel değer, istek başına tek okuma (`HttpContext.Items`). Belirleme modundaki hane göstergesi ve ipucu aynı değeri kullanır; tuple'daki `Length` artık okunmuyor (çağıranlar değişmedi). Görünümde sabit yok.
- `PinInput.js`: `beforeinput` rakam dışı eklemeyi engeller, `paste` yalnız rakamı ve kalan yeri alır; sunucu kuralları değişmedi. Tarayıcıda denenmedi.

## FE 9 (K26)
- Göz düğmesi (`.field-reveal`): dinlenmede dolgusuz ve kenarsız, köşe `max(0, radius-ui - 2px)`, hover/basılı/odakta `sunken` zemin, odak halkası alanın içinde (`outline-offset: -3px`), 44 px genişlik ve alan yüksekliği korunur. Parola alanı (`_Field`) da aynı sınıfı kullanır.
- Select: 16 select (Faz 1-4 ve prototip) `.select-wrap` + `chevron-down` (`.select-arrow`) ile sarıldı; `select.input` `appearance-none`, `h-12`, `pr-11`, pasifte kesik kenarlık/`sunken`/soluk ok. Yalnız select işaretlemesi değişti. `Dna.md` bileşen notu güncellendi (kart değişmedi).
- Beklenen (ölçülmedi): select yüksekliği 48 px, yanındaki `.input` ile eşit; ok dikey ortalı. 360-1440 px ve iki temada ön tur ölçmeli.

## Faz 5 ekranları
- `Views/Records/Run.cshtml` (E-15): kayıt başlığı (+ ilk adımı başlatan), kilit bildirimi, açık mola paneli ("Sayaçlar durdu", başlatan, başlangıç), tamamlanma özeti (net, brüt, mola, efor, ilk adım başlangıcı, tamamlanma; açılış ayrı), faz şeridi (net süre canlı ya da sabit, minimum çubuğu, brüt/mola), mola paneli (`CanStartBreak` iken "Mola ver" aracı, PIN'li; yoksa `CannotBreakReason` satırı), faz başına adımlar (sıradaki adım "Hazır" kenarlıklı, kilitli adım "Kilitli: neden", açıklama ve medya hazır/devam eden adımda, canlı adım sayacı, tamamlanan adımda başlatan/bitiren/süre/efor/mola, süre altı fazda gerekçe), başlat ve bitir formları (`.run-action`: PIN üstte, 56 px düğme altta). Aside: Molalar listesi, Malzeme. `data-record-id` ile `LotPicker.js` `/records/lots?q=&recordId=` çağırır (F4-A2 FE payı). Adım satırı başlık/rozet bloğu `min-h-11 items-center` korundu; satıra düğme eklenmedi (düğmeler `run-action` bloğunda).
- `Views/Records/_Participants.cshtml` (E-17): görevli çipleri; `CanManageParticipants` iken "Görevleri düzenle": her görevli için PIN'li Çıkar, kişi arama (JS açık: `ParticipantPicker.js`, `/records/people?q=&stepId=`; JS kapalı: GET formu `?stepId=&people=` + radyo listesi, uygun olmayan kişi pasif ve nedenli, K-41 uyarısı metinle), sahibin PIN'i ile Ekle. `PendingParticipant` JS kapalıda radyoyu işaretler, JS açıkta gizli alan ve seçili kişi satırını doldurur (adı `ParticipantOptions` içinde bulunamazsa "Önceden seçilen kişi").
- `Views/Records/FinishStep.cshtml`: süre altı gerekçe formu (faz, net, minimum, gerekçe, PIN, "Gerekçeyle bitir", "Vazgeç"); aynı `CommandId`.
- `Views/Breaks/Current.cshtml` (E-18): mola sayacı (sunucu saniyesi), kayıt, makine, başlangıç, PIN'li "Molayı bitir" (`.run-action`, 56 px, tek satır).
- `Views/MyWork/Index.cshtml`: açık mola bildirimi ve `/breaks/current` bağlantısı, "Görevli olduğum adımlar" kart listesi (boş durumlu).
- JS: `LiveTimer.js` (`<time data-live-timer data-seconds data-running>`, `data-running=false` iken ilerlemez; yalnız `performance.now` farkı, tarih hesabı yok; gösterim T-B14 biçimi), `ParticipantPicker.js`; `_LiveTimer` partial'ı sunucu metnini `_Duration` ile yazar. `App.js` bağlandı. CSS: `.run-action`, `.select-wrap`, `.select-arrow`, JS açık/kapalı kuralına `data-people-js/nojs`.

## G11 beklenen yerleşim (ölçülmedi)
Run, FinishStep, Current liste değil; satır kartı imzası yok. Run: lg ve üstü ana kolon + 20rem (xl 24rem) ray, altında ray ana kolonun altına iner; faz şeridi `auto-fit minmax(11rem,1fr)`; `.run-action` mobilde tam genişlik, md üstünde en çok 28rem; adım kartında damga sol, içerik sağ, 360 px'te yatay kaydırma yok. Mola ekranı ve FinishStep tek kolon, en çok 42rem. İşlerim görevli adımlar: 360 `1 sütun`, md+ `2 sütun`.

## DNA uygunluğu
Mevcut sınıflar ve token renkler, 2 px kenarlık/köşe; yeni ham renk yok. Hareket: yalnız canlı nokta ve 150 ms renk geçişi; mola açıkken nokta kehribar ve sabit. Yerleşim tarayıcıda doğrulanmadı.

## Öz denetim
- `npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build14` 0 uyarı 0 hata.
- §4 grep tam tablo: 0 ihlal (kalıplar, dash, `...`, ham renk, boyutsuz img). `asp-antiforgery`'siz eylemli POST formu yok (action'sız prototip formları tag helper belirteci alır, eskiden beri).
- Curl (port 5491, f4 DB kopyası, Operatör 2001, süreç kapatıldı): `/my-work` 200, `/records/run/1` ve `/2` 200, `?stepId=1&people=a` 200, `/machine-board` 200, `/records/create` 200; Run'da 5 PIN alanında `maxlength="4"`, form belirteç sayısı POST form sayısı kadar; Create'te 3 select sarmalı. `/breaks/current` 404 (action aşama 2'de).
- Denenmedi: devam eden adım, açık mola, tamamlanmış kayıt, görevli yönetimi ve FinishStep dalları çalışan veriyle (backend action'ları ve veri durumu yok); bu dallar yalnız derlendi. Tarayıcıda JS davranışı (LiveTimer, ParticipantPicker, PinInput yapıştırma) denenmedi.

## Backend'den beklenen/eksik
1. `ParticipantOptions` JS kapalı ret sonrası `PendingParticipant` kişisini de içermeli (adı gösterilebilsin); yoksa ekran "Önceden seçilen kişi" yazar.
2. `BelowMinimumReason` için üst uzunluk sınırı ve ModelState anahtarları `BelowMinimumReason`, `Pin`, `ParticipantUserId` (görünüm bunları okur); genel hata `ErrorMessage`.
3. `OpenBreak` için geçen süre saniyesi yok; Run bandı yalnız başlangıcı gösterir. Gerekirse `OpenBreakViewModel.ElapsedSeconds` eklenir.
4. `/breaks/current` ve komut action'ları gelince curl turu: Operatör `/records/run/{id}` tam akış, İşlerim, mola başlat/bitir, gerekçe formu.

## Açık
- Ön tur ölçümü: select ve göz düğmesi (K26), Run adım satırı hizası, `.run-action` yerleşimi, 360 px taşma.

## Güncelleme (backend aşama 2 sonrası)
- Görevli paneli düzeltmesi: kök neden `Run.cshtml`'de `_Participants` çağrısının `!isLocked` koşuluydu; kilitli (sıradaki olmayan Bekliyor) adımlarda hiç çizilmiyordu. Koşul `(!isLocked || step.CanManageParticipants)` oldu. `?stepId=` ile ilgili adımın fazı açık gelir.
- Açık mola bandına "Mola süresi" (`OpenBreak.ElapsedSeconds`, `_LiveTimer`) eklendi; `/breaks/current` sayacı zaten `ElapsedSeconds`'tan. Sapma: mola süresi sayacı bandda da ilerler (`data-running="true"`); faz ve adım sayaçları `TimersRunning` kuralıyla durur. Mola süresini de dondurmak istenirse tek parametre.
- Tamamlanmış kayıtta mola aracı, mola neden satırı, başlat ve bitir formları `!isCompleted` ile gizli; özet görünür.
- Curl turu (port 5492, `.qa/fe/build15`, `.qa/be/s5/s.db` kopyası, KSA, Operatör 2002, formun kendi belirteci ve CommandId'si; süreç kapatıldı): kayıt aç 302; JS kapalı `?stepId=25&people=Test` radyo listesi (Molada ve Adımda zaten görevli pasif, kendi açık kaydı uyarısı); görevli ekle 302, yanlış PIN ve zaten görevli ret 200 (PendingParticipant ile radyo ve şablon korundu); görevli başka kayıtta devam eden adımdaysa başlatma ret 200 (BR-24 mesajı); görevliyi çıkar 302; başlat 302 (saha referansı görünür, canlı sayaç `data-running="true"`, "Mola iki adım arasında verilir"); bitir 302; mola başlat 302 `/breaks/current`, `/my-work` ve Run 302 `/breaks/current`; diğer operatör Run'da band, "Sayaçlar durdu", `data-running="false"`, başlat formu yok; yanlış PIN ile mola bitir 200 Current; PIN ile 302; faz son adımı gerekçesiz bitir 200 FinishStep, gerekçeyle 302 ("Faz süre altında tamamlandı" ve gerekçe görünür); faz 2 aynı; kayıt tamamlandı özeti (net, brüt, ilk adım başlangıcı), formlar ve "Mola" yok. `/my-work`, `/machine-board`, `/records/create`, `?stepId=27&people=a` 200; 500 yok, günlükte hata yok. (Gerekçedeki Türkçe karakterler konsol kod sayfası yüzünden bozuk gitti, test düzeneği kaynaklı.)
- `npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build15` 0 uyarı 0 hata; §4 grep 0 ihlal.
- Denenmedi: tarayıcıda JS (LiveTimer, ParticipantPicker, PinInput yapıştırma), 360 px yerleşim.

## FE 10 (PL-70, K27): süre altı gerekçe modalı
- `Views/Records/FinishStep.cshtml`: tüm içerik (hata bildirimi, faz, net süre, minimum, gerekçe, PIN, "Gerekçeyle bitir", "Vazgeç") tek `form[data-finish-reason]` içinde; form kendi belirtecini taşır. "Vazgeç" bir bağlantı (`data-finish-cancel`, JS kapalıyken yürütme ekranına döner). Sayfadaki "süre altında" rozeti kaldırıldı (faz henüz tamamlanmamıştı).
- `Views/Records/Run.cshtml`: bitirilebilir adım varken ve kayıt tamamlanmamışken sayfa sonunda `<dialog class="finish-dialog" data-finish-dialog aria-labelledby="finish-dialog-title">` (başlık "Faz süre altında kalıyor", gövde `data-finish-body`). Başka değişiklik yok.
- `CommandForm.js`: yanıtta `form[data-finish-reason]` varsa ve sayfada dialog varsa sayfa değişmez; form dialog gövdesine alınır, `vaka:content` olayı yalnız o gövde için tetiklenir, `showModal()`; odak ilk hatalı alana, yoksa gerekçe alanına. Çıkış formundaki PIN temizlenir, düğme açılır. Dialogdan gönderimde ret aynı biçimde gövdeyi yeniler (gerekçe korunur, hata dialog içinde). Başarıda mevcut kural (aynı adrese yönlenme: `main` yanıttan değişir, açık dialog önce kapatılır). Kapanışta gövde boşaltılır ve odak "Adımı bitir" düğmesine döner. Esc yerel `dialog` davranışı.
- `App.js`: `vaka:content` dinleyicisi `event.detail.root` verilmişse yalnız onu bağlar (aksi halde `main`), çift bağlama olmasın diye. `Styles/app.css`: `.finish-dialog` (çok en 36rem, `92dvh`, taşmada dikey kaydırma).
- Doğrulama: JS kapalı yol (curl, port 5493): gerekçesiz bitirme 200 FinishStep sayfası, `data-finish-reason` ve `data-finish-cancel` var, sayfa yenilense de adım "Devam ediyor"; gerekçeli gönderim 302. JS açık yol Chrome başsız (CDP, 360 px, `.qa/fe/f5/cdp/t6.mjs`, kayıt 8 adım 36): "Adımı bitir" tıklanınca dialog açıldı, adres değişmedi, dialog içinde 1 belirteç, odak `BelowMinimumReason`, dialogda faz, net, minimum görünür; yatay kaydırma yok; iki düğme 56 px; yanlış PIN dialogda "PIN hatalı", gerekçe korundu; "Vazgeç" dialogu kapattı, odak "Adımı bitir"e döndü, adım "Devam ediyor" kaldı; yeniden açıp doğru PIN ve gerekçeyle gönderim: dialog kapandı, adım tamamlandı, kayıt tamamlandı. Esc, açık/koyu tema ve reduced-motion tarayıcıda ölçülmedi (yalnız token renkleri, yeni animasyon yok).
- `npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build17` 0 uyarı 0 hata; §4 grep 0 ihlal.

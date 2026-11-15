# Devir notu: frontend-engineer, Faz 3

Önceki not: Archive/frontend-engineer-20261115-0300.md.

## Yapılanlar
- Menü: Yönetici "Tanımlar" grubuna "Temizlik tipleri" (`/cleaning-types`, `Views/Shared/_Layout.cshtml`). Denetçi menüsüne kalem yok.
- `Views/CleaningTypes/`: `Index` (E-09), `Form` (oluştur/düzenle), `Machines` (eşleme, tesis ve hat gruplu), `Details` (E-10).
- `Views/ProcedureVersions/`: `Edit` (E-11 taslak düzenleyici), `Details` (E-12, salt okunur; Yönetici için "Tip detayı" bağlantısı, Denetçi için yok).
- Ortak parçalar: `_LabelBadge` (etiket ViewModel'den, ikon ve ton enum'dan), `_MediaList`, `_MediaDialog`, `_EditorError`.
- JS: `ProcedureEditor.js`, `MediaViewer.js`, `KindToggle.js`; `App.js` bağlar. Hepsi `data-*` kancalı, JS kapalıyken formlar tam sayfa çalışır.
- `Styles/app.css`: `.tool` (açılır işlem bölgesi, onay için de), `.media-list/.media-item/.media-thumb/.media-name`, `.media-dialog/.media-stage`.
- `Styles/BuildAssets.mjs`: ikon eklendi (arrow-up, arrow-down, trash-2, upload, image, video, send, file-text, list-ordered, list). Not: `list` ikonu Faz 2'de kullanılıyordu (Malzemeler/Lotlar) ama sprite'ta yoktu, şimdi eklendi.

## Düzenleyici davranışı (K11)
- Her işlem ayrı `form[data-editor-form]`; JS fetch ile gönderir, yanıttaki `[data-editor-region]` bölgesini değiştirir.
- Odak: hata varsa `[data-editor-error]` (rol alert, tabindex -1); yoksa formun `data-focus` listesindeki ilk var olan öğe (taşımada aynı yön düğmesi, yoksa karşı yön, yoksa öğenin kendisi; ekleme ve çıkarmada `phases-title` ya da faz), yoksa durum iletisi. Ekran okuyucuya `data-editor-announce` yazılır.
- Yönlenme başka adrese giderse (yayınlanmış versiyon vb.) `location.assign`.
- Dosya ön denetimi: `accept` listesi ve `data-max-photo-mb` / `data-max-video-mb` (ViewModel'den); 1 MB = 1 048 576 çevrimi JS'te tek sabit. Karar sunucuda. 413 ve ağ hatası form içinde mesaj verir.
- Yayın: sözleşmeye uygun tam sayfa form. Planın "PIN diyaloğu" yerine rayda sabit "Yayınla" paneli (blokaj listesi + PIN alanı doğrulama modunda + düğme); engel varken düğme pasif. Diyalog istenirse analist kararı gerekir.
- Çıkarma işlemleri (faz, adım) iç içe `details` ile onay ister, JS gerektirmez. Medya çıkarma tek tıkla (dosya silinmez).
- Hareket: yeni animasyon yok.

## G11 beklenen imzası
Ölçülmedi, tarayıcı kullanılmadı. Aşağıdakiler CSS kurallarından türetilmiş beklentidir.

E-09 `/cleaning-types` (Kod, Ad, Tür, Versiyon, Makine, Durum, İşlem; uzun metin alanı Ad):
- 360: `Ad:2@1 Kod:1@2 Tür:1@2 Versiyon:1@3 Makine:1@3 Durum:1@4 İşlem:1@4`
- 768: `Kod:1@1 Ad:2@1 Tür:1@1 Versiyon:1@2 Makine:1@2 Durum:1@2 İşlem:1@2`
- 1024: 768 ile aynı.
- 1440: tablo, 7 sütun, satır en az 56 px.
- Riskler: Versiyon hücresi iki çip alt alta olabilir (Yürürlükte + Taslak); İşlem hücresinde iki düğme 768'de bir sütuna (yaklaşık 160 px) sığmayıp sarar (Faz 2 makine listesiyle aynı davranış). Tablo modunda düğme, rozet, çip nowrap.

E-10 `/cleaning-types/details/{id}` versiyon tablosu (Versiyon, Durum, Yayın, Doküman no, İşlem; ray yanı, PL-39; uzun metin alanı Yayın = `td.cell-long`):
- 360: `Yayın:2@1 Versiyon:1@2 Durum:1@2 Doküman no:1@3 İşlem:1@3`
- 768 (ray altta, tam genişlik): `Versiyon:1@1 Durum:1@1 Yayın:2@1 Doküman no:1@2 İşlem:1@2`
- 1024-1279 (ray yanında, PL-39 2 sütun): 360 ile aynı.
- 1280 ve üstü: tablo, Yayın sütunu kalan alanı alır, diğerleri nowrap.
- Not: 768'de Yayın sırası DOM sırasındadır (başlık sırasında değil), `stack-lead` yalnız 768 altı ve lg-xl rayda öne alır.

E-12 ve E-11 liste tablosu değildir (kart ve adım listesi); form ve detay kuralı: lg ve üstü ana kolon + 22rem ray, altında ray altta.

## DNA uygunluğu
Mevcut sınıflar kullanıldı (`panel`, `btn`, `badge`, `choice`, `step`, `kv`, `notice`); yeni sınıflar token tabanlı. Yerleşimi tarayıcıda doğrulamadım, "DNA ile aynı" demiyorum; ön tur ve teslim ölçümü qa-tester'da.

## Öz denetim
- `npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build` 0 uyarı 0 hata (ViewModel'lere karşı derlendi).
- §4 grep (transition-all, outline-none, onclick, h-screen, user-scalable, CDN, jquery/gsap/aos, dash, ham renk, boyutsuz img, autocomplete): 0 ihlal. Görünür metinde `...` yok.
- §1 curl turu yapılamadı: `CleaningTypesController`, `ProcedureVersionsController`, `MediaController` henüz yok.

## Backend'den beklenen/eksik
1. `CleaningTypeListViewModel`'de tür seçenekleri yok; süzgeç seçeneklerinin "Planlı" ve "Acil" metinleri görünümde sabit yazılı (değer enum'dan). İstenirse `KindOptions[]: SelectOptionViewModel` eklenmeli.
2. Liste ve detayda Aktif/Pasif rozet metni sabit (`...Label` alanı yok); Faz 2 görünümleriyle aynı.
3. Eşlemede seçilemez ama eşli makine (`IsSelected && !IsSelectable`) işaretli ve `disabled` çizilir, POST'ta gönderilmez. Sunucu gönderilmeyen mevcut eşlemeyi korumalı (sözleşme: "mevcut eşlemesi korunur"). Beklenen davranış farklıysa bildirin.
4. Medya yükleme ipucunda biçim adları (JPEG, PNG, WebP, MP4) sabit, MB değerleri ViewModel'den.
5. `/cleaning-types/create` GET'te `KindOptions` dolu, `Kind` boş gelmeli; `/edit` için `KindLocked = true`. Acil tip düzenlemede `DefaultLockMinutes` alanı çizilmez.
6. Hata sonrası `Error.Target = header` için `MaterialRequired` kullanıldı (koordinatörün bildirdiği ekleme).
7. Medya fotoğrafları `/media/{id}` ile küçük önizlemede de tam dosya olarak yüklenir (küçük resim üretimi yok); `loading="lazy"`.

## Açık
- Controller'lar gelince QA veya ben §1 curl turunu çalıştırmalı (iki boş durum, taslak olmayan kimlikle düzenleyici yönlenmesi, Denetçi 404/403).

## PL-53 güncellemesi
- Yayın paneli rayda, `[data-editor-region]` içindedir; her fetch işleminden sonra bölge yanıttan değiştiği için engel listesi ve düğme durumu (`disabled`) güncel yayın engelleriyle yeniden çizilir. Ek JS gerekmedi. JS kapalıyken tam sayfa akış aynı.
- Tür süzgeci `Model.KindOptions` (SelectOptionViewModel) üzerinden çizilir; "Planlı"/"Acil" görünümde sabit değil. Backend `KindOptions` alanını eklemeden derleme `CS1061` verir (Index.cshtml:26). Ayrıca backend'in yarım işinden `CommandStatus.NotEditable` hatası (CommandResult.cs:46) var, benim kapsamımda değil.
- Doğrulama: build:css tamam, §4 grep 0 ihlal; `dotnet build -warnaserror` yalnız bu iki backend alanı eksikliği yüzünden geçmiyor, `KindOptions` gelince yeniden çalıştırılmalı.

## FE 9 (PL-54, G12) ve düzeltmeler
Tarayıcıda ölçülmedi; aşağıdaki "beklenen: kusur 0" CSS ve işaretlemeden türetilmiştir.
- Kök neden düzeltmesi: `Views/Shared/_PinInput.cshtml` ve `_Field.cshtml` (parola) göz düğmesi `.field-reveal` (`absolute top/right/bottom 2px, w-11, h-auto`): kenarlık içinde, yüksekliği alanla aynı, dikey merkez alanla aynı, 44 px genişlik, odak halkası aynen. Not: `_Field` parola alanı Faz 1 giriş ve güvenlik ekranlarında; prototipler dışında başka gömülü düğme/ikon yok (arama alanlarında ikon yok).
- Aynı satır kontrol ve düğme yüksekliği: Faz 2-3 liste süzgeç formlarında (Users, Machines, Materials, Facilities, Shifts, CleaningTypes) düğmeler `min-h-12` (alan 48 px, düğme 44 idi). Machines süzgecinde ek değişiklik yok (onay kutusu ve düğmeler zaten ikinci satırda).
- Faz 3 yeni ekranlarda aynı satır kusuru bulunmadı; panel içi sol/sağ kenar `panel-body`/`p-4 md:p-5` düzeninde.
- Ekran ve genişlik başına beklenen (360, 768, 1024, 1440; açık ve koyu): /account/login, /account/first-login, /account/security (PIN/parola kartları): kusur 0; /users, /machines, /materials, /facilities, /shifts, /cleaning-types: süzgeç satırı kusur 0; diğer form ve detay ekranları kusur 0 (değişmedi, ölçülmedi).
- G11 imzaları ve Faz 1-2 mekanik eval değişmez: yalnız süzgeç düğmesi 44 -> 48 px (hedef 44 üstü, `smallTargets` etkilenmez), liste hücreleri dokunulmadı.

## Backend isteği düzeltmeleri
1. `_MediaList` modeli `StepMediaViewModel[]` yapıldı (ValueTuple kovaryans 500'ü). Diğer tuple partial'ları tür uyumu açısından tarandı (string/array/Func, tam eşleşme); sorun yok.
2. Tip formu `IsActive` `asp-for` ile (gizli false alanını üretir). Etiketin yinelenen `id` değeri kaldırıldı.
3. Yanıt biçimi teyidi: görünüm `Error.Target` değerlerini aynen kullanır (`header`, `add-phase`, `phase:`, `add-step:`, `step:`, `media:`, `publish`). Uyuşmazlık yok. Medya çıkarma hedefi yalnız `media` (stepId yok): görünüm `media:<stepId>` bölgesini açar, çıkarma hatası hiçbir forma bağlanmaz; hata `[data-editor-error]` olarak çizilmediği için görünmez. Backend `media:<stepId>` döndürmeli ya da bildirmeli.
- curl turu (port 5481, h.db kopyası, Yönetici): `/cleaning-types` (kind=1 dahil), `details/1`, `create`, `edit/1`, `machines/1`, `/procedure-versions/edit/4`, `details/1`, `details/3`: hepsi 200, h1=1, dash 0. Reddedilen POST yeniden çizimi denenmedi. Süreç kapatıldı.
- Öz denetim: build:css tamam, `dotnet build -warnaserror` 0 uyarı 0 hata, §4 grep 0.

## PL-55: kart modunda İşlem hücresi ve hover
Tarayıcıda ölçülmedi; imzalar CSS ve işaretlemeden türetilmiş beklentidir.

Yöntem ve gerekçe: İşlem hücresine (`td.cell-action`) görünümde ekran başına `col-span` sınıfı verildi (örn. `max-md:col-span-2 md:max-xl:col-span-3`). Neden: satırdaki alan sayısı tabloya göre değişiyor, CSS ile genel hesap (`:has`, `nth-child`) kırılgan; sınıf okunur ve tablo başına tek satır. Kart modunda hücre `flex justify-end`, etiketi yok (`::before` kapalı), içindeki `.action-row` kart modunda nowrap, xl ve üstünde `flex-wrap` (tablo davranışı aynı). 1 sütuna sığma kararı ölçülmeden verildi: yalnız tek "Düzenle" düğmeli tablolarda (Shifts, Facilities hatlar, Materials lotlar 360 hariç) 1 sütun, "Görüntüle" ya da çip ("Yalnız görüntüleme") ve iki düğmede 2 sütun kendi satırı. QA 360'ta sığmayan düğme görürse o tabloyu 2 sütuna çekerim.
CleaningTypes Index'te 768-1279 dengeleme için Versiyon hücresi 2 sütun (`md:max-xl:col-span-2`), İşlem 4 sütun kendi satırı.

Beklenen imzalar (360 / 768 / 1024 / 1440; 1440 hepsinde tablo, değişmedi). Sütun:genişlik@satır:
- /users (Sicil no, Ad soyad uzun, Roller, Durum, İşlem): 360 `Ad:2@1 Sicil:1@2 Roller:1@2 Durum:1@3 İşlem:2@4`; 768 ve 1024 `Sicil:1@1 Ad:2@1 Roller:1@1 Durum:1@2 İşlem:3@2`.
- /machines: 360 `Ad:2@1 Kod:1@2 TesisHat:1@2 Durum:1@3 İşlem:2@4`; 768 ve 1024 `Kod:1@1 Ad:2@1 TesisHat:1@1 Durum:1@2 İşlem:3@2` (PL-55 örneği).
- /materials (Kod, Ad uzun, Lotlar, Durum): 360 `Ad:2@1 Kod:1@2 Lotlar:1@2 Durum:1@3 İşlem:2@4`; 768 ve 1024 `Kod:1@1 Ad:2@1 Lotlar:1@1 Durum:1@2 İşlem:3@2`.
- /facilities (Kod, Ad uzun, Hat, Vardiya, Durum): 360 `Ad:2@1 Kod:1@2 Hat:1@2 Vardiya:1@3 Durum:1@3 İşlem:2@4`; 768 ve 1024 `Kod:1@1 Ad:2@1 Hat:1@1 Vardiya:1@2 Durum:1@2 İşlem:2@2`.
- /shifts (Ad uzun, Başlangıç, Bitiş, Durum): 360 `Ad:2@1 Başlangıç:1@2 Bitiş:1@2 Durum:1@3 İşlem:1@3`; 768 ve 1024 `Ad:2@1 Başlangıç:1@1 Bitiş:1@1 Durum:1@2 İşlem:3@2`.
- /cleaning-types (Kod, Ad uzun, Tür, Versiyon, Makine, Durum): 360 `Ad:2@1 Kod:1@2 Tür:1@2 Versiyon:1@3 Makine:1@3 Durum:1@4 İşlem:2@5`; 768 ve 1024 `Kod:1@1 Ad:2@1 Tür:1@1 Versiyon:2@2 Makine:1@2 Durum:1@2 İşlem:4@3`.
- /facilities/details hatlar (ray yanı; Kod, Ad uzun, Makine, Durum): 360 ve 1024-1279 `Ad:2@1 Kod:1@2 Makine:1@2 Durum:1@3 İşlem:1@3`; 768 (ray altta) `Kod:1@1 Ad:2@1 Makine:1@1 Durum:1@2 İşlem:3@2`.
- /materials/details lotlar (Lot no uzun, SKT, Durum): 360 ve 1024-1279 `Lot no:2@1 SKT:1@2 Durum:1@2 İşlem:2@3`; 768 `Lot no:2@1 SKT:1@1 Durum:1@1 İşlem:4@2`.
- /cleaning-types/details versiyonlar (Versiyon, Durum, Yayın uzun, Doküman no): 360 ve 1024-1279 `Yayın:2@1 Versiyon:1@2 Durum:1@2 Doküman no:1@3 İşlem:2@4`; 768 `Versiyon:1@1 Durum:1@1 Yayın:2@1 Doküman no:1@2 İşlem:3@2`.
Faz 1-2'de değişen yalnız İşlem hücresi (genişlik, sağa hiza, etiketsiz); diğer hücre imzaları aynı. Önceki "E-09 ve E-10" imzaları bu listeyle değiştirildi.

Hover: kart modunda (`max-xl`) `tr:hover` satırın tamamına `sunken/50`, `td` şeffaf; xl ve üstünde tablo hover'ı (td) aynı.

PIN göz düğmesi teyidi: `_PinInput` ve `_Field` (parola) sarmalayıcıları `relative`; derlenmiş CSS'te `.field-reveal` `.icon-btn`'den sonra geliyor (position, top/bottom, `h-auto` kazanır). Eski CSS önbelleği büyük olasılık; `asp-append-version` css için açık, sert yenileme yeter.

Öz denetim: build:css tamam; `dotnet build -warnaserror -o .qa/fe/build3` 0 uyarı 0 hata; §4 grep 0.

## Ön tur düzeltmeleri
Tarayıcıda ölçülmedi.
1. `Materials/Details.cshtml` Lot no hücresine `stack-wide stack-lead` eklendi. Beklenen: 360 ve 1024-1279 `Lot no:2@1 SKT:1@2 Durum:1@2 İşlem:2@3`; 768 `Lot no:2@1 SKT:1@1 Durum:1@1 İşlem:4@2`.
2. `.input[type=date], .input[type=time]` `height: 3rem` (LotNo ve tarih 48 px; /shifts saat alanları dahil). `MaterialLots/Form.cshtml` iki sütunlu ızgara `md:` yerine `xl:` eşiğinden başlar (1024'te ray yanında tek sütun, tarih alanı tam genişlik). Beklenen: 768 ve 1024 tek sütun, 1440 iki sütun.
3. `CleaningTypes/Form.cshtml:86` gizli `IsActive=true` yalnız ekleme dalında (`isCreate`); düzenlemede yalnız `asp-for` kutusu ve onun gizli false alanı var, yani hata düzenleme formunda yok. Curl ile doğrulandı (h.db kopyası, Yönetici): `IsActive=false` POST 302, DB'de `IsActive = 0`, `?showInactive=true` listede Pasif rozeti. Süreç kapatıldı.
Öz denetim: build:css tamam, `dotnet build -warnaserror -o .qa/fe/build4` 0 uyarı 0 hata, §4 grep 0.

## Teslim testi B1, B2
- B1: `method="post"` formlarının tümüne (Faz 1-3, prototip hariç) `asp-antiforgery="true"` eklendi; kalan eksik form 0 (grep). `ProcedureEditor.js` `new FormData(form, submitter)` ile formdaki belirteci gönderiyor.
- Curl (h.db kopyası, her formun kendi belirteciyle): üst bilgi 302, faz ekle 302, adım ekle 302, medya yükle 200 (izinsiz dosya, hata ile yeniden çizim), yayınla 200 (yanlış PIN ile yeniden çizim); hiçbiri 400 değil. "Yeni taslak aç" formunun belirteçli çizildiği kaynak kontrolüyle doğrulandı, POST'u denenmedi.
- B2: `Edit.cshtml` faz adı ve minimum süre ızgarası `xl:grid-cols-2` (1024'te tek sütun). Tarayıcıda ölçülmedi.
- Öz denetim: build:css tamam, `dotnet build -warnaserror -o .qa/fe/build5` 0 uyarı 0 hata, §4 grep temiz.

## B3, B4: onay kutusu ve gizli false sırası
- Kök neden: elle yazılmış gizli `false` kutudan önce geliyordu. Düzeltme: gizli alan kutudan sonraya alındı (tag helper düzeni). Yerler: `ProcedureVersions/Edit.cshtml` (`MaterialRequired`), `Facilities/Form`, `Lines/Form`, `Materials/Form`, `Shifts/Form` (`IsActive`). `CleaningTypes/Form` zaten `asp-for` (sıra doğru).
- Tarama: Views/** içindeki tüm `type="checkbox"`: bool olanlar yukarıdakiler; `showInactive`, `showRetired` GET süzgeci (gizli alan yok), `SelectedMachineIds`, `SelectedRoles` çoklu seçim. Başka aynı kalıp yok.
- Doğrulama (h.db kopyası, formdan dönen gövde tarayıcı sırasıyla aynen gönderildi): tesis, hat, malzeme, vardiya, tip, düzenleyici malzeme zorunlu için dokunmadan kaydet 302 ve değer aynı (1); kutu işaretsiz 302 ve DB 0; kutu işaretli 302 ve DB 1. Süreç kapatıldı.
- Öz denetim: build:css tamam, `dotnet build -warnaserror -o .qa/fe/build6` 0 uyarı 0 hata, §4 grep temiz.

## G5b (PL-61)
- Elle yazılmış gizli false kalmadı: `Views/**` içinde `type="hidden"` ve `value="false"` birlikte 0 eşleşme. Facilities, Lines, Materials, Shifts `IsActive`; ProcedureVersions/Edit `MaterialRequired` artık `<input type="checkbox" asp-for=... />` (ad aynı, etiket sarmalayıcı ve sınıflar aynı). Edit'te hata sonrası gönderilen değer, `Error.Target == header` iken görünümde `Model.MaterialRequired`'a yazılıp `asp-for` ile çiziliyor.
- Üç durum (formdan dönen gövde, tarayıcı sırasıyla): tesis, hat, malzeme, vardiya, tip, düzenleyici malzeme zorunlu: dokunmadan 302 değer aynı, işaretsiz DB 0, işaretli DB 1.
- Düzenleyici JS akışı: `new FormData(form, submitter)` formu belge sırasıyla serileştirir (kutu, sonra tag helper'ın gizli false'u), gövde curl denemesiyle aynı; tarayıcıda JS ile çalıştırılmadı.
- Öz denetim: build:css tamam, `dotnet build -warnaserror -o .qa/fe/build7` 0 uyarı 0 hata, §4 grep temiz.

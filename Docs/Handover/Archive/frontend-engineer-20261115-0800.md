# Devir notu: frontend-engineer, Faz 4

Önceki not: Archive/frontend-engineer-20261115-0500.md.

## Yapılanlar
- `Views/MyWork/Index.cshtml` (E-13), `Views/Records/Create.cshtml` (E-14), `Views/Records/Run.cshtml` (E-15, Açıldı hali), `Views/MachineBoard/Index.cshtml` (E-16). `@model` tipleri `Vaka.Web.ViewModels.Records`; backend ViewModel'lerine karşı derlendi.
- Ortak parçalar: `_Duration` (`DurationPart[]`, rakam mono, birim sans, T-B14), `_Moment` (saat, farklı günse tarihli), `_FieldError`, `_TypeChoice`, `_LotPicker`, `_LotRow`, `_RecordSkeleton`.
- Rozet eşlemesi tek yerde: `_Badge` içinde `EnumKeys` (`RecordStatus.X`, `PhaseStatus.X`, `StepStatus.X`/`StepStatus.Ready`, `MachineState.X`, `CleaningTypeKind.X` -> sözlük anahtarı). Yeni sözlük girdisi: `machine.free` (Boş). Görünümler `"RecordStatus." + Model.Status` biçiminde çağırır.
- Menü (`_Layout`): Operatör İşlerim, Yeni kayıt, Makine panosu; Yönetici ve Denetçi Ana sayfa + İzleme grubunda Makine panosu.
- JS: `CommandForm.js`, `LotPicker.js`, `TypePicker.js` (yeni); `CascadeSelect.js` genelleştirildi (`data-cascade-param/noun/none/busy`, hedefe `change` olayı yollar, zincirleme tesis -> hat -> makine -> tip). `App.js` bağlayıcıları `bindContent(root)` oldu; `CommandForm` içeriği değiştirince `vaka:content` olayıyla yeniden bağlanır. Faz 3 `data-cascade-url` kullanımları (Machines/Form, süzgeçler) varsayılanlarla aynı çalışır.
- CSS: `.dur-unit`, `.machine-grid/.machine-tile`, `.lot-results/.lot-result/.lot-row`.

## Davranışlar
- `CommandForm`: `form[data-command-form]` fetch ile POST (URL kodlu gövde, formun kendi belirteci). Yanıt `redirected` ve adres sayfadan farklıysa `location.assign`; aksi halde (reddedilen 200 ya da aynı adrese yönlenme, ör. malzeme ekle/çıkar sonrası `/records/run/{id}`) `main` içeriği yanıttan değiştirilir, odak ilk `aria-invalid` ya da `data-form-summary`'ye gider. Ağ hatası ya da `navigator.onLine === false`: form ve `CommandId` değişmez, "İşlem yapılmadı, bağlantı yok…" gösterilir, düğme açılır. 400 ve 5xx: kod ile mesaj, değiştirme yok. `refresh` düğmesi yakalanmaz. Komut düğmelerinde `data-requires-online` bilerek yok: çevrimdışıyken pasif düğme geri bildirim vermez, mesaj gösterilir (K13).
- Sözleşmeden sapma: "yanıt adresi farklıysa" kuralını `response.redirected` ile birlikte uyguladım. Aksi halde reddedilen add-material (POST adresinde 200) yanlışlıkla `location.assign` ile POST adresine giderdi. Backend'in ret yanıtı yönlendirme değil doğrudan görünüm olmalı (sözleşmeyle aynı).
- Create, JS yok: tesis/hat/makine `refresh=true` düğmesiyle ("Seçimi uygula") yenilenir; JS varken düğme gizlenir. Tip listesi sunucudan (makine seçiliyse). Lot seçimi ve ekleme JS gerektirir (arama JSON ucuyla); JS yokken `noscript` uyarısı. Run'daki malzeme ekleme de aynı. Planlı malzemesiz tipte lot isteğe bağlı.
- Tip değişince kilit süresi tip varsayılanına gelir (kullanıcı elle değiştirmediyse). Malzeme notu tipe göre değişir. Meşgul makine: seçenek `disabled` ve metin "Ad - neden"; ayrıca seçenek altında neden listesi.
- Lot satırı: SKT geçmişse gerekçe alanı zorunlu çıkar; ret sonrası `Materials[]` görüntü alanlarıyla yeniden çizilir. Çoklu satırda `Materials[i].*` adları ekleme/çıkarmada yeniden numaralanır.
- Run: yalnız `CanManageMaterials` iken ekle/çıkar formları (her biri `.tool`, kendi PIN alanı, kendi belirteci). Faz ve adımlar salt okunur; ilk bekleyen/devam eden faz `details` açık. Adım "Hazır" rozeti `IsNext` ile. `ErrorMessage` üstte `role=alert`.
- T-B22: başlık `kv` ızgarası 2 sütun (lg-2xl altı), 4 sütun (md-lg ve 2xl+); kayıt no ve saha defteri `whitespace-nowrap`. Tarayıcıda ölçülmedi.
- Pano: filtre (GET, JS'siz çalışır), durum sayıları tek satır (dört kart yok), tesis/hat grubu başına kart ızgarası, durum rozet metni + üst çizgi biçimi; Opened'da kilit bitişi. Kayıt no bağlantısı `RecordUrl` doluysa.
- İşlerim: ViewModel'de yalnız Faz 4 alanları var; aktif kayıt kartı, Açıldı ise kilit bitişi bildirimi, yan panelde yeni kayıt durumu (`CannotOpenReason`, `CannotOpenUrl`).
- Dört durum: yükleniyor (`_RecordSkeleton`/`_ListSkeleton`, `data-busy-link`/`data-busy-form` ile gezinirken), boş, dolu çizildi. Hata: sayfa ViewModel'i hata taşımıyor; sunucu hatası `Error/Failure` sayfasına gider. Yazma hataları `ErrorMessage` ile.

## G11 imzası
`/my-work`, `/records/create`, `/records/run/{id}` ve `/machine-board` liste tablosu değildir (kart, form, ızgara); satır kartı imzası yok. Ölçülmedi, beklenen: pano kart ızgarası 360 `1 sütun`, 640+ `2`, 1280+ `3`, 1536+ `4`; yatay kaydırma yok. Form ve detay: lg ve üstü ana kolon + 22rem (Run 20rem, xl 24rem) ray, altında ray ana kolonun altında.

## DNA uygunluğu
Mevcut sınıflar (`panel`, `notice`, `choice`, `phase-cell`, `step`, `kv`, `badge`) ve token renkler; kenarlık 2 px, köşe 2 px. Yerleşimi tarayıcıda doğrulamadım, "DNA ile aynı" demiyorum.

## Öz denetim
- `npm run build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build9` 0 uyarı 0 hata.
- §4 grep (transition-all, outline-none, onclick, h-screen, user-scalable, CDN, jquery/gsap, dash, `...`, ham renk, boyutsuz img, gizli false, antiforgery'siz post formu): 0 ihlal.
- §1 curl turu yapılamadı: `MyWorkController`, `RecordsController`, `MachineBoardController` henüz yok. Tarayıcıda JS davranışı (CommandForm, LotPicker, TypePicker, CascadeSelect zinciri) denenmedi; controller gelince curl ve QA.

## Backend'den beklenen/eksik
1. Reddedilen komut yanıtı yönlendirme değil, aynı görünüm 200 olmalı (yukarıdaki sapma).
2. `/records/lots?q=` boş `q` ile de sonuç dönmeli (odaklanınca ilk liste); çok kayıt varsa sınırlı döndürsün. JSON camelCase, `expiryDate` `yyyy-MM-dd`.
3. `/records/cleaning-types` ve ret sonrası `Create` yeniden çiziminde `CleaningTypes[]` seçili makineye göre dolu olmalı; `refresh=true` yanıtında `CommandId` ve girilen alanlar korunmalı.
4. Malzeme ekle reddinde `RecordRunViewModel.ErrorMessage` dolu; seçilen lotun ret sonrası yeniden çizimi Run'da yok (tek lot, yeniden seçilir). İstenirse Run'a `PendingMaterial` eklenebilir.
5. ModelState anahtarları: `FacilityId, LineId, MachineId, CleaningTypeId, WorkOrderNo, LockMinutes, Pin, Materials[i].ExpiredReason` (görünüm bunları okur). Genel hata `ErrorMessage`.
6. No-JS lot seçimi için `AvailableLots[]` gerekir; sözleşmede yok, eklenmedi.

## Açık
- Controller'lar gelince §1 curl turu (Operatör: my-work, records/create, records/run, machine-board; Yönetici/Denetçi: machine-board; SuperAdmin 403).

## PL-64 güncellemesi (JS kapalı lot seçimi, PendingMaterial)
- `noscript` uyarısı kaldırıldı. Yeni `_LotFallback`; JS kapalı öğeler `data-lot-nojs`, JS açık öğeler `data-lot-js`. `App.js` `<html data-js>` yazar, CSS ikisini birbirine karşı gizler; `LotPicker.js` açıkken `[data-lot-nojs]` içindeki alanları `disabled` yapar (çift `ExpiredReason` gönderimi olmasın).
- Create, JS kapalı: `LotQuery` + "Lotları ara" (`refresh=true`); "Ekle" düğmesi `AddMaterialLotId`, gerekçe `AddExpiredReason`; satırda "Çıkar" düğmesi `RemoveMaterialIndex`. Ekle ve Çıkar `formaction="/records/create?refresh=true"` ile gider (düğme adı tek olduğu için `refresh` sorgudan okunmalı; backend sorgu dizgesinden de bağlamalı). Hepsi `formnovalidate`. İlk submit düğmesi "Seçimi uygula" (refresh) olduğundan Enter kaydı açmaz.
- Run, JS kapalı: ayrı GET formu `?q=`, `AvailableLots` radyo listesi (`MaterialLotId`), gerekçe `ExpiredReason`, `AvailableLotsTruncated` ise "Aramayı daraltın". `LotQuery` ya da `PendingMaterial` doluyken araç açık gelir. `PendingMaterial` seçili radyo ve gerekçeyi, JS açıkken `AvailableLots` içinden bulunan lotla satırı yeniden çizer (listede yoksa satır çizilmez).
- Doğrulama: build:css tamam, §4 grep ve antiforgery taraması 0. `dotnet build -warnaserror` görünümlerde hata vermiyor; tek hata backend/dbadmin tarafında: `Data/Migrations/20261115021022_AddCleaningRecordsAndEventLog.cs(493,90)` CS8632 (nullable açıklama, `#nullable` yok). Benim kapsamım dışı.

## Create 500 düzeltmesi (tuple modeli ve kovaryans)
- Kök neden: `_LotPicker` ve `_LotFallback` tuple modelinde `IReadOnlyList<...>`, çağıran dizi veriyordu; ValueTuple kovaryant değil. Düzeltme: tuple öğesi `MaterialEntryViewModel[]` ve `LotOptionViewModel[]` (somut dizi) yapıldı, `.Count` yerine `.Length`.
- Tarama (`@model (` içeren tüm partial'lar): tuple içinde arayüz/koleksiyon tipi taşıyan yalnız bu ikisi vardı; `_Pagination` (`Func<int,string>`) çağrıları açık cast ediyor (Faz 3'te düzeltilmişti). Diğer tuple partial'ları yalnız string, int, bool, DateTime, `int?` içeriyor ve çağrılar tam tür veriyor (`(int?)null`, `(string?)null`). Tuple olmayan `IReadOnlyList<(...)>` (`_PhaseStrip`) ve `IReadOnlyList<DurationPart>` (`_Duration`) doğrudan atanabilir tür, kovaryans sorunu yok.
- Curl turu (port 5483, build11, h.db yerine vaka.db kopyası `.qa/fe/f4/f4.db`, test kullanıcıları; her formun kendi belirteciyle; süreç kapatıldı):
  - Operatör: `/my-work` 200, `/records/create` 200, `/machine-board` 200, `/` 302 `/my-work`, `/records/run/1` 200, `?q=` 200; h1=1, dash 0, dış kaynak 0. Yönetici `/machine-board` 200. Oturumsuz 302.
  - JS kapalı akış: `refresh=true` ile tesis, hat, makine, tip listesi yenilendi, `CommandId` korundu; `?refresh=true` + `LotQuery` listeyi getirdi; `AddMaterialLotId=1` satırı ekledi (SKT geçmiş gerekçe satırı çizildi); gerekçesiz kayıt açma `Kayıt açılamadı` (SKT gerekçesi) ile reddedildi (CommandId aynı), gerekçeyle 302 `/records/run/1`.
  - Run: formda belirteç sayısı form sayısına eşit (GET formu hariç); `add-material` (lot 2, form belirteci) 302 `/records/run/1`, malzeme listede.
  - 500 yok.

## Run adım satırı hizası (G12)
- `Views/Records/Run.cshtml`: başlık ve rozet satırına `min-h-11` (damgayla aynı 44 px, `items-center`). Beklenen: tek satırda başlık ve rozet dikey merkezi damga merkeziyle aynı (768, 1024, 1440). Sarılan başlıkta ve 360'ta rozet başlıkla aynı blokta kalır, blok damganın üst kenarından başlar. Açıklama metni bloğun altında. Faz 5 Başlat/Bitir düğmesi aynı satır bloğuna (`items-center`) eklenebilir. Tarayıcıda ölçülmedi; ön tur yeniden ölçmeli.
- Öz denetim: build:css tamam, `dotnet build -warnaserror -o .qa/fe/build12` 0 uyarı 0 hata, §4 grep ve antiforgery taraması 0.

## B1: JS kapalı lot akışı
- (a) `Views/Records/Create.cshtml` formuna `action="/records/create"` (asp-antiforgery kalır); `formaction="...?refresh=true"` düğmeleri artık sayfa adresini `?refresh=true`'da bırakmıyor.
- (b) `_LotPicker` kök öznitelikleri `Html.Raw` ile yalnız gerektiğinde yazılıyor: `data-lot-js` yalnız tek lot modunda (Run), `data-lot-required` yalnız `Required` iken; çoklu modda (Create) hiç yok. Tek lot modunda seçili satır artık `<template data-lot-pending>` içinde (JS kapalıyken formda alan yok, çift `MaterialLotId`/`ExpiredReason` olmaz); `LotPicker.js` açılışta şablonu listeye ekler. JS kapalıyken gizli kalan hiçbir alanda `required` yok (satırdaki gerekçe alanı yalnız SKT geçmişte hem görünür hem `required`).
- Doğrulama (JS kapalı, tarayıcı gibi form gövdesiyle, port 5483, vaka.db kopyası, süreç kapatıldı): tesis/hat/makine yenileme, SKT geçmiş lot ekle (gerekçe alanı görünür, JS'e özel "Çıkar" gizli, JS'siz "Çıkar" görünür), gerekçesiz "Kaydı oluştur" 200 + "SKT geçmiş lot için gerekçe girin", çıkar, yeniden ekle (gerekçe korundu), "Kaydı oluştur" 302 `/records/run/2`. Form eylemi `/records/create`. JS açık akış tarayıcıda denenmedi (JS davranışı değişikliği: yalnız `data-lot-pending` şablonu ekleme).
- Öz denetim: build:css tamam, `dotnet build -warnaserror -o .qa/fe/build13` 0 uyarı 0 hata, §4 grep ve antiforgery taraması 0.

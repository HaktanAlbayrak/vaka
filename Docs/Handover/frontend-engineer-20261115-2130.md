# Devir notu: frontend-engineer, Faz 6A (E-27, E-30, E-31)

Önceki not: Archive/frontend-engineer-20261115-1900.md.

## Ekranlar ve dosyalar
- `Views/Emergencies/Create.cshtml` (E-27): Faz 4 yeni kayıt düzeni (ana kolon + "Gönderince" ve "Aynı kurallar" rayı). Form `action="/emergencies/create"`, `asp-antiforgery="true"`, `data-validate data-command-form data-unsaved-guard`, tek gizli alan `CommandId`. Tesis, hat, makine çağlayanı `CascadeSelect.js` (tesis için `/records/lines`, hat için `/emergencies/machines?lineId=`), JS kapalıyken "Seçimi uygula" (`refresh=true`). Acil tip: eşleme aranmaz, tüm `EmergencyTypes` radyo (`.choice`, versiyon ve "Malzeme gerekli/gerekmez" metni). Açıklama zorunlu textarea (`Description`, maxlength 1000). Malzeme: `_LotPicker` (multi) + `_LotFallback` (LotQuery, AddMaterialLotId, AddExpiredReason, RemoveMaterialIndex). PIN `_PinInput` (`Pin`). Gönder düğmesi `btn-danger btn-lg` "Kaydı aç ve başlat" (siren ikonu). `ErrorMessage` + `ErrorUrl` ("Kaydı görüntüle") ve ModelState (`_FormSummary`). `EmergencyTypes` boşsa form yerine boş durum ("Acil tip tanımlı değil").
- `Views/Shared/_LotFallback.cshtml` ve `_LotRow.cshtml`: JS kapalı ekle/çıkar `formaction`ı `ViewData["LotRefreshAction"]` okur, varsayılan `/records/create?refresh=true` (Records/Create ve Run değişmedi). Acil ekranı `/emergencies/create?refresh=true` verir.
- `Views/MyWork/Index.cshtml`: `CanOpenEmergency` ise "Acil müdahale" paneli ve "Acil müdahale aç" düğmesi (`btn-danger btn-lg`, 360'ta tam genişlik), "Görevli olduğum adımlar"ın üstünde.
- `Views/Records/Run.cshtml`: başlık bloğunda `EmergencyDescription` ("Acil durum açıklaması"); "Saha defteri" acil kayıtta "Yok" (planlıda eskisi gibi "Atanmadı"). `Details.cshtml` aynı "Yok" kuralı (`EmergencyDescription` zaten vardı). Liste ve detayda "Acil" rozeti Faz 6 `CleaningTypeKind.Emergency` rozetiyle zaten geliyor, saha referansı boşsa liste hücresi çizmiyor.
- `Views/Reports/Durations.cshtml` (E-30, yalnız tablo): GET süzgeç (`from, to, kind, cleaningTypeId, facilityId, lineId, machineId`), "Kayıt durumları" şeridi (`StatusCounts`, kart ızgarası değil `kv`), makine bazında (brüt, mola, net, efor, kayıt), faz bazında (asgari süre, kayıt, ortalama net, süre altı çipi, versiyon ayrı satır), en uzun süren adımlar. Süreler `IDurationFormatter.Format(int)`; görünümde hesap yok. Üç tablo da boşsa süzgeçli ve süzgeçsiz iki boş durum; yükleniyor `_ListSkeleton` (form gönderme).
- `Views/Reports/Materials.cshtml` (E-31): "Lota göre" / "Makineye göre" bağlantı sekmeleri (`?mode=Lot|Machine`, JS gerekmez). Lot: iki GET formu (1. `LotQuery` ile "Lotları bul", 2. `MaterialLotId` select + tarih + "Sorgula"; ikincisi `LotQuery`yi gizli alanla taşır, `LotOptions` boşken gösterilmez), `LotOptionsTruncated` uyarısı. Makine: tek form. Sonuç tabloları `LotUsages` ve `MachineMaterials`, SKT geçmiş rozeti, "Ayrıntı" bağlantısı (`Url`). Durumlar: sorgu yok (`HasResult` false), sonuç boş (aralıklıysa "Aralığı temizle"), dolu. `LotPicker.js` kullanılmadı: `/records/lots` yalnız Operatör'e açık, rapor rolleri için JSON ucu sözleşmede yok; akış JS'siz da JS'li de aynı.
- Menü (`_Layout`): Yönetici ve Denetçi için "Raporlar" grubu (Süre ve efor `chart-column`, Malzeme izlenebilirliği `route`), Kayıtlar/Makine panosundan sonra, Tanımlar'dan önce. Alt çubuk ilk 3 bağlantıyı aldığı için değişmedi. Operatör menüsünde acil yok (yalnız İşlerim düğmesi, sözleşme gereği).
- Yeni ikon: `siren` (BuildAssets listesi, sprite yeniden üretildi, 83 ikon).

## G11 beklenen imza (ölçüldü: 360, 768, 1024, 1440; gerçek veriyle kopya DB, Chrome CDP, ilk iki satır; 1440'ta tablo; yatay kaydırma 0)
Ölçüm kutusu: satır ızgara hattı (`Alan:genişlik@satır`). Raporlarda İşlem sütunu yok, boş yarım hücre yok.
- Süre, makine bazında (uzun alan: Makine ve tip): 360 `MakineTip:2@1 Kayıt:1@2 Brüt:1@2 Mola:1@3 Net:1@3 Efor:2@4`; 768 ve 1024 `MakineTip:2@1 Kayıt:1@1 Brüt:1@1 Mola:1@2 Net:1@2 Efor:2@2`.
- Süre, faz bazında (uzun: Tip ve faz): 360 `TipFaz:2@1 Asgari:1@2 Kayıt:1@2 OrtNet:1@3 SüreAltı:1@3`; 768 ve 1024 `TipFaz:2@1 Asgari:1@1 Kayıt:1@1 OrtNet:2@2 SüreAltı:2@2`.
- Süre, en uzun adımlar (uzun: Adım ve tip): 360 `Adım:2@1 Faz:1@2 Kayıt:1@2 OrtSüre:1@3 EnUzun:1@3`; 768 ve 1024 `Adım:2@1 Faz:1@1 Kayıt:1@1 OrtSüre:2@2 EnUzun:2@2`.
- Malzeme, lota göre (uzun: Makine): 360 `Makine:2@1 KayıtNo:1@2 Açılış:1@2 Durum:1@3 İşlem:1@3`; 768 ve 1024 `KayıtNo:1@1 Makine:2@1 Açılış:1@1 Durum:2@2 İşlem:2@2`.
- Malzeme, makineye göre (uzun: Malzeme): 360 `Malzeme:2@1 Giriş:1@2 KayıtNo:1@2 Lot:1@3 SKT:1@3 İşlem:2@4`; 768 ve 1024 `Giriş:1@1 Malzeme:2@1 KayıtNo:1@1 Lot:1@2 SKT:1@2 İşlem:2@2`.
- 1280 ve üstü: tablo, satırlar `h-14` (56 px) kuralı `.data-table` ile gelir. Ölçülen imzalar yukarıdakilerle aynı çıktı.
- Ölçülmedi: çok uzun makine/adım adı, uzun lot no, "SKT geçmiş" rozetli satır (veride yok), 360'ta "Ayrıntı" düğmesinin yarım hücreye sığması (Faz 6 listesiyle aynı sınıf, ölçülen düğme yüksekliği ≥43 px, taşma yok), G12 `getBoundingClientRect` hizalaması (yalnız görüntü incelendi: Acil formu 360 ve MyWork 1440, koyu tema).

## DNA uygunluğu
Mevcut token ve 2 px kenarlık/köşe, yeni ham renk ve token yok; acil vurgusu mevcut `danger` ailesi (rozet, düğme, MyWork paneli kenarlığı); rapor sayıları mono, tablolar veri dili satırına uygun; grafik ve hareket eklenmedi; dört eşit KPI kartı yok.

## Öz denetim
- `npm run build:assets`, `build:css` tamam; `dotnet build -warnaserror -o .qa/fe/build22` 0 uyarı 0 hata (backend dosyaları tamamlanmış hâliyle; ilk denemede backend `EmergencyOpener` derleme hatası vardı, o tamamlanınca düzeldi).
- §4 grep tam tablo: 0 ihlal (em/en-dash, `...`, boyutsuz img, onclick, CDN, h-screen, outline-none, ham renk); gizli alanlar yalnız `CommandId` ve rapor formlarındaki `mode`/`LotQuery` (bool kutu yok, JS kapalı gizli alanda `required` yok).
- Curl turu (port 5611, `.qa/fe/f6/f6.db` kopyası, süreç kapatıldı): oturumlar Operatör 2003/2004, Yönetici 1001, Denetçi 3001, Süper admin 0001. `/emergencies/create` ve `/emergencies/machines` Operatör 200; Yönetici, Denetçi, Süper admin access-denied 302. `/reports/durations` ve `/reports/materials` Yönetici ve Denetçi 200, Operatör ve Süper admin access-denied 302. Formda belirteç formun kendi alanından ve `CommandId` ile gönderildi: JS kapalı `refresh=true` tesis seçimi hatları getirdi; `AddMaterialLotId` ile lot satırı (SKT geçmiş gerekçe alanı) geldi; açıklamasız gönderim 200 "Ne olduğunu kısaca yazın."; dolu makine 200 sahip/durum mesajı; geçerli gönderim 302 `/records/run/11`, kayıt no `T1-H1-M02-A2026-3`, yürütmede "Saha defteri Yok", acil açıklama ve ilk adım başlamış, detay ve `?kind=2` listesinde "Acil". Raporlar: süzgeçsiz, tür, tarih aralığı, lot (3 lot), makine (2 makine) 200 ve satırlar doğru; "kullanılmamış" lot boş durumu.
- Tarayıcı: `/my-work` (Acil paneli), `/emergencies/create` ve üç rapor ekranı 4 genişlikte yatay kaydırma 0, `main` içinde 43 px altı düğme yok.
- Denenmedi: JS'li çağlayanın acil formunda tıklama akışı (kod Faz 4 ile aynı sınıflar, `CommandForm` çift gönderim ve `CommandId` G9 tarayıcıda ölçülmedi), zorunlu malzemeli acil tipte malzemesiz ret mesajı (seed'deki acil tip malzeme gerektirmiyor), PIN hatası, açık tema görüntüsü, koyu dışında tema ölçümü.

## Backend'den beklenen/eksik
1. JS'siz lot ekle/çıkar düğmeleri `formaction="/emergencies/create?refresh=true"` ile gidiyor (Faz 4 `/records/create` ile aynı kalıp); yanıt sonrası adres çubuğunda `?refresh=true` kalıyor. İsterseniz `refresh || AddMaterialLotId != null || RemoveMaterialIndex != null` kabul edilirse formaction sorgusu kalkar (görünümde tek satır).
2. `MaterialTraceViewModel.ModeOptions` kullanılmadı; sekmeler sabit `mode=Lot|Machine` adlarıyla (enum adı) bağlanıyor, bu adlar çalışıyor.
3. Rapor süzgecinde Hat ve Makine seçenekleri tesise/hatta göre daraltılmıyorsa liste uzun kalır; görünümde çağlayan yok (Records listesiyle aynı yaklaşım).
4. Rapor ekranlarında hata durumu yok (VM `ErrorMessage` taşımıyor); sorgu hatası genel `Failure` sayfasına düşer.
5. Süre altı sayısı çipi "N kayıt" yazar; faz satırında asgari süre yoksa "-" gösterir (acil tipte faz minimumu olmadığı için normal).

## Not
Yalnız kendi sunucum (PID dosyası `.qa/fe/f6/pid`, port 5611) kapatıldı; temizlik için `pkill -f` ile kendi Chrome profil yoluma bakan bir komut da çalıştırdım (eşleşen süreç yoktu). Önceki not `mv` ile Archive'a taşındı; git indeksine dokunulmadı.

## D-1 (PL-74, faz dışı düzeltme)
- B1: `Views/CleaningTypes/Form.cshtml:45` `fieldset#Kind` artık `class="field space-y-2" data-field`; istemci "Tür seçin." hatası (`FormValidation.js` `closest("[data-field], .field")`) fieldset sonuna, radyo etiketinin dışına düşer. Hata kimliği `Kind-error`, sunucu hatasıyla aynı kimlik ve yer; `aria-describedby` sunucu hatasında fieldset'te, istemci hatasında radyoda.
- B2: `max-w-md` ve `max-w-[12rem]` yalnız `xl:` önekiyle (tablo kipi): `Records/Index.cshtml` (Makine ve yer, Sahip), `Details.cshtml` (Mola, Malzeme), `Integrity.cshtml` (iki uzun hücre). Detaydaki 4 px farkın kaynağı `Details.cshtml:472` olay günlüğü "Özet" hücresinde önekiz kalan `max-w-xl` idi (D1-K1); `xl:max-w-xl` yapıldı. Önekiz `max-w` taşıyan başka `td` kalmadı (Views taraması). `xl:` altında sınır kalkar, G11 sütun/satır imzası değişmez (ızgara hücre izi `col-span` ile belirlenir); tarayıcıda yeniden ölçülmedi.
- `build:css`, `dotnet build -warnaserror -o .qa/fe/build25` 0 uyarı 0 hata; dash ve `...` grep 0.
- Aynı kusur 6 liste ekranında vardı (önekiz `max-w-xs`, Ad hücresi): CleaningTypes, Facilities, Machines, Materials, Shifts, Users Index; `xl:max-w-xs` yapıldı. Ölçülmedi.

# Devir notu: frontend-engineer, Faz 2 (Tesis yapısı, vardiya ve malzeme)

## Yapılan
- Görünümler (sözleşmedeki `@model` tipleriyle, `ViewModels/Definitions/**` gerçek sınıflarına karşı derlendi):
  - `Views/Facilities/`: `Index` (E-05), `Details` (hat tablosu + tesis ve bağlantı rayı), `Form`.
  - `Views/Lines/Form.cshtml`.
  - `Views/Shifts/`: `Index` (E-06, tesis seçici), `Form`.
  - `Views/Machines/`: `Index` (E-07, tesis ve hat filtreli), `Form` (bağlı seçim), `Retire` (engelleyen kayıt listesi, `CanRetire` false ise düğme yok).
  - `Views/Materials/`: `Index` (E-08, "SKT geçmiş" rozeti ve adedi), `Details` (lot tablosu), `Form`.
  - `Views/MaterialLots/Form.cshtml`.
- Ortak parçalar: yeni `_ListSkeleton` (liste yükleniyor iskeleti) ve `_ValueField` (tarih/saat alanı, etiket + ipucu + hata). `_Field`, `_FormSummary`, `_Pagination`, `_State`, `_Badge`, `.table-stack`, `stack-wide`, `stack-lead` yeniden kullanıldı. `_Badge` sözlüğüne `def.active`, `def.inactive`, `machine.retired` eklendi (`lot.expired` zaten vardı).
- `wwwroot/js/CascadeSelect.js` (vanilla ES modülü, `data-cascade-url/-target/-status` kancası, `App.js` içinde bağlandı). Tesis seçilince `GET /machines/lines?facilityId=` ile hat listesini yeniler; yarış için sıra belirteci, hata ve boş durum metni var.
- Menü (`_Layout`): yalnız `Roles.Manager` için "Tanımlar" grubu (Tesisler, Makineler, Vardiyalar, Malzemeler). Süper admin yalnız Kullanıcılar'ı görür (G10).
- `Styles/app.css`: `.table-stack-rail` (lg-xl arası 2 sütun) eklendi; `Styles/BuildAssets.mjs` 6 yeni ikon (arrow-left, building-2, calendar-days, moon-star, power-off, route); `npm run build:assets` ve `build:css` çalıştırıldı.
- Dört durum: iskelet (`_ListSkeleton`, `data-loading` ile FormBusy), boş (tesis yok, hat yok, lot yok, vardiya yok, filtre boş), hata (`_FormSummary`, alan altı hata; `notice-danger` engel), dolu. Uzun veri için `break-words`.

## G11 yerleşim imzaları (BEKLENEN, tarayıcıda ölçülmedi)
Biçim `Alan:kapladığı sütun@satır` (Faz 1 notuyla aynı). Uzun metin alanı her ekranda **Ad** (`stack-wide stack-lead`). 1440 px hepsinde tablo, satır en az 56 px (`xl:h-14`). 1024 px listelerde 768 ile aynı 4 sütun; farklı olan yalnız Details alt tabloları (aşağıda).

**E-05 `/facilities`** (hücreler: Kod, Ad, Hat, Vardiya, Durum, İşlem; ray yok, tam genişlik)
- 360: `Ad:2@1 Kod:1@2 Hat:1@2 Vardiya:1@3 Durum:1@3 İşlem:1@4` (İşlem tek başına, yarım boş hücre son satırda: kalan alan sayısı tek)
- 768: `Kod:1@1 Ad:2@1 Hat:1@1 Vardiya:1@2 Durum:1@2 İşlem:1@2`
- 1024: 768 ile aynı
- 1440: tablo, 6 sütun

**E-06 `/shifts`** (Ad, Başlangıç, Bitiş [+ "Ertesi gün biter" çipi], Durum, İşlem)
- 360: `Ad:2@1 Başlangıç:1@2 Bitiş:1@2 Durum:1@3 İşlem:1@3`
- 768: `Ad:2@1 Başlangıç:1@1 Bitiş:1@1 Durum:1@2 İşlem:1@2` (Ad DOM'da ilk, `stack-lead` 768 üstünde etkisiz)
- 1024: 768 ile aynı
- 1440: tablo, 5 sütun

**E-07 `/machines`** (Kod, Ad, "Tesis ve hat" [iki satır], Durum [+ kaldırılma anı], İşlem; "Tesis ve hat" ikinci uzun hücre ama tam satır almaz)
- 360: `Ad:2@1 Kod:1@2 Tesis ve hat:1@2 Durum:1@3 İşlem:1@3`
- 768: `Kod:1@1 Ad:2@1 Tesis ve hat:1@1 Durum:1@2 İşlem:1@2`
- 1024: 768 ile aynı
- 1440: tablo, 5 sütun

**E-08 `/materials`** (Kod, Ad, Lotlar [adet + "SKT geçmiş" rozeti + adet], Durum, İşlem)
- 360: `Ad:2@1 Kod:1@2 Lotlar:1@2 Durum:1@3 İşlem:1@3`
- 768: `Kod:1@1 Ad:2@1 Lotlar:1@1 Durum:1@2 İşlem:1@2`
- 1024: 768 ile aynı
- 1440: tablo, 5 sütun

**Detay alt tabloları** (liste ekranı değil ama aynı bileşen). Ana kolon + 22rem ray lg'de yan yana olduğundan 1024-1279 arasında ana kolon yaklaşık 350-600 px; 4 sütun kart orada ~80 px'lik hücre verirdi. G11 "768-1279: 4 sütun" kalıbından **gerekçeli sapma**: `.table-stack-rail` ile 1024-1279 arası 2 sütun. Kabul analist kararıdır.
- `/facilities/details/{id}` hat tablosu (Kod, Ad, Makine, Durum, İşlem): 360: `Ad:2@1 Kod:1@2 Makine:1@2 Durum:1@3 İşlem:1@3`; 768 (ray ana kolonun altında, tam genişlik): `Kod:1@1 Ad:2@1 Makine:1@1 Durum:1@2 İşlem:1@2`; 1024: 360 ile aynı; 1440: tablo.
- `/materials/details/{id}` lot tablosu (Lot no, Son kullanma tarihi, Durum, İşlem; uzun metin alanı yok, hepsi 1 sütun): 360: `Lot no:1@1 Son kullanma tarihi:1@1 Durum:1@2 İşlem:1@2`; 768: dördü `@1`; 1024: 360 ile aynı; 1440: tablo.

**Form ve detay iskeleti** (Form, Retire, Details): `lg:grid-cols-[minmax(0,1fr)_22rem]`. lg (1024) ve üstünde ana kolon + ray yan yana, lg altında ray altta. Faz 1 formları xl kullanıyordu; yeni ekranlar G11 gereği lg. Form içi alan çiftleri md'de 2 sütun, 360'ta tek sütun; kod + ad alanları `md:grid-cols-2`.

## DNA uygunluğu (kart maddeleri)
- Tek mavi accent, 2 px keskin çerçeve, ham renk yok (yalnız token sınıfları): uygun.
- Tablo: 2 px başlık çizgisi, mono kod ve saat, sayı sütunu `num` (xl'de sağa, altında sola), zebra yok: uygun.
- Satır kartı: aynı `<table>`, `data-label`, 768 altı 2 / 768-1279 4 sütun (Details alt tabloları hariç, yukarıda gerekçeli).
- Rozet tek biçim (`_Badge`, ikon + metin + kenarlık); durum renkle değil metinle ayrılır ("Aktif", "Pasif", "Kullanımdan kaldırıldı", "SKT geçmiş").
- Boş: kesik çizgili `_State`, birincil eylemli. Hata: `_FormSummary` / `notice-danger`. Yükleniyor: iskelet + "Yükleniyor…".
- Hareket yalnız renk geçişi; yeni animasyon yok. Dokunma hedefi: düğmeler `min-h-11`, birincil `btn-lg` 56 px, tüm alanlar `min-h-12`.
- Süre ya da zaman hesabı görünümde yok; saat/tarih yalnız biçimlenir. Rol sabiti yalnız `Roles.Manager`.

## Öz denetim
- `npm run build:css`, `npm run build:assets`: başarılı.
- `dotnet build Src/Vaka.Web -warnaserror -o .qa/fe/build`: 0 uyarı, 0 hata (backend controller'ları geldikten sonra, ikinci koşu).
- `UiDelivery.md §4` grep (Views yeni klasörler, `_ValueField`, `_ListSkeleton`, `_Layout`, `_Badge`, `CascadeSelect.js`, `App.js`): 0 ihlal; em/en-dash 0; `...` 0; ham hex/`slate-`/`gray-` 0; `<input` autocomplete eksiği 0.
- **Ölçülmedi:** hafif tur (curl) ve tarayıcı. Faz 2 tabloları geliştirme DB'sinde henüz yok (Migrations klasörü yok), bu yüzden uygulamayı çalıştırıp sayfaları açamadım. Yerleşim imzaları yukarıda yalnız beklenen değerdir.

## Backend'den beklenen/eksik
1. **Route uyuşmazlığı (sözleşme ihlali):** `MaterialLotsController` `[Route("[controller]/[action]")]` ile `/MaterialLots/create` üretir; sözleşme `/material-lots/create?materialId=` ve `/material-lots/edit/{id}`. Görünümler sözleşmedeki `/material-lots/...` adresine bağlandı (Materials/Details). Controller'a `[Route("material-lots/[action]")]` benzeri sabit gerekir; aksi halde lot ekle/düzenle bağlantıları 404 verir.
2. **Bağlı seçim JS yokken:** Makine formunda tesis değişince hat listesi JS ile gelir. JS kapalıyken kullanıcı tesisi değiştirip kaydederse sunucu "hat bu tesise ait değil" benzeri alan hatası dönmeli (`LineId` anahtarıyla) ve formu **yeni tesisin hatlarıyla** yeniden çizmeli (K3). Görünüm `LineId` ve `FacilityId` ModelState hatalarını alanın altında gösterir.
3. **Kilitli alanlar:** düzenleme formlarında `CodeLocked` true iken kod `readonly` gelir; makinede tesis ve hat `disabled` gösterilir ama gizli alanla (`FacilityId`, `LineId`) aynı değer gönderilir. Sunucu yine de değişikliği reddetmeli (K2).
4. **Aktif bayrağı:** `IsActive` onay kutusu gizli `false` ile birlikte gönderilir (işaretsiz = false).
5. **Sayfalama yok:** `FacilityListViewModel` ve `ShiftListViewModel` `Page/PageSize/Total` taşımıyor, bu yüzden E-05 ve E-06'da `_Pagination` yok. K8 "sayfalama sunucuda" ifadesi bu iki liste için sözleşmede karşılıksız; E-07 ve E-08'de sayfalama ve filtre bağlı. Analist kararı gerekir (ya da küçük kümeler için kapsam dışı yazılsın).
6. **Boş seçim:** `ShiftListViewModel.FacilityId` 0 gelirse "Vardiya ekle" düğmesi gizlenir; tesis yoksa boş durumda "Tesis ekle" bağlantısı gösterilir.
7. **Makine kaldırma:** `MachinesController` iki `Retire(int id, CancellationToken)` imzası taşıdığı için ilk derlemede CS0111 verdi; sonraki derlemede düzeldi (not olarak bırakıldı). `POST /machines/retire/{id}` PIN almaz (sözleşmede PIN yok, BR-48 listesinde de yok).
8. Durum mesajı: liste ve detay görünümleri `StatusMessage` alanını kabuktaki durum şeridine yazar (kaydet sonrası yönlendirmede doldurulmalı).

# Devir notu: frontend-engineer, Faz 2 ara kararları (PL-36..PL-41)

Önceki not: Archive/frontend-engineer-20261114-2330.md. Aşağıdaki görünüm listesi, G11 imzaları ve DNA uygunluğu o notla aynıdır; imza değişmedi.

## Bu turda
- `Machines/Form.cshtml` ekleme modu: eylem satırında "Seçilen tesisin hatlarını getir" düğmesi (`formmethod="get"`, `formaction="/machines/create"`, `formnovalidate`, `data-cascade-fallback`). `CascadeSelect.js` bağlandığında bu kancalı öğeleri `hidden` yapar, yani JS açıkken görünmez. Düğme ana kaydet düğmesinden sonra durur; Enter ile örtük gönderim kaydetmeye gider, yenilemeye değil. Kaydet düğmesinin yerleşimi ve imzalar değişmedi (ekleme formunda eylem satırına bir düğme eklendi, 360 ve 768'de alt alta/sarar; ölçülmedi).
- Düzenleme modu PL-36/37'ye uygun: tesis ve hat `disabled` gösterilir, gizli `FacilityId` ve `LineId` mevcut değerle gider; sunucu tesisi hattan türetir, farklı değeri reddeder. Görünümde değişiklik gerekmedi.
- Makine listesi filtresi: mevcut "Filtrele" düğmesi JS kapalıyken yeterli, dokunulmadı. Bir not: JS kapalıyken kullanıcı tesisi değiştirip "Filtrele"ye basarsa eski `lineId` gönderilir; sunucu uyuşmayan hattı yok sayıp tesisin hatlarıyla çizmeli (backend).
- `/facilities` ve `/shifts`: `_Pagination` yok, PL-38 ile uyumlu, dokunulmadı.
- `.table-stack-rail` yalnız `Facilities/Details` ve `Materials/Details` alt tablolarında (grep); hiçbir liste ekranında yok (PL-39).
- `StatusMessage` tüm liste ve ayrıntı görünümlerinde `ViewData["StatusMessage"]` ile kabuk şeridine yazılıyor: Facilities Index/Details, Shifts Index, Machines Index, Materials Index/Details (grep ile doğrulandı).
- Eski "Backend'den beklenen" 1. madde (MaterialLots rotası) geri çekildi (PL-41); QA çalışan uygulamada `/material-lots/create` adresini doğrular.

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
- `dotnet build Src/Vaka.Web -warnaserror -o .qa/fe/build`: 0 uyarı, 0 hata. CSS ve ikon derlemesi gerekmedi (yeni sınıf ve ikon yok; `refresh-cw` mevcut).
- Değişen dosyalarda em/en-dash, `...`, `transition: all`, `outline-none`, `h-screen`, `onclick=` grep: 0 ihlal.
- **Ölçülmedi:** curl turu ve tarayıcı (JS kapalı akış dahil). Yeni düğmenin yerleşimi ve JS ile gizlenmesi çalışan uygulamada QA tarafından doğrulanmalı.

## Backend'den beklenen/eksik
1. `GET /machines/create` yenilemede bütün form alanları sorgu dizesine gider (antiforgery belirteci dahil, `formmethod="get"` nedeniyle); `Create(int? facilityId)` yalnız tesisi okuyor. Girilmiş kod ve ad GET sorgusundan forma geri bağlanıyorsa korunur, bağlanmıyorsa kaybolur; K3 bunu bekliyorsa `MachineFormViewModel`'in GET'te de bağlanması gerekir.
2. Hat-tesis uyuşmazlığında POST `LineId` hatasıyla gönderilen tesisin hatlarıyla yeniden çizilmeli (PL-41, backend).
3. Makine filtresinde JS kapalıyken uyuşmayan `lineId` yok sayılmalı (yukarıda).
4. Kilitli alanlar (K2) ve `IsActive` gizli `false` davranışı önceki notla aynı.

# Devir notu: frontend-engineer, Faz 6 (E-19, E-20, E-21, E-26, F5-B2)

Önceki not: Archive/frontend-engineer-20261115-1500.md.

## F5-B2 (PL-71, K22)
- `--scrim` tokeni (`_Theme.cshtml`: açık `10 16 22`, koyu `4 8 12`), `tailwind.config.js` `scrim`. `Run.cshtml:590` ve `_MediaDialog.cshtml:1` `backdrop:bg-scrim/70`. Aynı kusur mobil çekmece perdesinde de vardı (`.drawer-scrim`), `bg-scrim/60` yapıldı.

## Ekranlar
- `Views/Records/Index.cshtml` (E-19): GET süzgeç formu (`query, mineOnly, status, facilityId, lineId, machineId, kind, from, to`; `mineOnly` iki seçenekli select, onay kutusu yok, böylece Operatör varsayılanı kaldırılınca `false` açıkça gider), tablo/kart, sayfalama süzgeci taşır, üç boş durum (süzgeçli, "benim kayıtlarım" boş, hiç kayıt yok), yükleniyor iskeleti (`_ListSkeleton`, form gönderme ve "Ayrıntı" bağlantısı). Tür ve Durum seçenekleri `KindOptions/StatusOptions`'tan (`Id` sayı). Satırda süre altı ve SKT geçmiş çipleri.
- `Views/Records/Details.cshtml` (E-20): üstte kayıt başlığı (tesis, hat, makine, tip, versiyon + doküman no, iş emri, saha defteri, "Müdahale et" yalnız `Roles.Manager` ve `CanIntervene`, "Yürütme ekranına git"), kapanış bilgisi, açık mola (Yönetici'de "Molayı bitir"), Süreler (net büyük; brüt, mola, bekletme, efor), faz ve adım kartları (süre altı gerekçe, açıklama, medya, başlatan/bitiren, görevliler ve onay), sağ ray: Bütünlük (rozet, metin, "Doğrulama ayrıntısı"), Kişiler ("Kaydı açan (sorumlu)" ve "İlk adımı başlatan" ayrı), Zamanlar. Ray altında tam genişlikte Molalar, Malzemeler ve lotlar, Olay günlüğü (saniyeli sunucu zamanı). Yazdırma: `Yazdır` düğmesi (`PrintButton.js`, JS kapalıyken gizli), yalnız yazdırmada görünen "Denetim çıktısı" bloğu (kayıt no, oluşturan, an, versiyon ve doküman, bütünlük), sayfa sonu "oluşturuldu" satırı.
- `Views/Records/Integrity.cshtml` (E-21): sonuç paneli (rozet, ilk tutarsız olay, değişen alan sayısı), "Değişen alanlar" (alan, olay sırası, saklanan ve olaydaki değer), olay zinciri tablosu (ihlalli satır `danger-soft`, sorun metni, Geçerli/Sorunlu rozeti).
- `Views/Interventions/Create.cshtml` ve `EndBreak.cshtml` (E-26): ana kolon + kayıt/mola bağlam rayı; tür radyoları (`.choice`, her birinin sonuç metni), devam eden adım ve açık mola uyarıları, gerekçe, PIN, "Müdahaleyi kaydet" (danger, 56 px) / "Molayı bitir", "Vazgeç"; `BlockReason` varsa form yerine neden ve ayrıntıya dönüş. Form: `action` açık, `asp-antiforgery="true"`, `data-command-form` (aynı `CommandId`), `ErrorMessage` ve ModelState (`_FormSummary`, `Type`, `Reason`, `Pin`).
- Kabuk: `_Layout` demo/prototip dalları kaldırıldı; menüye "Kayıtlar" (Yönetici, Denetçi: İzleme grubu, alt çubuk 2. sıra; Operatör: Makine panosundan sonra). `Views/Prototype/**` ve `_DemoBar` silindi (K13, repoda yok). `_Badge`: `integrity.ok|broken|event.ok|event.bad` ve etiket yoluyla çözme (`"record|Tamamlandı"`, `phase|`, `step|`; ViewModel yalnız etiket verince kullanılır, bulunamazsa soluk rozet).
- Yazdırma katmanı `Styles/app.css` sonu: A4 dikey, tokenler açık temaya sabit (`:root:root:root`), menü/üst bar/alt çubuk gizli, kart tabloları gerçek tabloya döner. Ekler: ikonlar `shield-alert, printer, history, octagon-alert` (sprite yeniden üretildi).
- Silinmeyen not: `Users/Deactivate.cshtml` zaten `work.Url` ile bağlantı çiziyor (F5-A1), değişiklik gerekmedi.

## G11 beklenen imza (ölçüldü: scratch uygulamasında gerçekçi demo veriyle, Chrome CDP, 360/768/1024/1440, yatay kaydırma 0)
Alan adlarında `Kayıt no` etiketsiz ilk hücre. Uzun metin alanı: Makine ve yer.
- `/records`: 360 `Makine:2@1 KayıtNo:1@2 Sahip:1@2 Durum:1@3 Açılış:1@3 Net:1@4 İşlem:1@4`; 768 ve 1024 `KayıtNo:1@1 Makine:2@1 Sahip:1@1 Durum:2@2 Açılış:1@2 Net:1@2 İşlem:4@3`; 1440 tablo, satır en az 56 px (ölçülen 87, uzun makine adında büyür).
- `/records/details` Molalar (uzun alan Mola): 360 `Mola:2@1 Başlangıç:1@2 Bitiş:1@2 Süre:1@3 Kapanış:1@3`; 768/1024 `Mola:2@1 Başlangıç:1@1 Bitiş:1@1 Süre:2@2 Kapanış:2@2`.
- Malzemeler (uzun: Malzeme): 360 `Malzeme:2@1 Lot:1@2 SKT:1@2 Giren:1@3 Giriş:1@3`; 768/1024 `Malzeme:2@1 Lot:1@1 SKT:1@1 Giren:2@2 Giriş:2@2`.
- Olay günlüğü (uzun: Özet): 360 `Özet:2@1 Sıra:1@2 Zaman:1@2 Yapan:1@3 Olay:1@3`; 768/1024 `Sıra:1@1 Zaman:1@1 Yapan:2@1 Olay:2@2 Özet:2@2`.
- `/records/integrity` Değişen alanlar (uzun: Alan ve olay): 360 `Alan:2@1 Saklanan:1@2 Olaydaki:1@2`; 768/1024 `Alan:2@1 Saklanan:1@1 Olaydaki:1@1`. Olay zinciri (uzun: Olay): 360 `Olay:2@1 Sıra:1@2 Zaman:1@2 Özet kodu:1@3 Durum:1@3`; 768/1024 `Sıra:1@1 Olay:2@1 Zaman:1@1 Özet kodu:2@2 Durum:2@2`.
- Ölçülen imzalar yukarıdakilerle aynı çıktı. Bu tablolar rayın yanında değil, tam genişlikte (PL-39 istisnası gerekmez). Detay ana kolon + ray lg ve üstünde yan yana, altında ray ana kolonun altında (ölçülmedi, yalnız görüntüler incelendi; 1440'ta ray sağda).
- İşlem hücresi yalnız `/records`'ta; 360'ta "Ayrıntı" yarım hücreye sığıyor (ekran görüntüsünde), 768 ve 1024'te kendi 4 sütunluk satırında sağa hizalı.

## Yazdırma (emulateMedia print, 718 px, koyu tema açıkken)
Menü, üst bar, alt çubuk `display:none`; ana alanda görünür düğme yok; zemin beyaz, yatay taşma 0, üç tablonun sağ kenarı panel sağ kenarının içinde. PDF çıktısı da üretildi (`printToPDF`, A4). Ölçülmedi: gerçek yazıcı önizlemesinin sayfa sonu bölünmesi (`break-inside: avoid` panel, adım ve satırda).

## DNA uygunluğu
Mevcut token, 2 px kenarlık/köşe, yeni ham renk yok; rozet ve çip dili aynı; hareket eklenmedi. Tek yeni token `--scrim` (kart değişmedi, "Kayıt ekranları (Faz 6)" notu eklendi).

## Öz denetim
- `npm run build:css` ve `build:assets` tamam; `dotnet build -warnaserror -o .qa/fe/build18` 0 uyarı 0 hata (backend dosyaları tamamlanmış hâliyle).
- §4 grep tam tablo: 0 ihlal (`slate-` eşleşmesi `-translate-y-` yanlış pozitifi); `type="hidden"` yalnız `CommandId`; boyutsuz img yok; görünür `...` yok.
- Gerçek uygulama curl turu (port 5497, `.qa/be/s5/s.db` kopyası, Yönetici 1001, süreç kapatıldı): `/records` (süzgeçli ve süzgeçsiz), `/records/details/1,4,5`, `/records/integrity/1,4`, `/interventions/create?recordId=4,5` 200; `/` Yönetici'yi `/records`'a 302; formda 1 belirteç (sayfada 3); yanlış PIN ile müdahale 200, "PIN hatalı", tür radyosu korundu.
- Tarayıcı turu scratch uygulamasında (gerçek verisi uç durum üretmeyen ekranlar için): `DemoController` ile uzun/kısa veri, Yarıda kapatıldı + ihlal, açık mola, boş liste, koyu tema; konsol 0 hata/uyarı. JS tıklama akışları (Yazdır düğmesi, komut formu) tarayıcıda denenmedi.
- Denenmedi: başarılı müdahale ve mola bitirme POST akışı (gerçek veride), 360'ta Create formunun düzeltme sonrası yeniden görüntüsü (`kv` tek sütun yapıldı), Operatör ve Denetçi hesabıyla gerçek çıktılar, G12 ölçümü (`getBoundingClientRect` kutuları).

## Backend'den beklenen/eksik
1. `RecordDetailHeaderViewModel.Kind` var ama Status enum'u yok; detayda durum rozeti `StatusLabel`'dan etiketle çözülüyor (etiket sözlükteki gibi olmalı). Enum eklenirse tek satır.
2. `RecordMaterialViewModel.ExpiryDate` `DateOnly`; SKT biçimi görünümde `dd.MM.yyyy`.
3. Liste `Lines` ve `Machines` seçili tesise/hatta göre daraltılmıyorsa seçenek sayısı büyür; görünümde çağlayan JS yok (JS kapalı uyumu için "Filtrele" ile uygulanır). Daraltma sunucuda yapılırsa görünüm değişmez.

## Not
Çalışma sırasında yanlış bir `git rm --cached -r` komutu çalıştırdım ve hemen `git reset -q` ile indeksi HEAD'e döndürdüm; çalışma ağacı ve dosyalar etkilenmedi. `Docs/Handover/frontend-engineer-20261115-1500.md` `git mv` ile Archive'a taşındı (staged); `git rm` ile silinen `Views/Prototype/**` ve `_DemoBar` indekste staged değil, çalışma ağacında silinmiş görünüyor.

## Güncelleme (QA ön tur bulguları)
- YÜKSEK: Details "Versiyon içeriğini aç" `btn btn-quiet inline-flex min-h-11 items-center` (44 px), `print:hidden` duruyor.
- ORTA: `<time>` kalıbı tarih ve saat ayrı `whitespace-nowrap` parçalara bölündü (aralarında kırılma serbest): `_Moment` (liste Açılış, Molalar, Malzemeler, Zamanlar dahil her kullanım), Details ve Integrity olay günlüğü Zaman hücresi. G11 imzaları değişmedi (hücre sütun ve satır yerleşimi aynı). Tarayıcıda yeniden ölçülmedi.
- `build:css` tamam, `dotnet build -warnaserror -o .qa/fe/build19` 0 uyarı 0 hata.

## Güncelleme (teslim testi B1)
- `Details.cshtml` görevli satırı: "Ekleyen X, eklendi <AddedAtLocal>"; `ConfirmedAtLocal` doluysa ayrıca ", onay <an>", boşsa onay ifadesi yok. Yazdırma aynı metni kullanır. Hücre yerleşimi değişmedi (G11/G12 aynı).
- `dotnet build -warnaserror -o .qa/fe/build20` 0 uyarı 0 hata (backend `AddedAtLocal` eklendikten sonra); CSS değişmedi; §4 grep değişiklikte yeni kalıp yok.

DEVİR: frontend-engineer -> koordinatör   Faz: 6D (FE 5, 6)   Dosya: Docs/Handover/frontend-engineer-20261116-2300.md

Yapıldı:
- E-34 `Views/Reports/Machine.cshtml`: süzgeç (tesis, hat, makine, tarih, tür; JS gerekmez), makine başlığı ve kullanımdan kaldırıldı rozeti, durum sayıları (kv), tip tablosu, tip başına net ve planlanan minimum grafiği (`_BarChart`, minimumun altı desenli ve "Minimumun altında" metinli), son kayıtlar, malzeme kullanım oranı. Menüde Raporlar altında "Makine raporu" (`_Layout.cshtml`, menü orada kuruluyor).
- E-31 `Materials.cshtml`: üçüncü mod "Kullanım oranı" (tarih, tip, tesis, hat, makine süzgeci; seçimsiz sorgu). Payda ve malzemeli kayıt sayısı panel başlığında yazılır.
- Yeni ortak partial `Views/Shared/_MaterialUsageTable.cshtml` (makine raporu ve E-31 kullanır).
- E-30 `Durations.cshtml`: makine satırına "Makine raporu" düğmesi (`MachineReportUrl`).
- Plan farkı işaret ayrı yazılır (`+`, `-`), süre `_Duration`; boşsa `-` ve `aria-label`.

Dört durum: yükleniyor (`_ListSkeleton`, `data-busy-form`), boş (makine seçilmedi, kayıt yok, tamamlanan kayıt yok, malzeme yok), hata (`ErrorMessage` bildirimi ve sonraki adım), dolu.

G11 imzaları (kart: baş = tanıtıcı ve rozet, gövde 2 sütun, ayak = işlem):
- Tip tablosu: stack-lead tip adı ve "N kayıt, M tamamlandı"; gövde Brüt ve mola, Net, Efor, Planlanan min., Plan farkı, Süre altı faz; işlem yok.
- Son kayıtlar: stack-lead kayıt no, stack-badge durum, stack-wide tip, gövde Açılış, Net, Efor, Planlanan min., ayak Ayrıntı.
- Malzeme ve kullanım oranı tablosu: stack-lead malzeme adı ve kod, gövde Kayıt, Kullanım oranı, Lot, SKT geçmiş.
- Durations makine satırı: eski imza, ayağa "Makine raporu".

Kabul kriterleri (FE payı): K1 menü rolü mevcut koşulda; K2 boş durum ve hata; K8 payda ve boş durum sayfada; K10 SVG sunucuda, minimum işareti ve desen artı metin; K11 bağlantı; K12 JS yok, Raporla süzgeci sunucudan yeniler. Tarayıcıda denenmedi; 360-1440 ve tema ölçümü QA'da. 1280-1440 px'te tip tablosu 7 sütun (brüt ve mola tek sütunda iki satır), kırpılma riski QA ön turunda ölçülmeli.

Backend'den beklenen / sapma:
- Tesis ya da hat değişince seçenekler JS ile değil, "Raporla" ile sunucudan yenilenir (yeni JS dosyası dosya sınırı dışı). Eski hat/makine seçili kalıp uyuşmazsa sunucu `ErrorMessage` döner. Önerilen: uyuşmayan hat/makine hata yerine boşaltılırsa kullanıcı akışı düzgün olur; backend kararı.
- `KindLabel` metin olduğundan tür rozeti yerine düz metin gösteriliyor (ViewModel'de enum yok).
- Kullanım oranı modunda `Usage` null ise "Süzgeci seçip sorgulayın" durumu gösterilir.

Doğrulama: `npm run build:css` temiz; `dotnet build -o <scratchpad>` 0 hata, 0 uyarı (PID 32568 bin'ine dokunulmadı); em/en-dash, transition-all, outline-none, h-screen, onclick grep 0.

Değişen dosyalar: Views/Reports/{Machine,Materials,Durations}.cshtml, Views/Shared/{_MaterialUsageTable,_Layout}.cshtml, wwwroot/css/app.css (yeniden üretildi).
DB talebi: yok

Ön tur düzeltmeleri:
- Son kayıtlar 1280 px taşması: Net, Efor, Planlanan min. tek "Süreler" sütununda üç satır oldu (8 sütun 6'ya indi); uzun süreler sütunu genişletmez. Tip sütunu artık sıkışmaz.
- Tarih hatasında "Bir makine seçin" boş durumu kaldırıldı: `ErrorMessage` varken ve makine yokken yalnız hata bildirimi görünür.
- Doğrulama: build:css temiz, dotnet build 0 uyarı 0 hata (ayrı çıktı klasörü).

Tur 1 iade düzeltmeleri (B2, B5, B6, B7, A1):
- B2: `_MaterialUsageTable.cshtml` "SKT geçmiş" hücresinde `whitespace-nowrap` yalnız `xl:`; çip `max-w-full`. Kart modunda metin kırılır, 320 px taşmaz.
- B5: `Machine.cshtml` boş `@if` dalı kaldırıldı; makine yoksa boş durum yalnız `ErrorMessage` boşken çizilir.
- B7: `Durations.cshtml` süzgeç düğme satırı `sm:col-span-2 xl:col-span-4`; 1280 ve 1440'ta tam satırda, tek sırada, alanların altında.
- A1: `Records/Index.cshtml` Durum hücresinde iki rozet `xl:` altında alt alta (`xl:flex-col xl:items-start`), Makine ve yer sütunu `xl:max-w-xs`. Uzun sahip adlı satırda tablo daralır, Ayrıntı kırpılmaz. Ölçüm QA'da (1280, 1300, 1366, 1440).
- Doğrulama: build:css temiz, dotnet build (ayrı çıktı klasörü) 0 uyarı 0 hata.

G11 imzaları, genişlik başına Alan:sütun@satır (kart modu < 1280, tablo >= 1280):
- Tip tablosu: kart 360/768/1024: Tip:1@1 (baş), Brüt ve mola:1@2, Net:2@2, Efor:1@3, Planlanan min.:2@3, Plan farkı:1@4, Süre altı faz:2@4. Tablo 1280/1440/1920: Tip:1@1, Brüt ve mola:2@1, Net:3@1, Efor:4@1, Planlanan min.:5@1, Plan farkı:6@1, Süre altı faz:7@1.
- Son kayıtlar: kart: Kayıt no:1@1 (baş), Durum:2@1 (baş sağ), Tip:1@2 (tam satır), Açılış:1@3, Süreler:2@3, İşlem:ayak. Tablo: Kayıt no:1@1, Tip:2@1, Açılış:3@1, Durum:4@1, Süreler:5@1, İşlem:6@1.
- Malzeme kullanım oranı: kart: Malzeme:1@1 (baş), Kayıt:1@2, Kullanım oranı:2@2, Lot:1@3, SKT geçmiş:2@3. Tablo: Malzeme:1@1, Kayıt:2@1, Kullanım oranı:3@1, Lot:4@1, SKT geçmiş:5@1.
- Durations makine satırı: kart: Makine ve tip:1@1 (baş), Kayıt:1@2, Brüt:2@2, Mola:1@3, Net:2@3, Efor:1@4, İşlem:ayak. Tablo: 7 sütun sırayla @1.

Tur 3 (B8, B6):
- B8: `Views/Records/Index.cshtml` "Açılış" ve "Net süre" tek sütun "Açılış ve net süre" oldu (üstte açılış, altta net süre, sağa hizalı mono; kart modunda iki etiketli değer). Sahip ve "Makine ve yer" hücreleri yeni `.wrap-anywhere` sınıfıyla (`Styles/app.css`, `.table-stack td.wrap-anywhere { overflow-wrap: anywhere }`) tablo modunda da her yerden kırılır; `.table-stack td`'nin `break-word` kuralı utility'yi ezdiği için ayrı sınıf gerekti.
- Ölçüm (kendi derlememle, `.qa/f6d/qa.db` kopyası, boş port 5811, Yönetici 1001, `.qa/f6d/t3/m.js` ve `n.js`; QA verisi, en uzun ad "Şükrüye İnceoğlu Karamustafapaşalıoğulları" satırı dahil 25 satır):
  - 1280 px: tablo 964, panel iç 964, kırpılan satır 0, Ayrıntı sağ kenarı panel iç kenarından en az 12 px içeride; sütunların en küçük genişlik toplamı 904, panel içi 964: pay 60 px (hedef >= 24).
  - 1300 px: tablo 984, panel iç 984, kırpılan 0, sağ boşluk en az 12 px; en küçük toplam 904, pay 80 px.
  - 1440 px: tablo 1124, panel iç 1124, kırpılan 0, sağ boşluk en az 12 px.
  - Açık ve koyu tema aynı; sayfa yatay kaydırması yok. Sütun en küçük genişlikleri (1280): Kayıt no 169, Makine ve yer 197, Sahip 59, Durum 154, Açılış ve net süre 190, İşlem 136.
  - Süreç: kendi başlattığım dotnet süreci kapatıldı, 32568'e dokunulmadı.
- B6 düzeltmesi, G11 imzaları (genişlik başına Alan:sütun@satır):
  - Tip tablosu 360/768/1024 (kart, 3 sütun): Tip:1@1 (baş, tam), Brüt ve mola:1@2, Net:2@2, Efor:3@2, Planlanan min.:1@3, Plan farkı:2@3, Süre altı faz:3@3. Önceki 2 sütunlu imza yanlıştı; 768 ve 1024 px'te kart gövdesi 3 sütundur.
  - /records (yeni): tablo 1280/1440/1920: Kayıt no:1@1, Makine ve yer:2@1, Sahip:3@1, Durum:4@1, Açılış ve net süre:5@1, İşlem:6@1. Kart modunda birleşik sütun "Açılış ve net süre" tek hücre, içinde iki etiketli satır (Açılış, Net süre) alt alta, sütun 1@3 (768/1024'te 3 sütun gövdede 3@2).
  - Not: 768 ve 1024 px kart gövdesinin sütun sayısı ölçüde QA doğrular; bu not beklenen değerdir, ben ölçmedim.

Tur 4 (B10):
- Değişiklik: `Views/Records/Index.cshtml` "Açılış ve net süre" hücresinin iki satırında `whitespace-nowrap` yalnız `xl:whitespace-nowrap`; kart modunda satırlar kırılır. Başka değişiklik yok. CSS yeniden üretildi, build 0 uyarı 0 hata.
- Ölçüm (kendi derlememle, `.qa/f6d/qa.db` kopyası, port 5812, Yönetici 1001, `.qa/f6d/t3/p.js`; her genişlikte 25 satırın hepsi, açık ve koyu aynı):

| Genişlik | Mod | Hücre taşması | Ayrıntı sağ kenarı (kart ya da panel iç kenarına göre) | Sayfa yatay kaydırma | Tablo / panel iç |
|---|---|---|---|---|---|
| 320 | kart | 0 | 14 px içeride | yok | 288 / 288 |
| 360 | kart | 0 | 14 px içeride | yok | 328 / 328 |
| 768 | kart | 0 | 18 px içeride | yok | 720 / 720 |
| 1280 | tablo | 0 | 12 px içeride | yok | 964 / 964 |
| 1440 | tablo | 0 | 12 px içeride | yok | 1124 / 1124 |

- 1280 px'te tablo modu sınıfları değişmedi (`xl:` aynı), önceki pay (en küçük genişlik toplamı 904, panel 964, pay 60 px) korunur.

# Faz 6B Teslim Raporu: Yönetici ana sayfası, rapor grafikleri ve arayüz iyileştirmeleri

| Alan | Değer |
|---|---|
| Faz | 6B |
| Tarih | 2026-11-16 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `8e99e02`; Faz 6B kodu commit'lenmemiş, ön tur düzeltmeleri dahil). Devir notları `Docs/Handover/backend-engineer-20261115-2345.md`, `Docs/Handover/frontend-engineer-20261116-0010.md` ("Ön tur düzeltmesi" dahil) |
| Ortam | LOCAL, Release build, port 5711 (Production), DB: `.qa/` kopyası. Yayın `.qa/f6b2/build`; `.qa/f6b2/qa.db` = `App_Data/vaka.db` dosyasının SQLite yedekleme API kopyası (salt okunur açıldı), `.qa/f6b2/base.db` aynı kopyanın dokunulmamış eşi. Test verisi uygulama akışıyla kuruldu (`.qa/f6b2/s0.py`-`s4.py`). `EventChain__HmacKey` yalnız süreçte üretilen rastgele 32 bayt. Chromium (playwright-cli). Ön tur `.qa/f6b` |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2; tur 1 KALDI) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-16) |

## 1. Kapsam

- BE 1: `/` yönlendirmesi (Operatör, çoklu rolde de, `/my-work`; Yönetici ya da Denetçi `/dashboard`; yalnız Süper admin `/users`).
- BE 2: `GET /dashboard` (`ManagerOrAuditor`), `IDashboardReader`/`DashboardReader`: özet sayılar, aktif kayıtlar, süre altı fazlar, son tamamlananlar, son 7 gün (Türkiye günü).
- BE 3: İsteğe bağlı iş emri no: boş ya da yalnız boşlukta `<TesisKodu>-IE-<Yıl>-<Sıra>` (`NumberSequence`, kapsam `WorkOrder:<TesisId>`), dolu değer trim'lenip saklanır, sistem biçimine uyan elle değer ve 64 karakter üstü alan hatası.
- DB: değişiklik yok (migration ve `Docs/Db` kaydı yok).
- FE 5: Kart listesi deseni (PL-77, yeni G11) 14 liste ve gömülü tablo görünümünde ve panoda.
- FE 6: Köşe tokenleri (ui 6 px, card 10 px, ui-inner 4 px), `Dna.md` köşe satırı.
- FE 7: Kabuk: kenar çubuğunda ve çekmecede sabit logo ve alt blok, kayan menü, daraltma ve çekmece geçişleri, `revealCurrent`.
- FE 8: Tam ekran düğmesi (`Fullscreen.js`, 768 px altında gizli).
- FE 9: `_BarChart` genelleştirmesi; E-30'da üç grafik, E-22'de son 7 gün grafiği.
- FE 10: E-22 Yönetici panosu (kısmi), menüde ve alt çubukta "Ana sayfa" `/dashboard`.
- FE 11: Yeni kayıt formunda "İş emri no (isteğe bağlı)".
- Ön tur düzeltmeleri: grafik dolgu sınıfları, kart ayağı etiketi, hareket azaltmada çekmece geçişi.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | `/` yönlendirmeleri (Yönetici, Denetçi, Operatör, Operatör+Yönetici, Süper admin) | GEÇTİ | `Evidence/Faz-6B/KabulKriterleri.txt` "K1": m1 302 `/dashboard`, au 302 `/dashboard`, op 302 `/my-work`, om (4001, Operatör+Yönetici, Süper admin arayüzüyle açıldı) 302 `/my-work`, sa 302 `/users`, oturumsuz 302 giriş |
| K2 | "Ana sayfa" `/dashboard`'u açar ve orada `aria-current`; "Kayıtlar" ayrı | GEÇTİ | "K2": Yönetici ve Denetçi'de ray, alt çubuk ve çekmecede "Ana sayfa" -> `/dashboard` [current] panoda, "Kayıtlar" -> `/records` [current] listede; iki bağlantı ayrı. Çoklu rolde menü durumu B5 |
| K3 | `/dashboard` Operatör ve Süper admin'de 403; Denetçi panoda yazma düğmesi görmez | GEÇTİ | `YetkiMatrisi.txt`: op ve sa 302>403 "Yetkiniz yok"; "K3": Denetçi ve Yönetici panosunun `main` bölümünde form 0, düğme 0, bağlantılar yalnız `/records` ve `/records/details/{id}` |
| K4 | Açık, devam eden, bugün tamamlanan sayıları DB ile aynı, Türkiye günü; acil kayıtta "Acil" | GEÇTİ | "K4/K6": UTC 20:52'de (Türkiye 15.11) ekran [2, 5, 4] = DB [2, 5, 4]; UTC 21:00:10'da (Türkiye 16.11) ekran [2, 5, 0] = DB [2, 5, 0]; tamamlamalardan sonra [1, 0, 9] = DB. Aktif listede acil kayıt `T1-H2-M05-A2026-1` "Acil" + "Devam ediyor" rozetleri; sıra en eski açılış önce, DB ile aynı |
| K5 | Süre altı listesi gerekçeli; iki liste en çok 10 satır; satırlar detaya gider | GEÇTİ | "K5": DB'de 24 süre altı faz ve 13 Tamamlandı kayıt varken iki liste 10'ar satır; kayıt no, gerekçe ve bağlantı en yeni önce sırayla DB ile 10/10 aynı; bütün bağlantılar 200 |
| K6 | Son 7 gün bugün dahil 7 gün, kaydı olmayan gün 0, sayılar DB ile aynı | GEÇTİ | "K4/K6": gece yarısından önce 9-15 Kas, sonra 10-16 Kas; 7/7 satır planlı ve acil sayıları açılış anının Türkiye gününe göre DB sayımıyla aynı; grafik satırları tabloyla aynı ("15 Kas Paz 12, 1 acil") |
| K7 | Makine, faz ve durum grafikleri; değerler tabloyla aynı; SVG sunucu HTML'inde | GEÇTİ | "K7/K8": sunucu HTML'inde 3 `figure`, çubuk SVG'leri 7, 3 ve 5 satır, `main` içinde script yok. Makine grafiği 3/3 satır (ör. "Net 1 dk 28 sn, Mola 40 dk" = tablo Net 1 dk 28 sn, Mola 40 dk), faz grafiği 5/5 satır ortalama ve asgari tabloyla aynı, durum grafiği 7/7 durum listesiyle aynı |
| K8 | Minimum işareti; asgarinin altındaki faz desen ya da metinle ayrılır; boş süzgeçte grafik yerine boş durum metni, konsol temiz | KALDI | Faz grafiğinde 5 asgari işaret çizgisi, 4 desenli çubuk ve satırda "Ortalama asgarinin altında" metni; konsol 0. Boş süzgeçte (`from=2030-01-01&to=2030-01-31`) makine ve faz grafikleri yerine boş durum metni var, ancak durum grafiği 7 adet 0 değerli çubukla çiziliyor (B3, `Sure-BosSuzgec-1024.png`) |
| K9 | Grafikler açık ve koyu temada okunur, metin AA; 360'ta kaydırma yok; kütüphane yok | GEÇTİ | `OnTurYenidenDogrulama.txt` B1: grafik metninde en düşük karşıtlık açıkta 5,71, koyuda 6,54; çubuk dolgusunun boş ize karşıtlığı açıkta 2,94-6,00, koyuda 5,81-7,10; 8 hücrede `hScroll` false, grafik taşması 0. `package.json` değişmedi; `wwwroot/lib` altında yalnız `Icons.svg` değişti (tam ekran için `maximize`, `minimize` ikonları, `BuildAssets.mjs` listesi); grafik kütüphanesi yok. Acil dolgusu açık temada B6 |
| K10 | Kalem 5 tabloları 360/768/1024'te yeni G11 imzası, 1440'ta tablo; G12 kart maddesi kusursuz | KALDI | `MekanikEval.txt` "G11 imzaları": 14 görünüm ve panonun bütün tabloları frontend devir notundaki imzayla aynı (360'ta 2, 768 ve 1024'te 3 sütun, raydaki tablolar 1024'te 2 sütun), 1440'ta tablo, satır 56-89 px, kapsayıcı 10 px ve `overflow: clip`. G12 kart maddesi: panoda "Şu an sahada" kartında Açıldı satırının rozeti kartın sağ iç kenarından 96 px içeride (B1) |
| K11 | 360 ve 768'de kartlar arası en az 12 px, dört kenar kenarlık, kart zemini sayfadan farklı | GEÇTİ | `MekanikEval.txt`: bütün kartlarda aralık 14 px, kenarlık 2/2/2/2, köşe 10 px; zemin açıkta rgb(247,249,250) / sayfa rgb(236,240,242), koyuda rgb(23,30,37) / rgb(15,20,25); kart içinde taşma 0 |
| K12 | Yazdırmada tablolar tablo, kart görünümü yok | GEÇTİ | "K12": `/records/details/{1,3,7}` print ortamında 360 ve 1024 px: `thead` static 37 px, `tr` `table-row`, köşe 0, etiket öğesi yok, zemin beyaz (koyu tema seçiliyken de) |
| K13 | Hesaplanan köşeler token değerinde; sabit köşe grep 0; `rounded-full` yalnız avatar ve nokta | GEÇTİ | Düğme, alan, select, rozet, çip, icon-btn ve menü bağlantısı 6 px; panel, kart satırı, çekmece ve tablo kapsayıcısı 10 px; göz düğmesi 4 px; avatar 9999 px. 247 hücrede token dışı köşe 0. `Views/**` ve `Styles/**` içinde `rounded-[` ve px'li `border-radius` 0; `rounded-full` 3 yerde (avatar, canlı nokta, PIN hanesi) |
| K14 | `Dna.md` köşe satırı ve gerekçe; iç köşe dış köşeden taşmaz | GEÇTİ | `Dna.md` ön blok `radius.ui 6px`, `card 10px`, köşe ölçeği satırında gerekçe ve kaçınılan değer (12 px üstü); kartın iç köşesi 8 px, içteki öğeler 6 px |
| K15 | İş emri alanı var, `required` ve zorunlu işareti yok; isteğe bağlı olduğu ve sistemin atayacağı yazılı | GEÇTİ | Etiket "İş emri no (isteğe bağlı)", yardım "Dış sistemden bir iş emri numaranız varsa yazın. Boş bırakırsanız kayıt açılınca sistem numara atar."; `required` false. 4 genişlik, açık ve koyu temada etiket, alan, yardım ve hata metninin solu aynı ve panel iç çizgisinde; sunucu hatası yardımın altında (`.qa/f6b2/r-wo.json`) |
| K16 | Boş bırakılan iki planlı kayıtta `T1-IE-2026-n` ve `n+1`; başka tesiste sıra ayrı; detay, yürütme, açılış olayı, arama, bütünlük | GEÇTİ | "K16": kayıt 7 `T1-IE-2026-1`, kayıt 8 `T1-IE-2026-2`; T2 tesisi Yönetici arayüzüyle etkinleştirildi, makine 5'te kayıt 11 `T2-IE-2026-1` (sayaç `WorkOrder:2`). Beş kayıtta değer detayda, yürütme başlığında ("İş emri T1-IE-2026-1"), `RecordOpened` olayında aynı; `/records?query=` aramasında tek sonuç o kayıt; bütünlük "Doğrulandı". Sonraki boş açılışlar `T1-IE-2026-4` ve `-5` |
| K17 | `ERP-778` aynen saklanır, sayaç ilerlemez; yalnız boşluk sistem numarası alır; aynı CommandId tek kayıt | GEÇTİ | "K16": `ERP-778` saklandı, sayaç 2'de kaldı; ardından `'   '` -> `T1-IE-2026-3`. Aynı CommandId ile ikinci POST 302 aynı `/records/run/7`, kayıt +1, olay +1, `ProcessedCommands` +1, sayaç 1. 64 karakterlik değer kabul edildi ve aynen saklandı |
| K17a | Sistem biçimine uyan elle değer ve 64 üstü alan hatasıyla reddedilir; kayıt ve olay yok; alanlar ve CommandId korunur | GEÇTİ | "K17a": `T1-IE-2026-5`, `xx-ie-2026-0005` ve `'  T1-IE-2026-1  '` "Bu biçim sistemin atadığı numaraya ayrılmıştır. ..."; 65 karakter "İş emri numarası en çok 64 karakter olabilir."; dördünde 200, kayıt 6 -> 6, olay 63 -> 63, sayaç 0; CommandId, tesis, makine, iş emri, not ve kilit süresi korundu, PIN boş |
| K18 | Acil açılış değişmez; eski değerler değişmez; migration yok; sistem numaralarında yineleme 0 | GEÇTİ | "K18": acil kayıt 12'de iş emri NULL; kayıt 1-6 değerleri `base.db` ile aynı; yinelenen sistem iş emri sorgusu ve büyük küçük harf duyarsız tüm değerler sorgusu 0 satır; `Data/Migrations` değişmedi (son `8730e7e`), `__EFMigrationsHistory` aynı |
| K19 | 1440x600 ve 1024x600'de, açık ve daraltılmış rayda logo ve alt blok sabit, yalnız menü kayar, son öğe görünür | GEÇTİ | `Kabuk.txt`: logo `0,0,246,64` ve alt blok `0,424,246,177` menü kaydırılınca (`scrollTop` 0 -> 436) ve sayfa kaydırılınca aynı; daraltılmışta `0,0,70,64` ve `0,424,70,177`; son öğe görünür |
| K20 | 768x600 ve 360x640 çekmecede başlık ve alt blok sabit, yalnız menü kayar, sayfa kaymaz | GEÇTİ | Başlık `0,0,350,64` / `0,0,315,64` ve alt blok tekerlek sonrası aynı; menü `scrollTop` 0 -> 388 / 348; `scrollY` 0 |
| K21 | En alttaki ekranda etkin öğe menünün görünür alanında | GEÇTİ | `/account/security` 1440x600 ve 1024x600: "Parola ve PIN" görünür, `scrollTop` 433 |
| K22 | Daraltma geçişi 50 ms ara değer, 400 ms uç değer; kaydırma, kırılma, taşma yok; `aria-expanded` | GEÇTİ | Daraltma 50 ms 145 px, 120 ms 84-86 px, 400 ms 72 px; genişletme 50 ms 174-185 px, 400 ms 248 px; belge genişliği sabit, ikinci satıra kırılan etiket 0, sınırdan taşan etiket 0; ikonlar daraltılmışta merkezde (sapma 0); `aria-expanded` ve "Daralt"/"Genişlet" güncelleniyor |
| K23 | Kayıtlı daraltmayla yüklemede ilk boyamada dar, geçiş yok | GEÇTİ | Yeniden yüklemede genişlik 72 px, `transitionrun` olayı 0 |
| K24 | Çekmece kayar ve perde solar; kapalıyken odak almaz; Esc ve perde kapatır; JS kapalıyken `#menu` | GEÇTİ | 50 ms'de panel x -125/-131 px, perde opaklığı 0,28-0,35; 400 ms'de 0 ve 1. Kapalıyken odaklanabilen öğe 0; Esc ve perde tıklaması kapatıyor, odak "Menü"ye dönüyor. JS kapalı: `#menu` ile görünür, kapat bağlantısıyla gizli |
| K25 | Hareket azaltmada daraltma ve çekmece geçişsiz (50 ms'de uç değer) | GEÇTİ | Ray 50-60 ms'de 72 ve 248 px; çekmece 50-60 ms'de x 0, perde 1. Aynı ayarda çekmece açılışında odak içeri girmiyor (B2) |
| K26 | 768, 1024, 1440'ta düğme görünür, 44 px; tıklama ve çıkışta durum ve etiket | GEÇTİ | `Kabuk.txt` "Tam ekran": 767 ve altı `display:none`, 768/1024/1440 44x44; tıklama: `fullscreenElement` dolu, `aria-pressed="true"`, "Tam ekrandan çık", çıkış simgesi; ikinci tıklama ve `exitFullscreen()` sonrası `false` ve "Tam ekrana geç". Esc tuşu ayrıca denenemedi (bölüm 8) |
| K27 | API kapalıyken ve JS kapalıyken düğme görünmez; 360'ta üst bar taşmaz | GEÇTİ | `fullscreenEnabled` false: `hidden`, görünmez; JS kapalı `display:none`; 360'ta `scrollWidth` 360, üst bar 64 px |
| K28 | Sayfa geçişinden sonra durum gerçekle aynı; reddedilen istek durumu değiştirmez, konsol temiz | GEÇTİ | Geçiş sonrası `aria-pressed="false"`, `fullscreenElement` boş; `requestFullscreen` reddedildiğinde `false`, konsol 0 |
| K29 | Faz 1-6A ekranlarında konsol 0, §4 0; PIN, select ve D-1 hata yerleri bozulmaz | GEÇTİ | `MekanikEval.txt` t2-t4: Operatör (İşlerim, yeni kayıt, yürütme, acil, makine panosu, kayıtlar, hesap), Süper admin ve Yönetici formları 126 hücrede konsol 0 (tek istisna QA'nın yanlış yazdığı `/shifts/create` adresinin 404 kaynak hatası, A2); G12 form ölçümü 18 + 30 hücrede a-e kontrolleri kusur 0 |
| G1 | `-warnaserror` 0/0; `build:css` | GEÇTİ | Bölüm 3 |
| G2 | §4 grep 0; öznitelikler; rol metni | GEÇTİ | Bölüm 3 |
| G3 | §2 matrisi kırık hücresiz, §4 0, konsol 0, DNA sapması yok | KALDI | §4 0, konsol 0, DNA sapması yok; kırık hücreler B1 (pano rozeti), B2 (hareket azaltmada çekmece odağı), B3 (boş süzgeçte durum grafiği) |
| G4 | 360'ta kaydırma yok, birincil 44 px, durum metinle | GEÇTİ | 360 ve 320'de `hScroll` false; düğmelerde 40 px altı hedef 0 (yalnız label içindeki onay ve radyo kutuları 20x20, tıklama alanı 52 px ve üstü); rozetler ikon + metin, grafikte değer metni |
| G5 | Belirteçsiz POST 400 | GEÇTİ | Operatör belirteçsiz POST `/records/create` 400 |
| G5a | Her POST formu kendi belirtecini taşır | GEÇTİ | Pano, raporlar, yeni kayıt, kayıtlar, kullanıcılar ve üç detay sayfasında POST formlarının hepsinde tek belirteç (ön tur sayımı; bu turda değişen dosyalar formlara dokunmuyor) |
| G5b | `bool` onay kutuları | GEÇTİ | Faz 6B formlarında yeni onay kutusu yok; `Views/**` içinde `type="hidden"` + `value="false"` 0 |
| G6 | em/en-dash 0 | GEÇTİ | `Src` görünümler, stiller, JS, controller, servis, ViewModel 0; eval `emdash` false; kanıt dosyaları 0 |
| G7 | `Up()` Drop yok | GEÇTİ | Faz 6B'de migration yok |
| G8 | Controller ve görünümde süre aritmetiği yok | GEÇTİ | `Controllers/`, `Views/` `TimeSpan`, `.Subtract(`, `AtUtc -` 0; pano saklı saniyeleri kullanıyor; çubuk uzunluğu yüzde ölçeği |
| G9 | Aynı CommandId tek etki | GEÇTİ | K17 satırı |
| G10 | Süper admin yeni adreste 403, menüde yok | GEÇTİ | `/dashboard` 302>403; Süper admin menüsü "Kullanıcılar", "Parola ve PIN"; pano ve rapor bağlantısı yok |
| G11 | Yerleşim imzası | GEÇTİ | K10 G11 bölümü; ön tur ve teslim imzaları aynı |
| G12 | Hizalama kusuru 0 | KALDI | Formlar ve paneller kusur 0; pano kartında rozet sağ kenarı 96 px (B1) |

Kabul özeti: Faz 6B kriterleri K1-K29 ve K17a 28/30 (K8, K10 KALDI); ortak kriterler G1-G12, G5a, G5b 12/14 (G3, G12 KALDI); toplam 40/44.

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | `--no-incremental -o .qa/f6b2/bchk`: 0 Uyarı, 0 Hata; `dotnet publish -c Release -o .qa/f6b2/build` başarılı |
| CSS | `npm run build:css` | QA çıktısı (`.qa/f6b2/app.check.css`) `wwwroot/css/app.css` ve yayındaki dosyayla bayt bayt aynı; `.fill-chart-1/2/3` derlenmiş CSS'te var |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme; sihirli rol metni 0 (`Authorize` ve `IsInRole`); `DashboardController` `[Authorize(Roles = Roles.ManagerOrAuditor)]`, `HomeController` `Roles.Any`; özniteliksiz iki dosya controller değil. MediatR, AutoMapper, generic repository, `new ...Service(` 0; `Migrate()` 0. `DashboardController` tek satır: servis çağrısı ve görünüm |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: `DashboardController`, `IDashboardReader`, `DashboardReader`, `ViewModels/Dashboard/Dashboard*ViewModel`, `RecordNumbers.WorkOrderScope/FormatWorkOrder/LooksLikeSystemWorkOrder`, `Views/Dashboard/Index.cshtml`, `wwwroot/js/Fullscreen.js` (`class Fullscreen`). §8: değişen dosyalarda `#region`, süs yorum, emoji, kalıp dil, `Helper/Utils/Manager` 0. `DashboardReader` 184, `RecordOpener` 200 satır. §7: yeni süre formülü yok; `IDashboardReader.GetAsync` özetinde "Bugün" ve "son 7 gün" tanımı yazılı ve kodla (`StartOfDayUtc`, `AddDays(1 - 7)`) uyumlu |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (`transition-all`, `outline-none`, `onclick`, CDN, `h-screen`, kütüphane, ham renk, sabit köşe, dash) |
| Mekanik eval | `UiDelivery.md §3` | Teslim 247 hücre (19 sayfa x 13 hücre) + gerileme 126 hücre: h1 1, atlama bağlantısı, yazı tipi Hanken Grotesk, `hScroll` false, dış script 0, konsol 0, üst bar 64 px, 1920'de sayfa 1600 px, reduced-motion'da animasyon 0, mobil gövde 16 px. `overflowing` yalnız 360 ve 320 px'te kart kipinde `sr-only` başlık satırının TR/TH öğeleri (kırpılmış, görünmez, ön tur N1; Faz 6 ölçümünde de vardı); `smallTargets` yalnız label içindeki onay ve radyo kutuları |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Faz 6B'de migration yok; `Data/Migrations` son değişikliği `8730e7e` (Faz 6), indeks 8 kayıt, `Docs/Db` değişmedi |

## 4. Yetki matrisi

Kolon eşlemesi: Viewer = Denetçi (3001), Editor = Operatör (2001), Admin = Süper admin (0001); Yönetici (1001) ve Operatör+Yönetici (4001) parantez içinde. Tam çıktı `Evidence/Faz-6B/YetkiMatrisi.txt`.

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/` | 302 (giriş) | 302 `/dashboard` (Yönetici 302 `/dashboard`) | 302 `/my-work` (4001 302 `/my-work`) | 302 `/users` |
| GET `/dashboard` | 302 (giriş) | 200 (Yönetici 200) | 302>403 (4001 200) | 302>403 |
| GET `/records/create` | 302 (giriş) | 302>403 (Yönetici 302>403) | 200 (4001 200) | 302>403 |
| POST `/records/create` | 302 (giriş) | 302>403 (Yönetici 302>403) | belirteçli 200 alan hatası / 302 başarı; belirteçsiz 400 | 302>403 |
| GET `/reports/durations` | 302 (giriş) | 200 (Yönetici 200) | 302>403 (4001 200) | 302>403 |
| GET `/reports/materials` | 302 (giriş) | 200 (Yönetici 200) | 302>403 (4001 200) | 302>403 |

Menüde gizli olup yetkisiz rolde adresle açılan sayfa yok. Ters durum: Operatör+Yönetici kullanıcısı `/dashboard`'a yetkili, ancak menüde ve alt çubukta bağlantısı yok (B5).

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/dashboard` (dolu, aktif liste boş, acil satırlı, Açıldı satırlı; Yönetici ve Denetçi) | Kart 2 sütun; Açıldı satırında rozet 96 px içeride (B1) | Kart 3 sütun; B1 | Kart 3 sütun; B1 | Tablo, satır 69 px; Son 7 gün satır 45 px | 1600 ile sınırlı | Temiz; grafik koyu renkleri `Pano-Grafik-1440-Koyu.png` | Temiz | 320x640 ve 640x400 temiz (B1 aynı) | Atlama bağlantısı ilk, menü, "Tüm kayıtlar", satır bağlantıları; hepsi halkalı |
| `/reports/durations` (dolu, boş süzgeç) | Kart 2 sütun, grafik taşması 0 | Kart 3 sütun | Kart 3 sütun | Tablo, satır 67 px | 1600 ile sınırlı | Temiz | Temiz | Temiz | Menü ve süzgeç alanları halkalı (tarih alanında `outline` 3 px) |
| `/records/create` (boş, sunucu alan hatası) | Temiz, G12 kusur 0 | Temiz | Ray yanında | Temiz | Ölçülmedi | Temiz | Ölçülmedi | Ölçülmedi | Ölçülmedi (Faz 4-5 sırası değişmedi) |
| Kart listeleri (14 görünüm, `/records/integrity/7` değişen alanlar dahil) | İmza aynı | İmza aynı | İmza aynı, raylı tablolar 2 sütun | Tablo | 1600 ile sınırlı | İmza aynı | Temiz | 320 ve 640 imza aynı | `/records` 768: süzgeç alanları sırayla halkalı |
| Kabuk (ray, çekmece, tam ekran) | Çekmece temiz | Çekmece ve tam ekran temiz | Ray ve tam ekran temiz | Ray ve tam ekran temiz | Ölçülmedi | Ölçülmedi | Geçiş yok; çekmece odağı B2 | Ölçülmedi | Çekmece Esc ve odak dönüşü; hareket azaltmada açılış odağı B2 |
| Faz 1-6A gerilemesi (Operatör 7, Süper admin 3, Yönetici 11 sayfa) | Temiz | Temiz | Temiz | Temiz | Ölçülmedi | Temiz | Ölçülmedi | Ölçülmedi | Ölçülmedi |

Dört durum: pano dolu, boş ("Şu an açık kayıt yok", aktif kayıtlar kapatıldıktan sonra) ölçüldü; yükleniyor durumu sunucu çizimli salt okunur sayfada yok; hata durumu genel hata sayfasına düşer (frontend devir notu; ViewModel'de hata alanı yok). E-30 boş süzgeçte boş durum kutusu var, durum grafiği B3.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Archivo başlık, Hanken Grotesk gövde, sayılar mono; `document.fonts.check` true.
- Palet: UYGUN. Grafik yalnız `chart-1`, `chart-2`, `chart-3` (açıkta rgb(33,82,160), rgb(196,112,14), nötr; koyuda açık tonlar); yeni ham renk yok. Acil dolgusunun açık temada boş ize karşıtlığı B6.
- Köşe ölçeği: UYGUN. 6 ve 10 px, gölge yok, tam yuvarlak yalnız avatar ve nokta.
- Gezinme arketipi: UYGUN. Yönetici ve Denetçi'de "Ana sayfa" ilk öğe, alt çubukta ilk üç bağlantı; ray daraltılabilir, çekmece ikincil yol.
- Sayfa iskeleti: UYGUN. Pano tek özet şeridi (bir büyük sayı, iki küçük), altında tablolar, 2xl'de Son 7 gün rayı; dört eşit KPI kartı yok.
- Veri dili: SAPMA (küçük). Kart listesi kartta yüzey, 2 px kenarlık, 14 px aralık, baş, gövde, ayak; panoda Açıldı satırında rozet başın sağ kenarında değil (B1).
- Hareket imzası: UYGUN. Ray 220 ms, çekmece ve perde geçişli, ilk yüklemede geçiş yok, reduced-motion'da geçiş yok (odak sorunu B2).
- İmza bileşen: UYGUN. Kayıt şeridi değişmedi.
- Anti-slop: temiz. Gradient, glow, eyebrow yığını, sürüm rozeti, emoji, em-dash, CDN yok; grafik gerçek veriden sunucuda çiziliyor; cam yüzey yalnız üst bar.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | ORTA | Panoda "Şu an sahada" kartında Açıldı durumundaki satırın durum rozeti kartın sağ iç kenarında değil: 360, 768 ve 1024 px'te rozetin sağ kenarı iç kenardan 96 px içeride; "Kilit bitişi 02:03" metni rozetle aynı satırda, rozetin sağında duruyor. Devam ediyor satırlarında rozet 0 px. G12 kart maddesi ("rozetin sağ kenarı kartın sağ iç kenarındadır"). Ön turda veride Açıldı kayıt olmadığı için görülmedi | `Evidence/Faz-6B/Pano-AcildiRozet-360.png`; `MekanikEval.txt` "SORUN ['rozet sağ -95.6']"; `.qa/f6b2/r-rz.json` | frontend-engineer | `Views/Dashboard/Index.cshtml:121-131`: `stack-badge` hücresi `Styles/app.css:721` kuralıyla satır yönlü flex, rozet `div`'i ve kilit `p`'si yan yana diziliyor. Kilit metni rozetin altına (tam satır) alınmalı ya da hücre içeriği tek sağa hizalı sütun olmalı; ölçüm 360/768/1024 Açıldı satırıyla yinelenir | KAPANDI (tur 2) |
| B2 | ORTA | Ön tur B3 kapanmadı: `prefers-reduced-motion: reduce` açıkken menü çekmecesi açılınca odak içeri girmiyor (`document.activeElement` BODY); Tab'a basınca tuzak odağı "Menüyü kapat"a getiriyor. Normal ayarda odak "Menüyü kapat". Kök neden: `.drawer` için `transition: none` eklendi, ancak genel kural (`*` için `transition-duration: 0.01ms`) `transition-property` varsayılanı `all` olan alt öğelerde görünürlüğü geçişe sokuyor; tıklama anında `.rail-brand` `visibility: hidden` ("all 1e-05s"), kapat bağlantısı da gizli; 50 ms sonra görünür ve `focus()` çalışıyor | `Evidence/Faz-6B/OnTurYenidenDogrulama.txt` "B3" ve "kök neden"; `.qa/f6b2/r-b3x.json` | frontend-engineer | `Styles/app.css:931` bloğunda çekmece alt öğelerini de geçişten çıkarmak (`.drawer *` için `transition: none`) ya da `ShellNav.js:94` odağını bir sonraki karede vermek; 360x640 ve 768x600'de reduce ile yinelenir | KAPANDI (tur 2) |
| B3 | ORTA | E-30'da boş süzgeç sonucunda (`from=2030-01-01&to=2030-01-31`, `kind=2&facilityId=2`) makine ve faz grafikleri yerine boş durum metni çıkıyor, ancak "Kayıt durumları" grafiği 7 durumun hepsi 0 olan çubuklarla çiziliyor. K8 "grafik yerine boş durum metni"; eksik boş durum | `Evidence/Faz-6B/Sure-BosSuzgec-1024.png`; `KabulKriterleri.txt` "BOŞ" satırları | frontend-engineer | `Views/Reports/Durations.cshtml:146` koşulu `StatusCounts.Length == 0`; liste her zaman 7 satır geldiği için hiç doğru olmuyor. Koşul "tüm sayılar 0" olmalı | KAPANDI (tur 2) |
| B4 | DÜŞÜK | Faz grafiğinin lejantında "Ortalama net" ve "Ortalaması asgarinin altında" kareleri aynı düz mavi (rgb(33,82,160)); desen yalnız çubuklarda. Satırlarda desen ve "Ortalama asgarinin altında" metni olduğu için anlam renge bağlı kalmıyor (NFR-05 sağlanıyor), lejant iki seriyi ayırmıyor | `Evidence/Faz-6B/Faz-Lejant-1440.png`; `.qa/f6b2/r-ev.json` "legend" | frontend-engineer | `Views/Shared/_BarChart.cshtml:28` lejant karesine `-hatch` tonunda desen katmanı eklemek | KAPANDI (tur 2) |
| B5 | DÜŞÜK | Operatör+Yönetici çoklu rollü kullanıcı (4001) `/dashboard`, raporlar ve tanımlara yetkili; menüsünde raporlar ve tanımlar var, ancak panoya giden bağlantı yok (ray, alt çubuk, çekmece). `/` bu kullanıcıyı `/my-work`'e gönderiyor (K1), pano yalnız adres yazılarak açılıyor | `KabulKriterleri.txt` "K2" om satırları; `Views/Shared/_Layout.cshtml:21-29` (`else if` Yönetici dalı) | analyst | Çoklu rolde panonun menüde yer alıp almayacağına karar; gerekiyorsa frontend `Views/Shared/_Layout.cshtml:21-29`'u günceller | KAPANDI (tur 2) |
| B6 | DÜŞÜK | Açık temada acil grafik dolgusu (`chart-2` rgb(196,112,14)) boş ize (`sunken`) karşı 2,94:1, grafik nesnesi için 3:1'in altında; panel zeminine karşı 3,51:1. Değer metinle de yazılıyor ("1 acil"). Koyu temada 7,10:1 | `Evidence/Faz-6B/OnTurYenidenDogrulama.txt` `/dashboard` açık tema satırları | frontend-engineer | `_Theme.cshtml:20` acil tonunu ya da `fill-sunken` izini 3:1 sağlayacak biçimde ayarlamak; `Dna.md` palet satırıyla birlikte | KAPANDI (tur 2) |

Ön tur bulguları (yerleşim ön turu `.qa/f6b`, rapor tablosuna alınmadı): ÖT-B1 YÜKSEK, grafik dolgu sınıfları derlenmiyordu, çubuklar siyahtı; bu turda kapandı (açık ve koyu temada `chart-1/2/3` renkleri, `Grafik-Sure-1440-Acik.png`, `Pano-Grafik-1440-Koyu.png`). ÖT-B2 ORTA, kart ayağında "İŞLEM" etiketi; kapandı (48 hücre, 384 ayakta etiket 0, düğmeler tek satır, sağa ve dikeyde ortalı). ÖT-B3 ORTA, hareket azaltmada çekmece odağı; kapanmadı, B2 olarak yukarıda.

## 7. Geçenler

- `/` yönlendirmesi beş rol bileşiminde doğru; pano yalnız Yönetici ve Denetçi'de (çoklu rolde Yönetici rolüyle) açılıyor, Operatör ve Süper admin 403.
- Pano sayıları ve son 7 gün DB ile birebir; Türkiye gece yarısı geçişi canlı ölçüldü (UTC 21:00'da "bugün" 15'ten 16 Kasım'a döndü, tamamlanan 4 -> 0, gün penceresi kaydı).
- Liste sınırları (10), sıralar, gerekçeler ve detay bağlantıları DB ile aynı; acil kayıt metinli "Acil" rozeti taşıyor.
- İş emri numarası: tesis ve yıl başına sıra (`T1-IE-2026-1..5`, `T2-IE-2026-1`), elle değer sayacı ilerletmiyor, yalnız boşluk boş sayılıyor, 64 karakter sınırı ve biçim reddi kayıt ve olay bırakmıyor, aynı CommandId tek kayıt; değer detayda, yürütmede, olayda ve aramada aynı; bütünlük "Doğrulandı".
- Grafikler sunucuda SVG; değerler tablolarla aynı; asgari işareti, desen ve metin işareti var; açık ve koyu temada renkler tokenlerden.
- Kart listesi deseni 14 görünümde ve panoda beklenen imzayla; yazdırmada tablo.
- Köşe tokenleri tutarlı; sabit köşe 0.
- Kabukta sabit logo ve alt blok, kayan menü, daraltma ve çekmece geçişleri, kayıtlı daraltmada geçiş yok, hareket azaltmada geçiş yok.
- Tam ekran düğmesi K26-K28.
- Ön tur bulguları ÖT-B1 ve ÖT-B2 kapandı.

## 8. Kapsam dışı / yapılamayan

- Tam ekrandan Esc tuşuyla çıkış: Playwright'ın gönderdiği Esc tarayıcının tam ekran kipinden çıkarmıyor; çıkış yolu `document.exitFullscreen()` ve ikinci tıklama ile `fullscreenchange` eşlemesi üzerinden doğrulandı.
- Production anahtarı süreç başına üretildiği için App_Data'dan gelen kayıt 1-6 bütünlükte ihlal gösterebilir; bu turda açılan kayıt 7-16 "Doğrulandı" ile ölçüldü.
- T2 tesisi kaynak veritabanında pasif (`IsActive 0`); K16'nın ikinci tesis adımı için QA kopyasında Yönetici arayüzüyle (`/facilities/edit/2`) etkinleştirildi. Kopyaya Yönetici arayüzüyle M03, M04, M05 makineleri, PLN eşlemesi ve sekiz QA operatörü, Süper admin arayüzüyle 4001 eklendi. Test parolası ve PIN yalnız kopyada `Users` tablosuna yazıldı (önceki fazlarla aynı yöntem).
- Pano "yükleniyor" durumu yok (sunucu çizimli sayfa); "hata" durumu genel hata sayfası, sorgu hatası üretilerek denenmedi. Süre altı ve son tamamlananlar listelerinin boş durumu, tamamlanmış kaydı olmayan bir veritabanı gerektirdiği için çalışan uygulamada görülmedi (görünümde `_State` var).
- 1920 px ve %200 zoom (320x640, 640x400) kart ve tablo ekranlarında ölçüldü; kabuk ve yeni kayıt formunda ölçülmedi.
- Chromium dışı tarayıcı, dokunmatik cihaz ve ekran okuyucu ile tur yapılmadı.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | `sr-only` tablo başlığının §3 eval'de `overflowing` sayılması (ön tur N1) | frontend-engineer | Görünür taşma ve kaydırma yok; eval yanlış pozitifi, Faz 6'dan beri aynı |
| A2 | `/shifts/create` tesis parametresiz 404 | analyst | Faz 2 davranışı, Faz 6B kapsamında değil; `?facilityId=1` ile 200 |
| A3 | B4, B5, B6 (DÜŞÜK) | frontend-engineer, analyst | Yazılı gerekçeyle ertelenebilir |

## 10. Analist kararı

```
FAZ 6B: İADE
Tarih: 2026-11-16
Gerekçe: 40/44; K8, K10, G3, G12 karşılanmadı, üç ORTA bulgu (B1, B2, B3) açık. Analist doğrulaması: B1 Views/Dashboard/Index.cshtml:121-131
  (rozet div'i ve kilit p'si satır yönlü flex hücrede yan yana, Styles/app.css:721), B3 Views/Reports/Durations.cshtml:146 (koşul Length == 0,
  liste her zaman 7 satır), B2 Styles/app.css:931 (genel 0.01ms kuralı) kodda aynen görüldü. KRİTİK ve YÜKSEK yok; diğer 40 madde kanıtlı geçti.
İade maddeleri (tur 2'de hepsi kapanır, erteleme yok):
  B1 ORTA, frontend-engineer: Açıldı satırında rozet başın sağ iç kenarında; kilit bitişi rozetin altında.
  B2 ORTA, frontend-engineer: hareket azaltmada çekmece açılınca odak içeride.
  B3 ORTA, frontend-engineer: tüm sayılar 0 iken durum grafiği yerine boş durum metni.
  B4 DÜŞÜK, frontend-engineer: faz grafiği lejantında desenli kare. Ertelenmez: aynı dosya ailesi (_BarChart), sunumda görünen ekran.
  B6 DÜŞÜK, frontend-engineer: açık temada acil dolgusu boş ize ve panel zeminine karşı en az 3:1; Dna.md palet satırı aynı değişiklikte.
    Ertelenmez: tek token, sunumda görünen grafik.
  B5 DÜŞÜK, analist kararı PL-84, frontend-engineer: Operatör ve Yönetici (ya da Denetçi) rolü birlikte olan kullanıcının rayında ve
    çekmecesinde "İzleme" grubunun ilk öğesi "Pano" (/dashboard, NavKey home, panoda aria-current="page"); / yönlendirmesi değişmez
    (/my-work, K1); alt çubuk değişmez. Tek rollü Operatör, Yönetici, Denetçi ve Süper admin menüleri değişmez.
Yeniden denetim şartı (qa-tester, Tur 2):
  B1: panonun üç kart tablosunda Açıldı ve Devam ediyor satırlarıyla 360, 768, 1024 px, açık ve koyu tema; G12 kart maddesi kusur 0,
    pano G11 imzası değişmemiş, 1440 tablo.
  B2: prefers-reduced-motion: reduce ile 360x640 ve 768x600'de çekmece açılınca document.activeElement çekmecenin içinde ("Menüyü kapat");
    normal ayarda K24 ve reduce ile K25 gerileme olarak yeniden.
  B3: from=2030-01-01&to=2030-01-31 ve kind=2&facilityId=2 süzgeçlerinde üç grafik yerine boş durum metni, konsol 0; dolu süzgeçte K7
    durum grafiği 7/7 tabloyla aynı.
  B4: lejantta iki seri açık ve koyu temada ayırt edilir (desen görünür).
  B6: chart-2 dolgusu açık temada boş ize ve panel zeminine karşı ölçülen karşıtlık en az 3:1, koyu temada gerileme yok; chart-2'yi kullanan
    pano ve E-30 grafikleri ile Acil rozeti yeniden görülür.
  B5: 4001 ile ray ve çekmecede "Pano" -> /dashboard, panoda aria-current; 1001, 2001, 3001, 0001 menüleri tur 1 ile aynı; / yönlendirmeleri K1 ile aynı.
  Ortak: K8, K10, G3, G12 yeniden işaretlenir; G1 derleme, §4 grep ve değişen sayfalarda konsol 0. A1 ve A2 açık kalem olarak devreder.
```

Tur 2 kararı: TESLİM ALINDI (2026-11-16); gerekçe "Tur 2" bölümünün karar bloğunda.

## Tur 2 (2026-11-16)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `8e99e02`); `Docs/Handover/frontend-engineer-20261116-0010.md` "Tur 2" bölümü |
| Ortam | LOCAL, Release build, port 5721 (Production), yayın `.qa/f6b3/build`; `.qa/f6b3/qa.db` = `App_Data/vaka.db` dosyasının yeni SQLite yedekleme API kopyası. Veri uygulama akışıyla: sekiz QA operatörü (Yönetici arayüzü), 4001 Operatör+Yönetici ve 4002 Operatör+Denetçi (Süper admin arayüzü), acil kayıt 7 (Devam ediyor), planlı kayıt 8 ve 9 (Açıldı, `T1-IE-2026-1`, `-2`). Süreçte üretilen rastgele olay anahtarı |
| Kapsam | Yalnız iade edilen maddeler: B1-B6, K8, K10, G3, G12; gerileme K24, K25, K7 durum grafiği; G1, §4 grep, değişen sayfalarda konsol |
| QA önerisi | GEÇTİ |
| Analist kararı | TESLİM ALINDI |

Yeniden test edilen maddeler:

| # | Yeniden denetim şartı | Sonuç | Kanıt |
|---|---|---|---|
| B1 | Panonun üç kart tablosunda Açıldı ve Devam ediyor satırları, 360/768/1024, açık ve koyu; G12 kart maddesi kusur 0, G11 imzası değişmemiş, 1440 tablo | KAPANDI | `Evidence/Faz-6B/OnTurYenidenDogrulama.txt` "Tur 2 B1": 360, 768 ve 1024 px'te Açıldı, Devam ediyor ve Acil + Devam ediyor satırlarında en sağdaki rozetin sağ kenarı kart iç kenarında (@R0). "Kilit bitişi" rozetin 8 px altında, sağ kenarı da iç kenarda (rozetAlt 471, kilitÜst 479, ikisinin sağı 330; açık ve koyu aynı). `MekanikEval.txt` "Tur 2": pano 8 hücre ve 19 sayfa x 11 hücrede kart kusuru 0, imzalar tur 1 ile aynı, 1440'ta tablo (satır 69 px, 10 px, `clip`). `Pano-AcildiRozet-360.png` tur 1 ve tur 2 yan yana |
| B2 | Hareket azaltmada 360x640 ve 768x600'de çekmece açılınca `activeElement` çekmecede; normal ayarda K24, reduce ile K25 gerileme | KAPANDI | "Tur 2 B2": dört hücrede (iki boyut x reduce/normal) açılıştan 60 ms sonra odak "Menüyü kapat", çekmecenin içinde. Tıklama anında `.rail-brand` `visibility: visible`, geçiş `none 0s`, odaklanan öğe A. Esc sonrası odak "Menü", çekmece gizli |
| K24 | Gerileme | GEÇTİ | `Kabuk.txt` "Tur 2": 50 ms'de panel x -126/-131 px, perde 0,28-0,35; 400 ms'de 0 ve 1. Kapalıyken odaklanabilen öğe 0; Esc ve perde kapatıyor, odak "Menü"ye dönüyor. JS kapalı `#menu` açıyor, kapat bağlantısı gizliyor (`KabulKriterleri.txt` "Tur 2 K24") |
| K25 | Gerileme | GEÇTİ | Ray 50-60 ms'de 72 ve 248 px (1440x600, 1024x600); çekmece 50-60 ms'de x 0, perde 1; normal ayarda daraltma 50 ms 145 px, 400 ms 72 px; kayıtlı daraltmada `transitionrun` 0; logo ve alt blok sabit |
| B3 | İki boş süzgeçte üç grafik yerine boş durum metni, konsol 0; dolu süzgeçte durum grafiği 7/7 tabloyla aynı | KAPANDI | `KabulKriterleri.txt` "Tur 2 K7/K8": `from=2030-01-01&to=2030-01-31` ve `kind=2&facilityId=2` 200, `figure` 0, `<rect` 0; durum bölümünde "Seçilen aralıkta kayıt yok.", altında "Filtreye uyan tamamlanmış kayıt yok"; tarayıcıda konsol 0 (`r-ev2.json`). Dolu süzgeçte durum grafiği 7/7 durum listesiyle aynı (Açıldı 2, Devam ediyor 2, Beklemede 0, Tamamlandı 4, Zaman aşımı 1, İptal 0, Yarıda 0). `Sure-BosSuzgec-1024.png` tur 1 ve tur 2 |
| B4 | Lejantta iki seri açık ve koyu temada ayırt edilir | KAPANDI | "Tur 2 B4": "Ortalaması asgarinin altında" lejant karesi düz dolgu + `url(#phase-chart-hatch)` desen katmanı; "Ortalama net" yalnız düz dolgu; açıkta rgb(33,82,160), koyuda rgb(122,170,235). Faz grafiğinde 5 desen (4 çubuk + 1 lejant). `Faz-Lejant-1440.png` tur 1 ve tur 2 açık ve koyu |
| B6 | `chart-2` açık temada boş ize ve panel zeminine en az 3:1, koyuda gerileme yok; pano ve E-30 grafikleri ile Acil rozeti | KAPANDI | "Tur 2 B6": açık tema `chart-2` (180,96,8) boş ize 3,61:1, panel zeminine 4,31:1 (tur 1: 2,94 ve 3,51); koyu tema 7,10 ve 7,99 (değişmedi). 16 grafik hücresinde taşma 0, `hScroll` false, grafik metni en düşük 5,71. E-30 grafikleri `chart-2` kullanmıyor (chart-1 ve chart-3), değerleri tur 1 ile aynı. Acil rozeti açıkta rgb(170,32,22) / rgb(249,224,220), koyuda rgb(246,130,116) / rgb(62,28,26), görünür (`danger` ailesi, `chart-2`'den bağımsız). `Dna.md` palet satırı acil `#B46008` |
| B5 | 4001 ile ray ve çekmecede "Pano" -> `/dashboard`, panoda `aria-current`; 1001, 2001, 3001, 0001 menüleri ve `/` yönlendirmeleri aynı | KAPANDI | `KabulKriterleri.txt` "Tur 2 B5": 4001 ve 4002 rayında ve çekmecesinde "İzleme" grubu "Pano" (`/dashboard`, panoda `aria-current="page"`), "Makine panosu", "Kayıtlar" sırasıyla başlıyor; alt çubuk "İşlerim, Yeni kayıt, Makine panosu, Menü" (değişmedi); `/` 302 `/my-work`. 1001 ve 3001 "Ana sayfa" -> `/dashboard` [current], 2001 "İşlerim" [current], 0001 "Kullanıcılar", "Parola ve PIN"; `/` yönlendirmeleri `/dashboard`, `/dashboard`, `/my-work`, `/users` (tur 1 ile aynı) |
| K8 | Yeniden işaret | GEÇTİ | B3 satırı; asgari işareti 5, desenli çubuk 4, satır metni "Ortalama asgarinin altında" |
| K10 | Yeniden işaret | GEÇTİ | B1 satırı; 19 sayfa x 11 hücre (360/768/1024/1440/1920/320, açık, koyu, reduce) G11 imza kümesi tur 1 ile aynı, kart kusuru 0, 1280 üstü tablo kipi kusur 0 |
| G3 | Yeniden işaret | GEÇTİ | B1, B2, B3 kapandı; §4 0; değişen sayfalarda konsol 0 (209 + 9 + 4 hücre); DNA sapması yok (veri dili maddesi B1 sonrası UYGUN, palet `chart-2` güncel) |
| G12 | Yeniden işaret | GEÇTİ | Kart maddesi (rozet, tanıtıcı, ayak) 222 hücrede kusur 0; G12 form ve panel ölçümü (`g12.tmpl.js`) 12 + 24 hücrede a-e kusur 0 (`[f]` eski PL-55 kuralı, tur 1'deki gibi sayılmadı) |
| G1 | Derleme | GEÇTİ | `dotnet build -c Release -warnaserror --no-incremental -o .qa/f6b3/bchk` 0 Uyarı 0 Hata; `npm run build:css` çıktısı `wwwroot/css/app.css` ve yayınla bayt bayt aynı |
| G2/§4 | Grep | GEÇTİ | Policy ve sihirli rol metni 0; §4 tablosu 0 ihlal; em/en-dash 0; sabit köşe 0 |

Tur 2 kabul özeti: tur 1'de KALDI olan K8, K10, G3, G12 GEÇTİ; toplam 44/44. Bulgular B1-B6 KAPANDI; açık KRİTİK, YÜKSEK, ORTA, DÜŞÜK bulgu 0.

Mekanik eval notları tur 1 ile aynı: `overflowing` yalnız 360 ve 320 px'te `sr-only` başlık satırının TR/TH öğeleri (A1); `smallTargets` yalnız label içindeki onay kutuları.

Kapsam dışı / yapılamayan (tur 2): Tur 1'de geçen kriterler yeniden test edilmedi (Workflow kuralı), yalnız B1-B6 düzeltmelerinin dokunduğu `app.css`, `_BarChart`, `_Theme`, `Durations`, `_Layout`, `ShellNav.js` alanlarının kart listesi, kabuk, grafik ve menü gerilemesi ölçüldü. Tam ekran Esc kısıtı tur 1 ile aynı.

Açık kalemler: A1 ve A2 devreder (bölüm 9).

Analist kararı (tur 2):

```
FAZ 6B: TESLİM ALINDI
Tarih: 2026-11-16
Gerekçe: 44/44; tur 1 iade maddeleri B1-B6 yeniden denetim şartlarıyla kapandı, K24 ve K25 gerilemesi geçti; açık KRİTİK, YÜKSEK, ORTA,
  DÜŞÜK bulgu yok. Analist doğrulaması kodda: B3 Durations.cshtml:146 koşulu "tüm sayılar 0"; B5 _Layout.cshtml:35 "Pano" öğesi ve :69 alt
  çubuk dışlaması; B6 _Theme.cshtml:20 chart-2 180 96 8 ve Dna.md palet satırı #B46008; B2 reduce bloğunda .drawer ve .drawer * geçişsiz.
İade maddeleri: yok   Yeniden denetim şartı: yok
Açık kalemler: A1 (frontend-engineer, eval yanlış pozitifi, görünür taşma yok) ve A2 (analyst, Faz 2 davranışı, Faz 7 öncesi karar)
  sonraki faza devreder.
```

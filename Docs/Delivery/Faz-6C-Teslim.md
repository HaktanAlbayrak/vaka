# Faz 6C Teslim Raporu: Kayıt zaman çizelgesi ve görevli seçimi

| Alan | Değer |
|---|---|
| Faz | 6C |
| Tarih | 2026-11-16 |
| Tur | 2 |
| Test edilen | `main` @ `çalışma ağacı` (HEAD `5ed7ad7`; Faz 6C kodu commit'lenmemiş, ön tur düzeltmeleri dahil). Kaynak test başında `.qa/f6c/src` altına kopyalandı ve oradan derlendi. Devir notları `Docs/Handover/backend-engineer-20261116-2100.md`, `Docs/Handover/frontend-engineer-20261116-2100.md`, `Docs/Handover/analyst-20261116-1800.md` |
| Ortam | LOCAL, Release build, port 5781 (Production), DB: `.qa/` kopyası. Yayın `.qa/f6c/build`; `.qa/f6c/qa.db` = `App_Data/vaka.db` dosyasının SQLite yedekleme API kopyası (kaynak salt okunur açıldı). `EventChain__HmacKey` geliştirme user-secrets değerinden yalnız süreç ortamına verildi (diske ve çıktıya yazılmadı), böylece eski kayıtların bütünlüğü ölçülebildi. K10 karşılaştırması için HEAD (`5ed7ad7`) ayrıca `.qa/f6c/build-head` olarak derlenip port 5782'de DB'nin ikinci kopyasıyla çalıştırıldı. Chromium (playwright-cli, oturum `f6c`). PID 32568 (başkasının Debug süreci) dokunulmadı |
| Test eden | qa-tester |
| QA önerisi | GEÇTİ (tur 2; tur 1 GEÇTİ, ORTA B1 ve B2 açıktı; analist İADE etti, B1, B2, B3, B6 tur 2'de kapandı) |
| Analist kararı | TESLİM ALINDI (tur 2, 2026-11-16). Tur 1: İADE (B1, B2, B3, B6). B5 ve B7 Faz 6E'ye devredildi |

**Test verisinin kurulumu.** DB'ye doğrudan tek yazım yapıldı: QA kopyasında Süper admin (`0001`) parola ve PIN özeti (`.qa/f6c/j/seed_admin.py`), çünkü kaynak DB'deki parolalar bilinmiyor. Kalan her şey uygulamanın uç noktalarıyla kuruldu: sayfadaki form okunup kendi belirteciyle gönderildi (JS kapalı tarayıcının gönderdiği gövde, `.qa/f6c/j/web.py`, `setup_users.py`, `flow.py`), görevli eklemelerinin ikisi tarayıcıda yalnız klavyeyle yapıldı.

- Süper admin `/users/edit/{id}` "Parola sıfırla" ve "PIN sıfırla" ile `1001`, `2001`, `2002`, `3001`'e geçici parola verdi; her biri `/account/first-login`'de kendi parolasını ve PIN'ini belirledi.
- `/users/create` ile: `2003` "Şükrüye İnceoğlu Karamustafapaşalıoğulları" (uzun, Türkçe büyük harf), `2004` "IŞIL ÖZGÜR", `2006` "Ahmet Yılmaz", `2005` "Pasif Operatör" (sonra `/users/deactivate/9` ile pasif), `1002` "Yönetici İki" (yalnız Yönetici), kısaltma için `2901`-`2951` "Saha Operatörü 01-51" (51 kişi).
- Molada operatör: `2001` kayıt 9'u açtı, adımı başlatıp bitirdi, mola başlattı (açık bırakıldı). Kendi açık kaydı olan: `2006` kayıt 10'u açıp adımı başlattı (Devam ediyor).
- Planlı kayıt 8 (sahip `2002`, iş emri `IE-QA6C-0101`): adım 30'a Şükrüye (klavye), adım 31'e IŞIL (klavye) eklendi; sahip adım 31'den kendini çıkardı; adım 30 iki görevliyle 70 sn; mola (kişi bitirdi); adım 31'i sahip başlattı; Faz 1 süre altı gerekçeyle; ikinci mola Yönetici `1001` tarafından gerekçeyle bitirildi; Faz 2 (adım 32'ye JS kapalı radyo yoluyla Saha Operatörü 05 eklendi) süre üstünde; kayıt tamamlandı.
- İptal kaydı 11 (sahip `2003`, `IE-QA6C-0301`): SKT'si geçmiş `L2026-001` gerekçeyle eklendi, adım başlatıldı, Yönetici gerekçeyle iptal etti.
- Zaman aşımı satırı QA kopyasındaki kayıt 1'den (mevcut `RecordTimedOut` olayı), iki güne yayılan kayıt kayıt 6'dan (mevcut).
- Ekran matrisi için `2002` kayıt 12'yi açtı (adım 46'ya Şükrüye eklendi).

## 1. Kapsam

- BE 1: `IRecordTimelineBuilder` / `RecordTimelineBuilder`: olay başına bir satır, `Sequence` sırası, Türkiye gününe göre gruplu çizelge; satır metinleri plan tablosundan, süre ve efor olay yükünden.
- BE 2: `RecordDetailViewModel.Timeline`, `RecordDetailReader` sözlükleri (ad, `Faz n / Adım m - başlık`, faz sırası), DI kaydı. Migration yok.
- BE 5: `GET /records/people?q=&stepId=` -> `{ items, truncated }`, 400/404/403 (sahip denetimi), yalnız aktif Operatör, adımda görevli olan yok, molada `IsAvailable=false`, Türkçe harf duyarsız arama; `Run?stepId=&people=` aynı süzgeç.
- FE 3-4: `_Timeline.cshtml` ("Olay günlüğü"nden önce), `.tl-*` stilleri, yazdırmada gizli.
- FE 6: `_Participants.cshtml` ve `ParticipantPicker.js` WAI-ARIA combobox/listbox, seçilen kişi ve "Değiştir", canlı durum bölgesi, JS kapalı yol.
- Ön tur düzeltmeleri: kayıt detayı 500, `.tool > summary` taşması, Tab'da odağın BODY'ye düşmesi, liste açıkken iskeletin yerleşimi itmesi, `.combo-option` tek dizilim, `.tl-mark` `rounded-ui`.

## 2. Kabul kriterleri

| # | Kriter | Sonuç (GEÇTİ/KALDI) | Kanıt |
|---|---|---|---|
| K1 | Yönetici "Zaman çizelgesi"ni "Olay günlüğü"nden önce görür; satır sayısı `CleaningEvents` sayısına eşit, sıra `Sequence` | GEÇTİ | `Evidence/Faz-6C/ZamanCizelgesi.txt`: kayıt 1-11'in hepsinde "çizelge olay günlüğünden önce: True", satır = `CleaningEvents` (2, 17, 14, 12, 12, 15, 5, 20, 4, 2, 5), DOM sırası `Sequence` sırası. `Details.cshtml:444` partial, `:446` olay günlüğü |
| K2 | Saat `OccurredAtUtc`'nin Türkiye saati, `HH:mm`, saniyeli `datetime`; iki güne yayılan kayıtta `dd.MM.yyyy` başlıkları | GEÇTİ | 108 satırın hepsinde `datetime` = UTC + 3 sa, saniyeli; görünür metin `HH:mm` ("saat uyuşmazlığı: 0"). Kayıt 6 canlı olarak iki gün: `15.11.2026` ve `16.11.2026` başlıkları, `<h3><time datetime="2026-11-15">`. Gruplama `RecordTimelineBuilder.cs:28` (`DateOnly.FromDateTime(OccurredAtLocal)`), yerel saat `:42` |
| K3 | İlk satır açanın adı, kayıt no, iş emri no | GEÇTİ | Kayıt 8: "Test Operatör 2 kaydı açtı / Kayıt no T1-H1-M02-P2026-1, iş emri IE-QA6C-0101"; kayıt 11 Şükrüye ile aynı kalıp; iş emri boş kayıt 7'de yalnız kayıt no |
| K4 | Tek ve iki görevli başlangıç metni, adlar yükteki görevliler, adım etiketi, "Başlatan" | GEÇTİ | Kayıt 8 satır 6 "Test Operatör 2 ve Şükrüye İnceoğlu Karamustafapaşalıoğulları birlikte adıma başladı" (yük `ParticipantUserIds:[4,6]`), satır 10 "IŞIL ÖZGÜR adıma başladı" + "Başlatan: Test Operatör 2" (yük `[7]`, yapan 4), satır 17 tek görevli. Etiketler `Faz 1 / Adım 1 - Makine Kapa` vb.; prosedür faz kimliği sırası `OrderNo`'dan farklı (faz 12 sıra 1, faz 11 sıra 2) ve etiketler `OrderNo`'ya göre doğru |
| K5 | Bitiş satırı bitireni, süreyi, eforu `sa dk sn` ile; `RecordSteps` ile aynı; iki görevlide efor = süre x 2 | GEÇTİ | `RecordSteps` 30: 70/140, 31: 18/18, 32: 35/70, 33: 35/35; çizelge "Süre 1 dk 10 sn Efor 2 dk 20 sn", "18 sn / 18 sn", "35 sn / 1 dk 10 sn", "35 sn / 35 sn". Saatli biçim kayıt 6'da "21 sa 7 dk 3 sn" |
| K6 | Mola satırları kişi ve süre, yönetici bitirdiyse adı ve gerekçe; süre altı faz metinle; tamamlanma net ve efor | GEÇTİ | Kayıt 8: "Test Operatör 2 mola başlattı", "Test Operatör 2 molayı bitirdi / Süre 20 sn", "Test Yönetici molayı yönetici olarak bitirdi / Gerekçe: Operatör molayı kapatmayı unuttu, vardiya amiri kapattı. / Süre 22 sn", "UYARI Faz 1 süre altında tamamlandı / Gerekçe: ... / Süre 1 dk 38 sn" (yük `NetSeconds` 98), "Kayıt tamamlandı / Süre 3 dk 10 sn Efor 4 dk 23 sn" (yük 190 / 263) |
| K7 | İptal satırı yönetici adı, gerekçe, kesilen adım; SKT uyarısı metinle; zaman aşımında kişi "Sistem" | GEÇTİ | Kayıt 11: "UYARI Test Yönetici kaydı iptal etti / Faz 1 / Adım 1 - Makine Kapa / Gerekçe: Makine arızası, temizlik iptal edildi."; "UYARI ... malzeme ekledi / Lot L2026-001, SKT geçmiş: ...". Kayıt 1 (gerçek zaman aşımı): "UYARI Kilit süresi doldu, kayıt zaman aşımına düştü / Kilit bitişi 15.11.2026 06:59:20" (açılış 05:29:20 + 90 dk). Satırda kişi adı hiç yazılmıyor; metin plandaki satır tablosuyla birebir aynı. "Sistem" ifadesi için bkz. B4 |
| K8 | Denetçi ve sahibi olmayan Operatör aynı bölümü görür, bölümde düğme ve form yok; yalnız Süper admin 403 | GEÇTİ | `YetkiMatrisi.txt` "K8": 1001, 3001, 2001, 2006, 2002 kayıt 8'de 20, kayıt 11'de 5 satır, bölümde `form/button/a/input` 0; tarayıcıda bölümde odaklanabilir öğe 0. `0001` 302 `/account/access-denied` > 403 |
| K9 | Yönetici `/records`'ta iş emri no ile kaydı bulur ve detayı açar | GEÇTİ | Tarayıcı: `query=IE-QA6C-0101` tek sonuç `/records/details/8`, tıklama ile detay açıldı, `h1` "Kayıt ayrıntısı" |
| K10 | Olay günlüğü, bütünlük rozeti ve yazdırma çıktısı değişmez; çizelge yazdırmada görünmez | GEÇTİ | `ResponsiveVeGerileme.txt` "K10": HEAD (5ed7ad7) ile çalışma ağacı aynı DB'de, kayıt 1-11: olay günlüğü bölümü bayt bayt aynı, çizelge dışındaki sayfa aynı (tek fark çizelgenin yerindeki boşluk). Yazdırma ortamı: çizelge `display:none` (yükseklik 0), olay günlüğü 20 satır görünür, denetim çıktısı başlığı duruyor. `_Timeline.cshtml:32` `print:hidden` |
| K11 | Faz 6C'den önceki kayıtta çizelge çizilir; migration yok, `CleaningEvents`'e satır eklenmez, bütünlük "Doğrulandı" | GEÇTİ | Kayıt 1-7 çizildi; kayıt 1-7 olay sayısı test başında 77, tüm görüntülemelerden sonra 77; bütünlük kayıt 1-12'de "Doğrulandı". Yeni migration yok (`Data/Migrations` değişmedi, son `20261115152212`) |
| K12 | Bilinmeyen olay türü tür adıyla tek satır; varsayılan dal kod okumasıyla | GEÇTİ | `RecordTimelineBuilder.cs:43-44`: her satır `Kind = Other`, `Title = RecordEventText.Label(EventType)` ile başlar; `switch` eşleşmezse bu değerler kalır (default dalı gerekmez). Görünümde `Other` -> `circle-dashed`, nötr (`_Timeline.cshtml:25`) |
| K13 | Çizelge 360-1440 px açık ve koyu yatay kaydırmasız; uyarı satırları metinle; G12 kusuru 0 | GEÇTİ | `ResponsiveVeGerileme.txt`: 5 kayıt x 12 hücre (360, 768, 1024, 1440, 1920 açık/koyu, 320 ve 640 yakınlaştırma) 60/60 temiz, `hScroll` false. Uyarı satırları (`IsWarning`) "UYARI" etiketi ve ikonla; tonlu satırların hepsi metinle anlaşılır. G12: işaret sütunu tek sol kenar, gövde tek sol kenar, dikey çizgi işaretin ortasında (±1 px), panel birinci düzey öğeleri iç boşluk çizgisinde: her ekran ve genişlikte kusur 0 |
| K14 | Odakla liste açılır; yalnız aktif Operatör; pasif, Operatör olmayan Yönetici, adımdaki görevliler yok; molada "Molada" seçilemez | GEÇTİ | `GorevliSecici.txt` K14: odakla `aria-expanded=true`, 50 seçenek + kısaltma notu. "pasif", "Pasif Operatör", "2005", "yönetici", "Yönetici İki", "1002", "Test Operatör 2", "2002" (sahip, adımda) boş sonuç; "Test" -> "Test Operatör 1 Sicil 2001 Eklenemez: Molada" `aria-disabled=true`. Adımda zaten görevli Şükrüye adım 30'da listelenmiyor (JS kapalı "şükr" boş) |
| K15 | Ad ve sicil parçası; "ahmet/AHMET/Ahmet"; "İ" küçük "i" ile, "I" "ı" ile; sicil son iki hane | GEÇTİ | "ahmet", "AHMET", "Ahmet" -> Ahmet Yılmaz; "inceoğlu", "İNCEOĞLU", "şükrüye", "ŞÜKRÜYE" -> Şükrüye; "ışıl", "IŞIL" -> IŞIL ÖZGÜR; "isil" boş (Türkçe kurala uygun); "06" -> Ahmet (2006) ve Saha 06 (2906); "51" -> 2951 |
| K16 | Klavye: Aşağı ok açar ve ilk seçilebiliri etkin yapar; ok tuşları seçilemeyeni atlar; Enter seçer, formu göndermez; Escape kapatır, odak alanda; Tab seçimsiz kapatır; akış faresiz tamamlanır | GEÇTİ | `GorevliSecici.txt` K16 (adım 31): 16 Tab ile "Görevleri düzenle", Enter, 2 Tab ile alan. Escape kapattı, odak alanda; kapalıyken Aşağı ok açtı ve "Ahmet Yılmaz" etkin; "1" yazınca 16 sonuç (son seçenek molada olan); Yukarı ok ilk seçenekten "Saha Operatörü 51"e sardı (molada olanı atladı), Aşağı ok yine atlayıp "01"e döndü, Home/End çalışıyor; Tab odak PIN'e geçti, liste kapandı, seçim yok; Enter "IŞIL ÖZGÜR, sicil 2004" seçti, URL değişmedi; 2 Tab PIN, 2 Tab "Görevliyi ekle", Enter: görevli eklendi (adım 31 görevlileri "Test Operatör 2, IŞIL ÖZGÜR"). Adım 30'da Şükrüye aynı akışla eklendi |
| K17 | ARIA öznitelikleri ve canlı bölge; erişilebilirlik ağacı | GEÇTİ | Kapalı: `role=combobox`, `aria-autocomplete=list`, `aria-expanded=false`, `aria-controls=step-30-results`, `aria-activedescendant` yok. Açık: `aria-expanded=true`, denetlenen öğe `role=listbox` "Bulunan kişiler" `tabindex=-1`, `aria-activedescendant` listedeki `role=option` seçeneğin kimliği (`aria-selected=true`), seçilemeyen `aria-disabled=true`. Durum `role=status` ve `aria-describedby` içinde. Ağaç: `combobox "Görevli ekle" [expanded]`, `listbox "Bulunan kişiler"`, `option ... [selected]` |
| K18 | Boş, hata, "Bir kişi seçin.", kısaltma, hızlı yazma | GEÇTİ | "zzzz": "Aramaya uyan aktif operatör yok.", liste kapalı. Uç nokta 500 ve bağlantı kesik (`internetdisconnected`): "Kişiler alınamadı. Bağlantıyı kontrol edip yeniden arayın." (`text-danger`), kişisiz gönderim engellendi. PIN dolu, kişi seçilmeden "Görevliyi ekle": "Bir kişi seçin." alanın hata yerinde, etiket, alan ve hata sol kenarı aynı (386 px), `aria-invalid`, odak alanda. Boş arama: "50 kişi bulundu. Liste kısaltıldı. Aramayı daraltın." ve listede not. "sa" yanıtı 1,8 sn geciktirilip "ahmet" yazıldı: sonuç yalnız Ahmet Yılmaz. PIN de boşken odak sırası için bkz. B1 |
| K19 | Seçilen kişi ad ve sicil ile; "Değiştir" temizler ve odağı alana verir; K-41 uyarısı; ret sonrası kişi korunur, PIN korunmaz | GEÇTİ | "Seçilen kişi: Ahmet Yılmaz, sicil 2006", "Değiştir" 44 px; uyarı "Bu kişinin kendi açık kaydı var. Yine de ekleyebilirsiniz."; "Değiştir" sonrası gizli değer boş ve `disabled`, odak alanda. Yanlış PIN ile ret: "PIN hatalı.", seçilen kişi ve uyarı duruyor, gizli değer 8, PIN alanı boş |
| K20 | `/records/people` 400/404/403/200; elle POST'ta pasif ve görevli reddi, olay yok | GEÇTİ | `YetkiMatrisi.txt`: stepId yok 400 (`q=a` ile de), metin 400, olmayan adım 404, sahip 200 `{items, truncated}`; sahibi olmayan Operatör (2001 molada, 2002, 2003, 2006), Yönetici, Denetçi, Süper admin 302 `/account/access-denied` > 403 (projenin 403 düzeni, Faz 2 K7 ile aynı); oturumsuz 302 giriş. Geçerli belirteçle POST: pasif "Pasif Operatör pasif, görevli olarak eklenemez.", Şükrüye ve sahip "bu adımda zaten görevli.", Yönetici İki "Kişi seçilemez.", molada "molada, görevli olarak eklenemez."; olay ve görevli sayısı 86/44 değişmedi. Sahibi olmayan 2002'nin başkasının adımına geçerli belirteçli POST'u 403, olay 108/108 |
| K21 | JS kapalıyken arama formu ve radyo listesi aynı süzgeçle, görevli eklenir | GEÇTİ | HTTP (adım 32): Türkçe harf çiftleri, pasif, yönetici, sahip ("2002") aynı sonuç; molada `disabled` "Eklenemez: Molada"; boş arama 50 + "Liste kısaltıldı"; radyo ile Saha Operatörü 05 eklendi (1 -> 2). JS kapalı Chromium bağlamı (360 px): combobox gizli, arama formu görünür, `people=ışıl` -> tek radyo IŞIL ÖZGÜR, seçip PIN ile gönderildi, eklendi |
| K22 | Panel 360-1440 açık ve koyu yatay kaydırmasız; liste alanı aşmaz; seçenek >= 44 px; G12 0; konsol 0 | GEÇTİ | `ResponsiveVeGerileme.txt` yürütme: 4 durum (hazır, liste açık, seçilmiş + uyarı, kişisiz gönderim) x 11 hücre 44/44 temiz. Liste sol farkı 0, genişlik farkı 0, alanın 6 px altında, 288 px yükseklikte kendi içinde kayıyor (`overflow-y:auto`); seçenek en az 66-67 px; etiket, alan, ipucu, hata ve seçilen kişi metni aynı sol kenar; "çıkar" özetleri 320 ve 360 px'te panele sığıyor. Konsol 0 |

Ortak kriterler: G1 GEÇTİ, G2 GEÇTİ, G3 GEÇTİ (ORTA B1, B2 açık), G4 GEÇTİ, G5 GEÇTİ, G5a GEÇTİ, G6 GEÇTİ, G7 GEÇTİ (migration yok), G8 GEÇTİ, G10 GEÇTİ, G12 GEÇTİ. G9 ve G11 uygulanmaz (plan).

## 3. Derleme ve mekanik kontroller

| Kontrol | Komut | Sonuç |
|---|---|---|
| Derleme | `dotnet build -c Release -warnaserror` | 0 uyarı, 0 hata (`.qa/f6c/src` kopyasında); `dotnet publish -c Release -o .qa/f6c/build` başarılı |
| CSS | `npm run build:css` | Kopyada aynı Tailwind CLI ile derlendi, çıktı repodaki `wwwroot/css/app.css` ile bayt bayt aynı (73808 bayt) |
| Test | `dotnet test` | Test projesi yok |
| Policy yasağı | `CodingStandards.md §4` grep | 0 eşleşme; sihirli rol metni 0; `[Authorize]`/`[AllowAnonymous]` taşımayan controller 0; MediatR, AutoMapper, generic repository 0; `Program.cs`'te `Migrate()` 0 |
| Adlandırma (PascalCase) | örneklem denetimi | Uygun: `RecordTimelineBuilder`, `IRecordTimelineBuilder`, `TimelineEntryKind` (üyeler PascalCase), `TimelineNames`, `RecordTimelineDayViewModel`, `RecordTimelineEntryViewModel`, `ParticipantSearchResponseViewModel`, `ParticipantPicker.js` (sınıf PascalCase, metotlar camelCase), `_Timeline.cshtml`, CSS `.tl-*`, `.combo-*` kebab. Özel alanlar `_clock`, `_timeline`, `_db` |
| UI grep tablosu | `UiDelivery.md §4` | 0 ihlal (değişen dosyalar `_Timeline`, `_Participants`, `Details`, `app.css`, `ParticipantPicker.js`). `slate-` eşleşmeleri `translate-` (yanlış pozitif); `app.css` `background: #fff` yazdırma katmanında ve HEAD'de de var (Faz 6); `...` yalnız JS yayılma işleci. G5b gizli `value="false"` 0 |
| Mekanik eval | `UiDelivery.md §3` | 104/104 hücre beklenen değerlerde: `h1` 1, `skip` true, `imgNoAlt`/`imgNoSize`/`inputNoLabel`/`iconBtnNoName` 0, `emdash` false, font Hanken Grotesk (`fonts.check` true), `hScroll` false, `overflowing` 0, `smallTargets` 0, `extScripts` [], üst bar 64 px, gövde 16 px. Not: malzeme tablosunun görsel olarak gizli (`clip: rect(0 0 0 0)`) başlık satırı ham eval'de taşma sayılıyordu; ekran dışı erişilebilirlik metni olduğu için sayımdan çıkarıldı (HEAD'de de aynı) |
| Kayıtsız migration | `git log -- Src/**/Migrations` x `Docs/Db/DbChanges.md` | Yeni migration yok; `Data/Migrations` çalışma ağacında değişmemiş, QA DB'de son migration `20261115152212_AddCleaningRecordListIndexes` |

Kod okuması (SOLID, §7, §8):

- Controller: `RecordStepsController.People` yalnız model bağlama, servis çağrısı ve sonucu 400/404/403/JSON'a çevirme; sahiplik kuralı servis sorgusunda (`ParticipantDirectory.cs:22-38`, K-30). `new` ile servis ve servis bulucu yok; bağımlılıklar arayüzle constructor'dan.
- §7: Bu fazda süre hesaplayan yeni metot yok. `RecordTimelineBuilder` süreyi olay yükünden taşır, aritmetik yapmaz (`RecordTimelineBuilder.cs:16-19` `<remarks>` ve `IRecordTimelineBuilder.cs:15-19` bunu yazıyor, kodla uyumlu). Görünüm ve JS'te süre aritmetiği yok (G8 grep 0).
- §8: emoji, `#region`, süs yorum, `Helper`/`Utils`/`Manager` adı, kodu tekrar eden yorum 0 eşleşme (değişen 13 dosya). Görünür metinde, devir notlarında ve `PhasePlan.md`'de em/en-dash 0.
- `RecordDetailReader.cs` 311 satır (HEAD 293); bkz. B5.

## 4. Yetki matrisi

Rollere karşılık: Viewer = Denetçi (3001), Editor = Operatör, Admin = Yönetici (1001); Süper admin (0001) ayrı yazıldı. 403 = 302 `/account/access-denied` > 403 (projenin düzeni).

| Route | Oturumsuz | Viewer | Editor | Admin |
|---|---|---|---|---|
| GET `/records/details/{id}` (kayıt 8, 10, 11) | 302 | 200 | 200 (sahip ve sahibi olmayan) | 200; Süper admin 403 |
| GET `/records/people?stepId=39` (kayıt 10, sahip 2006) | 302 | 403 | sahip 200; sahibi olmayan (2001, 2002, 2003) 403 | 403; Süper admin 403 |
| GET `/records/people` (stepId yok) / `?stepId=99999` / `?stepId=abc` | 302 | 403 | sahip oturumuyla 400 / 404 / 400 | 403 |
| GET `/records/run/10?stepId=39&people=saha` | 302 | 403 | sahip 200 (aday listesi); sahibi olmayan 2002 200 ama aday listesi ve görevli formu yok; molada 2001 302 `/breaks/current` | 403; Süper admin 403 |
| POST `/records/add-participant/{stepId}` belirteçsiz | - (denenmedi) | - (denenmedi) | 400 (sahip ve sahibi olmayan) | 302 > 403 (yetki filtresi antiforgery'den önce) |
| POST `/records/add-participant/39` geçerli belirteçle, sahibi olmayan | - | - | 403, olay yazılmadı | - |
| POST `/records/remove-participant/30` belirteçsiz | - | - | 400 | - |

Menüde yeni bağlantı yok (yeni adres yok); `/records/people` ve detay Süper admin'de 403 (G10).

## 5. Responsive ve görsel tur

| Sayfa | 360 | 768 | 1024 | 1440 | 1920 | Koyu | Reduced-motion | %200 zoom | Klavye |
|---|---|---|---|---|---|---|---|---|---|
| `/records/details/8` (20 satır, iki görevli, molalar, süre altı) | Temiz (saat üstte, cümle altta) | Temiz | Temiz | Temiz | Temiz | Temiz (360-1440, 1920) | Temiz; çizelgede animasyon ve geçiş 0 | 320 ve 640 px temiz | İlk Tab "İçeriğe geç"; çizelgede odaklanabilir öğe 0 |
| `/records/details/11` (iptal, SKT, kesilen adım) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz |
| `/records/details/6` (iki gün) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz |
| `/records/details/1` (zaman aşımı) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz |
| `/records/details/12` (devam eden) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz |
| `/records/run/12` görevli seçici: hazır | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | K16 turu temiz |
| `/records/run/12` liste açık (50 + not) | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz |
| `/records/run/12` seçilmiş kişi + K-41 uyarısı | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | "Değiştir" odağı alana verir |
| `/records/run/12` kişisiz gönderim | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | Temiz | PIN de boşsa odak PIN'e gidiyor (B1) |

Konsol: matris turlarında 0 hata ve uyarı (bilerek 500 ve bağlantı kesintisi verilen K18 adımları hariç); sunucu günlüğünde `fail`/`crit`/exception 0. Kontrast (bilgisayarla ölçüldü): açık temada çizelge soluk metin 6,48:1, başlık 15,96:1, "UYARI" etiketi 5,58:1, ton işaretleri en az 5,58:1; koyu temada en az 5,86:1. Dört durum: seçicide yükleniyor (liste kapalıyken iskelet + "Kişiler aranıyor…", liste açıkken eski sonuçlar `opacity .6` ve `aria-busy`, yerleşim kayması 0 px), boş, hata ve dolu ölçüldü; çizelge sunucuda çizilir, boş durumu ("Çizilecek olay yok.", `_Timeline.cshtml:35-38`) her kaydın açılış olayı olduğu için yalnız kod okumasıyla.

Ön tur düzeltmelerinin yeniden doğrulaması:

- Kayıt detayı 500: oturumlu yönetici `/records/details/1`-`12` hepsi 200, günlükte exception 0.
- `.tool > summary` taşması: "Şükrüye İnceoğlu Karamustafapaşalıoğulları çıkar" 320 px'te 148, 360 px'te 188 px genişlikte, 3-4 satır, panele sığıyor (`SeciciAcik-360.png`).
- Tab'da odağın BODY'ye düşmesi: 50 seçenekli listede 30 kez Aşağı ok ("Saha Operatörü 28" etkin), Tab -> odak `pin-add-person-47` (INPUT), liste kapandı.
- Liste açıkken iskelet: 1,5 sn geciktirilmiş yanıtta liste görünür kaldı, `opacity` 0,6, `aria-busy=true`, iskelet gizli, listenin üst kenarı 0 px kaydı; yanıttan sonra `aria-busy` kalktı.
- `.combo-option`: ad üstte (18 px, 700), "Sicil" altta (JetBrains Mono 14 px), aynı sol kenar, satır 66 px.
- `.tl-mark`: `border-radius` 6 px (`--radius-ui`) her hücrede.

Tasarım DNA'sı uygunluğu (`Docs/Design/Dna.md` kart maddeleri):

- Yazı tipi üçlüsü: UYGUN. Başlık Archivo, gövde Hanken Grotesk, saat, sicil ve süre JetBrains Mono. Adım etiketinin (Türkçe başlık metni) de mono yazılması için bkz. B3.
- Palet: UYGUN. Yalnız tokenler (`accent`, `ok`, `warn`, `danger` ve `-soft` zeminleri); ham hex yok; AA kontrast.
- Köşe ölçeği: SAPMA (küçük). İşaret `rounded-ui` 6 px (ikonlu işaret, nokta göstergesi değil; FE kararı DNA ile uyumlu), panel `--radius-card`. Ancak "UYARI" etiketi (`.tl-flag`, `app.css:1009-1010`) 1 px kenarlıklı; DNA "Kenarlık 2 px" ve rozet "ikon + metin + 2 px kenarlık" diyor (B2).
- Gezinme arketipi: UYGUN (değişmedi).
- Sayfa iskeleti: UYGUN. Çizelge detayın tam genişlik bölümlerinde, olay günlüğünden önce.
- Veri dili: UYGUN. Uyarı satırı renk dışında "UYARI" metni ve üçgen ikonla; seçicinin boş, hata ve yükleniyor durumları metinli.
- Hareket imzası: UYGUN. Çizelgede animasyon yok; seçicide süs hareketi yok.
- İmza bileşen: UYGUN (kayıt şeridi değişmedi; seçici yürütme ekranının `lot-results` diliyle aynı).
- Anti-slop (`DesignDna.md §4`): temiz. Kart yığını değil dikey çizelge; renkli durum noktası yok (ikonlu işaret); gölge, gradient, emoji, elle SVG yok (ikonlar Lucide sprite'ı); em/en-dash 0; "…" doğru karakter.

## 6. Bulgular

| # | Şiddet (KRİTİK/YÜKSEK/ORTA/DÜŞÜK) | Bulgu | Kanıt | Sahibi | Önerilen düzeltme | Durum |
|---|---|---|---|---|---|---|
| B1 | ORTA | "Görevliyi ekle" kişi ve PIN boşken gönderilince ilk hata "Bir kişi seçin." alanın altında çıkıyor, ama odak ikinci hatalı alana (PIN) gidiyor. `ParticipantPicker.js:53-58` arama alanına odaklanıyor, ardından `FormValidation.js:5-10` ilk `[required]` alana (PIN) odaklanıyor; combobox `required` olmadığı için sayılmıyor. `UiDelivery.md §2`: boş form gönderiminde odak ilk hatalı alana gider. PIN doluyken odak doğru (K18) | `Evidence/Faz-6C/SeciciBosGonderim-768.png` (odak halkası PIN'de); `.qa/f6c/j/focus.js` çıktısı `{"focus":"pin-add-person-46","order":["Bir kişi seçin.","Bu alanı doldurun."]}`; 360-1440 tüm hücrelerde aynı | frontend-engineer | Seçicinin denetimi `FormValidation`'ın sırasına katılsın: kişi seçilmemişse odak son olarak arama alanına verilsin (ör. `FormValidation` sonrasında çalışan dinleyici ya da arama alanına `data-required-message` ile ortak denetim). PIN boş, kişi boş ve yalnız biri boş üç durumda odak yeniden ölçülür | KAPANDI (tur 2) |
| B2 | ORTA | Çizelgedeki "UYARI" etiketi rozet görünümünde (ikon + metin + kenarlık) ama kenarlığı 1 px; DNA kartı "Kenarlık 2 px" ve rozet "2 px kenarlık" diyor. `UiDelivery.md §5` DNA sapması kabul etmez | `app.css:1009-1010` `.tl-flag ... border border-warn`; eval `flagBorder: 1px` (her hücre); `Evidence/Faz-6C/CizelgeKayit11-1440-Koyu.png` | frontend-engineer | `border-2` ya da DNA kararıyla `_Badge` uyarı biçimi; 360 ve 1440 px'te açık ve koyu yeniden ölçülür | KAPANDI (tur 2) |
| B3 | DÜŞÜK | Adım etiketi ("Faz 2 / Adım 2 - Alanı kontrol et ve teslim et") tümüyle mono yazılıyor; DNA mono'yu kayıt no, sayaç, süre ve PIN için tanımlıyor. 360 px'te Türkçe başlık mono olduğu için 2-3 satıra kırılıyor | `_Timeline.cshtml:63` `<span class="mono">@entry.StepLabel</span>`; `Evidence/Faz-6C/CizelgeKayit8-360.png` | frontend-engineer | Etiketi gövde yazı tipine almak ya da yalnız "Faz n / Adım m" kısmını mono bırakmak | KAPANDI (tur 2) |
| B4 | DÜŞÜK | K7 "zaman aşımı satırında kişi 'Sistem'dir" diyor, plandaki satır tablosu ise `RecordTimedOut` başlığını adsız veriyor ("Kilit süresi doldu, kayıt zaman aşımına düştü"). Uygulama tabloya uyuyor; satırda "Sistem" yazmıyor (yapan olmayan diğer olaylar için `SystemName` var, `RecordTimelineBuilder.cs:7`). Olay günlüğünde aynı olayın yapanı "Sistem" | Kayıt 1 satır 2 (`ZamanCizelgesi.txt`); `PhasePlan.md` Faz 6C satır metinleri tablosu ve K7 | analyst | Hangisinin geçerli olduğuna karar: metin olduğu gibi kalır ya da backend başlığa "Sistem" ekler | KAPANDI (analist kararı: metin kalır, K7 GEÇTİ, PL-95) |
| B5 | DÜŞÜK | `RecordDetailReader` 311 satır (HEAD 293); `CodingStandards.md §2` 200 satırı geçen servisi bölünmeye aday sayar. Faz 6C olay ve çizelge sözlüklerini aynı sınıfa ekledi | `wc -l Services/RecordDetailReader.cs`; `RecordDetailReader.cs:270-310` | backend-engineer | Olay okuma ve sözlük kurma (`ReadEventsAsync`) ayrı bir okuyucuya alınabilir; zorunlu değil | DEVREDİLDİ (Faz 6E, analist kararı) |
| B6 | DÜŞÜK | Görevli çipinde virgülden önce boşluk: "Test Operatör 2 2002 , sahip". Satır Faz 5'ten (bu fazda değişmedi), dosya bu fazda değişti | `_Participants.cshtml:23`; `Evidence/Faz-6C/SeciciBosGonderim-768.png` (çip satırı) | frontend-engineer | ", sahip" metnini sicil `span`'ının içine ya da hemen bitişiğine almak | KAPANDI (tur 2) |
| B7 | DÜŞÜK | `FormValidation.mark` hata öğesini silerken alanın `aria-describedby` listesindeki kimliği bırakıyor, alan yeniden hatalı olunca kimliği tekrar ekliyor: kimlik yinelenir ve öğe yokken boşa işaret eder. Faz 6C öncesinden kalma (HEAD'de aynı satır); tur 2'de seçici kişi seçilince hata öğesini sildiği için daha sık görülüyor. Hata metni görünür ve okunuyor, odak doğru | Seçicide 3 döngü sonrası `aria-describedby` = "step-46-hint step-46-status step-46-search-error step-46-search-error step-46-search-error step-46-search-error"; `/materials/create` `Code` için "Code-hint Code-error Code-error" (`Evidence/Faz-6C/Tur2Olcumler.txt`); `FormValidation.js:40`, `:43-44`; `ParticipantPicker.js` `showFieldError` | frontend-engineer | Eklemeden önce listede kimlik var mı bakmak, hata silinirken kimliği listeden çıkarmak | AÇIK |

## 7. Geçenler

- K1-K22 kanıtlarıyla geçti; çizelge 11 kayıtta (108 satır) DB ile satır, sıra ve saniyeli saat olarak birebir.
- Çizelge metinleri plan tablosuyla aynı: tek, iki ve "Başlatan" başlangıç; bitiş süre ve efor; molalar (kişi ve yönetici, gerekçe); süre altı faz; tamamlanma; iptal ve kesilen adım; SKT geçmiş lot; zaman aşımı ve kilit bitişi.
- Gerileme: HEAD ile aynı DB'de olay günlüğü ve çizelge dışındaki detay sayfası 11 kayıtta aynı; yazdırmada çizelge gizli; bütünlük "Doğrulandı"; eski kayıtlarda olay sayısı değişmedi.
- Görevli seçici: aktif Operatör süzgeci, molada nedenli seçilemez, Türkçe harf duyarsız arama, kısaltma, yalnız klavye akışı, ARIA ağacı, canlı bölge, boş ve hata durumları, eski yanıtın atılması, K-41 uyarısı, ret sonrası seçimin korunması, JS kapalı yol (HTTP ve JS kapalı tarayıcı).
- Sunucu denetimi: pasif, zaten görevli, Operatör olmayan ve molada kişinin elle POST'u nedenli reddedildi, olay yazılmadı; sahibi olmayanın POST'u 403.
- Ön tur düzeltmelerinin altısı tarayıcıda yeniden doğrulandı.
- Mekanik eval 104/104, grep tablosu 0, konsol 0, kontrast AA, derleme 0 uyarı, CSS çıktısı repodakiyle aynı, G5a her post formu kendi belirteciyle.

## 8. Kapsam dışı / yapılamayan

- Canlı gün geçişi: K2'nin izin verdiği gibi yeni kayıtta beklenmedi; QA kopyasındaki kayıt 6 zaten iki güne yayıldığı için iki gün başlığı canlı ölçüldü.
- Yeni zaman aşımı üretimi: `RecordTimeouts` ile yeni kayıt zaman aşımına düşürülmedi (kilit süresi 90 dk); mevcut kayıt 1'in gerçek `RecordTimedOut` olayı kullanıldı.
- Bilinmeyen olay türü: tablo tetikleyicileri ve bütünlük zinciri nedeniyle DB'ye uydurma olay yazılmadı; K12 plandaki gibi kod okumasıyla.
- "Yarıda kapatma" (`RecordAborted`) satırı bu turda canlı üretilmedi; iptal ile aynı dalda (`RecordTimelineBuilder.cs` `RecordCancelled`/`RecordAborted` ortak case) kod okumasıyla.
- Çizelgenin boş durumu her kaydın açılış olayı olduğu için arayüzde oluşmuyor; kod okumasıyla.
- Ekran okuyucu ile sesli tur yapılmadı; erişilebilirlik ağacı ve öznitelikler Playwright ile ölçüldü.
- Mobil yatay yön ayrıca alınmadı; 640x400 hücresi yatay telefon yerine sayıldı.

## 9. Açık kalemler (sonraki faza devreden)

| # | Kalem | Sahibi | Gerekçe |
|---|---|---|---|
| A1 | B3, B5, B6 (DÜŞÜK) analist kararıyla ertelenebilir | frontend-engineer, backend-engineer | Cila ve bölünme önerisi; işlev ve yerleşim etkilenmiyor |
| A2 | B4 metin kararı | analyst | Plan içi çelişki; uygulama satır tablosuna uyuyor |

## 10. Analist kararı

```
FAZ 6C: İADE
Tarih: 2026-11-16
Gerekçe: K1-K22 kanıtlarıyla karşılandı. Kanıtı kendim doğruladım: B1 (ParticipantPicker.js:53-58 ve FormValidation.js:5-10,
  odak en son PIN'e gidiyor), B2 (app.css:1010 .tl-flag tek "border", Dna.md rozet "2 px kenarlık"), B6 (_Participants.cshtml:23),
  B5 (RecordDetailReader.cs 311 satır), K7 (RecordTimelineBuilder.cs:145 başlık adsız, :35 yapan yoksa "Sistem").
  B1 UiDelivery §2'yi, B2 §5'i ihlal ediyor; ORTA aynı fazda kapanır ve UI fazı sıfır ihlalle teslim edilir (CLAUDE.md kural 8).
İade maddeleri:
  B1 ORTA, frontend-engineer: kişi seçilmemişse odak en son arama alanında kalır (FormValidation sırasına katılır).
  B2 ORTA, frontend-engineer: .tl-flag kenarlığı 2 px.
  B3 DÜŞÜK, frontend-engineer: ertelenmez (bu fazın dosyası, sunumda görünen çizelge, PL-84 emsali). Etiketin "Faz n / Adım m" kısmı
    mono kalabilir, adım başlığı gövde yazı tipine geçer.
  B6 DÜŞÜK, frontend-engineer: ertelenmez (bu fazda değişen dosya, sunumda görünen çip). ", sahip" sicil no'ya bitişik.
  B4 DÜŞÜK, analist kararı: metin olduğu gibi kalır, kod değişmez. Satır tablosu geçerlidir; K7'deki "kişi Sistem" satırın bir kişiye
    atfedilmediği anlamındadır (satırda ad yok, olay günlüğünde yapan "Sistem"). K7 GEÇTİ olarak kalır; PhasePlan K7 metni düzeltildi (PL-95).
  B5 DÜŞÜK, backend-engineer: Faz 6E'ye devredildi. RecordDetailReader Faz 6E kalem 5'te yeniden değişiyor; olay okuma ve sözlük kurma
    orada ayrı okuyucuya alınır. Şimdi bölmek sunum öncesi gerileme riski getirir, işlev etkilenmiyor.
Yeniden denetim şartı: B1 üç durumda (kişi ve PIN boş, yalnız kişi boş, yalnız PIN boş) odak ilk hatalı alanda, 360, 768, 1440 px;
  B2 flagBorder 2 px, 360 ve 1440 px açık ve koyu; B3 360 px'te etiket ve kırılma; B6 çip metni. Gerileme: K16-K19, K22 ve K13
  (yalnız değişen dosyalar), mekanik eval ve grep tablosu değişen dosyalarda, konsol 0.
```

```
FAZ 6C: TESLİM ALINDI (tur 2)
Tarih: 2026-11-16
Gerekçe: K 22/22; tur 1'in iade maddeleri B1, B2, B3, B6 kapandı, gerileme (K13, K16-K19, K22, diğer formlarda FormValidation)
  temiz; KRİTİK, YÜKSEK, ORTA açık bulgu yok. Kendim doğruladım: app.css:1010 .tl-flag "border-2"; _Participants.cshtml:23 çip
  metni tek satır içi kapta; B7 için FormValidation.js:40 ve :43-44, aynı satır HEAD 5ed7ad7'de de var.
Açık kalemler (devir):
  B5 DÜŞÜK, backend-engineer: Faz 6E kalem 5 (PL-95).
  B7 DÜŞÜK, frontend-engineer: Faz 6E'de kapanır. Gerekçe: Faz 6C öncesinden kalma; kusur ortak FormValidation.js'te ve tüm
    formları etkiler, düzeltmesi tüm formlarda gerileme ölçümü ister; görünür etkisi yok (hata metni görünür ve okunuyor, odak doğru).
    Tur 3 sunum öncesi Faz 6D'yi geciktirirdi. Faz 6E görevli formlarına zaten dokunuyor; ölçüm orada seçici ve en az iki başka formda yapılır.
```

## Tur 2 (2026-11-16)

| Alan | Değer |
|---|---|
| Test edilen | `main` @ `çalışma ağacı` (HEAD `5ed7ad7`), tur 2 düzeltmeleri dahil; kaynak `.qa/f6c2/src` kopyasından derlendi. Tur 1 kopyasına göre değişen dosyalar: `wwwroot/js/FormValidation.js`, `wwwroot/js/ParticipantPicker.js`, `Views/Records/_Participants.cshtml`, `Views/Records/_Timeline.cshtml`, `Styles/app.css`, `wwwroot/css/app.css` (satır sonu farkı yok sayılarak karşılaştırıldı); backend değişmedi |
| Ortam | LOCAL, Release build, port 5791 (Production), DB: `.qa/f6c2/qa.db` = tur 1 QA kopyasının (`.qa/f6c/qa.db`, test verisi dahil) SQLite yedekleme API kopyası. HMAC anahtarı tur 1'deki gibi yalnız süreç ortamında. Sunucu PID 1664 test sonunda kapatıldı, playwright oturumu `f6c2` kapatıldı; PID 32568'e dokunulmadı |
| Derleme | `dotnet build -c Release -warnaserror` 0 uyarı, 0 hata; CSS kopyada derlendi, repodaki `wwwroot/css/app.css` ile bayt bayt aynı (`.tl-flag` `border-width:2px`) |
| Kanıt | `Evidence/Faz-6C/Tur2Olcumler.txt` (yeni); `CizelgeKayit8-360.png`, `CizelgeKayit11-1440-Koyu.png`, `SeciciBosGonderim-768.png` tur 2 görüntüleriyle değiştirildi (toplam 10 dosya) |

Yeniden test edilen maddeler:

| Madde | Ölçüm | Sonuç |
|---|---|---|
| B1 | Kayıt 12 adım 46, düğmeye klavyeyle gelip Enter; 360, 768, 1440 px. Kişi ve PIN boş: odak `step-46-search`, hatalar "Bir kişi seçin." ve PIN "Bu alanı doldurun.", ikisi de `aria-invalid`. Yalnız kişi boş (PIN dolu): odak `step-46-search`, tek hata "Bir kişi seçin.". Yalnız PIN boş (kişi seçili): odak `pin-add-person-46`, kişi hatası yok. Kişi hatasından sonra kişi seçilince hata öğesi ve `aria-invalid` kalkıyor, ardından gönderimde odak PIN'e gidiyor. Hata `data-field` kabının içinde; etiket, alan ve hata sol kenarı aynı (360: 114, 768: 138, 1440: 394 px). Form gönderilmedi (URL aynı), konsol 0 | KAPANDI |
| B2 | Kayıt 8 ve 11, 360 ve 1440 px açık ve koyu: tüm `.tl-flag` `2px solid`; çizelge matrisinde 60 hücrenin hepsinde `flagBorder` 2 px | KAPANDI |
| B3 | 360 px: "Faz n / Adım m" JetBrains Mono, " - <başlık>" Hanken Grotesk; etiketlerin kırılması tur 1'de 2 satır olan "Faz 1 / Adım 2 - Makine Kontrol" artık 1 satır, en uzun "Faz 2 / Adım 2 - Alanı kontrol et ve teslim et" 2 satır (1440 px'te 1), taşma 0 | KAPANDI |
| B6 | Çip metni "Test Operatör 2 2002, sahip" (virgül sicil no'ya bitişik), adım 46 ve 47'de; `SeciciBosGonderim-768.png` | KAPANDI |
| B4 | Analist kararı (PL-95): metin kalır, K7 GEÇTİ | KAPANDI (yeniden test gerekmedi) |
| B5 | Analist kararı: Faz 6E'ye devredildi | DEVREDİLDİ |

Gerileme:

| Kontrol | Ölçüm | Sonuç |
|---|---|---|
| K13 | Kayıt 8, 11, 6, 1, 12 x 12 hücre (360, 768, 1024, 1440, 1920 açık/koyu, reduced-motion, 320 ve 640 px): 60/60 temiz, `hScroll` false, G12 kusuru 0, uyarı satırları metinli, konsol 0 | GEÇTİ |
| K16 | Adım 47'de yalnız klavye: 16 Tab + Enter + 2 Tab ile alan; Escape, kapalıyken Aşağı ok (ilk seçilebilir etkin), molada olanı atlayan sarma, Home/End, Tab seçimsiz kapatma (odak PIN), Enter seçim ve formu göndermeme; PIN ve düğmeye Tab, Enter: Saha Operatörü 07 eklendi | GEÇTİ |
| K17 | Kapalı ve açık öznitelikler, `aria-activedescendant` listedeki seçenekte, `aria-disabled` molada olanda, `role=status` `aria-describedby` içinde; erişilebilirlik ağacı tur 1 ile aynı | GEÇTİ |
| K18 | Boş sonuç, 500 ve bağlantı kesik hata metni, kişisiz gönderim engeli, "Bir kişi seçin." alanın hata yerinde (sol kenarlar 386 px eşit, odak alanda), kısaltma notu, geciktirilmiş eski yanıt atıldı | GEÇTİ |
| K19 | "Ahmet Yılmaz, sicil 2006" ve K-41 uyarısı, seçimle kişi hatası kalkıyor; "Değiştir" temizler ve odağı alana verir; yanlış PIN reddinde kişi ve uyarı korunuyor, PIN boş | GEÇTİ |
| K22 | Hazır, liste açık, seçilmiş + uyarı, kişisiz gönderim x 11 hücre: 44/44 temiz; liste alanla aynı sol ve genişlik, seçenek en az 67 px, "çıkar" özetleri 320 px'te sığıyor, konsol 0 | GEÇTİ |
| `FormValidation.js` başka formlar (768 px, klavyeyle boş gönderim) | `/users/create` (Süper admin): odak `RegistryNo`, 3 hata, etiket-alan-hata sol kenarı eşit; `RegistryNo` doldurulunca odak `FullName`. `/records/create` (Operatör 2003, "Kaydı oluştur"): odak `FacilityId`, 5 hata (tesis, hat, makine, kilit süresi, PIN). `/materials/create` (Yönetici): odak `Code`, 2 hata, sol kenarlar eşit. `/users/deactivate/11` (PIN'li işlem): odak `Pin`, "Bu alanı doldurun.". Konsol 0 | GEÇTİ |
| Mekanik eval | Tur 2 matrislerinde 104/104 hücre beklenen değerlerde | GEÇTİ |
| UI grep tablosu | Değişen beş dosyada (`_Timeline`, `_Participants`, `app.css`, `ParticipantPicker.js`, `FormValidation.js`) 0 ihlal; em/en-dash 0; emoji, `#region`, süs yorum 0; kaldırılan `data-people-error` için kalan başvuru 0 | GEÇTİ |
| Konsol ve günlük | Tüm tur 2 tarayıcı turlarında 0 hata ve uyarı; sunucu günlüğünde `fail`/`crit`/exception 0 | GEÇTİ |

Yeni bulgu: B7 (DÜŞÜK, frontend-engineer, Faz 6C öncesinden kalma `aria-describedby` yinelenmesi), bulgu tablosunda.

Güncel durum: KRİTİK 0, YÜKSEK 0, ORTA 0 açık; DÜŞÜK açık B7; B5 Faz 6E'ye devredildi. Kabul kriterleri 22/22.

```
Tur 2 QA önerisi: GEÇTİ
```

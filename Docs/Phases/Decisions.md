# Kararlar

> Sahibi: analyst. Her satır tarihli ve kimliklidir; ayrıntı ilgili K maddesinde (`Docs/Analysis/Kusur.md`) ya da PhasePlan'dadır.
> Durum değerleri: **Karar** (kullanıcı yanıtladı, ya da kararı açıkça analiste bıraktı ve analist belirledi); **Kabul (varsayılan)** (önerilen
> varsayılan kullanıcı tarafından toplu kabul edildi); **Varsayım** (analistin makul varsayılanı, kullanıcıya devir notuyla bildirildi; itiraz
> gelirse yeni satırla değişir). Sonraki değişiklikler aynı tabloya yeni tarihli satır olarak eklenir.

## Onaylar

| Tarih | Kimlik | Karar | Durum |
|---|---|---|---|
| 2026-11-13 | O-01 | Faz 0 planı onaylandı: kullanıcı K-38, K-39, K-40 ve PL-05'i yanıtladı, diğer varsayılanları kabul etti, "sonrasında bir şey kalmıyorsa devam" dedi. Yanıtlar bu dosyaya, Kusur, Brief, Requirements ve PhasePlan'a işlendi; ilk kod fazı Faz T. | Karar |
| 2026-11-13 | O-02 | Faz T TK8: kullanıcı tasarım dilini (`Docs/Design/Dna.md` kartı ve Faz T tur 2 kanıt görüntüleri, `Docs/Delivery/Evidence/Faz-T/`) onayladı. Faz 1 başlangıç şartı sağlandı. | Karar |
| 2026-11-14 | O-03 | Kullanıcı Salı (2026-11-17) takvimi için B'yi seçti (`Docs/Local/DokumanUyumu.md` §3, çekirdek önce); Ö-06, Ö-07, Ö-08 B ile onaylıdır. Kontrol noktaları: Pazar sonu Faz 2 ve 3, Pazartesi öğlen Faz 4 TESLİM ALINDI. Nokta kaçarsa C kullanıcıya o an sorulur; C şimdi onaylı değildir. | Karar |

## İş birimi kararları

| Tarih | Kimlik | Karar | Durum |
|---|---|---|---|
| 2026-11-13 | K-01 | Faz net süresi minimumun altındaysa son adım gerekçesiz bitirilemez; faz "Tamamlandı - süre altında". Acil tiplerde minimum yok. | Karar |
| 2026-11-13 | K-02 | Net = Brüt - açıkça başlatılıp bitirilen molalar; molada başlatan kişi yazma işlemi yapamaz; mola verilmemiş boşluk çalışma sayılır. | Karar |
| 2026-11-13 | K-03 | Kilit süresi tipten varsayılan gelir, operatör değiştirebilir; ilk adım kilit bitişine kadar başlamazsa kayıt Zaman aşımı. | Karar |
| 2026-11-13 | K-04 | Sahip değişmez; açık işi olan kullanıcı pasifleştirilemez, ekran açık işleri listeler. | Karar |
| 2026-11-13 | K-05 | İptal ve Yarıda kapatma ayrı, gerekçe zorunlu; operatör kapatmaz, yöneticiye talep açar; yönetici doğrudan da müdahale eder. | Karar |
| 2026-11-13 | K-06 | Katı sıra, atlama yok, kayıtta tek devam eden adım. | Karar |
| 2026-11-13 | K-07 | Görevli adım düzeyinde; yalnız sahip, adım başlamadan ekler ve çıkarır; efor = süre x görevli sayısı. | Karar |
| 2026-11-13 | K-08 | Kişi başına tek aktif kayıt ve tek devam eden adım (veritabanı kısıtı); başka işe geçmek için beklemeye alma talebi. | Karar |
| 2026-11-13 | K-09 | Sicil no + parola; ilk girişte parola değişimi ve PIN; kritik işlemler PIN ister (BR-48). | Karar |
| 2026-11-13 | K-10 | Yardımcı sistem kullanıcısıdır, katılımını aynı cihazda kendi PIN'iyle onaylar. | Karar |
| 2026-11-13 | K-11 | Malzeme kayıt düzeyinde, ilk adıma kadar eklenir; zorunlu tipte ilk adım malzemesiz başlamaz; lot listeden seçilir. Miktar yok, tipte izinli malzeme kısıtı yok (bu iki madde varsayılan olarak kabul edildi). | Karar |
| 2026-11-13 | K-12 | SKT geçmiş lot uyarı ve gerekçeyle kullanılır, işaretlenir. | Karar |
| 2026-11-13 | K-13 | Kayıt no `<TesisKodu>-<HatKodu>-<MakineKodu>-<Tür><Yıl>-<Sıra>`; saha referansı `<TesisKodu>-SD-<Yıl>-<Sıra>`, yalnız planlıda ilk adımda. | Karar |
| 2026-11-13 | K-14 | Acil müdahale ayrı, tek ekran; açılış ve ilk adım tek işlem; "Acil" türünde versiyonlu kısa tipler; yöneticilere uygulama içi bildirim. | Karar |
| 2026-11-13 | K-15 | Çevrimiçi zorunlu; istemcide işlem biriktirilmez. | Karar |
| 2026-11-13 | K-16 | Düzeltme yalnız Yönetici, gerekçeyle; hedefler lot, görevli listesi, iş emri, açıklama; zamanlar düzeltilmez. | Kabul (varsayılan) |
| 2026-11-13 | K-17 | Operatör tüm kayıtları salt okunur görür, liste varsayılanı "benim kayıtlarım" (PL-08 ile güncellendi). | Kabul (varsayılan) |
| 2026-11-13 | K-18 | İş emri serbest metin, planlıda zorunlu, acilde isteğe bağlı. | Kabul (varsayılan) |
| 2026-11-13 | K-19 | Tanımlar yönetici ekranından; ilk kurulum seed'i iş biriminin listesinden, gelene kadar nötr demo veri. | Kabul (varsayılan) |
| 2026-11-13 | K-20 | Kullanıcıya tesis ataması yok; seçim aktif tanımlar ve eşleme ile sınırlı. | Kabul (varsayılan) |
| 2026-11-13 | K-21 | Adım ayrıca duraklatılamaz; çalışmayı durdurmanın tek yolu kayıt molasıdır (K-39). Başladığı vardiya bitmiş açık adım ve mola yönetici panosunda (SH-16). | Kabul (varsayılan) |
| 2026-11-13 | K-22 | Mevzuat uyumu, e-imza ve dört göz onayı hedeflenmez; PIN K-09 ile kapsamda. | Kabul (varsayılan) |
| 2026-11-13 | K-23 | HMAC anahtarlı kayıt başına özet zinciri; anahtar ortam değişkeninde; dış çapa yok. | Kabul (varsayılan) |
| 2026-11-13 | K-24 | Yayın anında yürürlük, tek onay, versiyon no sistemce artar; isteğe bağlı doküman no. | Kabul (varsayılan) |
| 2026-11-13 | K-25 | Aktif ya da beklemedeki kaydı olan makine ve tip kaldırılamaz, ekran engelleyen kaydı gösterir. | Kabul (varsayılan) |
| 2026-11-13 | K-26 | Medyayı Yönetici yükler; JPEG, PNG, WebP, MP4; azami boyut `<AzamiMedyaBoyutu>` sistem ayarından (K-47). | Kabul (varsayılan) |
| 2026-11-13 | K-27 | Adımda yalnız başlat ve bitir; sonuç, ölçüm, kanıt fotoğrafı yok. | Kabul (varsayılan) |
| 2026-11-13 | K-28 | Kayıtlar süresiz saklanır; denetim çıktısı yazdırılabilir HTML ve CSV. | Kabul (varsayılan) |
| 2026-11-13 | K-29 | Çoklu rol; sahada çalışan yöneticiye Operatör rolü de verilir; yalnız Yönetici rolüyle kayıt açılmaz. | Kabul (varsayılan) |
| 2026-11-13 | K-30 | Sahiplik kontrolü servis katmanında `UserId` ile yapılan iş kuralıdır, yetki politikası değildir; `CodingStandards.md §4`'e yazıldı. | Karar |
| 2026-11-13 | K-31 | Yalnız Türkçe. | Kabul (varsayılan) |
| 2026-11-13 | K-32 | UTC saklama, Türkiye saatiyle gösterim; sunucu saat eşitlemesi BT'de. | Kabul (varsayılan) |
| 2026-11-13 | K-33 | SQLite, WAL, kısa transaction; ölçek sayıları gelince ölçüm. | Kabul (varsayılan) |
| 2026-11-13 | K-34 | Periyodik temizlik takvimi kapsam dışı. | Kabul (varsayılan) |
| 2026-11-13 | K-35 | "Kaydı açan (sorumlu)" ve "İlk adımı başlatan" ayrı gösterilir. | Kabul (varsayılan) |
| 2026-11-13 | K-36 | Rapor makine ve faz bazında; tarih aralığı; personel bazlı rapor yok. | Kabul (varsayılan) |
| 2026-11-13 | K-37 | Kanal sorusu K-43'e taşındı; panoda müdahale bekleyenler listesi. | Kabul (varsayılan) |
| 2026-11-13 | K-38 | Kullanıcı kararı analiste bıraktı. Vardiya tesis bazında, [başlangıç, bitiş) aralığı, gece yarısını geçebilir; kilit bitişi = en erken (açılış + kilit süresi, açılışı içeren vardiyanın bitişi); açılışta hesaplanıp saklanır, vardiya sonradan değişse de değişmez. | Karar |
| 2026-11-13 | K-39 | Kullanıcı: "temizlik sürerken mola verebilmeli". Mola kayıt düzeyinde, iki adım arasında ya da adım sürerken; açıkken kayıtta çalışma durur, adım bitirilemez; adımla kesişen mola adım süresinden ve eforundan düşülür (SH-17, SH-01). | Karar |
| 2026-11-13 | K-40 | Tüm yöneticiler tüm talepleri görür ve karara bağlar; ilk karar geçerli. Süper admin talep görmez. | Karar |
| 2026-11-13 | K-41 | Kendi kaydı devam ederken başka kayıtta görevli olmak serbest, ekran uyarır. | Kabul (varsayılan) |
| 2026-11-13 | K-42 | PIN yalnız rakam, `<PinUzunluğu>` hane, aynı ya da ardışık haneler reddedilir; `<PinDenemeSınırı>` hatada kilit; hesabı yönetebilen kullanıcı sıfırlar (PL-05). | Kabul (varsayılan) |
| 2026-11-13 | K-43 | Bildirim yalnız uygulama içi. | Kabul (varsayılan) |
| 2026-11-13 | K-44 | Makine kilidi ve tek aktif kayıt kuralı acilde de aynen uygulanır. | Kabul (varsayılan) |
| 2026-11-13 | K-45 | Beklemede makine kilitli, tek aktif kayda sayılmaz, bekleme net süreden düşülür, sahip PIN'le devam ettirir. | Kabul (varsayılan) |
| 2026-11-13 | K-46 | Kilit süresi `<AzamiKilitSüresi>`'ni aşamaz; yalnız ilk adım kilidi kalıcı yapar. | Kabul (varsayılan) |
| 2026-11-13 | K-47 | Kullanıcı: ayarlar "yönetici ayarlarından değiştirilebilir olmalı" (Kusur alternatifi). Sistem geneli değerler veritabanında; Yönetici ve Süper admin Sistem ayarları ekranından PIN'le değiştirir, değişiklik geçmişi tutulur. İlk değerler yer tutucu, uydurma değer yok. | Karar |
| 2026-11-13 | K-48 | `Auditor` rolü vardır, okuma ekranlarını salt okunur görür. | Kabul (varsayılan) |

## Plan kararları

| Tarih | Kimlik | Karar | Durum |
|---|---|---|---|
| 2026-11-13 | PL-01 | Faz yapısı 0, T, 1-9; gerekçe PhasePlan "Faz sayısı neden arttı". | Kabul (varsayılan) |
| 2026-11-13 | PL-02 | Bir tablonun sonraki fazlarda gereken kolonları tablo ilk oluşturulurken eklenir; amaç ALTER gerektirmemek. | Kabul (varsayılan) |
| 2026-11-13 | PL-03 | Yalnız Taslak versiyonun faz, adım ve medya bağlantıları düzenlenirken çıkarılabilir; yayınlanmış versiyon ve kayıtlar silinmez; tip başına tek taslak. | Kabul (varsayılan) |
| 2026-11-13 | PL-04 | Tesis, hat, makine kodu ve makinenin hattı, temizlik tipinin türü oluşturulduktan sonra değişmez (kayıt numarasının ve akışın parçası). | Kabul (varsayılan) |
| 2026-11-13 | PL-05 | İlk hali (son aktif Yönetici kuralı, seed'de iki Yönetici) kullanıcı yanıtıyla değiştirildi. Yeni rol `Roles.SuperAdmin`: Yönetici ve Süper admin hesaplarını açar, düzenler, pasifleştirir, parola ve PIN sıfırlar; Yönetici yalnız Operatör ve Denetçi hesaplarını yönetir. Seed tek Süper admin içerir; son aktif Süper admin pasifleştirilemez, rolü kaldırılamaz. Süper admin saha, tanım, talep, müdahale, rapor ve kayıt ekranlarına erişmez (SK-9). | Karar |
| 2026-11-13 | PL-06 | Parola sıfırlaması da PIN ister; BR-48 listesine eklendi. | Kabul (varsayılan) |
| 2026-11-13 | PL-07 | SH-16 eklendi: başladığı vardiya bitmiş açık adım ve mola için karar formülü (Brief §9.5). | Kabul (varsayılan) |
| 2026-11-13 | PL-08 | K-17 varsayılanındaki "kendi tesisi" K-20 varsayılanıyla (tesis ataması yok) tanımsız kaldığı için "tüm kayıtlar, varsayılan filtre benim kayıtlarım" olarak düzeltildi. | Kabul (varsayılan) |
| 2026-11-13 | PL-09 | Görevli düzeltmesinde orijinal efor korunur, düzeltilmiş görevli sayısıyla efor ayrıca gösterilir; raporlar düzeltilmiş değeri kullanır. | Kabul (varsayılan) |
| 2026-11-13 | PL-10 | Prototip sayfaları yalnız Development ortamında açılır ve en geç Faz 6 teslimiyle kaldırılır. | Kabul (varsayılan) |
| 2026-11-13 | PL-11 | Bütünlük doğrulaması iki kontrol yapar: özet zinciri ve durum tablolarının olay değerleriyle karşılaştırılması (SC-21 adım zamanı değişikliğini de yakalamak için). | Kabul (varsayılan) |
| 2026-11-13 | PL-12 | Yürütme ekranı yalnız Operatör'e açıktır; Yönetici ve Denetçi kayıt detayını kullanır. | Kabul (varsayılan) |
| 2026-11-13 | PL-13 | Düzeltme Faz 7 yerine Faz 8'de; Faz 7'nin ekran ve kural yükünü dengelemek için. | Kabul (varsayılan) |
| 2026-11-13 | PL-14 | Planlı tipte versiyon, her fazın minimum süresi tanımlı değilse yayınlanamaz (FR-05); acil tipte minimum alanı yoktur. | Kabul (varsayılan) |
| 2026-11-13 | PL-15 | Birleşik rol sabitleri: `ManagerOrAuditor`, `ManagerOrSuperAdmin`, `RecordReaders` (Operator, Manager, Auditor), `Any` (dört rol). Kayıt, makine panosu ve medya okuma ekranları `RecordReaders` ile korunur; Süper admin bu ekranlarda 403 alır. | Karar |
| 2026-11-13 | PL-16 | Kimse kendi hesabını pasifleştiremez ve kendi rollerini değiştiremez; açık iş listesinde kayıt bağlantısı yalnız görüntüleyenin erişebildiği ekrana verilir. | Karar |
| 2026-11-13 | PL-17 | Sistem ayarları tablosu Faz 1'de oluşur ve seed'den dolar; Sistem ayarları ekranı (E-33) Faz 9'a eklendi (Faz 1-8'de ekran sınırı dolu). Faz 9'a kadar değer değişikliği seed ya da QA kopyasında yapılır. | Karar |
| 2026-11-13 | PL-18 | Ayar değişikliğinin etkisi: oturum süresi ve PIN deneme sınırı sonraki istekte; PIN uzunluğu yalnız yeni belirlenen PIN'de (mevcut PIN'ler geçerli, PIN doğrulama alanı uzunluk sınırı koymaz); medya boyutu yeni yüklemede; azami kilit süresi aktif planlı tiplerin varsayılan kilit süresinin altına inemez ve açık kayıtların kilit bitişini değiştirmez. | Karar |
| 2026-11-13 | PL-19 | Adım molası SH-17 eklendi; adım tablosunda adım mola süresi kolonu Faz 4'te önden gelir (PL-02), mola tablosu molanın başladığı anda devam eden adımı tutar (Faz 5). | Karar |
| 2026-11-13 | PL-20 | Faz T B5: TK6 geçerli, DNA kartı güncellenir. lg altında "Kullanıcı değiştir" çekmece açılmadan görünür, görünür metinli, 44 px, tek dokunuş (Faz 1 K11). Neden: paylaşılan tablette sıradaki kişi önceki oturumda kalırsa işlemleri ona yazılır ve girdiği kendi PIN'i önceki kişinin hatalı deneme sayacını doldurup PIN'ini kilitler (NFR-09, K-42); çekmece adımı bu riski büyütür. | Karar |
| 2026-11-13 | PL-21 | Faz T tur 1 İADE. Ertelenen DÜŞÜK: B14 süre biçimi (rakam ve birim ayrımı) Faz 4 biçimleyici sözleşmesine, B15 antiforgery Faz 1 G5'e, B16 Production hata sayfası kabuğu Faz 1 K14'e. | Karar |
| 2026-11-13 | PL-22 | Faz T tur 2'de B18 YÜKSEK ve B19 ORTA kaldı; `Workflow.md §3` gereği kullanıcıya soruldu, kullanıcı B18, B19 ve gerileme ile sınırlı 3. turu onayladı. İki tur sınırının tek seferlik istisnasıdır, kural değişmez. | Karar |
| 2026-11-13 | PL-23 | Faz T tur 3 TESLİM ALINDI. Yeni DÜŞÜK'ler ertelendi: B21 (`Dna.md` veri dili sütun ve satır ayrıntısı) Faz 1, B22 (yürütme başlık ızgarası) Faz 4, B20 (mola paneli düğmesi) Faz 5; sahibi frontend-engineer. Commit şartı: `Dna.md:21` durum satırı onaylı yazılır. | Karar |
| 2026-11-13 | PL-24 | Kullanıcı Faz 1 ayar başlangıç değerlerini verdi: `<OturumSüresi>` 30 dakika (son istekten sonraki işlemsizlik), `<PinUzunluğu>` 4 hane, `<PinDenemeSınırı>` 5 (art arda 5. hatalı denemede PIN kilitlenir). Değerler seed'e girer, Faz 9'da E-33'ten değişir (K-47, PL-17, PL-18). `<AzamiMedyaBoyutu>` ve `<AzamiKilitSüresi>` açık, Faz 3 öncesi gerekir. | Karar |
| 2026-11-14 | PL-25 | Ayar tablosu ayar başına bir satırdır. Faz 1 seed'i yalnız değeri verilmiş üç satırı yazar; fotoğraf boyutu, video boyutu ve azami kilit süresi satırları değerler gelince Faz 3'te seed ekleme olarak gelir. Neden: boş ya da yer tutucu değer yazmamak ve sonradan `UPDATE` (DbProtocol ALTER sınıfı) gerektirmemek. | Karar |
| 2026-11-14 | PL-26 | Faz 1 sözleşmesi paralel başlangıç için tamamlandı: alt ViewModel adları ve form alanı önekleri, rol etiketi `Roles.Label`, claim sabitleri `VakaClaimTypes`, etkinleştirmeye PIN (matris ile hizalandı), `/account/access-denied` 403 durum kodu, "Kullanıcı değiştir" = `POST /account/logout`, menüde yalnız var olan adresler, dosya sınırına `Views/Error/**`, `Views/Prototype/**`, `Dna.md`, `PrototypeController.cs`, `ErrorController.cs`. | Karar |
| 2026-11-14 | PL-27 | Sicil no hesap oluşturulduktan sonra değişmez (giriş kimliği, kayıtlarda ve olay günlüğünde kişiyi tanıtır). Yanlış sicil noyla açılan hesap pasifleştirilir, doğru sicil noyla yeni hesap açılır. | Varsayım |
| 2026-11-14 | PL-28 | Faz 1 tur 1 İADE (B1 YÜKSEK, B2 ORTA). B1: uygulama `Dna.md` veri dili satırına uyar, kart satırı değişmez (onaylı kimlik, O-02). Satır kartında tam satırı yalnız tek uzun metin alanı kaplar; 768-1279 px arasında en çok iki sütun. Kural sonraki fazların tablolarında da geçerli. | Karar |
| 2026-11-14 | PL-29 | B2: son aktif Süper admin kuralı pasifleştirme ve rol kaldırmada tek yerde, başında yazma kilidi alınan tek işlemde denetlenir ve yazılır; sayım kilitten sonra yeniden yapılır. Şema değişikliği beklenmez; çıkarsa ALTER sınıfıdır, kullanıcı onayı ister. | Karar |
| 2026-11-14 | PL-30 | B3, B4, B6 bu turda kapanır: kimliğe `NameIdentifier`, EF komut günlüğü `Warning`, çevrimdışıyken "Kullanıcı değiştir" de devre dışı ve şerit nedenini söyler (K-15, NFR-14). B5 şema değişikliksiz kapanır: CHECK `Roles` sabitlerinden kurulur, üretilen SQL aynı, migration yok; model farkı çıkarsa Faz 2'ye ertelenir. Üretilmiş migration metni istisnadır. | Karar |
| 2026-11-14 | PL-31 | Faz 1 tur 2'de B1-B6 kapandı, B7 ORTA (768 px altı kart yerleşiminde boş yarım satırlar) kaldı; `Workflow.md §3` gereği kullanıcıya soruldu, kullanıcı B7 ve gerileme ile sınırlı 3. turu onayladı. PL-22'den sonra iki tur sınırının ikinci tek seferlik istisnasıdır, kural değişmez. | Karar |
| 2026-11-14 | PL-32 | Faz 1 tur 3 TESLİM ALINDI; Faz T'den devreden T-B15, T-B16, T-B21 kapandı. Faz 4'e devreden: pasifleştirme ekranının açık iş durumunun çalışan uygulamada ölçülmesi (qa-tester) ve BR-48 PIN doğrulamasıyla iş eyleminin aynı işlemde yapılması (backend-engineer). | Karar |
| 2026-11-14 | PL-33 | Faz T ve Faz 1 küçük ekran yerleşim bulgusu yüzünden üçüncü tura gitti. PhasePlan'a G11 yerleşim imzası ve "UI fazlarında yerleşim" kuralları eklendi: frontend ölçmediği yerleşimi "DNA ile aynı" diye yazmaz, beklenen imzayı yazar; iade maddesi ve yeniden denetim şartı aynı imzayla yazılır. Workflow ön tur önerisi kullanıcı kararını bekler. | Karar |
| 2026-11-14 | PL-34 | Kullanıcı `<AzamiKilitSüresi>` başlangıç değerini 120 dakika verdi; satır PL-25 gereği Faz 3 seed eklemesiyle gelir. `<AzamiMedyaBoyutu>` (fotoğraf, video) için kullanıcı "Faz 3'e kadar beklesin" dedi; açık kalır, Faz 3 başlamadan gerekir (Requirements §9). | Karar |
| 2026-11-14 | PL-35 | Kullanıcı PL-33'teki yerleşim ön turu önerisini onayladı: UI fazında geliştirmeden sonra qa-tester yalnız G11 imzası ve `UiDelivery.md §3` mekanik eval ile ön tur yapar (`Workflow.md §3` adım 2a); rapor yazılmaz, faz turu sayılmaz, bulgu temizlenmeden teslim testine geçilmez. Faz 2'den başlayarak geçerlidir. | Karar |
| 2026-11-14 | PL-36 | Faz 2 backend sözleşme ekleri kabul: liste ve ayrıntı ViewModel'lerine `StatusMessage?` (Faz 1 yönlendirme düzeni); form saatleri `TimeOnly?`, SKT `DateOnly?`, listede boş olmayan tip; `SelectOptionViewModel { Id, Label }` ("Kod - Ad") ve `FacilityOptionViewModel { Id, Name }`. Vardiya saati ve SKT tanım değeridir, olay zamanı değildir; NFR-01 değişmez. | Karar |
| 2026-11-14 | PL-37 | PL-04 düzenlemede gönderilmeyen, boş ya da 0 kod ve hat "değişmedi" sayılır; dolu ve farklı değer alan hatasıyla reddedilir. Makine düzenlemede tesis hattan türer, farklı `FacilityId` `LineId` hatasıyla reddedilir. Makine kullanımdan kaldırma PIN istemez: BR-48 tanım işlemlerini "PIN istenmeyenler"e koyar, onaylı rol matrisinde bu satırda PIN yok; PIN eklemek kullanıcı kararıdır. | Karar |
| 2026-11-14 | PL-38 | `/facilities` ve `/shifts` sayfalanmaz; aktiflik filtresi ve tesis seçimi sunucuda çalışır. Neden: tesis sayısı ve tesis başına vardiya sayısı küçük kümedir, vardiya listesi tek tesise bağlıdır. Kayıt sayısı sorun olursa yeni kararla eklenir. Faz 2 K8 buna göre yazıldı. | Varsayım |
| 2026-11-14 | PL-39 | G11 istisnası: ana kolon ve bağlam rayı yan yana duran ekranlardaki gömülü tablolar (ilk örnek `/facilities/details/{id}` hatları, `/materials/details/{id}` lotları) 1024-1279 px'te 768 px altı kart kuralını (2 sütun) izler. Neden: ray yanındaki ana kolon 4 sütunlu kartı okunmaz hücrelere böler. Liste ekranlarında 768-1279 px 4 sütun kuralı değişmez. | Karar |
| 2026-11-14 | PL-40 | `FindShiftAt` (saf hesap) Faz 2'de `IDurationCalculator`'da kalır (kapsam 3). Veritabanından okuyan `IShiftLocator` çağıranıyla birlikte Faz 4 kalem 5'e taşındı; Faz 2'den arayüz dosyası çıkarılır, kayıt yapılmaz. Neden: çağıransız servis kullanılmayan koddur, imzası SH-13 ihtiyacıyla belirlenir. | Karar |
| 2026-11-14 | PL-41 | `MaterialLotsController` için ayrı rota gerekmez: `RouteTokenTransformerConvention` `[controller]` jetonunu `material-lots` yapar (Faz 1 `first-login` aynı yolla çalışıyor); QA adresi çalışan uygulamada doğrular. K3 JS kapalı: makine ekleme formuna tesis yenileme düğmesi (frontend) ve hat-tesis uyuşmazlığında `LineId` hatasıyla gönderilen tesisin hatlarıyla yeniden çizim (backend). | Karar |
| 2026-11-14 | PL-42 | Faz 2 tur 1 İADE (B1, B2 YÜKSEK; B3, B4 ORTA). B1 Faz 1 kodunda (`ErrorController`, commit `38d0b4e`): hata eyleminin kodu yalnız rotadan bağlanır, 400-599 dışı kod 404 döner; Faz 1 POST'ları Faz 2 tur 2'de yeniden denetlenir, Faz 1 raporu değişmez. `ErrorController.cs` bu tur için backend dosya sınırına girer. | Karar |
| 2026-11-14 | PL-43 | B5: kullanımdan kaldırılmış makine salt okunurdur; ad dahil düzenleme reddedilir, geri alma yoktur. Neden: SC-12 geçmiş kayıtların ve raporların makineyi kaldırıldığı andaki adı ve koduyla göstermesini ister. `MachineFormViewModel` ve `MachineRetireViewModel`'e `RetiredAtLocal?` eklendi. | Karar |
| 2026-11-14 | PL-44 | B10: tesis seçilmemiş `/machines` süzgecinde hat etiketi `<TesisKodu>-<HatKodu> - <HatAdı>`; birleşik kod kayıt no önekiyle (K-13) aynıdır, aynı hat kodlu tesisleri ayırır. B7: JS kapalı tesis yenileme `GET` yerine aynı formda `refresh=true` POST'u; neden: GET antiforgery belirtecini adrese ve günlüğe taşıyor. | Karar |
| 2026-11-14 | PL-45 | Faz 2 DÜŞÜK bulguları (B5-B10) ertelenmez, tur 2'de kapanır: hepsi Faz 2 dosyalarında, sonraki fazlar bu ekranlara dönmüyor. B2 yönü: `overflow-wrap: anywhere` yalnız kart modunda ve uzun veri hücrelerinde; `Styles/app.css` frontend dosya sınırındadır. | Karar |
| 2026-11-14 | PL-46 | Ö-08 faz yeniden yapılanması (O-03): Faz 6 = E-19, E-20 (yazdırılabilir, Ö-07), E-21, E-26 (iptal, yarıda kapatma, yöneticinin molayı bitirmesi), `/records` aramasında lot no (Ö-06), `/` Yönetici ve Denetçi'de `/records`. Faz 7 = talep, beklemeye alma, E-22 pano ve SH-16. Faz 7'nin müdahale kriterleri Faz 6'ya, Faz 6'nın pano kriterleri Faz 7'ye taşındı; kapanış türü, kapatan ve gerekçe kolonları Faz 4 kayıt tablosunda önden gelir (PL-02). | Karar |
| 2026-11-14 | PL-47 | Kullanıcı `<AzamiMedyaBoyutu>` değerlerini verdi: fotoğraf 10 MB, video 100 MB. Faz 3 seed eklemesinde `<AzamiKilitSüresi>` 120 ile birlikte gelir (PL-25, PL-34); Requirements §9 engeli kalktı. | Karar |
| 2026-11-14 | PL-48 | Olay özeti anahtarı (K-23): Salı demosu için geliştirme anahtarı user-secrets'ta, repoya girmez; üretim anahtarı BT'den ortam değişkeniyle, aynı yapılandırma anahtarıyla ve kod değişmeden gelir. Geliştirme anahtarıyla yazılmış zincir üretim anahtarıyla doğrulanmaz; demo veritabanı üretime taşınmaz. | Karar |
| 2026-11-14 | PL-49 | Ö-01..Ö-05 kabul ekleri ve Ö-09 şimdi uygulanmaz, Salı sonrasına kalır (kullanıcı: "dökümanda istenilenleri karşılayacak projeyi yapalım ekleri sonra yaparız"). Acil müdahale Salı için şart değil; B uzatma sırası geçerli. Talep, beklemeye alma, pano, acil, bildirim, düzeltme, raporlar ve ayar ekranı Salı sonrası; K-05, K-08, K-47 karar olarak kalır, yalnız sırası değişir. | Karar |
| 2026-11-14 | PL-50 | Faz 3 sözleşmesi paralel başlangıç için tamamlandı: enum'lar `Services/` altında (`SettingKeys` düzeni), alt ViewModel adları, görünür adlar `...Label` alanlarından, düzenleyici hatası `Error.Target` ile, `CreateDraft` ve `Publish` tam sayfa form, controller öznitelik birleşimi, medya imza denetimi ve ayardan türetilen istek gövdesi sınırı, ayar anahtarları `MaxLockMinutes`, `MaxPhotoMb`, `MaxVideoMb`. Başlama şartı Faz 2 teslimi ve commit'i (CLAUDE.md kural 7). | Karar |
| 2026-11-14 | PL-51 | Faz 3 ayrıntıları: 1 MB = 1 048 576 bayt, sınıra eşit dosya kabul; demo versiyonlarda yayınlayan boş, ekranda "Kurulum verisi"; tip kodu kayıt numarasına girmediği için değiştirilebilir; Denetçi taslak versiyonu göremez (404). | Varsayım |
| 2026-11-14 | PL-52 | Faz 2 tur 2 TESLİM ALINDI (18/18, B1-B10 kapandı). B2'nin ön turda değişen çözümü (1280 px ve üstünde ray yanı tabloda yalnız `td.cell-long` kırılır, diğer hücreler `nowrap`) tur 1 şartını karşılar. `Machines/Form.cshtml:20` boş `data-unsaved-guard` etkisizdir, bulgu sayılmaz. Faz 4'e devreden: F2-A1, F2-A2 (qa-tester). | Karar |

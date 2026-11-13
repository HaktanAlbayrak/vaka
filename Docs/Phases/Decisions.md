# Kararlar

> Sahibi: analyst. Her satır tarihli ve kimliklidir; ayrıntı ilgili K maddesinde (`Docs/Analysis/Kusur.md`) ya da PhasePlan'dadır.
> Durum değerleri: **Karar** (kullanıcı yanıtladı, ya da kararı açıkça analiste bıraktı ve analist belirledi); **Kabul (varsayılan)** (önerilen
> varsayılan kullanıcı tarafından toplu kabul edildi). Sonraki değişiklikler aynı tabloya yeni tarihli satır olarak eklenir.

## Onaylar

| Tarih | Kimlik | Karar | Durum |
|---|---|---|---|
| 2026-11-13 | O-01 | Faz 0 planı onaylandı: kullanıcı K-38, K-39, K-40 ve PL-05'i yanıtladı, diğer varsayılanları kabul etti, "sonrasında bir şey kalmıyorsa devam" dedi. Yanıtlar bu dosyaya, Kusur, Brief, Requirements ve PhasePlan'a işlendi; ilk kod fazı Faz T. | Karar |
| 2026-11-13 | O-02 | Faz T TK8: kullanıcı tasarım dilini (`Docs/Design/Dna.md` kartı ve Faz T tur 2 kanıt görüntüleri, `Docs/Delivery/Evidence/Faz-T/`) onayladı. Faz 1 başlangıç şartı sağlandı. | Karar |

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

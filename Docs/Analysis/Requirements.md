# Gereksinimler

> Sahibi: analyst. Durum: **ONAYLANDI** (Faz 0, 2026-11-13, `Decisions.md` O-01).
> İster, iş kuralı, senaryo ve formül kimlikleri (FR, NFR, BR, SC, SH, SK) `Docs/Analysis/Brief.md`'dedir; açık konular (K) `Docs/Analysis/Kusur.md`'de,
> kararlar `Docs/Phases/Decisions.md`'de, fazlar ve sözleşmeler `Docs/Phases/PhasePlan.md`'de. Bu belge kimliklere atıf yapar, metinlerini tekrar etmez.
> Bu belgenin yetkili olduğu konular: rol ve yetki matrisi (§3), ekran envanteri (§4), veri varlıkları listesi (§5), ayar değerleri (§6).

## 1. Amaç

Üretim tesislerinde makine temizliğinin kağıt form yerine, işlem anında sunucu saatiyle oluşan, versiyonlu prosedüre bağlı ve sonradan
değiştirilemeyen dijital kaydını tutmak. Kayıt denetimde kanıt olarak sunulur ve makine, faz bazında süre ile eforu ölçülebilir kılar.
Problem, hedef ve başarı ölçütleri: Brief §1. Kavramlar: Brief §2.

## 2. Roller

| Rol | Sabit (`Authorization/Roles.cs`) | Kim | Kaynak |
|---|---|---|---|
| Operatör | `Roles.Operator` | Sahada temizliği yapan personel | Brief §3.1 |
| Yönetici | `Roles.Manager` | Tanımları, Operatör ve Denetçi hesaplarını, talepleri ve sistem ayarlarını yöneten, kayda müdahale eden kişi | Brief §3.1 |
| Denetçi | `Roles.Auditor` | Kayıtları, raporları ve bütünlüğü salt okunur inceleyen kişi (K-48) | Brief §3.1 |
| Süper admin | `Roles.SuperAdmin` | Yönetici ve Süper admin hesaplarını açan, pasifleştiren, parola ve PIN kilidini sıfırlayan; sistem ayarlarını değiştiren kişi (PL-05) | Brief §3.1 |

Birleşik sabitler (PL-15), yalnız tekil sabitlerden kurulur:

| Sabit | Roller | Kullanıldığı yer |
|---|---|---|
| `Roles.ManagerOrAuditor` | Manager, Auditor | Pano, bütünlük, talepler listesi, versiyon görüntüleme, raporlar |
| `Roles.ManagerOrSuperAdmin` | Manager, SuperAdmin | Kullanıcılar, Sistem ayarları |
| `Roles.RecordReaders` | Operator, Manager, Auditor | Makine panosu, kayıt listesi ve detayı, medya |
| `Roles.Any` | dört rol | Ana sayfa, kendi hesap ekranları, çıkış |

- Bir kullanıcı birden çok role sahip olabilir; yetkiler birleşir. Yalnız `Manager` rolüyle kayıt açılamaz ve adım yürütülemez (K-29).
  Süper admin saha ya da yönetim işi yapacaksa kişiye ayrıca ilgili rol verilir.
- Rol kontrolü yalnız `[Authorize(Roles = Roles.X)]` ile yapılır. Sahiplik, görevlilik, mola kilidi, PIN ve hesap yönetimi kapsamı
  (Brief §3.3, SK-1..SK-9) servis katmanında iş kuralıdır; K-30 ile onaylandı, `CodingStandards.md §4`. Policy, claim ya da izin tablosu yoktur.

## 3. Yetki matrisi (rol x ekran x işlem)

Gösterim: **E** yapar; **S** salt okunur görür; **-** erişemez (403, menüde yok); **PIN** işlemi yapanın PIN'i istenir (BR-48).
"İş kuralı" sütunundaki koşul rol yetkisine ek olarak serviste kontrol edilir; koşulu sağlamayan istek işlem yapmaz.

| Ekran / işlem | Operatör | Yönetici | Denetçi | Süper admin | İş kuralı |
|---|---|---|---|---|---|
| Giriş, çıkış, kullanıcı değiştir | E | E | E | E | FR-46 |
| İlk giriş: parola değiştirme, PIN belirleme | E | E | E | E | BR-55 |
| Kendi parolasını ve PIN'ini değiştirme | E | E | E | E | K-42: yeni PIN eski PIN'le |
| Kullanıcı listesi | - | S | - | S | Düzenleme bağlantısı yalnız yönetebildiği hesapta (SK-9) |
| Kullanıcı ekleme, düzenleme, rol verme | - | E | - | E | SK-9: Yönetici yalnız Operatör ve Denetçi hesabı ve rolü; kendi rolünü kimse değiştiremez |
| Kullanıcı pasifleştirme ve yeniden etkinleştirme | - | E, PIN | - | E, PIN | BR-36 açık iş; SK-9 kapsam; kendini ve son aktif Süper admin'i pasifleştirme yok |
| PIN sıfırlama, parola sıfırlama | - | E, PIN | - | E, PIN | BR-49; SK-9 kapsam |
| Sistem ayarları | - | E, PIN | - | E, PIN | FR-55, PL-18 |
| Tesis, hat, makine, vardiya tanımları | - | E | - | - | FR-01, FR-49, BR-54 |
| Makineyi kullanımdan kaldırma | - | E | - | - | BR-35 |
| Malzeme ve lot tanımları | - | E | - | - | FR-08 |
| Temizlik tipi ve makine eşleme | - | E | - | - | FR-03 |
| Prosedür taslağı düzenleme, medya yükleme | - | E | - | - | FR-04..FR-07 |
| Prosedür versiyonu yayınlama | - | E, PIN | - | - | BR-04 |
| Prosedür versiyonu görüntüleme | - | S | S | - | FR-09 |
| Adım medyasını açma | E | E | E | - | FR-06 |
| Makine durum panosu | S | S | S | - | FR-41 |
| İşlerim | E | - | - | - | Brief SC-03 |
| Yeni kayıt açma | E, PIN | - | - | - | BR-01, BR-02, BR-23, BR-37 |
| Malzeme ekleme ve çıkarma (ilk adımdan önce) | E, PIN | - | - | - | Yalnız sahip (BR-47) |
| Yürütme ekranını görme | S | - | - | - | K-17, PL-12 |
| Görevli ekleme ve çıkarma | E, PIN | - | - | - | Yalnız sahip, adım başlamadan (SK-2) |
| Katılımı onaylama | E, PIN | - | - | - | Eklenen kişinin kendi PIN'i (SK-4) |
| Adım başlatma ve bitirme | E, PIN | - | - | - | Sahip ya da adımın görevlisi (SK-1); kayıtta açık mola yokken (BR-39) |
| Mola başlatma | E, PIN | - | - | - | Sahip ya da o anki adımın görevlisi; iki adım arasında ya da adım sürerken; başlatan başka kayıtta devam eden adımda değil (SK-6, BR-38) |
| Molayı bitirme | E, PIN | E, PIN, gerekçe | - | - | Operatörde yalnız başlatan (SK-6) |
| Kayıt listesi | S | S | S | - | Operatörde varsayılan filtre "benim kayıtlarım" (K-17) |
| Kayıt detayı (13 soru, olay günlüğü, bütünlük rozeti) | S | S | S | - | FR-31 |
| Bütünlük doğrulama ekranı | - | S | S | - | FR-32 |
| Yönetici panosu | - | S | S | - | İÖ-07 |
| Talep açma ve geri çekme | E, PIN | - | - | - | Talebi yalnız sahip açar, yalnız açan geri çeker (SK-5) |
| Taleplerim | E | - | - | - | FR-43 |
| Talepler listesi | - | E | S | - | FR-44 |
| Talep onayı ve reddi | - | E, PIN | - | - | BR-42, K-40 |
| Doğrudan müdahale: iptal, yarıda kapatma, beklemeye alma | - | E, PIN, gerekçe | - | - | BR-27, BR-43 |
| Beklemedeki kaydı devam ettirme | E, PIN | - | - | - | Yalnız sahip (SK-7) |
| Acil müdahale açma | E, PIN | - | - | - | BR-50 |
| Bildirimler | - | E | - | - | BR-51 |
| Düzeltme | - | E, PIN, gerekçe | - | - | BR-29 |
| Süre ve efor raporu, malzeme izlenebilirliği | - | S | S | - | FR-36, FR-37, BR-34 |
| Denetim çıktısı ve CSV | - | S | S | - | FR-35 |

## 4. Ekran envanteri

Route'lar taslaktır; kesin liste ve ViewModel alanları PhasePlan sözleşmelerindedir. URL küçük harf ve kebab (`CodingStandards.md §1`).

| Kimlik | Ekran | Route taslağı | Rol | İster | Faz |
|---|---|---|---|---|---|
| E-01 | Giriş | `/account/login` | anonim | FR-46 | 1 |
| E-02 | Parola ve PIN (ilk giriş zorunlu modu, Hesabım modu) | `/account/first-login`, `/account/security` | Any | FR-46, BR-49, BR-55 | 1 |
| E-03 | Kullanıcılar | `/users` | ManagerOrSuperAdmin | FR-39, FR-54 | 1 |
| E-04 | Kullanıcı formu: ekle, düzenle, pasifleştir, PIN ve parola sıfırla | `/users/create`, `/users/edit/{id}`, `/users/deactivate/{id}` | ManagerOrSuperAdmin | FR-39, FR-54, BR-36 | 1 |
| E-05 | Tesisler ve hatlar | `/facilities`, `/facilities/details/{id}`, `/lines/edit/{id}` | Manager | FR-01, BR-54 | 2 |
| E-06 | Vardiyalar | `/shifts` | Manager | FR-49 | 2 |
| E-07 | Makineler | `/machines`, `/machines/retire/{id}` | Manager | FR-01, FR-02 | 2 |
| E-08 | Malzemeler ve lotlar | `/materials`, `/materials/details/{id}` | Manager | FR-08 | 2 |
| E-09 | Temizlik tipleri ve makine eşleme | `/cleaning-types`, `/cleaning-types/machines/{id}` | Manager | FR-03, FR-50 | 3 |
| E-10 | Tip detayı ve versiyon geçmişi | `/cleaning-types/details/{id}` | Manager | FR-09 | 3 |
| E-11 | Prosedür taslağı düzenleyici | `/procedure-versions/edit/{id}` | Manager | FR-04..FR-07 | 3 |
| E-12 | Versiyon görüntüleme | `/procedure-versions/details/{id}` | ManagerOrAuditor | FR-09 | 3 |
| E-13 | İşlerim | `/my-work` | Operator | FR-26, FR-45 | 4 |
| E-14 | Yeni kayıt | `/records/create` | Operator | FR-10, FR-11, FR-50, FR-53 | 4 |
| E-15 | Yürütme ekranı | `/records/run/{id}` | Operator | FR-16..FR-23, FR-42 | 4 (Açıldı hali), 5 (tam) |
| E-16 | Makine durum panosu | `/machine-board` | RecordReaders | FR-41 | 4 |
| E-17 | Görevli paneli ve katılım onayı (E-15 içinde) | `/records/add-participant/{stepId}` | Operator | FR-17, FR-48 | 5 (ekleme), 7 (katılım onayı, PL-57) |
| E-18 | Mola ekranı | `/breaks/current` | Operator | FR-42 | 5 |
| E-19 | Kayıtlar | `/records` | RecordReaders | FR-38 | 6 |
| E-20 | Kayıt detayı | `/records/details/{id}` | RecordReaders | FR-31, FR-34 | 6 |
| E-21 | Bütünlük doğrulama | `/records/integrity/{id}` | ManagerOrAuditor | FR-32 | 6 |
| E-22 | Yönetici panosu | `/dashboard` | ManagerOrAuditor | İÖ-07 | 7 (PL-46) |
| E-23 | Talep formu (E-15 ve E-20 içinde) | `/requests/create` | Operator | FR-43 | 7 |
| E-24 | Taleplerim | `/requests/mine` | Operator | FR-43 | 7 |
| E-25 | Talepler ve karar | `/requests`, `/requests/decide/{id}` | Manager (Auditor salt okunur liste) | FR-44 | 7 |
| E-26 | Müdahale formu (iptal, yarıda kapatma, beklemeye alma, mola bitirme) | `/interventions/create`, `/interventions/end-break/{id}` | Manager | FR-28, BR-40 | 6 (iptal, yarıda kapatma, mola bitirme), 7 (beklemeye alma); PL-46 |
| E-27 | Acil müdahale | `/emergencies/create` | Operator | FR-51 | 6A (PL-56) |
| E-28 | Bildirimler | `/notifications` | Manager | FR-52 | 8 |
| E-29 | Düzeltme | `/corrections/create` | Manager | BR-29 | 8 |
| E-30 | Süre ve efor raporu | `/reports/durations` | ManagerOrAuditor | FR-21, FR-36, FR-40 | 6A (tablo), 9 (grafik, CSV); PL-56 |
| E-31 | Malzeme izlenebilirliği | `/reports/materials` | ManagerOrAuditor | FR-37 | 6A (sorgu), 9 (CSV); PL-56 |
| E-32 | Denetim çıktısı | `/records/audit/{id}` | ManagerOrAuditor | FR-35 | 6 (detayın yazdırma çıktısı), 9 (ayrı adres, düzeltmeler, CSV); PL-56 |
| E-33 | Sistem ayarları ve değişiklik geçmişi | `/settings` | ManagerOrSuperAdmin | FR-55 | 9 |

Sistem sayfaları: 403 (`/account/access-denied`), hata sayfası, ana sayfa `/` (rolüne göre İşlerim, Yönetici panosu ya da Kullanıcılar'a yönlenir;
Yönetici ve Denetçi Faz 6'dan Faz 7'ye kadar Kayıtlar'a, PL-46).

Salı (2026-11-17) takvimi: Faz 3, 4, 5, 6 ve 6A Salı'dan önce, Faz 7, 8 ve 9 sonra (O-03, O-04); ekran envanteri ve rol matrisi değişmedi.

## 5. Veri varlıkları

Alan listesi Brief §7.g'deki taslaktır; tablo ve kolon adları dbadmin'in kararıdır. Burada varlık, sahip olduğu kural ve geldiği faz yazılır.

| Varlık | İçerik | Kural ve kısıt | Faz |
|---|---|---|---|
| User, kullanıcı rolleri | Sicil no, ad, parola ve PIN özeti, ilk giriş bayrakları, PIN hatalı deneme sayacı ve kilidi, aktiflik; roller Operator, Manager, Auditor, SuperAdmin | Sicil no benzersiz; parola ve PIN tek yönlü özet (NFR-15); silinmez (BR-05); son aktif Süper admin pasifleşmez ve rolünü kaybetmez (SK-9). Seed: tek Süper admin | 1 |
| SystemSetting, SettingChange | Ayar başına bir satır (anahtar, değer, birim): Faz 1'de oturum süresi, PIN uzunluğu, PIN deneme sınırı; Faz 3'te fotoğraf boyutu, video boyutu, azami kilit süresi (PL-25). Her değişiklikte eski, yeni değer, yapan, zaman | Değerler pozitif tam sayı; seed değerleri kullanıcıdan gelir (§6); yeni ayar satır eklenerek gelir, kolon eklenmez; ekran Faz 9 (PL-17) | 1 |
| Facility, ProductionLine, Machine | Kod, ad, aktiflik; makinede kullanımdan kaldırılma anı | BR-54 kod benzersizliği, kod tire içermez; kod ve makinenin hattı oluşturulduktan sonra değişmez (PL-04) | 2 |
| Shift | Tesis, ad, başlangıç ve bitiş saati, aktiflik | Aynı tesiste aktif vardiyalar çakışmaz, gece yarısını geçebilir, aralık başlangıç dahil bitiş hariç (K-38) | 2 |
| Material, MaterialLot | Malzeme kodu ve adı; lot no ve SKT | Lot no malzeme içinde benzersiz | 2 |
| CleaningType, MachineCleaningType | Kod, ad, tür (Planlı, Acil), varsayılan kilit süresi; makine eşlemesi | Acil tip eşleme ve faz minimumu taşımaz (BR-50); tür oluşturulduktan sonra değişmez (PL-04) | 3 |
| ProcedureVersion, ProcedurePhase, ProcedureStep, StepMedia | Versiyon no, durum, malzeme zorunluluğu, doküman no; sıralı faz ve adımlar; medya | Tip başına tek yürürlükteki ve tek taslak versiyon; yayınlanmış versiyon değişmez (BR-04, veritabanı tetikleyicisi) | 3 |
| CleaningRecord | Kayıt no, saha referansı, tür, durum, makine ve versiyon bağı, sahip, açılış anı değerleri, zamanlar, süre toplamları | Makine kilidi (BR-23), sahip kilidi (BR-37), kayıt no benzersiz; sonraki fazların kolonları ilk oluşturmada eklenir (PL-02) | 4 |
| RecordPhase, RecordStep | Faz ve adım yürütmeleri, zamanlar, adım molası, süre, efor, süre altı gerekçesi | Dolu zamanlar değişmez (BR-28, tetikleyici); adım molası kolonu Faz 4'te önden gelir (PL-19) | 4 |
| StepParticipant | Adım görevlileri, ekleyen, onay anı, çalışıyor bayrağı | Kişi başına tek çalışılan adım (BR-24) | 4 |
| RecordMaterial | Lot, açılış anı malzeme ve lot değerleri, SKT geçmiş işareti ve gerekçesi | İlk adımdan sonra değişmez (BR-47) | 4 |
| CleaningEvent | Olay günlüğü: sıra, tür, değerler, yapan, sunucu zamanı, önceki özet, özet | Yalnız eklenir (BR-31, tetikleyici); özet zinciri (BR-32) | 4 |
| İşlenmiş komut | Komut kimliği ve ilk sonuç | Aynı komut ikinci etki üretmez (BR-33) | 4 |
| NumberSequence | Kayıt no ve saha referansı sayaçları | Makine-yıl ve tesis-yıl kapsamı (BR-08, BR-09) | 4 |
| RecordBreak | Mola: başlatan, başladığı anda devam eden adım (boş olabilir), zamanlar, kapanış türü, gerekçe | Kayıtta ve kişide tek açık mola (BR-40) | 5 |
| RecordRequest | Talep: tür, gerekçe, durum, karar veren, karar notu | Kayıtta tek bekleyen talep (BR-41) | 7 |
| RecordHold | Bekleme aralıkları | Kayıtta tek açık bekleme | 7 |
| Notification, NotificationReceipt | Acil ve yeni talep bildirimi; kişi başına okundu bilgisi | BR-51; alıcı aktif Yönetici'ler | 8 |
| Düzeltme | Hedef, orijinal değer, yeni değer, gerekçe, yapan, zaman | Orijinal korunur, zaman alanı hedef olamaz (BR-29) | 8 |

## 6. Ayar değerleri

Değerler iş birimi ve BT'den gelir; plan, seed ve kodda uydurma değer kullanılmaz. Sistem geneli değerler veritabanındaki ayar tablosundadır,
Yönetici ve Süper admin Sistem ayarları ekranından değiştirir (K-47); tip bazlı değerler yönetici tanım ekranlarındadır.

Yer tutucu adı ayarın kimliğidir; dokümanlar kuralı bu adla yazar, değer ayar tablosundan okunur. "Başlangıç değeri" seed'e giren değerdir.

| Yer tutucu | Birim | Nerede | Okunduğu faz | Başlangıç değeri | Kaynak |
|---|---|---|---|---|---|
| `<OturumSüresi>` | dakika | Sistem ayarları | 1 | 30 (son istekten sonraki işlemsizlik) | Kullanıcı, 2026-11-13 (PL-24) |
| `<PinUzunluğu>` | hane | Sistem ayarları | 1 | 4 | Kullanıcı, 2026-11-13 (PL-24) |
| `<PinDenemeSınırı>` | deneme | Sistem ayarları | 1 | 5 (art arda 5. hatalı denemede kilit) | Kullanıcı, 2026-11-13 (PL-24) |
| `<AzamiMedyaBoyutu>` (fotoğraf ve video ayrı) | MB (1 MB = 1 048 576 bayt, PL-51) | Sistem ayarları | 3 | Fotoğraf 10, video 100 | Kullanıcı, 2026-11-14 (PL-47) |
| `<AzamiKilitSüresi>` | dakika | Sistem ayarları | 3 | 120 | Kullanıcı, 2026-11-14 (PL-34) |
| Varsayılan kilit süresi, faz minimum süresi, malzeme zorunluluğu | dakika, evet/hayır | Tip ve versiyon ekranları | 3 | Yönetici girer | İş birimi |
| Tesis, hat, makine kodları ve vardiyalar | - | Tanım ekranları ya da seed | 2 | Nötr demo veri | İş birimi (K-19) |
| Olay özeti anahtarı | - | Yapılandırma anahtarı `EventChain:HmacKey`; Development'ta user-secrets, üretimde ortam değişkeni; veritabanı, ayar dosyası ve repo dışında | 4 | Salı demosu için geliştirme anahtarı; üretim anahtarı sonra | Kullanıcı, 2026-11-14 (PL-48); üretim anahtarı BT (K-23) |

Sistem ayarları ekranı Faz 9'da gelir (PL-17); o zamana kadar değerler seed'den okunur, QA farklı değeri QA kopyasındaki ayar tablosunda dener.
Değişikliğin etkisi PL-18'dedir. Faz 1'den sonra kalan satırlar (`<AzamiKilitSüresi>`, iki `<AzamiMedyaBoyutu>` satırı) Faz 3'te seed
ekleme olarak gelir (PL-25); üç değer de geldi (PL-34, PL-47).

## 7. Kapsam dışı

| Konu | Gerekçe |
|---|---|
| Çevrimdışı çalışma, istemcide işlem biriktirme | K-15 kararı |
| Periyodik temizlik takvimi, geciken temizlik listesi | K-34, İÖ-08 |
| ERP, MES, İK ya da bakım sistemi entegrasyonu; iş emri listesi; CSV ile toplu içe aktarım | K-18, K-19 |
| Kart okuyucu, kurumsal dizin ile giriş | K-09 kararı |
| E-posta ve SMS bildirimi | K-43 |
| Sunucu tarafı PDF üretimi | K-28; yazdırılabilir HTML + CSV |
| Belirli bir mevzuat uyumu, elektronik imza, dört göz onayı, "üretime açıldı" onayı | K-22, İÖ-10 |
| Zincir başı özetlerinin dışa aktarılması (dış çapa) | K-23 |
| Çok dil | K-31 |
| Personel ve vardiya bazlı süre raporu | K-36 |
| Adım sonu sonuç, ölçüm, kanıt fotoğrafı | K-27; adımda yalnız başlat ve bitir |
| Malzeme miktarı, tip başına izinli malzeme listesi | K-11 kararı |
| Kullanıcıya tesis ya da hat ataması, kullanıcıya bağlı yönetici | K-20, K-40 |
| Sahiplik devri, adım atlama, paralel adım, adımın ayrıca duraklatılması, kişi düzeyi mola | K-04, K-06, K-21, K-39 kararları |
| Süper admin hesabını uygulama dışından kurtarma (komut satırı aracı) | §9 açık konu; ikinci Süper admin önerilir |
| İÖ-01..İÖ-06, İÖ-08, İÖ-10 | Brief §11; istenirse Faz 9'dan sonra |

## 8. Varsayımlar

Brief §12 (V-01..V-18) geçerlidir; V-08 ve V-15 2026-11-13 yanıtlarıyla güncellendi. Kullanıcının analiste bıraktığı ya da yanıtından türeyen
ayrıntılar `Decisions.md` PL-15..PL-19'dadır: birleşik rol sabitleri, kendi hesabını pasifleştirme yasağı, ayar ekranının Faz 9'a
yerleşmesi, ayar değişikliğinin etkisi ve ayar birimleri (dakika, hane, deneme, MB), adım molası kolonu.

Analistin makul varsayılanları (`Decisions.md` durumu "Varsayım"; itiraz gelirse yeni satırla değişir):

- PL-27: Sicil no hesap oluşturulduktan sonra değişmez; yanlış sicil noyla açılan hesap pasifleştirilir, doğrusuyla yeni hesap açılır.
- PL-51: 1 MB = 1 048 576 bayt ve sınıra eşit dosya kabul; demo versiyonlarda yayınlayan "Kurulum verisi"; tip kodu değiştirilebilir; Denetçi taslak versiyonu göremez.

## 9. Açık konular

Engelleyici soru yoktur; plan 2026-11-13'te onaylandı (O-01). Aşağıdakiler ilgili fazdan önce gerekir, yoksa o faz başlamaz.

| Konu | Gerektiği an | Sahibi | Not |
|---|---|---|---|
| Üretim olay özeti anahtarı | Üretime kurulumdan önce | BT | Faz 4 geliştirme anahtarıyla başlar (PL-48); üretim anahtarı ortam değişkeniyle gelir, kod değişmez. Geliştirme anahtarıyla yazılmış zincir üretimde doğrulanmaz, demo veritabanı taşınmaz |
| Gerçek tesis, hat, makine, vardiya, malzeme listesi | İstendiğinde | İş birimi | Gelene kadar nötr demo veri (K-19, V-13) |
| Tek Süper admin riski | Kurulumdan sonra | Kullanıcı | Tek Süper admin parolasını unutur ya da PIN'ini kilitlerse uygulama içinden açılamaz; ikinci bir Süper admin hesabı açılması önerilir |
| Ölçek sayıları | Performans ölçümünden önce | İş birimi | K-33 |

Tasarım girdisi: hava kelimeleri ve referans gelmedi; alandan türetilen öneri (net, sakin, güvenilir, sahada okunaklı) geçerlidir (PhasePlan Faz T).

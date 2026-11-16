# Dijital Temizlik Takip Projesi - Açık Konular (Kusur)

> Sahibi: analyst. Kaynak: `Docs/Dijital_Temizlik_Takip_Projesi_Is_Birimi_Anlatim_Senaryosu.docx` (satır numaraları `doc.txt` çıkarımına göredir).
> İlgili belge: `Docs/Analysis/Brief.md` (FR, BR, SC kimlikleri oradadır).
> Öncelik: **Engelleyici** = plan donmadan ve Faz 1 başlamadan yanıt gerekir (şemayı ya da çekirdek kuralı değiştirir);
> **Önemli** = ilgili faz başlamadan yanıt gerekir, önerilen varsayılanla ilerlenebilir; **Sonra netleşebilir** = varsayılanla ilerlenir.
> Yanıt verilmeyen maddede "Öneri" satırındaki varsayılan uygulanır ve `Docs/Phases/Decisions.md` dosyasına işlenir; Engelleyici maddelerde bu yapılmaz.
> 2026-11-13: K-01..K-15 yanıtlandı, kararlar her maddenin altında. Yanıtlardan doğan yeni sorular K-38..K-48. K-16..K-29 ve K-31..K-37
> için önerilen varsayılan uygulandı. K-30 2026-11-13'te onaylandı.
> K-01..K-37 "Etki" satırlarındaki faz numaraları ilk taslak plana göredir; güncel numaralar `Docs/Phases/PhasePlan.md`'dedir. K-38 ve sonrası güncel plana göre yazıldı.
> 2026-11-13 (kapanış): K-38, K-39, K-40 ve K-47 kullanıcı yanıtıyla karara bağlandı (kararlar maddelerin altında). K-16..K-29, K-31..K-37,
> K-41..K-46 ve K-48'in önerilen varsayılanları kullanıcı tarafından kabul edildi; "Öneri" satırındaki **Varsayılan** geçerlidir.
> Açık soru kalmadı; ayar değerleri yer tutucu olarak kalır (Requirements §6, §9).

## Özet

| Kimlik | Başlık | Öncelik |
|---|---|---|
| K-01 | Faz minimum süresi tutmazsa ne olacak | Engelleyici, karar 2026-11-13 |
| K-02 | Net çalışma süresinin tanımı (40 dakikalık boşluk) | Engelleyici, karar 2026-11-13 |
| K-03 | Başlamamış kaydın kilit süresi ve süre dolunca ne olacağı | Engelleyici, karar 2026-11-13 |
| K-04 | Sahibin devredilemezliği ile "kişi işten ayrıldı" durumunun çelişkisi | Engelleyici, karar 2026-11-13 |
| K-05 | Yönetici müdahalesinin türleri, gerekçe ve sahibin kendi kaydını iptali | Engelleyici, karar 2026-11-13 |
| K-06 | Adım sırasının katılığı (atlama, paralel adım) | Engelleyici, karar 2026-11-13 |
| K-07 | Yardımcı personel: kayıt düzeyi mi adım düzeyi mi, adım sürerken değişim, efor | Engelleyici, karar 2026-11-13 |
| K-08 | "Aynı anda iki temizlikte çalışamaz" kuralının kapsamı | Engelleyici, karar 2026-11-13 |
| K-09 | Kimlik doğrulama yöntemi ve paylaşılan cihaz | Engelleyici, karar 2026-11-13 |
| K-10 | Yardımcı personelin kaynağı ve katılımının kanıtı | Engelleyici, karar 2026-11-13 |
| K-11 | Malzemenin giriş anı, düzeyi, miktarı ve lot kaynağı | Engelleyici, karar 2026-11-13 |
| K-12 | Son kullanma tarihi geçmiş malzeme | Engelleyici, karar 2026-11-13 |
| K-13 | Kayıt numarası ve saha defteri referansının biçimi ve üretim anı | Engelleyici, karar 2026-11-13 |
| K-14 | Plansız müdahalenin checklist'e bağlı olup olmadığı | Engelleyici, karar 2026-11-13 |
| K-15 | Çevrimdışı çalışma ihtiyacı | Engelleyici, karar 2026-11-13 |
| K-16 | Düzeltme ihtiyacı (yanlış personel, yanlış lot) | Önemli |
| K-17 | Operatörün başkalarının kayıtlarını görme kapsamı | Önemli |
| K-18 | Üretim iş emri bilgisinin kaynağı ve zorunluluğu | Önemli |
| K-19 | Tesis, hat, makine ve kullanıcı verisinin kaynağı | Önemli |
| K-20 | "Herhangi bir makineyi rastgele seçememeli" ifadesinin kapsamı | Önemli |
| K-21 | Vardiya bitiminde açık bırakılan adım | Önemli |
| K-22 | Mevzuat çerçevesi, elektronik imza, kontrol/onay adımı | Önemli |
| K-23 | Bütünlük kanıtının düzeyi (dış çapa, BT erişimi) | Önemli |
| K-24 | Versiyon yayın onayı, yürürlük tarihi, doküman numarası | Önemli |
| K-25 | Açık kaydı olan makinenin kullanımdan kaldırılması | Önemli |
| K-26 | Medya biçimi, boyutu ve yükleme sorumluluğu | Önemli |
| K-27 | Adım sonunda veri toplama (sonuç, ölçüm, fotoğraf kanıtı) | Önemli |
| K-28 | Saklama süresi ve denetim çıktısının biçimi | Önemli |
| K-29 | Yönetici sahada iş yapabilir mi, çoklu rol | Önemli |
| K-30 | Sahiplik kontrolünün "yalnız rol bazlı yetki" kuralıyla uyumu (süreç) | Önemli, karar 2026-11-13 |
| K-31 | Dil ihtiyacı | Sonra netleşebilir |
| K-32 | Sistem saati ve saat dilimi | Sonra netleşebilir |
| K-33 | Ölçek ve SQLite uygunluğu | Sonra netleşebilir |
| K-34 | Periyodik temizliklerin planlanması kapsamda mı | Sonra netleşebilir |
| K-35 | "Kim başlatmış" sorusunun anlamı | Sonra netleşebilir |
| K-36 | Rapor içerikleri ve üretim planlama ihtiyacı | Sonra netleşebilir |
| K-37 | Bildirim ihtiyacı | Sonra netleşebilir |
| K-38 | Vardiya tanımı ve vardiya bitimine denk gelen kilit | Engelleyici, karar 2026-11-13 |
| K-39 | Molanın kapsamı: kayıt mı kişi mi, adım sürerken mola | Engelleyici, karar 2026-11-13 |
| K-40 | Talebi hangi yönetici görür ve karara bağlar | Engelleyici, karar 2026-11-13 |
| K-41 | Kendi kaydı devam ederken başka kayıtta yardım | Önemli |
| K-42 | PIN biçimi ve hatalı deneme kuralı | Önemli |
| K-43 | Bildirim kanalı | Önemli |
| K-44 | Acil müdahalenin makine kilidi ve tek aktif kayıt kuralıyla ilişkisi | Önemli |
| K-45 | Beklemedeki kaydın kilit, süre ve devam etkisi | Önemli |
| K-46 | Kilit süresinin üst sınırı ve süreyi uzatan işlem | Önemli |
| K-47 | Sistem geneli ayarların yeri | Sonra netleşebilir, karar 2026-11-13 |
| K-48 | Denetçi rolü | Önemli |

---

## Engelleyici

### K-01: Faz minimum süresi tutmazsa ne olacak
- Kaynak: doc.txt satır 17-18, 65-66
- Sorun: "Sistemin de buna göre kontrol yapabilmesini istiyoruz" deniyor ama kontrolün sonucu tanımlı değil: süre dolmadan faz kapatılamaz mı, uyarı mı verilir, gerekçe mi istenir, yalnız raporda mı işaretlenir? Ayrıca kontrolün hangi süreye (brüt mü, net mi; K-02) göre yapılacağı belli değil.
- Etki: FR-22, BR-21, Faz durum makinesi ("Tamamlandı - süre altında" durumu olacak mı), Faz 4 yürütme ekranı ve Faz 6 uyum raporu. Yanıt gelmezse Faz 4'ün kabul kriterleri yazılamaz.
- Öneri: **Varsayılan:** faz net süresi minimumun altındaysa son adım bitirilirken operatörden gerekçe istenir, faz "Tamamlandı - süre altında" olur, raporda işaretlenir. Gerekçe: "engel" seçilirse operatör son adımı açık tutup bekleyerek kuralı geçer, bu da net süreyi yapay biçimde şişirir ve ölçümü bozar. **Alternatifler:** (a) engel: minimum dolmadan son adım bitirilemez; (b) yalnız uyarı, gerekçesiz; (c) yalnız raporda işaret, sahada görünmez.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Gerekçe zorunlu. Fazın net süresi (SH-06) minimumun altındaysa fazın son adımı gerekçe yazılmadan bitirilemez; faz "Tamamlandı - süre altında" olur, gerekçe fazda saklanır, kayıt detayında, denetim çıktısında ve raporda görünür. Kontrol net süreye göre yapılır. Acil tiplerde faz minimum süresi yoktur (K-14).

### K-02: Net çalışma süresinin tanımı (40 dakikalık boşluk)
- Kaynak: doc.txt satır 67-71
- Sorun: 12 saniyelik adım + 40 dakikalık ayrılık örneğinde "40 dakikanın dahil edilmesini istemediğimiz durumlar olabilir" deniyor; "olabilir" ifadesi hangi durumlarda dahil edileceğini açık bırakıyor. Adımlar arası hazırlık süresi (malzeme getirme, kuruma bekleme) çalışma sayılmalı mı belli değil.
- Etki: FR-21, FR-23, BR-20, BR-21, K-01, Faz 4 hesapları, Faz 6 raporları. Veri modeli seçeneğe göre değişir (duraklat/devam seçilirse yeni olay türü ve alan gerekir).
- Öneri: **Varsayılan:** net süre = adım sürelerinin toplamı (adımlar arası boşluk hiç sayılmaz); brüt süre ayrıca saklanır ve gösterilir. **Alternatifler:** (a) brüt süreden yalnız `<Eşik>` dakikayı aşan boşlukların düşülmesi (eşik değeri iş biriminden gelmeli); (b) operatörün açıkça "duraklat / devam" yapması; (c) kuruma/bekleme gibi bilinçli bekleyişler için prosedüre "bekleme adımı" tanımlanması (bekleme de süre olarak sayılır).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Brüt = son adım bitişi - ilk adım başlangıcı; Net = Brüt - Σ (mola bitişi - mola başlangıcı). Molayı operatör açıkça başlatır ve bitirir, iki zaman da sunucu saatidir. Mola açıkken molayı başlatan kişi sistemde hiçbir yazma işlemi yapamaz. Mola verilmemiş boşluk çalışma sayılır. Molanın kapsamı ve adım içinde mola K-39'a ayrıldı; beklemeye alınan sürenin düşülmesi K-45.

### K-03: Başlamamış kaydın kilit süresi ve süre dolunca ne olacağı
- Kaynak: doc.txt satır 83-87
- Sorun: Açılmış ama başlanmamış kaydın makineyi "sonsuza kadar" kilitlememesi isteniyor; süre değeri ve süre dolunca ne olacağı (otomatik zaman aşımı mı, yalnız başkası istediğinde mi düşer, yönetici mi kaldırır) belirtilmemiş. Kaydı açıp 15 dakika sonra başlamak (satır 46) normal kabul ediliyor; süre bu örneği karşılamalı.
- Etki: FR-27, BR-26, kayıt durum makinesi ("Zaman aşımı" durumu), Faz 3 makine kilidi ve pano. Yanıt gelmezse Faz 3 kabul kriteri SC-15 yazılamaz.
- Öneri: **Varsayılan:** yönetici tarafından ayarlanabilir `<KilitSüresi>` (başlangıç değerini iş birimi verir); süre dolduktan sonra aynı makine için yeni kayıt istendiğinde ya da pano sorgulandığında eski kayıt "Zaman aşımı" olur ve olay günlüğüne yazılır; kayıt silinmez, numarası yeniden kullanılmaz. **Alternatifler:** (a) kilit süresiz, yalnız yönetici kaldırır; (b) süre dolunca kilit kalkar ama kayıt "Açıldı" kalır ve sahibi isterse yeniden kilit alır (makine boşsa).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Kilit süresi temizlik tipinde varsayılan değer olarak tanımlanır; operatör kayıt açarken isteğe bağlı değiştirir (üst sınır K-46). İlk adım kilit bitişine kadar başlamazsa kayıt "Zaman aşımı" olur, makine serbest kalır, kayıt silinmez. Vardiya kavramı belgede tanımlı olmadığı için K-38 açıldı; önerilen kural: kilit bitişi = en erken (açılış + kilit süresi, açılış anını içeren vardiyanın bitişi) (SH-13).

### K-04: Sahibin devredilemezliği ile "kişi işten ayrıldı" durumunun çelişkisi
- Kaynak: doc.txt satır 36, 90-94
- Sorun: Sahiplik "başka birinin üzerine geçirilememeli" (36); ama sahip işten ayrılır, bölüm değiştirir ya da işi sürdüremezse (90) yarım kalan temizliğin nasıl devam edeceği tanımsız. Devir yasaksa iş ya baştan yapılmalı ya da kalan adımlar için yeni bir kayıt açılmalı.
- Etki: FR-12, FR-28, BR-07, BR-27, Faz 5 müdahale tasarımı; "devam kaydı" seçilirse veri modeline önceki kayda bağlantı alanı eklenir (Faz 3 şeması).
- Öneri: **Varsayılan:** sahip asla değişmez; yönetici kaydı gerekçeyle "Yarıda kapatıldı" yapar, makine kilidi kalkar, makine temiz sayılmaz; yeni sorumlu yeni kayıt açar ve temizlik baştan yapılır; yeni kayıtta isteğe bağlı "önceki kayıt" bağlantısı tutulur. **Alternatifler:** (a) "devam kaydı": yeni kayıt kalan adımlardan başlar, tamamlanan adımlar önceki kayıtta kalır (rapor ve süre hesabı iki kayda bölünür); (b) yalnız yöneticinin gerekçeyle yaptığı, olay günlüğünde görünen sahiplik devri (satır 36 ile çelişir, iş birimi onayı şart).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Sahip değişmez, devir yok. Üzerinde açık işi olan kullanıcı pasifleştirilemez. Açık iş: sahibi olduğu Açıldı, Devam ediyor ya da Beklemede kayıt; görevli olarak listelendiği tamamlanmamış adım; bekleyen talebi; açık molası. Pasifleştirme ekranı bu işleri bağlantılarıyla listeleyen bir uyarı gösterir. Ayrılan kişinin kaydı önce yönetici tarafından yarıda kapatılır, sonra kişi pasifleştirilir.

### K-05: Yönetici müdahalesinin türleri, gerekçe ve sahibin kendi kaydını iptali
- Kaynak: doc.txt satır 88-94
- Sorun: "Yöneticinin müdahale edebileceği bir yapı" isteniyor ama müdahalenin türleri belirtilmemiş: iptal (kayıt geçersiz, temizlik sayılmaz) ile kapatma (iş yarıda kaldı) aynı mı? Gerekçe zorunlu mu? "Saha personelinin istediği kaydı kapatmasını istemiyoruz" (92): operatör, hiç başlamadığı ve yanlış açtığı kendi kaydını iptal edebilir mi?
- Etki: FR-28, BR-27, kayıt durum makinesi, Faz 5 müdahale paneli, Faz 3 sihirbaz (iptal düğmesi).
- Öneri: **Varsayılan:** iki ayrı müdahale: "İptal" (hatalı kayıt; temizlik sayılmaz) ve "Yarıda kapatma" (sürdürülemeyen iş; makine temiz sayılmaz); ikisinde de gerekçe zorunlu, kayıt silinmez; operatör yalnız kendi, hiçbir adımı başlamamış kaydını gerekçeyle iptal edebilir. **Alternatifler:** (a) tek tür "Yönetici tarafından kapatıldı" + gerekçe; (b) operatör hiçbir kaydı iptal edemez, her iptal yöneticidedir.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** İki ayrı müdahale: İptal (hatalı kayıt, temizlik sayılmaz) ve Yarıda kapatma (sürdürülemeyen iş, makine temiz sayılmaz); ikisinde de gerekçe zorunlu, kayıt silinmez. Operatör kaydı kendisi kapatmaz; kaydın sahibi yöneticiye gerekçeli talep açar. Yönetici talepleri "Talepler" ekranından izler, onaylar ya da gerekçeyle reddeder; onay ilgili müdahaleyi uygular. Yönetici talep olmadan da doğrudan müdahale edebilir. Talebi hangi yöneticinin göreceği K-40.

### K-06: Adım sırasının katılığı (atlama, paralel adım)
- Kaynak: doc.txt satır 14-15, 49
- Sorun: "Hangi işlemi önce, hangisini sonra yapacağını bilmeli" sıranın gösterilmesini istiyor; ama sıranın zorunlu olup olmadığı, bir adımın atlanabilir (uygulanamaz) olup olmadığı ve iki kişinin aynı kayıtta farklı adımları aynı anda yürütüp yürütemeyeceği belirtilmemiş.
- Etki: BR-14, BR-15, adım durum makinesi ("Atlandı" durumu gerekir mi), net süre hesabı (paralel adımda aralıkların birleşimi gerekir), Faz 4.
- Öneri: **Varsayılan:** katı sıra, atlama yok, kayıtta aynı anda tek devam eden adım. **Alternatifler:** (a) faz içinde sırasız, fazlar arası sıralı; (b) "uygulanamaz" işaretiyle gerekçeli atlama (yönetici onayı ile ya da onaysız); (c) faz içinde paralel adım (net süre = aralıkların birleşimi).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Katı sıra. Adım atlanamaz, bir adım kendinden önceki adım tamamlanmadan başlatılamaz, bir kayıtta aynı anda en çok bir adım devam eder. "Atlandı" durumu yoktur.

### K-07: Yardımcı personel: kayıt düzeyi mi adım düzeyi mi, adım sürerken değişim, efor
- Kaynak: doc.txt satır 37, 50-53
- Sorun: Satır 37 "varsa yardımcı personeller" bilgisini kayıt alanı olarak sayıyor; satır 50-53 personelin adım bazında seçildiğini söylüyor. Ayrıca (a) görevliyi kim ekler (yalnız sahip mi, adımın görevlisi de mi), (b) adım başladıktan sonra kişi eklenip çıkarılabilir mi, (c) sahip bir adımdan çıkarılabilir mi (satır 50 üçüncü adımda yalnız Mehmet) belirtilmemiş.
- Etki: FR-17, FR-21, BR-17, BR-19, veri modeli (`StepParticipants` katılım aralığı tutacak mı), Faz 4 görevli paneli ve efor hesabı.
- Öneri: **Varsayılan:** personel yalnız adım düzeyinde tutulur; kayıt düzeyindeki "yardımcılar" adım görevlilerinin birleşimi olarak gösterilir; görevliyi yalnız sahip ekler/çıkarır; görevli listesi adım başladıktan sonra değişmez; sahip bir adımdan çıkarılabilir ama o adımı başlatıp bitirebilir. Efor = adım süresi x görevli sayısı. **Alternatifler:** (a) adım sürerken katılma/ayrılma olayları; efor = kişi bazlı katılım aralıklarının toplamı (daha doğru, ek etkileşim); (b) kayıt açılışında bir "ekip" seçilir, adımda bu ekipten seçim yapılır.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Görevli adım düzeyinde tutulur; kayıt düzeyindeki "yardımcı personel" adım görevlilerinin birleşimidir. Görevliyi yalnız kaydın sahibi, adım başlamadan ekler ya da çıkarır; adım başladıktan sonra liste değişmez. Sahip bir adımdan çıkarılabilir ve o adımı yine de başlatıp bitirebilir. Efor = adım süresi x görevli sayısı (SH-02).
- **Karar (2026-11-16, O-10, PL-93):** Alternatif (a) seçildi, Faz 6E'den itibaren geçerli. Sahip adım başlamadan ya da sürerken görevli ekler; görevli adıma katılma ve isterse ayrılma anını kendi PIN'iyle girer (anlar sunucu saati, elle süre yok). Adım girişi bekleyen kişiyle de başlar; sahip dışında girişi bekleyen kişi varken adım bitmez. Sahip yalnız hiç katılmamış kişiyi çıkarır. Efor = kişi çalışma sürelerinin toplamı (SH-02, SH-02a). Kişi aralığı yeni tablo `StepWorkIntervals` (Eklemeli).

### K-08: "Aynı anda iki temizlikte çalışamaz" kuralının kapsamı
- Kaynak: doc.txt satır 76-77
- Sorun: Satır 77'deki örnek "aynı anda başka bir adımı başlatamamalı" diyerek kısıtı devam eden adım düzeyinde tarif ediyor; satır 76 ise "iki farklı temizlikte çalışamamalı" diyor. Bir kişinin birden çok açık kaydı olabilir mi (ör. vardiya sonunda yarım bıraktığı kayıt açıkken başka makinede yeni kayıt açmak)? Başlamamış bir adımın görevli listesinde olmak da kilit sayılır mı?
- Etki: FR-25, BR-24, kısmi unique index'in hangi tabloda ve hangi filtreyle kurulacağı (Faz 3 şeması), Faz 4.
- Öneri: **Varsayılan:** kısıt yalnız "Devam ediyor" adım düzeyinde: bir kişi aynı anda en çok bir devam eden adımda görevli olabilir; birden çok açık kayda sahip olmak ve başlamamış adımlarda görevli olarak listelenmek serbesttir. **Alternatifler:** (a) bir kişinin aynı anda en çok bir "Devam ediyor" kaydı olabilir (yarım kalan kayıt yeni kaydı engeller); (b) bir kişinin en çok bir "Açıldı" (başlamamış) kaydı olabilir (makine kilidi biriktirmeyi önler).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Belge satır 76-77 "aynı anda tek iş" diyor; iki kısıt uygulanır, ikisi de veritabanı kısıtıdır: (1) bir kişinin sahibi olduğu en çok bir aktif kayıt (Açıldı ya da Devam ediyor) olur; (2) bir kişi aynı anda en çok bir devam eden adımda görevli olur. Sahip, devam eden kaydını bırakıp başka işe geçmek için yöneticiye "beklemeye alma" talebi açar (K-05 talep yapısı). Beklemedeki kayıt kısıt (1)'e sayılmaz; makine kilidi, süre ve devam ayrıntısı K-45. Kendi kaydı devam ederken başkasına yardım K-41.

### K-09: Kimlik doğrulama yöntemi ve paylaşılan cihaz
- Kaynak: eksik (ilgili: satır 48, 111)
- Sorun: Sahada hangi cihazın kullanılacağı (kişisel telefon, paylaşılan tablet, sabit terminal) ve kişinin nasıl giriş yapacağı (kullanıcı adı + parola, sicil + PIN, kart) belirtilmemiş. Paylaşılan cihazda bir önceki kişinin oturumu açık kalırsa işlemler yanlış kişiye yazılır; bu, denetim kanıtını doğrudan zayıflatır.
- Etki: NFR-09, Faz 1 giriş ekranı ve kimlik altyapısı, Faz T saha kabuğu (kullanıcı değiştirme). Kart okuyucu ya da kurumsal dizin (ör. Active Directory) istenirse mevcut yığının (cookie kimlik doğrulama, yerel kullanıcı tablosu) dışına çıkılır ve kapsam büyür.
- Öneri: **Varsayılan:** sicil no + parola ile cookie oturumu; işlemsizlikte `<OturumSüresi>` sonra oturum kapanır (değeri iş birimi verir); kabukta oturumdaki kişinin adı sürekli görünür ve tek dokunuşla "kullanıcı değiştir" vardır. **Alternatifler:** (a) sicil no + kısa PIN (sahada hızlı, parola politikası zayıflar); (b) kurumsal dizin ile tek oturum (entegrasyon fazı gerekir); (c) kart okuyucu (donanım ve entegrasyon gerekir).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Giriş sicil no + parola ile. Yönetici kullanıcıyı geçici parolayla açar; kullanıcı ilk girişte parolasını değiştirir ve kişisel PIN belirler, bunlar bitmeden başka ekrana geçemez. Kritik işlemlerde işlemi yapanın PIN'i istenir; liste Brief BR-48. PIN biçimi ve hatalı deneme kuralı K-42. İşlemsizlikte `<OturumSüresi>` sonra oturum kapanır (başlangıç değeri 30 dakika, 2026-11-13, PL-24).

### K-10: Yardımcı personelin kaynağı ve katılımının kanıtı
- Kaynak: doc.txt satır 37, 50-53, 112
- Sorun: Yardımcı personelin sisteme giriş yapan bir kullanıcı mı, yoksa yalnız listeden seçilen bir personel kaydı mı olduğu belirtilmemiş. Sahip, yanında olmayan birini görevli seçebilir; o kişinin eforu ve kilidi onun haberi olmadan oluşur. "Kimler hangi adımlarda görev almış" (112) sorusunun kanıt değeri, katılımın nasıl doğrulandığına bağlıdır.
- Etki: FR-17, BR-17, BR-24, veri modeli (`Users` ile ayrı `Personnel` tablosu), Faz 1 kullanıcı yönetimi, Faz 4 görevli paneli.
- Öneri: **Varsayılan:** tüm saha personeli sistem kullanıcısıdır (sicil no ile); sahip görevliyi listeden seçer, yardımcı ayrıca onay vermez; seçim olay günlüğüne "ekleyen: sahip" olarak yazılır. **Alternatifler:** (a) yardımcı, kendi sicil + PIN'iyle aynı cihazda katılımı onaylar (kanıt güçlenir, sahada bir adım daha); (b) giriş yapmayan personel kayıtları (`Personnel`) ayrı tutulur, yalnız sahip giriş yapar.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Belgeye göre seçenek (a). Tüm saha personeli sistem kullanıcısıdır (sicil no). Sahip yardımcıyı listeden seçer, yardımcı katılımını aynı cihazda kendi PIN'iyle onaylar; onay vermeyen kişi görevli olmaz. Gerekçe: satır 52 "kimlerin gerçekten çalıştığı" ve satır 112 bunun kanıtını istiyor; K-09'daki PIN altyapısı onayı tek PIN girişine indiriyor; kişi kısıtı (K-08) kimseyi habersiz kilitlemiyor.
- **Not (2026-11-16, PL-93):** Onay ekleme isteğinden ayrıldı; görevli katılma ve ayrılmayı kendi PIN'iyle, aynı cihazda ya da kendi oturumunda girer (K-07 kararı 2026-11-16).

### K-11: Malzemenin giriş anı, düzeyi, miktarı ve lot kaynağı
- Kaynak: doc.txt satır 20-24, 37
- Sorun: (a) Malzeme kayıt açılışında mı, belirli bir adımda mı, yoksa tamamlanmadan önce herhangi bir anda mı girilecek? Zorunluluk "atlayıp devam etmesini istemiyoruz" (23) hangi noktada kontrol edilecek? (b) Malzeme kayıt düzeyinde mi adım düzeyinde mi tutulacak? (c) Miktar tutulacak mı? (d) Lotlar yönetici tarafından önceden mi tanımlanacak, yoksa operatör lot no ve SKT'yi kendisi mi girecek ("kafasına göre yazmasın", satır 11)? (e) Bir temizlik tipinde hangi malzemelerin kullanılabileceği kısıtlı mı?
- Etki: FR-07, FR-08, FR-11, FR-37, BR-11, veri modeli (`RecordMaterials` kayda mı adıma mı bağlı, miktar alanı), Faz 2 malzeme ekranları, Faz 3 sihirbaz.
- Öneri: **Varsayılan:** malzeme kayıt düzeyinde, kayıt açılışında girilir; zorunlu tipte en az bir malzeme girilmeden kayıt açılamaz; lotlar yöneticinin önceden tanımladığı listeden seçilir; miktar tutulmaz; tipte malzeme kısıtı yoktur. **Alternatifler:** (a) malzeme ilgili adımda girilir (hangi adımda hangi malzeme, daha ayrıntılı izlenebilirlik); (b) operatör lot no ve SKT'yi elle girer (tanım kontrolü zayıflar); (c) miktar ve birim tutulur; (d) tip başına izinli malzeme listesi.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Malzeme kayıt düzeyinde tutulur; kayıt açılışında ya da ilk adım başlatılana kadar eklenir ve çıkarılır. Malzeme zorunlu tipte ilk adım malzemesiz başlatılamaz. Lot yöneticinin tanımladığı listeden seçilir, operatör lot yazmaz. Miktar tutulmaz, tipte izinli malzeme kısıtı yoktur (bu ikisi varsayılan olarak önerildi, 2026-11-13'te kabul edildi). İlk adımdan sonra yanlış lot yalnız düzeltme olayıyla düzeltilir (K-16).

### K-12: Son kullanma tarihi geçmiş malzeme
- Kaynak: doc.txt satır 21
- Sorun: SKT bilgisinin tutulacağı söyleniyor ama SKT geçmiş malzemenin kullanımında sistemin ne yapacağı belirtilmemiş. Kayıt açılışında geçerli olan lotun temizlik sırasında SKT'si dolabilir.
- Etki: BR-12, Faz 2 lot rozetleri, Faz 3 sihirbaz doğrulaması, SC-08.
- Öneri: **Varsayılan:** sunucu tarihine göre SKT'si geçmiş lot seçilemez; kontrol giriş anında yapılır, sonradan dolan SKT kaydı etkilemez. **Alternatifler:** (a) uyarı + gerekçe ile seçilebilir, raporda işaretlenir; (b) kontrol yok, yalnız bilgi.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** SKT'si sunucu tarihine göre geçmiş lot seçilebilir; ekran uyarır, gerekçe zorunludur; malzeme satırı "SKT geçmiş" olarak işaretlenir ve kayıt detayında, denetim çıktısında ve raporda görünür. Kontrol giriş anında yapılır; giriş sonrası dolan SKT kaydı etkilemez.

### K-13: Kayıt numarası ve saha defteri referansının biçimi ve üretim anı
- Kaynak: doc.txt satır 38-40
- Sorun: Kayıt numarasının "işin nerede yapıldığını anlatması" isteniyor ama biçim verilmemiş; tesis, hat, makine için mevcut kısa kodlar olup olmadığı bilinmiyor. Saha defterindeki referansın bugünkü biçimi (sayfa/sıra no, tesis başına mı hat başına mı defter) bilinmiyor. Referansın kayıt açılışında mı ilk adımda mı üretileceği ve iptal edilen kaydın referansının boşa düşmesinin kabul edilip edilmediği (defterde boşluk) belirtilmemiş.
- Etki: FR-13, FR-14, BR-08, BR-09, sayaç kapsamı (Faz 3 şeması), Faz 3 "kayıt oluşturuldu" ekranı.
- Öneri: **Varsayılan:** kayıt no `<TesisKodu>-<HatKodu>-<MakineKodu>-<Tür><Yıl>-<Sıra>` (Tür: planlı `P`, plansız `A`); saha referansı `<TesisKodu>-SD-<Yıl>-<Sıra>`, tesis ve yıl başına sayaç, kayıt açılışında üretilir; iptalde numara boşa düşer ve "iptal" olarak görünür. Sıra uzunluğu ve kodlar iş biriminden. **Alternatifler:** (a) kayıt no makine kodu olmadan; (b) saha referansı hat başına; (c) saha referansı ilk adım başladığında üretilir (iptal ve zaman aşımı numara tüketmez).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Belgede biçim yok; proje biçimi seçildi. Kayıt no: `<TesisKodu>-<HatKodu>-<MakineKodu>-<Tür><Yıl>-<Sıra>`; Tür planlı için `P`, acil için `A`; sıra makine ve yıl başına 1'den artar, sıfır dolgusu yok; kayıt açılışında üretilir. Saha defteri referansı: `<TesisKodu>-SD-<Yıl>-<Sıra>`, tesis ve yıl başına sayaç, yalnız planlı kayıtta ve ilk adım başlatıldığında üretilir. Gerekçe: numara işin yerini okutur (satır 39), tür harfi acili ayırır (satır 44); referans ilk adımda üretildiği için zaman aşımına düşen ya da başlamadan iptal edilen kayıt defterde boşluk bırakmaz. Kodları yönetici tesis yapısı tanımlarında girer (PhasePlan Faz 2); kod tire içermez. Yıl Türkiye saatine göre alınır.

### K-14: Plansız müdahalenin checklist'e bağlı olup olmadığı
- Kaynak: doc.txt satır 41-44 (ilgili: 11-13)
- Sorun: Plansız müdahalenin saha referansı olmayacağı ve ayrı görüneceği belli; ama plansız işte de önceden tanımlı faz/adım listesi (temizlik tipi ve versiyon) kullanılıp kullanılmayacağı, faz minimum süresi ve malzeme zorunluluğunun uygulanıp uygulanmayacağı belirtilmemiş. Acil işte checklist'siz serbest kayıt, "kafasına göre yazmasın" (11) ilkesiyle çelişebilir.
- Etki: FR-15, BR-10, veri modeli (plansız kayıtta `ProcedureVersionId` zorunlu mu), Faz 2 tip tanımı (planlı/plansız kullanım alanları), Faz 3 sihirbaz.
- Öneri: **Varsayılan:** plansız müdahale de tanımlı bir temizlik tipine ve versiyona bağlıdır; yönetici bir tipi "plansız kullanımda geçerli" olarak işaretler; minimum süre ve malzeme kuralları aynen uygulanır. **Alternatifler:** (a) plansız müdahale için tek, kısa, genel bir "acil müdahale" tipi; (b) checklist'siz kayıt: yalnız başlangıç/bitiş, görevliler, açıklama ve malzeme.
- Öncelik: Engelleyici
- **Karar (2026-11-13):** (b) ve (c)'nin birleşimi: ayrı ve hızlı bir acil müdahale akışı. Operatör tek ekranda makineyi ve acil tipini seçer, ne olduğunu yazar (zorunlu), PIN'ini girer; kayıt açılır ve ilk adım aynı işlemde, aynı sunucu anında başlar. Checklist olarak yöneticinin tanımladığı kısa, genel "Acil" türündeki temizlik tipleri kullanılır: versiyonludur, makine eşlemesi gerekmez, faz minimum süresi yoktur. Saha referansı üretilmez. Açılışta yöneticilere uygulama içi bildirim düşer; başka kanal K-43. Gerekçe: acil işte açma ile başlama arasında bekleme yoktur (satır 42 "hemen müdahale"); tamamen checklist'siz kayıt satır 11 ilkesine ve denetim kanıtına aykırıdır.

### K-15: Çevrimdışı çalışma ihtiyacı
- Kaynak: eksik (ilgili: satır 48, 55-57)
- Sorun: Sahada kablosuz ağın her makinenin başında kesintisiz olup olmadığı belirtilmemiş. Çevrimdışı çalışma istenirse zaman cihazda üretilir; bu, "başlama saatini sistem kaydedecek" (55) ve geriye dönük değiştirilemezlik (127) ile doğrudan çelişir ve mevcut yığında (sunucu taraflı MVC) ciddi kapsam büyümesi demektir (servis çalışanı, yerel depo, eşitleme, çakışma çözümü).
- Etki: NFR-01, tüm yürütme mimarisi, Faz 4. Çevrimdışı istenirse en az bir ek faz gerekir ve yürütme mimarisi değişir.
- Öneri: **Varsayılan:** çevrimiçi zorunlu; bağlantı yoksa işlem yapılamaz ve ekran bunu açıkça gösterir; sunucu onayı gelmeden adım durumu değişmez. **Alternatifler:** (a) sınırlı çevrimdışı: yalnız görüntüleme (adım listesi ve medya önbellekte); (b) tam çevrimdışı: cihaz zamanı kaydedilir, eşitlemede "cihaz zamanı" ve "sunucuya ulaşma zamanı" ayrı gösterilir (kanıt değeri düşer).
- Öncelik: Engelleyici
- **Karar (2026-11-13):** Çevrimiçi zorunlu. Bağlantı yoksa işlem yapılamaz ve ekran bunu gösterir; sunucu onayı gelmeden adım, mola ya da kayıt durumu değişmez; istemcide işlem biriktirilmez.

---

## Önemli

### K-16: Düzeltme ihtiyacı (yanlış personel, yanlış lot)
- Kaynak: doc.txt satır 126-129 (ilgili: 89)
- Sorun: Kayıtların değiştirilememesi isteniyor; ama yanlış görevli seçilmesi ya da yanlış lot girilmesi gibi gerçek hatalarda ne yapılacağı belirtilmemiş. Hiç düzeltme olmazsa kayıt bilerek yanlış kalır; düzeltme varsa kimin yapacağı, hangi alanların düzeltilebileceği ve orijinalin nasıl gösterileceği belirlenmeli.
- Etki: BR-28, BR-29, Faz 5 müdahale paneli ve düzeltme gösterimi, denetim çıktısı.
- Öneri: **Varsayılan:** yalnız Yönetici, gerekçeyle "Düzeltme" olayı ekler; düzeltilebilir alanlar: lot, görevli listesi, iş emri, açıklama; başlangıç ve bitiş zamanları ile adımın yapılıp yapılmadığı düzeltilemez; orijinal değer her yerde görünür kalır. **Alternatifler:** (a) sahip, kayıt tamamlanmadan önce kendi girdiği malzeme ve açıklamayı düzeltebilir; (b) hiç düzeltme yok, yalnız açıklayıcı not eklenir.
- Öncelik: Önemli

### K-17: Operatörün başkalarının kayıtlarını görme kapsamı
- Kaynak: doc.txt satır 97, 102-103
- Sorun: Satır 97 operatörün "kendi kayıtlarını görebilmesini" söylüyor; satır 102-103 ise "Ahmet'in kaydını Mehmet görebilir" diyor. Operatörün tüm kayıtları mı, kendi tesisindekileri mi, yalnız kendisinin sahip ya da görevli olduğu kayıtları mı göreceği belirsiz.
- Etki: FR-30, FR-38, §3.2 matrisi, Faz 4 kayıt detayı ve Faz 5 liste sorgusu.
- Öneri: **Varsayılan:** operatör kendi tesisindeki tüm kayıtları salt okunur görür; listede varsayılan filtre "benim kayıtlarım" (sahip ya da görevli); makine durum panosu herkese açıktır. **Alternatifler:** (a) operatör tüm tesislerin kayıtlarını görür; (b) yalnız sahip ya da görevli olduğu kayıtlar + makine durum panosu.
- Öncelik: Önemli
- **Not (2026-11-13):** K-20 varsayılanında kullanıcıya tesis ataması olmadığı için "kendi tesisi" tanımsızdır. Uygulanan varsayılan alternatif (a)'dır: operatör tüm kayıtları salt okunur görür, liste varsayılan filtresi "benim kayıtlarım" kalır (Decisions PL-08). K-20 alternatif (a) seçilirse tesis kısıtı eklenir.

### K-18: Üretim iş emri bilgisinin kaynağı ve zorunluluğu
- Kaynak: doc.txt satır 37, 120
- Sorun: İş emrinin elle mi girileceği, bir üretim sisteminden (ERP/MES) mi seçileceği, planlı/plansız kayıtlarda zorunlu olup olmadığı ve biçim kontrolü olup olmadığı belirtilmemiş.
- Etki: FR-11, Faz 3 sihirbaz; entegrasyon istenirse ayrı faz ve kapsam büyümesi.
- Öneri: **Varsayılan:** serbest metin iş emri no, planlı kayıtta zorunlu, plansız kayıtta isteğe bağlı; biçim kontrolü yok. **Alternatifler:** (a) zorunlu biçim (desen iş biriminden); (b) üretim sisteminden açık iş emirleri listesi (entegrasyon fazı).
- Öncelik: Önemli

### K-19: Tesis, hat, makine ve kullanıcı verisinin kaynağı
- Kaynak: eksik (ilgili: satır 99-100)
- Sorun: Tesis/hat/makine listesinin ve personel listesinin elle mi tanımlanacağı yoksa bir kaynak sistemden (ERP, bakım sistemi, İK) mi alınacağı belirtilmemiş. Mevcut kod sistemi (makine numaraları) kayıt no biçimini de etkiler (K-13).
- Etki: FR-01, FR-39, Faz 1 ekranları ve seed; entegrasyon istenirse ayrı faz.
- Öneri: **Varsayılan:** yönetici ekranlarından elle tanım; ilk kurulum için iş biriminin vereceği listeden seed. **Alternatifler:** (a) CSV ile toplu içe aktarım (ek FE/BE kalemi); (b) kaynak sistem entegrasyonu (ayrı faz).
- Öncelik: Önemli

### K-20: "Herhangi bir makineyi rastgele seçememeli" ifadesinin kapsamı
- Kaynak: doc.txt satır 13
- Sorun: Kısıtın yalnız "tanımlı listeden seçim" mi, yoksa kullanıcının yalnız kendi tesis/hattının makinelerini seçebilmesi mi (kullanıcı-tesis ataması) olduğu belirsiz.
- Etki: BR-01, BR-02, veri modeli (kullanıcı-tesis/hat ataması), Faz 1 kullanıcı formu, Faz 3 sihirbaz.
- Öneri: **Varsayılan:** kullanıcı tanımlı ve aktif makineler arasından, yalnız makineye eşlenmiş temizlik tiplerini seçebilir; kullanıcı-tesis ataması yok. **Alternatifler:** (a) kullanıcıya bir ya da birden çok tesis atanır, sihirbaz yalnız bunları gösterir; (b) kullanıcıya hat ataması.
- Öncelik: Önemli

### K-21: Vardiya bitiminde açık bırakılan adım
- Kaynak: doc.txt satır 55-56, 79-81
- Sorun: Yarım kalan kaydın açık kalması isteniyor; ama operatör bir adımı başlatıp bitirmeden vardiyadan ayrılırsa adım saatlerce "Devam ediyor" kalır, süre ve efor şişer, kişi kilitli kalır (başka işte adım başlatamaz). Adımın duraklatılması ya da yönetici tarafından kapatılması tanımlı değil.
- Etki: BR-19, BR-24, BR-25, K-02, Faz 4 ve Faz 5.
- Öneri: **Varsayılan:** adım duraklatılamaz; açık adımı yalnız sahip ya da görevli bitirebilir; `<UzunAdımEşiği>` aşan açık adım yönetici panosunda "müdahale bekliyor" listesine düşer; süre olduğu gibi kaydedilir ve raporda işaretlenir (eşik iş biriminden). **Alternatifler:** (a) duraklat/devam (K-02 seçenek b); (b) yönetici açık adımı gerekçeyle "Kesildi" yapar ve adım baştan yürütülür.
- Öncelik: Önemli
- **Not (2026-11-13):** K-38 kararıyla `<UzunAdımEşiği>` gerekmez: başladığı vardiya bitmiş açık adım ve açık mola yönetici panosunda "müdahale bekliyor" listesine düşer (SH-16). K-39 kararıyla adım sürerken kayıt molası verilebilir; adımın ayrıca duraklatılması yoktur. Yönetici açık molayı gerekçeyle bitirebilir.

### K-22: Mevzuat çerçevesi, elektronik imza, kontrol/onay adımı
- Kaynak: eksik (ilgili: satır 4, 123-124)
- Sorun: Denetimin hangi çerçevede yapıldığı belirtilmemiş (ör. GMP, gıda güvenliği standartları, elektronik kayıt ve imza düzenlemeleri). Bazı çerçeveler adım tamamlamada yeniden kimlik doğrulama (elektronik imza), temizlik sonrası ikinci kişi kontrolü ve iz kaydının periyodik gözden geçirilmesini bekler.
- Etki: NFR-03, NFR-09, Faz 4 (imza adımı), yeni bir onay fazı olasılığı, İÖ-09 ve İÖ-10.
- Öneri: **Varsayılan:** belirli bir mevzuat uyumu hedeflenmez; değiştirilemez olay günlüğü + özet zinciri + yazdırılabilir denetim çıktısı sağlanır; elektronik imza ve dört göz onayı yoktur. **Alternatifler:** (a) adım bitiminde PIN ile yeniden kimlik doğrulama; (b) temizlik sonrası kalite/vardiya sorumlusu onayı ve "üretime açıldı" durumu (ayrı faz).
- Öncelik: Önemli
- **Not (2026-11-13):** Alternatif (a) K-09 kararıyla kapsama girdi: kritik işlemler PIN ile onaylanır. Belirli bir mevzuat uyumu ve dört göz onayı hâlâ kapsam dışı.

### K-23: Bütünlük kanıtının düzeyi (dış çapa, BT erişimi)
- Kaynak: doc.txt satır 122, 131
- Sorun: "Sonradan değiştirilip değiştirilmediğini anlayabilmemiz" isteniyor. Uygulama üzerinden değişiklik engellenebilir; ancak veritabanı dosyasına doğrudan erişimi olan biri (ör. sunucu yöneticisi) için yalnız tespit mümkündür. Tespitin kimden korunacağı (yalnız uygulama kullanıcıları mı, sunucu yöneticisi de mi) belirtilmemiş.
- Etki: NFR-03, BR-32, özet anahtarının yönetimi (kimde, nerede), Faz 3 altyapısı, Faz 6 doğrulama ekranı.
- Öneri: **Varsayılan:** HMAC anahtarlı kayıt başına özet zinciri; anahtar veritabanı ve `appsettings.json` dışında, sunucu ortam değişkeninde. **Alternatifler:** (a) ek olarak günlük zincir başı özetlerinin dışa aktarılıp ayrı bir yerde (ör. kalite biriminde) saklanması; (b) anahtarsız düz özet zinciri (yalnız kaza ve uygulama içi değişikliğe karşı).
- Öncelik: Önemli

### K-24: Versiyon yayın onayı, yürürlük tarihi, doküman numarası
- Kaynak: doc.txt satır 25-33
- Sorun: Checklist dokümanlarının kurumda kontrollü doküman olup olmadığı (doküman no ve revizyon no), yeni versiyonun yayınlanması için ikinci bir onay gerekip gerekmediği ve versiyonun yayın anında mı ileri bir tarihte mi yürürlüğe gireceği belirtilmemiş.
- Etki: FR-09, BR-03, BR-04, Faz 2 yayınlama akışı ve veri modeli (onaylayan, yürürlük tarihi, doküman no alanları).
- Öneri: **Varsayılan:** yöneticinin yayınladığı anda yürürlüğe girer, tek onay, versiyon numarası sistem tarafından artırılır; isteğe bağlı "doküman/revizyon no" metin alanı. **Alternatifler:** (a) ikinci yöneticinin onayıyla yayın; (b) ileri tarihli yürürlük.
- Öncelik: Önemli

### K-25: Açık kaydı olan makinenin kullanımdan kaldırılması
- Kaynak: doc.txt satır 30
- Sorun: Makinenin kullanımdan kaldırılabileceği söyleniyor; o anda makinede açık ya da devam eden temizlik varsa ne olacağı belirtilmemiş. Aynı soru, bir temizlik tipinin pasifleştirilmesi için de geçerli.
- Etki: BR-35, Faz 1 makine formu, Faz 2 tip ekranı.
- Öneri: **Varsayılan:** açık ya da devam eden kaydı olan makine ve tip kullanımdan kaldırılamaz; ekran engelleyen kaydı gösterir. **Alternatif:** kaldırma yapılır, yalnız yeni kayıt engellenir, açık kayıt tamamlanabilir.
- Öncelik: Önemli

### K-26: Medya biçimi, boyutu ve yükleme sorumluluğu
- Kaynak: doc.txt satır 16
- Sorun: Fotoğraf/video için biçim, azami boyut ve süre, kimin yükleyeceği, sahadaki ağın video oynatmaya uygun olup olmadığı belirtilmemiş. Mevcut medya (kurum eğitim videoları) varsa kaynağı bilinmiyor.
- Etki: FR-06, NFR-10, Faz 2 medya yükleme, Faz 4 medya görüntüleyici, sunucu disk ihtiyacı.
- Öneri: **Varsayılan:** yalnız Yönetici yükler; fotoğraf JPEG/PNG/WebP, video MP4 (H.264); azami boyut iş birimi ve BT ile belirlenir; dosyalar sunucu diskinde, içerik özetiyle adlandırılır. **Alternatifler:** (a) yalnız fotoğraf, video bağlantısı kurum içi bir adrese; (b) kurum doküman sistemine bağlantı.
- Öncelik: Önemli

### K-27: Adım sonunda veri toplama (sonuç, ölçüm, fotoğraf kanıtı)
- Kaynak: eksik (ilgili: satır 4 "hangi adımları uyguladığını", 128)
- Sorun: Adımın yalnız başlatılıp bitirilmesi mi yeterli, yoksa sonuç (uygun/uygun değil), ölçüm değeri, operatörün çektiği kanıt fotoğrafı ya da not alınması mı gerekiyor? Doküman yalnız zaman ve kişi bilgisini anlatıyor.
- Etki: Veri modeli (`RecordSteps` ek alanları ya da ayrı tablo), Faz 4 adım kartı, denetim çıktısı.
- Öneri: **Varsayılan:** adım yalnız başlat/bitir + isteğe bağlı kısa not; sonuç, ölçüm ve kanıt fotoğrafı yok. **Alternatifler:** (a) adım tanımında "sonuç zorunlu" bayrağı ve uygun/uygun değil seçimi; (b) operatörün çektiği kanıt fotoğrafı (sahada kamera, depolama ihtiyacı artar).
- Öncelik: Önemli

### K-28: Saklama süresi ve denetim çıktısının biçimi
- Kaynak: doc.txt satır 131 (süre ve biçim eksik)
- Sorun: Kayıtların ve medyanın ne kadar süre saklanacağı, denetçiye hangi biçimde sunulacağı (ekranda gösterim, yazdırma, PDF, Excel), imzalı/mühürlü çıktı gerekip gerekmediği belirtilmemiş.
- Etki: FR-35, NFR-13, Faz 6 denetim çıktısı ve dışa aktarım, yedekleme planı (BT).
- Öneri: **Varsayılan:** kayıtlar süresiz saklanır (silme özelliği yok); denetim çıktısı yazdırılabilir HTML ("Yazdır / PDF olarak kaydet") ve CSV. **Alternatifler:** (a) sunucu tarafı PDF üretimi (üçüncü parti kütüphane ve lisans incelemesi); (b) Excel biçimi.
- Öncelik: Önemli

### K-29: Yönetici sahada iş yapabilir mi, çoklu rol
- Kaynak: doc.txt satır 96-100
- Sorun: "Operatör ile yöneticinin aynı şeyleri görmesini istemiyoruz" deniyor; yöneticinin (ör. vardiya amiri) kendisinin temizlik kaydı açıp açamayacağı ya da yardımcı olarak adımda çalışıp çalışamayacağı belirtilmemiş.
- Etki: §3.2 matrisi, `Roles` sabitleri, Faz 1 kullanıcı formu (çoklu rol seçimi), Faz 3 ve Faz 4 yetki.
- Öneri: **Varsayılan:** bir kullanıcıya birden çok rol atanabilir; sahada çalışan yöneticiye `Operator` rolü de verilir; yalnız `Manager` rolüyle kayıt açılamaz. **Alternatif:** `Manager` rolü sahadaki tüm işlemleri de kapsar.
- Öncelik: Önemli

### K-30: Sahiplik kontrolünün "yalnız rol bazlı yetki" kuralıyla uyumu (süreç)
- Kaynak: doc.txt satır 102-104; `CLAUDE.md` kural 5, `Docs/Process/CodingStandards.md` §4
- Sorun: Proje kuralları policy, `IAuthorizationHandler` ve "claim tabanlı özel kontrol"ü yasaklıyor. Satır 104 ise rolden bağımsız, veriye bağlı bir kural istiyor (yalnız sahip ya da görevli). Bu kuralın bir yetki politikası değil iş kuralı olarak servis katmanında uygulanması öneriliyor; servis, oturumdaki kullanıcının kimliğini (UserId) parametre olarak alacak. Bu yorumun kural ihlali sayılmadığının koordinatör ve kullanıcı tarafından onaylanması gerekir; aksi halde qa-tester denetiminde bulgu çıkabilir.
- Etki: NFR-06, BR-16, Faz 4 servis tasarımı ve QA denetim kuralları.
- Öneri: **Varsayılan:** rol kontrolü yalnız `[Authorize(Roles = ...)]`; sahiplik kontrolü servis metodunda veri sorgusuyla (`OwnerUserId` ve görevli listesi); controller yalnız `UserId`'yi okuyup geçirir; `AddPolicy`, `IAuthorizationHandler` kullanılmaz. Onay `Decisions.md`'ye işlenir. **Alternatif:** yok (policy tabanlı kaynak yetkisi proje kurallarına aykırıdır).
- Öncelik: Önemli
- **Karar (2026-11-13):** Kullanıcı onayladı. Sahiplik kontrolü servis katmanında `UserId` ile yapılan bir iş kuralıdır, yetki politikası değildir. Controller oturumdaki kullanıcının `UserId` değerini servise geçirir; servis kuralı ihlal eden isteği iş hatası olarak döner. `Docs/Process/CodingStandards.md §4`'e yazıldı.

---

## Sonra netleşebilir

### K-31: Dil ihtiyacı
- Kaynak: eksik
- Sorun: Sahada Türkçe dışında dil konuşan personel olup olmadığı belirtilmemiş.
- Etki: NFR-11; çok dil istenirse tüm görünümlerde kaynak dosyası altyapısı ve prosedür metinlerinin çevirisi (veri modeli) gerekir.
- Öneri: **Varsayılan:** yalnız Türkçe. **Alternatif:** arayüz çok dilli, prosedür metinleri tek dil; ya da prosedür metinleri de çok dilli (ayrı faz).
- Öncelik: Sonra netleşebilir

### K-32: Sistem saati ve saat dilimi
- Kaynak: doc.txt satır 55-56 (saat dilimi eksik)
- Sorun: Tüm tesislerin aynı saat diliminde olup olmadığı ve sunucu saatinin bir zaman sunucusuyla eşitlenip eşitlenmediği belirtilmemiş. Sunucu saati kayarsa tüm kayıtların zaman kanıtı etkilenir.
- Etki: NFR-01, gösterim katmanı, raporlarda gün sınırı.
- Öneri: **Varsayılan:** UTC saklama, Türkiye saatiyle gösterim; sunucunun zaman sunucusuyla eşitlenmesi BT sorumluluğunda. **Alternatif:** tesis bazında saat dilimi alanı.
- Öncelik: Sonra netleşebilir

### K-33: Ölçek ve SQLite uygunluğu
- Kaynak: eksik (ilgili: satır 73 "aynı anda çok fazla kişi çalışıyor")
- Sorun: Tesis, hat, makine, kullanıcı sayısı ve aynı anda işlem yapan kişi sayısı belirtilmemiş. SQLite tek yazarlıdır; kısa transaction'larla orta düzey eşzamanlı yazmayı karşılar, ancak çok yüksek eşzamanlılıkta bekleme süreleri uzar.
- Etki: NFR-02, teknik yığın seçimi (SQLite proje kısıtıdır), performans kabul kriterleri.
- Öneri: **Varsayılan:** SQLite, WAL modu, kısa transaction; iş biriminden sayılar geldiğinde ölçüm yapılır. **Alternatif:** ölçek SQLite'ı aşıyorsa veritabanı değişikliği ayrı bir karar olarak kullanıcıya sunulur.
- Öncelik: Sonra netleşebilir

### K-34: Periyodik temizliklerin planlanması kapsamda mı
- Kaynak: doc.txt satır 3
- Sorun: Temizliğin "ürün değiştiğinde ya da belirli aralıklarla" yapıldığı söyleniyor; sistemin hangi makinede temizliğin geldiğini/geciktiğini takip edip etmeyeceği belirtilmemiş.
- Etki: Kapsam; istenirse ayrı faz (İÖ-08).
- Öneri: **Varsayılan:** kapsam dışı; sistem yalnız yapılan temizliği kaydeder. **Alternatif:** makine-tip bazında periyot tanımı ve geciken temizlik listesi (ayrı faz).
- Öncelik: Sonra netleşebilir

### K-35: "Kim başlatmış" sorusunun anlamı
- Kaynak: doc.txt satır 111
- Sorun: "Kim başlatmış" kaydı açan kişi mi (sahip), yoksa ilk adımı fiilen başlatan kişi mi? Sahip bir adımdan çıkarılabildiğinde (V-11) bu ikisi farklı olabilir.
- Etki: FR-31, kayıt detayı ve denetim çıktısı.
- Öneri: **Varsayılan:** ikisi ayrı gösterilir: "Kaydı açan (sorumlu)" ve "İlk adımı başlatan". **Alternatif:** yalnız sahip.
- Öncelik: Sonra netleşebilir

### K-36: Rapor içerikleri ve üretim planlama ihtiyacı
- Kaynak: doc.txt satır 7-8, 64
- Sorun: Raporların kırılımları (makine, hat, tesis, tip, faz, personel, vardiya), dönemleri ve üretim planlamanın hangi göstergeyi kullanacağı belirtilmemiş. Personel bazlı süre/efor raporu istenip istenmediği (çalışan performansı izleme hassasiyeti) belli değil.
- Etki: FR-36, FR-40, Faz 6 ekranları.
- Öneri: **Varsayılan:** makine ve faz bazında ortalama net süre, brüt süre, efor ve minimum süre uyumu; tarih aralığı filtresi; personel bazlı rapor yok. **Alternatif:** personel ve vardiya kırılımı eklenir.
- Öncelik: Sonra netleşebilir

### K-37: Bildirim ihtiyacı
- Kaynak: eksik (ilgili: satır 94)
- Sorun: Yöneticinin müdahale gerektiren kayıtlardan (kilit süresi dolmuş, pasif sahipli, uzun açık adım) nasıl haberdar olacağı belirtilmemiş.
- Etki: Faz 5 yönetici panosu; e-posta ya da anlık bildirim istenirse ek altyapı.
- Öneri: **Varsayılan:** bildirim yok; yönetici panosunda "müdahale bekleyenler" listesi. **Alternatif:** e-posta bildirimi (SMTP altyapısı ve ayar gerekir).
- Öncelik: Sonra netleşebilir
- **Not (2026-11-13):** K-14 kararıyla uygulama içi bildirim kapsama girdi (acil müdahale ve yeni talep). Kanal sorusu K-43'e taşındı.

---

## Yanıtlardan doğan sorular (2026-11-13)

K-38..K-40 Engelleyiciydi; 2026-11-13'te karara bağlandı. Diğerlerinin önerilen varsayılanı kabul edildi, K-47 kullanıcı yanıtıyla değişti.

### K-38: Vardiya tanımı ve vardiya bitimine denk gelen kilit
- Kaynak: K-03 yanıtı; belgede vardiya yalnız satır 79'da geçer, tanımı yok.
- Sorun: Kullanıcı, başlamamış kaydın kilit süresi vardiya bitimine denk gelirse buna uygun davranılmasını istiyor. Sistemde vardiya kavramı yok: vardiyaların nerede tanımlanacağı (tesis mi, hat mı) ve bitime denk gelen kilidin ne olacağı belirsiz.
- Etki: Yeni tanım ekranı ve tablo (PhasePlan Faz 2), SH-13, BR-26, BR-45, Faz 4 kilit hesabı; K-21 için de kullanılır.
- Öneri: **Varsayılan:** vardiyalar tesis bazında yönetici tarafından tanımlanır (ad, başlangıç saati, bitiş saati; gece yarısını geçen vardiya desteklenir; aynı tesiste vardiyalar çakışmaz). Kilit bitişi = en erken (açılış + kilit süresi, açılış anını içeren vardiyanın bitişi). Vardiya sonunda açılıp başlanmayan kayıt sonraki vardiyanın makinesini kilitlemez. Tesiste vardiya tanımlı değilse yalnız kilit süresi uygulanır. **Alternatifler:** (a) vardiya bitimi kilide etki etmez; (b) kilit, sonraki vardiyanın başlangıcından `<EkSüre>` sonra düşer (sonraki vardiya kaydı devralmaz, yalnız ilk dakikalarda makine korunur); (c) vardiya hat bazında tanımlanır.
- Öncelik: Engelleyici (Faz 2 şeması ve SH-13)
- **Karar (2026-11-13):** Kullanıcı kararı analiste bıraktı ("kararlaştır"). Önerilen varsayılan şu ince ayarlarla uygulanır. Vardiyalar tesis bazında yönetici tarafından tanımlanır: ad, başlangıç ve bitiş saati (Türkiye saati); bitiş başlangıçtan küçükse vardiya ertesi gün biter; aynı tesiste aktif vardiyalar çakışmaz; pasif vardiya hesaba girmez. Bir an, başlangıcı dahil bitişi hariç aralığına düştüğü vardiyaya aittir (bitiş anı sonraki vardiyanındır). Kilit bitişi = en erken (açılış + kilit süresi, açılış anını içeren vardiyanın bitişi); tesiste aktif vardiya yoksa ya da açılış iki vardiya arasındaki boşluğa düşerse açılış + kilit süresi (SH-13). Kilit bitişi açılışta hesaplanıp kayıtta saklanır; vardiya tanımı sonradan değişse de açık kaydın kilit bitişi değişmez. SH-16 aynı eşleştirmeyi kullanır. Gerekçe: vardiya sonunda açılıp başlanmayan kayıt sonraki vardiyanın makinesini kilitlemez; operatörün ekranda gördüğü kilit bitişi sonradan kaymaz.

### K-39: Molanın kapsamı: kayıt mı kişi mi, adım sürerken mola
- Kaynak: K-02 yanıtı; belge satır 67-71.
- Sorun: Kullanıcı molanın başlangıç ve bitişini operatörün belirtmesini ve mola açıkken kişinin işlem yapamamasını istedi. (a) Adımda birden çok görevli varken biri molaya çıkarsa kayıt mı durur, yalnız o kişinin eforu mu düşer? (b) Devam eden bir adımın içinde mola verilebilir mi? (b) evetse adım süresi ve efor formülü de değişir.
- Etki: SH-01, SH-02, SH-04, SH-06, SH-11, BR-38..BR-40, `RecordBreaks` tablosu, Faz 5 yürütme ekranı.
- Öneri: **Varsayılan:** mola kayıt düzeyindedir. Yalnız "Devam ediyor" kayıtta ve devam eden adım yokken (iki adım arasında) verilir; kaydın sahibi ya da sıradaki adımın görevlisi başlatır. Mola açıkken kayıtta kimse adım başlatamaz, görevli ya da malzeme değiştiremez; molayı başlatan kişi sistemin hiçbir yerinde yazma işlemi yapamaz. Molayı başlatan kişi bitirir; bitiremezse yönetici gerekçeyle bitirir. Adım içinde mola olmadığı için adım süresinden ve eforundan bir şey düşülmez. Diğer görevliler çalışmaya devam edecekse mola verilmez; molaya çıkacak kişi sıradaki adımın görevli listesinden çıkarılır. **Alternatifler:** (a) kişi düzeyi mola: kayıt durmaz, net süre değişmez, yalnız kişinin eforu düşer; (b) adım içinde mola: adım net süresi = adım süresi - adım içindeki molalar, efor buna göre.
- Öncelik: Engelleyici (çekirdek süre kuralı)
- **Karar (2026-11-13):** Kullanıcı: "temizlik sürerken mola verebilmeli". Önerilen "yalnız iki adım arasında" reddedildi; ayrıntıyı analist belirledi:
  - Kapsam: mola kayıt düzeyindedir; açıkken o kayıtta çalışma durmuş sayılır. Kişi düzeyi mola yoktur; diğer görevliler çalışmaya devam edecekse mola verilmez (K-02 tanımı kayıt netinden düşmeyi gerektirir).
  - Ne zaman: kayıt "Devam ediyor" iken; iki adım arasında ya da bir adım devam ederken.
  - Kim başlatır: kaydın sahibi ya da o anki adımın (devam eden adım, yoksa sıradaki adım) görevlisi, PIN ile. Başlatan kişi o anda başka bir kayıtta devam eden adımda görevli olamaz.
  - Kim bitirir: yalnız başlatan kişi, PIN ile. Bitiremezse yönetici gerekçe ve PIN ile bitirir, bitiş yöneticinin işlem anıdır (Faz 7). Kayıt iptal edilir ya da yarıda kapatılırsa mola müdahale anında kapanır.
  - Mola açıkken: devam eden adım "Devam ediyor" kalır, yeni durum yoktur; kayıtta adım başlatılamaz ve bitirilemez, görevli ve malzeme değişmez; ekrandaki adım, faz ve kayıt sayaçları durur. Molayı başlatan kişi sistemin hiçbir yerinde yazma işlemi yapamaz (K-02). Devam eden adımın diğer görevlileri başka kayıtta adım başlatamaz, çünkü hâlâ o adımdadır (BR-24).
  - Süre: adım molası = adımla kesişen mola süresi (SH-17); adım süresi = adım bitişi - adım başlangıcı - adım molası (SH-01); adım eforu = adım süresi x görevli sayısı (SH-02). Faz molası (SH-04) ve kayıt molası (SH-09) aynı molaları içerir. Mola açıkken adım bitirilemediği ve adım başlatılamadığı için her mola ya bütünüyle bir adımın içinde ya da bütünüyle iki adım arasındadır. Bu yüzden faz net = adım sürelerinin toplamı + adımlar arasındaki molasız boşluklar; faz minimumu (SH-07) adım süreleriyle tutarlıdır.

### K-40: Talebi hangi yönetici görür ve karara bağlar
- Kaynak: K-05 yanıtı ("operatör yöneticisine talep açabilmeli").
- Sorun: "Yöneticisine" ifadesi her operatörün belirli bir yöneticisi olduğunu düşündürüyor. Sistemde kullanıcı-yönetici bağı yok. Talebi Yönetici rolündeki herkes mi görür, yalnız kişinin bağlı olduğu yönetici mi?
- Etki: `Users` tablosu (yönetici alanı), Faz 1 kullanıcı formu, Faz 7 talep sorgusu, Faz 8 bildirim alıcıları.
- Öneri: **Varsayılan:** Yönetici rolündeki tüm kullanıcılar tüm talepleri görür ve karara bağlar; ilk karar geçerlidir, ikinci karar denemesi "talep zaten karara bağlandı" mesajı alır. **Alternatifler:** (a) kullanıcı formunda "bağlı olduğu yönetici" alanı; talep yalnız o yöneticiye düşer, diğerleri salt okunur görür (yetki rol bazlı kalır, bağ kontrolü servis katmanında iş kuralıdır); (b) yöneticiye tesis ataması, talep tesisin yöneticilerine düşer.
- Öncelik: Engelleyici (Faz 1 şeması)
- **Karar (2026-11-13):** Önerilen varsayılan kabul edildi. Yönetici rolündeki tüm aktif kullanıcılar tüm talepleri görür ve karara bağlar; ilk karar geçerlidir, ikinci karar denemesi "Talep zaten karara bağlandı" mesajı alır. Kullanıcıya yönetici alanı eklenmez. Süper admin (PL-05) talep görmez ve bildirim almaz.

### K-41: Kendi kaydı devam ederken başka kayıtta yardım
- Kaynak: satır 76-77, K-08 kararı.
- Sorun: Ahmet'in kendi kaydı "Devam ediyor" (iki adım arasında, mola yok). Ahmet bu sırada Mehmet'in kaydında bir adımda görevli olabilir mi? Belge yalnız eşzamanlı adımı yasaklıyor. İzin verilirse yardım süresi, mola verilmediği için Ahmet'in kaydında çalışma sayılır.
- Etki: BR-24, BR-37, SH-11, Faz 5 görevli ekleme kontrolü.
- Öneri: **Varsayılan:** izin verilir; yardımcı onay ekranında "Kendi kaydınız devam ediyor; mola vermediğiniz süre o kayıtta çalışma sayılır" uyarısı gösterilir. **Alternatif:** kendi kaydı Devam ediyor olan kişi başka kayıtta görevli olamaz; önce beklemeye alma talebi gerekir.
- Öncelik: Önemli

### K-42: PIN biçimi ve hatalı deneme kuralı
- Kaynak: K-09 yanıtı.
- Sorun: PIN uzunluğu, yalnız rakamdan oluşup oluşmadığı, art arda hatalı denemede ne olacağı ve PIN'i kimin sıfırlayacağı belirtilmedi.
- Etki: BR-49, Faz 1 ilk giriş ve kullanıcı yönetimi, tüm PIN'li işlemler.
- Öneri: **Varsayılan:** yalnız rakam, `<PinUzunluğu>` hane; tüm haneleri aynı ya da ardışık PIN reddedilir; `<PinDenemeSınırı>` art arda hatalı denemede PIN kilitlenir ve oturum kapanır; yönetici PIN'i sıfırlar, kullanıcı sonraki girişte yeni PIN belirler; kullanıcı eski PIN'iyle kendi PIN'ini değiştirebilir. Değerler iş birimi ve BT'den. **Alternatif:** süreli kilit (belirli süre sonra kendiliğinden açılır).
- Öncelik: Önemli
- **Not (2026-11-13):** Varsayılan kabul edildi. "Yönetici sıfırlar" PL-05 ile şöyle okunur: Operatör ve Denetçi hesabını Yönetici ya da Süper admin, Yönetici ve Süper admin hesabını yalnız Süper admin sıfırlar.
- **Not (2026-11-13):** Kullanıcı değerleri verdi: `<PinUzunluğu>` 4 hane, `<PinDenemeSınırı>` 5; art arda 5. hatalı denemede PIN kilitlenir, doğru PIN sayacı sıfırlar. Değerler seed'e girer, Sistem ayarlarından değişir (PL-24, Requirements §6).

### K-43: Bildirim kanalı
- Kaynak: K-14 yanıtı ("aciliyet durumu bildirimi olabilir"), K-37.
- Sorun: Acil müdahale açıldığında ve talep açıldığında yöneticinin nasıl haberdar olacağı belirtilmedi: yalnız uygulama içi mi, e-posta ya da SMS de mi?
- Etki: FR-52, BR-51, Faz 8; e-posta ya da SMS seçilirse sunucu ayarı ve dış servis gerekir, Faz 8'e kalem eklenir.
- Öneri: **Varsayılan:** yalnız uygulama içi: yönetici kabuğunda okunmamış bildirim sayacı, bildirim listesi, yönetici panosunda acil müdahaleler bölümü. **Alternatifler:** (a) e-posta (SMTP ayarı BT'den); (b) SMS (dış servis ve sözleşme).
- Öncelik: Önemli

### K-44: Acil müdahalenin makine kilidi ve tek aktif kayıt kuralıyla ilişkisi
- Kaynak: K-14 kararı, satır 42, 75.
- Sorun: Makinede başlamamış (Açıldı) bir kayıt varken ya da operatörün kendi kaydı devam ederken acil müdahale gerekirse ne olacak? Kurallar aynen uygulanırsa acil müdahale gecikebilir.
- Etki: BR-23, BR-37, BR-50, Faz 8.
- Öneri: **Varsayılan:** kurallar acilde de aynen uygulanır; çakışma ekranı kilidi tutan kaydı, sahibini ve durumunu gösterir. Kendi kaydı devam eden operatörün yerine başka bir operatör acili açar. **Alternatif:** acil açılışı makinedeki başlamamış (Açıldı) kaydı "Zaman aşımı"na düşürür; acil kayıt tek aktif kayıt kuralından muaftır.
- Öncelik: Önemli

### K-45: Beklemedeki kaydın kilit, süre ve devam etkisi
- Kaynak: K-08 yanıtı.
- Sorun: Beklemeye alınan kaydın makine kilidi, süre hesabı, zaman aşımı ve nasıl devam ettirileceği belirtilmedi.
- Etki: BR-43, BR-44, SH-05, SH-10, SH-11, Faz 7.
- Öneri: **Varsayılan:** yalnız "Devam ediyor", devam eden adımı ve açık molası olmayan kayıt beklemeye alınır; makine kilidi kalır (makine yarı temizdir); kayıt sahibin tek aktif kayıt kısıtına sayılmaz; bekleme süresi net süreden düşülür (Net = Brüt - mola - bekleme); zaman aşımı uygulanmaz, kayıt kendiliğinden kapanmaz; sahip, başka aktif kaydı yoksa PIN ile "Devam et" der, onay gerekmez; beklemedeki kayıtlar yönetici panosunda listelenir. **Alternatifler:** (a) devam ettirmek de yönetici onayı ister; (b) beklemede makine kilidi kalkar.
- Öncelik: Önemli

### K-46: Kilit süresinin üst sınırı ve süreyi uzatan işlem
- Kaynak: K-03 yanıtı.
- Sorun: Operatörün kayıt açarken verdiği süreye üst sınır yoksa bir kişi makineyi uzun süre kilitleyebilir. "Hiçbir işlem yapılmadığı takdirde" ifadesinde görevli ya da malzeme eklemenin süreyi uzatıp uzatmadığı belirsiz.
- Etki: BR-45, BR-46, SH-13, Faz 3 tip formu, Faz 4 sihirbaz.
- Öneri: **Varsayılan:** operatör tipin varsayılanından farklı süre girebilir ama `<AzamiKilitSüresi>` (sistem ayarı) aşılamaz; süre açılış anından sayılır; yalnız ilk adımın başlatılması kilidi kalıcı yapar; görevli ya da malzeme eklemek süreyi uzatmaz. **Alternatif:** her işlem sayacı yeniden başlatır (kilit bitişi = son işlem + kilit süresi).
- Öncelik: Önemli
- **Not (2026-11-14):** `<AzamiKilitSüresi>` başlangıç değeri kullanıcıdan geldi: 120 dakika (PL-34). Satır Faz 3 seed eklemesiyle gelir (PL-25).

### K-47: Sistem geneli ayarların yeri
- Kaynak: Brief V-15.
- Sorun: `<OturumSüresi>`, `<PinUzunluğu>`, `<PinDenemeSınırı>`, `<AzamiKilitSüresi>` gibi sistem geneli değerlerin ekrandan mı, sunucu ayar dosyasından mı değiştirileceği belirtilmedi.
- Etki: Ek ayar ekranı ya da `appsettings.json`.
- Öneri: **Varsayılan:** sistem geneli değerler sunucu ayar dosyasındadır, BT değiştirir; tip bazlı değerler (varsayılan kilit süresi, faz minimum süresi, malzeme zorunluluğu) yönetici ekranındadır. **Alternatif:** yönetici ayar ekranı (Faz 9'a bir ekran eklenir).
- Öncelik: Sonra netleşebilir
- **Karar (2026-11-13):** Kullanıcı: değerler "yönetici ayarlarından değiştirilebilir olmalı"; alternatif seçildi. `<OturumSüresi>`, `<PinUzunluğu>`, `<PinDenemeSınırı>`, `<AzamiMedyaBoyutu>` (fotoğraf ve video ayrı) ve `<AzamiKilitSüresi>` veritabanındaki sistem ayarları tablosundadır. Yönetici ve Süper admin bunları Sistem ayarları ekranından (E-33, Faz 9) PIN ile değiştirir; her değişiklik eski değer, yeni değer, yapan ve zamanla geçmişe yazılır. Tablo Faz 1'de oluşur ve seed'den dolar (PL-17); değişikliğin etkisi PL-18. Kullanıcı değer vermedi: değerler yer tutucu kalır, uydurma değer yazılmaz; ilgili fazdan önce gerekir (Requirements §9). Süper admin kurulumda değerleri ilk yapılandıran kişidir (seed'de tek kullanıcı olduğu için ekran yetkisi ona da verildi).
- **Not (2026-11-13):** Faz 1 başlangıç değerleri kullanıcıdan geldi: `<OturumSüresi>` 30 dakika, `<PinUzunluğu>` 4 hane, `<PinDenemeSınırı>` 5 (PL-24). `<AzamiMedyaBoyutu>` ve `<AzamiKilitSüresi>` Faz 3 öncesi açık; satırları değer gelince eklenir (PL-25). Değerlerin tek yeri Requirements §6.
- **Not (2026-11-14):** `<AzamiKilitSüresi>` 120 dakika geldi (PL-34). `<AzamiMedyaBoyutu>` (fotoğraf, video) kullanıcı kararıyla Faz 3 öncesine kadar açık.

### K-48: Denetçi rolü
- Kaynak: Brief §3.1 (öneri), satır 4, 123.
- Sorun: Denetim ve kalite için salt okunur bir rol önerildi; onaylanmadı.
- Etki: `Roles` sabitleri (Faz 1), Requirements §3 matrisi, Faz 6 ve Faz 9 erişimleri.
- Öneri: **Varsayılan:** `Auditor` rolü vardır: kayıtları, yönetici panosunu, raporları, denetim çıktısını ve bütünlük ekranını salt okunur görür. **Alternatif:** rol yok, bu yetkiler Yönetici'de kalır.
- Öncelik: Önemli

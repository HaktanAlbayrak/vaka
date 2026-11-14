DEVİR: backend-engineer -> koordinatör (qa-tester)   Faz: 2 (tamamlandı)   Dosya: Docs/Handover/backend-engineer-20261115-0030.md

Yapıldı:
- Altı servis uygulaması DbContext'ten: `FacilityService`, `LineService`, `ShiftService`, `MachineService`, `MachineRetirement`, `MaterialService`, `MaterialLotService`; `Program.cs` kayıtları eklendi. `IShiftLocator` silindi, kayıtsız (PL-40).
- Kurallar: BR-54 (kod boş/uzunluk/tire, benzersizlik hem önceden hem `DbUpdateException` ile, NOCASE), PL-04/PL-37 (kod ve hat değişimi alan hatası; makinede farklı `FacilityId` de `LineId` hatası), BR-05 (pasif tesise hat/vardiya, pasif hatta makine eklenemez), K-38 vardiya çakışması (`DoShiftsOverlap`, yalnız aktif, mesajda çakışan vardiyanın adı; pasif kayıt kontrole girmez; başlangıç = bitiş reddedilir), BR-12 SKT geçmiş yalnız `IsLotExpired` ile (liste sayacı ve ayrıntı rozeti), retire (BR-35: `IsActive=false`, `RetiredAtUtc`, engel sağlayıcı boş; PIN yok, PL-37). Listeler `AsNoTracking` + projeksiyon, `LIKE`, sunucuda sayfalama (20, `/machines` ve `/materials`).
- Koordinatör ekleri: makine ekleme GET'i `facilityId, lineId, code, name` okuyup forma geri bağlar (hat tesisle uyuşmuyorsa boş); makine listesinde tesise ait olmayan `lineId` sessizce yok sayılır. Hat-tesis uyuşmazlığında POST `LineId` hatası verir, form gönderilen tesisin aktif hatlarıyla çizilir, kayıt yok. Tüm liste/ayrıntı VM'lerinde `StatusMessage` dolar.
- K9: Faz 2 metotlarında (`EndsNextDay`, `DoShiftsOverlap`, `FindShiftAt`, `IsLotExpired`) formül ve K-38/BR-12 kimliği XML yorumunda; controller'larda saat/tarih karşılaştırması ve `TimeSpan` yok (grep).

Doğrulama (kopya DB `.qa/be/f2.db`, geliştirme DB'sinden kopya, port 5331, kapatıldı; kopyada 0001'in zorunlu değişim bayrakları kapatıldı ve Yönetici 2001, Operatör 2002, Denetçi 2003 kullanıcıları eklendi, parola 0001'inkiyle aynı hash; dosyalara parola yazılmadı):
- `dotnet build Src/Vaka.Web -warnaserror -o .qa/be/build`: 0 uyarı 0 hata. §4 grep, rol metni grep, em/en-dash: 0 eşleşme.
- 21 GET adresi (`/material-lots/create?materialId=` ve `/material-lots/edit/{id}` dahil, PL-41): oturumsuz 302 giriş; Süper admin, Operatör, Denetçi 302 `/account/access-denied` -> 403; Yönetici hepsinde 200.
- K1: tesis kodu `T1` ve `t1` yinelenen, `A-B` tireli reddedildi; aynı tesiste `H1`, aynı hatta `m01` yinelenen reddedildi; yeni tesis, hat, makine eklendi. K2: tesis, hat, makine kodu değişimi alan hatası; makinede başka hat ve başka tesis `LineId` hatası; adsal düzenleme 302. K3: `GET /machines/create?facilityId=1&lineId=3&code=M77&name=...` kod ve adı geri bağlar, hat seçimi boş; `/machines/lines?facilityId=1` JSON `[{id,label}]`; hat başka tesisin ise POST `LineId` hatası. K4: 14-16 çakışması, 22-07 gece vardiyası çakışması, 10:00=10:00, boş/çözülemeyen saat Türkçe hata; 15-23 yeniden kaydı (bitiş=başlangıç sınırı) 302; pasif vardiya 302; liste "Ertesi gün biter" gösterir. K5: retire 302, ikinci deneme "zaten kaldırılmış" reddi, `showRetired` ile listede, normal listede yok. K6: seed lot `L2026-001` ve dünkü lot "SKT geçmiş", bugünkü lot "Kullanılabilir". Malzeme kodu `mlz001` ve lot no yinelenen reddedildi, tireli malzeme kodu serbest, boş SKT reddedildi. Pasif tesise hat/vardiya eklenemez.
- Liste: `/machines?facilityId=1&lineId=3` tesise ait olmayan hattı yok sayıp tesis filtresiyle 200 döndü.
- G5: antiforgery'siz POST için `/lines`, `/machines`, `/materials`, `/shifts`, `/material-lots` create ve `/users/create` 400 döndü. `Code=...` içeren gövdede curl 8 "Unsupported HTTP/1 subversion" ile 000 verdi (`/users/create` dahil, Faz 1 adresinde de aynı); Kestrel yanıtı `Name=Y` gövdesinde 400. Yanıt başlığı sorunu araca özgü olabilir; qa-tester tarayıcı ya da başka istemciyle doğrulasın.

Frontend/QA için:
- 768 altı ve ray yerleşimi dahil görünüm doğrulaması benim kapsamım değil.
- Retire POST'unda gövde yok; antiforgery yeter.

DB talebi: yok (DB-2026-004 uygulandı).
Açık/ertelenen: önceki notun maddeleri (LIKE kaçışı, parola kilidi yok, PIN işlem ayrımı Faz 4) değişmedi. Makine kaldırmayı engelleyen kayıt kontrolü Faz 4 ve 7'de dolar.

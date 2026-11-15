DEVİR: backend-engineer -> koordinatör (frontend-engineer, qa-tester)   Faz: 6 backend tamam   Dosya: Docs/Handover/backend-engineer-20261115-1930.md

Önceki not: Archive/backend-engineer-20261115-1700.md. Aşama 1 ve 2 tek seferde yapıldı (DB talebi bloklayıcı değil, aşağıda).

Route ve roller
- GET `/records` (RecordsController.Index, RecordReaders): sorgu `Query, FacilityId, LineId, MachineId, Kind, Status, From, To, MineOnly` (düz ya da `Filter.` önekli), `page`, `pageSize` (varsayılan 25, 1-100). `MineOnly` gönderilmezse Operatör'de true, diğerlerinde false; formda onay kutusu `asp-for` ile çizilirse gönderilir.
- GET `/records/details/{id}` (RecordReaders), GET `/records/integrity/{id}` (ManagerOrAuditor, Operatör 403).
- GET/POST `/interventions/create?recordId=&type=` ve `/interventions/end-break/{breakId}` (yalnız Manager; Operatör, Denetçi, Süper admin 302 access-denied/403).
- `RecordsController` sınıf düzeyi RecordReaders oldu; eski tüm action'lar tek tek `[Authorize(Roles = Roles.Operator)]` taşır (Manager, Denetçi, Süper admin ile 9 eski adres curl'le denendi, hepsi access-denied).
- `/` Yönetici ve Denetçi'yi `/records`'a, Süper admin'i `/users`'a yönlendirir. `PrototypeController` silindi.

ViewModel'ler: sözleşmedeki adlarla `ViewModels/Records/**` ve `ViewModels/Interventions/**`; koleksiyonlar `T[]`. Sözleşmeye ekler (bilgi, frontend görünümleri bunlarla derleniyor):
1. `RecordListItemViewModel.Status` (enum), `RecordListViewModel.KindOptions[]`, `StatusOptions[]` (Id enum değeri), `PageCount`. Hat seçenekleri yalnız seçili tesisin, makine seçenekleri yalnız seçili hat ya da tesisin makineleridir, seçim yoksa boş (koordinatör isteği).
2. `RecordDetailHeaderViewModel.Status` (enum) ve `DocumentRef?` (K14 doküman no). `Materials[]` mevcut `RecordMaterialViewModel`'i kullanır.
3. `InterventionViewModel.ErrorMessage`, `EndBreakViewModel.ErrorMessage`. ModelState anahtarları `Type`, `Reason`, `Pin`; genel hata `ErrorMessage`. `ViewModels.Interventions.EndBreakViewModel` adı `ViewModels.Breaks.EndBreakViewModel` ile aynı, namespace farklı.
4. `IntegrityMismatchViewModel.Sequence` 0 ise alanı doğrulayacak olay günlükte yok. Mola bitirme ve müdahale formları `/interventions/create?recordId=N` adresine gönderilir (RecordId sorgudan bağlanır).
5. `RecordClosureViewModel` zaman aşımında da dolar (kapatan "Sistem").

Davranış
- Ret 200 görünüm, yetki 403, olmayan kayıt 404, başarı 302 `/records/details/{id}`; antiforgery'siz POST 400; aynı CommandId tek olay (curl ile denendi: iptal ve mola bitirme ikişer gönderim, 1 komut satırı, olay sayısı artmadı).
- Müdahale tek işlemde: açık mola "Müdahale" kapanışı + `BreakEnded`, devam eden adım Kesildi (+ `StepInterrupted`, SH-17/01/02 yazıldı), tamamlanmamış fazlar Kesildi, `IsWorking` kalkar, kayıt İptal edildi/Yarıda kapatıldı (`ClosureKind` = enum değeri), `RecordCancelled`/`RecordAborted`. Açıldı kayıtta Abort reddedilir (form yalnız İptal listeler). Kilit bitişi geçmiş kayıtta zaman aşımı önce uygulanır, istek "Kayıt zaman aşımına düştü." ile reddedilir. Gerekçe 1-500, PIN en son; yanlış PIN sayacı artar, olay yok.
- Yeni olay türleri: `StepInterrupted`, `RecordCancelled`, `RecordAborted`; `BreakEnded` yükü `Reason` taşır. Faz 4 `RecordOpened` yüküne `MaterialLots[] {LotId, LotNo, ExpiredAtEntry, ExpiredReason}` eklendi (bütünlük doğrulaması için).
- Bütünlük: `IRecordIntegrity` zinciri yeniden hesaplar (sıra, önceki özet, HMAC; kanonik biçim `EventCanonical` ile yazma kodu ile ortak) ve durum tablosu alanlarını olay değerleriyle karşılaştırır (kayıt no, makine, versiyon, iş emri, sahip, açılış/başlangıç/tamamlanma/kapanış anı, kapanış alanları, durum, adım zaman ve kişi ve görevlileri, faz değerlendirmesi, malzeme lot no ve SKT, mola alanları). Sınır: Faz 6 öncesi açılmış kayıtların `RecordOpened` olayında lot no yok; onlarda açılışta eklenen malzemenin lot no değeri yalnız lot kimliği kümesiyle denetlenir (sonradan eklenen malzemeler olaydan tam denetlenir).
- Oturum sertleştirmesi (K23): `VakaClaimTypes.SessionId`, `ISessionRevocations` (bellek, `<OturumSüresi>`), `VakaCookieEvents.SigningOut` çıkışta kimliği listeye alır (Logout ve PIN kilidi çıkışları dahil), doğrulamada iptal ya da claim'siz çerez `reason=signed-out` ile girişe yönlenir. Eski çerezler (claim'siz) bir kez yeniden giriş ister. Uygulama yeniden başlayınca liste boşalır; denemede yeniden başlatma sonrası eski çerez yine girişe yönlendi (çerez doğrulaması başka nedenle düştü, listeye bağlı değil).
- F5-B1: `CalculateLiveElapsed` farkı tick düzeyinde alır, yalnız sonucu aşağı yuvarlar; XML yorumu güncel. Açık molalı kayıtta kayıt sayacı art arda yenilemede 27, 27, 27.
- F5-A1: `RecordLinks.For` Yönetici ve Denetçi'ye detay, yalnız Operatör'e yürütme bağlantısı verir; pasifleştirme ekranında Yönetici'de `/records/details/{id}`, Süper admin'de bağlantısız. Makine panosu `RecordUrl` aynı kuralla.

DB talebi: DB-2026-008 (üç index, TALEP). Koordinatör kararıyla bu gece uygulanmaz; Faz 6 indekssiz tamamlandı ve test edildi, kod ona bağlı değil.

Doğrulama (kopya DB `.qa/be/s6/s.db`, port 5396, kapatıldı; `dotnet build -warnaserror -o .qa/be/s6build` 0 uyarı 0 hata; §4 grep 0, sihirli rol 0, G8 grep 0, em/en-dash ve `#region` 0, her controller `[Authorize]`):
- Liste (5 ve sonra 10 kayıt): Operatör varsayılan yalnız sahibi/görevlisi olduğu kayıtlar, `MineOnly=false` hepsi; durum, makine, tür, tarih aralığı, kayıt no, saha referansı, iş emri, lot no (büyük küçük harf gözetmez), `%` ve `_` aranan metin olarak; sayfalama (pageSize=2, page=2).
- Rol matrisi (oturumsuz, Operatör, Yönetici, Denetçi, Süper admin) 5 yeni adres: oturumsuz 302 giriş, yetkisiz roller 302 access-denied, yetkili 200; Operatör integrity ve interventions'ta reddedildi.
- Müdahale: Yönetici mola bitirme (olay, kapanış türü Yönetici), açık molalı yarıda kapatma (mola Müdahale), devam eden adımlı iptal (adım Kesildi, 5170 sn), Açıldı kayıtta iptal (makine yeniden açılabildi), Açıldı kayıtta yarıda kapatma reddi, kilidi geçmiş kayıtta iptal reddi (RecordTimedOut, anı kilit bitişi), gerekçesiz, 501 karakter, türsüz, geçersiz tür, yanlış PIN: 200 ve olay yok. Her müdahaleden sonra bütünlük "Doğrulandı".
- Bütünlük bozma (kopya DB): iş emri ve lot no SQL ile değişince alan, saklanan ve olaydaki değer ekranda; olay yükü/zamanı değişince ilk tutarsız sıra gösterildi (1 ve 12).
Denenmedi: tarayıcıda JS davranışı ve yazdırma önizlemesi (frontend), beklemedeki kayıt (Faz 7), adım sürerken açık mola (Faz 7), K20 uçtan uca tam akış (parçaları ayrı denendi, pasifleştirme sonrası sahip değişmedi kontrolü yapılmadı).

Frontend notları: `Views/Home/Index.cshtml` ve `HomeViewModel` artık erişilemez (dört rol de yönlendirilir); Faz 7 `/dashboard`'da silinebilir, backend şimdilik bıraktı (görünüm frontend'in).

Düzeltme (teslim testi B1): `RecordDetailParticipantViewModel.ConfirmedAtLocal` artık `DateTime?` ve yalnız `ConfirmedAtUtc` doluysa dolar (Faz 7, PL-57); yeni `AddedAtLocal: DateTime` ParticipantAdded olayından, açılıştaki sahip için açılış anından gelir. Bütünlük denetimi onay alanına bakmıyor, değişiklik gerekmedi. `dotnet build -warnaserror -o .qa/be/s6build` 0 uyarı 0 hata; kopya DB'de detay 200.

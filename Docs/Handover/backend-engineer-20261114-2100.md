DEVİR: backend-engineer -> koordinatör (qa-tester tur 2)   Faz: 1 düzeltme turu 1   Dosya: Docs/Handover/backend-engineer-20261114-2100.md

Yapıldı:
- B2 (PL-29): `Services/UserAdministration.cs` içinde `RunExclusiveAsync`: pasifleştirme ve rol güncelleme (Süper admin rolünü kaldırabilen tek iki yol) başta yazma kilidi alan tek işlemde çalışır. Kilitten sonra izleyici temizlenir; işlemi yapan kişinin aktifliği, hedef, roller ve aktif Süper admin sayısı veritabanından yeniden okunur. İşlemi yapan artık aktif değilse ya da kapsamı kalmadıysa `Forbidden` (403 sayfası), kural ihlali olursa mevcut "Son aktif Süper admin ..." hatası. Pasifleştirmede ön denetim ve PIN doğrulaması kilit dışında kalır (PIN sayaç yazar), engel denetimi kilit içinde tekrarlanır. Kilidi bekleyen istek 30 sn varsayılan zaman aşımından sonra SQLite 5/6 hatasını "Sistem şu an meşgul. İşlemi birkaç saniye sonra yeniden deneyin." iş hatasına çevirir (500 yok). Şema değişikliği gerekmedi.
- EF `BeginTransactionAsync` varsayılanı: Microsoft.Data.Sqlite 10 ile ayrı denemede (iki bağlantı, 1 sn zaman aşımı) hem `Unspecified` (EF varsayılanı) hem `Serializable` ikinci yazıcıyı hemen engelledi (kod 5), yani ikisi de `BEGIN IMMEDIATE`. Kilit ayrıca açıkça alınmadı; kodda `IsolationLevel.Serializable` niyet için açık yazıldı ve XML yorumunda bu not var.
- B3: `AccountPrincipal.Create` içine `ClaimTypes.NameIdentifier` (kullanıcı kimliği) eklendi.
- B4: `appsettings.json` içinde `Microsoft.EntityFrameworkCore.Database.Command` = `Warning`.
- B5: `Docs/Db/DB-2026-003.md` açıldı (durum TALEP, sınıf Eklemeli, model farkı çıkarsa dbadmin durur), `DbChanges.md` indeksine satır eklendi. Entity/Configuration'a dokunulmadı.

Doğrulama (kendi kopyam `.qa/be/race.db`, port 5293, kapatıldı; geliştirme DB'sine dokunulmadı):
- `dotnet build Src/Vaka.Web -warnaserror`: 0 uyarı, 0 hata (bin kilitli olduğundan `-o .qa/be/build`). §4 grep'leri ve em/en-dash: 0 eşleşme.
- İki Süper admin (0001, 2001), her biri kendi oturum ve PIN'iyle, barrier ile eşzamanlı POST:
  - Karşılıklı pasifleştirme 3 deneme: her seferinde 302/302, biri başarılı, kaybeden 403 sayfasına (`access-denied`) gitti; sonda aktif Süper admin 1, 500 yok. Kazanan iki yönde de görüldü (0001 de 2001 de).
  - Pasifleştirme + rol kaldırma (0001'in Süper admin rolü Yönetici ile değiştirilir) 6 deneme, ikinci istek 0-150 ms gecikmeli: rol kaldırma kazandığında pasifleştirmeyi yapan Yönetici'ye düştüğü için 403 sayfası; pasifleştirme kazandığında rol kaldırmayı yapan oturum `reason=inactive` ile düştü. Sonda her seferinde aktif Süper admin 1, 500 yok.
- B4: bu koşuda uygulama günlüğünde "Executed DbCommand" 0.
- B3 sonrası formun yeniden gönderilmesi QA tur 2 maddesidir; benim turumda yalnız claim eklendi.

Frontend/QA için:
- Yeni kullanıcıya dönük metin: "Sistem şu an meşgul. İşlemi birkaç saniye sonra yeniden deneyin." (form düzeyi hata, alan anahtarı boş).
- Yarışı kaybeden Süper admin/Yönetici 403 sayfası görür (kapsam kalmadı) ya da oturumu düşer; bu bilinçli.

DB talebi: DB-2026-003 (CK_UserRoles_Role ifadesi Roles sabitlerinden, SQL aynı, migration yok; model farkı çıkarsa Faz 2'ye ertelenir). Koordinatör dbadmin'i çağırmalı.
Açık/ertelenen: önceki notun maddeleri (LIKE kaçışı, parola kilidi yok, PIN işlem ayrımı Faz 4) değişmedi.

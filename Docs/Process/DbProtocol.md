# Veritabanı Protokolü (SQLite)

> Okur: dbadmin, backend-engineer, qa-tester. Mekanik uygulayıcı: `.claude/hooks/Guard.mjs`.

## 1. Erişim

- Şema, entity, migration, seed ve doğrudan SQL **yalnız dbadmin**'dedir: `Domain/Entities/**`, `Data/**`, `dotnet ef`, `sqlite3`.
- dbadmin'e **yalnız backend-engineer** talep açar. Analist, frontend ve QA'nın veri ihtiyacı backend-engineer'a yazılır; o talebe çevirir.
- Koordinatör dbadmin'i yalnız `Docs/Db/DB-<yıl>-<sıra>.md` talep dosyasıyla çağırır. Talepsiz çağrıyı dbadmin reddeder.
- Uygulama açılışta `Migrate()` **çağırmaz**. Migration'ı dbadmin `dotnet ef database update` ile uygular; tek kapı budur.

## 2. İşlem sınıfları

| Sınıf | Örnek | Kural |
|---|---|---|
| **Eklemeli** | Yeni tablo (`CreateTable`), yeni tabloya index, seed ekleme | dbadmin kısa kayıtla uygular |
| **ALTER** | Mevcut tabloya kolon ekleme, kolon tipi/null değişimi, yeniden adlandırma, FK/unique/check ekleme, veri `UPDATE` | **Kullanıcı onayı zorunlu.** dbadmin uygulamadan durur, koordinatör onayı alır, onay kayda işlenir |
| **DROP** | Tablo, kolon, index, FK, kısıt düşürme; `dotnet ef database drop`; `.db` dosyasını silme; veri `DELETE` | **Yasak.** dbadmin yapamaz. Gerçekten gerekiyorsa kullanıcı elle yapar |

SQLite notu: `AlterColumn` ve bazı kısıt değişiklikleri tabloyu yeniden kurar (kopyala, eskisini düşür). Bu yüzden ALTER her zaman onaylıdır
ve mümkün olan her yerde **expand** yaklaşımı tercih edilir: eski kolonu bırak, yenisini ekle, veriyi taşı.

Hook davranışı (bilgi):

- `git commit` ve `dotnet ef database update` sırasında commit'lenmemiş migration'ların `Up()` gövdesi taranır: `Drop*` varsa **ret**.
- `dotnet ef database update` sırasında `Up()` içinde `AddColumn`, `AlterColumn`, `Rename*`, `Add*Constraint/Key` varsa **onay istemi**.
- Ham SQL'de `DROP ...` **ret**, `ALTER TABLE` **onay istemi**. `Down()` gövdesi taranmaz (geri alma kodu doğal olarak Drop içerir).

## 3. Talep ve kayıt (`Docs/Db/DB-<yıl>-<sıra>.md`)

backend-engineer üst bölümü, dbadmin alt bölümü doldurur. İndeks: `Docs/Db/DbChanges.md` (kayıt başına tek satır).

```markdown
# DB-2026-001: <başlık>

Durum: TALEP | ONAY BEKLİYOR | ONAYLI | UYGULANDI | DOĞRULANDI | REDDEDİLDİ
Faz: <N>   Talep eden: backend-engineer   Tarih: <yyyy-mm-dd>   Sınıf: Eklemeli | ALTER

## Talep (backend-engineer)
Neden: <hangi ekran/servis için>
İstenen: <varlık, alanlar, ilişkiler, beklenen sorgular>

## Tasarım ve sonuç (dbadmin)
Tablolar/ilişkiler: <1-N / N-N, OnDelete>
Index'ler: <hangi sorgu için>
Migration: <ad>
Seed etkisi: <yok | ...>
Onay (yalnız ALTER): <kullanıcı onayının tarihi ve özeti>
Doğrulama: <komut + sonuç>
Backend'in bilmesi gerekenler: <entity adı, navigation, DbSet>
```

## 4. Tasarım kuralları

- Tablo ve kolonlar PascalCase; PK `Id`; FK `<Varlık>Id` + index + açık `OnDelete` (Cascade / Restrict / SetNull bilinçli seçilir).
- N-N ilişki açık join entity + bileşik PK. Metin kolonlarında `HasMaxLength`. Para `decimal` (SQLite'ta `TEXT` dönüşümü ya da kuruş `INTEGER`), tarih UTC.
- Soft delete `IsActive`; fiziksel silme özelliği istenirse analist kararıyla.
- Index yalnız gerçek sorgu için: FK'ler, unique alanlar (`Email`), liste ekranının sıralama/filtre kolonları. Ölçümsüz index eklenmez.
- Yapılandırma `Data/Configurations/<Entity>Configuration.cs` (`IEntityTypeConfiguration<T>`); DbContext içinde yığın `OnModelCreating` yok.
- Migration adı anlamlı PascalCase: `AddOrdersAndOrderItems`. Üretilen dosya okunmadan uygulanmaz.

## 5. Seed

Idempotent (anahtar kontrolü), mevcut veriyi ezmez. Dashboard'un dolu görünmesi için gerçekçi demo veri gerekir (grafikler, listeler,
boş olmayan durumlar); veriler analistin `Requirements.md` dosyasındaki alan tanımlarına uyar. Jenerik isim (John Doe, Acme), lorem ipsum
ve em-dash yok. Varsayılan kullanıcılar ve rolleri seed'dedir; parolalar hash'lenir, düz metin parola kayda ve koda yazılmaz
(geliştirme parolası yalnız README'de belirtilir).

## 6. QA ortamı

qa-tester kaynak DB'ye dokunmaz: `.qa/` altında kendi DB kopyasıyla çalışır. Index ya da şema önerisi bulgu olarak backend-engineer'a yazılır.

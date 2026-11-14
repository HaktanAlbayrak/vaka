# DB Değişiklik İndeksi

Kayıt başına tek satır; ayrıntı `Docs/Db/DB-<yıl>-<sıra>.md` dosyasında. Protokol: [DbProtocol.md](../Process/DbProtocol.md).

| Kayıt | Başlık | Faz | Sınıf | Durum | Migration |
|---|---|---|---|---|---|
| [DB-2026-001](DB-2026-001.md) | Kullanıcı ve kullanıcı rolleri tabloları, Süper admin seed | 1 | Eklemeli | UYGULANDI | AddUsersRolesAndSettings |
| [DB-2026-002](DB-2026-002.md) | Sistem ayarları ve ayar değişiklik geçmişi, üç ayar satırı seed | 1 | Eklemeli | UYGULANDI | AddUsersRolesAndSettings |
| [DB-2026-003](DB-2026-003.md) | CK_UserRoles_Role ifadesi Roles sabitlerinden kurulsun | 1 | Eklemeli | UYGULANDI | yok (şema değişmez) |
| [DB-2026-004](DB-2026-004.md) | Tesis, hat, makine, vardiya, malzeme ve lot tabloları, demo seed | 2 | Eklemeli | UYGULANDI | AddFacilitiesMachinesShiftsMaterials |

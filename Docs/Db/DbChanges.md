# DB Değişiklik İndeksi

Kayıt başına tek satır; ayrıntı `Docs/Db/DB-<yıl>-<sıra>.md` dosyasında. Protokol: [DbProtocol.md](../Process/DbProtocol.md).

| Kayıt | Başlık | Faz | Sınıf | Durum | Migration |
|---|---|---|---|---|---|
| [DB-2026-001](DB-2026-001.md) | Kullanıcı ve kullanıcı rolleri tabloları, Süper admin seed | 1 | Eklemeli | UYGULANDI | AddUsersRolesAndSettings |
| [DB-2026-002](DB-2026-002.md) | Sistem ayarları ve ayar değişiklik geçmişi, üç ayar satırı seed | 1 | Eklemeli | UYGULANDI | AddUsersRolesAndSettings |
| [DB-2026-003](DB-2026-003.md) | CK_UserRoles_Role ifadesi Roles sabitlerinden kurulsun | 1 | Eklemeli | UYGULANDI | yok (şema değişmez) |
| [DB-2026-004](DB-2026-004.md) | Tesis, hat, makine, vardiya, malzeme ve lot tabloları, demo seed | 2 | Eklemeli | UYGULANDI | AddFacilitiesMachinesShiftsMaterials |
| [DB-2026-005](DB-2026-005.md) | Temizlik tipi, makine eşlemesi, prosedür versiyonu, faz, adım, medya, değişmezlik tetikleyicileri, üç ayar satırı | 3 | Eklemeli | DOĞRULANDI | AddCleaningTypesAndProcedureVersions |
| [DB-2026-006](DB-2026-006.md) | Temizlik kaydı, faz ve adım yürütmeleri, görevliler, kayıt malzemeleri, olay günlüğü, işlenmiş komutlar, numara sayaçları, tetikleyiciler, üçüncü demo tipi | 4 | Eklemeli | DOĞRULANDI | AddCleaningRecordsAndEventLog |
| [DB-2026-007](DB-2026-007.md) | Mola tablosu ve dördüncü demo tipi (KSA, kısa planlı temizlik) | 5 | Eklemeli | DOĞRULANDI | AddRecordBreaks |
| [DB-2026-008](DB-2026-008.md) | Kayıt listesi süzgeçleri ve lot araması için index'ler | 6 | Eklemeli | DOĞRULANDI | AddCleaningRecordListIndexes |
| [DB-2026-009](DB-2026-009.md) | Görevli çalışma aralığı tablosu (StepWorkIntervals), değişmezlik tetikleyicileri | 6E | Eklemeli | DOĞRULANDI | AddStepWorkIntervals |
| [DB-2026-010](DB-2026-010.md) | Users giriş sayacı ve süreli kilit kolonları, LoginLockMinutes ayarı | 6F | ALTER (kolon ekleme) ve Eklemeli | DOĞRULANDI | AddLoginLockToUsers |

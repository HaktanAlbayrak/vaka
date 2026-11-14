-- Faz 2 notr demo verisi (DB-2026-004). Migration disindadir; yalniz gelistirme/QA kopya veritabanina yuklenir.
-- Idempotent: sabit Id + INSERT OR IGNORE, tekrar calistirilinca cogaltmaz, var olan satiri ezmez.

INSERT OR IGNORE INTO "Facilities" ("Id","Code","Name","IsActive") VALUES
 (1,'T1','Ana tesis',1),
 (2,'T2','Yedek tesis',0);

INSERT OR IGNORE INTO "ProductionLines" ("Id","FacilityId","Code","Name","IsActive") VALUES
 (1,1,'H1','Hazırlık hattı',1),
 (2,1,'H2','Dolum hattı',1),
 (3,2,'H1','Depo hattı',1);

INSERT OR IGNORE INTO "Machines" ("Id","ProductionLineId","Code","Name","IsActive","RetiredAtUtc") VALUES
 (1,1,'M01','Karıştırıcı',1,NULL),
 (2,1,'M02','Tartı ünitesi',1,NULL),
 (3,2,'M01','Dolum makinesi',1,NULL),
 (4,2,'M02','Etiketleme makinesi',0,'2026-09-30 09:00:00'),
 (5,3,'M01','Depo asansörü',1,NULL);

INSERT OR IGNORE INTO "Shifts" ("Id","FacilityId","Name","StartTime","EndTime","IsActive") VALUES
 (1,1,'Gündüz','07:00:00','15:00:00',1),
 (2,1,'Akşam','15:00:00','23:00:00',1),
 (3,1,'Gece','23:00:00','07:00:00',1);

INSERT OR IGNORE INTO "Materials" ("Id","Code","Name","IsActive") VALUES
 (1,'MLZ001','Yüzey dezenfektanı',1),
 (2,'MLZ002','Kimyasal temizleyici',1),
 (3,'MLZ003','Temizlik bezi',0);

INSERT OR IGNORE INTO "MaterialLots" ("Id","MaterialId","LotNo","ExpiryDate") VALUES
 (1,1,'L2026-001','2026-06-30'),
 (2,1,'L2027-001','2027-03-31'),
 (3,1,'L2027-002','2027-09-30'),
 (4,2,'K2027A','2027-01-31'),
 (5,2,'K2026B','2026-12-31');

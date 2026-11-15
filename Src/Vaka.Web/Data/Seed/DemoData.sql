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

-- Faz 3 notr demo verisi (DB-2026-005). Migration disindadir; Faz 2 duzeni: sabit Id + INSERT OR IGNORE.
-- Yayinlanmis versiyona faz/adim eklenemedigi icin (tetikleyici) versiyonlar once Taslak yazilir, icerik girilir,
-- en sonda Yururlukte yapilir. Icerik satirlari yalniz versiyon Taslak iken eklenir; ikinci calistirmada
-- versiyonlar zaten Yururlukte oldugundan hicbir ifade satir eklemez ya da degistirmez.
INSERT OR IGNORE INTO "CleaningTypes" ("Id","Code","Name","Kind","DefaultLockMinutes","IsActive") VALUES
 (1,'PLN','Planlı temizlik',1,90,1),
 (2,'ACL','Acil müdahale',2,NULL,1);

INSERT OR IGNORE INTO "MachineCleaningTypes" ("MachineId","CleaningTypeId")
 SELECT "Id", 1 FROM "Machines" WHERE "Id" IN (1,2,3,5) AND "IsActive" = 1 AND "RetiredAtUtc" IS NULL;

INSERT OR IGNORE INTO "ProcedureVersions" ("Id","CleaningTypeId","VersionNo","Status","MaterialRequired","DocumentRef","PublishedAtUtc","PublishedByUserId") VALUES
 (1,1,1,1,1,NULL,NULL,NULL),
 (2,2,1,1,0,NULL,NULL,NULL);

INSERT OR IGNORE INTO "ProcedurePhases" ("Id","ProcedureVersionId","OrderNo","Title","MinDurationSeconds")
 SELECT column1, column2, column3, column4, column5 FROM (VALUES
  (1,1,1,'Hazırlık',300),
  (2,1,2,'Temizlik',900),
  (3,1,3,'Kontrol',300))
 WHERE EXISTS (SELECT 1 FROM "ProcedureVersions" WHERE "Id" = 1 AND "Status" = 1);

INSERT OR IGNORE INTO "ProcedurePhases" ("Id","ProcedureVersionId","OrderNo","Title","MinDurationSeconds")
 SELECT column1, column2, column3, column4, column5 FROM (VALUES
  (4,2,1,'Müdahale',NULL))
 WHERE EXISTS (SELECT 1 FROM "ProcedureVersions" WHERE "Id" = 2 AND "Status" = 1);

INSERT OR IGNORE INTO "ProcedureSteps" ("Id","ProcedurePhaseId","OrderNo","Title","Description")
 SELECT column1, column2, column3, column4, column5 FROM (VALUES
  (1,1,1,'Makineyi durdur ve enerjiyi kes',NULL),
  (2,1,2,'Ürün kalıntısını boşalt',NULL),
  (3,2,1,'Yüzeyi dezenfektanla sil',NULL),
  (4,2,2,'Temas parçalarını sök ve yıka',NULL),
  (5,2,3,'Parçaları yerine tak',NULL),
  (6,3,1,'Yüzeyi gözle kontrol et ve onayla',NULL))
 WHERE EXISTS (SELECT 1 FROM "ProcedureVersions" WHERE "Id" = 1 AND "Status" = 1);

INSERT OR IGNORE INTO "ProcedureSteps" ("Id","ProcedurePhaseId","OrderNo","Title","Description")
 SELECT column1, column2, column3, column4, column5 FROM (VALUES
  (7,4,1,'Müdahaleyi yap ve durumu yaz',NULL))
 WHERE EXISTS (SELECT 1 FROM "ProcedureVersions" WHERE "Id" = 2 AND "Status" = 1);

UPDATE "ProcedureVersions" SET "Status" = 2, "PublishedAtUtc" = '2026-10-01 08:00:00'
 WHERE "Id" IN (1,2) AND "Status" = 1 AND "PublishedByUserId" IS NULL
   AND EXISTS (SELECT 1 FROM "ProcedurePhases" WHERE "ProcedureVersionId" = "ProcedureVersions"."Id");

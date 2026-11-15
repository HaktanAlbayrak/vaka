using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCleaningTypesAndProcedureVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleaningTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false, collation: "NOCASE"),
                    Name = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    DefaultLockMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningTypes", x => x.Id);
                    table.CheckConstraint("CK_CleaningTypes_Kind", "\"Kind\" IN (1, 2)");
                    table.CheckConstraint("CK_CleaningTypes_LockMinutes", "(\"Kind\" = 1 AND \"DefaultLockMinutes\" > 0) OR (\"Kind\" = 2 AND \"DefaultLockMinutes\" IS NULL)");
                });

            migrationBuilder.CreateTable(
                name: "MachineCleaningTypes",
                columns: table => new
                {
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    CleaningTypeId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineCleaningTypes", x => new { x.MachineId, x.CleaningTypeId });
                    table.ForeignKey(
                        name: "FK_MachineCleaningTypes_CleaningTypes_CleaningTypeId",
                        column: x => x.CleaningTypeId,
                        principalTable: "CleaningTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MachineCleaningTypes_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CleaningTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    VersionNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialRequired = table.Column<bool>(type: "INTEGER", nullable: false),
                    DocumentRef = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    PublishedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PublishedByUserId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureVersions", x => x.Id);
                    table.CheckConstraint("CK_ProcedureVersions_Published", "(\"Status\" = 1 AND \"PublishedAtUtc\" IS NULL) OR (\"Status\" <> 1 AND \"PublishedAtUtc\" IS NOT NULL)");
                    table.CheckConstraint("CK_ProcedureVersions_Status", "\"Status\" IN (1, 2, 3)");
                    table.CheckConstraint("CK_ProcedureVersions_VersionNo", "\"VersionNo\" > 0");
                    table.ForeignKey(
                        name: "FK_ProcedureVersions_CleaningTypes_CleaningTypeId",
                        column: x => x.CleaningTypeId,
                        principalTable: "CleaningTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProcedureVersions_Users_PublishedByUserId",
                        column: x => x.PublishedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcedurePhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcedureVersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    MinDurationSeconds = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedurePhases", x => x.Id);
                    table.CheckConstraint("CK_ProcedurePhases_MinDuration", "\"MinDurationSeconds\" IS NULL OR \"MinDurationSeconds\" > 0");
                    table.CheckConstraint("CK_ProcedurePhases_OrderNo", "\"OrderNo\" > 0");
                    table.ForeignKey(
                        name: "FK_ProcedurePhases_ProcedureVersions_ProcedureVersionId",
                        column: x => x.ProcedureVersionId,
                        principalTable: "ProcedureVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcedurePhaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    OrderNo = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureSteps", x => x.Id);
                    table.CheckConstraint("CK_ProcedureSteps_OrderNo", "\"OrderNo\" > 0");
                    table.ForeignKey(
                        name: "FK_ProcedureSteps_ProcedurePhases_ProcedurePhaseId",
                        column: x => x.ProcedurePhaseId,
                        principalTable: "ProcedurePhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepMedia",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProcedureStepId = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaType = table.Column<int>(type: "INTEGER", nullable: false),
                    Sha256 = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    SizeBytes = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepMedia", x => x.Id);
                    table.CheckConstraint("CK_StepMedia_MediaType", "\"MediaType\" IN (1, 2)");
                    table.CheckConstraint("CK_StepMedia_Sha256", "length(\"Sha256\") = 64");
                    table.CheckConstraint("CK_StepMedia_SizeBytes", "\"SizeBytes\" > 0");
                    table.ForeignKey(
                        name: "FK_StepMedia_ProcedureSteps_ProcedureStepId",
                        column: x => x.ProcedureStepId,
                        principalTable: "ProcedureSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Key", "Unit", "Value" },
                values: new object[,]
                {
                    { 4, "MaxLockMinutes", "dakika", 120 },
                    { 5, "MaxPhotoMb", "MB", 10 },
                    { 6, "MaxVideoMb", "MB", 100 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningTypes_Code",
                table: "CleaningTypes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineCleaningTypes_CleaningTypeId",
                table: "MachineCleaningTypes",
                column: "CleaningTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedurePhases_ProcedureVersionId_OrderNo",
                table: "ProcedurePhases",
                columns: new[] { "ProcedureVersionId", "OrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureSteps_ProcedurePhaseId_OrderNo",
                table: "ProcedureSteps",
                columns: new[] { "ProcedurePhaseId", "OrderNo" });

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureVersions_CleaningTypeId_VersionNo",
                table: "ProcedureVersions",
                columns: new[] { "CleaningTypeId", "VersionNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureVersions_OneCurrent",
                table: "ProcedureVersions",
                column: "CleaningTypeId",
                unique: true,
                filter: "\"Status\" = 2");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureVersions_OneDraft",
                table: "ProcedureVersions",
                column: "CleaningTypeId",
                unique: true,
                filter: "\"Status\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureVersions_PublishedByUserId",
                table: "ProcedureVersions",
                column: "PublishedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StepMedia_ProcedureStepId",
                table: "StepMedia",
                column: "ProcedureStepId");

            foreach (var statement in ImmutabilityTriggers())
            {
                migrationBuilder.Sql(statement);
            }
        }

        private const string PublishedMessage = "Yayınlanmış versiyon değiştirilemez.";

        // Yayınlanmış = üst versiyonun Status değeri 2 ya da 3 (BR-04).
        private static IEnumerable<string> ImmutabilityTriggers()
        {
            const string phaseVersion = "SELECT 1 FROM \"ProcedureVersions\" WHERE \"Id\" = {0} AND \"Status\" IN (2, 3)";
            const string stepVersion = "SELECT 1 FROM \"ProcedurePhases\" p JOIN \"ProcedureVersions\" v ON v.\"Id\" = p.\"ProcedureVersionId\" WHERE p.\"Id\" = {0} AND v.\"Status\" IN (2, 3)";
            const string mediaVersion = "SELECT 1 FROM \"ProcedureSteps\" s JOIN \"ProcedurePhases\" p ON p.\"Id\" = s.\"ProcedurePhaseId\" JOIN \"ProcedureVersions\" v ON v.\"Id\" = p.\"ProcedureVersionId\" WHERE s.\"Id\" = {0} AND v.\"Status\" IN (2, 3)";

            var targets = new (string Table, string Lookup, string Column)[]
            {
                ("ProcedurePhases", phaseVersion, "ProcedureVersionId"),
                ("ProcedureSteps", stepVersion, "ProcedurePhaseId"),
                ("StepMedia", mediaVersion, "ProcedureStepId")
            };

            foreach (var (table, lookup, column) in targets)
            {
                yield return Trigger($"TR_{table}_NoInsertWhenPublished", "INSERT", table, string.Format(lookup, $"NEW.\"{column}\""));
                yield return Trigger($"TR_{table}_NoDeleteWhenPublished", "DELETE", table, string.Format(lookup, $"OLD.\"{column}\""));
                yield return Trigger(
                    $"TR_{table}_NoUpdateWhenPublished",
                    "UPDATE",
                    table,
                    $"SELECT 1 WHERE EXISTS ({string.Format(lookup, $"OLD.\"{column}\"")}) OR EXISTS ({string.Format(lookup, $"NEW.\"{column}\"")})");
            }

            yield return "CREATE TRIGGER \"TR_ProcedureVersions_NoDeleteWhenPublished\" BEFORE DELETE ON \"ProcedureVersions\" "
                + $"WHEN OLD.\"Status\" <> 1 BEGIN SELECT RAISE(ABORT, '{PublishedMessage}'); END;";

            yield return "CREATE TRIGGER \"TR_ProcedureVersions_NoUpdateWhenPublished\" BEFORE UPDATE ON \"ProcedureVersions\" "
                + "WHEN OLD.\"Status\" <> 1 AND NOT (OLD.\"Status\" = 2 AND NEW.\"Status\" = 3 "
                + "AND NEW.\"Id\" IS OLD.\"Id\" AND NEW.\"CleaningTypeId\" IS OLD.\"CleaningTypeId\" AND NEW.\"VersionNo\" IS OLD.\"VersionNo\" "
                + "AND NEW.\"MaterialRequired\" IS OLD.\"MaterialRequired\" AND NEW.\"DocumentRef\" IS OLD.\"DocumentRef\" "
                + "AND NEW.\"PublishedAtUtc\" IS OLD.\"PublishedAtUtc\" AND NEW.\"PublishedByUserId\" IS OLD.\"PublishedByUserId\") "
                + $"BEGIN SELECT RAISE(ABORT, '{PublishedMessage}'); END;";
        }

        private static string Trigger(string name, string eventName, string table, string condition)
        {
            var when = condition.StartsWith("SELECT 1 WHERE EXISTS", StringComparison.Ordinal)
                ? condition.Substring("SELECT 1 WHERE ".Length)
                : $"EXISTS ({condition})";
            return $"CREATE TRIGGER \"{name}\" BEFORE {eventName} ON \"{table}\" WHEN {when} BEGIN SELECT RAISE(ABORT, '{PublishedMessage}'); END;";
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MachineCleaningTypes");

            migrationBuilder.DropTable(
                name: "StepMedia");

            migrationBuilder.DropTable(
                name: "ProcedureSteps");

            migrationBuilder.DropTable(
                name: "ProcedurePhases");

            migrationBuilder.DropTable(
                name: "ProcedureVersions");

            migrationBuilder.DropTable(
                name: "CleaningTypes");

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 6);
        }
    }
}

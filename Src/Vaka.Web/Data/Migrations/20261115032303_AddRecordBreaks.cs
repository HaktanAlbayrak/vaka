using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordBreaks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordBreaks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CleaningRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DuringStepId = table.Column<int>(type: "INTEGER", nullable: true),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    EndKind = table.Column<int>(type: "INTEGER", nullable: true),
                    EndReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordBreaks", x => x.Id);
                    table.CheckConstraint("CK_RecordBreaks_EndedAfterStart", "\"EndedAtUtc\" IS NULL OR \"EndedAtUtc\" >= \"StartedAtUtc\"");
                    table.CheckConstraint("CK_RecordBreaks_EndFieldsTogether", "(\"EndedAtUtc\" IS NULL) = (\"EndKind\" IS NULL) AND (\"EndedAtUtc\" IS NULL) = (\"EndedByUserId\" IS NULL)");
                    table.CheckConstraint("CK_RecordBreaks_EndKind", "\"EndKind\" IS NULL OR \"EndKind\" IN (1, 2, 3)");
                    table.ForeignKey(
                        name: "FK_RecordBreaks_CleaningRecords_CleaningRecordId",
                        column: x => x.CleaningRecordId,
                        principalTable: "CleaningRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordBreaks_RecordSteps_DuringStepId",
                        column: x => x.DuringStepId,
                        principalTable: "RecordSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordBreaks_Users_EndedByUserId",
                        column: x => x.EndedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordBreaks_Users_StartedByUserId",
                        column: x => x.StartedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordBreaks_CleaningRecordId_StartedAtUtc",
                table: "RecordBreaks",
                columns: new[] { "CleaningRecordId", "StartedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_RecordBreaks_DuringStepId",
                table: "RecordBreaks",
                column: "DuringStepId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordBreaks_EndedByUserId",
                table: "RecordBreaks",
                column: "EndedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordBreaks_OneOpenPerRecord",
                table: "RecordBreaks",
                column: "CleaningRecordId",
                unique: true,
                filter: "\"EndedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecordBreaks_OneOpenPerUser",
                table: "RecordBreaks",
                column: "StartedByUserId",
                unique: true,
                filter: "\"EndedAtUtc\" IS NULL");

            foreach (var statement in GuardTriggers())
            {
                migrationBuilder.Sql(statement);
            }
        }

        private static IEnumerable<string> GuardTriggers()
        {
            // Başlangıç anı, kayıt, başlatan ve adım bağı hiç değişmez; kapanmış molanın kapanış alanları da değişmez.
            // Boştan doluya (açık molayı kapatma) geçiş serbesttir.
            yield return "CREATE TRIGGER \"TR_RecordBreaks_NoTimeChange\" BEFORE UPDATE ON \"RecordBreaks\" FOR EACH ROW "
                + "WHEN NEW.\"StartedAtUtc\" IS NOT OLD.\"StartedAtUtc\" OR NEW.\"CleaningRecordId\" IS NOT OLD.\"CleaningRecordId\" "
                + "OR NEW.\"StartedByUserId\" IS NOT OLD.\"StartedByUserId\" OR NEW.\"DuringStepId\" IS NOT OLD.\"DuringStepId\" "
                + "OR (OLD.\"EndedAtUtc\" IS NOT NULL AND (NEW.\"EndedAtUtc\" IS NOT OLD.\"EndedAtUtc\" OR NEW.\"EndedByUserId\" IS NOT OLD.\"EndedByUserId\" OR NEW.\"EndKind\" IS NOT OLD.\"EndKind\")) "
                + "BEGIN SELECT RAISE(ABORT, 'Başlamış ya da bitmiş molanın zamanı değiştirilemez.'); END;";

            // Mola kaydı süre hesabına girer, silinmez.
            yield return "CREATE TRIGGER \"TR_RecordBreaks_NoDelete\" BEFORE DELETE ON \"RecordBreaks\" FOR EACH ROW "
                + "BEGIN SELECT RAISE(ABORT, 'Mola kaydı silinemez.'); END;";
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordBreaks");
        }
    }
}

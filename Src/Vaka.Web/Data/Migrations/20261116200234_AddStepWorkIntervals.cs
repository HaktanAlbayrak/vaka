using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddStepWorkIntervals : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StepWorkIntervals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    StepParticipantId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndKind = table.Column<int>(type: "INTEGER", nullable: true),
                    EndedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    WorkSeconds = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepWorkIntervals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepWorkIntervals_StepParticipants_StepParticipantId",
                        column: x => x.StepParticipantId,
                        principalTable: "StepParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepWorkIntervals_Users_EndedByUserId",
                        column: x => x.EndedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StepWorkIntervals_EndedByUserId",
                table: "StepWorkIntervals",
                column: "EndedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StepWorkIntervals_StepParticipantId",
                table: "StepWorkIntervals",
                column: "StepParticipantId",
                unique: true);
            // BR-28 düzeni: dolu başlangıç ya da bitiş anı değişmez, boştan doluya geçiş ve kapanış alanları serbest.
            migrationBuilder.Sql("CREATE TRIGGER \"TR_StepWorkIntervals_NoTimeChange\" BEFORE UPDATE ON \"StepWorkIntervals\" FOR EACH ROW WHEN (OLD.\"StartedAtUtc\" IS NOT NULL AND NEW.\"StartedAtUtc\" IS NOT OLD.\"StartedAtUtc\") OR (OLD.\"EndedAtUtc\" IS NOT NULL AND NEW.\"EndedAtUtc\" IS NOT OLD.\"EndedAtUtc\") BEGIN SELECT RAISE(ABORT, 'Çalışma aralığının zamanı değiştirilemez.'); END;");
            migrationBuilder.Sql("CREATE TRIGGER \"TR_StepWorkIntervals_NoDelete\" BEFORE DELETE ON \"StepWorkIntervals\" FOR EACH ROW BEGIN SELECT RAISE(ABORT, 'Çalışma aralığı silinemez.'); END;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StepWorkIntervals");
        }
    }
}

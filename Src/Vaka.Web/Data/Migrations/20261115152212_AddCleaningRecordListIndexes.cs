using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCleaningRecordListIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_OpenedAtUtc",
                table: "CleaningRecords",
                column: "OpenedAtUtc");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_Status_OpenedAtUtc",
                table: "CleaningRecords",
                columns: new[] { "Status", "OpenedAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CleaningRecords_OpenedAtUtc",
                table: "CleaningRecords");

            migrationBuilder.DropIndex(
                name: "IX_CleaningRecords_Status_OpenedAtUtc",
                table: "CleaningRecords");
        }
    }
}

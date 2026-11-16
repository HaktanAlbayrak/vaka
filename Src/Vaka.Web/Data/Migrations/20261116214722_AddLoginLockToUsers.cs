using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginLockToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoginFailedCount",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LoginLockedUntilUtc",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Key", "Unit", "Value" },
                values: new object[] { 7, "LoginLockMinutes", "dakika", 15 });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DropColumn(
                name: "LoginFailedCount",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LoginLockedUntilUtc",
                table: "Users");
        }
    }
}

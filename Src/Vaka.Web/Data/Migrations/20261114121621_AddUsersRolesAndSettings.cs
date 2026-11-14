using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUsersRolesAndSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.CheckConstraint("CK_SystemSettings_Value", "\"Value\" > 0");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RegistryNo = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false, collation: "NOCASE"),
                    FullName = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: false),
                    PinHash = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    MustChangePassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    MustSetPin = table.Column<bool>(type: "INTEGER", nullable: false),
                    PinFailedCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    PinLockedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_Users_PinFailedCount", "\"PinFailedCount\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "SettingChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SettingId = table.Column<int>(type: "INTEGER", nullable: false),
                    OldValue = table.Column<int>(type: "INTEGER", nullable: false),
                    NewValue = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChangedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SettingChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SettingChanges_SystemSettings_SettingId",
                        column: x => x.SettingId,
                        principalTable: "SystemSettings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SettingChanges_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.Role });
                    table.CheckConstraint("CK_UserRoles_Role", "\"Role\" IN ('Operator', 'Manager', 'Auditor', 'SuperAdmin')");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "SystemSettings",
                columns: new[] { "Id", "Key", "Unit", "Value" },
                values: new object[,]
                {
                    { 1, "SessionMinutes", "dakika", 30 },
                    { 2, "PinLength", "hane", 4 },
                    { 3, "PinAttemptLimit", "deneme", 5 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAtUtc", "FullName", "IsActive", "MustChangePassword", "MustSetPin", "PasswordHash", "PinHash", "PinLockedAtUtc", "RegistryNo" },
                values: new object[] { 1, new DateTime(2026, 11, 14, 0, 0, 0, 0, DateTimeKind.Utc), "Süper Admin", true, true, true, "AQAAAAIAAYagAAAAEP7pSGzAxhrALT7qPx0axoziu+k8Bm98QYmCO4lubR57uLg3035M3nigzCHOMojTwA==", null, null, "0001" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { "SuperAdmin", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_SettingChanges_ChangedByUserId",
                table: "SettingChanges",
                column: "ChangedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SettingChanges_SettingId_ChangedAtUtc",
                table: "SettingChanges",
                columns: new[] { "SettingId", "ChangedAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Key",
                table: "SystemSettings",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Role",
                table: "UserRoles",
                column: "Role");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RegistryNo",
                table: "Users",
                column: "RegistryNo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SettingChanges");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

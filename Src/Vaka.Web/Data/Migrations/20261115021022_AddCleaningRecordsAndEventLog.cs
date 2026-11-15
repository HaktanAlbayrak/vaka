using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Vaka.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCleaningRecordsAndEventLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CleaningRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordNo = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false, collation: "NOCASE"),
                    FieldBookRef = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true, collation: "NOCASE"),
                    Kind = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MachineId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcedureVersionId = table.Column<int>(type: "INTEGER", nullable: false),
                    OwnerUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    FacilityCodeSnapshot = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    FacilityNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    LineCodeSnapshot = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    LineNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    MachineCodeSnapshot = table.Column<string>(type: "TEXT", maxLength: 16, nullable: false),
                    MachineNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    WorkOrderNo = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Note = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    EmergencyDescription = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    LockMinutes = table.Column<int>(type: "INTEGER", nullable: true),
                    LockExpiresAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    OpenedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ClosureKind = table.Column<int>(type: "INTEGER", nullable: true),
                    ClosedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClosureReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    GrossSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    BreakSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    HoldSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    NetSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    EffortSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    Version = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningRecords", x => x.Id);
                    table.CheckConstraint("CK_CleaningRecords_Emergency", "\"Kind\" = 1 OR \"EmergencyDescription\" IS NOT NULL");
                    table.CheckConstraint("CK_CleaningRecords_Kind", "\"Kind\" IN (1, 2)");
                    table.CheckConstraint("CK_CleaningRecords_LockMinutes", "\"LockMinutes\" IS NULL OR \"LockMinutes\" > 0");
                    table.CheckConstraint("CK_CleaningRecords_Seconds", "(\"GrossSeconds\" IS NULL OR \"GrossSeconds\" >= 0) AND (\"BreakSeconds\" IS NULL OR \"BreakSeconds\" >= 0) AND (\"HoldSeconds\" IS NULL OR \"HoldSeconds\" >= 0) AND (\"NetSeconds\" IS NULL OR \"NetSeconds\" >= 0) AND (\"EffortSeconds\" IS NULL OR \"EffortSeconds\" >= 0)");
                    table.CheckConstraint("CK_CleaningRecords_Status", "\"Status\" IN (1, 2, 3, 4, 5, 6, 7)");
                    table.ForeignKey(
                        name: "FK_CleaningRecords_Machines_MachineId",
                        column: x => x.MachineId,
                        principalTable: "Machines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CleaningRecords_ProcedureVersions_ProcedureVersionId",
                        column: x => x.ProcedureVersionId,
                        principalTable: "ProcedureVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CleaningRecords_Users_ClosedByUserId",
                        column: x => x.ClosedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CleaningRecords_Users_OwnerUserId",
                        column: x => x.OwnerUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NumberSequences",
                columns: table => new
                {
                    Scope = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false),
                    LastValue = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberSequences", x => new { x.Scope, x.Year });
                    table.CheckConstraint("CK_NumberSequences_LastValue", "\"LastValue\" >= 0");
                });

            migrationBuilder.CreateTable(
                name: "ProcessedCommands",
                columns: table => new
                {
                    CommandId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    CommandType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ResultJson = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessedCommands", x => x.CommandId);
                    table.CheckConstraint("CK_ProcessedCommands_CommandId", "length(\"CommandId\") = 36");
                    table.ForeignKey(
                        name: "FK_ProcessedCommands_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CleaningEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CleaningRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    Sequence = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    PayloadJson = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    ActorUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    OccurredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CommandId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: true),
                    PreviousHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: true),
                    Hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CleaningEvents", x => x.Id);
                    table.CheckConstraint("CK_CleaningEvents_CommandId", "\"CommandId\" IS NULL OR length(\"CommandId\") = 36");
                    table.CheckConstraint("CK_CleaningEvents_HashFormat", "length(\"Hash\") = 64 AND \"Hash\" NOT GLOB '*[^0-9a-f]*'");
                    table.CheckConstraint("CK_CleaningEvents_PreviousHashFormat", "\"PreviousHash\" IS NULL OR (length(\"PreviousHash\") = 64 AND \"PreviousHash\" NOT GLOB '*[^0-9a-f]*')");
                    table.CheckConstraint("CK_CleaningEvents_PreviousHashPresence", "(\"Sequence\" = 1 AND \"PreviousHash\" IS NULL) OR (\"Sequence\" > 1 AND \"PreviousHash\" IS NOT NULL)");
                    table.CheckConstraint("CK_CleaningEvents_Sequence", "\"Sequence\" > 0");
                    table.ForeignKey(
                        name: "FK_CleaningEvents_CleaningRecords_CleaningRecordId",
                        column: x => x.CleaningRecordId,
                        principalTable: "CleaningRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CleaningEvents_Users_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordMaterials",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CleaningRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialLotId = table.Column<int>(type: "INTEGER", nullable: false),
                    MaterialCodeSnapshot = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false, collation: "NOCASE"),
                    MaterialNameSnapshot = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    LotNoSnapshot = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false, collation: "NOCASE"),
                    ExpiryDateSnapshot = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ExpiredAtEntry = table.Column<bool>(type: "INTEGER", nullable: false),
                    ExpiredReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    EnteredByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    EnteredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RemovedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordMaterials", x => x.Id);
                    table.CheckConstraint("CK_RecordMaterials_ExpiredReason", "\"ExpiredAtEntry\" = 0 OR \"ExpiredReason\" IS NOT NULL");
                    table.ForeignKey(
                        name: "FK_RecordMaterials_CleaningRecords_CleaningRecordId",
                        column: x => x.CleaningRecordId,
                        principalTable: "CleaningRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordMaterials_MaterialLots_MaterialLotId",
                        column: x => x.MaterialLotId,
                        principalTable: "MaterialLots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordMaterials_Users_EnteredByUserId",
                        column: x => x.EnteredByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordPhases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CleaningRecordId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcedurePhaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    MinDurationSecondsSnapshot = table.Column<int>(type: "INTEGER", nullable: true),
                    GrossSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    BreakSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    HoldSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    NetSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    BelowMinimum = table.Column<bool>(type: "INTEGER", nullable: false),
                    BelowMinimumReason = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordPhases", x => x.Id);
                    table.CheckConstraint("CK_RecordPhases_MinDuration", "\"MinDurationSecondsSnapshot\" IS NULL OR \"MinDurationSecondsSnapshot\" > 0");
                    table.CheckConstraint("CK_RecordPhases_Seconds", "(\"GrossSeconds\" IS NULL OR \"GrossSeconds\" >= 0) AND (\"BreakSeconds\" IS NULL OR \"BreakSeconds\" >= 0) AND (\"HoldSeconds\" IS NULL OR \"HoldSeconds\" >= 0) AND (\"NetSeconds\" IS NULL OR \"NetSeconds\" >= 0)");
                    table.CheckConstraint("CK_RecordPhases_Status", "\"Status\" IN (1, 2, 3, 4, 5)");
                    table.ForeignKey(
                        name: "FK_RecordPhases_CleaningRecords_CleaningRecordId",
                        column: x => x.CleaningRecordId,
                        principalTable: "CleaningRecords",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordPhases_ProcedurePhases_ProcedurePhaseId",
                        column: x => x.ProcedurePhaseId,
                        principalTable: "ProcedurePhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecordSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordPhaseId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcedureStepId = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    StartedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    EndedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndedByUserId = table.Column<int>(type: "INTEGER", nullable: true),
                    BreakSeconds = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    DurationSeconds = table.Column<int>(type: "INTEGER", nullable: true),
                    EffortSeconds = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordSteps", x => x.Id);
                    table.CheckConstraint("CK_RecordSteps_BreakSeconds", "\"BreakSeconds\" >= 0");
                    table.CheckConstraint("CK_RecordSteps_DurationSeconds", "\"DurationSeconds\" IS NULL OR \"DurationSeconds\" >= 0");
                    table.CheckConstraint("CK_RecordSteps_EffortSeconds", "\"EffortSeconds\" IS NULL OR \"EffortSeconds\" >= 0");
                    table.CheckConstraint("CK_RecordSteps_Status", "\"Status\" IN (1, 2, 3, 4)");
                    table.ForeignKey(
                        name: "FK_RecordSteps_ProcedureSteps_ProcedureStepId",
                        column: x => x.ProcedureStepId,
                        principalTable: "ProcedureSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordSteps_RecordPhases_RecordPhaseId",
                        column: x => x.RecordPhaseId,
                        principalTable: "RecordPhases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordSteps_Users_EndedByUserId",
                        column: x => x.EndedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecordSteps_Users_StartedByUserId",
                        column: x => x.StartedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepParticipants",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RecordStepId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsWorking = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    AddedByUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    ConfirmedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepParticipants_RecordSteps_RecordStepId",
                        column: x => x.RecordStepId,
                        principalTable: "RecordSteps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepParticipants_Users_AddedByUserId",
                        column: x => x.AddedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StepParticipants_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CleaningEvents_ActorUserId",
                table: "CleaningEvents",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningEvents_CleaningRecordId_Sequence",
                table: "CleaningEvents",
                columns: new[] { "CleaningRecordId", "Sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CleaningEvents_CommandId",
                table: "CleaningEvents",
                column: "CommandId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_ClosedByUserId",
                table: "CleaningRecords",
                column: "ClosedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_FieldBookRef",
                table: "CleaningRecords",
                column: "FieldBookRef",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_MachineId",
                table: "CleaningRecords",
                column: "MachineId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_OneActivePerMachine",
                table: "CleaningRecords",
                column: "MachineId",
                unique: true,
                filter: "\"Status\" IN (1, 2, 3)");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_OneActivePerOwner",
                table: "CleaningRecords",
                column: "OwnerUserId",
                unique: true,
                filter: "\"Status\" IN (1, 2)");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_OwnerUserId",
                table: "CleaningRecords",
                column: "OwnerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_ProcedureVersionId",
                table: "CleaningRecords",
                column: "ProcedureVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_CleaningRecords_RecordNo",
                table: "CleaningRecords",
                column: "RecordNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProcessedCommands_UserId",
                table: "ProcessedCommands",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordMaterials_EnteredByUserId",
                table: "RecordMaterials",
                column: "EnteredByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordMaterials_MaterialLotId",
                table: "RecordMaterials",
                column: "MaterialLotId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordMaterials_OneActiveLotPerRecord",
                table: "RecordMaterials",
                columns: new[] { "CleaningRecordId", "MaterialLotId" },
                unique: true,
                filter: "\"RemovedAtUtc\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecordPhases_CleaningRecordId_ProcedurePhaseId",
                table: "RecordPhases",
                columns: new[] { "CleaningRecordId", "ProcedurePhaseId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecordPhases_ProcedurePhaseId",
                table: "RecordPhases",
                column: "ProcedurePhaseId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordSteps_EndedByUserId",
                table: "RecordSteps",
                column: "EndedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordSteps_ProcedureStepId",
                table: "RecordSteps",
                column: "ProcedureStepId");

            migrationBuilder.CreateIndex(
                name: "IX_RecordSteps_RecordPhaseId_ProcedureStepId",
                table: "RecordSteps",
                columns: new[] { "RecordPhaseId", "ProcedureStepId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecordSteps_StartedByUserId",
                table: "RecordSteps",
                column: "StartedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StepParticipants_AddedByUserId",
                table: "StepParticipants",
                column: "AddedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_StepParticipants_OneWorkingPerUser",
                table: "StepParticipants",
                column: "UserId",
                unique: true,
                filter: "\"IsWorking\" = 1");

            migrationBuilder.CreateIndex(
                name: "IX_StepParticipants_RecordStepId_UserId",
                table: "StepParticipants",
                columns: new[] { "RecordStepId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepParticipants_UserId",
                table: "StepParticipants",
                column: "UserId");

            foreach (var statement in GuardTriggers())
            {
                migrationBuilder.Sql(statement);
            }
        }

        private static IEnumerable<string> GuardTriggers()
        {
            // BR-31: olay günlüğü yalnız eklenir.
            yield return Trigger("TR_CleaningEvents_NoUpdate", "UPDATE", "CleaningEvents", null, "Olay günlüğü değiştirilemez.");
            yield return Trigger("TR_CleaningEvents_NoDelete", "DELETE", "CleaningEvents", null, "Olay günlüğü değiştirilemez.");

            // BR-28: dolu başlangıç ya da bitiş anı değişmez, boştan doluya geçiş serbest.
            yield return Trigger(
                "TR_RecordSteps_NoTimeChange",
                "UPDATE",
                "RecordSteps",
                "(OLD.\"StartedAtUtc\" IS NOT NULL AND NEW.\"StartedAtUtc\" IS NOT OLD.\"StartedAtUtc\") OR (OLD.\"EndedAtUtc\" IS NOT NULL AND NEW.\"EndedAtUtc\" IS NOT OLD.\"EndedAtUtc\")",
                "Başlamış ya da bitmiş adımın zamanı değiştirilemez.");

            // V-07: kullanılmış numara yeniden kullanılmaz; sayaç yalnız artar, satır silinmez.
            yield return Trigger("TR_NumberSequences_NoDelete", "DELETE", "NumberSequences", null, "Numara sayacı silinemez.");
            yield return Trigger(
                "TR_NumberSequences_OnlyIncrease",
                "UPDATE",
                "NumberSequences",
                "NEW.\"LastValue\" <= OLD.\"LastValue\" OR NEW.\"Scope\" IS NOT OLD.\"Scope\" OR NEW.\"Year\" IS NOT OLD.\"Year\"",
                "Numara sayacı yalnız artırılabilir.");
        }

        private static string Trigger(string name, string operation, string table, string when, string message)
        {
            var whenClause = when is null ? string.Empty : $" WHEN {when}";
            return $"CREATE TRIGGER \"{name}\" BEFORE {operation} ON \"{table}\" FOR EACH ROW{whenClause} BEGIN SELECT RAISE(ABORT, '{message}'); END;";
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CleaningEvents");

            migrationBuilder.DropTable(
                name: "NumberSequences");

            migrationBuilder.DropTable(
                name: "ProcessedCommands");

            migrationBuilder.DropTable(
                name: "RecordMaterials");

            migrationBuilder.DropTable(
                name: "StepParticipants");

            migrationBuilder.DropTable(
                name: "RecordSteps");

            migrationBuilder.DropTable(
                name: "RecordPhases");

            migrationBuilder.DropTable(
                name: "CleaningRecords");
        }
    }
}

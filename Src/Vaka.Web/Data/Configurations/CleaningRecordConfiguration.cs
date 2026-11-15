using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class CleaningRecordConfiguration : IEntityTypeConfiguration<CleaningRecord>
{
    public void Configure(EntityTypeBuilder<CleaningRecord> builder)
    {
        builder.ToTable("CleaningRecords", t =>
        {
            t.HasCheckConstraint("CK_CleaningRecords_Kind", "\"Kind\" IN (1, 2)");
            t.HasCheckConstraint("CK_CleaningRecords_Status", "\"Status\" IN (1, 2, 3, 4, 5, 6, 7)");
            t.HasCheckConstraint("CK_CleaningRecords_Emergency", "\"Kind\" = 1 OR \"EmergencyDescription\" IS NOT NULL");
            t.HasCheckConstraint("CK_CleaningRecords_LockMinutes", "\"LockMinutes\" IS NULL OR \"LockMinutes\" > 0");
            t.HasCheckConstraint("CK_CleaningRecords_Seconds",
                "(\"GrossSeconds\" IS NULL OR \"GrossSeconds\" >= 0) AND (\"BreakSeconds\" IS NULL OR \"BreakSeconds\" >= 0) AND (\"HoldSeconds\" IS NULL OR \"HoldSeconds\" >= 0) AND (\"NetSeconds\" IS NULL OR \"NetSeconds\" >= 0) AND (\"EffortSeconds\" IS NULL OR \"EffortSeconds\" >= 0)");
        });
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RecordNo).IsRequired().HasMaxLength(64).UseCollation("NOCASE");
        builder.Property(r => r.FieldBookRef).HasMaxLength(64).UseCollation("NOCASE");
        builder.Property(r => r.Kind).HasConversion<int>();
        builder.Property(r => r.Status).HasConversion<int>();
        builder.Property(r => r.FacilityCodeSnapshot).IsRequired().HasMaxLength(16);
        builder.Property(r => r.FacilityNameSnapshot).IsRequired().HasMaxLength(120);
        builder.Property(r => r.LineCodeSnapshot).IsRequired().HasMaxLength(16);
        builder.Property(r => r.LineNameSnapshot).IsRequired().HasMaxLength(120);
        builder.Property(r => r.MachineCodeSnapshot).IsRequired().HasMaxLength(16);
        builder.Property(r => r.MachineNameSnapshot).IsRequired().HasMaxLength(120);
        builder.Property(r => r.WorkOrderNo).HasMaxLength(64);
        builder.Property(r => r.Note).HasMaxLength(1000);
        builder.Property(r => r.EmergencyDescription).HasMaxLength(1000);
        builder.Property(r => r.ClosureReason).HasMaxLength(1000);
        builder.Property(r => r.Version).IsConcurrencyToken().HasDefaultValue(0);

        builder.HasIndex(r => r.RecordNo).IsUnique();
        builder.HasIndex(r => r.FieldBookRef).IsUnique();
        builder.HasIndex(r => r.MachineId);
        builder.HasIndex(r => r.ProcedureVersionId);
        builder.HasIndex(r => r.OwnerUserId);
        builder.HasIndex(r => r.ClosedByUserId);
        builder.HasIndex(r => r.OpenedAtUtc);
        builder.HasIndex(r => new { r.Status, r.OpenedAtUtc });
        builder.HasIndex(r => r.MachineId, "IX_CleaningRecords_OneActivePerMachine").IsUnique().HasFilter("\"Status\" IN (1, 2, 3)");
        builder.HasIndex(r => r.OwnerUserId, "IX_CleaningRecords_OneActivePerOwner").IsUnique().HasFilter("\"Status\" IN (1, 2)");

        builder.HasOne(r => r.Machine).WithMany().HasForeignKey(r => r.MachineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.ProcedureVersion).WithMany().HasForeignKey(r => r.ProcedureVersionId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.Owner).WithMany().HasForeignKey(r => r.OwnerUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(r => r.ClosedBy).WithMany().HasForeignKey(r => r.ClosedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

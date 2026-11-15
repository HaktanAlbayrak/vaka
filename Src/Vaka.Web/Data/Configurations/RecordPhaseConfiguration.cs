using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class RecordPhaseConfiguration : IEntityTypeConfiguration<RecordPhase>
{
    public void Configure(EntityTypeBuilder<RecordPhase> builder)
    {
        builder.ToTable("RecordPhases", t =>
        {
            t.HasCheckConstraint("CK_RecordPhases_Status", "\"Status\" IN (1, 2, 3, 4, 5)");
            t.HasCheckConstraint("CK_RecordPhases_MinDuration", "\"MinDurationSecondsSnapshot\" IS NULL OR \"MinDurationSecondsSnapshot\" > 0");
            t.HasCheckConstraint("CK_RecordPhases_Seconds",
                "(\"GrossSeconds\" IS NULL OR \"GrossSeconds\" >= 0) AND (\"BreakSeconds\" IS NULL OR \"BreakSeconds\" >= 0) AND (\"HoldSeconds\" IS NULL OR \"HoldSeconds\" >= 0) AND (\"NetSeconds\" IS NULL OR \"NetSeconds\" >= 0)");
        });
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Status).HasConversion<int>();
        builder.Property(p => p.BelowMinimumReason).HasMaxLength(500);
        builder.HasIndex(p => new { p.CleaningRecordId, p.ProcedurePhaseId }).IsUnique();
        builder.HasIndex(p => p.ProcedurePhaseId);
        builder.HasOne(p => p.Record).WithMany(r => r.Phases).HasForeignKey(p => p.CleaningRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.ProcedurePhase).WithMany().HasForeignKey(p => p.ProcedurePhaseId).OnDelete(DeleteBehavior.Restrict);
    }
}

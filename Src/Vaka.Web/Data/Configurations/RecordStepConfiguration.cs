using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class RecordStepConfiguration : IEntityTypeConfiguration<RecordStep>
{
    public void Configure(EntityTypeBuilder<RecordStep> builder)
    {
        builder.ToTable("RecordSteps", t =>
        {
            t.HasCheckConstraint("CK_RecordSteps_Status", "\"Status\" IN (1, 2, 3, 4)");
            t.HasCheckConstraint("CK_RecordSteps_BreakSeconds", "\"BreakSeconds\" >= 0");
            t.HasCheckConstraint("CK_RecordSteps_DurationSeconds", "\"DurationSeconds\" IS NULL OR \"DurationSeconds\" >= 0");
            t.HasCheckConstraint("CK_RecordSteps_EffortSeconds", "\"EffortSeconds\" IS NULL OR \"EffortSeconds\" >= 0");
        });
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Status).HasConversion<int>();
        builder.Property(s => s.BreakSeconds).HasDefaultValue(0);
        builder.HasIndex(s => new { s.RecordPhaseId, s.ProcedureStepId }).IsUnique();
        builder.HasIndex(s => s.ProcedureStepId);
        builder.HasIndex(s => s.StartedByUserId);
        builder.HasIndex(s => s.EndedByUserId);
        builder.HasOne(s => s.Phase).WithMany(p => p.Steps).HasForeignKey(s => s.RecordPhaseId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.ProcedureStep).WithMany().HasForeignKey(s => s.ProcedureStepId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.StartedBy).WithMany().HasForeignKey(s => s.StartedByUserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(s => s.EndedBy).WithMany().HasForeignKey(s => s.EndedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

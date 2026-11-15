using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class ProcedurePhaseConfiguration : IEntityTypeConfiguration<ProcedurePhase>
{
    public void Configure(EntityTypeBuilder<ProcedurePhase> builder)
    {
        builder.ToTable("ProcedurePhases", t =>
        {
            t.HasCheckConstraint("CK_ProcedurePhases_OrderNo", "\"OrderNo\" > 0");
            t.HasCheckConstraint("CK_ProcedurePhases_MinDuration", "\"MinDurationSeconds\" IS NULL OR \"MinDurationSeconds\" > 0");
        });
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).IsRequired().HasMaxLength(120);
        builder.HasIndex(p => new { p.ProcedureVersionId, p.OrderNo });
        builder.HasOne(p => p.Version).WithMany(v => v.Phases).HasForeignKey(p => p.ProcedureVersionId).OnDelete(DeleteBehavior.Restrict);
    }
}

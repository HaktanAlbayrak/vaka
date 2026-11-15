using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class ProcedureStepConfiguration : IEntityTypeConfiguration<ProcedureStep>
{
    public void Configure(EntityTypeBuilder<ProcedureStep> builder)
    {
        builder.ToTable("ProcedureSteps", t => t.HasCheckConstraint("CK_ProcedureSteps_OrderNo", "\"OrderNo\" > 0"));
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Title).IsRequired().HasMaxLength(120);
        builder.Property(s => s.Description).HasMaxLength(1000);
        builder.HasIndex(s => new { s.ProcedurePhaseId, s.OrderNo });
        builder.HasOne(s => s.Phase).WithMany(p => p.Steps).HasForeignKey(s => s.ProcedurePhaseId).OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class StepMediaConfiguration : IEntityTypeConfiguration<StepMedia>
{
    public void Configure(EntityTypeBuilder<StepMedia> builder)
    {
        builder.ToTable("StepMedia", t =>
        {
            t.HasCheckConstraint("CK_StepMedia_MediaType", "\"MediaType\" IN (1, 2)");
            t.HasCheckConstraint("CK_StepMedia_Sha256", "length(\"Sha256\") = 64");
            t.HasCheckConstraint("CK_StepMedia_SizeBytes", "\"SizeBytes\" > 0");
        });
        builder.HasKey(m => m.Id);
        builder.Property(m => m.MediaType).HasConversion<int>();
        builder.Property(m => m.Sha256).IsRequired().HasMaxLength(64);
        builder.Property(m => m.OriginalFileName).IsRequired().HasMaxLength(255);
        builder.Property(m => m.ContentType).IsRequired().HasMaxLength(64);
        builder.HasIndex(m => m.ProcedureStepId);
        builder.HasOne(m => m.Step).WithMany(s => s.Media).HasForeignKey(m => m.ProcedureStepId).OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class StepWorkIntervalConfiguration : IEntityTypeConfiguration<StepWorkInterval>
{
    public void Configure(EntityTypeBuilder<StepWorkInterval> builder)
    {
        builder.ToTable("StepWorkIntervals");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.EndKind).HasConversion<int?>();
        builder.HasIndex(i => i.StepParticipantId).IsUnique();
        builder.HasIndex(i => i.EndedByUserId);
        builder.HasOne(i => i.Participant).WithOne(p => p.WorkInterval).HasForeignKey<StepWorkInterval>(i => i.StepParticipantId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(i => i.EndedBy).WithMany().HasForeignKey(i => i.EndedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

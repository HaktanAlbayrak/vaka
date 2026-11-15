using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class StepParticipantConfiguration : IEntityTypeConfiguration<StepParticipant>
{
    public void Configure(EntityTypeBuilder<StepParticipant> builder)
    {
        builder.ToTable("StepParticipants");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.IsWorking).HasDefaultValue(false);
        builder.HasIndex(p => new { p.RecordStepId, p.UserId }).IsUnique();
        builder.HasIndex(p => p.UserId);
        builder.HasIndex(p => p.AddedByUserId);
        builder.HasIndex(p => p.UserId, "IX_StepParticipants_OneWorkingPerUser").IsUnique().HasFilter("\"IsWorking\" = 1");
        builder.HasOne(p => p.Step).WithMany(s => s.Participants).HasForeignKey(p => p.RecordStepId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(p => p.AddedBy).WithMany().HasForeignKey(p => p.AddedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

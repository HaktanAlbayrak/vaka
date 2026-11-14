using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class SettingChangeConfiguration : IEntityTypeConfiguration<SettingChange>
{
    public void Configure(EntityTypeBuilder<SettingChange> builder)
    {
        builder.ToTable("SettingChanges");
        builder.HasKey(c => c.Id);

        builder.HasOne(c => c.Setting)
            .WithMany()
            .HasForeignKey(c => c.SettingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.ChangedByUser)
            .WithMany()
            .HasForeignKey(c => c.ChangedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.SettingId, c.ChangedAtUtc });
        builder.HasIndex(c => c.ChangedByUserId);
    }
}

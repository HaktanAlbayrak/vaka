using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class ProcedureVersionConfiguration : IEntityTypeConfiguration<ProcedureVersion>
{
    public void Configure(EntityTypeBuilder<ProcedureVersion> builder)
    {
        builder.ToTable("ProcedureVersions", t =>
        {
            t.HasCheckConstraint("CK_ProcedureVersions_Status", "\"Status\" IN (1, 2, 3)");
            t.HasCheckConstraint("CK_ProcedureVersions_VersionNo", "\"VersionNo\" > 0");
            t.HasCheckConstraint("CK_ProcedureVersions_Published", "(\"Status\" = 1 AND \"PublishedAtUtc\" IS NULL) OR (\"Status\" <> 1 AND \"PublishedAtUtc\" IS NOT NULL)");
        });
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Status).HasConversion<int>();
        builder.Property(v => v.DocumentRef).HasMaxLength(64);
        builder.HasIndex(v => new { v.CleaningTypeId, v.VersionNo }).IsUnique();
        builder.HasIndex(v => v.CleaningTypeId, "IX_ProcedureVersions_OneCurrent").IsUnique().HasFilter("\"Status\" = 2");
        builder.HasIndex(v => v.CleaningTypeId, "IX_ProcedureVersions_OneDraft").IsUnique().HasFilter("\"Status\" = 1");
        builder.HasIndex(v => v.PublishedByUserId);
        builder.HasOne(v => v.CleaningType).WithMany(c => c.Versions).HasForeignKey(v => v.CleaningTypeId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(v => v.PublishedBy).WithMany().HasForeignKey(v => v.PublishedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

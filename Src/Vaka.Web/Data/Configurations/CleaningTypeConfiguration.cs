using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class CleaningTypeConfiguration : IEntityTypeConfiguration<CleaningType>
{
    public void Configure(EntityTypeBuilder<CleaningType> builder)
    {
        builder.ToTable("CleaningTypes", t =>
        {
            t.HasCheckConstraint("CK_CleaningTypes_Kind", "\"Kind\" IN (1, 2)");
            t.HasCheckConstraint("CK_CleaningTypes_LockMinutes", "(\"Kind\" = 1 AND \"DefaultLockMinutes\" > 0) OR (\"Kind\" = 2 AND \"DefaultLockMinutes\" IS NULL)");
        });
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Code).IsRequired().HasMaxLength(16).UseCollation("NOCASE");
        builder.Property(c => c.Name).IsRequired().HasMaxLength(120);
        builder.Property(c => c.Kind).HasConversion<int>();
        builder.Property(c => c.IsActive).HasDefaultValue(true);
        builder.HasIndex(c => c.Code).IsUnique();
    }
}

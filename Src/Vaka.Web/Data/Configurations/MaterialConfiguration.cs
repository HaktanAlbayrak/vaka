using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> builder)
    {
        builder.ToTable("Materials");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Code).IsRequired().HasMaxLength(32).UseCollation("NOCASE");
        builder.Property(m => m.Name).IsRequired().HasMaxLength(120);
        builder.Property(m => m.IsActive).HasDefaultValue(true);
        builder.HasIndex(m => m.Code).IsUnique();
    }
}

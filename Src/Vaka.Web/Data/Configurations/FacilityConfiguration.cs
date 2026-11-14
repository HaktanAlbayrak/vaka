using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facilities");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Code).IsRequired().HasMaxLength(16).UseCollation("NOCASE");
        builder.Property(f => f.Name).IsRequired().HasMaxLength(120);
        builder.Property(f => f.IsActive).HasDefaultValue(true);
        builder.HasIndex(f => f.Code).IsUnique();
    }
}

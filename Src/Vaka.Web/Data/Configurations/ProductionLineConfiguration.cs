using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class ProductionLineConfiguration : IEntityTypeConfiguration<ProductionLine>
{
    public void Configure(EntityTypeBuilder<ProductionLine> builder)
    {
        builder.ToTable("ProductionLines");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Code).IsRequired().HasMaxLength(16).UseCollation("NOCASE");
        builder.Property(l => l.Name).IsRequired().HasMaxLength(120);
        builder.Property(l => l.IsActive).HasDefaultValue(true);
        builder.HasIndex(l => new { l.FacilityId, l.Code }).IsUnique();
        builder.HasOne(l => l.Facility).WithMany(f => f.Lines).HasForeignKey(l => l.FacilityId).OnDelete(DeleteBehavior.Restrict);
    }
}

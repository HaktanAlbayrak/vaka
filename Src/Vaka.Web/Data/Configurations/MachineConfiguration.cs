using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.ToTable("Machines", t => t.HasCheckConstraint("CK_Machines_RetiredInactive", "\"RetiredAtUtc\" IS NULL OR \"IsActive\" = 0"));
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Code).IsRequired().HasMaxLength(16).UseCollation("NOCASE");
        builder.Property(m => m.Name).IsRequired().HasMaxLength(120);
        builder.Property(m => m.IsActive).HasDefaultValue(true);
        builder.HasIndex(m => new { m.ProductionLineId, m.Code }).IsUnique();
        builder.HasOne(m => m.Line).WithMany(l => l.Machines).HasForeignKey(m => m.ProductionLineId).OnDelete(DeleteBehavior.Restrict);
    }
}

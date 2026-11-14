using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class MaterialLotConfiguration : IEntityTypeConfiguration<MaterialLot>
{
    public void Configure(EntityTypeBuilder<MaterialLot> builder)
    {
        builder.ToTable("MaterialLots");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.LotNo).IsRequired().HasMaxLength(64).UseCollation("NOCASE");
        builder.HasIndex(l => new { l.MaterialId, l.LotNo }).IsUnique();
        builder.HasOne(l => l.Material).WithMany(m => m.Lots).HasForeignKey(l => l.MaterialId).OnDelete(DeleteBehavior.Restrict);
    }
}

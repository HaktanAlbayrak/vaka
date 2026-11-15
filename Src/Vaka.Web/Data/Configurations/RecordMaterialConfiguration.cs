using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class RecordMaterialConfiguration : IEntityTypeConfiguration<RecordMaterial>
{
    public void Configure(EntityTypeBuilder<RecordMaterial> builder)
    {
        builder.ToTable("RecordMaterials", t =>
            t.HasCheckConstraint("CK_RecordMaterials_ExpiredReason", "\"ExpiredAtEntry\" = 0 OR \"ExpiredReason\" IS NOT NULL"));
        builder.HasKey(m => m.Id);
        builder.Property(m => m.MaterialCodeSnapshot).IsRequired().HasMaxLength(32).UseCollation("NOCASE");
        builder.Property(m => m.MaterialNameSnapshot).IsRequired().HasMaxLength(120);
        builder.Property(m => m.LotNoSnapshot).IsRequired().HasMaxLength(64).UseCollation("NOCASE");
        builder.Property(m => m.ExpiredReason).HasMaxLength(500);
        builder.HasIndex(m => m.MaterialLotId);
        builder.HasIndex(m => m.EnteredByUserId);
        builder.HasIndex(m => new { m.CleaningRecordId, m.MaterialLotId }, "IX_RecordMaterials_OneActiveLotPerRecord").IsUnique().HasFilter("\"RemovedAtUtc\" IS NULL");
        builder.HasOne(m => m.Record).WithMany(r => r.Materials).HasForeignKey(m => m.CleaningRecordId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.MaterialLot).WithMany().HasForeignKey(m => m.MaterialLotId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(m => m.EnteredBy).WithMany().HasForeignKey(m => m.EnteredByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

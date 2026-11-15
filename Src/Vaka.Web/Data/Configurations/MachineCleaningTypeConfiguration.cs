using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class MachineCleaningTypeConfiguration : IEntityTypeConfiguration<MachineCleaningType>
{
    public void Configure(EntityTypeBuilder<MachineCleaningType> builder)
    {
        builder.ToTable("MachineCleaningTypes");
        builder.HasKey(l => new { l.MachineId, l.CleaningTypeId });
        builder.HasIndex(l => l.CleaningTypeId);
        builder.HasOne(l => l.Machine).WithMany(m => m.CleaningTypeLinks).HasForeignKey(l => l.MachineId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(l => l.CleaningType).WithMany(c => c.MachineLinks).HasForeignKey(l => l.CleaningTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}

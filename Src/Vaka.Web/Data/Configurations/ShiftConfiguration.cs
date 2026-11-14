using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class ShiftConfiguration : IEntityTypeConfiguration<Shift>
{
    public void Configure(EntityTypeBuilder<Shift> builder)
    {
        builder.ToTable("Shifts", t => t.HasCheckConstraint("CK_Shifts_StartNotEnd", "\"StartTime\" <> \"EndTime\""));
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(80);
        builder.Property(s => s.IsActive).HasDefaultValue(true);
        builder.HasIndex(s => s.FacilityId);
        builder.HasOne(s => s.Facility).WithMany(f => f.Shifts).HasForeignKey(s => s.FacilityId).OnDelete(DeleteBehavior.Restrict);
    }
}

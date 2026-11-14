using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Data.Seed;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> builder)
    {
        builder.ToTable("SystemSettings", t => t.HasCheckConstraint("CK_SystemSettings_Value", "\"Value\" > 0"));
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Key).IsRequired().HasMaxLength(64);
        builder.Property(s => s.Unit).IsRequired().HasMaxLength(16);

        builder.HasIndex(s => s.Key).IsUnique();

        builder.HasData(SettingSeed.Rows);
    }
}

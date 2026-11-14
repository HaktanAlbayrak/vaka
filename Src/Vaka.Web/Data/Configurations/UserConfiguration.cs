using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Data.Seed;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t => t.HasCheckConstraint("CK_Users_PinFailedCount", "\"PinFailedCount\" >= 0"));
        builder.HasKey(u => u.Id);

        builder.Property(u => u.RegistryNo).IsRequired().HasMaxLength(32).UseCollation("NOCASE");
        builder.Property(u => u.FullName).IsRequired().HasMaxLength(120);
        builder.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PinHash).HasMaxLength(256);
        builder.Property(u => u.PinFailedCount).HasDefaultValue(0);
        builder.Property(u => u.IsActive).HasDefaultValue(true);

        builder.HasIndex(u => u.RegistryNo).IsUnique();

        builder.HasData(AuthSeed.SuperAdminUser);
    }
}

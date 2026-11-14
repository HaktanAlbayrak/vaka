using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vaka.Web.Authorization;
using Vaka.Web.Data.Seed;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        builder.ToTable("UserRoles", t => t.HasCheckConstraint(
            "CK_UserRoles_Role",
            $"\"Role\" IN ('{Roles.Operator}', '{Roles.Manager}', '{Roles.Auditor}', '{Roles.SuperAdmin}')"));
        builder.HasKey(r => new { r.UserId, r.Role });

        builder.Property(r => r.Role).IsRequired().HasMaxLength(32);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Roles)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.Role);

        builder.HasData(AuthSeed.SuperAdminRole);
    }
}

using Microsoft.EntityFrameworkCore;
using Vaka.Web.Domain.Entities;

namespace Vaka.Web.Data;

public class VakaDbContext : DbContext
{
    public VakaDbContext(DbContextOptions<VakaDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<SettingChange> SettingChanges => Set<SettingChange>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VakaDbContext).Assembly);
    }
}

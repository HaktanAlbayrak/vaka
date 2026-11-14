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
    public DbSet<Facility> Facilities => Set<Facility>();
    public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Shift> Shifts => Set<Shift>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<MaterialLot> MaterialLots => Set<MaterialLot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VakaDbContext).Assembly);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Vaka.Web.Data;

public class VakaDbContextFactory : IDesignTimeDbContextFactory<VakaDbContext>
{
    public const string DefaultConnectionString = "Data Source=App_Data/vaka.db";

    public VakaDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<VakaDbContext>()
            .UseSqlite(DefaultConnectionString)
            .Options;
        return new VakaDbContext(options);
    }
}

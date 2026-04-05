using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Recipes.Infrastructure;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BaseDbContext>
{
    public BaseDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();

        optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__PostgreSqlConnectionString"));

        return new BaseDbContext(optionsBuilder.Options);
    }
}
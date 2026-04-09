using Microsoft.EntityFrameworkCore;
using Recipes.Infrastructure;

namespace Recipes.API.ServiceCollectionExtension;

public static class DbContextServiceCollectionExtension
{
    public static void AddDbContext(this IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddDbContext<BaseDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("PostgreSqlConnectionString")));
    }
}
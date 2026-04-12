using Recipes.Application.Options.Implementations;

namespace Recipes.API.ServiceCollectionExtension;

public static class ConfigureServiceCollectionExtension
{
    public static void AddConfigure(this IServiceCollection services,
        ConfigurationManager configuration)
    {
        services.Configure<JwtOptions>(
            configuration.GetSection("Jwt"));

        services.Configure<ObjectStorageOptions>(
            configuration.GetSection("ObjectStorage"));
    }
}
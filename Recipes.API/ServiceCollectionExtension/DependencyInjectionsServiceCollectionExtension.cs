using Microsoft.Extensions.Options;
using Recipes.Application.Options.Implementations;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Providers;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Implementations;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Infrastructure.Repositories.Implementations;
using Recipes.Infrastructure.UnitOfWork.Implementations;

namespace Recipes.API.ServiceCollectionExtension;

public static class DependencyInjectionsServiceCollectionExtension
{
    public static void AddDependencyInjections(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ClaimsProvider>();

        services.AddSingleton<IJwtOptions>(sp =>
            sp.GetRequiredService<IOptions<JwtOptions>>().Value);
        services.AddSingleton<IJwtGenerateService, JwtGenerateService>();
    }
}
using Microsoft.Extensions.Options;
using Recipes.Application.Options.Implementations;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Providers;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Implementations;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.UserRelations;
using Recipes.Infrastructure.Repositories.Implementations;
using Recipes.Infrastructure.Services;
using Recipes.Infrastructure.UnitOfWork.Implementations;

namespace Recipes.API.ServiceCollectionExtension;

public static class DependencyInjectionsServiceCollectionExtension
{
    public static void AddDependencyInjections(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IClock, UtcClock>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserAccessService, UserAccessService>();
        services.AddScoped<IUserAvatarService, UserAvatarService>();
        services.AddScoped<IUserAuthTokenService, UserAuthTokenService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRecipeCrudService, RecipeCrudService>();
        services.AddScoped<IRecipeInteractionService, RecipeInteractionService>();
        services.AddScoped<IRecipeImageService, RecipeImageService>();
        services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IRecipeImageRepository, RecipeImageRepository>();
        services.AddScoped<IRecipeIngredientRepository, RecipeIngredientRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped(typeof(IUserIngredientRelationRepository<>), typeof(UserIngredientRelationRepository<>));
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IClaimsProvider, ClaimsProvider>();
        services
            .AddScoped<IUserIngredientRelationService<UnwantedIngredients>,
                UserIngredientRelationService<UnwantedIngredients>>();
        services
            .AddScoped<IUserIngredientRelationService<Allergens>,
                UserIngredientRelationService<Allergens>>();

        services.AddScoped<IImageStorageService, ImageStorageService>();

        services.AddSingleton<IJwtOptions>(sp =>
            sp.GetRequiredService<IOptions<JwtOptions>>().Value);
        services.AddSingleton<IJwtGenerateService, JwtGenerateService>();
        services.AddSingleton<IObjectStorageOptions>(sp =>
            sp.GetRequiredService<IOptions<ObjectStorageOptions>>().Value);
    }
}

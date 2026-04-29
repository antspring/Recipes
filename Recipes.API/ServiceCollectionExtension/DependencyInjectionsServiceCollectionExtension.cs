using Microsoft.Extensions.Options;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Providers;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Implementations;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.UserRelations;
using Recipes.Infrastructure.Repositories.Implementations;
using Recipes.Infrastructure.Options;
using Recipes.Infrastructure.Services;
using Recipes.Infrastructure.UnitOfWork.Implementations;

namespace Recipes.API.ServiceCollectionExtension;

public static class DependencyInjectionsServiceCollectionExtension
{
    public static void AddDependencyInjections(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices();
        services.AddRepositories();
        services.AddOptionsAdapters();
    }

    private static void AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IClock, UtcClock>();
        services.AddScoped<IUserAccessService, UserAccessService>();
        services.AddScoped<IUserRegistrationService, UserRegistrationService>();
        services.AddScoped<IUserAuthTokenService, UserAuthTokenService>();
        services.AddScoped<IUserProfileService, UserProfileService>();
        services.AddScoped<IUserUniquenessService, UserUniquenessService>();
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        services.AddScoped<IRecipeCrudService, RecipeCrudService>();
        services.AddScoped<IRecipeSearchService, RecipeSearchService>();
        services.AddScoped<IRecipeDtoFactory, RecipeDtoFactory>();
        services.AddScoped<IRecipeInteractionService, RecipeInteractionService>();
        services.AddScoped<IRecipeImageService, RecipeImageService>();
        services.AddScoped<IRecipeStepService, RecipeStepService>();
        services.AddScoped<ICommentImageService, CommentImageService>();
        services.AddScoped<IRecipeIngredientService, RecipeIngredientService>();
        services.AddScoped<ICommentService, CommentService>();
        services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
        services.AddScoped<IUserPublicProfileService, UserPublicProfileService>();
        services.AddScoped<IClaimsProvider, ClaimsProvider>();
        services
            .AddScoped<IUserIngredientRelationService<UnwantedIngredients>,
                UserIngredientRelationService<UnwantedIngredients>>();
        services
            .AddScoped<IUserIngredientRelationService<Allergens>,
                UserIngredientRelationService<Allergens>>();
    }

    private static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserAvatarService, UserAvatarService>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddImageStorageServices();
        services.AddSingleton<IJwtGenerateService, JwtGenerateService>();
    }

    private static void AddImageStorageServices(this IServiceCollection services)
    {
        services.AddScoped<ImageStorageService>();
        services.AddScoped<IImageStorageService>(sp => sp.GetRequiredService<ImageStorageService>());
        services.AddScoped<IImageUrlProvider>(sp => sp.GetRequiredService<ImageStorageService>());
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRecipeRepository, RecipeRepository>();
        services.AddScoped<IRecipeStepRepository, RecipeStepRepository>();
        services.AddScoped<IRecipeExistenceRepository, RecipeExistenceRepository>();
        services.AddScoped<IRecipeInteractionRepository, RecipeInteractionRepository>();
        services.AddScoped<IImageRepository, ImageRepository>();
        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
        services.AddScoped(typeof(IUserIngredientRelationRepository<>), typeof(UserIngredientRelationRepository<>));
    }

    private static void AddOptionsAdapters(this IServiceCollection services)
    {
        services.AddSingleton<IJwtOptions>(sp =>
            sp.GetRequiredService<IOptions<JwtOptions>>().Value);
        services.AddSingleton<IObjectStorageOptions>(sp =>
            sp.GetRequiredService<IOptions<ObjectStorageOptions>>().Value);
    }
}

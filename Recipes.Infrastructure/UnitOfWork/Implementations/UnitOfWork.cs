using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Infrastructure.UnitOfWork.Implementations;

public class UnitOfWork(
    BaseDbContext dbContext,
    IRefreshTokenRepository refreshTokens,
    IUserRepository users,
    IRecipeRepository recipes,
    IImageRepository images,
    IRecipeImageRepository recipeImages,
    IRecipeIngredientRepository recipeIngredients,
    IIngredientRepository ingredients,
    ICommentRepository comments) : IUnitOfWork
{
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;
    public IUserRepository Users { get; } = users;
    public IRecipeRepository Recipes { get; } = recipes;
    public IImageRepository Images { get; } = images;
    public IRecipeImageRepository RecipeImages { get; } = recipeImages;
    public IRecipeIngredientRepository RecipeIngredients { get; } = recipeIngredients;
    public IIngredientRepository Ingredients { get; } = ingredients;
    public ICommentRepository Comments { get; } = comments;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

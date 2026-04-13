using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Infrastructure.UnitOfWork.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly BaseDbContext _dbContext;

    public UnitOfWork(
        BaseDbContext dbContext,
        IRefreshTokenRepository refreshTokens,
        IUserRepository users,
        IRecipeRepository recipes,
        IImageRepository images,
        IRecipeImageRepository recipeImages,
        IRecipeIngredientRepository recipeIngredients,
        IIngredientRepository ingredients,
        IUnwantedIngredientsRepository unwantedIngredients)
    {
        _dbContext = dbContext;
        RefreshTokens = refreshTokens;
        Users = users;
        Recipes = recipes;
        Images = images;
        RecipeImages = recipeImages;
        RecipeIngredients = recipeIngredients;
        Ingredients = ingredients;
        UnwantedIngredients = unwantedIngredients;
    }

    public IRefreshTokenRepository RefreshTokens { get; }
    public IUserRepository Users { get; }
    public IRecipeRepository Recipes { get; }
    public IImageRepository Images { get; }
    public IRecipeImageRepository RecipeImages { get; }
    public IRecipeIngredientRepository RecipeIngredients { get; }
    public IIngredientRepository Ingredients { get; }
    public IUnwantedIngredientsRepository UnwantedIngredients { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
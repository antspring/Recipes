using Recipes.Application.Repositories.Interfaces;

namespace Recipes.Application.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    public IRefreshTokenRepository RefreshTokens { get; }
    public IUserRepository Users { get; }
    public IRecipeRepository Recipes { get; }
    public IImageRepository Images { get; }
    public IRecipeImageRepository RecipeImages { get; }
    public IRecipeIngredientRepository RecipeIngredients { get; }
    public IIngredientRepository Ingredients { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
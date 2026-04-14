using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.UserRelations;

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

    IUserIngredientRelationRepository<T> GetUserIngredientRelationRepository<T>()
        where T : class, IUserIngredientRelation;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
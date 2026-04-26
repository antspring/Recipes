namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeExistenceRepository
{
    Task<bool> ExistsAsync(Guid recipeId);
}

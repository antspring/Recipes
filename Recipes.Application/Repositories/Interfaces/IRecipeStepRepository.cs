using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeStepRepository
{
    Task<RecipeStep?> GetByIdAsync(Guid id);
    Task<List<RecipeStep>> GetByRecipeIdAsync(Guid recipeId);
    Task AddAsync(RecipeStep step);
    Task UpdateAsync(RecipeStep step);
    Task DeleteAsync(RecipeStep step);
}

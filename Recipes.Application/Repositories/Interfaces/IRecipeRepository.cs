using Recipes.Application.Common;
using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id, RecipeIncludes includes);
    Task<List<Recipe>> GetAllAsync(RecipeIncludes includes);
    Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId, RecipeIncludes includes);
    Task AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(Recipe recipe);
}

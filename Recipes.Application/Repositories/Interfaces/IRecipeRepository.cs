using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id);
    Task<List<Recipe>> GetAllAsync();
    Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId);
    Task AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(Recipe recipe);
    Task<bool> ExistsAsync(Guid id);
    Task<Like?> GetLikeAsync(Guid recipeId, Guid userId);
    Task ToggleLikeAsync(Recipe recipe, Guid userId, bool isLiked);
}

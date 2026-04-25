using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

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
    Task AddLikeAsync(Like like);
    Task RemoveLikeAsync(Like like);
    Task<Favorite?> GetFavoriteAsync(Guid recipeId, Guid userId);
    Task AddFavoriteAsync(Favorite favorite);
    Task RemoveFavoriteAsync(Favorite favorite);
}
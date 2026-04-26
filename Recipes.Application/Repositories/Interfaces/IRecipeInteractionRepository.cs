using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeInteractionRepository
{
    Task<Like?> GetLikeAsync(Guid recipeId, Guid userId);
    Task AddLikeAsync(Like like);
    Task RemoveLikeAsync(Like like);
    Task<Favorite?> GetFavoriteAsync(Guid recipeId, Guid userId);
    Task AddFavoriteAsync(Favorite favorite);
    Task RemoveFavoriteAsync(Favorite favorite);
}

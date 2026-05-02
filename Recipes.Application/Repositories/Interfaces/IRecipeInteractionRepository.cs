using Recipes.Application.Common;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeInteractionRepository
{
    Task<Like?> GetLikeAsync(Guid recipeId, Guid userId);
    Task<List<Recipe>> GetLikedRecipesByUserIdAsync(Guid userId, RecipeIncludes includes);
    Task<Dictionary<Guid, int>> GetLikeCountsByRecipeIdsAsync(IReadOnlyCollection<Guid> recipeIds);
    Task AddLikeAsync(Like like);
    Task RemoveLikeAsync(Like like);
    Task<RecipeRating?> GetRatingAsync(Guid recipeId, Guid userId);
    Task<Dictionary<Guid, (double AverageRating, int RatingsCount)>> GetRatingStatsByRecipeIdsAsync(
        IReadOnlyCollection<Guid> recipeIds);
    Task AddRatingAsync(RecipeRating rating);
    Task RemoveRatingAsync(RecipeRating rating);
    Task<Favorite?> GetFavoriteAsync(Guid recipeId, Guid userId);
    Task<List<Recipe>> GetFavoriteRecipesByUserIdAsync(Guid userId, RecipeIncludes includes);
    Task<Dictionary<Guid, int>> GetFavoriteCountsByRecipeIdsAsync(IReadOnlyCollection<Guid> recipeIds);
    Task AddFavoriteAsync(Favorite favorite);
    Task RemoveFavoriteAsync(Favorite favorite);
}

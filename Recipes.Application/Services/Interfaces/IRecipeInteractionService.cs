namespace Recipes.Application.Services.Interfaces;

public interface IRecipeInteractionService
{
    Task ToggleLikeAsync(Guid recipeId, Guid userId, bool isLiked);
    Task ToggleFavoriteAsync(Guid recipeId, Guid userId, bool isFavorite);
    Task SetRatingAsync(Guid recipeId, Guid userId, int value);
    Task DeleteRatingAsync(Guid recipeId, Guid userId);
}
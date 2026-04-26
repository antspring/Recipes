using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeInteractionService(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork,
    IClock clock) : IRecipeInteractionService
{
    public async Task ToggleLikeAsync(Guid recipeId, Guid userId, bool isLiked)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        var existingLike = await recipeRepository.GetLikeAsync(recipeId, userId);

        if (isLiked && existingLike == null)
        {
            await recipeRepository.AddLikeAsync(new Like
            {
                RecipeId = recipe.Id,
                UserId = userId,
                CreatedAt = clock.UtcNow
            });
        }

        if (!isLiked && existingLike != null)
        {
            await recipeRepository.RemoveLikeAsync(existingLike);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleFavoriteAsync(Guid recipeId, Guid userId, bool isFavorite)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        var existingFavorite = await recipeRepository.GetFavoriteAsync(recipeId, userId);

        if (isFavorite && existingFavorite == null)
        {
            await recipeRepository.AddFavoriteAsync(new Favorite
            {
                RecipeId = recipe.Id,
                UserId = userId,
                CreatedAt = clock.UtcNow
            });
        }

        if (!isFavorite && existingFavorite != null)
        {
            await recipeRepository.RemoveFavoriteAsync(existingFavorite);
        }

        await unitOfWork.SaveChangesAsync();
    }
}

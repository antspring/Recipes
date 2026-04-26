using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeInteractionService(
    IRecipeInteractionRepository recipeInteractionRepository,
    IRecipeExistenceRepository recipeExistenceRepository,
    IUnitOfWork unitOfWork,
    IClock clock) : IRecipeInteractionService
{
    public async Task ToggleLikeAsync(Guid recipeId, Guid userId, bool isLiked)
    {
        await EnsureRecipeExistsAsync(recipeId);

        var existingLike = await recipeInteractionRepository.GetLikeAsync(recipeId, userId);

        if (isLiked && existingLike == null)
        {
            await recipeInteractionRepository.AddLikeAsync(new Like
            {
                RecipeId = recipeId,
                UserId = userId,
                CreatedAt = clock.UtcNow
            });
        }

        if (!isLiked && existingLike != null)
        {
            await recipeInteractionRepository.RemoveLikeAsync(existingLike);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleFavoriteAsync(Guid recipeId, Guid userId, bool isFavorite)
    {
        await EnsureRecipeExistsAsync(recipeId);

        var existingFavorite = await recipeInteractionRepository.GetFavoriteAsync(recipeId, userId);

        if (isFavorite && existingFavorite == null)
        {
            await recipeInteractionRepository.AddFavoriteAsync(new Favorite
            {
                RecipeId = recipeId,
                UserId = userId,
                CreatedAt = clock.UtcNow
            });
        }

        if (!isFavorite && existingFavorite != null)
        {
            await recipeInteractionRepository.RemoveFavoriteAsync(existingFavorite);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureRecipeExistsAsync(Guid recipeId)
    {
        if (!await recipeExistenceRepository.ExistsAsync(recipeId))
            throw new ArgumentException("Recipe not found");
    }
}

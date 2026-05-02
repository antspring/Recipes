using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeInteractionService(
    IRecipeInteractionRepository recipeInteractionRepository,
    IRecipeExistenceRepository recipeExistenceRepository,
    IRecipeDtoFactory recipeDtoFactory,
    IUnitOfWork unitOfWork,
    IClock clock) : IRecipeInteractionService
{
    public async Task<List<RecipeDto>> GetLikedRecipesAsync(Guid userId)
    {
        var recipes = await recipeInteractionRepository.GetLikedRecipesByUserIdAsync(userId, RecipeIncludes.Full);
        return await recipeDtoFactory.CreateManyAsync(recipes);
    }

    public async Task<List<RecipeDto>> GetFavoriteRecipesAsync(Guid userId)
    {
        var recipes = await recipeInteractionRepository.GetFavoriteRecipesByUserIdAsync(userId, RecipeIncludes.Full);
        return await recipeDtoFactory.CreateManyAsync(recipes);
    }

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

    public async Task SetRatingAsync(Guid recipeId, Guid userId, int value)
    {
        EnsureValidRating(value);
        await EnsureRecipeExistsAsync(recipeId);

        var existingRating = await recipeInteractionRepository.GetRatingAsync(recipeId, userId);
        var now = clock.UtcNow;

        if (existingRating == null)
        {
            await recipeInteractionRepository.AddRatingAsync(new RecipeRating
            {
                RecipeId = recipeId,
                UserId = userId,
                Value = value,
                CreatedAt = now,
                UpdatedAt = now
            });
        }
        else
        {
            existingRating.Value = value;
            existingRating.UpdatedAt = now;
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteRatingAsync(Guid recipeId, Guid userId)
    {
        await EnsureRecipeExistsAsync(recipeId);

        var existingRating = await recipeInteractionRepository.GetRatingAsync(recipeId, userId);
        if (existingRating != null)
        {
            await recipeInteractionRepository.RemoveRatingAsync(existingRating);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private async Task EnsureRecipeExistsAsync(Guid recipeId)
    {
        if (!await recipeExistenceRepository.ExistsAsync(recipeId))
            throw new ArgumentException("Recipe not found");
    }

    private static void EnsureValidRating(int value)
    {
        if (value is < 1 or > 5)
            throw new InvalidOperationException("Rating value must be between 1 and 5");
    }
}

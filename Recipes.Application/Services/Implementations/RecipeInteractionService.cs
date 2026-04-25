using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class RecipeInteractionService(
    IUnitOfWork unitOfWork) : IRecipeInteractionService
{
    public async Task ToggleLikeAsync(Guid recipeId, Guid userId, bool isLiked)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        var existingLike = await unitOfWork.Recipes.GetLikeAsync(recipeId, userId);

        if (isLiked && existingLike == null)
        {
            await unitOfWork.Recipes.AddLikeAsync(new Like
            {
                RecipeId = recipe.Id,
                UserId = userId,
                CreatedAt = DateTime.Now.ToUniversalTime()
            });
        }

        if (!isLiked && existingLike != null)
        {
            await unitOfWork.Recipes.RemoveLikeAsync(existingLike);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleFavoriteAsync(Guid recipeId, Guid userId, bool isFavorite)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        var existingFavorite = await unitOfWork.Recipes.GetFavoriteAsync(recipeId, userId);

        if (isFavorite && existingFavorite == null)
        {
            await unitOfWork.Recipes.AddFavoriteAsync(new Favorite
            {
                RecipeId = recipe.Id,
                UserId = userId,
                CreatedAt = DateTime.Now.ToUniversalTime()
            });
        }

        if (!isFavorite && existingFavorite != null)
        {
            await unitOfWork.Recipes.RemoveFavoriteAsync(existingFavorite);
        }

        await unitOfWork.SaveChangesAsync();
    }
}
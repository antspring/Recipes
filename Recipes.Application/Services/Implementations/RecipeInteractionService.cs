using Microsoft.Extensions.Logging;
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

        await unitOfWork.Recipes.ToggleLikeAsync(recipe, userId, isLiked);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleFavoriteAsync(Guid recipeId, Guid userId, bool isFavorite)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        await unitOfWork.Recipes.ToggleFavoriteAsync(recipe, userId, isFavorite);
        await unitOfWork.SaveChangesAsync();
    }
}
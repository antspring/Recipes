using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeImageService
{
    Task<List<RecipeImage>> SaveImagesAsync(List<ImageUpload> imageUploads, Guid recipeId);
}
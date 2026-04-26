using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeImageService
{
    Task<List<RecipeImage>> SaveImagesAsync(
        IReadOnlyCollection<ImageUpload> imageUploads,
        Guid recipeId,
        int startOrder = 0);

    Task DeleteImagesAsync(IReadOnlyCollection<Guid> imageIds, Domain.Models.Recipe recipe);
    Task DeleteImagesAsync(IReadOnlyCollection<RecipeImage> recipeImages, Domain.Models.Recipe recipe);
}

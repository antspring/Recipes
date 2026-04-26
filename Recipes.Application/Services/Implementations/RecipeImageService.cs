using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeImageService(
    IImageRepository imageRepository,
    IImageStorageService imageStorageService,
    IClock clock) : IRecipeImageService
{
    public async Task<List<RecipeImage>> SaveImagesAsync(
        List<ImageUpload> imageUploads,
        Guid recipeId,
        int startOrder = 0)
    {
        var recipeImages = new List<RecipeImage>();
        var order = startOrder;
        foreach (var imageUpload in imageUploads)
        {
            var fileName = await imageStorageService.UploadImageAsync(
                imageUpload.Stream,
                imageUpload.FileName,
                imageUpload.ContentType);

            var image = new Image
            {
                FileName = fileName,
                CreatedAt = clock.UtcNow,
            };
            await imageRepository.AddAsync(image);

            recipeImages.Add(new RecipeImage
            {
                RecipeId = recipeId,
                ImageId = image.Id,
                Order = order++
            });
        }

        return recipeImages;
    }

    public async Task DeleteImagesAsync(List<Guid> imageIds, Recipe recipe)
    {
        var recipeImagesToDelete = recipe.RecipeImages
            .Where(ri => imageIds.Contains(ri.ImageId))
            .ToList();

        await DeleteRecipeImagesAsync(recipeImagesToDelete, recipe);
    }

    public async Task DeleteImagesAsync(List<RecipeImage> recipeImages, Recipe recipe)
    {
        if (recipeImages.Count == 0)
            return;

        await DeleteRecipeImagesAsync(recipeImages, recipe);
    }

    private async Task DeleteRecipeImagesAsync(List<RecipeImage> recipeImagesToDelete, Recipe recipe)
    {
        if (recipeImagesToDelete.Count == 0)
            return;

        var fileNames = recipeImagesToDelete.Select(ri => ri.Image.FileName);
        await imageStorageService.DeleteImagesAsync(fileNames);

        foreach (var recipeImage in recipeImagesToDelete)
        {
            await imageRepository.DeleteAsync(recipeImage.Image);
            recipe.RecipeImages.Remove(recipeImage);
        }
    }
}

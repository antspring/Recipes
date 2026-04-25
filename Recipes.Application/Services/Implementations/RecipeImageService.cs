using Microsoft.Extensions.Logging;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeImageService(
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService) : IRecipeImageService
{
    public async Task<List<RecipeImage>> SaveImagesAsync(List<ImageUpload> imageUploads, Guid recipeId)
    {
        var recipeImages = new List<RecipeImage>();
        var order = 0;
        foreach (var imageUpload in imageUploads)
        {
            var fileName = await imageStorageService.UploadImageAsync(
                imageUpload.Stream,
                imageUpload.FileName,
                imageUpload.ContentType);

            var image = new Image
            {
                FileName = fileName,
                CreatedAt = DateTime.UtcNow,
            };
            await unitOfWork.Images.AddAsync(image);

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
            recipe.RecipeImages.Remove(recipeImage);
        }
    }
}

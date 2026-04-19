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
                CreatedAt = DateTime.Now.ToUniversalTime(),
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
}
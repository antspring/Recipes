using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.Extensions.Logging;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeService(
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService,
    ILogger<RecipeService> logger,
    IMapper mapper) : IRecipeService
{
    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto)
    {
        var recipe = mapper.Map<Recipe>(createRecipeDto);

        recipe.RecipeIngredients = await SaveRecipeIngredientsAsync(createRecipeDto, recipe.Id);
        recipe.RecipeImages = await SaveImagesAsync(createRecipeDto, recipe.Id);

        await unitOfWork.Recipes.AddAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        var createdRecipe = await unitOfWork.Recipes.GetByIdAsync(recipe.Id);
        var dto = RecipeDto.FromRecipe(createdRecipe!);
        dto.ApplyImageUrls(imageStorageService);
        return dto;
    }

    public async Task<RecipeDto?> GetRecipeByIdAsync(Guid id)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(id);
        if (recipe == null) return null;

        var dto = RecipeDto.FromRecipe(recipe);
        dto.ApplyImageUrls(imageStorageService);
        return dto;
    }

    public async Task<List<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await unitOfWork.Recipes.GetAllAsync();
        var dtos = recipes.Select(RecipeDto.FromRecipe).ToList();
        return dtos.Select(dto =>
        {
            dto.ApplyImageUrls(imageStorageService);
            return dto;
        }).ToList();
    }

    public async Task<List<RecipeDto>> GetRecipesByCreatorIdAsync(Guid creatorId)
    {
        var recipes = await unitOfWork.Recipes.GetByCreatorIdAsync(creatorId);
        var dtos = recipes.Select(RecipeDto.FromRecipe).ToList();
        return dtos.Select(dto =>
        {
            dto.ApplyImageUrls(imageStorageService);
            return dto;
        }).ToList();
    }

    public async Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto updateRecipeDto)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(updateRecipeDto.Id);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        mapper.Map(updateRecipeDto, recipe);

        if (updateRecipeDto.Ingredients != null)
        {
            var recipeIngredients = new List<RecipeIngredient>();
            foreach (var ingredientDto in updateRecipeDto.Ingredients)
            {
                var ingredient = await unitOfWork.Ingredients.GetByIdAsync(ingredientDto.IngredientId);
                if (ingredient == null)
                    throw new ArgumentException($"Ingredient with id {ingredientDto.IngredientId} not found");

                var recipeIngredient = mapper.Map<RecipeIngredient>(ingredientDto, opt => opt.Items.Add("RecipeId", recipe.Id));
                recipeIngredients.Add(recipeIngredient);
            }
            recipe.RecipeIngredients = recipeIngredients;
        }

        if (updateRecipeDto.ImageUploads is { Count: > 0 })
        {
            var recipeImages = new List<RecipeImage>();
            var order = 0;
            foreach (var imageUpload in updateRecipeDto.ImageUploads)
            {
                var fileName = await imageStorageService.UploadImageAsync(
                    imageUpload.Stream,
                    imageUpload.FileName,
                    imageUpload.ContentType);

                var image = new Image
                {
                    Id = Guid.NewGuid(),
                    FileName = fileName,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                };
                await unitOfWork.Images.AddAsync(image);

                recipeImages.Add(new RecipeImage
                {
                    RecipeId = recipe.Id,
                    ImageId = image.Id,
                    Order = order++
                });
            }
            recipe.RecipeImages = recipeImages;
        }

        await unitOfWork.Recipes.UpdateAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        var updatedRecipe = await unitOfWork.Recipes.GetByIdAsync(recipe.Id);
        var dto = RecipeDto.FromRecipe(updatedRecipe!);
        dto.ApplyImageUrls(imageStorageService);
        return dto;
    }

    public async Task DeleteRecipeAsync(Guid id)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(id);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        if (recipe.RecipeImages != null)
        {
            foreach (var recipeImage in recipe.RecipeImages)
            {
                try
                {
                    await imageStorageService.DeleteImageAsync(recipeImage.Image.FileName);
                    await unitOfWork.Images.DeleteAsync(recipeImage.Image);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete image from Object Storage: {FileName}",
                        recipeImage.Image.FileName);
                }
            }
        }

        await unitOfWork.Recipes.DeleteAsync(recipe);
        await unitOfWork.SaveChangesAsync();
    }

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

    private async Task<List<RecipeImage>> SaveImagesAsync(CreateRecipeDto createRecipeDto, Guid recipeId)
    {
        var recipeImages = new List<RecipeImage>();
        var order = 0;
        foreach (var imageUpload in createRecipeDto.ImageUploads)
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

    private async Task<List<RecipeIngredient>> SaveRecipeIngredientsAsync(CreateRecipeDto createRecipeDto, Guid recipeId)
    {
        var recipeIngredients = new List<RecipeIngredient>();
        foreach (var ingredientDto in createRecipeDto.Ingredients)
        {
            var ingredient = await unitOfWork.Ingredients.GetByIdAsync(ingredientDto.IngredientId);
            if (ingredient == null)
                throw new ArgumentException($"Ingredient with id {ingredientDto.IngredientId} not found");

            var recipeIngredient = mapper.Map<RecipeIngredient>(ingredientDto, opt => opt.Items.Add("RecipeId", recipeId));
            recipeIngredients.Add(recipeIngredient);
        }

        return recipeIngredients;
    }
}
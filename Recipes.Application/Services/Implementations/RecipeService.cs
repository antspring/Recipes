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
    ILogger<RecipeService> logger) : IRecipeService
{
    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto)
    {
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = createRecipeDto.Title,
            Description = createRecipeDto.Description,
            CaloricValue = createRecipeDto.CaloricValue,
            Proteins = createRecipeDto.Proteins,
            Fats = createRecipeDto.Fats,
            Carbohydrates = createRecipeDto.Carbohydrates,
            CreatorId = createRecipeDto.CreatorId,
            CreatedAt = DateTime.Now.ToUniversalTime(),
            UpdatedAt = DateTime.Now.ToUniversalTime(),
            CookingTime = createRecipeDto.CookingTime,
            DishType = createRecipeDto.DishType,
            MealType = createRecipeDto.MealType
        };

        var recipeIngredients = new List<RecipeIngredient>();
        foreach (var ingredientDto in createRecipeDto.Ingredients)
        {
            var ingredient = await unitOfWork.Ingredients.GetByIdAsync(ingredientDto.IngredientId);
            if (ingredient == null)
                throw new ArgumentException($"Ingredient with id {ingredientDto.IngredientId} not found");

            recipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipe.Id,
                IngredientId = ingredientDto.IngredientId,
                Weight = ingredientDto.Weight,
                AlternativeWeight = ingredientDto.AlternativeWeight
            });
        }

        recipe.RecipeIngredients = recipeIngredients;

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

        recipe.Title = updateRecipeDto.Title;
        recipe.Description = updateRecipeDto.Description;
        recipe.CaloricValue = updateRecipeDto.CaloricValue;
        recipe.Proteins = updateRecipeDto.Proteins;
        recipe.Fats = updateRecipeDto.Fats;
        recipe.Carbohydrates = updateRecipeDto.Carbohydrates;
        recipe.CookingTime = updateRecipeDto.CookingTime;
        recipe.DishType = updateRecipeDto.DishType;
        recipe.MealType = updateRecipeDto.MealType;

        if (updateRecipeDto.Ingredients != null)
        {
            var recipeIngredients = new List<RecipeIngredient>();
            foreach (var ingredientDto in updateRecipeDto.Ingredients)
            {
                var ingredient = await unitOfWork.Ingredients.GetByIdAsync(ingredientDto.IngredientId);
                if (ingredient == null)
                    throw new ArgumentException($"Ingredient with id {ingredientDto.IngredientId} not found");

                recipeIngredients.Add(new RecipeIngredient
                {
                    RecipeId = recipe.Id,
                    IngredientId = ingredientDto.IngredientId,
                    Weight = ingredientDto.Weight,
                    AlternativeWeight = ingredientDto.AlternativeWeight
                });
            }
            recipe.RecipeIngredients = recipeIngredients;
        }

        if (updateRecipeDto.ImageUploads != null && updateRecipeDto.ImageUploads.Count > 0)
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
}
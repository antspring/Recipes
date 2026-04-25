using AutoMapper;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Implementations;

public class RecipeCrudService(
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService,
    IRecipeImageService imageService,
    IRecipeIngredientService ingredientService,
    IMapper mapper) : IRecipeCrudService
{
    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto)
    {
        var recipe = mapper.Map<Recipe>(createRecipeDto);

        recipe.RecipeIngredients =
            await ingredientService.SaveRecipeIngredientsAsync(createRecipeDto.Ingredients, recipe.Id);
        recipe.RecipeImages = await imageService.SaveImagesAsync(createRecipeDto.ImageUploads, recipe.Id);

        await unitOfWork.Recipes.AddAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredRecipeDtoAsync(recipe.Id);
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

        if (recipe.CreatorId != updateRecipeDto.ActorUserId)
            throw new UnauthorizedAccessException("Only the author can update this recipe");

        mapper.Map(updateRecipeDto, recipe);

        if (updateRecipeDto.Ingredients != null)
        {
            recipe.RecipeIngredients =
                await ingredientService.SaveRecipeIngredientsAsync(updateRecipeDto.Ingredients, recipe.Id);
        }

        if (updateRecipeDto.ImageUploads is { Count: > 0 })
        {
            var newRecipeImages = await imageService.SaveImagesAsync(updateRecipeDto.ImageUploads, recipe.Id);
            recipe.RecipeImages ??= new List<RecipeImage>();
            recipe.RecipeImages.AddRange(newRecipeImages);
        }

        if (updateRecipeDto.ImageIdsToDelete != null)
        {
            var existingImageIds = recipe.Images.Select(i => i.Id).ToHashSet();
            var invalidImageIds = updateRecipeDto.ImageIdsToDelete
                .Where(id => !existingImageIds.Contains(id))
                .ToList();

            if (invalidImageIds.Count > 0)
                throw new ArgumentException($"Images not found: {string.Join(", ", invalidImageIds)}");

            await imageService.DeleteImagesAsync(updateRecipeDto.ImageIdsToDelete, recipe);
        }

        recipe.UpdatedAt = DateTime.UtcNow;
        await unitOfWork.Recipes.UpdateAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredRecipeDtoAsync(recipe.Id);
    }

    public async Task DeleteRecipeAsync(Guid id, Guid actorUserId)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(id);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        if (recipe.CreatorId != actorUserId)
            throw new UnauthorizedAccessException("Only the author can delete this recipe");

        if (recipe.RecipeImages != null && recipe.RecipeImages.Count > 0)
        {
            await imageService.DeleteImagesAsync(recipe.RecipeImages.ToList(), recipe);
        }

        await unitOfWork.Recipes.DeleteAsync(recipe);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<RecipeDto> GetRequiredRecipeDtoAsync(Guid recipeId)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new InvalidOperationException("Recipe not found after save");

        var dto = RecipeDto.FromRecipe(recipe);
        dto.ApplyImageUrls(imageStorageService);
        return dto;
    }
}

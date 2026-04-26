using AutoMapper;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class RecipeCrudService(
    IRecipeRepository recipeRepository,
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService,
    IRecipeImageService imageService,
    IRecipeIngredientService ingredientService,
    IMapper mapper,
    IClock clock) : IRecipeCrudService
{
    public async Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto)
    {
        var recipe = mapper.Map<Recipe>(createRecipeDto);
        var now = clock.UtcNow;

        recipe.CreatedAt = now;
        recipe.UpdatedAt = now;

        recipe.RecipeIngredients =
            await ingredientService.SaveRecipeIngredientsAsync(createRecipeDto.Ingredients, recipe.Id);
        recipe.RecipeImages = await imageService.SaveImagesAsync(createRecipeDto.ImageUploads, recipe.Id);

        await recipeRepository.AddAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredRecipeDtoAsync(recipe.Id);
    }

    public async Task<RecipeDto?> GetRecipeByIdAsync(Guid id)
    {
        var recipe = await recipeRepository.GetByIdAsync(id);
        return recipe == null ? null : ToRecipeDto(recipe);
    }

    public async Task<List<RecipeDto>> GetAllRecipesAsync()
    {
        var recipes = await recipeRepository.GetAllAsync();
        return ToRecipeDtos(recipes);
    }

    public async Task<List<RecipeDto>> GetRecipesByCreatorIdAsync(Guid creatorId)
    {
        var recipes = await recipeRepository.GetByCreatorIdAsync(creatorId);
        return ToRecipeDtos(recipes);
    }

    public async Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto updateRecipeDto)
    {
        var recipe = await GetRequiredRecipeAsync(updateRecipeDto.Id);
        EnsureRecipeAuthor(recipe, updateRecipeDto.ActorUserId, "update");

        mapper.Map(updateRecipeDto, recipe);

        await UpdateIngredientsAsync(recipe, updateRecipeDto.Ingredients);
        await AddImagesAsync(recipe, updateRecipeDto.ImageUploads);
        await DeleteImagesAsync(recipe, updateRecipeDto.ImageIdsToDelete);

        recipe.UpdatedAt = clock.UtcNow;
        await recipeRepository.UpdateAsync(recipe);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredRecipeDtoAsync(recipe.Id);
    }

    public async Task DeleteRecipeAsync(Guid id, Guid actorUserId)
    {
        var recipe = await GetRequiredRecipeAsync(id);
        EnsureRecipeAuthor(recipe, actorUserId, "delete");

        if (recipe.RecipeImages.Count > 0)
        {
            await imageService.DeleteImagesAsync(recipe.RecipeImages.ToList(), recipe);
        }

        await recipeRepository.DeleteAsync(recipe);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<Recipe> GetRequiredRecipeAsync(Guid recipeId)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        return recipe;
    }

    private static void EnsureRecipeAuthor(Recipe recipe, Guid actorUserId, string action)
    {
        if (recipe.CreatorId != actorUserId)
            throw new UnauthorizedAccessException($"Only the author can {action} this recipe");
    }

    private async Task UpdateIngredientsAsync(Recipe recipe, List<RecipeIngredientInputDto>? ingredients)
    {
        if (ingredients == null)
            return;

        recipe.RecipeIngredients = await ingredientService.SaveRecipeIngredientsAsync(ingredients, recipe.Id);
    }

    private async Task AddImagesAsync(Recipe recipe, List<ImageUpload> imageUploads)
    {
        if (imageUploads.Count == 0)
            return;

        var startOrder = recipe.RecipeImages.Count == 0
            ? 0
            : recipe.RecipeImages.Max(ri => ri.Order) + 1;

        var newRecipeImages = await imageService.SaveImagesAsync(imageUploads, recipe.Id, startOrder);
        recipe.RecipeImages.AddRange(newRecipeImages);
    }

    private async Task DeleteImagesAsync(Recipe recipe, List<Guid>? imageIdsToDelete)
    {
        if (imageIdsToDelete == null)
            return;

        var existingImageIds = recipe.RecipeImages.Select(ri => ri.ImageId).ToHashSet();
        var invalidImageIds = imageIdsToDelete
            .Where(id => !existingImageIds.Contains(id))
            .ToList();

        if (invalidImageIds.Count > 0)
            throw new ArgumentException($"Images not found: {string.Join(", ", invalidImageIds)}");

        await imageService.DeleteImagesAsync(imageIdsToDelete, recipe);
    }

    private async Task<RecipeDto> GetRequiredRecipeDtoAsync(Guid recipeId)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new InvalidOperationException("Recipe not found after save");

        return ToRecipeDto(recipe);
    }

    private RecipeDto ToRecipeDto(Recipe recipe)
    {
        var dto = RecipeDto.FromRecipe(recipe);
        dto.ApplyImageUrls(imageStorageService);
        return dto;
    }

    private List<RecipeDto> ToRecipeDtos(IEnumerable<Recipe> recipes)
    {
        return recipes.Select(ToRecipeDto).ToList();
    }
}

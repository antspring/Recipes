using AutoMapper;
using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class RecipeCrudService(
    IRecipeRepository recipeRepository,
    ICommentRepository commentRepository,
    IRecipeInteractionRepository recipeInteractionRepository,
    IUnitOfWork unitOfWork,
    IImageUrlProvider imageUrlProvider,
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

    public async Task<RecipeDto?> GetRecipeByIdAsync(Guid id, RecipeIncludes? includes = null)
    {
        var recipe = await recipeRepository.GetByIdAsync(id, GetReadIncludes(includes));
        return recipe == null ? null : await ToRecipeDtoAsync(recipe);
    }

    public async Task<List<RecipeDto>> GetAllRecipesAsync(RecipeIncludes? includes = null)
    {
        var recipes = await recipeRepository.GetAllAsync(GetReadIncludes(includes));
        return await ToRecipeDtosAsync(recipes);
    }

    public async Task<List<RecipeDto>> GetRecipesByCreatorIdAsync(Guid creatorId, RecipeIncludes? includes = null)
    {
        var recipes = await recipeRepository.GetByCreatorIdAsync(creatorId, GetReadIncludes(includes));
        return await ToRecipeDtosAsync(recipes);
    }

    public async Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto updateRecipeDto)
    {
        var recipe = await GetRequiredRecipeAsync(updateRecipeDto.Id, RecipeIncludes.Ingredients | RecipeIncludes.Images);
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
        var recipe = await GetRequiredRecipeAsync(id, RecipeIncludes.Images);
        EnsureRecipeAuthor(recipe, actorUserId, "delete");

        if (recipe.RecipeImages.Count > 0)
        {
            await imageService.DeleteImagesAsync(recipe.RecipeImages.ToList(), recipe);
        }

        await recipeRepository.DeleteAsync(recipe);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<Recipe> GetRequiredRecipeAsync(Guid recipeId, RecipeIncludes includes)
    {
        var recipe = await recipeRepository.GetByIdAsync(recipeId, includes);
        if (recipe == null)
            throw new ArgumentException("Recipe not found");

        return recipe;
    }

    private static void EnsureRecipeAuthor(Recipe recipe, Guid actorUserId, string action)
    {
        if (recipe.CreatorId != actorUserId)
            throw new UnauthorizedAccessException($"Only the author can {action} this recipe");
    }

    private async Task UpdateIngredientsAsync(Recipe recipe, IReadOnlyCollection<RecipeIngredientInputDto>? ingredients)
    {
        if (ingredients == null)
            return;

        recipe.RecipeIngredients = await ingredientService.SaveRecipeIngredientsAsync(ingredients, recipe.Id);
    }

    private async Task AddImagesAsync(Recipe recipe, IReadOnlyCollection<ImageUpload> imageUploads)
    {
        if (imageUploads.Count == 0)
            return;

        var startOrder = recipe.RecipeImages.Count == 0
            ? 0
            : recipe.RecipeImages.Max(ri => ri.Order) + 1;

        var newRecipeImages = await imageService.SaveImagesAsync(imageUploads, recipe.Id, startOrder);
        recipe.RecipeImages.AddRange(newRecipeImages);
    }

    private async Task DeleteImagesAsync(Recipe recipe, IReadOnlyCollection<Guid>? imageIdsToDelete)
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
        var recipe = await recipeRepository.GetByIdAsync(recipeId, RecipeIncludes.Full);
        if (recipe == null)
            throw new InvalidOperationException("Recipe not found after save");

        return await ToRecipeDtoAsync(recipe);
    }

    private static RecipeIncludes GetReadIncludes(RecipeIncludes? includes)
    {
        return includes ?? RecipeIncludes.Full;
    }

    private async Task<RecipeDto> ToRecipeDtoAsync(Recipe recipe)
    {
        var dto = RecipeDto.FromRecipe(recipe);
        ApplyImageUrls(dto);
        await ApplyCountersAsync([dto]);
        return dto;
    }

    private async Task<List<RecipeDto>> ToRecipeDtosAsync(IEnumerable<Recipe> recipes)
    {
        var dtos = recipes.Select(recipe =>
        {
            var dto = RecipeDto.FromRecipe(recipe);
            ApplyImageUrls(dto);
            return dto;
        }).ToList();

        await ApplyCountersAsync(dtos);
        return dtos;
    }

    private async Task ApplyCountersAsync(IReadOnlyCollection<RecipeDto> recipes)
    {
        if (recipes.Count == 0)
            return;

        var recipeIds = recipes.Select(recipe => recipe.Id).ToList();
        var commentCounts = await commentRepository.GetCountsByRecipeIdsAsync(recipeIds);
        var likeCounts = await recipeInteractionRepository.GetLikeCountsByRecipeIdsAsync(recipeIds);

        foreach (var recipe in recipes)
        {
            recipe.CommentsCount = commentCounts.GetValueOrDefault(recipe.Id);
            recipe.LikesCount = likeCounts.GetValueOrDefault(recipe.Id);
        }
    }

    private void ApplyImageUrls(RecipeDto recipe)
    {
        foreach (var image in recipe.Images)
        {
            image.Url = imageUrlProvider.GetImageUrl(image.FileName);
        }

        foreach (var step in recipe.Steps.Where(step => step.Image != null))
        {
            step.Image!.Url = imageUrlProvider.GetImageUrl(step.Image.FileName);
        }
    }
}

using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class RecipeDtoFactory(
    ICommentRepository commentRepository,
    IRecipeInteractionRepository recipeInteractionRepository,
    IImageUrlMapper imageUrlMapper) : IRecipeDtoFactory
{
    public async Task<RecipeDto> CreateAsync(Recipe recipe, Guid? currentUserId = null)
    {
        var dto = ToRecipeDto(recipe);
        await ApplyCountersAsync([dto], currentUserId);
        return dto;
    }

    public async Task<List<RecipeDto>> CreateManyAsync(IEnumerable<Recipe> recipes, Guid? currentUserId = null)
    {
        var dtos = recipes.Select(ToRecipeDto).ToList();

        await ApplyCountersAsync(dtos, currentUserId);
        return dtos;
    }

    private async Task ApplyCountersAsync(IReadOnlyCollection<RecipeDto> recipes, Guid? currentUserId)
    {
        if (recipes.Count == 0)
            return;

        var recipeIds = recipes.Select(recipe => recipe.Id).ToList();
        var commentCounts = await commentRepository.GetCountsByRecipeIdsAsync(recipeIds);
        var likeCounts = await recipeInteractionRepository.GetLikeCountsByRecipeIdsAsync(recipeIds);
        var favoriteCounts = await recipeInteractionRepository.GetFavoriteCountsByRecipeIdsAsync(recipeIds);
        var ratingStats = await recipeInteractionRepository.GetRatingStatsByRecipeIdsAsync(recipeIds);
        var currentUserRatings = currentUserId.HasValue
            ? await recipeInteractionRepository.GetRatingsByRecipeIdsAsync(recipeIds, currentUserId.Value)
            : new Dictionary<Guid, int>();

        foreach (var recipe in recipes)
        {
            recipe.CommentsCount = commentCounts.GetValueOrDefault(recipe.Id);
            recipe.LikesCount = likeCounts.GetValueOrDefault(recipe.Id);
            recipe.FavoritesCount = favoriteCounts.GetValueOrDefault(recipe.Id);
            var stats = ratingStats.GetValueOrDefault(recipe.Id);
            recipe.AverageRating = stats.AverageRating;
            recipe.RatingsCount = stats.RatingsCount;
            recipe.CurrentUserRating = currentUserRatings.TryGetValue(recipe.Id, out var currentUserRating)
                ? currentUserRating
                : null;
        }
    }

    private RecipeDto ToRecipeDto(Recipe recipe)
    {
        var dto = RecipeDto.FromRecipe(recipe);
        imageUrlMapper.ApplyUrls(dto.Images, image => image.FileName, (image, url) => image.Url = url);

        foreach (var step in dto.Steps)
        {
            imageUrlMapper.ApplyUrl(step.Image, image => image.FileName, (image, url) => image.Url = url);
        }

        return dto;
    }
}

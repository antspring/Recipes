using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class RecipeDtoFactory(
    ICommentRepository commentRepository,
    IRecipeInteractionRepository recipeInteractionRepository,
    IImageUrlProvider imageUrlProvider) : IRecipeDtoFactory
{
    public async Task<RecipeDto> CreateAsync(Recipe recipe)
    {
        var dto = RecipeDto.FromRecipe(recipe);
        ApplyImageUrls(dto);
        await ApplyCountersAsync([dto]);
        return dto;
    }

    public async Task<List<RecipeDto>> CreateManyAsync(IEnumerable<Recipe> recipes)
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
        var favoriteCounts = await recipeInteractionRepository.GetFavoriteCountsByRecipeIdsAsync(recipeIds);
        var ratingStats = await recipeInteractionRepository.GetRatingStatsByRecipeIdsAsync(recipeIds);

        foreach (var recipe in recipes)
        {
            recipe.CommentsCount = commentCounts.GetValueOrDefault(recipe.Id);
            recipe.LikesCount = likeCounts.GetValueOrDefault(recipe.Id);
            recipe.FavoritesCount = favoriteCounts.GetValueOrDefault(recipe.Id);
            var stats = ratingStats.GetValueOrDefault(recipe.Id);
            recipe.AverageRating = stats.AverageRating;
            recipe.RatingsCount = stats.RatingsCount;
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

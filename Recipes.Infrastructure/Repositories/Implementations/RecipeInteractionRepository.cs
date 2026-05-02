using Microsoft.EntityFrameworkCore;
using Recipes.Application.Common;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeInteractionRepository(BaseDbContext context) : IRecipeInteractionRepository
{
    public Task<Like?> GetLikeAsync(Guid recipeId, Guid userId)
    {
        return context.Likes.FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
    }

    public Task<List<Recipe>> GetLikedRecipesByUserIdAsync(Guid userId, RecipeIncludes includes)
    {
        var query = context.Recipes
            .Where(recipe => recipe.Likes.Any(like => like.UserId == userId))
            .OrderByDescending(recipe => recipe.Likes
                .Where(like => like.UserId == userId)
                .Max(like => like.CreatedAt));

        return ApplyIncludes(query, includes).ToListAsync();
    }

    public Task<Dictionary<Guid, int>> GetLikeCountsByRecipeIdsAsync(IReadOnlyCollection<Guid> recipeIds)
    {
        return context.Likes
            .Where(l => recipeIds.Contains(l.RecipeId))
            .GroupBy(l => l.RecipeId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public Task AddLikeAsync(Like like)
    {
        return context.Likes.AddAsync(like).AsTask();
    }

    public Task RemoveLikeAsync(Like like)
    {
        context.Likes.Remove(like);
        return Task.CompletedTask;
    }

    public Task<RecipeRating?> GetRatingAsync(Guid recipeId, Guid userId)
    {
        return context.RecipeRatings.FirstOrDefaultAsync(r => r.RecipeId == recipeId && r.UserId == userId);
    }

    public Task<Dictionary<Guid, (double AverageRating, int RatingsCount)>> GetRatingStatsByRecipeIdsAsync(
        IReadOnlyCollection<Guid> recipeIds)
    {
        return context.RecipeRatings
            .Where(r => recipeIds.Contains(r.RecipeId))
            .GroupBy(r => r.RecipeId)
            .ToDictionaryAsync(
                g => g.Key,
                g => ((double)g.Average(r => r.Value), g.Count()));
    }

    public Task AddRatingAsync(RecipeRating rating)
    {
        return context.RecipeRatings.AddAsync(rating).AsTask();
    }

    public Task RemoveRatingAsync(RecipeRating rating)
    {
        context.RecipeRatings.Remove(rating);
        return Task.CompletedTask;
    }

    public Task<Favorite?> GetFavoriteAsync(Guid recipeId, Guid userId)
    {
        return context.Favorites.FirstOrDefaultAsync(f => f.RecipeId == recipeId && f.UserId == userId);
    }

    public Task<List<Recipe>> GetFavoriteRecipesByUserIdAsync(Guid userId, RecipeIncludes includes)
    {
        var query = context.Recipes
            .Where(recipe => context.Favorites.Any(favorite =>
                favorite.RecipeId == recipe.Id && favorite.UserId == userId))
            .OrderByDescending(recipe => context.Favorites
                .Where(favorite => favorite.RecipeId == recipe.Id && favorite.UserId == userId)
                .Max(favorite => favorite.CreatedAt));

        return ApplyIncludes(query, includes).ToListAsync();
    }

    public Task<Dictionary<Guid, int>> GetFavoriteCountsByRecipeIdsAsync(IReadOnlyCollection<Guid> recipeIds)
    {
        return context.Favorites
            .Where(f => recipeIds.Contains(f.RecipeId))
            .GroupBy(f => f.RecipeId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public Task AddFavoriteAsync(Favorite favorite)
    {
        return context.Favorites.AddAsync(favorite).AsTask();
    }

    public Task RemoveFavoriteAsync(Favorite favorite)
    {
        context.Favorites.Remove(favorite);
        return Task.CompletedTask;
    }

    private static IQueryable<Recipe> ApplyIncludes(IQueryable<Recipe> query, RecipeIncludes includes)
    {
        if (includes.HasFlag(RecipeIncludes.Creator))
            query = query.Include(r => r.Creator);

        if (includes.HasFlag(RecipeIncludes.Ingredients))
            query = query
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient);

        if (includes.HasFlag(RecipeIncludes.Images))
            query = query
                .Include(r => r.RecipeImages)
                .ThenInclude(ri => ri.Image);

        if (includes.HasFlag(RecipeIncludes.Steps))
            query = query
                .Include(r => r.Steps)
                .ThenInclude(rs => rs.Image);

        return query;
    }
}

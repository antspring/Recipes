using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeInteractionRepository(BaseDbContext context) : IRecipeInteractionRepository
{
    public Task<Like?> GetLikeAsync(Guid recipeId, Guid userId)
    {
        return context.Likes.FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
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

    public Task<Favorite?> GetFavoriteAsync(Guid recipeId, Guid userId)
    {
        return context.Favorites.FirstOrDefaultAsync(f => f.RecipeId == recipeId && f.UserId == userId);
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
}

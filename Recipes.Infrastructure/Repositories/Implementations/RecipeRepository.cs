using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeRepository(BaseDbContext context) : IRecipeRepository
{
    public async Task<Recipe?> GetByIdAsync(Guid id)
    {
        return await context.Recipes
            .Include(r => r.Creator)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeImages)
            .ThenInclude(ri => ri.Image)
            .Include(r => r.Likes)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Recipe>> GetAllAsync()
    {
        return await context.Recipes
            .Include(r => r.Creator)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeImages)
            .ThenInclude(ri => ri.Image)
            .Include(r => r.Likes)
            .Include(r => r.Comments)
            .ToListAsync();
    }

    public async Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId)
    {
        return await context.Recipes
            .Where(r => r.CreatorId == creatorId)
            .Include(r => r.Creator)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeImages)
            .ThenInclude(ri => ri.Image)
            .Include(r => r.Likes)
            .Include(r => r.Comments)
            .ToListAsync();
    }

    public async Task AddAsync(Recipe recipe)
    {
        await context.Recipes.AddAsync(recipe);
    }

    public async Task UpdateAsync(Recipe recipe)
    {
        context.Recipes.Update(recipe);
    }

    public async Task DeleteAsync(Recipe recipe)
    {
        context.Recipes.Remove(recipe);
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Recipes.AnyAsync(r => r.Id == id);
    }

    public Task<Like?> GetLikeAsync(Guid recipeId, Guid userId)
    {
        return context.Likes.FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
    }

    public async Task AddLikeAsync(Like like)
    {
        await context.Likes.AddAsync(like);
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

    public async Task AddFavoriteAsync(Favorite favorite)
    {
        await context.Favorites.AddAsync(favorite);
    }

    public Task RemoveFavoriteAsync(Favorite favorite)
    {
        context.Favorites.Remove(favorite);
        return Task.CompletedTask;
    }
}

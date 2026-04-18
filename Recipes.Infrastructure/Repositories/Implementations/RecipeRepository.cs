using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.RecipesRelations;

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
        recipe.UpdatedAt = DateTime.Now.ToUniversalTime();
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

    public async Task<Like?> GetLikeAsync(Guid recipeId, Guid userId)
    {
        return await context.Likes
            .FirstOrDefaultAsync(l => l.RecipeId == recipeId && l.UserId == userId);
    }

    public async Task ToggleLikeAsync(Recipe recipe, Guid userId, bool isLiked)
    {
        var existingLike = recipe.Likes?.FirstOrDefault(l => l.UserId == userId);

        if (isLiked)
        {
            if (existingLike == null)
            {
                var like = new Like
                {
                    RecipeId = recipe.Id,
                    UserId = userId,
                    CreatedAt = DateTime.Now.ToUniversalTime()
                };
                await context.Likes.AddAsync(like);
            }
        }
        else
        {
            if (existingLike != null)
            {
                context.Likes.Remove(existingLike);
            }
        }
    }
}

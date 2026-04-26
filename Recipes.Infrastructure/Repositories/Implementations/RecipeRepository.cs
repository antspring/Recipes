using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

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

    public Task UpdateAsync(Recipe recipe)
    {
        context.Recipes.Update(recipe);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Recipe recipe)
    {
        context.Recipes.Remove(recipe);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await context.Recipes.AnyAsync(r => r.Id == id);
    }
}

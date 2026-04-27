using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeRepository(BaseDbContext context) : IRecipeRepository
{
    public Task<Recipe?> GetByIdAsync(Guid id)
    {
        return context.Recipes
            .Include(r => r.Creator)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeImages)
            .ThenInclude(ri => ri.Image)
            .Include(r => r.Steps)
            .ThenInclude(rs => rs.Image)
            .Include(r => r.Likes)
            .Include(r => r.Comments)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<List<Recipe>> GetAllAsync()
    {
        return context.Recipes
            .Include(r => r.Creator)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeImages)
            .ThenInclude(ri => ri.Image)
            .Include(r => r.Steps)
            .ThenInclude(rs => rs.Image)
            .Include(r => r.Likes)
            .Include(r => r.Comments)
            .ToListAsync();
    }

    public Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId)
    {
        return context.Recipes
            .Where(r => r.CreatorId == creatorId)
            .Include(r => r.Creator)
            .Include(r => r.RecipeIngredients)
            .ThenInclude(ri => ri.Ingredient)
            .Include(r => r.RecipeImages)
            .ThenInclude(ri => ri.Image)
            .Include(r => r.Steps)
            .ThenInclude(rs => rs.Image)
            .Include(r => r.Likes)
            .Include(r => r.Comments)
            .ToListAsync();
    }

    public Task AddAsync(Recipe recipe)
    {
        return context.Recipes.AddAsync(recipe).AsTask();
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

}

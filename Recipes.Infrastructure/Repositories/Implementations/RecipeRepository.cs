using Microsoft.EntityFrameworkCore;
using Recipes.Application.Common;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeRepository(BaseDbContext context) : IRecipeRepository
{
    public Task<Recipe?> GetByIdAsync(Guid id, RecipeIncludes includes)
    {
        return ApplyIncludes(context.Recipes, includes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<List<Recipe>> GetAllAsync(RecipeIncludes includes)
    {
        return ApplyIncludes(context.Recipes, includes)
            .ToListAsync();
    }

    public Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId, RecipeIncludes includes)
    {
        return ApplyIncludes(context.Recipes, includes)
            .Where(r => r.CreatorId == creatorId)
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

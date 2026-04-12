using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeIngredientRepository(BaseDbContext context) : IRecipeIngredientRepository
{
    public async Task<List<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId)
    {
        return await context.RecipeIngredients
            .Include(ri => ri.Ingredient)
            .Where(ri => ri.RecipeId == recipeId)
            .ToListAsync();
    }

    public async Task AddAsync(RecipeIngredient recipeIngredient)
    {
        await context.RecipeIngredients.AddAsync(recipeIngredient);
    }

    public async Task AddRangeAsync(IEnumerable<RecipeIngredient> recipeIngredients)
    {
        await context.RecipeIngredients.AddRangeAsync(recipeIngredients);
    }

    public async Task DeleteByRecipeIdAsync(Guid recipeId)
    {
        var recipeIngredients = await context.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId)
            .ToListAsync();
        context.RecipeIngredients.RemoveRange(recipeIngredients);
    }

    public async Task DeleteAsync(RecipeIngredient recipeIngredient)
    {
        context.RecipeIngredients.Remove(recipeIngredient);
    }
}

using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.RecipesRelations;
using Recipes.Infrastructure;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeImageRepository(BaseDbContext context) : IRecipeImageRepository
{
    public async Task<List<RecipeImage>> GetByRecipeIdAsync(Guid recipeId)
    {
        return await context.RecipeImages
            .Include(ri => ri.Image)
            .Where(ri => ri.RecipeId == recipeId)
            .OrderBy(ri => ri.Order)
            .ToListAsync();
    }

    public async Task AddAsync(RecipeImage recipeImage)
    {
        await context.RecipeImages.AddAsync(recipeImage);
    }

    public async Task AddRangeAsync(IEnumerable<RecipeImage> recipeImages)
    {
        await context.RecipeImages.AddRangeAsync(recipeImages);
    }

    public async Task DeleteByRecipeIdAsync(Guid recipeId)
    {
        var recipeImages = await context.RecipeImages
            .Where(ri => ri.RecipeId == recipeId)
            .ToListAsync();
        context.RecipeImages.RemoveRange(recipeImages);
    }

    public async Task DeleteAsync(RecipeImage recipeImage)
    {
        context.RecipeImages.Remove(recipeImage);
    }
}
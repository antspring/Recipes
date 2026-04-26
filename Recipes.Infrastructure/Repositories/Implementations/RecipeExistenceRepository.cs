using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeExistenceRepository(BaseDbContext context) : IRecipeExistenceRepository
{
    public Task<bool> ExistsAsync(Guid recipeId)
    {
        return context.Recipes.AnyAsync(r => r.Id == recipeId);
    }
}

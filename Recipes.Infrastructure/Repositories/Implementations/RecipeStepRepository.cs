using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeStepRepository(BaseDbContext context) : IRecipeStepRepository
{
    public Task<RecipeStep?> GetByIdAsync(Guid id)
    {
        return context.RecipeSteps
            .Include(rs => rs.Recipe)
            .Include(rs => rs.Image)
            .FirstOrDefaultAsync(rs => rs.Id == id);
    }

    public Task<List<RecipeStep>> GetByRecipeIdAsync(Guid recipeId)
    {
        return context.RecipeSteps
            .Where(rs => rs.RecipeId == recipeId)
            .Include(rs => rs.Image)
            .OrderBy(rs => rs.Order)
            .ToListAsync();
    }

    public Task AddAsync(RecipeStep step)
    {
        return context.RecipeSteps.AddAsync(step).AsTask();
    }

    public Task UpdateAsync(RecipeStep step)
    {
        context.RecipeSteps.Update(step);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(RecipeStep step)
    {
        context.RecipeSteps.Remove(step);
        return Task.CompletedTask;
    }
}

using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class IngredientRepository(BaseDbContext context) : IIngredientRepository
{
    public async Task<Ingredient?> GetByIdAsync(Guid id)
    {
        return await context.Ingredients.FindAsync(id);
    }

    public async Task<List<Ingredient>> GetAllAsync()
    {
        return await context.Ingredients.ToListAsync();
    }

    public async Task AddAsync(Ingredient ingredient)
    {
        await context.Ingredients.AddAsync(ingredient);
    }

    public async Task UpdateAsync(Ingredient ingredient)
    {
        context.Ingredients.Update(ingredient);
    }

    public async Task DeleteAsync(Ingredient ingredient)
    {
        context.Ingredients.Remove(ingredient);
    }

    public async Task<List<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids)
    {
        return await context.Ingredients
            .Where(i => ids.Contains(i.Id))
            .Select(i => i.Id).ToListAsync();
    }
}
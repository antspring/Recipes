using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class IngredientRepository(BaseDbContext context) : IIngredientRepository
{
    public Task<List<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids)
    {
        return context.Ingredients
            .Where(i => ids.Contains(i.Id))
            .Select(i => i.Id).ToListAsync();
    }
}
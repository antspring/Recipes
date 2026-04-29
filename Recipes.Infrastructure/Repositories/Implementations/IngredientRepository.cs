using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class IngredientRepository(BaseDbContext context) : IIngredientRepository
{
    public Task<List<Ingredient>> GetAllAsync()
    {
        return context.Ingredients
            .OrderBy(i => i.Title)
            .ToListAsync();
    }

    public Task<List<Ingredient>> SearchByTitleAsync(string? title)
    {
        var query = context.Ingredients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            query = query.Where(i => EF.Functions.ILike(i.Title, ToContainsPattern(title)));
        }

        return query
            .OrderBy(i => i.Title)
            .ToListAsync();
    }

    public Task<List<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids)
    {
        return context.Ingredients
            .Where(i => ids.Contains(i.Id))
            .Select(i => i.Id).ToListAsync();
    }

    private static string ToContainsPattern(string value)
    {
        return $"%{value.Trim()}%";
    }
}

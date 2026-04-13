using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class UnwantedIngredientsRepository : IUnwantedIngredientsRepository
{
    private readonly BaseDbContext _context;

    public UnwantedIngredientsRepository(BaseDbContext context)
    {
        _context = context;
    }

    public Task<List<UnwantedIngredients>> GetByUserIdAsync(Guid userId)
    {
        return _context.UnwantedIngredients
            .Include(ui => ui.Ingredient)
            .Where(ui => ui.UserId == userId)
            .ToListAsync();
    }

    public Task AddRangeAsync(IEnumerable<UnwantedIngredients> ingredients)
    {
        return _context.UnwantedIngredients.AddRangeAsync(ingredients);
    }

    public async Task RemoveRangeAsync(IEnumerable<(Guid UserId, Guid IngredientId)> ingredients)
    {
        var tuples = ingredients.ToList();
        if (tuples.Count == 0) return;

        var ingredientsToDelete = await _context.UnwantedIngredients
            .Where(ui => tuples.Any(t => t.UserId == ui.UserId && t.IngredientId == ui.IngredientId))
            .ToListAsync();

        _context.UnwantedIngredients.RemoveRange(ingredientsToDelete);
    }

    public async Task RemoveByUserIdAsync(Guid userId)
    {
        var ingredients = await _context.UnwantedIngredients
            .Where(ui => ui.UserId == userId)
            .ToListAsync();

        _context.UnwantedIngredients.RemoveRange(ingredients);
    }
}
using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class UserIngredientRelationRepository<T>(BaseDbContext context) : IUserIngredientRelationRepository<T>
    where T : class, IUserIngredientRelation
{
    private readonly DbSet<T> _dbSet = context.Set<T>();

    public Task<List<T>> GetByUserIdAsync(Guid userId)
    {
        return _dbSet
            .Include(ui => ui.Ingredient)
            .Where(ui => ui.UserId == userId)
            .ToListAsync();
    }

    public Task AddRangeAsync(IEnumerable<T> ingredients)
    {
        return _dbSet.AddRangeAsync(ingredients);
    }

    public async Task RemoveRangeAsync(Guid userId, IEnumerable<Guid> ingredients)
    {
        var ingredientsToDelete = await _dbSet
            .Where(ui => userId == ui.UserId && ingredients.Contains(ui.IngredientId))
            .ToListAsync();

        _dbSet.RemoveRange(ingredientsToDelete);
    }

    public async Task RemoveByUserIdAsync(Guid userId)
    {
        var ingredients = await _dbSet
            .Where(ui => ui.UserId == userId)
            .ToListAsync();

        _dbSet.RemoveRange(ingredients);
    }
}

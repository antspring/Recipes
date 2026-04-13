using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IUnwantedIngredientsRepository
{
    Task<List<UnwantedIngredients>> GetByUserIdAsync(Guid userId);
    Task AddRangeAsync(IEnumerable<UnwantedIngredients> ingredients);
    Task RemoveRangeAsync(IEnumerable<(Guid UserId, Guid IngredientId)> ingredients);
    Task RemoveByUserIdAsync(Guid userId);
}
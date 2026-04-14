using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IUserIngredientRelationRepository<T> where T : class, IUserIngredientRelation
{
    Task<List<T>> GetByUserIdAsync(Guid userId);
    Task AddRangeAsync(IEnumerable<T> ingredients);
    Task RemoveRangeAsync(Guid userId, IEnumerable<Guid> ingredients);
    Task RemoveByUserIdAsync(Guid userId);
}
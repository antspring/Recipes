using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IIngredientRepository
{
    Task<List<Ingredient>> GetAllAsync();
    Task<List<Ingredient>> SearchByTitleAsync(string? title);
    Task<List<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids);
}

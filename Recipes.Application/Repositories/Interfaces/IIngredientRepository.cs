using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IIngredientRepository
{
    Task<Ingredient?> GetByIdAsync(Guid id);
    Task<List<Ingredient>> GetAllAsync();
    Task AddAsync(Ingredient ingredient);
    Task UpdateAsync(Ingredient ingredient);
    Task DeleteAsync(Ingredient ingredient);
}

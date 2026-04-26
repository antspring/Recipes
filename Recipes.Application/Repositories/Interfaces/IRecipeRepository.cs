using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id);
    Task<List<Recipe>> GetAllAsync();
    Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId);
    Task AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(Recipe recipe);
    Task<bool> ExistsAsync(Guid id);
}

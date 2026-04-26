namespace Recipes.Application.Repositories.Interfaces;

public interface IIngredientRepository
{
    Task<List<Guid>> GetExistingIdsAsync(IEnumerable<Guid> ids);
}
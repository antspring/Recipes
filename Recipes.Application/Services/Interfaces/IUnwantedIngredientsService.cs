using Recipes.Application.DTO.Ingredient;

namespace Recipes.Application.Services.Interfaces;

public interface IUnwantedIngredientsService
{
    Task<List<IngredientDto>> GetUnwantedIngredientsAsync(Guid userId);
    Task SetUnwantedIngredientsAsync(Guid userId, List<Guid> ingredientIds);
    Task AddUnwantedIngredientsAsync(Guid userId, List<Guid> ingredientIds);
    Task RemoveUnwantedIngredientsAsync(Guid userId, List<Guid> ingredientIds);
}
using Recipes.Application.DTO.Ingredient;

namespace Recipes.Application.Services.Interfaces;

public interface IIngredientService
{
    Task<List<IngredientDto>> GetAllAsync();
    Task<List<IngredientDto>> SearchByTitleAsync(string? title);
}

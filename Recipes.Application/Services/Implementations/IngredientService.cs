using Recipes.Application.DTO.Ingredient;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class IngredientService(IIngredientRepository ingredientRepository) : IIngredientService
{
    public async Task<List<IngredientDto>> GetAllAsync()
    {
        var ingredients = await ingredientRepository.GetAllAsync();
        return ingredients.Select(ToDto).ToList();
    }

    public async Task<List<IngredientDto>> SearchByTitleAsync(string? title)
    {
        var ingredients = await ingredientRepository.SearchByTitleAsync(title);
        return ingredients.Select(ToDto).ToList();
    }

    private static IngredientDto ToDto(Ingredient ingredient)
    {
        return new IngredientDto
        {
            Id = ingredient.Id,
            Title = ingredient.Title
        };
    }
}

using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeDtoFactory
{
    Task<RecipeDto> CreateAsync(Recipe recipe);
    Task<List<RecipeDto>> CreateManyAsync(IEnumerable<Recipe> recipes);
}

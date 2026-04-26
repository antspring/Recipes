using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeIngredientService
{
    Task<List<RecipeIngredient>> SaveRecipeIngredientsAsync(
        IReadOnlyCollection<RecipeIngredientInputDto> ingredientsDto,
        Guid recipeId);
}

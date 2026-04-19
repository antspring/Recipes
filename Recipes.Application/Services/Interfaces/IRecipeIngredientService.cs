using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeIngredientService
{
    Task<List<RecipeIngredient>> SaveRecipeIngredientsAsync(List<RecipeIngredientInputDto> ingredientsDto,
        Guid recipeId);
}
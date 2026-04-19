using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeIngredientRepository
{
    Task<List<RecipeIngredient>> GetByRecipeIdAsync(Guid recipeId);
    Task AddAsync(RecipeIngredient recipeIngredient);
    Task AddRangeAsync(IEnumerable<RecipeIngredient> recipeIngredients);
    Task DeleteByRecipeIdAsync(Guid recipeId);
    Task DeleteAsync(RecipeIngredient recipeIngredient);
}
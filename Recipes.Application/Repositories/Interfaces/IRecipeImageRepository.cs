using Recipes.Domain.Models.RecipesRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeImageRepository
{
    Task<List<RecipeImage>> GetByRecipeIdAsync(Guid recipeId);
    Task AddAsync(RecipeImage recipeImage);
    Task AddRangeAsync(IEnumerable<RecipeImage> recipeImages);
    Task DeleteByRecipeIdAsync(Guid recipeId);
    Task DeleteAsync(RecipeImage recipeImage);
}
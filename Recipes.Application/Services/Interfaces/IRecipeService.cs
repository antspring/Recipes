using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeService
{
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto);
    Task<RecipeDto?> GetRecipeByIdAsync(Guid id);
    Task<List<RecipeDto>> GetAllRecipesAsync();
    Task<List<RecipeDto>> GetRecipesByCreatorIdAsync(Guid creatorId);
    Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto updateRecipeDto);
    Task DeleteRecipeAsync(Guid id);
}

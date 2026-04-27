using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeCrudService
{
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto);
    Task<RecipeDto?> GetRecipeByIdAsync(Guid id, RecipeIncludes? includes = null);
    Task<List<RecipeDto>> GetAllRecipesAsync(RecipeIncludes? includes = null);
    Task<List<RecipeDto>> GetRecipesByCreatorIdAsync(Guid creatorId, RecipeIncludes? includes = null);
    Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto updateRecipeDto);
    Task DeleteRecipeAsync(Guid id, Guid actorUserId);
}

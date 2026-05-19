using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeCrudService
{
    Task<RecipeDto> CreateRecipeAsync(CreateRecipeDto createRecipeDto);
    Task<RecipeDto?> GetRecipeByIdAsync(Guid id, RecipeIncludes? includes = null, Guid? currentUserId = null);
    Task<PagedResult<RecipeDto>> GetAllRecipesAsync(RecipeIncludes? includes = null, Guid? currentUserId = null,
        int page = 1, int pageSize = 20);
    Task<List<RecipeDto>> GetRecipesByCreatorIdAsync(Guid creatorId, RecipeIncludes? includes = null, Guid? currentUserId = null);
    Task<RecipeDto> UpdateRecipeAsync(UpdateRecipeDto updateRecipeDto);
    Task DeleteRecipeAsync(Guid id, Guid actorUserId);
    Task DeleteRecipeByModeratorAsync(Guid id);
}

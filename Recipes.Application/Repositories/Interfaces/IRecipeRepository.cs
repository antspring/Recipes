using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRecipeRepository
{
    Task<Recipe?> GetByIdAsync(Guid id, RecipeIncludes includes);
    Task<List<Recipe>> GetAllAsync(RecipeIncludes includes);
    Task<PagedResult<Recipe>> GetPagedAsync(RecipeIncludes includes, int page, int pageSize);
    Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId, RecipeIncludes includes);
    Task<PagedResult<Recipe>> SearchAsync(RecipeSearchFilterDto filter, RecipeIncludes includes, Guid? actorUserId);
    Task AddAsync(Recipe recipe);
    Task UpdateAsync(Recipe recipe);
    Task DeleteAsync(Recipe recipe);
}

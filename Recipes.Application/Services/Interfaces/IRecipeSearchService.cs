using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.Services.Interfaces;

public interface IRecipeSearchService
{
    Task<List<RecipeDto>> SearchAsync(
        RecipeSearchFilterDto filter,
        RecipeIncludes? includes = null,
        Guid? actorUserId = null);
}

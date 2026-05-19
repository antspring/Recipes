using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class RecipeSearchService(
    IRecipeRepository recipeRepository,
    IRecipeDtoFactory recipeDtoFactory) : IRecipeSearchService
{
    public async Task<PagedResult<RecipeDto>> SearchAsync(
        RecipeSearchFilterDto filter,
        RecipeIncludes? includes = null,
        Guid? actorUserId = null)
    {
        ValidateCaloriesRange(filter);
        PaginationValidator.EnsureValid(filter.Page, filter.PageSize);

        var recipes = await recipeRepository.SearchAsync(filter, GetReadIncludes(includes), actorUserId);
        var dtos = await recipeDtoFactory.CreateManyAsync(recipes.Items, actorUserId);

        return new PagedResult<RecipeDto>
        {
            Items = dtos,
            TotalCount = recipes.TotalCount,
            Page = recipes.Page,
            PageSize = recipes.PageSize
        };
    }

    private static void ValidateCaloriesRange(RecipeSearchFilterDto filter)
    {
        if (filter.MinCalories < 0)
            throw new ArgumentException("Min calories must be non-negative");

        if (filter.MaxCalories < 0)
            throw new ArgumentException("Max calories must be non-negative");

        if (filter.MinCalories > filter.MaxCalories)
            throw new ArgumentException("Min calories must be less than or equal to max calories");

        if (filter.MaxCookingTime < TimeSpan.Zero || filter.MaxCookingTime >= TimeSpan.FromDays(1))
            throw new ArgumentException("Max cooking time must be between 00:00:00 and 23:59:59");
    }

    private static RecipeIncludes GetReadIncludes(RecipeIncludes? includes)
    {
        return includes ?? RecipeIncludes.Full;
    }

}

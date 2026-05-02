using Microsoft.EntityFrameworkCore;
using Recipes.Application.Common;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Extensions;

public static class RecipeQueryExtensions
{
    public static IQueryable<Recipe> ApplyIncludes(this IQueryable<Recipe> query, RecipeIncludes includes)
    {
        if (includes.HasFlag(RecipeIncludes.Creator))
            query = query.Include(r => r.Creator);

        if (includes.HasFlag(RecipeIncludes.Ingredients))
            query = query
                .Include(r => r.RecipeIngredients)
                .ThenInclude(ri => ri.Ingredient);

        if (includes.HasFlag(RecipeIncludes.Images))
            query = query
                .Include(r => r.RecipeImages)
                .ThenInclude(ri => ri.Image);

        if (includes.HasFlag(RecipeIncludes.Steps))
            query = query
                .Include(r => r.Steps)
                .ThenInclude(rs => rs.Image);

        return query;
    }
}

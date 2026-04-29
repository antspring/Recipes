using Microsoft.EntityFrameworkCore;
using Recipes.Application.Common;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RecipeRepository(BaseDbContext context) : IRecipeRepository
{
    public Task<Recipe?> GetByIdAsync(Guid id, RecipeIncludes includes)
    {
        return ApplyIncludes(context.Recipes, includes)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public Task<List<Recipe>> GetAllAsync(RecipeIncludes includes)
    {
        return ApplyIncludes(context.Recipes, includes)
            .ToListAsync();
    }

    public Task<List<Recipe>> GetByCreatorIdAsync(Guid creatorId, RecipeIncludes includes)
    {
        return ApplyIncludes(context.Recipes, includes)
            .Where(r => r.CreatorId == creatorId)
            .ToListAsync();
    }

    public Task<List<Recipe>> SearchAsync(RecipeSearchFilterDto filter, RecipeIncludes includes, Guid? actorUserId)
    {
        var query = ApplySearchFilters(context.Recipes, filter, actorUserId);

        return ApplyIncludes(query, includes)
            .ToListAsync();
    }

    public Task AddAsync(Recipe recipe)
    {
        return context.Recipes.AddAsync(recipe).AsTask();
    }

    public Task UpdateAsync(Recipe recipe)
    {
        context.Recipes.Update(recipe);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Recipe recipe)
    {
        context.Recipes.Remove(recipe);
        return Task.CompletedTask;
    }

    private static IQueryable<Recipe> ApplyIncludes(IQueryable<Recipe> query, RecipeIncludes includes)
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

    private static IQueryable<Recipe> ApplySearchFilters(
        IQueryable<Recipe> query,
        RecipeSearchFilterDto filter,
        Guid? actorUserId)
    {
        if (!string.IsNullOrWhiteSpace(filter.Title))
            query = query.Where(r => EF.Functions.ILike(r.Title, ToContainsPattern(filter.Title)));

        if (!string.IsNullOrWhiteSpace(filter.MealType))
            query = query.Where(r => r.MealType != null && EF.Functions.ILike(r.MealType, ToContainsPattern(filter.MealType)));

        if (!string.IsNullOrWhiteSpace(filter.DishType))
            query = query.Where(r => r.DishType != null && EF.Functions.ILike(r.DishType, ToContainsPattern(filter.DishType)));

        if (filter.MaxCookingTime.HasValue)
            query = query.Where(r => r.CookingTime.HasValue && r.CookingTime <= filter.MaxCookingTime);

        if (filter.MinCalories.HasValue)
            query = query.Where(r => r.CaloricValue >= filter.MinCalories);

        if (filter.MaxCalories.HasValue)
            query = query.Where(r => r.CaloricValue <= filter.MaxCalories);

        if (filter.ContainsIngredientIds.Count > 0)
            query = query.Where(r => r.RecipeIngredients.Any(ri => filter.ContainsIngredientIds.Contains(ri.IngredientId)));

        if (filter.ExcludedIngredientIds.Count > 0)
            query = query.Where(r => !r.RecipeIngredients.Any(ri => filter.ExcludedIngredientIds.Contains(ri.IngredientId)));

        if (filter.ExcludeUserAllergens && actorUserId.HasValue)
        {
            query = query.Where(r => !r.RecipeIngredients.Any(ri =>
                ri.Ingredient.UsersAllergens.Any(al => al.UserId == actorUserId.Value)));
        }

        return query;
    }

    private static string ToContainsPattern(string value)
    {
        return $"%{value.Trim()}%";
    }
}

using Recipes.Application.DTO.Ingredient;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class UserIngredientRelationService<T>(
    IUnitOfWork unitOfWork,
    IUserIngredientRelationRepository<T> repository,
    IIngredientRepository ingredientRepository) : IUserIngredientRelationService<T>
    where T : class, IUserIngredientRelation, new()
{
    private T CreateRelation(Guid userId, Guid ingredientId)
    {
        return new T
        {
            UserId = userId,
            IngredientId = ingredientId
        };
    }

    public async Task<List<IngredientDto>> GetUserIngredientRelationAsync(Guid userId)
    {
        var ingredients = await repository
            .GetByUserIdAsync(userId);

        return ingredients
            .Select(relation => ToIngredientDto(relation.Ingredient))
            .ToList();
    }

    public async Task SetUserIngredientRelationAsync(Guid userId, IReadOnlyCollection<Guid> ingredientIds)
    {
        var normalizedIngredientIds = await ValidateAndNormalizeIngredientIdsAsync(ingredientIds);
        await repository.RemoveByUserIdAsync(userId);

        var ingredients = normalizedIngredientIds
            .Select(id => CreateRelation(userId, id));

        await repository.AddRangeAsync(ingredients);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task AddUserIngredientRelationAsync(Guid userId, IReadOnlyCollection<Guid> ingredientIds)
    {
        var normalizedIngredientIds = await ValidateAndNormalizeIngredientIdsAsync(ingredientIds);

        var existing = await repository.GetByUserIdAsync(userId);
        var existingIds = existing.Select(i => i.IngredientId).ToHashSet();

        var newIngredients = normalizedIngredientIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => CreateRelation(userId, id))
            .ToList();

        if (newIngredients.Count > 0)
        {
            await repository.AddRangeAsync(newIngredients);
            await unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveUserIngredientRelationAsync(Guid userId, IReadOnlyCollection<Guid> ingredientIds)
    {
        var normalizedIngredientIds = await ValidateAndNormalizeIngredientIdsAsync(ingredientIds);

        await repository.RemoveRangeAsync(userId, normalizedIngredientIds);
        await unitOfWork.SaveChangesAsync();
    }

    private static List<Guid> NormalizeIngredientIds(IEnumerable<Guid> ingredientIds)
    {
        return ingredientIds.Distinct().ToList();
    }

    private static IngredientDto ToIngredientDto(Ingredient ingredient)
    {
        return new IngredientDto
        {
            Id = ingredient.Id,
            Title = ingredient.Title
        };
    }

    private async Task<List<Guid>> ValidateAndNormalizeIngredientIdsAsync(IEnumerable<Guid> ingredientIds)
    {
        var normalizedIngredientIds = NormalizeIngredientIds(ingredientIds);
        await ValidateIngredientsExistAsync(normalizedIngredientIds);
        return normalizedIngredientIds;
    }

    private async Task ValidateIngredientsExistAsync(List<Guid> ingredientIds)
    {
        var existingIds = await ingredientRepository.GetExistingIdsAsync(ingredientIds);
        var notFoundIds = ingredientIds.Except(existingIds).ToArray();

        if (notFoundIds.Length > 0)
            throw new KeyNotFoundException($"Ingredients with Id {string.Join(", ", notFoundIds)} not found");
    }
}

using AutoMapper;
using Recipes.Application.DTO.Ingredient;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class UserIngredientRelationService<T> : IUserIngredientRelationService<T>
    where T : class, IUserIngredientRelation
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserIngredientRelationService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private IUserIngredientRelationRepository<T> Repository
        => _unitOfWork.GetUserIngredientRelationRepository<T>();

    private T CreateRelation(Guid userId, Guid ingredientId)
    {
        return (T)Activator.CreateInstance(
            typeof(T),
            userId,
            ingredientId
        )!;
    }

    public async Task<List<IngredientDto>> GetUserIngredientRelationAsync(Guid userId)
    {
        var ingredients = await Repository
            .GetByUserIdAsync(userId);

        return _mapper.Map<List<IngredientDto>>(
            ingredients.Select(i => i.Ingredient).ToList()
        );
    }

    public async Task SetUserIngredientRelationAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);
        await Repository.RemoveByUserIdAsync(userId);

        var ingredients = ingredientIds
            .Select(id => CreateRelation(userId, id));

        await Repository.AddRangeAsync(ingredients);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddUserIngredientRelationAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        var existing = await Repository.GetByUserIdAsync(userId);
        var existingIds = existing.Select(i => i.IngredientId).ToHashSet();

        var newIngredients = ingredientIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => CreateRelation(userId, id))
            .ToList();

        if (newIngredients.Count > 0)
        {
            await Repository.AddRangeAsync(newIngredients);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveUserIngredientRelationAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        await Repository.RemoveRangeAsync(userId, ingredientIds);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ValidateIngredientsExistAsync(List<Guid> ingredientIds)
    {
        var existingIds = await _unitOfWork.Ingredients.GetExistingIdsAsync(ingredientIds);
        var notFoundIds = ingredientIds.Except(existingIds).ToArray();

        if (notFoundIds.Length > 0)
            throw new KeyNotFoundException($"Ingredients with Id {string.Join(", ", notFoundIds)} not found");
    }
}
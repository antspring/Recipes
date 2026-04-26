using AutoMapper;
using Recipes.Application.DTO.Ingredient;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class UserIngredientRelationService<T> : IUserIngredientRelationService<T>
    where T : class, IUserIngredientRelation, new()
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserIngredientRelationRepository<T> _repository;
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IMapper _mapper;

    public UserIngredientRelationService(
        IUnitOfWork unitOfWork,
        IUserIngredientRelationRepository<T> repository,
        IIngredientRepository ingredientRepository,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _ingredientRepository = ingredientRepository;
        _mapper = mapper;
    }

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
        var ingredients = await _repository
            .GetByUserIdAsync(userId);

        return _mapper.Map<List<IngredientDto>>(
            ingredients.Select(i => i.Ingredient).ToList()
        );
    }

    public async Task SetUserIngredientRelationAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);
        await _repository.RemoveByUserIdAsync(userId);

        var ingredients = ingredientIds
            .Select(id => CreateRelation(userId, id));

        await _repository.AddRangeAsync(ingredients);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddUserIngredientRelationAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        var existing = await _repository.GetByUserIdAsync(userId);
        var existingIds = existing.Select(i => i.IngredientId).ToHashSet();

        var newIngredients = ingredientIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => CreateRelation(userId, id))
            .ToList();

        if (newIngredients.Count > 0)
        {
            await _repository.AddRangeAsync(newIngredients);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveUserIngredientRelationAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        await _repository.RemoveRangeAsync(userId, ingredientIds);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ValidateIngredientsExistAsync(List<Guid> ingredientIds)
    {
        var existingIds = await _ingredientRepository.GetExistingIdsAsync(ingredientIds);
        var notFoundIds = ingredientIds.Except(existingIds).ToArray();

        if (notFoundIds.Length > 0)
            throw new KeyNotFoundException($"Ingredients with Id {string.Join(", ", notFoundIds)} not found");
    }
}
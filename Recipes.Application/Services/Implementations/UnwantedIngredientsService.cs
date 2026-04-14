using AutoMapper;
using Recipes.Application.DTO.Ingredient;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class UnwantedIngredientsService : IUnwantedIngredientsService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UnwantedIngredientsService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<IngredientDto>> GetUnwantedIngredientsAsync(Guid userId)
    {
        var ingredients = await _unitOfWork.UnwantedIngredients
            .GetByUserIdAsync(userId);

        return _mapper.Map<List<IngredientDto>>(
            ingredients.Select(i => i.Ingredient).ToList()
        );
    }

    public async Task SetUnwantedIngredientsAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        await _unitOfWork.UnwantedIngredients.RemoveByUserIdAsync(userId);

        var ingredients = ingredientIds
            .Select(id =>
                new UnwantedIngredients
                {
                    UserId = userId,
                    IngredientId = id
                });

        await _unitOfWork.UnwantedIngredients.AddRangeAsync(ingredients);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddUnwantedIngredientsAsync(Guid userId, List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        var existing = await _unitOfWork.UnwantedIngredients.GetByUserIdAsync(userId);
        var existingIds = existing.Select(i => i.IngredientId).ToHashSet();

        var newIngredients = ingredientIds
            .Where(id => !existingIds.Contains(id))
            .Select(id => new UnwantedIngredients
            {
                UserId = userId,
                IngredientId = id
            }).ToList();

        if (newIngredients.Count > 0)
        {
            await _unitOfWork.UnwantedIngredients.AddRangeAsync(newIngredients);
            await _unitOfWork.SaveChangesAsync();
        }
    }

    public async Task RemoveUnwantedIngredientsAsync(Guid userId, List<Guid> ingredientIds)
    {
        var tuples = ingredientIds.Select(id => (userId, id)).ToList();
        await _unitOfWork.UnwantedIngredients.RemoveRangeAsync(tuples);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task ValidateIngredientsExistAsync(List<Guid> ingredientIds)
    {
        await ValidateIngredientsExistAsync(ingredientIds);

        var existingIds = await _unitOfWork.Ingredients.GetExistingIdsAsync(ingredientIds);
        var notFoundIds = ingredientIds.Except(existingIds);

        if (notFoundIds.Any())
            throw new KeyNotFoundException($"Ingredients with Id {string.Join(", ", notFoundIds)} not found");
    }
}
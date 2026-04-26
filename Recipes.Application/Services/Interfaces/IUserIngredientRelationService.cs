using Recipes.Application.DTO.Ingredient;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Interfaces;

public interface IUserIngredientRelationService<T> where T : class, IUserIngredientRelation, new()
{
    Task<List<IngredientDto>> GetUserIngredientRelationAsync(Guid userId);
    Task SetUserIngredientRelationAsync(Guid userId, IReadOnlyCollection<Guid> ingredientIds);
    Task AddUserIngredientRelationAsync(Guid userId, IReadOnlyCollection<Guid> ingredientIds);
    Task RemoveUserIngredientRelationAsync(Guid userId, IReadOnlyCollection<Guid> ingredientIds);
}
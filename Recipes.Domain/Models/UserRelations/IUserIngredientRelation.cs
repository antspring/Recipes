namespace Recipes.Domain.Models.UserRelations;

public interface IUserIngredientRelation
{
    Guid UserId { get; init; }
    Guid IngredientId { get; init; }
    User User { get; init; }
    Ingredient Ingredient { get; init; }
}
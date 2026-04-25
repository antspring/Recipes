namespace Recipes.Domain.Models.UserRelations;

public class Allergens : IUserIngredientRelation
{
    public Guid UserId { get; init; }
    public Guid IngredientId { get; init; }

    public User User { get; init; } = null!;
    public Ingredient Ingredient { get; init; } = null!;

    public Allergens()
    {
    }

    public Allergens(Guid userId, Guid ingredientId)
    {
        UserId = userId;
        IngredientId = ingredientId;
    }
}
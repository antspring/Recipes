namespace Recipes.Domain.Models.UserRelations;

public class UnwantedIngredients : IUserIngredientRelation
{
    public Guid UserId { get; init; }
    public Guid IngredientId { get; init; }

    public User User { get; init; } = null!;
    public Ingredient Ingredient { get; init; } = null!;

    public UnwantedIngredients()
    {
    }

    public UnwantedIngredients(Guid userId, Guid ingredientId)
    {
        UserId = userId;
        IngredientId = ingredientId;
    }
}
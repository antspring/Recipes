using Microsoft.EntityFrameworkCore;

namespace Recipes.Domain.Models.UserRelations;

[PrimaryKey(nameof(UserId), nameof(IngredientId))]
public class UnwantedIngredients
{
    public Guid UserId { get; init; }
    public Guid IngredientId { get; init; }

    public User User { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
}
namespace Recipes.Domain.Models.RecipesRelations;

public class RecipeRating
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }

    public int Value { get; set; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }

    public Recipe Recipe { get; init; } = null!;
    public User User { get; init; } = null!;
}

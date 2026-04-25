namespace Recipes.Domain.Models.RecipesRelations;

public class RecipeIngredient
{
    public Guid RecipeId { get; init; }
    public Guid IngredientId { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public Ingredient Ingredient { get; init; } = null!;

    public int? Weight { get; set; }

    public string? AlternativeWeight { get; set; }
}
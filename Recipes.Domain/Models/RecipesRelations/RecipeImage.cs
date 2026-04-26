namespace Recipes.Domain.Models.RecipesRelations;

public class RecipeImage
{
    public Guid RecipeId { get; init; }
    public Guid ImageId { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public Image Image { get; init; } = null!;

    public int Order { get; set; }
}
namespace Recipes.Domain.Models;

public class RecipeStep
{
    public Guid Id { get; init; }

    public Guid RecipeId { get; init; }
    public Recipe Recipe { get; init; } = null!;

    public string Description { get; set; } = null!;
    public int Order { get; set; }

    public Guid? ImageId { get; set; }
    public Image? Image { get; set; }
}

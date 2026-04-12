namespace Recipes.Application.DTO.Recipe;

public class RecipeIngredientDto
{
    public Guid IngredientId { get; set; }
    public string IngredientTitle { get; set; } = null!;
    public int? Weight { get; set; }
    public string? AlternativeWeight { get; set; }
}
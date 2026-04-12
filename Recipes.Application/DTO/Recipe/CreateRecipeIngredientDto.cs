namespace Recipes.Application.DTO.Recipe;

public class CreateRecipeIngredientDto
{
    public Guid IngredientId { get; set; }
    public int? Weight { get; set; }
    public string? AlternativeWeight { get; set; }
}
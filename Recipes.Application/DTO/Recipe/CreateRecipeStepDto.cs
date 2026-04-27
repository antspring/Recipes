namespace Recipes.Application.DTO.Recipe;

public class CreateRecipeStepDto
{
    public Guid RecipeId { get; set; }
    public Guid ActorUserId { get; set; }
    public string Description { get; set; } = null!;
    public int? Order { get; set; }
    public ImageUpload? ImageUpload { get; set; }
}

namespace Recipes.Application.DTO.Recipe;

public class UpdateRecipeStepDto
{
    public Guid RecipeId { get; set; }
    public Guid StepId { get; set; }
    public Guid ActorUserId { get; set; }
    public string Description { get; set; } = null!;
    public int? Order { get; set; }
    public TimeSpan? CookingTime { get; set; }
    public ImageUpload? ImageUpload { get; set; }
    public bool DeleteImage { get; set; }
}

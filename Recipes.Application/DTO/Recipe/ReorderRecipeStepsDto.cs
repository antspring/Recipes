namespace Recipes.Application.DTO.Recipe;

public class ReorderRecipeStepsDto
{
    public Guid RecipeId { get; set; }
    public Guid ActorUserId { get; set; }
    public List<Guid> StepIds { get; set; } = new();
}

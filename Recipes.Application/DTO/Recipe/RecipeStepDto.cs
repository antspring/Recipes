namespace Recipes.Application.DTO.Recipe;

public class RecipeStepDto
{
    public Guid Id { get; set; }
    public Guid RecipeId { get; set; }
    public string Description { get; set; } = null!;
    public int Order { get; set; }
    public TimeSpan? CookingTime { get; set; }
    public ImageDto? Image { get; set; }

    public static RecipeStepDto FromRecipeStep(Domain.Models.RecipeStep step)
    {
        return new RecipeStepDto
        {
            Id = step.Id,
            RecipeId = step.RecipeId,
            Description = step.Description,
            Order = step.Order,
            CookingTime = step.CookingTime,
            Image = step.Image == null
                ? null
                : new ImageDto
                {
                    Id = step.Image.Id,
                    FileName = step.Image.FileName,
                    CreatedAt = step.Image.CreatedAt
                }
        };
    }
}

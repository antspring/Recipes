namespace Recipes.Application.DTO.Recipe;

public class CreateRecipeDto
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CaloricValue { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }
    public int PortionsCount { get; set; }
    public Guid CreatorId { get; set; }
    public TimeSpan? CookingTime { get; set; }
    public string? DishType { get; set; }
    public string? MealType { get; set; }
    public List<RecipeIngredientInputDto> Ingredients { get; set; } = new();
    public List<ImageUpload> ImageUploads { get; set; } = new();
}
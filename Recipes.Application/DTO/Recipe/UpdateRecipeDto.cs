namespace Recipes.Application.DTO.Recipe;

public class UpdateRecipeDto
{
    public Guid Id { get; set; }
    public Guid ActorUserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? CaloricValue { get; set; }
    public double? Proteins { get; set; }
    public double? Fats { get; set; }
    public double? Carbohydrates { get; set; }
    public int? PortionsCount { get; set; }
    public TimeSpan? CookingTime { get; set; }
    public string? DishType { get; set; }
    public string? MealType { get; set; }
    public List<RecipeIngredientInputDto>? Ingredients { get; set; }
    public List<ImageUpload> ImageUploads { get; set; } = new();
    public List<Guid>? ImageIdsToDelete { get; set; }
}
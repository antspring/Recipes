namespace Recipes.Application.DTO.Recipe;

public class UpdateRecipeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int CaloricValue { get; set; }
    public double Proteins { get; set; }
    public double Fats { get; set; }
    public double Carbohydrates { get; set; }
    public TimeSpan? CookingTime { get; set; }
    public string? DishType { get; set; }
    public string? MealType { get; set; }
    public List<UpdateRecipeIngredientDto> Ingredients { get; set; } = new();
    public List<ImageUpload> ImageUploads { get; set; } = new();
}

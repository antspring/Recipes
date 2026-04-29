namespace Recipes.Application.DTO.Recipe;

public class RecipeSearchFilterDto
{
    public string? Title { get; set; }
    public string? MealType { get; set; }
    public string? DishType { get; set; }
    public TimeSpan? MaxCookingTime { get; set; }
    public int? MinCalories { get; set; }
    public int? MaxCalories { get; set; }
    public bool ExcludeUserAllergens { get; set; }
    public IReadOnlyCollection<Guid> ContainsIngredientIds { get; set; } = [];
    public IReadOnlyCollection<Guid> ExcludedIngredientIds { get; set; } = [];
}

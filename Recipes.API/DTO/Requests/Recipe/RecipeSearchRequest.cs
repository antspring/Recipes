using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class RecipeSearchRequest
{
    public string? Title { get; set; }
    public string? MealType { get; set; }
    public string? DishType { get; set; }

    [Range(typeof(TimeSpan), "00:00:00", "23:59:59",
        ErrorMessage = "MaxCookingTime must be between 00:00:00 and 23:59:59")]
    public TimeSpan? MaxCookingTime { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "MinCalories must be non-negative")]
    public int? MinCalories { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "MaxCalories must be non-negative")]
    public int? MaxCalories { get; set; }

    public bool? ExcludeUserAllergens { get; set; }
    public Guid[]? ContainsIngredientIds { get; set; }
    public Guid[]? ExcludedIngredientIds { get; set; }
}

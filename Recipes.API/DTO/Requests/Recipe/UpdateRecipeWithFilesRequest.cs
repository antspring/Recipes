using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class UpdateRecipeWithFilesRequest
{
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
    public string? Title { get; set; }

    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters")]
    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "CaloricValue must be non-negative")]
    public int? CaloricValue { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Proteins must be non-negative")]
    public double? Proteins { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Fats must be non-negative")]
    public double? Fats { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Carbohydrates must be non-negative")]
    public double? Carbohydrates { get; set; }

    public string? CookingTime { get; set; }

    [StringLength(100, ErrorMessage = "DishType must be between 0 and 100 characters")]
    public string? DishType { get; set; }

    [StringLength(100, ErrorMessage = "MealType must be between 0 and 100 characters")]
    public string? MealType { get; set; }

    [MinLength(1, ErrorMessage = "At least one ingredient is required")]
    public string? IngredientsJson { get; set; } // JSON string of UpdateRecipeIngredientRequest[]

    public IFormFileCollection? Images { get; set; }

    public string? ImageIdsToDelete { get; set; }
}

public class UpdateRecipeIngredientRequest
{
    [Required(ErrorMessage = "IngredientId is required")]
    public Guid IngredientId { get; set; }

    public int? Weight { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "AlternativeWeight must be between 2 and 50 characters")]
    public string? AlternativeWeight { get; set; }
}

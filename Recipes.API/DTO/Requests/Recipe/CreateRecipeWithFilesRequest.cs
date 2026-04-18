using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class CreateRecipeWithFilesRequest
{
    [Required(ErrorMessage = "Title is required")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 150 characters")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "CaloricValue is required")]
    [Range(0, int.MaxValue, ErrorMessage = "CaloricValue must be non-negative")]
    public int CaloricValue { get; set; }

    [Required(ErrorMessage = "Proteins is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Proteins must be non-negative")]
    public double Proteins { get; set; }

    [Required(ErrorMessage = "Fats is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Fats must be non-negative")]
    public double Fats { get; set; }

    [Required(ErrorMessage = "Carbohydrates is required")]
    [Range(0, double.MaxValue, ErrorMessage = "Carbohydrates must be non-negative")]
    public double Carbohydrates { get; set; }

    [Required(ErrorMessage = "Ingredients are required")]
    public string IngredientsJson { get; set; } = null!;

    public IFormFileCollection? Images { get; set; }
}

public class CreateRecipeIngredientRequest
{
    [Required(ErrorMessage = "IngredientId is required")]
    public Guid IngredientId { get; set; }

    public int? Weight { get; set; }

    [StringLength(50, MinimumLength = 2, ErrorMessage = "AlternativeWeight must be between 2 and 50 characters")]
    public string? AlternativeWeight { get; set; }
}
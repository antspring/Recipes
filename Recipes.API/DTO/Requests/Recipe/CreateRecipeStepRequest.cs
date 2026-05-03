using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class CreateRecipeStepRequest
{
    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters")]
    public string Description { get; set; } = null!;

    [Range(0, int.MaxValue, ErrorMessage = "Order must be non-negative")]
    public int? Order { get; set; }

    [Range(typeof(TimeSpan), "00:00:00", "23:59:59",
        ErrorMessage = "CookingTime must be between 00:00:00 and 23:59:59")]
    public TimeSpan? CookingTime { get; set; }

    public IFormFile? Image { get; set; }
}

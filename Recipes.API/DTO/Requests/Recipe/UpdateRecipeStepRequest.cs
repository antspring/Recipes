using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class UpdateRecipeStepRequest
{
    [Required(ErrorMessage = "Description is required")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters")]
    public string Description { get; set; } = null!;

    [Range(0, int.MaxValue, ErrorMessage = "Order must be non-negative")]
    public int? Order { get; set; }

    public IFormFile? Image { get; set; }

    public bool DeleteImage { get; set; }
}

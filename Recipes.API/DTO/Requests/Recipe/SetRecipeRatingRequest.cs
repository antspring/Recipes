using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class SetRecipeRatingRequest
{
    [Required(ErrorMessage = "value field is required")]
    public int Value { get; set; }
}

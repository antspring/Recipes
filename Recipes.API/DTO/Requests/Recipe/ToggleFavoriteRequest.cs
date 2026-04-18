using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class ToggleFavoriteRequest
{
    [Required(ErrorMessage = "isFavorite field is required")]
    public bool IsFavorite { get; set; }
}
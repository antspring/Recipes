using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Recipe;

public class ToggleLikeRequest
{
    [Required(ErrorMessage = "isLiked field is required")]
    public bool IsLiked { get; set; }
}
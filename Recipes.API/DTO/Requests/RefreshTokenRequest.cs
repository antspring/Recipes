using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests;

public class RefreshTokenRequest
{
    [Required(ErrorMessage = "RefreshToken is required")]
    public string RefreshToken { get; set; } = null!;
}

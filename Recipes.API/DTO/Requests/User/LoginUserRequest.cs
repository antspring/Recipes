using System.ComponentModel.DataAnnotations;
using Recipes.API.DTO.Requests.Attributes;

namespace Recipes.API.DTO.Requests.User;

public class LoginUserRequest
{
    [EitherRequired("Email", ErrorMessage = "Either UserName or Email must be provided")]
    public string? UserName { get; set; }

    [EitherRequired("UserName", ErrorMessage = "Either UserName or Email must be provided")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}
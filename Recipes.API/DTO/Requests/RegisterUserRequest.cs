using System.ComponentModel.DataAnnotations;
using Recipes.Application.DTO.User;

namespace Recipes.API.DTO.Requests;

public class RegisterUserRequest
{
    [Required(ErrorMessage = "UserName is required")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 50 characters")]
    public string UserName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Name must be between 1 and 100 characters")]
    public string Name { get; set; } = null!;

    [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "ConfirmPassword is required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; } = null!;

    public CreateUserDto ToCreateUserDto()
    {

        return new CreateUserDto
        {
            UserName = UserName,
            Email = Email,
            Name = Name,
            Description = Description,
            Password = Password
        };
    }
}
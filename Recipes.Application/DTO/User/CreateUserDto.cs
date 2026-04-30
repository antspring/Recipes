using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.DTO.User;

public class CreateUserDto
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ImageUpload? Avatar { get; set; }
    public string Password { get; set; } = null!;
    public string EmailVerificationCode { get; set; } = null!;
}

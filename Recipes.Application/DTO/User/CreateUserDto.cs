namespace Recipes.Application.DTO.User;

public class CreateUserDto
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public byte[]? Avatar { get; set; }
    public string Password { get; set; } = null!;
}
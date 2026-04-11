using Recipes.Application.DTO.User;

namespace Recipes.API.DTO.Requests;

public class RegisterUserRequest
{
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Password { get; set; } = null!;

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
using Recipes.Application.DTO.User;

namespace Recipes.API.DTO.Requests;

public class LoginUserRequest
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = null!;

    public LoginUserDto ToLoginUserDto(string? userAgent = null)
    {
        return new LoginUserDto
        {
            UserName = UserName,
            Email = Email,
            Password = Password,
            UserAgent = userAgent
        };
    }
}
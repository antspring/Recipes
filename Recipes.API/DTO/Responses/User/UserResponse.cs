namespace Recipes.API.DTO.Responses.User;

public class UserResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public UserResponse(Recipes.Domain.Models.User user, string accessToken = "", string refreshToken = "")
    {
        UserName = user.UserName;
        Email = user.Email;
        Name = user.Name;
        Description = user.Description;
        AvatarUrl = user.AvatarUrl;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public UserResponse(Recipes.Application.DTO.User.UserAuthDto userAuthDto)
    {
        UserName = userAuthDto.UserName;
        Email = userAuthDto.Email;
        Name = userAuthDto.Name;
        Description = userAuthDto.Description;
        AvatarUrl = userAuthDto.AvatarUrl;
        AccessToken = userAuthDto.AccessToken;
        RefreshToken = userAuthDto.RefreshToken;
    }
}

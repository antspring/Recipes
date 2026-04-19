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
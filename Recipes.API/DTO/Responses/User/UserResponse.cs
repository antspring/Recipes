using Recipes.Application.Services.Interfaces;

namespace Recipes.API.DTO.Responses.User;

public class UserResponse
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public UserResponse(
        Recipes.Domain.Models.User user,
        string accessToken = "",
        string refreshToken = "",
        IImageUrlMapper? imageUrlMapper = null)
    {
        UserName = user.UserName;
        Email = user.Email;
        Name = user.Name;
        Description = user.Description;
        AvatarUrl = imageUrlMapper == null
            ? user.AvatarUrl
            : imageUrlMapper.ToImageUrl(user.AvatarUrl);
        Role = user.Role.ToString();
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public UserResponse(Recipes.Application.DTO.User.UserAuthDto userAuthDto, IImageUrlMapper imageUrlMapper)
    {
        UserName = userAuthDto.UserName;
        Email = userAuthDto.Email;
        Name = userAuthDto.Name;
        Description = userAuthDto.Description;
        AvatarUrl = imageUrlMapper.ToImageUrl(userAuthDto.AvatarUrl);
        Role = userAuthDto.Role;
        AccessToken = userAuthDto.AccessToken;
        RefreshToken = userAuthDto.RefreshToken;
    }

}

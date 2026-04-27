using Recipes.Application.Services.Interfaces;

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

    public UserResponse(
        Recipes.Domain.Models.User user,
        string accessToken = "",
        string refreshToken = "",
        IImageUrlProvider? imageUrlProvider = null)
    {
        UserName = user.UserName;
        Email = user.Email;
        Name = user.Name;
        Description = user.Description;
        AvatarUrl = imageUrlProvider == null
            ? user.AvatarUrl
            : ToFullAvatarUrl(user.AvatarUrl, imageUrlProvider);
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public UserResponse(Recipes.Application.DTO.User.UserAuthDto userAuthDto, IImageUrlProvider imageUrlProvider)
    {
        UserName = userAuthDto.UserName;
        Email = userAuthDto.Email;
        Name = userAuthDto.Name;
        Description = userAuthDto.Description;
        AvatarUrl = ToFullAvatarUrl(userAuthDto.AvatarUrl, imageUrlProvider);
        AccessToken = userAuthDto.AccessToken;
        RefreshToken = userAuthDto.RefreshToken;
    }

    private static string? ToFullAvatarUrl(string? fileName, IImageUrlProvider imageUrlProvider)
    {
        return string.IsNullOrWhiteSpace(fileName)
            ? null
            : imageUrlProvider.GetImageUrl(fileName);
    }
}

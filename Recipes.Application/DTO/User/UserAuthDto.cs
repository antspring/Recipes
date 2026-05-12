namespace Recipes.Application.DTO.User;

public class UserAuthDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? AvatarUrl { get; set; }
    public string Role { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }

    public UserAuthDto(Recipes.Domain.Models.User user, string accessToken, string refreshToken)
    {
        UserId = user.Id;
        UserName = user.UserName;
        Email = user.Email;
        Name = user.Name;
        Description = user.Description;
        AvatarUrl = user.AvatarUrl;
        Role = user.Role.ToString();
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}
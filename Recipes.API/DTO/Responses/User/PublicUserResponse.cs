using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.DTO.Responses.User;

public class PublicUserResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? AvatarUrl { get; init; }
    public int FollowersCount { get; init; }
    public int FollowingCount { get; init; }
    public bool IsSubscribedByCurrentUser { get; init; }

    public PublicUserResponse(PublicUserDto user, IImageUrlProvider imageUrlProvider)
    {
        Id = user.Id;
        UserName = user.UserName;
        Name = user.Name;
        Description = user.Description;
        AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)
            ? null
            : imageUrlProvider.GetImageUrl(user.AvatarUrl);
        FollowersCount = user.FollowersCount;
        FollowingCount = user.FollowingCount;
        IsSubscribedByCurrentUser = user.IsSubscribedByCurrentUser;
    }
}

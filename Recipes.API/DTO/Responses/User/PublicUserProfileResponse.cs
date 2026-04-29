using Recipes.Application.DTO.Recipe;
using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.DTO.Responses.User;

public class PublicUserProfileResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public string? AvatarUrl { get; init; }
    public int FollowersCount { get; init; }
    public int FollowingCount { get; init; }
    public bool IsSubscribedByCurrentUser { get; init; }
    public List<RecipeDto> Recipes { get; init; } = new();

    public PublicUserProfileResponse(PublicUserProfileDto profile, IImageUrlProvider imageUrlProvider)
    {
        Id = profile.Id;
        UserName = profile.UserName;
        Name = profile.Name;
        Description = profile.Description;
        AvatarUrl = string.IsNullOrWhiteSpace(profile.AvatarUrl)
            ? null
            : imageUrlProvider.GetImageUrl(profile.AvatarUrl);
        FollowersCount = profile.FollowersCount;
        FollowingCount = profile.FollowingCount;
        IsSubscribedByCurrentUser = profile.IsSubscribedByCurrentUser;
        Recipes = profile.Recipes;
    }
}

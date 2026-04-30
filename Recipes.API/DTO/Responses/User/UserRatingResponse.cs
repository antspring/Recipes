using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;

namespace Recipes.API.DTO.Responses.User;

public class UserRatingResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = null!;
    public string? AvatarUrl { get; init; }
    public int RecipesCount { get; init; }
    public int FollowersCount { get; init; }
    public int Rating { get; init; }
    public bool IsSubscribedByCurrentUser { get; init; }

    public UserRatingResponse(UserRatingDto user, IImageUrlProvider imageUrlProvider)
    {
        Id = user.Id;
        UserName = user.UserName;
        AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarUrl)
            ? null
            : imageUrlProvider.GetImageUrl(user.AvatarUrl);
        RecipesCount = user.RecipesCount;
        FollowersCount = user.FollowersCount;
        Rating = user.Rating;
        IsSubscribedByCurrentUser = user.IsSubscribedByCurrentUser;
    }
}

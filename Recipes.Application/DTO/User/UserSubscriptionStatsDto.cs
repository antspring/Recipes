namespace Recipes.Application.DTO.User;

public class UserSubscriptionStatsDto
{
    public int FollowersCount { get; init; }
    public int FollowingCount { get; init; }
    public bool IsSubscribedByCurrentUser { get; init; }
}

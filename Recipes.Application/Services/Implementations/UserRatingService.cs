using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class UserRatingService(
    IUserRatingRepository userRatingRepository,
    IUserSubscriptionRepository userSubscriptionRepository) : IUserRatingService
{
    private const int TopUsersCount = 10;

    public async Task<List<UserRatingDto>> GetTopAsync(Guid? currentUserId)
    {
        var users = await userRatingRepository.GetTopAsync(TopUsersCount);
        if (!currentUserId.HasValue || users.Count == 0)
            return users;

        var userIds = users.Select(user => user.Id).ToArray();
        var subscribedToIds = await userSubscriptionRepository.GetSubscribedToIdsAsync(currentUserId.Value, userIds);

        users.ForEach(user => user.IsSubscribedByCurrentUser = subscribedToIds.Contains(user.Id));

        return users;
    }
}

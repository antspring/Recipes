using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Services.Implementations;

public class UserSubscriptionService(
    IUserRepository userRepository,
    IUserSubscriptionRepository userSubscriptionRepository,
    IUnitOfWork unitOfWork,
    IClock clock) : IUserSubscriptionService
{
    public async Task SetSubscriptionAsync(Guid currentUserId, Guid targetUserId, bool isSubscribed)
    {
        EnsureNotSelfSubscription(currentUserId, targetUserId);
        await EnsureUserExistsAsync(targetUserId);

        var existingSubscription = await userSubscriptionRepository.GetAsync(currentUserId, targetUserId);

        if (isSubscribed && existingSubscription == null)
        {
            await userSubscriptionRepository.AddAsync(new UserSubscription
            {
                SubscriberId = currentUserId,
                SubscribedToId = targetUserId,
                CreatedAt = clock.UtcNow
            });
        }

        if (!isSubscribed && existingSubscription != null)
        {
            await userSubscriptionRepository.RemoveAsync(existingSubscription);
        }

        await unitOfWork.SaveChangesAsync();
    }

    public async Task<List<PublicUserDto>> GetFollowersAsync(Guid targetUserId, Guid? currentUserId)
    {
        await EnsureUserExistsAsync(targetUserId);

        var users = await userSubscriptionRepository.GetFollowersAsync(targetUserId);
        return await BuildPublicUsersAsync(users, currentUserId);
    }

    public async Task<List<PublicUserDto>> GetFollowingAsync(Guid targetUserId, Guid? currentUserId)
    {
        await EnsureUserExistsAsync(targetUserId);

        var users = await userSubscriptionRepository.GetFollowingAsync(targetUserId);
        return await BuildPublicUsersAsync(users, currentUserId);
    }

    public async Task<UserSubscriptionStatsDto> GetStatsAsync(Guid targetUserId, Guid? currentUserId)
    {
        await EnsureUserExistsAsync(targetUserId);

        var followersCount = await userSubscriptionRepository.GetFollowersCountAsync(targetUserId);
        var followingCount = await userSubscriptionRepository.GetFollowingCountAsync(targetUserId);
        var isSubscribedByCurrentUser = currentUserId.HasValue
            && await userSubscriptionRepository.GetAsync(currentUserId.Value, targetUserId) != null;

        return new UserSubscriptionStatsDto
        {
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            IsSubscribedByCurrentUser = isSubscribedByCurrentUser
        };
    }

    private async Task<List<PublicUserDto>> BuildPublicUsersAsync(List<User> users, Guid? currentUserId)
    {
        if (users.Count == 0)
            return new List<PublicUserDto>();

        var userIds = users.Select(u => u.Id).ToArray();
        var followersCounts = await userSubscriptionRepository.GetFollowersCountsAsync(userIds);
        var followingCounts = await userSubscriptionRepository.GetFollowingCountsAsync(userIds);
        var subscribedToIds = currentUserId.HasValue
            ? await userSubscriptionRepository.GetSubscribedToIdsAsync(currentUserId.Value, userIds)
            : new HashSet<Guid>();

        return users.Select(user => new PublicUserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Description = user.Description,
            AvatarUrl = user.AvatarUrl,
            FollowersCount = followersCounts.GetValueOrDefault(user.Id),
            FollowingCount = followingCounts.GetValueOrDefault(user.Id),
            IsSubscribedByCurrentUser = subscribedToIds.Contains(user.Id)
        }).ToList();
    }

    private async Task EnsureUserExistsAsync(Guid userId)
    {
        if (await userRepository.GetByIdAsync(userId) == null)
            throw new ArgumentException("User not found");
    }

    private static void EnsureNotSelfSubscription(Guid currentUserId, Guid targetUserId)
    {
        if (currentUserId == targetUserId)
            throw new InvalidOperationException("Cannot subscribe to yourself");
    }
}

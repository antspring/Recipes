using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class UserSubscriptionRepository(BaseDbContext context) : IUserSubscriptionRepository
{
    public Task<UserSubscription?> GetAsync(Guid subscriberId, Guid subscribedToId)
    {
        return context.UserSubscriptions.FirstOrDefaultAsync(us =>
            us.SubscriberId == subscriberId && us.SubscribedToId == subscribedToId);
    }

    public Task AddAsync(UserSubscription subscription)
    {
        return context.UserSubscriptions.AddAsync(subscription).AsTask();
    }

    public Task RemoveAsync(UserSubscription subscription)
    {
        context.UserSubscriptions.Remove(subscription);
        return Task.CompletedTask;
    }

    public Task<List<User>> GetFollowersAsync(Guid userId)
    {
        return context.UserSubscriptions
            .Where(us => us.SubscribedToId == userId)
            .OrderByDescending(us => us.CreatedAt)
            .Select(us => us.Subscriber)
            .ToListAsync();
    }

    public Task<List<User>> GetFollowingAsync(Guid userId)
    {
        return context.UserSubscriptions
            .Where(us => us.SubscriberId == userId)
            .OrderByDescending(us => us.CreatedAt)
            .Select(us => us.SubscribedTo)
            .ToListAsync();
    }

    public Task<int> GetFollowersCountAsync(Guid userId)
    {
        return context.UserSubscriptions.CountAsync(us => us.SubscribedToId == userId);
    }

    public Task<int> GetFollowingCountAsync(Guid userId)
    {
        return context.UserSubscriptions.CountAsync(us => us.SubscriberId == userId);
    }

    public Task<Dictionary<Guid, int>> GetFollowersCountsAsync(IReadOnlyCollection<Guid> userIds)
    {
        return context.UserSubscriptions
            .Where(us => userIds.Contains(us.SubscribedToId))
            .GroupBy(us => us.SubscribedToId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public Task<Dictionary<Guid, int>> GetFollowingCountsAsync(IReadOnlyCollection<Guid> userIds)
    {
        return context.UserSubscriptions
            .Where(us => userIds.Contains(us.SubscriberId))
            .GroupBy(us => us.SubscriberId)
            .ToDictionaryAsync(g => g.Key, g => g.Count());
    }

    public async Task<HashSet<Guid>> GetSubscribedToIdsAsync(Guid subscriberId, IReadOnlyCollection<Guid> userIds)
    {
        var subscribedToIds = await context.UserSubscriptions
            .Where(us => us.SubscriberId == subscriberId && userIds.Contains(us.SubscribedToId))
            .Select(us => us.SubscribedToId)
            .ToListAsync();

        return subscribedToIds.ToHashSet();
    }
}

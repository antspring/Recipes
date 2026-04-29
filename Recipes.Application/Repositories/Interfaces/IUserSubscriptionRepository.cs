using Recipes.Domain.Models;
using Recipes.Domain.Models.UserRelations;

namespace Recipes.Application.Repositories.Interfaces;

public interface IUserSubscriptionRepository
{
    Task<UserSubscription?> GetAsync(Guid subscriberId, Guid subscribedToId);
    Task AddAsync(UserSubscription subscription);
    Task RemoveAsync(UserSubscription subscription);
    Task<List<User>> GetFollowersAsync(Guid userId);
    Task<List<User>> GetFollowingAsync(Guid userId);
    Task<int> GetFollowersCountAsync(Guid userId);
    Task<int> GetFollowingCountAsync(Guid userId);
    Task<Dictionary<Guid, int>> GetFollowersCountsAsync(IReadOnlyCollection<Guid> userIds);
    Task<Dictionary<Guid, int>> GetFollowingCountsAsync(IReadOnlyCollection<Guid> userIds);
    Task<HashSet<Guid>> GetSubscribedToIdsAsync(Guid subscriberId, IReadOnlyCollection<Guid> userIds);
}

using Recipes.Application.DTO.User;

namespace Recipes.Application.Services.Interfaces;

public interface IUserSubscriptionService
{
    Task SetSubscriptionAsync(Guid currentUserId, Guid targetUserId, bool isSubscribed);
    Task<List<PublicUserDto>> GetFollowersAsync(Guid targetUserId, Guid? currentUserId);
    Task<List<PublicUserDto>> GetFollowingAsync(Guid targetUserId, Guid? currentUserId);
    Task<UserSubscriptionStatsDto> GetStatsAsync(Guid targetUserId, Guid? currentUserId);
}

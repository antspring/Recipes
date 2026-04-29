using Recipes.Application.Common;
using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class UserPublicProfileService(
    IUserRepository userRepository,
    IUserSubscriptionRepository userSubscriptionRepository,
    IRecipeRepository recipeRepository,
    IRecipeDtoFactory recipeDtoFactory) : IUserPublicProfileService
{
    public async Task<PublicUserProfileDto> GetProfileAsync(Guid userId, Guid? currentUserId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        var followersCount = await userSubscriptionRepository.GetFollowersCountAsync(userId);
        var followingCount = await userSubscriptionRepository.GetFollowingCountAsync(userId);
        var isSubscribedByCurrentUser = currentUserId.HasValue
            && await userSubscriptionRepository.GetAsync(currentUserId.Value, userId) != null;
        var recipes = await recipeRepository.GetByCreatorIdAsync(userId, RecipeIncludes.Full);
        var recipeDtos = await recipeDtoFactory.CreateManyAsync(recipes);

        return new PublicUserProfileDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Name = user.Name,
            Description = user.Description,
            AvatarUrl = user.AvatarUrl,
            FollowersCount = followersCount,
            FollowingCount = followingCount,
            IsSubscribedByCurrentUser = isSubscribedByCurrentUser,
            Recipes = recipeDtos
        };
    }
}

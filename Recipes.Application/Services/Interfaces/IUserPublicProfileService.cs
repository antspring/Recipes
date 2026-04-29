using Recipes.Application.DTO.User;

namespace Recipes.Application.Services.Interfaces;

public interface IUserPublicProfileService
{
    Task<PublicUserProfileDto> GetProfileAsync(Guid userId, Guid? currentUserId);
}

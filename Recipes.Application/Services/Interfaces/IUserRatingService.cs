using Recipes.Application.DTO.User;

namespace Recipes.Application.Services.Interfaces;

public interface IUserRatingService
{
    Task<List<UserRatingDto>> GetTopAsync(Guid? currentUserId);
}

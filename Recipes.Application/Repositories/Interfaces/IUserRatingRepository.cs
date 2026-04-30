using Recipes.Application.DTO.User;

namespace Recipes.Application.Repositories.Interfaces;

public interface IUserRatingRepository
{
    Task<List<UserRatingDto>> GetTopAsync(int count);
}

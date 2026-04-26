using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IUserProfileService
{
    Task<User> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
    Task<User> DeleteAvatarAsync(Guid userId);
}

using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IUserAccessService
{
    Task<User> AuthenticateAsync(LoginUserDto loginUserDto);
    Task<User> GetRequiredUserAsync(Guid userId);
}

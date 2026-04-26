using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IUserRegistrationService
{
    Task<User> RegisterAsync(CreateUserDto createUserDto);
}

using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserAccessService(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher) : IUserAccessService
{
    public async Task<User> AuthenticateAsync(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrWhiteSpace(loginUserDto.UserName) && string.IsNullOrWhiteSpace(loginUserDto.Email))
            throw new ArgumentException("Either UserName or Email must be provided");

        var user = await userRepository.GetByUserNameOrEmailAsync(loginUserDto.UserName, loginUserDto.Email);
        if (user == null)
            throw new ArgumentException("Invalid username or email");

        var isPasswordValid = passwordHasher.Verify(loginUserDto.Password, user.Password);
        if (!isPasswordValid)
            throw new ArgumentException("Invalid password");

        return user;
    }

    public async Task<User> GetRequiredUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        return user;
    }
}

using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserAccessService(IUnitOfWork unitOfWork) : IUserAccessService
{
    public async Task<User> AuthenticateAsync(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrWhiteSpace(loginUserDto.UserName) && string.IsNullOrWhiteSpace(loginUserDto.Email))
            throw new ArgumentException("Either UserName or Email must be provided");

        var user = await unitOfWork.Users.GetByUserNameOrEmailAsync(loginUserDto.UserName, loginUserDto.Email);
        if (user == null)
            throw new ArgumentException("Invalid username or email");

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password);
        if (!isPasswordValid)
            throw new ArgumentException("Invalid password");

        return user;
    }

    public async Task<User> GetRequiredUserAsync(Guid userId)
    {
        var user = await unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            throw new ArgumentException("User not found");

        return user;
    }
}

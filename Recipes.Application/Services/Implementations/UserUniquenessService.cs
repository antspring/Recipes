using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class UserUniquenessService(IUserRepository userRepository) : IUserUniquenessService
{
    public async Task EnsureUserNameOrEmailAvailableAsync(string? userName, string? email, Guid? currentUserId = null)
    {
        if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(email))
            return;

        var existingUser = await userRepository.GetByUserNameOrEmailAsync(userName, email);
        if (existingUser != null && existingUser.Id != currentUserId)
            throw new ArgumentException("User with this email or username already exists");
    }
}

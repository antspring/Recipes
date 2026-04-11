using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IUserRepository
{
    public Task<User> CreateAsync(User user);
    public Task<User?> GetByUserNameOrEmailAsync(string? userName, string? email);
}
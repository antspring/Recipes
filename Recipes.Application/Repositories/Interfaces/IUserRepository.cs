using Recipes.Domain.Models;

namespace Recipes.Application.Repositories.Interfaces;

public interface IUserRepository
{
    public Task<User?> GetAsync(string username);
    public Task<User> CreateAsync(User user);
}
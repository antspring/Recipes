using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class UserRepository(BaseDbContext dbContext) : IUserRepository
{
    public Task CreateAsync(User user)
    {
        return dbContext.Users.AddAsync(user).AsTask();
    }

    public Task<User?> GetByUserNameOrEmailAsync(string? userName, string? email)
    {
        return dbContext.Users
            .Where(u => (userName != null && u.UserName == userName) || (email != null && u.Email == email))
            .FirstOrDefaultAsync();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task UpdateAsync(User user)
    {
        dbContext.Users.Update(user);
        return Task.CompletedTask;
    }
}

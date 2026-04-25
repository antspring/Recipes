using Microsoft.EntityFrameworkCore;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly BaseDbContext _dbContext;

    public UserRepository(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> CreateAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);

        return user;
    }

    public Task<User?> GetByUserNameOrEmailAsync(string? userName, string? email)
    {
        return _dbContext.Users
            .Where(u => (userName != null && u.UserName == userName) || (email != null && u.Email == email))
            .FirstOrDefaultAsync();
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        return _dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public Task<User> UpdateAsync(User user)
    {
        _dbContext.Users.Update(user);
        return Task.FromResult(user);
    }
}
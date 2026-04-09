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

    public Task<User?> GetAsync(string username)
    {
        return _dbContext.Users.Where(u => u.UserName == username).FirstOrDefaultAsync();
    }

    public async Task<User> CreateAsync(User user)
    {
        await _dbContext.Users.AddAsync(user);

        return user;
    }
}
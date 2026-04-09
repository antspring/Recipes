using Microsoft.EntityFrameworkCore;
using Recipes.Application.Auth;
using Recipes.Application.Repositories.Interfaces;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly BaseDbContext _dbContext;

    public RefreshTokenRepository(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<RefreshToken?> GetAsync(string refreshToken)
    {
        return _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
    }

    public async Task AddAsync(RefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(refreshToken);
    }

    public Task RemoveAsync(Guid id)
    {
        return _dbContext.RefreshTokens.Where(r => r.Id == id).ExecuteDeleteAsync();
    }
}
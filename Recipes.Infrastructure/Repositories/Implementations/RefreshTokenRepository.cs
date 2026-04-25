using Microsoft.EntityFrameworkCore;
using Recipes.Application.Auth;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Infrastructure.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly BaseDbContext _dbContext;

    public RefreshTokenRepository(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StoredRefreshToken?> GetAsync(string refreshToken)
    {
        var entity = await _dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        return entity == null
            ? null
            : new StoredRefreshToken(entity.Id, entity.UserId, entity.Token, entity.ExpiresAt, entity.UserAgent);
    }

    public async Task CreateAsync(GeneratedRefreshToken refreshToken)
    {
        await _dbContext.RefreshTokens.AddAsync(new RefreshToken(
            refreshToken.UserId,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            refreshToken.UserAgent));
    }

    public Task RemoveAsync(Guid id)
    {
        return _dbContext.RefreshTokens.Where(r => r.Id == id).ExecuteDeleteAsync();
    }
}

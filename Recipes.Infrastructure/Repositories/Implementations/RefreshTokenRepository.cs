using Microsoft.EntityFrameworkCore;
using Recipes.Application.Auth;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Infrastructure.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class RefreshTokenRepository(BaseDbContext dbContext) : IRefreshTokenRepository
{
    public async Task<StoredRefreshToken?> GetAsync(string refreshToken)
    {
        var entity = await dbContext.RefreshTokens.FirstOrDefaultAsync(r => r.Token == refreshToken);
        return entity == null
            ? null
            : new StoredRefreshToken(entity.Id, entity.UserId, entity.Token, entity.ExpiresAt, entity.UserAgent);
    }

    public async Task CreateAsync(GeneratedRefreshToken refreshToken)
    {
        await dbContext.RefreshTokens.AddAsync(new RefreshToken(
            refreshToken.UserId,
            refreshToken.Token,
            refreshToken.ExpiresAt,
            refreshToken.UserAgent));
    }

    public Task RemoveAsync(Guid id)
    {
        return dbContext.RefreshTokens.Where(r => r.Id == id).ExecuteDeleteAsync();
    }
}

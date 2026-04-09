using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Infrastructure.UnitOfWork.Implementations;

public class UnitOfWork : IUnitOfWork
{
    private readonly BaseDbContext _dbContext;

    public UnitOfWork(
        BaseDbContext dbContext,
        IRefreshTokenRepository refreshTokens,
        IUserRepository users)
    {
        _dbContext = dbContext;
        RefreshTokens = refreshTokens;
        Users = users;
    }

    public IRefreshTokenRepository RefreshTokens { get; }
    public IUserRepository Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
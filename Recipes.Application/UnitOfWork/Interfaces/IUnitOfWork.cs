using Recipes.Application.Repositories.Interfaces;

namespace Recipes.Application.UnitOfWork.Interfaces;

public interface IUnitOfWork
{
    public IRefreshTokenRepository RefreshTokens { get; }
    public IUserRepository Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
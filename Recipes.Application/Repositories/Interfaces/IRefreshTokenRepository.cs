using Recipes.Application.Auth;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    public Task<StoredRefreshToken?> GetAsync(string refreshToken);
    public Task CreateAsync(GeneratedRefreshToken refreshToken);
    public Task RemoveAsync(Guid id);
}

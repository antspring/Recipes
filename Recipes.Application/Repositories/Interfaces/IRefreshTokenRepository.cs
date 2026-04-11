using Recipes.Application.Auth;

namespace Recipes.Application.Repositories.Interfaces;

public interface IRefreshTokenRepository
{
    public Task<RefreshToken?> GetAsync(string refreshToken);
    public Task CreateAsync(RefreshToken refreshToken);
    public Task RemoveAsync(Guid id);
}
using Recipes.Application.Auth;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class RefreshTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IClock clock) : IRefreshTokenService
{
    public async Task<StoredRefreshToken> GetValidTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await refreshTokenRepository.GetAsync(refreshToken);
        if (storedRefreshToken == null)
            throw new ArgumentException("Invalid refresh token");

        if (storedRefreshToken.ExpiresAt < clock.UtcNow)
            throw new ArgumentException("Refresh token expired");

        return storedRefreshToken;
    }

    public Task RevokeAsync(Guid tokenId)
    {
        return refreshTokenRepository.RemoveAsync(tokenId);
    }
}

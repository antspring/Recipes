using Recipes.Application.Auth;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class RefreshTokenService(IUnitOfWork unitOfWork) : IRefreshTokenService
{
    public async Task<StoredRefreshToken> GetValidTokenAsync(string refreshToken)
    {
        var storedRefreshToken = await unitOfWork.RefreshTokens.GetAsync(refreshToken);
        if (storedRefreshToken == null)
            throw new ArgumentException("Invalid refresh token");

        if (storedRefreshToken.ExpiresAt < DateTime.Now)
            throw new ArgumentException("Refresh token expired");

        return storedRefreshToken;
    }

    public Task RevokeAsync(Guid tokenId)
    {
        return unitOfWork.RefreshTokens.RemoveAsync(tokenId);
    }
}

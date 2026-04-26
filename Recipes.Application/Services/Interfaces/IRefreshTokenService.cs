using Recipes.Application.Auth;

namespace Recipes.Application.Services.Interfaces;

public interface IRefreshTokenService
{
    Task<StoredRefreshToken> GetValidTokenAsync(string refreshToken);
    Task RevokeAsync(Guid tokenId);
}

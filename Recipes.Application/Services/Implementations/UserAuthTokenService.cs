using Recipes.Application.DTO.User;
using Recipes.Application.Providers;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserAuthTokenService(
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork,
    IJwtGenerateService jwtGenerateService,
    IClaimsProvider claimsProvider) : IUserAuthTokenService
{
    public async Task<UserAuthDto> IssueTokensAsync(User user, string? userAgent)
    {
        var claims = claimsProvider.GetClaims(user);
        var accessToken = jwtGenerateService.GenerateAccessToken(claims);
        var refreshToken = jwtGenerateService.GenerateRefreshToken(user.Id, userAgent);

        await refreshTokenRepository.CreateAsync(refreshToken);
        await unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, accessToken, refreshToken.Token);
    }
}

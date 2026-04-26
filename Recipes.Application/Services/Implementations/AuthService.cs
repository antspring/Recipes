using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class AuthService(
    IUserAccessService userAccessService,
    IUserRegistrationService userRegistrationService,
    IUserProfileService userProfileService,
    IRefreshTokenService refreshTokenService,
    IUserAuthTokenService userAuthTokenService) : IAuthService
{
    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent)
    {
        var user = await userRegistrationService.RegisterAsync(createUserDto);
        return await userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> Login(LoginUserDto loginUserDto)
    {
        var user = await userAccessService.AuthenticateAsync(loginUserDto);
        return await userAuthTokenService.IssueTokensAsync(user, loginUserDto.UserAgent);
    }

    public async Task<UserAuthDto> UpdateToken(string refreshToken, string? userAgent)
    {
        var storedRefreshToken = await refreshTokenService.GetValidTokenAsync(refreshToken);
        await refreshTokenService.RevokeAsync(storedRefreshToken.Id);

        var user = await userAccessService.GetRequiredUserAsync(storedRefreshToken.UserId);
        return await userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto, string? userAgent)
    {
        var user = await userProfileService.UpdateUserAsync(userId, updateUserDto);
        return await userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> DeleteAvatarAsync(Guid userId)
    {
        var user = await userProfileService.DeleteAvatarAsync(userId);
        return new UserAuthDto(user, "", "");
    }
}

using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserAccessService _userAccessService;
    private readonly IUserRegistrationService _userRegistrationService;
    private readonly IUserProfileService _userProfileService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserAuthTokenService _userAuthTokenService;

    public AuthService(
        IUserAccessService userAccessService,
        IUserRegistrationService userRegistrationService,
        IUserProfileService userProfileService,
        IRefreshTokenService refreshTokenService,
        IUserAuthTokenService userAuthTokenService)
    {
        _userAccessService = userAccessService;
        _userRegistrationService = userRegistrationService;
        _userProfileService = userProfileService;
        _refreshTokenService = refreshTokenService;
        _userAuthTokenService = userAuthTokenService;
    }

    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent)
    {
        var user = await _userRegistrationService.RegisterAsync(createUserDto);
        return await _userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> Login(LoginUserDto loginUserDto)
    {
        var user = await _userAccessService.AuthenticateAsync(loginUserDto);
        return await _userAuthTokenService.IssueTokensAsync(user, loginUserDto.UserAgent);
    }

    public async Task<UserAuthDto> UpdateToken(string refreshToken, string? userAgent)
    {
        var storedRefreshToken = await _refreshTokenService.GetValidTokenAsync(refreshToken);
        await _refreshTokenService.RevokeAsync(storedRefreshToken.Id);

        var user = await _userAccessService.GetRequiredUserAsync(storedRefreshToken.UserId);
        return await _userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto, string? userAgent)
    {
        var user = await _userProfileService.UpdateUserAsync(userId, updateUserDto);
        return await _userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> DeleteAvatarAsync(Guid userId)
    {
        var user = await _userProfileService.DeleteAvatarAsync(userId);
        return new UserAuthDto(user, "", "");
    }
}

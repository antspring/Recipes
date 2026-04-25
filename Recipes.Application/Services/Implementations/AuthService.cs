using AutoMapper;
using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserAccessService _userAccessService;
    private readonly IUserAvatarService _userAvatarService;
    private readonly IUserAuthTokenService _userAuthTokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserAccessService userAccessService,
        IUserAvatarService userAvatarService,
        IUserAuthTokenService userAuthTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userAccessService = userAccessService;
        _userAvatarService = userAvatarService;
        _userAuthTokenService = userAuthTokenService;
    }

    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent)
    {
        var user = _mapper.Map<User>(createUserDto);
        user.Password = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        user.AvatarUrl = await _userAvatarService.UploadAvatarAsync(createUserDto.Avatar);

        user = await _unitOfWork.Users.CreateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return await _userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> Login(LoginUserDto loginUserDto)
    {
        var user = await _userAccessService.AuthenticateAsync(loginUserDto);
        return await _userAuthTokenService.IssueTokensAsync(user, loginUserDto.UserAgent);
    }

    public async Task<UserAuthDto> UpdateToken(string refreshToken, string? userAgent)
    {
        var storedRefreshToken = await _unitOfWork.RefreshTokens.GetAsync(refreshToken);
        if (storedRefreshToken == null)
        {
            throw new ArgumentException("Invalid refresh token");
        }

        if (storedRefreshToken.ExpiresAt < DateTime.Now)
        {
            throw new ArgumentException("Refresh token expired");
        }

        await _unitOfWork.RefreshTokens.RemoveAsync(storedRefreshToken.Id);

        var user = await _userAccessService.GetRequiredUserAsync(storedRefreshToken.UserId);
        return await _userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto, string? userAgent)
    {
        var user = await _userAccessService.GetRequiredUserAsync(userId);
        await _userAvatarService.DeleteAvatarAsync(user);
        user.AvatarUrl = await _userAvatarService.UploadAvatarAsync(updateUserDto.Avatar);

        _mapper.Map(updateUserDto, user);
        user.UpdatedAt = DateTime.Now.ToUniversalTime();

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return await _userAuthTokenService.IssueTokensAsync(user, userAgent);
    }

    public async Task<UserAuthDto> DeleteAvatarAsync(Guid userId)
    {
        var user = await _userAccessService.GetRequiredUserAsync(userId);
        await _userAvatarService.DeleteAvatarAsync(user);

        user.AvatarUrl = null;
        user.UpdatedAt = DateTime.Now.ToUniversalTime();

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, "", "");
    }
}

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
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserAccessService _userAccessService;
    private readonly IUserAvatarService _userAvatarService;
    private readonly IUserProfileService _userProfileService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserAuthTokenService _userAuthTokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IPasswordHasher passwordHasher,
        IUserAccessService userAccessService,
        IUserAvatarService userAvatarService,
        IUserProfileService userProfileService,
        IRefreshTokenService refreshTokenService,
        IUserAuthTokenService userAuthTokenService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _passwordHasher = passwordHasher;
        _userAccessService = userAccessService;
        _userAvatarService = userAvatarService;
        _userProfileService = userProfileService;
        _refreshTokenService = refreshTokenService;
        _userAuthTokenService = userAuthTokenService;
    }

    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent)
    {
        var user = await CreateUserAsync(createUserDto);
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

    private async Task<User> CreateUserAsync(CreateUserDto createUserDto)
    {
        var user = _mapper.Map<User>(createUserDto);
        user.Password = _passwordHasher.Hash(createUserDto.Password);
        user.AvatarUrl = await _userAvatarService.UploadAvatarAsync(createUserDto.Avatar);
        return await _unitOfWork.Users.CreateAsync(user);
    }
}

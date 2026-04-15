using AutoMapper;
using Recipes.Application.Auth;
using Recipes.Application.DTO.User;
using Recipes.Application.Providers;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IJwtGenerateService _jwtGenerateService;
    private readonly ClaimsProvider _claimsProvider;
    private readonly IImageStorageService _imageStorageService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IJwtGenerateService jwtGenerateService,
        ClaimsProvider claimsProvider,
        IImageStorageService imageStorageService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtGenerateService = jwtGenerateService;
        _claimsProvider = claimsProvider;
        _imageStorageService = imageStorageService;
    }

    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent)
    {
        var user = _mapper.Map<User>(createUserDto);

        if (createUserDto.Avatar != null && createUserDto.Avatar.Length > 0)
        {
            using var memoryStream = new MemoryStream(createUserDto.Avatar);
            var fileName = await _imageStorageService.UploadImageAsync(memoryStream, "avatar.jpg", "image/jpeg");
            user.AvatarUrl = _imageStorageService.GetImageUrl(fileName);
        }

        user = await _unitOfWork.Users.CreateAsync(user);

        var (accessToken, refreshToken) = GetTokens(user, userAgent);
        await _unitOfWork.RefreshTokens.CreateAsync(refreshToken);

        await _unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, accessToken, refreshToken.Token);
    }

    public async Task<UserAuthDto> Login(LoginUserDto loginUserDto)
    {
        if (string.IsNullOrWhiteSpace(loginUserDto.UserName) && string.IsNullOrWhiteSpace(loginUserDto.Email))
        {
            throw new ArgumentException("Either UserName or Email must be provided");
        }

        var user = await _unitOfWork.Users.GetByUserNameOrEmailAsync(loginUserDto.UserName, loginUserDto.Email);
        if (user is null)
        {
            throw new ArgumentException("Invalid username or email");
        }
        
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginUserDto.Password, user.Password);
        if (!isPasswordValid)
        {
            throw new ArgumentException("Invalid password");
        }

        var (accessToken, refreshToken) = GetTokens(user, loginUserDto.UserAgent);
        await _unitOfWork.RefreshTokens.CreateAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();
        
        return new UserAuthDto(user, accessToken, refreshToken.Token);
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

        var user = await _unitOfWork.Users.GetByIdAsync(storedRefreshToken.UserId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        var (accessToken, newRefreshToken) = GetTokens(user, userAgent);
        await _unitOfWork.RefreshTokens.CreateAsync(newRefreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, accessToken, newRefreshToken.Token);
    }

    public async Task<UserAuthDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto, string? userAgent)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var fileName = ExtractFileName(user.AvatarUrl);
            if (await _imageStorageService.ImageExistsAsync(fileName))
            {
                await _imageStorageService.DeleteImageAsync(fileName);
            }
        }

        if (updateUserDto.Avatar != null && updateUserDto.Avatar.Length > 0)
        {
            using var memoryStream = new MemoryStream(updateUserDto.Avatar);
            var fileName = await _imageStorageService.UploadImageAsync(memoryStream, "avatar.jpg", "image/jpeg");
            user.AvatarUrl = _imageStorageService.GetImageUrl(fileName);
        }

        _mapper.Map(updateUserDto, user);
        user.UpdatedAt = DateTime.Now.ToUniversalTime();

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var (accessToken, refreshToken) = GetTokens(user, userAgent);
        await _unitOfWork.RefreshTokens.CreateAsync(refreshToken);
        await _unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, accessToken, refreshToken.Token);
    }

    public async Task<UserAuthDto> DeleteAvatarAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        if (!string.IsNullOrEmpty(user.AvatarUrl))
        {
            var fileName = ExtractFileName(user.AvatarUrl);
            if (await _imageStorageService.ImageExistsAsync(fileName))
            {
                await _imageStorageService.DeleteImageAsync(fileName);
            }
        }

        user.AvatarUrl = null;
        user.UpdatedAt = DateTime.Now.ToUniversalTime();

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, "", "");
    }

    private (string, RefreshToken) GetTokens(User user, string? userAgent)
    {
        var claims = _claimsProvider.GetClaims(user);
        var accessToken = _jwtGenerateService.GenerateAccessToken(claims);
        var refreshToken = _jwtGenerateService.GenerateRefreshToken(user.Id, userAgent);
        return (accessToken, refreshToken);
    }

    private static string ExtractFileName(string avatarUrl)
    {
        var uri = new Uri(avatarUrl);
        return uri.Segments.Last();
    }
}
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

    public AuthService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IJwtGenerateService jwtGenerateService,
        ClaimsProvider claimsProvider)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _jwtGenerateService = jwtGenerateService;
        _claimsProvider = claimsProvider;
    }

    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent)
    {
        var user = _mapper.Map<User>(createUserDto);
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

        // Удаляем старый refresh token
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

    private (string, RefreshToken) GetTokens(User user, string? userAgent)
    {
        var claims = _claimsProvider.GetClaims(user);
        var accessToken = _jwtGenerateService.GenerateAccessToken(claims);
        var refreshToken = _jwtGenerateService.GenerateRefreshToken(user.Id, userAgent);
        return (accessToken, refreshToken);
    }
}
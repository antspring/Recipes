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

    public async Task<UserAuthDto> Register(CreateUserDto createUserDto, string userAgent)
    {
        var user = _mapper.Map<User>(createUserDto);
        user = await _unitOfWork.Users.CreateAsync(user);

        var (accessToken, refreshToken) = GetTokens(user, userAgent);
        await _unitOfWork.RefreshTokens.CreateAsync(refreshToken);

        await _unitOfWork.SaveChangesAsync();

        return new UserAuthDto(user, accessToken, refreshToken.Token);
    }

    public Task<UserAuthDto> Login(string emailOrUserName, string password)
    {
        throw new NotImplementedException();
    }

    public string UpdateToken(string token)
    {
        throw new NotImplementedException();
    }

    private (string, RefreshToken) GetTokens(User user, string userAgent)
    {
        var claims = _claimsProvider.GetClaims(user);
        var accessToken = _jwtGenerateService.GenerateAccessToken(claims);
        var refreshToken = _jwtGenerateService.GenerateRefreshToken(user.Id, userAgent);
        return (accessToken, refreshToken);
    }
}
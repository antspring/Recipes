using Recipes.Application.DTO.User;

namespace Recipes.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<UserAuthDto> Register(CreateUserDto createUserDto, string userAgent);
    public Task<UserAuthDto> Login(LoginUserDto loginUserDto);
    public string UpdateToken(string token);
}
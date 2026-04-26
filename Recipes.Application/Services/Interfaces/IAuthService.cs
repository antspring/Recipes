using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IAuthService
{
    public Task<UserAuthDto> Register(CreateUserDto createUserDto, string? userAgent);
    public Task<UserAuthDto> Login(LoginUserDto loginUserDto);
    public Task<UserAuthDto> UpdateToken(string refreshToken, string? userAgent);
    public Task<UserAuthDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto, string? userAgent);
    public Task<User> DeleteAvatarAsync(Guid userId);
}

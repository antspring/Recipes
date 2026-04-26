using Recipes.Application.DTO.User;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IUserAuthTokenService
{
    Task<UserAuthDto> IssueTokensAsync(User user, string? userAgent);
}

using System.Security.Claims;
using Recipes.Application.Auth;

namespace Recipes.Application.Services.Interfaces;

public interface IJwtGenerateService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims);
    public GeneratedRefreshToken GenerateRefreshToken(Guid userId, string? userAgent);
}

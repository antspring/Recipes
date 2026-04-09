using System.Security.Claims;

namespace Recipes.Application.Services.Interfaces;

public interface IJwtGenerateService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims);
    public string GenerateRefreshToken();
}
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Recipes.Application.Auth;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Infrastructure.Services;

public class JwtGenerateService(
    IJwtOptions jwtOptions,
    IClock clock) : IJwtGenerateService
{
    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = jwtOptions.Issuer,
            Audience = jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = clock.UtcNow,
            Expires = clock.UtcNow.AddMinutes(jwtOptions.AccessExpirationMinutes),
            SigningCredentials = CreateSigningCredentials()
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public GeneratedRefreshToken GenerateRefreshToken(Guid userId, string? userAgent)
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);
        var token = Convert.ToBase64String(randomNumber);

        return new GeneratedRefreshToken(
            userId,
            token,
            clock.UtcNow.AddDays(jwtOptions.RefreshExpirationDays),
            userAgent);
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var key = Encoding.UTF8.GetBytes(jwtOptions.Key);
        var securityKey = new SymmetricSecurityKey(key);

        return new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256Signature);
    }
}

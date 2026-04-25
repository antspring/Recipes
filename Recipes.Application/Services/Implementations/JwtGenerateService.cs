using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Recipes.Application.Auth;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class JwtGenerateService : IJwtGenerateService
{
    private readonly IJwtOptions _jwtOptions;

    public JwtGenerateService(IJwtOptions jwtOptions)
    {
        _jwtOptions = jwtOptions;
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);
        var securityKey = new SymmetricSecurityKey(key);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.Now,
            Expires = DateTime.Now.AddMinutes(_jwtOptions.AccessExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256Signature)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public GeneratedRefreshToken GenerateRefreshToken(Guid userId, string? userAgent)
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);
        var toke = Convert.ToBase64String(randomNumber);

        return new GeneratedRefreshToken(
            userId,
            toke,
            DateTime.Now.AddDays(_jwtOptions.RefreshExpirationDays).ToUniversalTime(),
            userAgent);
    }
}

using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Recipes.Application.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using Recipes.Application.Options.Interfaces;

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
            NotBefore = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256Signature)
        };

        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rnd = RandomNumberGenerator.Create();
        rnd.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
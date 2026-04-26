using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Recipes.API.ServiceCollectionExtension;

public static class JwtAuthenticationServiceCollectionExtension
{
    public static void AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSection = configuration.GetSection("Jwt");
        var issuer = GetRequiredJwtOption(jwtSection, "Issuer");
        var audience = GetRequiredJwtOption(jwtSection, "Audience");
        var key = GetRequiredJwtOption(jwtSection, "Key");

        services.AddAuthorization();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
        });
    }

    private static string GetRequiredJwtOption(IConfigurationSection jwtSection, string key)
    {
        return jwtSection[key] ?? throw new InvalidOperationException($"Jwt:{key} is not configured");
    }
}
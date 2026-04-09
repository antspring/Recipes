using Recipes.Application.Options.Interfaces;

namespace Recipes.Application.Options.Implementations;

public class JwtOptions : IJwtOptions
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int ExpirationMinutes { get; set; }
}
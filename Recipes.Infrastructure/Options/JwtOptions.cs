using Recipes.Application.Options.Interfaces;

namespace Recipes.Infrastructure.Options;

public class JwtOptions : IJwtOptions
{
    public string Key { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessExpirationMinutes { get; set; }
    public int RefreshExpirationDays { get; set; }
}

namespace Recipes.Application.Options.Interfaces;

public interface IJwtOptions
{
    string Key { get; }
    string Issuer { get; }
    string Audience { get; }
    int AccessExpirationMinutes { get; }
    int RefreshExpirationDays { get; }
}
namespace Recipes.Application.Auth;

public sealed record GeneratedRefreshToken(
    Guid UserId,
    string Token,
    DateTime ExpiresAt,
    string? UserAgent);

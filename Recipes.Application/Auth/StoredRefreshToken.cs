namespace Recipes.Application.Auth;

public sealed record StoredRefreshToken(
    Guid Id,
    Guid UserId,
    string Token,
    DateTime ExpiresAt,
    string? UserAgent);

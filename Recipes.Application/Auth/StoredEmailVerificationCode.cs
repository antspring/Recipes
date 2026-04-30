namespace Recipes.Application.Auth;

public record StoredEmailVerificationCode(
    Guid Id,
    string Email,
    string CodeHash,
    DateTime ExpiresAt,
    DateTime CreatedAt);

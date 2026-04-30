namespace Recipes.Application.Auth;

public record GeneratedEmailVerificationCode(
    string Email,
    string CodeHash,
    DateTime ExpiresAt,
    DateTime CreatedAt);

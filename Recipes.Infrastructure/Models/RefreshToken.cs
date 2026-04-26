using System.ComponentModel.DataAnnotations;
using Recipes.Domain.Models;

namespace Recipes.Infrastructure.Models;

public class RefreshToken
{
    public RefreshToken(Guid userId, string token, DateTime expiresAt, string? userAgent)
    {
        UserId = userId;
        Token = token;
        ExpiresAt = expiresAt;
        UserAgent = userAgent;
    }

    [Key] public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Token { get; init; }
    public DateTime ExpiresAt { get; init; }
    public string? UserAgent { get; init; }

    public User User { get; init; } = null!;
}

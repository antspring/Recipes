using System.ComponentModel.DataAnnotations;

namespace Recipes.Infrastructure.Models;

public class EmailVerificationCode
{
    [Key] public Guid Id { get; init; }
    public string Email { get; init; } = null!;
    public string CodeHash { get; init; } = null!;
    public DateTime ExpiresAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

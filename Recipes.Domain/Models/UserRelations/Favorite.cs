using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Recipes.Domain.Models.UserRelations;

[PrimaryKey(nameof(RecipeId), nameof(UserId))]
public class Favorite
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }

    public DateTime CreatedAt { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public User User { get; init; } = null!;
}
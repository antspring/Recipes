using Microsoft.EntityFrameworkCore;

namespace Recipes.Domain.Models.RecipesRelations;

[PrimaryKey(nameof(RecipeId), nameof(UserId))]
public class Like
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }

    public DateTime CreatedAt { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public User User { get; init; } = null!;
}
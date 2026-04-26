namespace Recipes.Domain.Models.UserRelations;

public class Favorite
{
    public Guid RecipeId { get; init; }
    public Guid UserId { get; init; }

    public DateTime CreatedAt { get; init; }

    public Recipe Recipe { get; init; } = null!;
    public User User { get; init; } = null!;
}
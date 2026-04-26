namespace Recipes.Domain.Models;

public class Comment
{
    public Guid Id { get; init; }

    public string Value { get; set; } = null!;

    public Guid CommentatorId { get; init; }
    public User Commentator { get; init; } = null!;

    public Guid RecipeId { get; init; }
    public Recipe Recipe { get; init; } = null!;

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public ICollection<Image> Images { get; set; } = new List<Image>();
}
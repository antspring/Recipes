using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipes.Domain.Models;

public class Comment
{
    public Guid Id { get; init; }

    [StringLength(1000, MinimumLength = 3)]
    [Column(TypeName = "varchar(1000)")]
    public string Value { get; set; } = null!;

    public Guid CommentatorId { get; init; }
    public User Commentator { get; init; } = null!;

    public Guid RecipeId { get; init; }
    public Recipe Recipe { get; init; } = null!;

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}
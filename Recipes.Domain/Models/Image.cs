namespace Recipes.Domain.Models;

public class Image
{
    public Guid Id { get; init; }
    public string FileName { get; set; } = null!;
    public DateTime CreatedAt { get; init; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
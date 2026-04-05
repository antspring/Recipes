namespace Recipes.Domain.Models;

public class Image
{
    public Guid Id { get; init; }
    public string Url { get; set; } = null!;
    public DateTime CreatedAt { get; init; }
}
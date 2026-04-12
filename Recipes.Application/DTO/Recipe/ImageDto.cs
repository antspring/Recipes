namespace Recipes.Application.DTO.Recipe;

public class ImageDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string? Url { get; set; }
    public DateTime CreatedAt { get; set; }
}
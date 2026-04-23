namespace Recipes.Application.DTO.Comment;

public class CommentImageDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = null!;
    public string? Url { get; set; }
}
namespace Recipes.Application.DTO.Comment;

public class UpdateCommentDto
{
    public Guid CommentId { get; set; }
    public Guid CommentatorId { get; set; }
    public string Value { get; set; } = null!;
}
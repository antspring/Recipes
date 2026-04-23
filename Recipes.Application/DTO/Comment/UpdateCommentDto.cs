using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.DTO.Comment;

public class UpdateCommentDto
{
    public Guid Id { get; set; }
    public Guid CommentatorId { get; set; }
    public string? Value { get; set; }
    public List<Guid> ImageIdsToDelete { get; set; } = new();
    public List<ImageUpload> Images { get; set; } = new();
}
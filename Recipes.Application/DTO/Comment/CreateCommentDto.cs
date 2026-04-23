using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.DTO.Comment;

public class CreateCommentDto
{
    public Guid RecipeId { get; set; }
    public Guid CommentatorId { get; set; }
    public string Value { get; set; } = null!;
    public List<ImageUpload> Images { get; set; } = new();
}
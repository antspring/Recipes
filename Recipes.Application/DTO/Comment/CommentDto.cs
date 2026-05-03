namespace Recipes.Application.DTO.Comment;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Value { get; set; } = null!;
    public Guid CommentatorId { get; set; }
    public string CommentatorUserName { get; set; } = null!;
    public string? CommentatorAvatarUrl { get; set; }
    public Guid RecipeId { get; set; }
    public Guid? ParentCommentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CommentImageDto> Images { get; set; } = new();
    public List<CommentDto> Replies { get; set; } = new();

    public static CommentDto FromComment(Recipes.Domain.Models.Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Value = comment.Value,
            CommentatorId = comment.CommentatorId,
            CommentatorUserName = comment.Commentator.UserName,
            CommentatorAvatarUrl = comment.Commentator.AvatarUrl,
            RecipeId = comment.RecipeId,
            ParentCommentId = comment.ParentCommentId,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            Images = comment.Images.Select(i => new CommentImageDto
            {
                Id = i.Id,
                FileName = i.FileName,
                Url = null
            }).ToList()
        };
    }

}

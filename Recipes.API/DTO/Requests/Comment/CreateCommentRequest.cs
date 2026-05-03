using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Comment;

public class CreateCommentRequest
{
    [Required]
    public string Value { get; set; } = null!;

    public Guid? ParentCommentId { get; set; }

    public IFormFileCollection? Images { get; set; }
}
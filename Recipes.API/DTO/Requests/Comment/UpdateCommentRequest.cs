using System.ComponentModel.DataAnnotations;

namespace Recipes.API.DTO.Requests.Comment;

public class UpdateCommentRequest
{
    [Required(ErrorMessage = "Comment text is required")]
    [StringLength(1000, MinimumLength = 3, ErrorMessage = "Text length must be between 3 and 1000 characters")]
    public string Value { get; set; } = null!;
}
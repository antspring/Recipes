using Recipes.Application.DTO.Comment;

namespace Recipes.Application.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(Guid recipeId, Guid userId, string value);
    Task<PagedResult<CommentDto>> GetCommentsByRecipeIdAsync(Guid recipeId, int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null);
    Task<CommentDto> UpdateCommentAsync(Guid commentId, Guid userId, string value);
    Task DeleteCommentAsync(Guid commentId, Guid userId);
}
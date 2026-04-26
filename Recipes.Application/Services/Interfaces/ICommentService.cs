using Recipes.Application.Common;
using Recipes.Application.DTO.Comment;

namespace Recipes.Application.Services.Interfaces;

public interface ICommentService
{
    Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto);

    Task<PagedResult<CommentDto>> GetCommentsByRecipeIdAsync(Guid recipeId, int page = 1, int pageSize = 20,
        DateTime? from = null, DateTime? to = null);

    Task<CommentDto> UpdateCommentAsync(UpdateCommentDto updateCommentDto);

    Task DeleteCommentAsync(Guid commentId, Guid userId);
}
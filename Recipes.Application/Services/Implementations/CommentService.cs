using Microsoft.Extensions.Logging;
using Recipes.Application.DTO.Comment;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class CommentService(
    IUnitOfWork unitOfWork,
    ILogger<CommentService> logger) : ICommentService
{
    public async Task<CommentDto> CreateCommentAsync(Guid recipeId, Guid userId, string value)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(recipeId);
        if (recipe == null)
            throw new ArgumentException($"Recipe with id {recipeId} not found");

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            Value = value,
            CommentatorId = userId,
            RecipeId = recipeId,
            CreatedAt = DateTime.Now.ToUniversalTime(),
            UpdatedAt = DateTime.Now.ToUniversalTime()
        };

        await unitOfWork.Comments.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();

        var createdComment = await unitOfWork.Comments.GetByIdAsync(comment.Id);
        return CommentDto.FromComment(createdComment!);
    }

    public async Task<PagedResult<CommentDto>> GetCommentsByRecipeIdAsync(Guid recipeId, int page = 1,
        int pageSize = 20, DateTime? from = null, DateTime? to = null)
    {
        var pagedResult = await unitOfWork.Comments.GetByRecipeIdPagedAsync(recipeId, page, pageSize, from, to);
        var dtos = pagedResult.Items.Select(CommentDto.FromComment).ToList();

        return new PagedResult<CommentDto>
        {
            Items = dtos,
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<CommentDto> UpdateCommentAsync(Guid commentId, Guid userId, string value)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
            throw new ArgumentException($"Comment with id {commentId} not found");

        if (comment.CommentatorId != userId)
            throw new UnauthorizedAccessException("Only the author can update this comment");

        comment.Value = value;

        await unitOfWork.Comments.UpdateAsync(comment);
        await unitOfWork.SaveChangesAsync();

        var updatedComment = await unitOfWork.Comments.GetByIdAsync(commentId);
        return CommentDto.FromComment(updatedComment!);
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
            throw new ArgumentException($"Comment with id {commentId} not found");

        if (comment.CommentatorId != userId)
            throw new UnauthorizedAccessException("Only the author can delete this comment");

        await unitOfWork.Comments.DeleteAsync(comment);
        await unitOfWork.SaveChangesAsync();
    }
}
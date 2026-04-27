using Recipes.Application.Common;
using Recipes.Application.DTO.Comment;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class CommentService(
    IRecipeExistenceRepository recipeExistenceRepository,
    ICommentRepository commentRepository,
    IUnitOfWork unitOfWork,
    ICommentImageService commentImageService,
    IImageUrlProvider imageUrlProvider,
    IClock clock) : ICommentService
{
    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        if (!await recipeExistenceRepository.ExistsAsync(createCommentDto.RecipeId))
            throw new ArgumentException($"Recipe with id {createCommentDto.RecipeId} not found");

        var now = clock.UtcNow;
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            RecipeId = createCommentDto.RecipeId,
            CommentatorId = createCommentDto.CommentatorId,
            Value = createCommentDto.Value,
            CreatedAt = now,
            UpdatedAt = now
        };

        await commentImageService.AddImagesAsync(comment, createCommentDto.Images);

        await commentRepository.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredCommentDtoAsync(comment.Id);
    }

    public async Task<PagedResult<CommentDto>> GetCommentsByRecipeIdAsync(Guid recipeId, int page = 1,
        int pageSize = 20, DateTime? from = null, DateTime? to = null)
    {
        var pagedResult = await commentRepository.GetByRecipeIdPagedAsync(recipeId, page, pageSize, from, to);
        var dtos = ToCommentDtos(pagedResult.Items);

        return new PagedResult<CommentDto>
        {
            Items = dtos,
            TotalCount = pagedResult.TotalCount,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize
        };
    }

    public async Task<CommentDto> UpdateCommentAsync(UpdateCommentDto updateCommentDto)
    {
        var comment = await GetRequiredCommentAsync(updateCommentDto.Id);
        EnsureCommentAuthor(comment, updateCommentDto.CommentatorId, "update");

        if (updateCommentDto.Value is not null)
            comment.Value = updateCommentDto.Value;

        await commentImageService.DeleteImagesAsync(comment, updateCommentDto.ImageIdsToDelete);
        await commentImageService.AddImagesAsync(comment, updateCommentDto.Images);

        comment.UpdatedAt = clock.UtcNow;
        await commentRepository.UpdateAsync(comment);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredCommentDtoAsync(updateCommentDto.Id);
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await GetRequiredCommentAsync(commentId);
        EnsureCommentAuthor(comment, userId, "delete");

        await commentImageService.DeleteAllImagesAsync(comment);

        await commentRepository.DeleteAsync(comment);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task<Comment> GetRequiredCommentAsync(Guid commentId)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            throw new ArgumentException($"Comment with id {commentId} not found");

        return comment;
    }

    private static void EnsureCommentAuthor(Comment comment, Guid userId, string action)
    {
        if (comment.CommentatorId != userId)
            throw new UnauthorizedAccessException($"Only the author can {action} this comment");
    }

    private async Task<CommentDto> GetRequiredCommentDtoAsync(Guid commentId)
    {
        var comment = await commentRepository.GetByIdAsync(commentId);
        if (comment == null)
            throw new InvalidOperationException("Comment not found after save");

        return ToCommentDto(comment);
    }

    private CommentDto ToCommentDto(Comment comment)
    {
        var dto = CommentDto.FromComment(comment);
        ApplyImageUrls(dto);
        return dto;
    }

    private List<CommentDto> ToCommentDtos(IEnumerable<Comment> comments)
    {
        return comments.Select(ToCommentDto).ToList();
    }

    private void ApplyImageUrls(CommentDto comment)
    {
        comment.CommentatorAvatarUrl = string.IsNullOrWhiteSpace(comment.CommentatorAvatarUrl)
            ? null
            : imageUrlProvider.GetImageUrl(comment.CommentatorAvatarUrl);

        foreach (var image in comment.Images)
        {
            image.Url = imageUrlProvider.GetImageUrl(image.FileName);
        }
    }
}

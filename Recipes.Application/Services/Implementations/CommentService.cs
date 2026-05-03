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

        if (createCommentDto.ParentCommentId.HasValue)
            await EnsureParentCommentBelongsToRecipeAsync(createCommentDto.ParentCommentId.Value, createCommentDto.RecipeId);

        var now = clock.UtcNow;
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            RecipeId = createCommentDto.RecipeId,
            CommentatorId = createCommentDto.CommentatorId,
            ParentCommentId = createCommentDto.ParentCommentId,
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
        var rootComments = pagedResult.Items
            .Where(comment => comment.ParentCommentId == null)
            .OrderByDescending(comment => comment.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var repliesByParentId = pagedResult.Items
            .Where(comment => comment.ParentCommentId.HasValue)
            .GroupBy(comment => comment.ParentCommentId!.Value)
            .ToDictionary(group => group.Key, group => group.OrderBy(comment => comment.CreatedAt).ToList());
        var dtos = rootComments.Select(comment => ToCommentDto(comment, repliesByParentId)).ToList();

        return new PagedResult<CommentDto>
        {
            Items = dtos,
            TotalCount = pagedResult.Items.Count(comment => comment.ParentCommentId == null),
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

        var comments = await commentRepository.GetByRecipeIdAsync(comment.RecipeId);
        var repliesByParentId = comments
            .Where(c => c.ParentCommentId.HasValue)
            .GroupBy(c => c.ParentCommentId!.Value)
            .ToDictionary(group => group.Key, group => group.ToList());

        foreach (var commentToDelete in GetCommentBranch(comment.Id, comments, repliesByParentId))
        {
            await commentImageService.DeleteAllImagesAsync(commentToDelete);
        }

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

    private async Task EnsureParentCommentBelongsToRecipeAsync(Guid parentCommentId, Guid recipeId)
    {
        var parentComment = await GetRequiredCommentAsync(parentCommentId);
        if (parentComment.RecipeId != recipeId)
            throw new ArgumentException($"Parent comment with id {parentCommentId} not found");
    }

    private static IEnumerable<Comment> GetCommentBranch(
        Guid commentId,
        IReadOnlyCollection<Comment> comments,
        IReadOnlyDictionary<Guid, List<Comment>> repliesByParentId)
    {
        var comment = comments.FirstOrDefault(c => c.Id == commentId);
        if (comment == null)
            yield break;

        yield return comment;

        if (!repliesByParentId.TryGetValue(commentId, out var replies))
            yield break;

        foreach (var reply in replies.SelectMany(reply => GetCommentBranch(reply.Id, comments, repliesByParentId)))
        {
            yield return reply;
        }
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

    private CommentDto ToCommentDto(
        Comment comment,
        IReadOnlyDictionary<Guid, List<Comment>>? repliesByParentId = null)
    {
        var dto = CommentDto.FromComment(comment);
        if (repliesByParentId != null && repliesByParentId.TryGetValue(comment.Id, out var replies))
        {
            dto.Replies = replies.Select(reply => ToCommentDto(reply, repliesByParentId)).ToList();
        }

        ApplyImageUrls(dto);
        return dto;
    }

    private List<CommentDto> ToCommentDtos(IEnumerable<Comment> comments)
    {
        return comments.Select(comment => ToCommentDto(comment)).ToList();
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

using AutoMapper;
using Recipes.Application.DTO.Comment;
using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class CommentService(
    IUnitOfWork unitOfWork,
    IImageStorageService imageStorageService,
    IMapper mapper,
    IClock clock) : ICommentService
{
    public async Task<CommentDto> CreateCommentAsync(CreateCommentDto createCommentDto)
    {
        var recipe = await unitOfWork.Recipes.GetByIdAsync(createCommentDto.RecipeId);
        if (recipe == null)
            throw new ArgumentException($"Recipe with id {createCommentDto.RecipeId} not found");

        var comment = mapper.Map<Comment>(createCommentDto);
        var now = clock.UtcNow;

        comment.CreatedAt = now;
        comment.UpdatedAt = now;

        await AddImagesToCommentAsync(comment, createCommentDto.Images);

        await unitOfWork.Comments.AddAsync(comment);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredCommentDtoAsync(comment.Id);
    }

    public async Task<PagedResult<CommentDto>> GetCommentsByRecipeIdAsync(Guid recipeId, int page = 1,
        int pageSize = 20, DateTime? from = null, DateTime? to = null)
    {
        var pagedResult = await unitOfWork.Comments.GetByRecipeIdPagedAsync(recipeId, page, pageSize, from, to);
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
        var comment = await unitOfWork.Comments.GetByIdAsync(updateCommentDto.Id);
        if (comment == null)
            throw new ArgumentException($"Comment with id {updateCommentDto.Id} not found");

        if (comment.CommentatorId != updateCommentDto.CommentatorId)
            throw new UnauthorizedAccessException("Only the author can update this comment");

        if (updateCommentDto.Value is not null)
            comment.Value = updateCommentDto.Value;

        await DeleteImagesFromCommentAsync(comment, updateCommentDto.ImageIdsToDelete);
        await AddImagesToCommentAsync(comment, updateCommentDto.Images);

        comment.UpdatedAt = clock.UtcNow;
        await unitOfWork.Comments.UpdateAsync(comment);
        await unitOfWork.SaveChangesAsync();

        return await GetRequiredCommentDtoAsync(updateCommentDto.Id);
    }

    public async Task DeleteCommentAsync(Guid commentId, Guid userId)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
            throw new ArgumentException($"Comment with id {commentId} not found");

        if (comment.CommentatorId != userId)
            throw new UnauthorizedAccessException("Only the author can delete this comment");

        foreach (var image in comment.Images)
        {
            await imageStorageService.DeleteImageAsync(image.FileName);
            await unitOfWork.Images.DeleteAsync(image);
        }

        await unitOfWork.Comments.DeleteAsync(comment);
        await unitOfWork.SaveChangesAsync();
    }

    private async Task AddImagesToCommentAsync(Comment comment, List<ImageUpload> images)
    {
        if (images.Count == 0)
            return;

        foreach (var imageUpload in images)
        {
            var fileName = await imageStorageService.UploadImageAsync(
                imageUpload.Stream,
                imageUpload.FileName,
                imageUpload.ContentType);

            var image = new Image
            {
                FileName = fileName,
                CreatedAt = clock.UtcNow
            };

            await unitOfWork.Images.AddAsync(image);
            comment.Images.Add(image);
        }
    }

    private async Task DeleteImagesFromCommentAsync(Comment comment, List<Guid> imageIdsToDelete)
    {
        if (imageIdsToDelete.Count == 0)
            return;

        var imagesToDelete = comment.Images
            .Where(i => imageIdsToDelete.Contains(i.Id))
            .ToList();

        foreach (var image in imagesToDelete)
        {
            await imageStorageService.DeleteImageAsync(image.FileName);
            await unitOfWork.Images.DeleteAsync(image);
            comment.Images.Remove(image);
        }
    }

    private async Task<CommentDto> GetRequiredCommentDtoAsync(Guid commentId)
    {
        var comment = await unitOfWork.Comments.GetByIdAsync(commentId);
        if (comment == null)
            throw new InvalidOperationException("Comment not found after save");

        return ToCommentDto(comment);
    }

    private CommentDto ToCommentDto(Comment comment)
    {
        var dto = CommentDto.FromComment(comment);
        dto.ApplyImageUrls(imageStorageService);
        return dto;
    }

    private List<CommentDto> ToCommentDtos(IEnumerable<Comment> comments)
    {
        return comments.Select(ToCommentDto).ToList();
    }
}

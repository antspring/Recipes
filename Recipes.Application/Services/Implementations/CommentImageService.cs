using Recipes.Application.DTO.Recipe;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class CommentImageService(
    IImageStorageService imageStorageService,
    IImageRepository imageRepository,
    IClock clock) : ICommentImageService
{
    public async Task AddImagesAsync(Comment comment, IReadOnlyCollection<ImageUpload> imageUploads)
    {
        if (imageUploads.Count == 0)
            return;

        foreach (var imageUpload in imageUploads)
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

            await imageRepository.AddAsync(image);
            comment.Images.Add(image);
        }
    }

    public async Task DeleteImagesAsync(Comment comment, IReadOnlyCollection<Guid> imageIdsToDelete)
    {
        if (imageIdsToDelete.Count == 0)
            return;

        var imagesToDelete = comment.Images
            .Where(image => imageIdsToDelete.Contains(image.Id))
            .ToList();

        await DeleteImagesAsync(comment, imagesToDelete);
    }

    public Task DeleteAllImagesAsync(Comment comment)
    {
        return DeleteImagesAsync(comment, comment.Images.ToList());
    }

    private async Task DeleteImagesAsync(Comment comment, IReadOnlyCollection<Image> imagesToDelete)
    {
        if (imagesToDelete.Count == 0)
            return;

        await imageStorageService.DeleteImagesAsync(imagesToDelete.Select(image => image.FileName));

        foreach (var image in imagesToDelete)
        {
            await imageRepository.DeleteAsync(image);
            comment.Images.Remove(image);
        }
    }
}
using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface ICommentImageService
{
    Task AddImagesAsync(Comment comment, IReadOnlyCollection<ImageUpload> imageUploads);
    Task DeleteImagesAsync(Comment comment, IReadOnlyCollection<Guid> imageIdsToDelete);
    Task DeleteAllImagesAsync(Comment comment);
}

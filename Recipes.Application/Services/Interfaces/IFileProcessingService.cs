using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.Services.Interfaces;

public interface IFileProcessingService
{
    Task<List<ImageUpload>> ProcessUploadedFilesAsync(IEnumerable<IUploadedFile> uploadedFiles);
    Task<ImageUpload> ProcessUploadedFileAsync(IUploadedFile uploadedFile);
}
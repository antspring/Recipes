using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class FileProcessingService : IFileProcessingService
{
    public async Task<List<ImageUpload>> ProcessUploadedFilesAsync(IEnumerable<IUploadedFile> uploadedFiles)
    {
        var imageUploads = new List<ImageUpload>();
        
        foreach (var uploadedFile in uploadedFiles)
        {
            await using var stream = uploadedFile.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            imageUploads.Add(new ImageUpload
            {
                Stream = memoryStream,
                FileName = uploadedFile.FileName,
                ContentType = uploadedFile.ContentType
            });
        }
        
        return imageUploads;
    }
}
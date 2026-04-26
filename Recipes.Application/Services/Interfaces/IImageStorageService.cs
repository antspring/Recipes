namespace Recipes.Application.Services.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
    Task DeleteImageAsync(string fileName);
    Task DeleteImagesAsync(IEnumerable<string> fileNames);
    Task<bool> ImageExistsAsync(string fileName);
    string GetImageUrl(string fileName);
}
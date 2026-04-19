namespace Recipes.Application.Services.Interfaces;

public interface IImageStorageService
{
    Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType);
    Task<List<string>> UploadImagesAsync(IEnumerable<(Stream Stream, string FileName, string ContentType)> files);
    Task DeleteImageAsync(string fileName);
    Task DeleteImagesAsync(IEnumerable<string> fileNames);
    Task<bool> ImageExistsAsync(string fileName);
    string GetImageUrl(string fileName);

    List<string> GetImageUrls(IEnumerable<string> fileNames)
        => fileNames.Select(GetImageUrl).ToList();
}
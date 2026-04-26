using Recipes.Application.DTO.Recipe;

namespace Recipes.API.Helpers;

public static class ImageUploadFactory
{
    public static async Task<ImageUpload> CreateAsync(IFormFile formFile)
    {
        await using var stream = formFile.OpenReadStream();
        var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        return new ImageUpload
        {
            Stream = memoryStream,
            FileName = formFile.FileName,
            ContentType = formFile.ContentType
        };
    }

    public static async Task<List<ImageUpload>> CreateManyAsync(IEnumerable<IFormFile> formFiles)
    {
        var uploads = new List<ImageUpload>();

        foreach (var formFile in formFiles)
        {
            uploads.Add(await CreateAsync(formFile));
        }

        return uploads;
    }
}

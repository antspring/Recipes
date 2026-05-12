using Recipes.Application.Services.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class ImageUrlMapper(IImageUrlProvider imageUrlProvider) : IImageUrlMapper
{
    public string? ToImageUrl(string? fileName)
    {
        return string.IsNullOrWhiteSpace(fileName)
            ? null
            : imageUrlProvider.GetImageUrl(fileName);
    }

    public void ApplyUrl<T>(T? item, Func<T, string?> getFileName, Action<T, string?> setUrl) where T : class
    {
        if (item == null)
            return;

        setUrl(item, ToImageUrl(getFileName(item)));
    }

    public void ApplyUrls<T>(IEnumerable<T> items, Func<T, string?> getFileName, Action<T, string?> setUrl)
        where T : class
    {
        foreach (var item in items)
        {
            ApplyUrl(item, getFileName, setUrl);
        }
    }
}

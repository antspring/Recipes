namespace Recipes.Application.Services.Interfaces;

public interface IImageUrlMapper
{
    string? ToImageUrl(string? fileName);
    void ApplyUrl<T>(T? item, Func<T, string?> getFileName, Action<T, string?> setUrl) where T : class;
    void ApplyUrls<T>(IEnumerable<T> items, Func<T, string?> getFileName, Action<T, string?> setUrl) where T : class;
}

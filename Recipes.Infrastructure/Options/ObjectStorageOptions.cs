using Recipes.Application.Options.Interfaces;

namespace Recipes.Infrastructure.Options;

public class ObjectStorageOptions : IObjectStorageOptions
{
    public string Endpoint { get; set; } = "https://storage.yandexcloud.net";
    public string Bucket { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = "ru-central1";
}

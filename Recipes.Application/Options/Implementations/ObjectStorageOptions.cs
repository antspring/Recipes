using Recipes.Application.Options.Interfaces;

namespace Recipes.Application.Options.Implementations;

public class ObjectStorageOptions : IObjectStorageOptions
{
    public string Endpoint { get; set; } = "https://storage.yandexcloud.net";
    public string Bucket { get; set; } = string.Empty;
    public string AccessKey { get; set; } = string.Empty;
    public string SecretKey { get; set; } = string.Empty;
    public string Region { get; set; } = "ru-central1";
}
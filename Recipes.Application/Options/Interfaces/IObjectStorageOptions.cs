namespace Recipes.Application.Options.Interfaces;

public interface IObjectStorageOptions
{
    string Endpoint { get; set; }
    string Bucket { get; set; }
    string AccessKey { get; set; }
    string SecretKey { get; set; }
    string Region { get; set; }
}
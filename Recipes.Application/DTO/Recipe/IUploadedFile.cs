namespace Recipes.Application.DTO.Recipe;

public interface IUploadedFile
{
    Stream OpenReadStream();
    string FileName { get; }
    string ContentType { get; }
}
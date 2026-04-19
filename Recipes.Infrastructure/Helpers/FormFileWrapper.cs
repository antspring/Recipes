using Microsoft.AspNetCore.Http;
using Recipes.Application.DTO.Recipe;

namespace Recipes.Infrastructure.Helpers;

public class FormFileWrapper : IUploadedFile
{
    private readonly IFormFile _formFile;

    public FormFileWrapper(IFormFile formFile)
    {
        _formFile = formFile ?? throw new ArgumentNullException(nameof(formFile));
    }

    public Stream OpenReadStream()
    {
        return _formFile.OpenReadStream();
    }

    public string FileName => _formFile.FileName;

    public string ContentType => _formFile.ContentType;
}
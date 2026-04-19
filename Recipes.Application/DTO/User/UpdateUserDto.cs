using Recipes.Application.DTO.Recipe;

namespace Recipes.Application.DTO.User;

public class UpdateUserDto
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public IUploadedFile? Avatar { get; set; }
}
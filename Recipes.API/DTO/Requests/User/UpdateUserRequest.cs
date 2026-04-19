using System.ComponentModel.DataAnnotations;
using Recipes.API.DTO.Requests.Attributes;
using Recipes.Application.DTO.User;
using Recipes.Infrastructure.Helpers;

namespace Recipes.API.DTO.Requests.User;

public class UpdateUserRequest
{
    [StringLength(50, MinimumLength = 3)] public string? UserName { get; set; }

    [EmailAddress] public string? Email { get; set; }

    [StringLength(100, MinimumLength = 1)] public string? Name { get; set; }

    [StringLength(500)] public string? Description { get; set; }

    [AvatarFile(ErrorMessage = "Invalid avatar file")]
    public IFormFile? Avatar { get; set; }

    public UpdateUserDto ToUpdateUserDto()
    {
        return new UpdateUserDto
        {
            UserName = UserName,
            Email = Email,
            Name = Name,
            Description = Description,
            Avatar = Avatar != null ? new FormFileWrapper(Avatar) : null
        };
    }
}
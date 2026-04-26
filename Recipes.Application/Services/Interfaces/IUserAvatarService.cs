using Recipes.Application.DTO.Recipe;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Interfaces;

public interface IUserAvatarService
{
    Task<string?> UploadAvatarAsync(ImageUpload? avatar);
    Task DeleteAvatarAsync(User user);
}

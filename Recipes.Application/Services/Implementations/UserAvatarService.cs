using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserAvatarService(
    IImageStorageService imageStorageService) : IUserAvatarService
{
    public async Task<string?> UploadAvatarAsync(ImageUpload? avatar)
    {
        if (avatar == null)
            return null;

        return await imageStorageService.UploadImageAsync(
            avatar.Stream,
            avatar.FileName,
            avatar.ContentType);
    }

    public async Task DeleteAvatarAsync(User user)
    {
        if (string.IsNullOrEmpty(user.AvatarUrl))
            return;

        await imageStorageService.DeleteImageAsync(user.AvatarUrl);
    }
}

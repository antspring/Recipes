using Recipes.Application.DTO.Recipe;
using Recipes.Application.Services.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserAvatarService(
    IImageStorageService imageStorageService,
    IImageUrlProvider imageUrlProvider) : IUserAvatarService
{
    public async Task<string?> UploadAvatarAsync(ImageUpload? avatar)
    {
        if (avatar == null)
            return null;

        var fileName = await imageStorageService.UploadImageAsync(
            avatar.Stream,
            avatar.FileName,
            avatar.ContentType);

        return imageUrlProvider.GetImageUrl(fileName);
    }

    public async Task DeleteAvatarAsync(User user)
    {
        if (string.IsNullOrEmpty(user.AvatarUrl))
            return;

        var fileName = ExtractFileName(user.AvatarUrl);
        await imageStorageService.DeleteImageAsync(fileName);
    }

    private static string ExtractFileName(string avatarUrl)
    {
        var uri = new Uri(avatarUrl);
        return uri.Segments.Last();
    }
}

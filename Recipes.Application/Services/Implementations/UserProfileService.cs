using AutoMapper;
using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserProfileService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IUserAccessService userAccessService,
    IUserAvatarService userAvatarService,
    IClock clock) : IUserProfileService
{
    public async Task<User> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
    {
        var user = await userAccessService.GetRequiredUserAsync(userId);
        await userAvatarService.DeleteAvatarAsync(user);
        user.AvatarUrl = await userAvatarService.UploadAvatarAsync(updateUserDto.Avatar);

        mapper.Map(updateUserDto, user);
        user.UpdatedAt = clock.UtcNow;

        await userRepository.UpdateAsync(user);
        await unitOfWork.SaveChangesAsync();

        return user;
    }

    public async Task<User> DeleteAvatarAsync(Guid userId)
    {
        var user = await userAccessService.GetRequiredUserAsync(userId);
        await userAvatarService.DeleteAvatarAsync(user);

        user.AvatarUrl = null;
        user.UpdatedAt = clock.UtcNow;

        await userRepository.UpdateAsync(user);
        await unitOfWork.SaveChangesAsync();

        return user;
    }
}

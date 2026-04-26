using AutoMapper;
using Recipes.Application.DTO.User;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserRegistrationService(
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPasswordHasher passwordHasher,
    IUserAvatarService userAvatarService) : IUserRegistrationService
{
    public async Task<User> RegisterAsync(CreateUserDto createUserDto)
    {
        var user = mapper.Map<User>(createUserDto);
        user.Password = passwordHasher.Hash(createUserDto.Password);
        user.AvatarUrl = await userAvatarService.UploadAvatarAsync(createUserDto.Avatar);

        await unitOfWork.Users.CreateAsync(user);
        await unitOfWork.SaveChangesAsync();

        return user;
    }
}

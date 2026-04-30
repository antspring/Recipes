using AutoMapper;
using Recipes.Application.DTO.User;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;
using Recipes.Domain.Models;

namespace Recipes.Application.Services.Implementations;

public class UserRegistrationService(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IMapper mapper,
    IPasswordHasher passwordHasher,
    IUserAvatarService userAvatarService,
    IClock clock,
    IUserUniquenessService userUniquenessService,
    IEmailVerificationService emailVerificationService,
    IEmailVerificationCodeRepository emailVerificationCodeRepository) : IUserRegistrationService
{
    public async Task<User> RegisterAsync(CreateUserDto createUserDto)
    {
        await userUniquenessService.EnsureUserNameOrEmailAvailableAsync(createUserDto.UserName, createUserDto.Email);
        await emailVerificationService.VerifyRegistrationCodeAsync(
            createUserDto.Email,
            createUserDto.EmailVerificationCode);

        var user = mapper.Map<User>(createUserDto);
        var now = clock.UtcNow;

        user.Password = passwordHasher.Hash(createUserDto.Password);
        user.AvatarUrl = await userAvatarService.UploadAvatarAsync(createUserDto.Avatar);
        user.CreatedAt = now;
        user.UpdatedAt = now;

        await userRepository.CreateAsync(user);
        await emailVerificationCodeRepository.DeleteByEmailAsync(createUserDto.Email);
        await unitOfWork.SaveChangesAsync();

        return user;
    }
}

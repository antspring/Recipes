using System.Security.Cryptography;
using Recipes.Application.Auth;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Application.Services.Interfaces;
using Recipes.Application.UnitOfWork.Interfaces;

namespace Recipes.Application.Services.Implementations;

public class EmailVerificationService(
    IEmailVerificationCodeRepository emailVerificationCodeRepository,
    IUserUniquenessService userUniquenessService,
    IEmailSender emailSender,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork,
    IClock clock) : IEmailVerificationService
{
    private const int CodeExpirationMinutes = 10;

    public async Task SendRegistrationCodeAsync(string email)
    {
        await userUniquenessService.EnsureUserNameOrEmailAvailableAsync(null, email);

        var now = clock.UtcNow;
        var code = GenerateCode();

        await emailVerificationCodeRepository.DeleteByEmailAsync(email);
        await emailVerificationCodeRepository.CreateAsync(new GeneratedEmailVerificationCode(
            email,
            passwordHasher.Hash(code),
            now.AddMinutes(CodeExpirationMinutes),
            now));
        await unitOfWork.SaveChangesAsync();

        await emailSender.SendAsync(
            email,
            "Registration confirmation code",
            $"Your registration confirmation code: {code}");
    }

    public async Task VerifyRegistrationCodeAsync(string email, string code)
    {
        var now = clock.UtcNow;
        var storedCode = await emailVerificationCodeRepository.GetLatestActiveAsync(email, now);

        if (storedCode == null || !passwordHasher.Verify(code, storedCode.CodeHash))
        {
            throw new ArgumentException("Invalid email verification code");
        }
    }

    private static string GenerateCode()
    {
        return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
    }
}

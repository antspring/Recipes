using Recipes.Application.Auth;

namespace Recipes.Application.Repositories.Interfaces;

public interface IEmailVerificationCodeRepository
{
    Task CreateAsync(GeneratedEmailVerificationCode code);
    Task<StoredEmailVerificationCode?> GetLatestActiveAsync(string email, DateTime now);
    Task DeleteByEmailAsync(string email);
}

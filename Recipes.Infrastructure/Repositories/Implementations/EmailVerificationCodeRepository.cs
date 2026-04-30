using Microsoft.EntityFrameworkCore;
using Recipes.Application.Auth;
using Recipes.Application.Repositories.Interfaces;
using Recipes.Infrastructure.Models;

namespace Recipes.Infrastructure.Repositories.Implementations;

public class EmailVerificationCodeRepository(BaseDbContext dbContext) : IEmailVerificationCodeRepository
{
    public Task CreateAsync(GeneratedEmailVerificationCode code)
    {
        return dbContext.EmailVerificationCodes.AddAsync(new EmailVerificationCode
        {
            Email = code.Email,
            CodeHash = code.CodeHash,
            ExpiresAt = code.ExpiresAt,
            CreatedAt = code.CreatedAt
        }).AsTask();
    }

    public async Task<StoredEmailVerificationCode?> GetLatestActiveAsync(string email, DateTime now)
    {
        var entity = await dbContext.EmailVerificationCodes
            .Where(code =>
                code.Email == email &&
                code.ExpiresAt > now)
            .OrderByDescending(code => code.CreatedAt)
            .FirstOrDefaultAsync();

        return entity == null
            ? null
            : new StoredEmailVerificationCode(
                entity.Id,
                entity.Email,
                entity.CodeHash,
                entity.ExpiresAt,
                entity.CreatedAt);
    }

    public Task DeleteByEmailAsync(string email)
    {
        return dbContext.EmailVerificationCodes
            .Where(code => code.Email == email)
            .ExecuteDeleteAsync();
    }
}

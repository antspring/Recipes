namespace Recipes.Application.Services.Interfaces;

public interface IEmailVerificationService
{
    Task SendRegistrationCodeAsync(string email);
    Task VerifyRegistrationCodeAsync(string email, string code);
}

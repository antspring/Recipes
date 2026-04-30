namespace Recipes.Application.Services.Interfaces;

public interface IEmailSender
{
    Task SendAsync(string email, string subject, string body);
}

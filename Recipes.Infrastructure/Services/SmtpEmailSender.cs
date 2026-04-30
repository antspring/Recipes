using System.Net;
using System.Net.Mail;
using Recipes.Application.Options.Interfaces;
using Recipes.Application.Services.Interfaces;

namespace Recipes.Infrastructure.Services;

public class SmtpEmailSender(ISmtpOptions smtpOptions) : IEmailSender
{
    public async Task SendAsync(string email, string subject, string body)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(smtpOptions.FromEmail, smtpOptions.FromName),
            Subject = subject,
            Body = body
        };
        message.To.Add(email);

        using var client = new SmtpClient(smtpOptions.Host, smtpOptions.Port)
        {
            EnableSsl = smtpOptions.UseSsl
        };

        if (!string.IsNullOrWhiteSpace(smtpOptions.UserName))
        {
            client.Credentials = new NetworkCredential(smtpOptions.UserName, smtpOptions.Password);
        }

        await client.SendMailAsync(message);
    }
}

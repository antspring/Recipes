namespace Recipes.Application.Options.Interfaces;

public interface ISmtpOptions
{
    string Host { get; }
    int Port { get; }
    string? UserName { get; }
    string? Password { get; }
    string FromEmail { get; }
    string FromName { get; }
    bool UseSsl { get; }
}
